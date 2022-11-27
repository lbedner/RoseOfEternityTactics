#!/usr/bin/env bash

echo "Running Tests..."
EXECUTABLE="/Applications/Unity/Hub/Editor/2022.1.22f1/Unity.app/Contents/MacOS/Unity"
$EXECUTABLE -runTests -batchmode -projectPath ../ -testPlatform EditMode -logFile -
echo "Finished!"
