#!/bin/bash

echo "Generating test files"

mkfile -n 5m small.file
mkfile -n 25m medium.file
mkfile -n 100m large.file
mkfile -n 500m huge.file

echo "Done"