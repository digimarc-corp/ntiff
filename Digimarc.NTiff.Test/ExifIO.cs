using Digimarc.NTiff;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Digimarc.NTiff.Test
{
    [TestClass]
    public class ExifIO
    {
        [TestMethod]
        public void CanReadExifBlock()
        {
            using (var stream = new FileStream(Samples.LAB, FileMode.Open, FileAccess.Read, FileShare.Read))
            {

                var tiffStream = new TiffStreamReader(stream);
                var exif = tiffStream.ParseIFD(2991224);

                Assert.AreEqual(36, exif.tags.Length);
                Assert.AreEqual(0u, exif.nextIfd);
                Assert.AreEqual((short)400, exif.tags.Where(t => t.ID == (ushort)Tags.ExifTags.ISOSpeedRatings).First().GetValue<short>(0));
            }
        }
    }
}
