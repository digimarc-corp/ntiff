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
using System.Text;

namespace Digimarc.NTiff.Types
{
    public class Image
    {
        public List<Tag> Tags { get; set; } = new List<Tag>();
        public List<Strip> Strips { get; set; } = new List<Strip>();
        public List<Image> SubImages { get; set; } = new List<Image>();
        public List<Tag> Exif { get; set; }
    }
}
