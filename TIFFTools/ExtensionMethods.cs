using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;

namespace TIFFTools
{
    public static class ExtensionMethods
    {
        /// <summary>
        /// Writes extended tag data to end-of-stream, and updates the previously written short tag pointer value
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="tag"></param>
        public static void WriteExtendedTag(this Stream stream, ExtendedTag tag)
        {
            //update short tag with new pointer value
            tag.ShortTag.Data = (uint)stream.Position;
            stream.WriteShortTag(tag.ShortTag, overwrite: true);

            //write extended value to end-of-stream
            stream.Seek(0, SeekOrigin.End);
            stream.Write(tag.Data, 0, tag.Data.Length);
        }

        /// <summary>
        /// Writes the short TIFF tag to the stream. If overwrite is false, the tag is written to the end of the stream, and it's offset is updated.
        /// If overwrite is true, the tag is rewritten at its existing offset.
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="tag"></param>
        /// <param name="overwrite"></param>
        public static void WriteShortTag(this Stream stream, ShortTag tag, bool overwrite = false)
        {
            if (overwrite) { stream.Seek((long)tag.TagOffset, SeekOrigin.Begin); }
            else { tag.TagOffset = (ulong)stream.Position; }

            stream.WriteWord(tag.TagID);
            stream.WriteWord(((UInt16)tag.Datatype));
            stream.WriteDWord(tag.Length);
            stream.WriteDWord(tag.Data);
        }

        /// <summary>
        /// Rewrites a TIFF IFD to ensure tags are sorted properly
        /// </summary>
        /// <param name="stream"></param>
        public static void ReWriteIFD(this Stream stream, long ifdOffset)
        {
            var tags = stream.ReadIFD(ifdOffset).ToList();
            stream.Seek(ifdOffset, SeekOrigin.Begin);
            stream.WriteWord((ushort)tags.Count());
            foreach (var tag in tags.OrderBy(t => t.TagID))
            {
                stream.WriteShortTag(tag);
            }
        }

        public static IEnumerable<ShortTag> ReadIFD(this Stream stream, long offset)
        {
            stream.Seek(offset, SeekOrigin.Begin);
            return stream.ReadIFD();
        }

        public static IEnumerable<ShortTag> ReadIFD(this Stream stream)
        {
            var tagCount = stream.ReadWord();
            for (int i = 0; i < tagCount; i++)
            {
                var tagOffset = stream.Position;

                var tag = stream.ReadWord();
                var tagType = stream.ReadWord();
                var tagLength = stream.ReadDWord();
                var tagData = stream.ReadDWord();
                yield return new ShortTag { TagOffset = (UInt64)tagOffset, Data = tagData, Datatype = (TiffDatatype)tagType, Length = tagLength, TagID = tag };
            }
        }

        public static ExtendedTag ReadExtendedTag(this Stream stream, ShortTag tag, bool asExifRational = false)
        {
            var length = asExifRational ? 8 : tag.Length;
            stream.Seek(Convert.ToUInt32(tag.Data), SeekOrigin.Begin);
            var bytes = new byte[length];
            stream.Read(bytes, 0, (int)length);
            return new ExtendedTag
            {
                ShortTag = tag,
                Data = bytes,
                IsExifRational = asExifRational
            };
        }

    }
}
