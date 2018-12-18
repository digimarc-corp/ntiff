/*
   Copyright 2018 Digimarc, Inc

   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

       http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.

   SPDX-License-Identifier: Apache-2.0
*/

using Digimarc.NTiff.Tags;
using Digimarc.NTiff.Types;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Digimarc.NTiff
{
    public class TiffStreamReader : TiffStreamBase
    {
        /// <summary>
        /// Initializes a new TiffStream.
        /// </summary>
        /// <param name="forceBigEndian">Optionally, force TiffStream to use Big-Endian mode for writing a new TIFF.</param>
        public TiffStreamReader(bool forceBigEndian = false)
        {
            _Stream = new MemoryStream();
            IsBigEndian = forceBigEndian;
            _IsWritable = true;
        }

        /// <summary>
        /// Initializes a new TiffStream from the given stream.
        /// </summary>
        /// <param name="stream"></param>
        public TiffStreamReader(Stream stream)
        {
            _Stream = stream;
            _IsWritable = _Stream.CanWrite;
        }

        public (Image image, uint nextOffset) ReadImage(uint offset)
        {
            var rawIFD = ParseIFD(offset);

            var image = new Image();

            // get image tags
            image.Tags = rawIFD.tags.ToList();

            // get image data
            image.Strips = ReadStrips(offset).OrderBy(s => s.StripNumber).ToList();

            // get subimages, if any
            var subIfds = image.Tags.FirstOrDefault(t => t.ID == (uint)ExtensionTags.SubIFDs);
            if (subIfds != null)
            {
                foreach (var subOffset in ((Tag<uint>)subIfds).Values)
                {
                    image.SubImages.Add(ReadImage(subOffset).image);
                }
            }

            // load Exif, if present
            var exifOffset = rawIFD.tags.Where(t => t.ID == (ushort)PrivateTags.ExifIFD).FirstOrDefault();
            if (exifOffset != null)
            {
                image.Exif = ParseIFD(exifOffset.GetValue<uint>(0)).tags.ToList();
            }


            return (image, rawIFD.nextIfd);
        }

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

        public (UInt32 nextIfd, Tag[] tags) ReadIFD(UInt32 offset)
        {
            _Stream.Seek(offset, SeekOrigin.Begin);

            var tagCount = ReadWord();
            var tags = new Tag[tagCount];

            for (int i = 0; i < tagCount; i++)
            {
                tags[i] = ReadTag();
            }

            var nextIfd = ReadDWord();
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
        protected Tag ParseTag(Tag tag)
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
                case TagDataType.IFD:
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



        protected Tag<T> ParseTag<T>(Tag tag) where T : struct
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



        public UInt16 ReadWord() { return BitConverter.ToUInt16(ReadEndianBytes(2), 0); }
        public UInt32 ReadDWord() { return BitConverter.ToUInt32(ReadEndianBytes(4), 0); }
        public Int16 ReadShort() { return BitConverter.ToInt16(ReadEndianBytes(2), 0); }
        public Int32 ReadLong() { return BitConverter.ToInt32(ReadEndianBytes(4), 0); }
        public float ReadFloat() { return BitConverter.ToSingle(ReadEndianBytes(4), 0); }
        public double ReadDouble() { return BitConverter.ToDouble(ReadEndianBytes(4), 0); }
        public Rational ReadRational() { return new Rational(ReadDWord(), ReadDWord()); }
        public SRational ReadSRational() { return new SRational(ReadLong(), ReadLong()); }
        
        protected byte[] ReadEndianBytes(int count)
        {
            var bytes = new byte[count];
            _Stream.Read(bytes, 0, count);
            CheckEndian(bytes);
            return bytes;
        }
    }
}
