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
    public class TiffStream : Stream, IDisposable
    {
        /// <summary>
        /// Initializes a new TiffStream.
        /// </summary>
        /// <param name="forceBigEndian">Optionally, force TiffStream to use Big-Endian mode for writing a new TIFF.</param>
        public TiffStream(bool forceBigEndian = false)
        {
            _Stream = new MemoryStream();
            IsBigEndian = forceBigEndian;
            _IsWritable = true;
        }

        /// <summary>
        /// Initializes a new read-only TiffStream from the given file.
        /// </summary>
        /// <param name="fileName"></param>
        public TiffStream(string fileName)
        {
            _Stream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read);
            _IsWritable = false;
        }

        /// <summary>
        /// Initializes a new TiffStream from the given stream.
        /// </summary>
        /// <param name="stream"></param>
        public TiffStream(Stream stream)
        {
            _Stream = stream;
            _IsWritable = _Stream.CanWrite;
        }

        public bool IsBigEndian { get; private set; } = false;
        bool _IsWritable = false;
        Stream _Stream;

        #region image I/O

        /// <summary>
        /// Read all image strips from the given IFD
        /// </summary>
        /// <param name="ifdOffset"></param>
        /// <returns></returns>
        public Strip[] ReadStrips(uint ifdOffset)
        {
            var tags = ReadIFD(ifdOffset).tags;
            var offsets = tags.Where(t => t.ID == (ushort)BaselineTags.StripOffsets).FirstOrDefault();
            var byteLengths = tags.Where(t => t.ID == (ushort)BaselineTags.StripByteCounts).FirstOrDefault();

            if (offsets == null) { throw new IOException("IFD is missing StripOffsets tag."); }
            if (byteLengths == null) { throw new IOException("IFD is missing StripByteCounts tag."); }

            // Offsets and lengths can be either ushort or uint, which complicates things with our Tag<T> generics
            offsets = ParseTag(offsets);
            byteLengths = ParseTag(byteLengths);

            var strips = new Strip[offsets.Length];

            for (ushort i = 0; i < offsets.Length; i++)
            {
                var offset = offsets.GetValue<uint>(i);
                var len = byteLengths.GetValue<uint>(i);
                var bytes = new byte[len];
                _Stream.Seek(offset, SeekOrigin.Begin);
                _Stream.Read(bytes, 0, (int)len);

                strips[i] = new Strip
                {
                    ImageData = bytes,
                    StripNumber = i,
                    StripOffset = offset
                };
            }

            return strips;
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
        /// Writes the actual data payload for a tag and updates the pointer value (if necessary)
        /// </summary>
        /// <param name="tag"></param>
        public void FinalizeTag<T>(Tag<T> tag) where T : struct
        {
            if (tag.Length != tag.Values.Length) { throw new DataMisalignedException("Tag Length property and the length of the Values array do not match."); }

            var atomicLength = tag.DataType.AtomicLength();
            _Stream.Seek(tag.Offset + 4, SeekOrigin.Begin);
            WriteDWord(tag.Length);
            if (atomicLength * tag.Length > 4)
            {
                var pointer = (uint)_Stream.Seek(0, SeekOrigin.End);
                foreach (var value in tag.Values)
                {
                    WriteValue<T>(value);
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
                    WriteValue<T>(value);
                }
                //reread raw value
                _Stream.Seek(tag.Offset + 8, SeekOrigin.Begin);
                _Stream.Read(tag.RawValue, 0, 4);
            }

        }

        #endregion

        public (UInt32 nextIfd, Tag[] tags) ReadIFD(UInt32 offset)
        {
            _Stream.Seek(offset, SeekOrigin.Begin);

            var tagCount = ReadWord();
            var tags = new Tag[tagCount];

            for (int i = 0; i < tagCount; i++)
            {
                tags[i] = ReadTag();
            }

            var nextIfd = ReadWord();
            return (nextIfd, tags);
        }

        public (UInt32 nextIfd, Tag[] tags) ParseIFD(UInt32 offset)
        {
            var result = ReadIFD(offset);
            for (int i = 0; i < result.tags.Length; i++)
            {
                result.tags[i] = ParseTag(result.tags[i]);
            }
            return result;
        }

        public Tag ReadTag()
        {
            var tag = new Tag();
            tag.Offset = (uint)_Stream.Position;
            tag.ID = ReadWord();
            tag.DataType = (TagDataType)ReadWord();
            tag.Length = ReadDWord();
            _Stream.Read(tag.RawValue, 0, 4);

            return tag;
        }

        /// <summary>
        /// Attempts to parse the given Tag into an appropriate Tag<T> and returns a boxed Tag value
        /// </summary>
        /// <param name="tag"></param>
        /// <returns></returns>
        Tag ParseTag(Tag tag)
        {
            switch (tag.DataType)
            {
                case TagDataType.ASCII:
                    return ParseTag<char>(tag);
                case TagDataType.Byte:
                case TagDataType.SByte:
                case TagDataType.Undefined:
                    return ParseTag<byte>(tag);
                case TagDataType.Double:
                    return ParseTag<double>(tag);
                case TagDataType.Float:
                    return ParseTag<float>(tag);
                case TagDataType.Long:
                    return ParseTag<uint>(tag);
                case TagDataType.Rational:
                    return ParseTag<Rational>(tag);
                case TagDataType.SRational:
                    return ParseTag<SRational>(tag);
                case TagDataType.Short:
                    return ParseTag<short>(tag);
                case TagDataType.SLong:
                    return ParseTag<int>(tag);
                case TagDataType.SShort:
                    return ParseTag<short>(tag);
                default:
                    return tag;
            }
        }

        Tag<T> ParseTag<T>(Tag tag) where T : struct
        {
            var startingOffet = _Stream.Position;

            var parsedTag = new Tag<T>()
            {
                ID = tag.ID,
                Length = tag.Length,
                Offset = tag.Offset,
                RawValue = tag.RawValue,
                DataType = tag.DataType,
                Values = new T[tag.Length]
            };

            // seek data portion of the raw tag
            _Stream.Seek(tag.Offset + 8, SeekOrigin.Begin);

            // if tag data is too long, read a pointer and seek data block
            var atomicLen = tag.DataType.AtomicLength();
            if (atomicLen * tag.Length > 4)
            {
                var pointer = ReadDWord();
                _Stream.Seek(pointer, SeekOrigin.Begin);
            }

            // read actual values into tag.Values
            for (int i = 0; i < tag.Length; i++)
            {
                parsedTag.Values[i] = ReadValue<T>();
            }

            _Stream.Seek(startingOffet, SeekOrigin.Begin);
            return parsedTag;
        }

        #endregion

        #region basic I/O

        /// <summary>
        /// Reads and validates the TIFF header from the underlying stream, and returns the offset of the first IFD (IFD0).
        /// </summary>
        /// <returns></returns>
        public UInt32 ReadHeader()
        {
            _Stream.Seek(0, SeekOrigin.Begin);
            var magic = ReadWord(); // header bytes are paired so endianness does not matter

            if (magic == 0x4d4d) { IsBigEndian = true; }
            else if (magic == 0x4949) { IsBigEndian = false; }
            else
            {
                throw new FormatException("Stream does not contain a valid TIFF header.");
            }

            var version = ReadWord();
            if (version != 42)
            {
                throw new FormatException($"Invalid TIFF version: {version}");
            }

            var ifd0 = ReadDWord();
            if (ifd0 >= 8 && ifd0 < _Stream.Length)
            {
                return ifd0;
            }
            else
            {
                throw new IOException($"Value for IFD0 {ifd0} is not valid.");
            }
        }

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

        public byte[] ToArray()
        {
            _Stream.Seek(0, SeekOrigin.Begin);
            var bytes = new byte[_Stream.Length];
            _Stream.Read(bytes, 0, (int)_Stream.Length);
            return bytes;
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

        T ReadValue<T>() where T : struct
        {
            object obj = null;
            if (typeof(T) == typeof(char)) { obj = (char)_Stream.ReadByte(); }
            else if (typeof(T) == typeof(byte)) { obj = (byte)_Stream.ReadByte(); }
            else if (typeof(T) == typeof(double)) { obj = ReadDouble(); }
            else if (typeof(T) == typeof(float)) { obj = ReadFloat(); }
            else if (typeof(T) == typeof(uint)) { obj = ReadDWord(); }
            else if (typeof(T) == typeof(int)) { obj = ReadLong(); }
            else if (typeof(T) == typeof(short)) { obj = ReadShort(); }
            else if (typeof(T) == typeof(ushort)) { obj = ReadWord(); }
            else if (typeof(T) == typeof(Rational)) { obj = ReadRational(); }
            else if (typeof(T) == typeof(SRational)) { obj = ReadSRational(); }

            if (obj != null)
            {
                return (T)obj;
            }
            else
            {
                throw new ArgumentException($"Can't read value of type {typeof(T).ToString()}");
            }
        }

        /// <summary>
        /// Advance the stream (if necessary) to the next word boundary.
        /// </summary>
        public void SeekWord()
        {
            if (_Stream.Position % 2 != 0)
            {
                _Stream.Seek(1, SeekOrigin.Current);
            }
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

        public UInt16 ReadWord() { return BitConverter.ToUInt16(ReadEndianBytes(2), 0); }
        public UInt32 ReadDWord() { return BitConverter.ToUInt32(ReadEndianBytes(4), 0); }
        public Int16 ReadShort() { return BitConverter.ToInt16(ReadEndianBytes(2), 0); }
        public Int32 ReadLong() { return BitConverter.ToInt32(ReadEndianBytes(4), 0); }
        public float ReadFloat() { return BitConverter.ToSingle(ReadEndianBytes(4), 0); }
        public double ReadDouble() { return BitConverter.ToDouble(ReadEndianBytes(4), 0); }
        public Rational ReadRational() { return new Rational(ReadDWord(), ReadDWord()); }
        public SRational ReadSRational() { return new SRational(ReadLong(), ReadLong()); }

        byte[] ReadEndianBytes(int count)
        {
            var bytes = new byte[count];
            _Stream.Read(bytes, 0, count);
            CheckEndian(bytes);
            return bytes;
        }

        void WriteEndianBytes(byte[] bytes)
        {
            CheckEndian(bytes);
            _Stream.Write(bytes, 0, bytes.Length);
        }

        /// <summary>
        /// If the active endianness conflicts with our architecture, reverse the entire byte array.
        /// </summary>
        /// <param name="bytes"></param>
        void CheckEndian(byte[] bytes)
        {
            if (IsBigEndian == BitConverter.IsLittleEndian)
            {
                Array.Reverse(bytes);
            }
        }

        #endregion

        #region abstract implementations from Stream

        public override bool CanRead => _Stream.CanRead;

        public override bool CanSeek => _Stream.CanSeek;

        public override bool CanWrite => _IsWritable && _Stream.CanWrite;

        public override long Length => _Stream.Length;

        public override long Position { get => _Stream.Position; set => _Stream.Position = value; }

        public override void Flush()
        {
            _Stream.Flush();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            return _Stream.Read(buffer, offset, count);
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            return _Stream.Seek(offset, origin);
        }

        public override void SetLength(long value)
        {
            if (CanWrite) { _Stream.SetLength(value); }
            else { throw new IOException("Stream is in a read-only state."); }
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            if (CanWrite) { _Stream.Write(buffer, offset, count); }
            else { throw new IOException("Stream is not writable."); }
        }

        #endregion
    }
}
