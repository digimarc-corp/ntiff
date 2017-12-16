using System;
using System.Collections.Generic;
using System.Text;

namespace TIFFTools
{
    public enum TiffDatatype
    {
        Byte = 1, //8-bit unsigned integer
        ASCII = 2, //8-bit, NULL-terminated string
        Short = 3, //16-bit unsigned integer
        Long = 4, //32-bit unsigned integer
        Rational = 5, //Two 32-bit unsigned integers
        Undefined = 7, //Other special-purpose data
        SRational = 10 //Signed rational
    }
}
