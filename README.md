# gregCore

A .NET 6 IL2CPP mod framework for MelonLoader and BepInEx — targeting Data Center and compatible Unity IL2CPP games.

## Overview

**gregCore** is a modular .NET 6 framework that provides:

- Harmony-based runtime patching system (Prefix / Postfix only — no IL transpilers in IL2CPP)
- UI overlay and widget management (UI Toolkit / UGUI)
- Save engine with versioning and migration (LiteDB)
- Multi-mod architecture with dependency resolution
- Wall rack and grid placement systems
- Custom shop and employee management APIs
- Logging and diagnostic infrastructure
- Lua, JS and Python scripting bridges
- FishNet multiplayer sync layer (optional)

## Compatibility

| Loader | Platform | Status |
|--------|----------|--------|
| MelonLoader 0.7+ | Windows x64 | ✅ Supported |
| MelonLoader 0.7+ | Linux x64 | ✅ Supported |
| BepInEx 6+ | Windows x64 | ✅ Supported |
| BepInEx 6+ | Linux x64 | ✅ Supported |

- .NET 6.0 (net6.0)
- Unity 2020.3+ (IL2CPP backend)

## CI / Releases

Every push to `main` automatically:

1. **Bumps the patch version** (`X.Y.Z → X.Y.Z+1`) in `VERSION`, `gregCore.csproj` and `GregCoreMod.cs`
2. **Builds** four release packages: MelonLoader-Windows, MelonLoader-Linux, BepInEx-Windows, BepInEx-Linux
3. **Publishes** a GitHub Release with all four ZIPs attached
4. **Regenerates** `docs/FrameworkAPI.md` when `game_hooks.json` or `framework/greg_hooks.json` change

No tag required. No manual trigger required. Fully automatic.

## Installation

### MelonLoader

1. Download `gregCore-vX.Y.Z-melonloader-windows.zip` (or `-linux.zip`)
2. Extract into your game's root folder
3. Your `Mods/` folder will contain `gregCore.dll`

### BepInEx

1. Download `gregCore-vX.Y.Z-bepinex-windows.zip` (or `-linux.zip`)
2. Extract into your game's root folder
3. `BepInEx/plugins/gregCore/gregCore.dll` is placed automatically

## Repository Layout

```
gregCore/
├─ src/                    # Framework + mod source code
│  ├─ Core/               # GregCoreMod.cs – MelonLoader entry point
│  ├─ Infrastructure/     # Config, logging, persistence
│  ├─ GameLayer/          # Harmony patches for game classes
│  ├─ UI/                 # UI Toolkit overlay
│  └─ …
├─ framework/              # greg_hooks.json – canonical hook registry
├─ game_hooks.json         # Patchable methods from IL2CPP dump
├─ lib/                   # Reference assemblies (game stubs, MelonLoader, etc.)
├─ docs/                  # Auto-generated API docs (FrameworkAPI.md)
├─ scripts/               # Build and code-generation helpers
├─ tests/                 # Unit tests
├─ .github/workflows/     # CI pipeline
├─ VERSION                # Single source of truth for version
└─ gregCore.csproj
```

## Prerequisites (local development)

- .NET 6 SDK
- Visual Studio 2022+ or VS Code with C# extension
- Game reference assemblies in `lib/references/MelonLoader/` (not committed; see below)

## Building Locally

```powershell
# Windows
./build.ps1

# Linux / macOS
./build.sh
```

Output goes to `bin/Release/net6.0/gregCore.dll`.

## Reference Assemblies

The `lib/references/MelonLoader/` folder is **not** committed (`.gitignore`).
Populate it from your local game install:

1. Run the game with MelonLoader once to generate IL2CPP assemblies
2. Copy `MelonLoader/Il2CppAssemblies/` and `MelonLoader/net6/` into `lib/references/MelonLoader/`

For CI, dummy stubs are created by `ci-stubs/create-stubs.sh` so the build
succeeds without the real game binaries.

## Updating Hook Definitions

When the game updates:

```bash
# 1. Re-run Cpp2IL / Il2CppDumper on the new GameAssembly
./scripts/Generate-GregHooksFromIl2CppDump.ps1

# 2. Commit the updated JSONs
git add game_hooks.json framework/greg_hooks.json
git commit -m "chore: update game hooks for vX.Y"
git push
# → CI automatically regenerates docs/FrameworkAPI.md
```

## API Documentation

See [`docs/FrameworkAPI.md`](docs/FrameworkAPI.md) for the auto-generated hook reference.

## Contributing

See [CONTRIBUTING.md](CONTRIBUTING.md) for guidelines.

## License

See [LICENSE](LICENSE) for details.