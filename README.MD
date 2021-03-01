# Getting started

## Create assets files

The application expectes 4 test files within the `FileServer/Assets` folder.

- small.file
- medium.file
- large.file
- huge.file

You can create those files with the according script within the `FileServer/Assets` folder.

- generate-test-files.sh (Linux/MacOS)
- generate-test-files.bat (Windows)

## Debug

For debugging the implementations you can run the FileServer via the launch.json entry `.NET Core Launch (web)`. Afterwards navigate a terminal to the `FileClient` directory and execute `dotnet run`.

## Benchmark

Navigate a terminal to the `FileServer.Benchmarks` directory. Execute the `dotnet publish -c Release -o ./pub` command. Under Linux and MacOS navigate to the newly created `FileServer.Benchmarks/pub` directory and execute `sudo dotnet FileServer.Benchmarks.dll`. Under Windows open an elevated command prompt (Open as Administrator), navigate to the `FileServer.Benchmarks/pub` directory and execute `dotnet FileServer.Benchmarks.dll`.
