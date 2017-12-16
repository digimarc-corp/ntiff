using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;

namespace TIFFTools
{
    public class Tiff
    {
        public const UInt16 ExifPointerTagID = 0x8769;
        public const UInt16 LittleEndianHeader = 0x4949;
        public const UInt16 TiffMagicNumber = 42;

        /*
        public List<ShortTag> IFD0 = new List<ShortTag>();

        public Tiff(string fileName)
        {
            using (var stream = new FileStream(fileName, FileMode.Open))
            {
                var tiff = new TiffReader(stream);

                var head = tiff.ReadWord();
                var bom = tiff.ReadWord();
                var ifd0 = tiff.ReadDWord();

                IFD0 = tiff.ReadIFD(ifd0).ToList();

                var exifOffset = IFD0.Where(t => t.TagID == 0x8769).First();

                var exifTags = new List<Tag>();
                var shortTags = tiff.ReadIFD(exifOffset.Data).ToList();
                var longTags = shortTags.Where(t => t.Length > 4).Select(t => tiff.ReadExtendedTag(t)).ToList();
                exifTags.AddRange(shortTags.Where(t => t.Length <= 4));
                exifTags.AddRange(longTags);
            }
        }*/
    }
}
