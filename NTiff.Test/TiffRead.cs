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
            var tiff = new Tiff(Samples.LAB);

            Assert.AreEqual(1, tiff.Images.Count);
            Assert.AreEqual(23, tiff.Images[0].Tags.Count);
            Assert.AreEqual(1, tiff.Images[0].Strips.Count);
            Assert.AreEqual(36, tiff.Images[0].Exif.Count);
        }

        [TestMethod]
        public void CanLoadPyramid()
        {
            var tiff = new Tiff(Samples.Pyramid);

            Assert.AreEqual(1, tiff.Images.Count);
            Assert.AreEqual(1, tiff.Images[0].SubImages.Count);
            Assert.AreEqual(1, tiff.Images[0].SubImages[0].Strips.Count);
            Assert.AreEqual(15, tiff.Images[0].SubImages[0].Tags.Count);
        }
    }
}
