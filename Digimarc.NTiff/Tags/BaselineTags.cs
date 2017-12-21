using System;
using System.Collections.Generic;
using System.Text;

namespace Digimarc.NTiff.Tags
{
    /// <summary>
    /// Baseline TIFF tag IDs, as taken from https://www.awaresystems.be/imaging/tiff/tifftags/baseline.html
    /// </summary>
    public enum BaselineTags : ushort
    {
        NewSubfileType = 254,
        SubfileType = 255,
        ImageWidth = 256,
        ImageLength = 257,
        BitsPerSample = 258,
        Compression = 259,
        PhotometricInterpretation = 262,
        Threshholding = 263,
        CellWidth = 264,
        CellLength = 265,
        FillOrder = 266,
        ImageDescription = 270,
        Make = 271,
        Model = 272,
        StripOffsets = 273,
        Orientation = 274,
        SamplesPerPixel = 277,
        RowsPerStrip = 278,
        StripByteCounts = 279,
        MinSampleValue = 280,
        MaxSampleValue = 281,
        XResolution = 282,
        YResolution = 283,
        PlanarConfiguration = 284,
        FreeOffsets = 288,
        FreeByteCounts = 289,
        GrayResponseUnit = 290,
        GrayResponseCurve = 291,
        ResolutionUnit = 296,
        Software = 305,
        DateTime = 306,
        Artist = 315,
        HostComputer = 316,
        ColorMap = 320,
        ExtraSamples = 338,
        Copyright = 33432
    }
}
