using System;
using System.Collections.Generic;
using System.Text;

namespace NTiff.Tags
{
    /// <summary>
    /// EXIF IFD tag IDs, as taken from https://www.awaresystems.be/imaging/tiff/tifftags/privateifd/exif.html
    /// </summary>
    public enum ExifTags : ushort
    {
        ExposureTime = 33434,
        FNumber = 33437,
        ExposureProgram = 34850,
        SpectralSensitivity = 34852,
        ISOSpeedRatings = 34855,
        OECF = 34856,
        ExifVersion = 36864,
        DateTimeOriginal = 36867,
        DateTimeDigitized = 36868,
        ComponentsConfiguration = 37121,
        CompressedBitsPerPixel = 37122,
        ShutterSpeedValue = 37377,
        ApertureValue = 37378,
        BrightnessValue = 37379,
        ExposureBiasValue = 37380,
        MaxApertureValue = 37381,
        SubjectDistance = 37382,
        MeteringMode = 37383,
        LightSource = 37384,
        Flash = 37385,
        FocalLength = 37386,
        SubjectArea = 37396,
        MakerNote = 37500,
        UserComment = 37510,
        SubsecTime = 37520,
        SubsecTimeOriginal = 37521,
        SubsecTimeDigitized = 37522,
        FlashpixVersion = 40960,
        ColorSpace = 40961,
        PixelXDimension = 40962,
        PixelYDimension = 40963,
        RelatedSoundFile = 40964,
        FlashEnergy = 41483,
        SpatialFrequencyResponse = 41484,
        FocalPlaneXResolution = 41486,
        FocalPlaneYResolution = 41487,
        FocalPlaneResolutionUnit = 41488,
        SubjectLocation = 41492,
        ExposureIndex = 41493,
        SensingMethod = 41495,
        FileSource = 41728,
        SceneType = 41729,
        CFAPattern = 41730,
        CustomRendered = 41985,
        ExposureMode = 41986,
        WhiteBalance = 41987,
        DigitalZoomRatio = 41988,
        FocalLengthIn35mmFilm = 41989,
        SceneCaptureType = 41990,
        GainControl = 41991,
        Contrast = 41992,
        Saturation = 41993,
        Sharpness = 41994,
        DeviceSettingDescription = 41995,
        SubjectDistanceRange = 41996,
        ImageUniqueID = 42016
    }
}
