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
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Xunit;

namespace Digimarc.NTiff.Test
{
    public class TiffRead
    {
        [Fact]
        public void CanLoadTiff()
        {
            var tiff = new Tiff(SamplesList.LAB);

            Assert.Single(tiff.Images);
            Assert.Equal(23, tiff.Images[0].Tags.Count);
            Assert.Single(tiff.Images[0].Strips);
            Assert.Equal(36, tiff.Images[0].Exif.Count);
        }

        [Fact]
        public void CanLoadPyramid()
        {
            var tiff = new Tiff(SamplesList.Pyramid);

            Assert.Single(tiff.Images);
            Assert.Single(tiff.Images[0].SubImages);
            Assert.Single(tiff.Images[0].SubImages[0].Strips);
            Assert.Equal(15, tiff.Images[0].SubImages[0].Tags.Count);
        }

        [Fact]
        public void CanLeaveStreamOpen()
        {
            using (var stream = new FileStream(SamplesList.LittleEndian, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                var tiff = new Tiff(stream);
                Assert.True(stream.CanRead);
            }
        }
    }
}
