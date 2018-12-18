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
