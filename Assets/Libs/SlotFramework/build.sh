#!/bin/bash 
UNITY=/Applications/Unity/Unity.app/Contents/MacOS/Unity

BaseDir=$(pwd -L)
echo $BaseDir

ProjectDir=$(dirname $0)
echo $ProjectDir

Dir="$BaseDir/$ProjectDir"

cd $Dir
git pull

$UNITY -batchmode -logFile -projectPath $Dir -quit
