using System;
using System.Collections.Generic;
using System.Text;

namespace NTiff
{
    static class ExtensionMethods
    {
        /// <summary>
        /// Gets the length, in bytes, of a given TIFF tag datatype.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static int AtomicLength(this TagDataType type)
        {
            switch (type)
            {
                case TagDataType.ASCII:
                case TagDataType.Byte:
                case TagDataType.SByte:
                case TagDataType.Undefined:
                    return 1;
                case TagDataType.Short:
                case TagDataType.SShort:
                    return 2;
                case TagDataType.Float:
                case TagDataType.Long:
                case TagDataType.SLong:
                    return 4;
                case TagDataType.Double:
                case TagDataType.Rational:
                case TagDataType.SRational:
                    return 8;
                default:
                    throw new ArgumentException($"Unknown tag datatype {type}");
            }
        }

        public static string PrettyPrint(this byte[] bytes, int maxLength = 20)
        {
            var str = BitConverter.ToString(bytes).Replace("-", "").ToLower();
            if (str.Length>maxLength)
            {
                str = str.Substring(0, 20) + "...";
            }
            return str;
        }
    }
}
