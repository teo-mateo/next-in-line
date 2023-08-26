#!/bin/bash

#echo the commands 
set -x

# Build script for the project

# Clean the project
#echo "Cleaning the project..."
#dotnet clean || exit 1

# Save current dir in variable
BUILD_ROOT=$PWD

# Build the project
echo "Building the project..."
cd $BUILD_ROOT/NextInLine.Server || exit 1

dotnet build || exit 1

# Cleanup the publish folder
echo "Cleaning the publish folder..."
rm -rf ./bin/Release/net67.0/linux-x64/publish/* || exit 1

# publish in release mode, for linux, and exit if errors #/p:PublishTrimmed=true
dotnet publish -c Release || exit 1  