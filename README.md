# NTiff

|Platform|Status|
|---|---|
|Windows|[![Build Status](https://dev.azure.com/superstator/ntiff/_apis/build/status/digimarc-corp.ntiff?branchName=master&jobName=Windows)](https://dev.azure.com/superstator/ntiff/_build/latest?definitionId=1?branchName=master)|
|Linux|[![Build Status](https://dev.azure.com/superstator/ntiff/_apis/build/status/digimarc-corp.ntiff?branchName=master&jobName=Linux)](https://dev.azure.com/superstator/ntiff/_build/latest?definitionId=1?branchName=master)|
|MacOS|[![Build Status](https://dev.azure.com/superstator/ntiff/_apis/build/status/digimarc-corp.ntiff?branchName=master&jobName=Mac)](https://dev.azure.com/superstator/ntiff/_build/latest?definitionId=1?branchName=master)|

NTiff is a .NET Standard 2.0 native library for TIFF files. It is a clean rewrite based on the Adobe TIFF 6.0 specification and addenda, intended to avoid issues with metadata and extended tags that exist in `libtiff` and it's derivatives.

NTiff does not provide any image data decoding or manipulation functions; it only provides access to the raw tags and "image strips" data within a TIFF file. NTiff allows reading and writing of all TIFF tags, including private tags and others not in the base spec, while maintaining endianness and remaining agnostic about image data format, compression, etc.

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