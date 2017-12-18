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
        public void CanReWriteTiff()
        {
            var src = "Samples/eagle_cap_lab.tif";
            var tiff = new Tiff(src);
            var temp = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString() + ".tiff");
            try
            {
                tiff.Save(temp);
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
