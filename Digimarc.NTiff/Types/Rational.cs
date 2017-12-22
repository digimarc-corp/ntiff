using System;
using System.Collections.Generic;
using System.Text;

namespace Digimarc.NTiff.Types
{
    public struct Rational
    {
        public Rational(uint numerator, uint denominator)
        {
            Numerator = numerator;
            Denominator = denominator;
        }

        public readonly uint Numerator;
        public readonly uint Denominator;
        public double ToDouble()
        {
            return Numerator / Denominator;
        }

        public override string ToString()
        {
            return $"{Numerator:N0}/{Denominator:N0}";
        }
    }   
}
