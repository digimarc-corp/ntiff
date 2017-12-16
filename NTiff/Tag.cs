using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NTiff
{
    /// <summary>
    /// Represents a raw un-parsed tag as read from a TIFF IFD
    /// </summary>
    public class Tag : TagBase
    {
        public override string GetString()
        {
            return "0x" + RawValue.PrettyPrint();
        }

        public override T GetValue<T>(int index)
        {
            throw new IndexOutOfRangeException("Tag must be parsed to access values by index");
        }
    }

    /// <summary>
    /// Represents a parsed tag with a defined type and fetched pointer values (if any).
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Tag<T> : Tag
    {
        /// <summary>
        /// Attempts to fetch a value and cast it to the given type.
        /// </summary>
        /// <typeparam name="V"></typeparam>
        /// <param name="index"></param>
        /// <returns></returns>
        public override V GetValue<V>(int index)
        {
            return (V)(object)Values[index];
        }

        public override string GetString()
        {
            string valueStr = string.Empty;
            switch (DataType)
            {
                case TagDataType.ASCII:
                    valueStr = new string((char[])(object)Values.ToArray()).Replace("\0", "");
                    break;
                case TagDataType.Byte:
                case TagDataType.Undefined:
                    valueStr = ((byte[])(object)Values.ToArray()).PrettyPrint();
                    break;
                default:
                    valueStr = string.Join(", ", Values);
                    break;
            }
            return valueStr;
        }

        public T[] Values { get; set; } = new T[0];
        public override string ToString()
        {
            return $"{(BaselineTags)ID}:{DataType}:{Length}:{GetString()}";
        }

    }
}
