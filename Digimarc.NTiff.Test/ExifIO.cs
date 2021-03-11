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
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Xunit;

namespace Digimarc.NTiff.Test
{
    public class ExifIO
    {
        [Fact]
        public void CanReadExifBlock()
        {
            using (var stream = new FileStream(Samples.LAB, FileMode.Open, FileAccess.Read, FileShare.Read))
            {

                var tiffStream = new TiffStreamReader(stream);
                var exif = tiffStream.ParseIFD(2991224);

                Assert.Equal(36, exif.tags.Length);
                Assert.Equal(0u, exif.nextIfd);
                Assert.Equal((short)400, exif.tags.Where(t => t.ID == (ushort)Tags.ExifTags.ISOSpeedRatings).First().GetValue<short>(0));
            }
        }
    }
}
