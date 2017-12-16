using System;
using System.Collections.Generic;
using System.Text;

namespace NTiff
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

    public struct SRational
    {
        public SRational(int numerator, int denominator)
        {
            Numerator = numerator;
            Denominator = denominator;
        }

        public readonly int Numerator;
        public readonly int Denominator;
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
