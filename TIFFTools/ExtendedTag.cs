using System;
using System.Collections.Generic;
using System.Text;

namespace TIFFTools
{
    public class ExtendedTag
    {
        public ShortTag ShortTag { get; set; }

        public byte[] Data { get; set; }
        public bool IsExifRational { get; set; }

        public override string ToString()
        {
            var data = Encoding.ASCII.GetString(Data);
            return ShortTag.ToString() + $":{data}";
        }
    }
}
