﻿/*
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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Xunit;

namespace Digimarc.NTiff.Test
{
    public class TiffWrite
    {
        [Fact]
        public void CanAddExifToStrippedFile()
        {
            var temp = SamplesList.GetTemp();
            try
            {
                var tiff1 = new Tiff(SamplesList.LAB);
                var exif = tiff1.Images[0].Exif;

                var tiff2 = new Tiff(SamplesList.NoExif);
                Assert.Null(tiff2.Images[0].Exif);

                tiff2.Images[0].Exif = exif;
                Assert.Equal(36, tiff2.Images[0].Exif.Count);

                tiff2.Save(temp);

                var tiff3 = new Tiff(temp);
                Assert.Equal(tiff2.Images[0].Strips[0].GetHash(), tiff3.Images[0].Strips[0].GetHash());
            }
            finally
            {
                SamplesList.Cleanup(temp);
            }
        }

        [Fact]
        public void CanAddTiffTag()
        {
            var tiff1 = new Tiff(SamplesList.LittleEndian);
            tiff1.Images[0].Tags.Add(new Tag<byte>()
            {
                DataType = TagDataType.Byte,
                ID = (ushort)PrivateTags.AliasLayerMetadata,
                Values = Encoding.UTF8.GetBytes("Hello world"),
                Length = 11
            });
            var temp = SamplesList.GetTemp();
            try
            {
                tiff1.Save(temp);

                var tiff2 = new Tiff(temp);
                Assert.Equal(tiff1.Images[0].Tags.Count, tiff2.Images[0].Tags.Count);
                Assert.Equal(tiff1.Images[0].Strips[0].GetHash(), tiff2.Images[0].Strips[0].GetHash());
            }
            finally
            {
                SamplesList.Cleanup(temp);
            }
        }

        [Fact]
        public void CanCombineTiffs()
        {
            var tiff1 = new Tiff(SamplesList.LittleEndian);
            var tiff2 = new Tiff(SamplesList.LZW);
            Assert.Equal(tiff1.IsBigEndian, tiff2.IsBigEndian);
            var temp = SamplesList.GetTemp();
            try
            {
                var tempTiff = new Tiff();
                tempTiff.Images.Add(tiff1.Images[0]);
                tempTiff.Images.Add(tiff2.Images[0]);
                tempTiff.Save(temp);

                System.Threading.Thread.Sleep(1000);

                var newTiff = new Tiff(temp);
                Assert.Equal(2, newTiff.Images.Count);
            }
            finally
            {
                SamplesList.Cleanup(temp);
            }
        }

        [Theory]
        [InlineData(SamplesList.LAB)]
        [InlineData(SamplesList.LittleEndian)]
        [InlineData(SamplesList.Bilevel)]
        public void CanRewrite(string filename) => CheckRewrite(filename);

        [Theory]
        [InlineData(SamplesList.Pyramid)]
        public void ThrowsOnUnsupportedWrite(string filename) => Assert.Throws<TiffWriteException>(() => CheckRewrite(filename));

        [Fact]
        public void CanWriteStream()
        {
            string hash1, hash2;
            using (var stream = new MemoryStream())
            {
                var tiff = new Tiff(SamplesList.LAB);
                tiff.Save(stream);
                stream.Seek(0, SeekOrigin.Begin);
                hash1 = Convert.ToBase64String(System.Security.Cryptography.MD5.Create().ComputeHash(stream));
            }
            hash2 = CheckRewrite(SamplesList.LAB);

            Assert.Equal(hash1, hash2);
        }

        /// <summary>
        /// Read and parse a sample file from disk, write it back out to a temp file, and verify metadata and properties via NTiff & ImageMagick. Returns hash of final output.
        /// </summary>
        /// <param name="src"></param>
        private static string CheckRewrite(string src)
        {
            var tiff = new Tiff(src);
            var temp = SamplesList.GetTemp();
            try
            {
                tiff.Save(temp);

                var newImg = new Tiff(temp);
                Assert.Equal(tiff.Images[0].Tags.Count, newImg.Images[0].Tags.Count);
                Assert.Equal(tiff.Images[0].Exif.Count, newImg.Images[0].Exif.Count);

                // Load source and temp in ImageMagick and compare properties
                var srcImg = new ImageMagick.MagickImage(src);
                var tempImg = new ImageMagick.MagickImage(temp);

                var srcAttributes = srcImg.AttributeNames;
                var tempAttributes = tempImg.AttributeNames;

                Assert.Equal(srcImg.Signature, tempImg.Signature);
                Assert.True(srcAttributes.SequenceEqual(tempAttributes));
                Assert.Equal(srcAttributes.Count(), tempAttributes.Count());

                foreach (var attr in srcAttributes)
                {
                    if (attr == "date:create" || attr == "date:modify") { continue; }
                    Assert.Equal(srcImg.GetAttribute(attr), tempImg.GetAttribute(attr));
                }

                return Convert.ToBase64String(System.Security.Cryptography.MD5.Create().ComputeHash(File.ReadAllBytes(temp)));
            }
            finally
            {
                SamplesList.Cleanup(temp);
            }
        }
    }
}
