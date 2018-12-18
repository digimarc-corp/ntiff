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

namespace Digimarc.NTiff
{
    public static class ExtensionMethods
    {
        /// <summary>
        /// Gets the length, in bytes, of a given TIFF tag datatype.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static int AtomicLength(this TagDataType type)
        {
            switch (type)
            {
                case TagDataType.ASCII:
                case TagDataType.Byte:
                case TagDataType.SByte:
                case TagDataType.Undefined:
                    return 1;
                case TagDataType.Short:
                case TagDataType.SShort:
                    return 2;
                case TagDataType.Float:
                case TagDataType.Long:
                case TagDataType.SLong:
                case TagDataType.IFD:
                    return 4;
                case TagDataType.Double:
                case TagDataType.Rational:
                case TagDataType.SRational:
                    return 8;
                default:
                    throw new ArgumentException($"Unknown tag datatype {type}");
            }
        }

        /// <summary>
        /// Returns a string as a nul-terminated ASCII char array
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static char[] ToASCIIArray(this string str)
        {
            return (str + "\0").ToCharArray();
        }

        public static string PrettyPrint(this byte[] bytes, int maxLength = 20)
        {
            var str = BitConverter.ToString(bytes).Replace("-", "").ToLower();
            if (str.Length > maxLength)
            {
                str = str.Substring(0, 20) + "...";
            }
            return str;
        }
    }
}
