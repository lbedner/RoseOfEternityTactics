#!/bin/sh

EXECUTABLE="/Applications/Unity/Hub/Editor/2022.1.22f1/Unity.app/Contents/MacOS/Unity"

#echo "Sanitizing Build Directory..."
#rm -rf ../Builds/

echo "Creating MacOSX Build..."
$EXECUTABLE -quit -batchmode -projectPath .. -logFile - executeMethod BuildScript.BuildStandaloneOSX
echo "Finished!!!"
