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
        public void CanReadRawIFD0Tags()
        {
            var stream = new TiffStream("Samples/eagle_cap_lab.tif");
            var ifd0 = stream.ReadIFD(stream.ReadHeader());

            Assert.AreEqual(23, ifd0.tags.Length);
            Assert.AreEqual(0u, ifd0.nextIfd);
        }

        [TestMethod]
        public void CanParseIFD0Tags()
        {
            var stream = new TiffStream("Samples/eagle_cap_lab.tif");
            var ifd0 = stream.ParseIFD(stream.ReadHeader());

            Assert.AreEqual("NIKON D90", ifd0.tags[7].GetString());
            Assert.AreEqual(8, ifd0.tags[3].GetValue<ushort>(2));
            Assert.AreEqual(2991224u, ifd0.tags[22].GetValue<uint>(0));
        }

        [TestMethod]
        public void CanReadIFDFromFixedOffset()
        {
            var stream = new TiffStream("Samples/eagle_cap_lab.tif");
            var ifd0 = stream.ReadIFD(8);

            Assert.AreEqual(23, ifd0.tags.Length);
        }
                
        [TestMethod]
        public void CanLoadStrips()
        {
            var stream = new TiffStream("Samples/eagle_cap_lab.tif");
            var strips = stream.ReadStrips(8);

            Assert.AreEqual(1, strips.Length);
            Assert.AreEqual(2965650, strips[0].ImageData.Length);
        }        
    }
}
