#!/bin/sh

if [ "$(basename $(pwd))" = "scripts" ]; then
node ./release.js $1
else 
node ./scripts/release.js $1
fi