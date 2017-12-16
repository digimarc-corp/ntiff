using System;
using System.Collections.Generic;
using System.Text;

namespace NTiff
{
    public enum TagDataType
    {
        /// <summary>
        /// 8-bit unsigned int (char/byte)
        /// </summary>
        Byte = 1,
        /// <summary>
        /// 7-bit nul-terminated string
        /// </summary>
        ASCII = 2,
        /// <summary>
        /// 16-bit unsigned int (Uint16/ushort)
        /// </summary>
        Short = 3,
        /// <summary>
        /// 32-bit unsigned int (UInt32/uint)
        /// </summary>
        Long = 4,
        /// <summary>
        /// 64-bit unsigned rational/decimal
        /// </summary>
        Rational = 5,
        /// <summary>
        /// 8-bit signed int (no .Net equivalent)
        /// </summary>
        SByte = 6,
        /// <summary>
        /// Array of special-purpose bytes
        /// </summary>
        Undefined = 7,
        /// <summary>
        /// 16-bit signed int (Int16/short)
        /// </summary>
        SShort = 8,
        /// <summary>
        /// 32-bit signed int (Int32/int)
        /// </summary>
        SLong = 9,
        /// <summary>
        /// 64-bit signed rational/decimal
        /// </summary>
        SRational = 10,
        /// <summary>
        /// 32-bit single precision float (float)
        /// </summary>
        Float = 11,
        /// <summary>
        /// 64-bit single precision float (double)
        /// </summary>
        Double = 12
    }
}
