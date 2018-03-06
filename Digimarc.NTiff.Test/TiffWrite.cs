using Digimarc.NTiff;
using Digimarc.NTiff.Tags;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Digimarc.NTiff.Test
{
    [TestClass]
    public class TiffWrite
    {
        [TestMethod]
        public void CanAddExifToStrippedFile()
        {
            var temp = Samples.GetTemp();
            try
            {
                var tiff1 = new Tiff(Samples.LAB);
                var exif = tiff1.Images[0].Exif;

                var tiff2 = new Tiff(Samples.NoExif);
                Assert.IsNull(tiff2.Images[0].Exif);

                tiff2.Images[0].Exif = exif;
                Assert.AreEqual(36, tiff2.Images[0].Exif.Count);

                tiff2.Save(temp);

                var tiff3 = new Tiff(temp);
                Assert.AreEqual(tiff2.Images[0].Strips[0].GetHash(), tiff3.Images[0].Strips[0].GetHash());
            }
            finally
            {
                Samples.Cleanup(temp);
            }
        }

        [TestMethod]
        public void CanAddTiffTag()
        {
            var tiff1 = new Tiff(Samples.LittleEndian);
            tiff1.Images[0].Tags.Add(new Tag<byte>()
            {
                DataType = TagDataType.Byte,
                ID = (ushort)PrivateTags.AliasLayerMetadata,
                Values = Encoding.UTF8.GetBytes("Hello world"),
                Length = 11
            });
            var temp = Samples.GetTemp();
            try
            {
                tiff1.Save(temp);

                var tiff2 = new Tiff(temp);
                Assert.AreEqual(tiff1.Images[0].Tags.Count, tiff2.Images[0].Tags.Count);
                Assert.AreEqual(tiff1.Images[0].Strips[0].GetHash(), tiff2.Images[0].Strips[0].GetHash());
            }
            finally
            {
                Samples.Cleanup(temp);
            }
        }

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
                Samples.Cleanup(temp);
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

        [TestMethod]
        public void CanWriteStream()
        {
            string hash1, hash2;
            using (var stream = new MemoryStream())
            {
                var tiff = new Tiff(Samples.LAB);
                tiff.Save(stream);
                stream.Seek(0, SeekOrigin.Begin);
                hash1 = Convert.ToBase64String(System.Security.Cryptography.MD5.Create().ComputeHash(stream));
            }
            hash2 = CheckRewrite(Samples.LAB);

            Assert.AreEqual(hash1, hash2);
        }       

        /// <summary>
        /// Read and parse a sample file from disk, write it back out to a temp file, and verify metadata and properties via NTiff & ImageMagick. Returns hash of final output.
        /// </summary>
        /// <param name="src"></param>
        private static string CheckRewrite(string src)
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

                return Convert.ToBase64String(System.Security.Cryptography.MD5.Create().ComputeHash(File.ReadAllBytes(temp)));
            }
            finally
            {
                Samples.Cleanup(temp);
            }
        }
    }
}
