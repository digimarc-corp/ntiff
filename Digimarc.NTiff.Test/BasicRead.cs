/*
   Copyright 2018 Digimarc, Inc

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

using Digimarc.NTiff;
using Digimarc.NTiff.Tags;
using System;
using System.IO;
using System.Linq;
using Xunit;

namespace Digimarc.NTiff.Test
{
    public class BasicRead
    {
        [Fact]
        public void CanGetTagAsString()
        {
            var tag = new Tag<char>
            {
                DataType = TagDataType.ASCII,
                ID = 271,
                Length = 6,
                Offset = 8,
                RawValue = new byte[] { 0x08, 0x00, 0x00, 0x00 },
                Values = "Nikon".ToASCIIArray()
            };

            var str = tag.ToString();

            Assert.Equal("Make:ASCII:6:Nikon", str);
        }

        [Fact]
        public void CanReadTiffHeader()
        {
            using (var stream = new FileStream(Samples.LAB, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                var tiffStream = new TiffStreamReader(stream);
                var ifd0 = tiffStream.ReadHeader();
                Assert.Equal(8u, ifd0);
            }
        }

        [Fact]
        public void CanReadRawIFD0Tags()
        {
            using (var stream = new FileStream(Samples.LAB, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                var tiffStream = new TiffStreamReader(stream);
                var ifd0 = tiffStream.ReadIFD(tiffStream.ReadHeader());

                Assert.Equal(23, ifd0.tags.Length);
                Assert.Equal(0u, ifd0.nextIfd);
            }
        }

        [Fact]
        public void CanParseIFD0Tags()
        {
            using (var stream = new FileStream(Samples.LAB, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                var tiffStream = new TiffStreamReader(stream);
                var ifd0 = tiffStream.ParseIFD(tiffStream.ReadHeader());

                Assert.Equal("NIKON D90", ifd0.tags[7].GetString());
                Assert.Equal(8, ifd0.tags[3].GetValue<short>(2));
                Assert.Equal(2991224u, ifd0.tags[22].GetValue<uint>(0));
            }
        }

        [Fact]
        public void CanReadIFDFromFixedOffset()
        {
            using (var stream = new FileStream(Samples.LAB, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                var tiffStream = new TiffStreamReader(stream);
                var ifd0 = tiffStream.ReadIFD(8);

                Assert.Equal(23, ifd0.tags.Length);
            }
        }

        [Fact]
        public void CanLoadStrips()
        {
            using (var stream = new FileStream(Samples.LAB, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                var tiffStream = new TiffStreamReader(stream);
                var strips = tiffStream.ReadStrips(8);

                Assert.Equal(1, strips.Length);
                Assert.Equal(2965650, strips[0].ImageData.Length);
            }
        }
    }
}
