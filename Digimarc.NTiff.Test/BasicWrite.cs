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

using Digimarc.NTiff.Tags;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Digimarc.NTiff;
using Xunit;

namespace Digimarc.NTiff.Test
{
    public class BasicWrite
    {
        [Fact]
        public void CanWriteTagPlaceholder()
        {
            var tag = new Tag
            {
                DataType = TagDataType.Short,
                ID = (ushort)BaselineTags.BitsPerSample,
                Length = 1
            };

            using (var stream = new TiffStreamWriter())
            {
                stream.WriteTagPlaceholder(tag);

                var data = stream.ToArray();
                Assert.True(data.SequenceEqual(new byte[] { 0x02, 0x01, 0x03, 0x00, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }));
            }

            using (var stream = new TiffStreamWriter(forceBigEndian: true))
            {
                stream.WriteTagPlaceholder(tag);

                var data = stream.ToArray();
                Assert.True(data.SequenceEqual(new byte[] { 0x01, 0x02, 0x00, 0x03, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x00 }));
            }
        }

        [Fact]
        public void WriteTagPlaceholderUpdatesOffset()
        {
            var tag = new Tag
            {
                DataType = TagDataType.ASCII,
                ID = (ushort)BaselineTags.Artist,
                Length = 1
            };

            using (var stream = new TiffStreamWriter())
            {
                stream.WriteTagPlaceholder(tag);
                stream.WriteTagPlaceholder(tag);

                Assert.Equal(24, stream.Length);
                Assert.Equal(12u, tag.Offset);
            }
        }

        [Fact]
        public void MyTestMethod()
        {
            char x = 'A';
            var b = (byte)(char)(object)x;
            Assert.True(b < 127);
        }

        [Fact]
        public void CanFinalizeShortTag()
        {
            var artist = "Bob";
            var tag = new Tag<char>
            {
                DataType = TagDataType.ASCII,
                ID = (ushort)BaselineTags.Artist,
                Length = (uint)artist.Length + 1,
                Values = artist.ToASCIIArray()
            };

            using (var stream = new TiffStreamWriter())
            {
                stream.WriteDWord(1);
                stream.WriteTagPlaceholder(tag);
                stream.FinalizeTag(tag);

                var data = stream.ToArray();

                Assert.Equal(4u, tag.Offset);
                Assert.Equal(4u, tag.Length);
                Assert.Equal(16, data.Length);
                Assert.Equal(artist, Encoding.ASCII.GetString(tag.RawValue).Trim('\0'));
            }
        }



        [Fact]
        public void CanFinalizeLongTag()
        {
            var artist = "Ansel Adams";
            var tag = new Tag<char>
            {
                DataType = TagDataType.ASCII,
                ID = (ushort)BaselineTags.Artist,
                Length = (uint)artist.Length + 1,
                Values = artist.ToASCIIArray()
            };           

            using (var stream = new TiffStreamWriter())
            {
                stream.WriteDWord(1);
                stream.WriteTagPlaceholder(tag);
                stream.FinalizeTag(tag);

                var data = stream.ToArray();

                Assert.Equal(4u, tag.Offset);
                Assert.Equal(12u, tag.Length);
                Assert.Equal(28, data.Length);
                Assert.True(tag.RawValue.SequenceEqual(BitConverter.GetBytes(16u)));
            }
        }

        [Fact]
        public void CanWriteWord()
        {
            using (var stream = new TiffStreamWriter())
            {
                stream.WriteWord(42);

                var data = stream.ToArray();
                Assert.True(data.SequenceEqual(new byte[] { 0x2a, 0x00 }));
            }

            using (var stream = new TiffStreamWriter(forceBigEndian: true))
            {
                stream.WriteWord(42);

                var data = stream.ToArray();
                Assert.True(data.SequenceEqual(new byte[] { 0x00, 0x2a }));
            }
        }

        [Fact]
        public void CanWriteDWord()
        {
            using (var stream = new TiffStreamWriter())
            {
                stream.WriteDWord(42);

                var data = stream.ToArray();
                Assert.True(data.SequenceEqual(new byte[] { 0x2a, 0x00, 0x00, 0x00 }));
            }

            using (var stream = new TiffStreamWriter(forceBigEndian: true))
            {
                stream.WriteDWord(42);

                var data = stream.ToArray();
                Assert.True(data.SequenceEqual(new byte[] { 0x00, 0x00, 0x00, 0x2a }));
            }
        }

        [Fact]
        public void CanWriteHeader()
        {
            using (var stream = new TiffStreamWriter())
            {
                stream.WriteHeader();

                var header = stream.ToArray();
                Assert.True(header.SequenceEqual(new byte[] { 0x49, 0x49, 0x2a, 0x00 }));
            }

            using (var stream = new TiffStreamWriter(forceBigEndian: true))
            {
                stream.WriteHeader();

                var header = stream.ToArray();
                Assert.True(header.SequenceEqual(new byte[] { 0x4d, 0x4d, 0x00, 0x2a }));
            }
        }
    }
}
