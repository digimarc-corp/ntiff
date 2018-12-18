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
using System.Text;

namespace Digimarc.NTiff.Test
{
    public static class Samples
    {
        public const string LAB = "../../../../Samples/eagle_cap_lab.tif";
        public const string LittleEndian = "../../../../Samples/eagle_cap_le.tif";
        public const string Pyramid = "../../../../Samples/eagle_cap_pyramid.tif";
        public const string LZW = "../../../../Samples/eagle_cap_lzw.tif";
        public const string NoExif = "../../../../Samples/eagle_cap_noexif.tif";

        public static string GetTemp() { return Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString() + ".tiff"); }
        public static void Cleanup(string filename) { try { if (File.Exists(filename)) File.Delete(filename); } catch { } }
    }
}
