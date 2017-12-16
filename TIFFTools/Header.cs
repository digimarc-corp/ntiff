using System;
using System.Collections.Generic;
using System.Text;

namespace TIFFTools
{
    public class Header
    {
        public bool IsBigEndian { get; set; } = false;
        public uint IFD0 { get; set; }
    }
}
