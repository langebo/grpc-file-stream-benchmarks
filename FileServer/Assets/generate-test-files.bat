@ECHO OFF

ECHO Generating test files

fsutil file createnew small.file 5242880
fsutil file createnew medium.file 26214400
fsutil file createnew large.file 104857600
fsutil file createnew huge.file 524288000

echo Done

