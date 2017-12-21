using Digimarc.NTiff;
using Digimarc.NTiff.Tags;
using Digimarc.NTiff.Types;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Digimarc.NTiffTool
{
    class Compare : CommandBase
    {
        int Differences = 0;

        public Compare()
        {
            Name = "compare";
            Description = "Compare two TIFF files and output differences.";
            HelpOption("-?|-h|--help");

            var target1 = Argument("target", "First target file to compare");
            var target2 = Argument("target", "Second target file to compare");

            OnExecute(() =>
            {
                OutLn(target1.Value + " : " + target2.Value);
                if (!File.Exists(target1.Value)) { OutLn($"File not found - {target1.Value}"); return -1; }
                if (!File.Exists(target2.Value)) { OutLn($"File not found - {target2.Value}"); return -1; }
                else
                {
                    var tiff1 = new Tiff(target1.Value);
                    var tiff2 = new Tiff(target2.Value);
                    OutLn($"1: {target1.Value}");
                    OutLn($"2: {target2.Value}");
                    if (tiff1.IsBigEndian != tiff2.IsBigEndian)
                    {
                        Differences++;
                        OutLn("Endian mismatch.");
                        OutLn($"1: {(tiff1.IsBigEndian ? "big endian" : "little endian")}");
                        OutLn($"2: {(tiff2.IsBigEndian ? "big endian" : "little endian")}");
                    }
                    if (tiff1.Images.Count != tiff2.Images.Count)
                    {
                        Differences++;
                        OutLn("IFD count mismatch.");
                        OutLn($"1: {tiff1.Images.Count:N0} IFD(s)");
                        OutLn($"2: {tiff2.Images.Count:N0} IFD(s)");
                    }
                    for (int i = 0; i < Math.Min(tiff1.Images.Count, tiff2.Images.Count); i++)
                    {
                        OutLn($"\tIFD{i:N0}:");
                        var ifd1 = tiff1.Images[i];
                        var ifd2 = tiff2.Images[i];
                        CompareTags(ifd1.Tags, ifd2.Tags, "\t\t");
                        if (ifd1.Exif?.Count > 0 || ifd2.Exif?.Count > 0)
                        {
                            OutLn("\tExif Data:");
                            CompareTags(ifd1.Exif, ifd2.Exif, "\t\t");
                        }
                        OutLn("\tImage Data:");
                        for (int s = 0; s < Math.Max(ifd1.Strips.Count, ifd2.Strips.Count); s++)
                        {
                            CompareStrip(ifd1, ifd2, s);                            
                        }
                        OutLn("\tEnd IFD\n");
                    }

                    OutLn(Differences > 0 ? $"{Differences:N0} differences found." : "File data is identical.");

                    return 0;
                }
            });
        }

        private void CompareStrip(Image ifd1, Image ifd2, int stripNumber)
        {
            var strip1 = stripNumber < ifd1.Strips.Count ? ifd1.Strips[stripNumber] : null;
            var strip2 = stripNumber < ifd2.Strips.Count ? ifd2.Strips[stripNumber] : null;

            var hash1 = strip1?.GetHash() ?? "";
            var hash2 = strip2?.GetHash() ?? "";

            if (hash1 != hash2)
            {
                Differences++;
                OutLn($"\t\t1: Strip {stripNumber} : {strip1.ImageData.Length:N0} bytes ({hash1})");
                OutLn($"\t\t2: Strip {stripNumber} : {strip2.ImageData.Length:N0} bytes ({hash2})");
            }
        }

        private void CompareTags(IEnumerable<Tag> set1, IEnumerable<Tag> set2, string prefix = "")
        {
            set1 = set1 ?? new Tag[0];
            set2 = set2 ?? new Tag[0];
            var tagSuperset = set1.Select(t => t.ID).Union(set2.Select(t => t.ID));
            foreach (var id in tagSuperset.OrderBy(t => t))
            {
                CompareTag(id, set1, set2, prefix);
            }
        }

        private void CompareTag(ushort id, IEnumerable<Tag> set1, IEnumerable<Tag> set2, string prefix = "")
        {
            var tag1 = set1.FirstOrDefault(t => t.ID == id);
            var tag2 = set2.FirstOrDefault(t => t.ID == id);
            if (tag1?.ToString() != tag2?.ToString())
            {
                Differences++;
                OutLn($"{prefix}1: {(tag1?.ToString() ?? "<missing>")}");
                OutLn($"{prefix}2: {(tag2?.ToString() ?? "<missing>")}");
            }
        }
    }
}
