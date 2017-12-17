using Microsoft.VisualStudio.TestTools.UnitTesting;
using NTiff.Tags;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NTiff.Test
{
    [TestClass]
    public class BasicWrite
    {
        [TestMethod]
        public void CanWriteTagPlaceholder()
        {
            var tag = new Tag
            {
                DataType = TagDataType.Short,
                ID = (ushort)BaselineTags.BitsPerSample,
                Length = 1
            };

            using (var stream = new TiffStream())
            {
                stream.WriteTagPlaceholder(tag);

                var data = stream.ToArray();
                Assert.IsTrue(data.SequenceEqual(new byte[] { 0x02, 0x01, 0x03, 0x00, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }));
            }

            using (var stream = new TiffStream(forceBigEndian: true))
            {
                stream.WriteTagPlaceholder(tag);

                var data = stream.ToArray();
                Assert.IsTrue(data.SequenceEqual(new byte[] { 0x01, 0x02, 0x00, 0x03, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x00 }));
            }
        }

        [TestMethod]
        public void WriteTagPlaceholderUpdatesOffset()
        {
            var tag = new Tag
            {
                DataType = TagDataType.ASCII,
                ID = (ushort)BaselineTags.Artist,
                Length = 1
            };

            using (var stream = new TiffStream())
            {
                stream.WriteTagPlaceholder(tag);
                stream.WriteTagPlaceholder(tag);

                Assert.AreEqual(24, stream.Length);
                Assert.AreEqual(12u, tag.Offset);
            }
        }

        [TestMethod]
        public void MyTestMethod()
        {
            char x = 'A';
            var b = (byte)(char)(object)x;
            Assert.IsTrue(b < 127);
        }

        [TestMethod]
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

            using (var stream = new TiffStream())
            {
                stream.WriteDWord(1);
                stream.WriteTagPlaceholder(tag);
                stream.FinalizeTag(tag);

                var data = stream.ToArray();

                Assert.AreEqual(4u, tag.Offset);
                Assert.AreEqual(4u, tag.Length);
                Assert.AreEqual(16, data.Length);
                Assert.AreEqual(artist, Encoding.ASCII.GetString(tag.RawValue).Trim('\0'));
            }
        }



        [TestMethod]
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

            using (var stream = new TiffStream())
            {
                stream.WriteDWord(1);
                stream.WriteTagPlaceholder(tag);
                stream.FinalizeTag(tag);

                var data = stream.ToArray();

                Assert.AreEqual(4u, tag.Offset);
                Assert.AreEqual(12u, tag.Length);
                Assert.AreEqual(28, data.Length);
                Assert.IsTrue(tag.RawValue.SequenceEqual(BitConverter.GetBytes(16u)));
            }
        }

        [TestMethod]
        public void CanWriteWord()
        {
            using (var stream = new TiffStream())
            {
                stream.WriteWord(42);

                var data = stream.ToArray();
                Assert.IsTrue(data.SequenceEqual(new byte[] { 0x2a, 0x00 }));
            }

            using (var stream = new TiffStream(forceBigEndian: true))
            {
                stream.WriteWord(42);

                var data = stream.ToArray();
                Assert.IsTrue(data.SequenceEqual(new byte[] { 0x00, 0x2a }));
            }
        }

        [TestMethod]
        public void CanWriteDWord()
        {
            using (var stream = new TiffStream())
            {
                stream.WriteDWord(42);

                var data = stream.ToArray();
                Assert.IsTrue(data.SequenceEqual(new byte[] { 0x2a, 0x00, 0x00, 0x00 }));
            }

            using (var stream = new TiffStream(forceBigEndian: true))
            {
                stream.WriteDWord(42);

                var data = stream.ToArray();
                Assert.IsTrue(data.SequenceEqual(new byte[] { 0x00, 0x00, 0x00, 0x2a }));
            }
        }

        [TestMethod]
        public void CanWriteHeader()
        {
            using (var stream = new TiffStream())
            {
                stream.WriteHeader();

                var header = stream.ToArray();
                Assert.IsTrue(header.SequenceEqual(new byte[] { 0x49, 0x49, 0x2a, 0x00 }));
            }

            using (var stream = new TiffStream(forceBigEndian: true))
            {
                stream.WriteHeader();

                var header = stream.ToArray();
                Assert.IsTrue(header.SequenceEqual(new byte[] { 0x4d, 0x4d, 0x00, 0x2a }));
            }
        }
    }
}
