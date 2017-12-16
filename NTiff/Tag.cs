using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NTiff
{
    public class Tag
    {
        public UInt32 Offset { get; set; }
        public UInt16 ID { get; set; }
        public TagDataType DataType { get; set; }
        public UInt32 Length { get; set; }
        public byte[] RawValue { get; set; } = new byte[4];

        public override string ToString()
        {
            return $"{ID}:{DataType}:{Length}:0x{RawValue.PrettyPrint()}";
        }
    }

    public class Tag<T> : Tag
    {
        public T[] Values { get; set; } = new T[0];
        public override string ToString()
        {
            string values = string.Empty;
            switch (DataType)
            {
                case TagDataType.ASCII:
                    values = new string((char[])(object)Values.ToArray());
                    break;
                case TagDataType.Byte:
                case TagDataType.Undefined:
                    values = ((byte[])(object)Values.ToArray()).PrettyPrint();
                    break;
                default:
                    values = string.Join(", ", Values);
                    break;

            }
            return $"{(BaselineTags)ID}:{DataType}:{Length}:{values}";
        }

    }
}
