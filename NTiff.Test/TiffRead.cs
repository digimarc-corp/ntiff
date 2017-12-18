using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace NTiff.Test
{
    [TestClass]
    public class TiffRead
    {
        [TestMethod]
        public void CanLoadTiff()
        {
            var tiff = new Tiff("Samples/eagle_cap_lab.tif");

            Assert.AreEqual(1, tiff.IFDs.Count);
            Assert.AreEqual(23, tiff.IFDs[0].Tags.Count);
            Assert.AreEqual(1, tiff.IFDs[0].Strips.Count);
            Assert.AreEqual(36, tiff.IFDs[0].Exif.Count);
        }
    }
}
