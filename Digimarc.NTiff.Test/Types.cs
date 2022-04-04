/*
   Copyright 2022 Digimarc, Inc

   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

       http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.

   SPDX-License-Identifier: Apache-2.0
*/

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
