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
    /// TIFF Extension tag IDs, taken from https://www.awaresystems.be/imaging/tiff/tifftags/extension.html
    /// JPEG tags invalidated by Tech Note 2 are intentionally omitted.
    /// </summary>
    public enum ExtensionTags : ushort
    {
        DocumentName = 269,
        PageName = 285,
        XPosition = 286,
        YPosition = 287,
        T4Options = 292,
        T6Options = 293,
        PageNumber = 297,
        TransferFunction = 301,
        Predictor = 317,
        WhitePoint = 318,
        PrimaryChromaticities = 319,
        HalftoneHints = 321,
        TileWidth = 322,
        TileLength = 323,
        TileOffsets = 324,
        TileByteCounts = 325,
        BadFaxLines = 326,
        CleanFaxData = 327,
        ConsecutiveBadFaxLine = 328,
        SubIFDs = 330,
        InkSet = 332,
        InkNames = 333,
        NumberOfInks = 334,
        DotRange = 336,
        TargetPrinter = 337,
        SampleFormat = 339,
        SMinSampleValue = 340,
        SMaxSampleValue = 341,
        TransferRange = 342,
        ClipPath = 343,
        XClipPathUnits = 344,
        YClipPathUnits = 345,
        Indexed = 346,
        JPEGTables = 347,
        OPIProxy = 351,
        GlobalParametersIFD = 400,
        ProfileType = 401,
        FaxProfile = 402,
        CodingMethods = 403,
        VersionYear = 404,
        ModeNumber = 405,
        Decode = 433,
        DefaultImageColor = 434,
        YCbCrCoefficients = 529,
        YCbCrSubSampling = 530,
        YCbCrPositioning = 531,
        ReferenceBlackWhite = 532,
        StripRowCounts = 559,
        XMP = 700,
        ImageID = 32781,
        ImageLayer = 34732
    }
}
