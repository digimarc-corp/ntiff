using Digimarc.NTiff;
using Digimarc.NTiff.Tags;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Linq;

namespace Digimarc.NTiff.Test
{
    [TestClass]
    public class BasicRead
    {
        [TestMethod]
        public void CanGetTagAsString()
        {
            var tag = new Tag<char>
            {
                DataType = TagDataType.ASCII,
                ID = 271,
                Length = 6,
                Offset = 8,
                RawValue = new byte[] { 0x08, 0x00, 0x00, 0x00 },
                Values = "Nikon".ToASCIIArray()
            };

            var str = tag.ToString();

            Assert.AreEqual("Make:ASCII:6:Nikon", str);
        }

        [TestMethod]
        public void CanReadTiffHeader()
        {
            using (var stream = new FileStream(Samples.LAB, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                var tiffStream = new TiffStreamReader(stream);
                var ifd0 = tiffStream.ReadHeader();
                Assert.AreEqual(8u, ifd0);
            }
        }

        [TestMethod]
        public void CanReadRawIFD0Tags()
        {
            using (var stream = new FileStream(Samples.LAB, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                var tiffStream = new TiffStreamReader(stream);
                var ifd0 = tiffStream.ReadIFD(tiffStream.ReadHeader());

                Assert.AreEqual(23, ifd0.tags.Length);
                Assert.AreEqual(0u, ifd0.nextIfd);
            }
        }

        [TestMethod]
        public void CanParseIFD0Tags()
        {
            using (var stream = new FileStream(Samples.LAB, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                var tiffStream = new TiffStreamReader(stream);
                var ifd0 = tiffStream.ParseIFD(tiffStream.ReadHeader());

                Assert.AreEqual("NIKON D90", ifd0.tags[7].GetString());
                Assert.AreEqual(8, ifd0.tags[3].GetValue<short>(2));
                Assert.AreEqual(2991224u, ifd0.tags[22].GetValue<uint>(0));
            }
        }

        [TestMethod]
        public void CanReadIFDFromFixedOffset()
        {
            using (var stream = new FileStream(Samples.LAB, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                var tiffStream = new TiffStreamReader(stream);
                var ifd0 = tiffStream.ReadIFD(8);

                Assert.AreEqual(23, ifd0.tags.Length);
            }
        }

        [TestMethod]
        public void CanLoadStrips()
        {
            using (var stream = new FileStream(Samples.LAB, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                var tiffStream = new TiffStreamReader(stream);
                var strips = tiffStream.ReadStrips(8);

                Assert.AreEqual(1, strips.Length);
                Assert.AreEqual(2965650, strips[0].ImageData.Length);
            }
        }
    }
}
