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

using Microsoft.Extensions.CommandLineUtils;
using Digimarc.NTiff;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Digimarc.NTiffTool.Commands
{
    class Inspect : CommandBase
    {      
        public Inspect()
        {
            Name = "inspect";
            Description = "Inspect a TIFF file, parsing and outputting metadata.";
            HelpOption("-?|-h|--help");

            var target = Argument("target", "Target file to inspect");

            OnExecute(() =>
            {
                OutLn(target.Value + ":");
                if (!File.Exists(target.Value)) { OutLn("File not found."); return -1; }
                else
                {
                    var fileInfo = new FileInfo(target.Value);
                    OutLn($"{fileInfo.Length} bytes");

                    var tiff = new Tiff(target.Value);
                    OutLn(tiff.IsBigEndian ? "Big Endian TIFF" : "Little Endian TIFF");
                    int i = 0;
                    foreach (var ifd in tiff.Images)
                    {
                        OutLn($"IFD{i:N0}:");
                        foreach (var tag in ifd.Tags)
                        {
                            OutLn($"\t{tag}");
                        }
                        if (ifd.Exif?.Count > 0)
                        {
                            OutLn("\tEXIF Data:");
                            foreach (var tag in ifd.Exif)
                            {
                                OutLn($"\t\t{tag}");
                            }
                        }
                        OutLn("\tImage data:");
                        foreach (var strip in ifd.Strips)
                        {
                            OutLn($"\t\tStrip {strip.StripNumber}: {strip.ImageData.Length:N0} bytes @ offset {strip.StripOffset}");
                        }
                        OutLn("End IFD\n");
                        i++;
                    }

                    return 0;
                }
            });
        }
    }
}
