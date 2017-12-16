using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;

namespace TIFFTools
{
    /// <summary>
    /// Reads the Exif block from a source TIFF and stores it for later rewriting. Rewrite requires the target file have no existing Exif block, and a placeholder tag
    /// in IFD0 prepped for an Exif pointer.
    /// </summary>
    public class ExifBuffer
    {
        public ExifBuffer() { }

        public byte[] ExifBlock { get; private set; }

        /// <summary>
        /// Copies an Exif-formatted IFD from the source stream to the destination, and updates Exif pointers.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="sourceOffset"></param>
        /// <param name="destination"></param>
        /// <param name="destinationOffset"></param>
        /// <returns></returns>
        public static bool CopyExif(Stream source, long sourceOffset, Stream destination, long destinationOffset)
        {
            try
            {
                var exifShortTags = source.ReadIFD(sourceOffset).ToList();
                var exifLongTags = exifShortTags.Where(t => t.Length > 4)
                    .Select(t => source.ReadExtendedTag(t)).ToList();
                var exifRationalTags = exifShortTags.Where(t => t.Datatype == TiffDatatype.Rational || t.Datatype == TiffDatatype.SRational)
                    .Select(t => source.ReadExtendedTag(t, asExifRational: true)).ToList();

                destination.Seek(destinationOffset, SeekOrigin.Begin);
                destination.WriteWord((ushort)exifShortTags.Count());

                // write out the Exif IFD
                exifShortTags.ForEach(t => destination.WriteShortTag(t));

                // write out extended data and realign pointers
                exifLongTags.ForEach(t => destination.WriteExtendedTag(t));
                exifRationalTags.ForEach(t => destination.WriteExtendedTag(t));

                // sort final IFD
                destination.ReWriteIFD(destinationOffset);

                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Writes a buffered Exif block to a TIFF stream. Requires that a placeholder tag of some kind exist in the destination.
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="placeholderId"></param>
        /// <returns></returns>
        public bool Write(Stream stream, UInt16 placeholderId = Tiff.ExifPointerTagID)
        {
            stream.Seek(0, SeekOrigin.Begin);
            var head = stream.ReadTiffHeader();
            var ifd0 = stream.ReadIFD(head.IFD0).ToList();

            var exifPointer = ifd0.Where(t => t.TagID == placeholderId).FirstOrDefault();
            if (exifPointer == null) { return false; }
            else
            {
                var ifdOffset = stream.Seek(0, SeekOrigin.End);
                exifPointer.TagID = Tiff.ExifPointerTagID;
                exifPointer.Data = (uint)ifdOffset;
                exifPointer.Datatype = TiffDatatype.Long;
                exifPointer.Length = 1;
                stream.WriteShortTag(exifPointer, overwrite: true);
                stream.ReWriteIFD(head.IFD0);

                return CopyExif(new MemoryStream(ExifBlock), 0, stream, ifdOffset);
            }
        }

        /// <summary>
        /// Finds the Exif block (if any) in a source TIFF stream, and copies it to a temporary buffer.
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public bool Read(Stream stream)
        {
            stream.Seek(0, SeekOrigin.Begin);
            var head = stream.ReadTiffHeader();
            var ifd0 = stream.ReadIFD(head.IFD0).ToList();

            var exifPointer = ifd0.Where(t => t.TagID == Tiff.ExifPointerTagID).FirstOrDefault();
            if (exifPointer == null) { return false; }
            else
            {
                var dest = new MemoryStream();
                if (CopyExif(stream, exifPointer.Data, dest, 0))
                {
                    ExifBlock = dest.ToArray();
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
    }
}
