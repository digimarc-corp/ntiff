using NTiff.Tags;
using NTiff.Types;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace NTiff
{
    public class Tiff
    {
        /// <summary>
        /// Loads a TIFF file from disk.
        /// </summary>
        /// <param name="fileName"></param>
        public Tiff(string fileName)
        {
            _Stream = new TiffStreamReader(fileName);
            var ifd0 = _Stream.ReadHeader();

            var rawIFD = _Stream.ParseIFD(ifd0);
            var ifd = new IFD();
            ifd.Tags = rawIFD.tags.ToList();
            ifd.Strips = _Stream.ReadStrips(ifd0).OrderBy(s => s.StripNumber).ToList();
            var exifOffset = rawIFD.tags.Where(t => t.ID == (ushort)PrivateTags.ExifIFD).FirstOrDefault();
            if (exifOffset != null)
            {
                ifd.Exif = _Stream.ParseIFD(exifOffset.GetValue<uint>(0)).tags.ToList();
            }
            IFDs.Add(ifd);
        }

        public void Save(string fileName)
        {
            using (var tiffStream = new TiffStreamWriter(forceBigEndian: _Stream.IsBigEndian))
            {
                tiffStream.WriteHeader();
                tiffStream.WriteDWord(8); // IFD0 will always be immediately after the header
                var previousOffset = (uint)tiffStream.Position;

                foreach (var ifd in IFDs)
                {
                    if (ifd.Exif?.Count > 0 && !ifd.Tags.Any(t => t.ID == (ushort)PrivateTags.ExifIFD))
                    {
                        // We have EXIF tags to write, and no current ExifIFD pointer
                        ifd.Tags.Add(new Tag<uint>() { ID = (ushort)PrivateTags.ExifIFD, DataType = TagDataType.Long, Length = 1, Values = new uint[] { 0 } });
                    }
                    else if ((ifd.Exif?.Count ?? 0) == 0 && ifd.Tags.Any(t => t.ID == (ushort)PrivateTags.ExifIFD))
                    {
                        // There are no EXIF tags to write, get rid of the superfluous pointer
                        ifd.Tags.RemoveAll(t => t.ID == (ushort)Tags.PrivateTags.ExifIFD);
                    }
                    tiffStream.WriteIFD(ifd.Tags);

                    // write image strip data
                    tiffStream.WriteStrips(previousOffset, ifd.Strips.ToArray());
                    tiffStream.UpdateTags(ifd.Tags.Where(t => t.ID == (ushort)BaselineTags.StripByteCounts || t.ID == (ushort)BaselineTags.StripOffsets).ToArray());

                    // write Exif block, if necessary
                    if (ifd.Exif?.Count > 0)
                    {
                        var exifIfdOffset = (uint)tiffStream.Seek(0, SeekOrigin.End);
                        tiffStream.WriteIFD(ifd.Exif);
                        var exifTag = ifd.Tags.Where(t => t.ID == (ushort)PrivateTags.ExifIFD).First() as Tag<uint>;
                        exifTag.Values[0] = exifIfdOffset;
                        tiffStream.UpdateTags(exifTag);
                    }
                }

                using (var stream = new FileStream(fileName, FileMode.CreateNew, FileAccess.ReadWrite, FileShare.Read))
                {
                    tiffStream.Seek(0, SeekOrigin.Begin);
                    tiffStream.CopyTo(stream);
                }
            }
        }

        TiffStreamReader _Stream;

        public List<IFD> IFDs { get; set; } = new List<IFD>();
    }
}
