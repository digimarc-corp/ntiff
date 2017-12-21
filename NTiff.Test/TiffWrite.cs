using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace NTiff.Test
{
    [TestClass]
    public class TiffWrite
    {
        [TestMethod]
        public void CanCombineTiffs()
        {
            var tiff1 = new Tiff(Samples.LittleEndian);
            var tiff2 = new Tiff(Samples.LZW);
            Assert.AreEqual(tiff1.IsBigEndian, tiff2.IsBigEndian);
            var temp = Samples.GetTemp();
            try
            {
                var tempTiff = new Tiff();
                tempTiff.Images.Add(tiff1.Images[0]);
                tempTiff.Images.Add(tiff2.Images[0]);
                tempTiff.Save(temp);

                System.Threading.Thread.Sleep(1000);

                var newTiff = new Tiff(temp);
                Assert.AreEqual(2, newTiff.Images.Count);
            }
            finally
            {
                if (File.Exists(temp)) { File.Delete(temp); }
            }
        }

        [TestMethod]
        public void CanReWritePyramid()
        {
            CheckRewrite(Samples.Pyramid);
        }

        [TestMethod]
        public void CanReWriteTiff()
        {
            CheckRewrite(Samples.LAB);
        }

        [TestMethod]
        public void CanReWriteLittleEndianTiff()
        {
            CheckRewrite(Samples.LittleEndian);
        }

        /// <summary>
        /// Read and parse a sample file from disk, write it back out to a temp file, and verify metadata and properties via NTiff & ImageMagick
        /// </summary>
        /// <param name="src"></param>
        private static void CheckRewrite(string src)
        {
            var tiff = new Tiff(src);
            var temp = Samples.GetTemp();
            try
            {
                tiff.Save(temp);

                // Load source and temp in ImageMagick and compare properties
                var srcImg = new ImageMagick.MagickImage(src);
                var tempImg = new ImageMagick.MagickImage(temp);

                var srcAttributes = srcImg.AttributeNames;
                var tempAttributes = tempImg.AttributeNames;

                Assert.AreEqual(srcImg.Signature, tempImg.Signature);
                Assert.IsTrue(srcAttributes.SequenceEqual(tempAttributes));
                Assert.AreEqual(srcAttributes.Count(), tempAttributes.Count());

                foreach (var attr in srcAttributes)
                {
                    if (attr == "date:create" || attr == "date:modify") { continue; }
                    Assert.AreEqual(srcImg.GetAttribute(attr), tempImg.GetAttribute(attr), attr);
                }
            }
            finally
            {
                if (File.Exists(temp)) { File.Delete(temp); }
            }
        }
    }
}
