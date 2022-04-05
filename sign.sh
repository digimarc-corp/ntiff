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

# get timestamp and commit hash
date=$(date)
hash=`git rev-parse HEAD | cut -c 1-11`
if [ -z "$(git status --porcelain)" ]; then
  echo "Working directory clean, HEAD $hash"
else 
  echo 'Working directory dirty; commit changes before pushing a release'
  if [ "$force" != true ]; then
    exit -1
  fi
fi

version=$(cat Digimarc.NTiff/Digimarc.NTiff.csproj | grep 'Version' | sed -E 's/.+Version>([0-9\.]+)<\/Version.*/\1/')
package="Digimarc.NTiff.$version.nupkg"
echo "Building NTiff $version ($package)"

dotnet clean -c Release
dotnet test -c Release
dotnet build -c Release

cp "./Digimarc.NTiff/bin/Release/$package" .

dotnet nuget sign "$package" --certificate-path "$cert" --certificate-password "$password" --timestamper "http://timestamp.sectigo.com"