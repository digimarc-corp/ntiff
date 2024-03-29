﻿/*
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
    /// TIFF Private Tag IDs, as taken from https://www.awaresystems.be/imaging/tiff/tifftags/private.html
    /// </summary>
    public enum PrivateTags : ushort
    {
        WangAnnotation = 32932,
        MDFileTag = 33445,
        MDScalePixel = 33446,
        MDColorTable = 33447,
        MDLabName = 33448,
        MDSampleInfo = 33449,
        MDPrepDate = 33450,
        MDPrepTime = 33451,
        MDFileUnits = 33452,
        ModelPixelScaleTag = 33550,
        IPTC = 33723,
        INGRPacketDataTag = 33918,
        INGRFlagRegisters = 33919,
        IrasBTransformationMatrix = 33920,
        ModelTiepointTag = 33922,
        ModelTransformationTag = 34264,
        Photoshop = 34377,
        ExifIFD = 34665,
        ICCProfile = 34675,
        GeoKeyDirectoryTag = 34735,
        GeoDoubleParamsTag = 34736,
        GeoAsciiParamsTag = 34737,
        GPSIFD = 34853,
        HylaFAXFaxRecvParams = 34908,
        HylaFAXFaxSubAddress = 34909,
        HylaFAXFaxRecvTime = 34910,
        ImageSourceData = 37724,
        InteroperabilityIFD = 40965,
        GDAL_METADATA = 42112,
        GDAL_NODATA = 42113,
        OceScanjobDescription = 50215,
        OceApplicationSelector = 50216,
        OceIdentificationNumber = 50217,
        OceImageLogicCharacteristics = 50218,
        DNGVersion = 50706,
        DNGBackwardVersion = 50707,
        UniqueCameraModel = 50708,
        LocalizedCameraModel = 50709,
        CFAPlaneColor = 50710,
        CFALayout = 50711,
        LinearizationTable = 50712,
        BlackLevelRepeatDim = 50713,
        BlackLevel = 50714,
        BlackLevelDeltaH = 50715,
        BlackLevelDeltaV = 50716,
        WhiteLevel = 50717,
        DefaultScale = 50718,
        DefaultCropOrigin = 50719,
        DefaultCropSize = 50720,
        ColorMatrix1 = 50721,
        ColorMatrix2 = 50722,
        CameraCalibration1 = 50723,
        CameraCalibration2 = 50724,
        ReductionMatrix1 = 50725,
        ReductionMatrix2 = 50726,
        AnalogBalance = 50727,
        AsShotNeutral = 50728,
        AsShotWhiteXY = 50729,
        BaselineExposure = 50730,
        BaselineNoise = 50731,
        BaselineSharpness = 50732,
        BayerGreenSplit = 50733,
        LinearResponseLimit = 50734,
        CameraSerialNumber = 50735,
        LensInfo = 50736,
        ChromaBlurRadius = 50737,
        AntiAliasStrength = 50738,
        DNGPrivateData = 50740,
        MakerNoteSafety = 50741,
        CalibrationIlluminant1 = 50778,
        CalibrationIlluminant2 = 50779,
        BestQualityScale = 50780,
        AliasLayerMetadata = 50784
    }
}
