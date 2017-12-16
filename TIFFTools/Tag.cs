using System;
using System.Collections.Generic;
using System.Text;

namespace TIFFTools
{
    public abstract class Tag
    {
        public UInt64 TagOffset { get; set; }
        public UInt16 TagID { get; set; }
        public TiffDatatype Datatype { get; set; }
        public UInt32 Length { get; set; }

        public override string ToString()
        {
            return $"{TagID}:{Datatype}:{Length}";
        }
    }
}
