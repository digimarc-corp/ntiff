using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Digimarc.NTiff.Test
{
    public static class Samples
    {
        public const string LAB = "../../../../Samples/eagle_cap_lab.tif";
        public const string LittleEndian = "../../../../Samples/eagle_cap_le.tif";
        public const string Pyramid = "../../../../Samples/eagle_cap_pyramid.tif";
        public const string LZW = "../../../../Samples/eagle_cap_lzw.tif";

        public static string GetTemp() { return Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString() + ".tiff"); }
    }
}
