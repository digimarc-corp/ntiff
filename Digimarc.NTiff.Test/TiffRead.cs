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
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Digimarc.NTiff.Test
{
    [TestClass]
    public class TiffRead
    {
        [TestMethod]
        public void CanLoadTiff()
        {
            var tiff = new Tiff(Samples.LAB);

            Assert.AreEqual(1, tiff.Images.Count);
            Assert.AreEqual(23, tiff.Images[0].Tags.Count);
            Assert.AreEqual(1, tiff.Images[0].Strips.Count);
            Assert.AreEqual(36, tiff.Images[0].Exif.Count);
        }

        [TestMethod]
        public void CanLoadPyramid()
        {
            var tiff = new Tiff(Samples.Pyramid);

            Assert.AreEqual(1, tiff.Images.Count);
            Assert.AreEqual(1, tiff.Images[0].SubImages.Count);
            Assert.AreEqual(1, tiff.Images[0].SubImages[0].Strips.Count);
            Assert.AreEqual(15, tiff.Images[0].SubImages[0].Tags.Count);
        }

        [TestMethod]
        public void CanLeaveStreamOpen()
        {
            using (var stream = new FileStream(Samples.LittleEndian, FileMode.Open, FileAccess.ReadWrite, FileShare.None))
            {
                var tiff = new Tiff(stream);
                Assert.IsTrue(stream.CanRead);
            }
        }
    }
}
