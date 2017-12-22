using Digimarc.NTiff.Tags;
using Digimarc.NTiff.Types;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Digimarc.NTiff
{
    public class Tiff
    {
        public List<Image> Images { get; set; } = new List<Image>();
        public bool IsBigEndian { get; set; }

        public Tiff() { }

        /// <summary>
        /// Loads a TIFF file from disk.
        /// </summary>
        /// <param name="fileName"></param>
        public Tiff(string fileName)
        {
            LoadTiff(fileName);
        }

        private void LoadTiff(string fileName)
        {
            using (var stream = new TiffStreamReader(fileName))
            {
                LoadImage(stream);
            }
        }

        private void LoadImage(TiffStreamReader stream)
        {
            Image image;
            uint offset;
            var offsets = new HashSet<uint>(); // keep track of read offsets so we can detect loops

            offset = stream.ReadHeader();

            // set endianness now that the stream header has been validated
            IsBigEndian = stream.IsBigEndian;

            // read all images from file
            while (offset != 0)
            {
                if (offsets.Contains(offset)) { throw new InvalidDataException("Circular image reference detected in file. Aborting load."); }
                else
                {
                    offsets.Add(offset);
                    (image, offset) = stream.ReadImage(offset);
                    Images.Add(image);
                }
            }
        }

        public void Save(Stream stream)
        {
            using (var tiffStream = new TiffStreamWriter(forceBigEndian: IsBigEndian))
            {
                WriteTo(tiffStream);


                tiffStream.Seek(0, SeekOrigin.Begin);
                tiffStream.CopyTo(stream);
            }
        }

        public void Save(string fileName)
        {
            using (var tiffStream = new TiffStreamWriter(forceBigEndian: IsBigEndian))
            {
                WriteTo(tiffStream);

                using (var stream = new FileStream(fileName, FileMode.CreateNew, FileAccess.ReadWrite, FileShare.Read))
                {
                    tiffStream.Seek(0, SeekOrigin.Begin);
                    tiffStream.CopyTo(stream);
                }
            }
        }

        void WriteTo(TiffStreamWriter tiffStream)
        {
            tiffStream.WriteHeader();
            tiffStream.WriteDWord(8); // IFD0 will always be immediately after the header
            uint previousOffset = 0;

            foreach (var image in Images)
            {
                var imageOffset = (uint)tiffStream.SeekWord(0, SeekOrigin.End);

                // update the pointer from the previous image
                if (previousOffset != 0)
                {
                    tiffStream.UpdateIFDPointer(previousOffset, imageOffset);
                    tiffStream.Seek(imageOffset, SeekOrigin.Begin);
                }


                if (image.Exif?.Count > 0 && !image.Tags.Any(t => t.ID == (ushort)PrivateTags.ExifIFD))
                {
                    // We have EXIF tags to write, and no current ExifIFD pointer
                    image.Tags.Add(new Tag<uint>() { ID = (ushort)PrivateTags.ExifIFD, DataType = TagDataType.Long, Length = 1, Values = new uint[] { 0 } });
                }
                else if ((image.Exif?.Count ?? 0) == 0 && image.Tags.Any(t => t.ID == (ushort)PrivateTags.ExifIFD))
                {
                    // There are no EXIF tags to write, get rid of the superfluous pointer
                    image.Tags.RemoveAll(t => t.ID == (ushort)Tags.PrivateTags.ExifIFD);
                }
                tiffStream.WriteIFD(image.Tags);

                // write image strip data
                tiffStream.WriteStrips(imageOffset, image.Strips.ToArray());

                // write Exif block, if necessary
                if (image.Exif?.Count > 0)
                {
                    var exifIfdOffset = (uint)tiffStream.Seek(0, SeekOrigin.End);
                    tiffStream.WriteIFD(image.Exif);
                    var exifTag = image.Tags.Where(t => t.ID == (ushort)PrivateTags.ExifIFD).First() as Tag<uint>;
                    exifTag.Values[0] = exifIfdOffset;
                    tiffStream.UpdateTags(exifTag);
                }

                previousOffset = imageOffset;
            }
        }
    }
}
