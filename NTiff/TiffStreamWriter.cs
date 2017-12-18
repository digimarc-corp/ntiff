using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;
using NTiff.Tags;
using NTiff.Types;

namespace NTiff
{
    /// <summary>
    /// Supports basic read/write functionality for a TIFF stream.
    /// </summary>
    public class TiffStreamWriter : TiffStreamReader, IDisposable
    {
        /// <summary>
        /// Initializes a new TiffStream.
        /// </summary>
        /// <param name="forceBigEndian">Optionally, force TiffStream to use Big-Endian mode for writing a new TIFF.</param>
        public TiffStreamWriter(bool forceBigEndian = false) : base(forceBigEndian) { }


        /// <summary>
        /// Initializes a new TiffStream from the given stream.
        /// </summary>
        /// <param name="stream"></param>
        public TiffStreamWriter(Stream stream) : base(stream) { }

        #region image I/O

        /// <summary>
        /// Write and finalize the given tags to a new IFD at the end of the stream.
        /// </summary>
        /// <param name="tags"></param>
        /// <returns></returns>
        internal uint WriteIFD(IEnumerable<Tag> tags)
        {
            var offset = (uint)SeekWord();
            WriteWord((ushort)tags.Count());
            // write all placeholder tags first
            foreach (var tag in tags.OrderBy(t => t.ID)) { WriteTagPlaceholder(tag); }
            // write placeholder value for next IFD pointer
            WriteDWord(0);
            // then finalize each tag, writing long values after the IFD
            foreach (var tag in tags.OrderBy(t => t.ID)) { FinalizeTag(tag); }
            _Stream.Seek(0, SeekOrigin.End);
            return offset;
        }

        /// <summary>
        /// Write image strip data for the given IFD, and update strip tags
        /// </summary>
        /// <param name="ifdOffset"></param>
        /// <param name="strips"></param>
        internal void WriteStrips(uint ifdOffset, Strip[] strips)
        {
            var ifd = ReadIFD(ifdOffset);
            var stripCount = strips.Length;

            var lengthTag = ifd.tags.Where(t => t.ID == (ushort)BaselineTags.StripByteCounts).FirstOrDefault();
            var offsetTag = ifd.tags.Where(t => t.ID == (ushort)BaselineTags.StripOffsets).FirstOrDefault();
            if (offsetTag.Length != stripCount || lengthTag.Length != stripCount)
            {
                throw new ArgumentException("The given IFD does not contain valid tags for StripByteCounts or StripOffsets. Data may be corrupt.");
            }
            lengthTag = ParseTag(lengthTag);
            offsetTag = ParseTag(offsetTag);

            for (int i = 0; i < stripCount; i++)
            {
                var strip = strips[i];

                var stripOffset = (uint)SeekWord(0, SeekOrigin.End);
                Write(strip.ImageData, 0, strip.ImageData.Length);

                // update strip tag values
                if (lengthTag is Tag<ushort>) { ((Tag<ushort>)lengthTag).Values[i] = (ushort)strip.ImageData.Length; }
                else if (lengthTag is Tag<uint>) { ((Tag<uint>)lengthTag).Values[i] = (uint)strip.ImageData.Length; }
                if (offsetTag is Tag<ushort>) { ((Tag<ushort>)offsetTag).Values[i] = (ushort)stripOffset; }
                else if (offsetTag is Tag<uint>) { ((Tag<uint>)offsetTag).Values[i] = (uint)stripOffset; }
            }

            UpdateTags(offsetTag, lengthTag);
        }

        /// <summary>
        /// Update a finalized tag. Method will throw if the number or type of items has changed.
        /// </summary>
        /// <param name="tags"></param>
        internal void UpdateTags(params Tag[] tags)
        {
            foreach (var tag in tags)
            {
                Seek(tag.Offset, SeekOrigin.Begin);
                var readTag = ReadTag();
                var parsedTag = ParseTag(readTag);
                if (tag.Length != parsedTag.Length || tag.DataType != parsedTag.DataType)
                {
                    throw new ArgumentException("Tag length and/or type has changed, and is not updateable.");
                }
                else
                {
                    FinalizeTag(tag, rewrite: true);
                }
            }
        }

        #region tag I/O

        /// <summary>
        /// Writes an initial tag placeholder, and updates the offset property if the tag object.
        /// </summary>
        /// <param name="tag"></param>
        public void WriteTagPlaceholder(Tag tag)
        {
            var offset = _Stream.Position;
            WriteWord(tag.ID);
            WriteWord((ushort)tag.DataType);
            WriteDWord(tag.Length);
            WriteDWord(0);

            tag.Offset = (uint)offset;
        }

        /// <summary>
        /// Writes the actual data payload for a tag and updates the placeholder value (if necessary)
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="rewrite">Signals that a previously finalized tag should be overwritten without allocating more space.</param>
        public void FinalizeTag(Tag tag, bool rewrite = false)
        {
            if (tag is Tag<char>) { FinalizeTag((Tag<char>)tag, rewrite); }
            else if (tag is Tag<byte>) { FinalizeTag((Tag<byte>)tag, rewrite); }
            else if (tag is Tag<ushort>) { FinalizeTag((Tag<ushort>)tag, rewrite); }
            else if (tag is Tag<short>) { FinalizeTag((Tag<short>)tag, rewrite); }
            else if (tag is Tag<uint>) { FinalizeTag((Tag<uint>)tag, rewrite); }
            else if (tag is Tag<int>) { FinalizeTag((Tag<int>)tag, rewrite); }
            else if (tag is Tag<float>) { FinalizeTag((Tag<float>)tag, rewrite); }
            else if (tag is Tag<double>) { FinalizeTag((Tag<double>)tag, rewrite); }
            else if (tag is Tag<Rational>) { FinalizeTag((Tag<Rational>)tag, rewrite); }
            else if (tag is Tag<SRational>) { FinalizeTag((Tag<SRational>)tag, rewrite); }
            else
            {
                throw new ArgumentException($"Don't know how to finalize a tag of type {tag.GetType()}");
            }
        }

        /// <summary>
        /// Writes the actual data payload for a tag and updates the pointer value (if necessary)
        /// </summary>
        /// <param name="tag"></param>
        public void FinalizeTag<T>(Tag<T> tag, bool rewrite) where T : struct
        {
            if (tag.Length != tag.Values.Length) { throw new DataMisalignedException("Tag Length property and the length of the Values array do not match."); }

            var atomicLength = tag.DataType.AtomicLength();
            _Stream.Seek(tag.Offset + 4, SeekOrigin.Begin);
            WriteDWord(tag.Length);
            if (atomicLength * tag.Length > 4)
            {
                uint pointer = rewrite ? ReadDWord() : (uint)SeekWord(0, SeekOrigin.End);
                Seek(pointer, SeekOrigin.Begin);
                foreach (var value in tag.Values)
                {
                    WriteValue(value);
                }
                _Stream.Seek(tag.Offset + 8, SeekOrigin.Begin);
                var bytes = BitConverter.GetBytes(pointer);
                CheckEndian(bytes);
                _Stream.Write(bytes, 0, 4);
                tag.RawValue = bytes;
            }
            else
            {
                // zero out the data field in case we don't have enough values to fill it.
                WriteDWord(0);
                _Stream.Seek(-4, SeekOrigin.Current);
                foreach (var value in tag.Values)
                {
                    // arrays written to the Tag data field are always left-justified, even in little-endian systems
                    WriteValue(value);
                }
                //reread raw value
                _Stream.Seek(tag.Offset + 8, SeekOrigin.Begin);
                _Stream.Read(tag.RawValue, 0, 4);
            }

        }

        #endregion




        #endregion

        #region basic I/O


        /// <summary>
        /// Writes a standard TIFF header at the beginning of the stream.
        /// </summary>
        public void WriteHeader()
        {
            _Stream.Seek(0, SeekOrigin.Begin);
            if (IsBigEndian) { _Stream.Write(new byte[] { 0x4d, 0x4d }, 0, 2); }
            else { _Stream.Write(new byte[] { 0x49, 0x49 }, 0, 2); }

            WriteWord(42);
        }

        void WriteValue<T>(T value) where T : struct
        {
            if (typeof(T) == typeof(byte)) { WriteByte((byte)(object)value); }
            else if (typeof(T) == typeof(char)) { WriteByte((byte)(char)(object)value); }
            else if (typeof(T) == typeof(double)) { WriteDouble((double)(object)value); }
            else if (typeof(T) == typeof(float)) { WriteFloat((float)(object)value); }
            else if (typeof(T) == typeof(uint)) { WriteDWord((uint)(object)value); }
            else if (typeof(T) == typeof(int)) { WriteLong((int)(object)value); }
            else if (typeof(T) == typeof(short)) { WriteShort((short)(object)value); }
            else if (typeof(T) == typeof(ushort)) { WriteWord((ushort)(object)value); }
            else if (typeof(T) == typeof(Rational)) { WriteRational((Rational)(object)value); }
            else if (typeof(T) == typeof(SRational)) { WriteSRational((SRational)(object)value); }
            else
            {
                throw new ArgumentException($"Can't write value of type {typeof(T).ToString()}");
            }
        }

        private void WriteSRational(SRational value)
        {
            WriteLong(value.Numerator);
            WriteLong(value.Denominator);
        }

        private void WriteRational(Rational value)
        {
            WriteDWord(value.Numerator);
            WriteDWord(value.Denominator);
        }

        private void WriteShort(short value)
        {
            var bytes = BitConverter.GetBytes(value);
            WriteEndianBytes(bytes);
        }

        private void WriteLong(int value)
        {
            var bytes = BitConverter.GetBytes(value);
            WriteEndianBytes(bytes);
        }

        private void WriteFloat(float value)
        {
            var bytes = BitConverter.GetBytes(value);
            WriteEndianBytes(bytes);
        }

        private void WriteDouble(double value)
        {
            var bytes = BitConverter.GetBytes(value);
            WriteEndianBytes(bytes);
        }

        /// <summary>
        /// Advance the stream (if necessary) to the next word boundary.
        /// </summary>
        public long SeekWord()
        {
            // if we're on an odd byte, advance one position
            if (_Stream.Position % 2 != 0)
            {
                // if we're at the end, pad with a 0
                if (_Stream.Position == _Stream.Length - 1)
                {
                    WriteByte(0x00);
                }
                else
                {
                    _Stream.Seek(1, SeekOrigin.Current);
                }
            }
            return _Stream.Position;
        }

        /// <summary>
        /// Seeks to the given offset, and then +1 if the offset is odd.
        /// </summary>
        /// <param name="offset"></param>
        /// <param name="origin"></param>
        /// <returns></returns>
        long SeekWord(uint offset, SeekOrigin origin)
        {
            Seek(offset, origin);
            return SeekWord();
        }

        public void WriteWord(ushort value)
        {
            var bytes = BitConverter.GetBytes(value);
            WriteEndianBytes(bytes);
        }

        public void WriteDWord(uint value)
        {
            var bytes = BitConverter.GetBytes(value);
            WriteEndianBytes(bytes);
        }

        void WriteEndianBytes(byte[] bytes)
        {
            CheckEndian(bytes);
            _Stream.Write(bytes, 0, bytes.Length);
        }


        #endregion

        #region abstract implementations from Stream



        #endregion
    }
}
