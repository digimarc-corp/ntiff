using System;
using System.Collections.Generic;
using System.Text;

namespace Digimarc.NTiff.Types
{
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
