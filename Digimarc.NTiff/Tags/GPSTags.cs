using System;
using System.Collections.Generic;
using System.Text;

namespace Digimarc.NTiff.Tags
{
    /// <summary>
    /// GPS IFD tag IDs, taken from https://www.awaresystems.be/imaging/tiff/tifftags/privateifd/gps.html
    /// </summary>
    public enum GPSTags : ushort
    {
        GPSVersionID = 0,
        GPSLatitudeRef = 1,
        GPSLatitude = 2,
        GPSLongitudeRef = 3,
        GPSLongitude = 4,
        GPSAltitudeRef = 5,
        GPSAltitude = 6,
        GPSTimeStamp = 7,
        GPSSatellites = 8,
        GPSStatus = 9,
        GPSMeasureMode = 10,
        GPSDOP = 11,
        GPSSpeedRef = 12,
        GPSSpeed = 13,
        GPSTrackRef = 14,
        GPSTrack = 15,
        GPSImgDirectionRef = 16,
        GPSImgDirection = 17,
        GPSMapDatum = 18,
        GPSDestLatitudeRef = 19,
        GPSDestLatitude = 20,
        GPSDestLongitudeRef = 21,
        GPSDestLongitude = 22,
        GPSDestBearingRef = 23,
        GPSDestBearing = 24,
        GPSDestDistanceRef = 25,
        GPSDestDistance = 26,
        GPSProcessingMethod = 27,
        GPSAreaInformation = 28,
        GPSDateStamp = 29,
        GPSDifferential = 30
    }
}
