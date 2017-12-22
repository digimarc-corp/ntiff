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
