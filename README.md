# gregCore

A .NET 6 MelonLoader mod framework for Data Center and similar Unity IL2CPP games.

## Product Name

The official product name is **gregCore**. This is a multi-project .NET 6 framework designed for MelonLoader-based modding.

## Project Purpose

This repository provides a modular modding framework for Unity-based games using MelonLoader. It includes hooks, patching systems, UI extensions, save handling, and core API services for building game modifications.

## Target Users

- Mod developers building extensions for Data Center or compatible IL2CPP Unity games
- Contributors familiar with C#, MelonLoader, and Harmony patching
- AI agents assisting with mod development workflows

## Major Capabilities

- Harmony-based runtime patching system
- UI overlay and widget management
- Save engine with versioning and migration support
- Multi-mod architecture with dependency resolution
- Wall rack and grid placement systems
- Custom shop and employee management APIs
- Logging and diagnostic infrastructure

## Repository Layout

```
GameFramework/
├─ src/                    # Mod source code (multiple independent mods)
├─ framework/              # Shared hook definitions
├─ lib/                   # Reference assemblies and dependencies
├─ tests/                 # Unit and integration tests
├─ build/                 # Build scripts and artifacts
├─ docs/                  # Documentation
├─ .github/               # GitHub workflows and templates
└─ [root files]          # Solution, project, build config
```

## Prerequisites

- .NET 6 SDK (net6.0)
- Visual Studio 2022+ or VS Code with C# extension
- MelonLoaderinstalled game (default: Data Center)
- Reference assemblies from target game (local paths required)

## Quick Start

```powershell
# Restore and build
./build.ps1

# Output goes to bin/Debug/net6.0/ or bin/Release/net6.0/
# Deploy DLL to your game's MelonLoader Mods folder
```

## Build Instructions

### Windows (PowerShell)

```powershell
./build.ps1
```

### Linux/macOS (Shell)

```bash
./build.sh
```

### Options

- `-Configuration Debug|Release` - Build configuration
- `-Clean` - Clean before build

## MelonLoader Deployment

Built DLLs (`gregCore.dll`) go into your game's `Mods` folder:

```
<Data Center>/Mods/gregCore.dll
```

Ensure reference DLLs (MelonLoader, Harmony, Il2CppInterop) are in the game's MelonLoader directory.

## IL2CPP and AI-Assisted Modding

> **Important**: When using AI for mod development against IL2CPP-based games, an IL2CPP unpack/decompilation workflow is strongly recommended.

AI-assisted modding works best when you have readable game references and type information. For practical reverse-engineering and inspection:

1. **Decompile or unpack** relevant game assemblies into a browsable C#-oriented reference project
2. Tools such as **dnSpy** or **dotPeek** may be useful in the inspection pipeline where applicable
3. Note that pure IL2CPP targets require additional metadata extraction steps beyond ordinary managed assembly inspection

A **future dedicated helper tool** from this project is planned to simplify the IL2CPP reference extraction process.

## Contributing

See [CONTRIBUTING.md](CONTRIBUTING.md) for contribution guidelines.

## Security

See [SECURITY.md](SECURITY.md) for vulnerability reporting.

## Compatibility

- .NET 6.0 (net6.0)
- MelonLoader compatible games
- Unity 2020.3+ (IL2CPP backend)
- Platform: Windows x64 (primary)

## License

See LICENSE file for details.