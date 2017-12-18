using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
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
                var srcInfo = new FileInfo(src);
                var tmpInfo = new FileInfo(temp);
                Assert.AreEqual(srcInfo.Length, tmpInfo.Length);
            }
            finally
            {
                if (File.Exists(temp)) { File.Delete(temp); }
            }
        }
    }
}
