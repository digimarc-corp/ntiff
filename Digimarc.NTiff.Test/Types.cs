using Digimarc.NTiff.Types;
using Xunit;

namespace Digimarc.NTiff.Test
{
    public class Types
    {
        [Theory]
        [InlineData(10, 1, 10.0)]
        [InlineData(1, 10, 0.1)]
        public void RationalToDouble(uint numerator, uint denominator, double result)
        {
            var number = new Rational(numerator, denominator);

            Assert.Equal(result, number.ToDouble());
        }

        [Theory]
        [InlineData(10, 1, 10.0)]
        [InlineData(1, 10, 0.1)]
        [InlineData(-1, 10, -0.1)]
        [InlineData(-1, -10, 0.1)]
        public void SRationalToDouble(int numerator, int denominator, double result)
        {
            var number = new SRational(numerator, denominator);

            Assert.Equal(result, number.ToDouble());
        }
    }
}
