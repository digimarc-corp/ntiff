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

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Digimarc.NTiff.Tags;
using Digimarc.NTiff.Types;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Digimarc.NTiff.Test
{
    public static class Samples
    {
        public const string LAB = "../../../../Samples/eagle_cap_lab.tif";
        public const string LittleEndian = "../../../../Samples/eagle_cap_le.tif";
        public const string Pyramid = "../../../../Samples/eagle_cap_pyramid.tif";
        public const string LZW = "../../../../Samples/eagle_cap_lzw.tif";
        public const string NoExif = "../../../../Samples/eagle_cap_noexif.tif";
        public const string Alpha = "../../../../Samples/eagle_cap_rgba.tif";

        public static string GetTemp() { return Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString() + ".tiff"); }
        public static void Cleanup(string filename) { try { if (File.Exists(filename)) File.Delete(filename); } catch { } }
    }

    // Runable/testable versions of sample code from README
    [TestClass]
    public class SampleTests
    {

        [TestMethod]
        public void ReadStandardTag()
        {
            var tiff = new Tiff(Samples.Alpha);
            var make = tiff.Images[0].Tags.Where(t => t.ID == (ushort)Tags.BaselineTags.Make).First().GetString();
            Assert.AreEqual(make.ToString(), "NIKON CORPORATION");
        }

        [TestMethod]
        public void ReadExifTag()
        {
            var tiff = new Tiff(Samples.Alpha);
            var tag = tiff.Images[0].Exif.Where(t => t.ID == (ushort)Tags.ExifTags.FocalLength).First();
            Assert.AreEqual(tag.ToString(), "FocalLength:Rational:1:18/1");
            var rational = tag.GetValue<Rational>(0);
            var dbl = rational.ToDouble();
        }

        [TestMethod]
        public void WritePrivateTag()
        {
            var payload = "some xml";

            // turn a .NET string into a nul-terminated ASCII character array
            var tag = new Tag<char>()
            {
                DataType = TagDataType.ASCII,
                ID = (ushort)PrivateTags.GDAL_METADATA,
                Values = payload.ToASCIIArray(),
                Length = (uint)payload.Length + 1 // nul terminated
            };

            var tiff = new Tiff(Samples.LittleEndian);
            tiff.Images[0].Tags.Add(tag);

            using (var stream = new MemoryStream())
            {
                tiff.Save(stream);
                stream.Seek(0, SeekOrigin.Begin);
                var newtiff = new Tiff(stream);
            }
        }
    }
}
