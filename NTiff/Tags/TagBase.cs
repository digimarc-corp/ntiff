using System;
using System.Collections.Generic;
using System.Text;

namespace NTiff.Tags
{
    public abstract class TagBase
    {
        public UInt32 Offset { get; set; }
        public UInt16 ID { get; set; }
        public TagDataType DataType { get; set; }
        public UInt32 Length { get; set; }
        public byte[] RawValue { get; set; } = new byte[4];
                
        public abstract T GetValue<T>(int index);
        public abstract string GetString();

        public override string ToString()
        {
            return $"{ID}:{DataType}:{Length}:{GetString()}";
        }
    }
}
