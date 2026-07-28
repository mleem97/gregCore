# gregCore

> Profile-driven .NET 6 IL2CPP mod framework for **Data Center** with Harmony hooks, UI, persistence, scripting and stable mod APIs.

[![Discord](https://img.shields.io/badge/Discord-Join-5865F2?style=for-the-badge&logo=discord&logoColor=white)](https://discord.gg/greg)
[![gregFramework](https://img.shields.io/badge/gregFramework-Website-blue?style=for-the-badge)](https://gregframework.eu)
[![License](https://img.shields.io/badge/License-Apache%202.0-green?style=for-the-badge)](./LICENSE)
[![Version](https://img.shields.io/badge/Version-1.2.1-orange?style=for-the-badge)](./VERSION)
[![GameVersion](https://img.shields.io/badge/Game%20Version-1.0.50.15-yellow?style=for-the-badge)](./compat/current.json)
[![Unity](https://img.shields.io/badge/Unity-6000.5%20profile-black?style=for-the-badge&logo=unity&logoColor=white)](./compat/current.json)

## Status

`main` represents the newest tested reference profile. Compatibility is not inferred from the Unity major/minor version alone. gregCore records and verifies the complete runtime tuple:

```text
gregCore version
+ game build
+ Unity version
+ IL2CPP/reference fingerprint
+ loader and Il2CppInterop version
+ platform and architecture
```

The current development profile is:

```text
Data Center 1.0.50.15
Unity 6000.5 line
MelonLoader 0.7.x
Windows/Linux x64
```

The exact Unity patch version and SHA-256 values must still be captured from a verified local installation before the profile can be promoted to hash-verified/runtime-verified status. An unknown or mismatched runtime starts in **safe mode**: managed services, logging, configuration and diagnostics remain available while class injection and game-specific Harmony adapters are disabled.

## Loader support

| Loader | Status | Notes |
|---|---|---|
| MelonLoader 0.7.x | Current reference | Dedicated host currently shipped and tested by CI |
| BepInEx IL2CPP | Adapter planned | Not packaged or advertised as supported until a dedicated pinned host passes the same profile matrix |

A MelonLoader DLL placed under a BepInEx directory is not considered BepInEx support.

## Features

- Versioned compatibility profiles and runtime safe mode
- Full-signature, profile-aware Harmony hook resolution
- Lazy activation for high-frequency hooks
- Stable greg hook IDs independent of changing game method signatures
- Centralized, idempotent IL2CPP class injection
- UI Toolkit and UGUI integration
- Save engine with migrations
- Multi-mod dependency and event architecture
- Lua, JavaScript and Python bridges
- Custom shop, employee, rack and grid APIs
- Logging, diagnostics and performance governance
- Optional multiplayer integration

## Installation

### MelonLoader

1. Install the MelonLoader version declared by the selected compatibility profile.
2. Download the matching `gregCore-<version>-<profile>-melonloader-<platform>.zip` release.
3. Extract it into the game root.
4. Keep `gregCore.dll`, `game_hooks*.json` and `compat/` together under `Mods/gregCore/`.

Do not mix a DLL from one profile with hook manifests or compatibility files from another profile.

## Build from source

Requirements:

- .NET 6 SDK
- a legal local Data Center installation
- generated MelonLoader/Il2CppInterop reference assemblies

Set the reference root explicitly:

```bash
export GREG_REFERENCE_ROOT="/path/to/reference-pack"
dotnet restore gregCore.sln
dotnet build gregCore.sln -c Release -p:DeployToGameOnBuild=false
```

PowerShell:

```powershell
$env:GREG_REFERENCE_ROOT = "C:\path\to\reference-pack"
dotnet restore gregCore.sln
dotnet build gregCore.sln -c Release -p:DeployToGameOnBuild=false
```

The reference root must contain the assemblies named in `gregCore.csproj`, including `MelonLoader.dll`, `Il2CppInterop.Runtime.dll`, `Assembly-CSharp.dll` and the required Unity modules.

To capture sizes and SHA-256 hashes into a profile:

```bash
python scripts/capture_compat_profile.py \
  compat/profiles/datacenter-1.0.50.15-unity6000.5.json \
  --root /path/to/MelonLoader/Il2CppAssemblies \
  --root /path/to/MelonLoader/net6
```

Validate metadata without starting the game:

```bash
python scripts/validate_compat_profiles.py
python scripts/validate_hook_manifest.py game_hooks.json framework/game_hooks.v2.json
dotnet test tests/gregCore.Tests.csproj -c Release
```

## Architecture

```text
stable contracts
  gregCore.Abstractions (netstandard2.0)
  gregCore.SDK          (netstandard2.0)

managed framework
  gregCore.Core         (netstandard2.0 migration boundary)
  gregCore.Shared       (netstandard2.0 migration boundary)

runtime adapters
  gregCore.Mod
  gregCore.Hooks
  gregCore.Patches
  gregCore.Compatibility
  gregCore.Bridge
  gregCore.UI

legacy host
  gregCore.dll          (net6.0, retained during staged extraction)
```

The existing `gregCore.dll` remains the executable MelonLoader host while production types are moved gradually into the new assemblies. This avoids a flag-day namespace or binary break.

## Hook manifests

`framework/game_hooks.v2.json` maps stable greg hook IDs to one or more complete IL2CPP method candidates. Candidates include assembly, full type, method name, generic arity, static/instance state, return type and every parameter type. Unresolvable parameters invalidate the complete candidate; they are never silently removed.

The legacy `game_hooks.json` array remains readable during migration. Convert it deterministically with:

```bash
python scripts/convert_hook_manifest_v2.py \
  game_hooks.json framework/game_hooks.v2.generated.json \
  --profile-id datacenter-1.0.50.15-unity6000.5
```

## Version and branch policy

Normal pushes do not bump versions, create tags, publish releases or generate branches. Releases use the manual, profile-driven workflow.

- Current development: `main`
- Maintained line: `compat/u<unity>/game-<game>/gc-<major>.<minor>.x`
- Exact archive branch: `archive/u<unity>/game-<game>/gc-<version>`
- Immutable tag: `u<unity>-game<game>-gc<version>`

See:

- [`docs/VERSIONING_AND_BRANCHES.md`](docs/VERSIONING_AND_BRANCHES.md)
- [`docs/BACKWARD_COMPATIBILITY.md`](docs/BACKWARD_COMPATIBILITY.md)
- [`compat/README.md`](compat/README.md)

## Backward compatibility

The 1.x line keeps `AssemblyVersion` at `1.0.0.0`, treats the public API baseline as append-only, keeps stable hook IDs, uses additive payload/DTO changes and replaces broad assembly redirects with exact legacy facades or type forwarding.

## Repository layout

```text
compat/                      compatibility profiles and schema
framework/                   hook manifests and schemas
src/Core/                    current managed core and MelonLoader host
src/GameLayer/               IL2CPP/Harmony adapters
src/gregCore.*/              staged assembly boundaries
eng/PublicApi.Shipped.txt    1.x API baseline
scripts/                     profile, hook, release and branch tools
tests/                       managed compatibility and framework tests
.github/workflows/build.yml  validation/build CI only
.github/workflows/release.yml manual profile-driven release
```

## API documentation

See [`docs/FrameworkAPI.md`](docs/FrameworkAPI.md) for the generated hook reference.

## Contributing

See [`CONTRIBUTING.md`](CONTRIBUTING.md). Changes that affect game types or hooks must update or add a compatibility profile and pass the metadata validation jobs.

## License

Apache License 2.0. See [`LICENSE`](LICENSE).

## Contact

- Repository: [github.com/mleem97/gregCore](https://github.com/mleem97/gregCore)
- Discord: [discord.gg/greg](https://discord.gg/greg)
- Website: [gregframework.eu](https://gregframework.eu)
- Team applications: **apply@gregframework.eu**
