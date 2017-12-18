using NTiff.Tags;
using System;
using System.Collections.Generic;
using System.Text;

namespace NTiff.Types
{
    public class IFD
    {
        public List<Tag> Tags { get; set; } = new List<Tag>();
        public List<Strip> Strips { get; set; } = new List<Strip>();
        public List<Tag> Exif { get; set; }
    }
}
