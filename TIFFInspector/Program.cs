using System;
using System.IO;
using System.Linq;
using TIFFTools;

namespace TIFFInspector
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 1)
            {
                using (var stream = new FileStream(args[0], FileMode.Open))
                {
                    var header = stream.ReadTiffHeader();
                    Console.WriteLine("Read tiff header.");
                    Console.WriteLine($"IFD0 at {header.IFD0}");
                    var tags = stream.ReadIFD(header.IFD0).ToList();
                    Console.WriteLine($"{tags.Count} tags in IFD0");
                    foreach (var tag in tags) { Console.WriteLine($"{tag.TagID}:\t{tag.Datatype}\t\t{tag.Length}:\t{tag.Data}"); }
                    var exifPointer = tags.Where(t => t.TagID == Tiff.ExifPointerTagID).FirstOrDefault();
                    if (exifPointer != null)
                    {
                        Console.WriteLine($"Found Exif pointer: {exifPointer.Data}");
                        var exif = stream.ReadIFD(exifPointer.Data).ToList();
                        Console.WriteLine($"Read {exif.Count} tags in Exif IFD.");
                        foreach (var tag in exif) { Console.WriteLine($"{tag.TagID}:\t{tag.Datatype}\t\t{tag.Length}:\t{tag.Data}"); }
                    }
                }
            }
            else if (args.Length == 2)
            {
                var buffer = new ExifBuffer();
                using (var source = new FileStream(args[0], FileMode.Open))
                {
                    buffer.Read(source);
                }

                /*
                var stream = new MemoryStream();
                stream.Write(new byte[] { 0x01, 0x02, 0x01, 0x02, 0x03, 0x04 }, 0, 6);
                ExifBuffer.CopyExif(new MemoryStream(buffer.ExifBlock), 0, stream, 6);
                var bytes = stream.ToArray();*/

                using (var dest = new FileStream(args[1], FileMode.Open))
                {
                    buffer.Write(dest, 318); // overwriting WhitePoint as a quick and dirty test
                }
            }
        }
    }
}
