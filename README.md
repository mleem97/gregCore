# gregCore

> Modular .NET 6 IL2CPP mod framework for **Data Center** — Harmony patching, UI overlays, save engine, scripting, and multi-mod architecture.

[![Discord](https://img.shields.io/badge/Discord-Join-5865F2?style=for-the-badge&logo=discord&logoColor=white)](https://discord.gg/greg)
[![gregFramework](https://img.shields.io/badge/gregFramework-Website-blue?style=for-the-badge)](https://gregframework.eu)
[![License](https://img.shields.io/badge/License-Apache%202.0-green?style=for-the-badge)](./LICENSE)
[![Version](https://img.shields.io/badge/Version-1.2.2--dev.0-orange?style=for-the-badge)]()
[![GameVersion](https://img.shields.io/badge/Game%20Version-1.0.50.15-yellow?style=for-the-badge)]()
[![Unity](https://img.shields.io/badge/Unity-6000.5-black?style=for-the-badge&logo=unity&logoColor=white)]()

## Links

- **Repository:** [github.com/mleem97/gregCore](https://github.com/mleem97/gregCore)
- **Discord / Support:** [discord.gg/greg](https://discord.gg/greg)
- **Website:** [gregframework.eu](https://gregframework.eu)

## Overview

**gregCore** is a modular .NET 6 framework for **Data Center** that provides Harmony-based runtime patching, UI overlay management, save engine with versioning, multi-mod architecture with dependency resolution, scripting bridges (Lua, JS, Python), and more.

## Compatibility

| Loader | Platform | Status |
|--------|----------|--------|
| MelonLoader 0.7+ | Windows x64 | Supported |
| MelonLoader 0.7+ | Linux x64 | Supported |
| BepInEx 6+ | Windows x64 | Supported |
| BepInEx 6+ | Linux x64 | Supported |

## Features

- Harmony-based runtime patching system (Prefix / Postfix)
- UI overlay and widget management (UI Toolkit / UGUI)
- Save engine with versioning and migration (LiteDB)
- Multi-mod architecture with dependency resolution
- Wall rack and grid placement systems
- Custom shop and employee management APIs
- Logging and diagnostic infrastructure
- Lua, JS and Python scripting bridges
- FishNet multiplayer sync layer (optional)

## Installation

### MelonLoader

1. Download `gregCore-vX.Y.Z-melonloader-windows.zip` (or `-linux.zip`)
2. Extract into your game's root folder
3. Your `Mods/` folder will contain `gregCore.dll`

### BepInEx

1. Download `gregCore-vX.Y.Z-bepinex-windows.zip` (or `-linux.zip`)
2. Extract into your game's root folder
3. `BepInEx/plugins/gregCore/gregCore.dll` is placed automatically

## Dependencies

### Runtime

- **MelonLoader** (v0.7.2+) or **BepInEx** (v6+)

### NuGet packages (bundled in release)

- Jint 4.8.0, LiteDB 5.0.21, Mono.Cecil 0.11.6, MoonSharp 2.0.0, Newtonsoft.Json 13.0.3, pythonnet 3.0.5

### Build only

- .NET 6 SDK
- Game reference assemblies in `lib/references/MelonLoader/`

## Build from Source

Requirements:

- .NET 6 SDK
- local Data Center / MelonLoader installation

> **Note:** This framework was built on Linux using Proton-GE 10-34. Populate `lib/references/MelonLoader/` from your local game install (run the game once with MelonLoader, then copy `MelonLoader/Il2CppAssemblies/` and `MelonLoader/net6/`).

Build:

```bash
git clone https://github.com/mleem97/gregCore.git
cd gregCore
dotnet build -c Release
```

Release output:

```
bin/Release/net6.0/gregCore.dll
```

## Repository Layout

```
gregCore.Framework/
├── src/                    # Framework + mod source code
│   ├── Core/               # GregCoreMod.cs — entry point
│   ├── Infrastructure/     # Config, logging, persistence
│   ├── GameLayer/          # Harmony patches for game classes
│   ├── UI/                 # UI Toolkit overlay
│   ├── API/                # Public API surface
│   └── ...                 # 27 modules total
├── framework/              # greg_hooks.json — canonical hook registry
├── game_hooks.json         # Patchable methods from IL2CPP dump
├── lib/                    # Reference assemblies (game stubs, MelonLoader)
├── docs/                   # Auto-generated API docs
├── scripts/                # Build and code-generation helpers
├── tests/                  # Unit tests
├── sdk/                    # SDK packs
├── examples/               # Example mods (C#, Go, JS, Lua, Python, Rust)
├── .github/workflows/      # CI pipeline
├── VERSION                 # Single source of truth for version
├── gregCore.csproj         # Project file
├── LICENSE                 # Apache 2.0
└── README.md
```

## API Documentation

See [`docs/FrameworkAPI.md`](docs/FrameworkAPI.md) for the auto-generated hook reference.

## Credits

| Role | Contributor |
|------|-------------|
| **Codebase** | [mleem97](https://github.com/mleem97) ([TeamGreg Modding](https://github.com/teamGregModding)) |

## Contributing

See [CONTRIBUTING.md](CONTRIBUTING.md) for guidelines.

Development follows `dev -> pre-release -> main`. See
[the branch and release policy](docs/maintainers/branch-protection.md) before
opening a pull request. Downloads are published on the GitHub Releases page;
development builds are intentionally not presented as stable releases.

## License

This project is licensed under the **Apache License 2.0**. See [`LICENSE`](./LICENSE).

## 🚀 Join the gregFramework Team!

### macOS Support

A native macOS version of Data Center already exists. At the moment, however, there is no implementation path available for macOS support in gregCore, and I do not have access to an Apple device for development or testing. I am actively looking for contributors who can help make macOS support possible. See “Join the gregFramework Team” below.

Building the ultimate modding framework for Data Center is a massive undertaking. gregFramework is currently maintained by a passionate core team of three, and we are looking for fellow creators to help us scale this mission!

**Your place in the team:** We won't throw you into the deep end. Depending on your individual strengths and skills, we will match you with the right areas of the project so you can contribute exactly where you have the most fun.

**🌍 Language Requirement:** A solid grasp of written English is required (without relying on machine translation). Being comfortable speaking English in voice chats is a huge plus, but we completely respect those who prefer to stick to text!

**We are looking for motivated volunteers to join our crew across several roles:**

- 💻 **Code Wizards** (C#, Rust, Lua, TS, GO) — Build and expand the core framework and mod packages
- 🎨 **Asset Creators** (3D Models, hardware assets) — Bring the framework to life visually
- 📚 **Technical Writers** — Craft wiki entries, maintain documentation, and write user guides
- 🎮 **Alpha Testers** — Hunt down bugs, stress-test the framework, and provide critical feedback
- ⚙️ **System Guardians** — Maintain our Linux servers, Docker containers, and infrastructure
- 🤝 **Community Managers** — Foster our Discord community, gather feedback, and keep the energy high

Interested in joining the project? Everyone is absolutely welcome! Send us an email at **apply@gregframework.eu**, shoot a quick DM, or drop a message on [Discord](https://discord.gg/greg).

---

**gregFramework — powered by the community.**
