using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NTiff.Test
{
    [TestClass]
    public class ExifIO
    {
        [TestMethod]
        public void CanReadExifBlock()
        {
            var stream = new TiffStreamReader("Samples/eagle_cap_lab.tif");
            var exif = stream.ParseIFD(2991224);

            Assert.AreEqual(36, exif.tags.Length);
            Assert.AreEqual(0u, exif.nextIfd);
            Assert.AreEqual((short)400, exif.tags.Where(t => t.ID == (ushort)Tags.ExifTags.ISOSpeedRatings).First().GetValue<short>(0));
        }
    }
}
