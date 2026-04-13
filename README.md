# gregCore - Framework Core

> The heart of the gregFramework modding ecosystem for Data Center.

**Author:** MLeeM97 (teamGreg) | **License:** MIT | **Framework:** [gregCore](https://git.datacentermods.com/teamGreg/gregCore)

---

## Features
- **GregSaveService** - Persistent mod data storage
- **GregUIBuilder** - FixedTableUI, Panel System, Canvas Management
- **GregPersistenceService** - Centralized `UserData/gregCore/Data/` storage
- **GregMCPServer** - Embedded HTTP MCP server (Port 10420) for external tooling
- **GregMultiplayerService** - WebSocket relay-based multiplayer
- **Harmony Patches** - MainMenu, MODS Button injection
- **IL2CPP Compatibility** - InputSystem, Coroutines, Il2CppTMPro
- **MoonSharp Lua** - Embedded scripting engine

## Installation

1. Install **MelonLoader** (v0.6+)
2. Place `gregCore.dll` + `gregCore.dll` into `Game/Mods/`
3. Start the game and press **Framework auto-initializes on game start**

## Dependencies

- None (this IS the core)

## Building from Source

```bash
cd gregFramework/gregCore
dotnet build -c Release
# Output: bin/Release/net6.0/gregCore.dll
```

Or build everything at once:

```bash
cd gregFramework/deploy
./build-all.ps1
# Output: gregFramework/BuiltModsForGame/
```

## Links

- **Primary:** [git.datacentermods.com/teamGreg](https://git.datacentermods.com/teamGreg)
- **Discord:** [discord.gg/greg](https://discord.gg/greg)

---

## Contributors & Thanks

### Discord Community
**Thanks to:**
- **Noootry**
- **TheSlickers**
- **Jarvis**
- **Kirei**
- **TeamWaseku** (ModernSamurai, GamerFrankstar, Ultra, Zyn)

*...for keeping the community alive!*

### Code & Testing
**Special thanks:**
- **Joniii** & **mochimus** - Code + Tests
- **Baker**, **Sharpy1o1**, **MachineFreak** - Testing + Modeling

### Sponsors
- **@tobiasreichel** - Haupt-Sponsor
- **SQ8** - Infrastructure Hosting

---
*gregFramework - Powered by the Community!*
