using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NTiff.Test
{
    [TestClass]
    public class BasicIO
    {
        [TestMethod]
        public void CanReadTiffHeader()
        {
            var stream = new TiffStream("Samples/eagle_cap_lab.tif");
            var ifd0 = stream.ReadHeader();
            Assert.AreEqual(8u, ifd0);
        }

        [TestMethod]
        public void CanReadIFD0BaselineTags()
        {
            var stream = new TiffStream("Samples/eagle_cap_lab.tif");
            var ifd0 = stream.ReadIFD(stream.ReadHeader());

            Assert.AreEqual(23, ifd0.tags.Length);
            Assert.AreEqual(0u, ifd0.nextIfd);
        }

        /*
        [TestMethod]
        public void CanLoadStrips()
        {
            var stream = new TiffStream("Samples/eagle_cap_lab.tiff");
            var strips = stream.ReadStrips(8);

            Assert.AreEqual(1, strips.Length);
            Assert.AreEqual(0, strips[0].ImageData.Length);
        }
        */
    }
}
