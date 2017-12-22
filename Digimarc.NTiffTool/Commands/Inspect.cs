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
