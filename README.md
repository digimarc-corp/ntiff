# NTiff

[![Build Status](https://dev.azure.com/superstator/ntiff/_apis/build/status/digimarc-corp.ntiff?branchName=master)](https://dev.azure.com/superstator/ntiff/_build/latest?definitionId=1?branchName=master) ![Nuget Version](https://img.shields.io/nuget/v/Digimarc.NTiff.svg)

NTiff is a .NET Standard 2.0 native library for TIFF files. It is a clean rewrite based on the Adobe TIFF 6.0 specification and addenda, intended to avoid issues with metadata and extended tags that exist in `libtiff` and it's derivatives.

NTiff does not provide any image data decoding or manipulation functions; it only provides access to the raw tags and "image strips" data within a TIFF file. NTiff allows reading and writing of all TIFF tags, including private tags and others not in the base spec, while maintaining endianness and remaining agnostic about image data format, compression, etc.

## Usage

**Read a single standard tag from a file:**

```c#
var tiff = new Tiff("photo.tif");
Console.Write(tiff.Images[0].Tags.Where(t => t.ID == (ushort)BaselineTags.Make).First().GetString());
// NIKON CORPORATION
```

Note that the `ID` property of a `Tag` object is typed as a `ushort`. This allows NTiff to support any custom tag ID you may encounter without a lot of extra work. Just look for your specific ID, or create your own enum to test against.

**Read an EXIF tag from an image:**

```c#
var tiff = new Tiff(@"photo.tif");
var tag = tiff.Images[0].Exif.Where(t => t.ID == (ushort)ExifTags.FocalLength).First();
var value = tag.GetValue<Rational>(0).ToDouble()

Console.WriteLine(tag);
Console.WriteLine(value);
// FocalLength:Rational:1:18/1
// 18
```

When taking a basic string representation of a tag via `ToString()`, you get several values separated by colons. The first value is the tag name (if known) or ID. The second is the basic TIFF datatype. The third is the length of the tag value - this is the number of discrete values for types like `Rational` or `Short`, or length for a `byte` or `ASCII` value. Rationals are given in a raw numerator/denominator format, and not simplified in any way unless you call the `ToDouble()` method.

**Add a private tag to an existing file, and save it to a new stream:**

```c#
var payload = "some xml";

// turn a .NET string into a nul-terminated ASCII character array
var tag = new Tag<char>() {
    DataType = TagDataType.ASCII,
    ID = (ushort)PrivateTags.GDAL_METADATA,
    Values = payload.ToASCIIArray(),
    Length = (uint)payload.Length + 1
};

var tiff = new Tiff(@"my_file.tiff");
tiff.Images[0].Tags.Add(tag);

using (var stream = new MemoryStream()){
    tiff.Save(stream);
    stream.Seek(0, SeekOrigin.Begin);
    var newtiff = new Tiff(stream);
}
```
*todo: examples for complex operations like custom IFDs or image strip manipulation*

## Build

NTiff is built as a .NET Standard library, meaning it should be portable across any platform supported by .NET Core or .NET Framework.

To build, go to the `Digimarc.NTiff` folder and run `dotnet build`.

A NuGet package is generated as part of the primary build.

## Tools

Digimarc.NTiffTool provides a command-line interface for inspecting a TIFF file and importing/exporting metadata like EXIF.

## Attribution

NTiff links to or uses source code from the following projects:

- [Magick.NET](https://github.com/dlemstra/Magick.NET) - [Apache-2.0](http://www.apache.org/licenses/LICENSE-2.0.html)
- [vstest](https://github.com/microsoft/vstest/) - [MIT](https://opensource.org/licenses/MIT)

Sample/test images:

- ["Mirror Lake"](https://www.flickr.com/photos/araddon/3794400754/) - [araddon](https://www.flickr.com/people/araddon/) - [CC BY 2.0](https://creativecommons.org/licenses/by/2.0/)