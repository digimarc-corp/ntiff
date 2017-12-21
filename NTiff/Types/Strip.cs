using System;
using System.Collections.Generic;
using System.Text;

namespace NTiff.Types
{
    public class Strip
    {
        public byte[] ImageData { get; set; }
        public ushort StripNumber { get; set; }
        public uint StripOffset { get; set; }

        public string GetHash()
        {
            var md5 = System.Security.Cryptography.MD5.Create();
            var hash = md5.ComputeHash(ImageData);
            return Convert.ToBase64String(hash);
        }
    }
}
