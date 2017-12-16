using System;
using System.Collections.Generic;
using System.Text;

namespace TIFFTools
{
    public class ShortTag : Tag
    {
        public UInt32 Data { get; set; }

        public override string ToString()
        {
            var data = BitConverter.ToString(BitConverter.GetBytes(Data)).Replace("-", "").ToLower();
            return base.ToString() + $":{data}";
        }
    }
}
