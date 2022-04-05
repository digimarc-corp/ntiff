#!/usr/bin/env bash
#
# Builds and signs the ntiff nupkg
#
# usage:
# sign.sh -c <signing cert/pfx>

while getopts “c:p:” opt; do
  case $opt in
    c) cert=$OPTARG ;;
    p) password=$OPTARG ;;
  esac
done

if [ -z $cert ]; then
    echo "Must supply cert/pfx file."
    exit -1
fi

if [ -z $password ]; then    
    echo -n "Certificate password: "
    read -s password
fi

set -e

version=$(cat Digimarc.NTiff/Digimarc.NTiff.csproj | grep 'Version' | sed -E 's/.+Version>([0-9\.]+)<\/Version.*/\1/')
package="Digimarc.NTiff.$version.nupkg"
echo "Building NTiff $version ($package)"

dotnet clean -c Release
dotnet test -c Release
dotnet build -c Release

cp "./Digimarc.NTiff/bin/Release/$package" .

dotnet nuget sign "$package" --certificate-path "$cert" --certificate-password "$password" --timestamper "http://timestamp.sectigo.com"