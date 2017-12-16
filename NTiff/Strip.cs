using System;
using System.Collections.Generic;
using System.Text;

namespace NTiff
{
    public class Strip
    {
        public byte[] ImageData { get; set; }
        public ushort StripNumber { get; set; }
        public uint StripOffset { get; set; }
    }
}
