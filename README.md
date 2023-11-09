# Dodoco.Core

A .NET library to manage Genshin Impact installation, updates and gameplay on Linux-based operating systems, written in C#.

Please keep in mind that this project is experimental and that the source code may change abruptly or even seem to make no sense.

## Build

### Requirements

- .NET SDK 7.0

### Clone

Some of the dependencies used by this project is not available in the NuGet's gallery and therefore cannot be installed by it. Clone this project's repository with `--recurse-submodules` flag to get these dependencies' source code directly from their GitHub repositories; the build scripts are already configured to reference them:

```sh
git clone --recurse-submodules https://github.com/Dodoco-Project/Dodoco.Core.git
```

Enter project's directory:

```sh
cd ./Dodoco.Core
```

### Build

Build the library:

```sh
dotnet build
```

### Test

Run all library's tests:

```sh
dotnet test
```

### Release

Creates a release build of the library (`Dodoco.Core.dll`) to `/Source/Dodoco.Core/bin/Release/net7.0/` directory:

```sh
dotnet publish -c Release
```

## Acknowledgements

This project is possible thanks to the following people and projects:

- [@neon-nyan](https://github.com/neon-nyan) — author of the [Hi3Helper.SharpHDiffPatch](https://github.com/neon-nyan/Hi3Helper.SharpHDiffPatch) library;
- [@jplane](https://github.com/jplane) — author of the [MultiStream](https://github.com/jplane/MultiStream) library;
- [@jean-lourenco](https://github.com/jean-lourenco) — author of the [UrlCombine](https://github.com/jean-lourenco/UrlCombine) library;
- The [contributors](https://github.com/devlooped/moq/graphs/contributors) of the [moq](https://github.com/devlooped/moq) library;
- The [contributors](https://github.com/aaubry/YamlDotNet/graphs/contributors) of the [YamlDotNet](https://github.com/aaubry/YamlDotNet) library;
- All the contributors from the [NUnit](https://nunit.org/) framework.

## License

Source code avaliable under [MIT](/LICENSE) license.