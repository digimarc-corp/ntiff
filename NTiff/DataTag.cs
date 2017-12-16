using System;
using System.Collections.Generic;
using System.Text;

namespace NTiff
{
    public class DataTag<T> : Tag
    {
        public List<T> Values { get; set; }
    }
}
