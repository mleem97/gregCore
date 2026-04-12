# 🟢 gregCore Framework — v1.0.0.8

> The ultimate modding ecosystem for *Data Center*. Direct IL2CPP bridges, native FFI support, and high-performance scripting.

## 🔢 **Versioning Scheme**
We use a strict `vX.X.X.P` format:
- **X.X.X**: Major.Minor.Patch version of the framework.
- **P**: **Pre-Release version** (Current: `8`). 
*Example: v1.0.0.8 means Framework 1.0.0, Pre-Release 8.*

## 📦 **Installation (End-User)**

1.  **Requirement**: Ensure you have [MelonLoader v0.6.1](https://github.com/LavaGang/MelonLoader/releases) (or newer) installed for *Data Center*.
2.  **Download**: Get the latest `gregCore-v1.0.0.6.zip` from the [Releases](https://github.com/mleem97/gregCore/releases) page.
3.  **Extract**: 
    *   Copy `gregCore.dll` to your game's `Mods/` folder.
    *   Copy `Jint.dll` and `Esprima.dll` to your game's `MelonLoader/Libs/` folder.
    *   Copy `greg_hooks.json` to your game's root directory (next to the `.exe`).
4.  **Launch**: Start the game. You should see "gregCore v1.0.0.6 initialized" in the console.

## 🧪 **Installation (gregTester)**

1.  **Manual Build**: If you are testing a dev branch, run `dotnet build -c Release`.
2.  **Deployment**: Copy the resulting DLLs from `bin/Release/net6.0/` to your `Mods` folder.
3.  **Debug Mode**:
    *   Press **F5** in-game to toggle the gregCore debug overlay.
    *   Check `MelonLoader/Latest.log` for any "CRASH" or "HOOK" errors.
4.  **VLAN Testing**: Verify the new `greg.NETWORK.SetVlanAllowed` hooks using the provided Lua test scripts in `gregReferences`.

## 🛠️ **Developer Info**

- **Language Support**: C#, Lua (MoonSharp), JS/TS (Jint), Rust (FFI).
- **API Version**: 10
- **Vendored Components**: 
  - `MoonSharp` (Source-integrated)
  - `Jint` (Bundled)

## ⚖️ Disclaimer & Credits

- **gregCore** uses code from [DataCenter-RustBridge](https://github.com/Joniii11/DataCenter-RustBridge) for the Rust implementation. **Joniii11** is an active part of **teamGreg**.
- **gregModmanager** utilizes modloading capabilities derived from [DataCenter-ModLoader](https://github.com/ASavageSwan/-DataCenter-ModLoader).
- **MoonSharp**: Used for Lua scripting support. Licensed under the [MIT License](https://opensource.org/licenses/MIT).
- **Jint**: Used for JavaScript/TypeScript execution. Licensed under the [BSD 2-Clause License](https://opensource.org/licenses/BSD-2-Clause).
- **Harmony**: Used for runtime patching. Licensed under the [MIT License](https://opensource.org/licenses/MIT).
- Special thanks to all contributors and the creators of the libraries used in this project.

## 🚀 Join the Team!

To truly build the ultimate modding framework for *Data Center*, we still have a solid chunk of work ahead of us. That's why I am constantly on the lookout for fellow creators who want to join the team.

No matter what your strengths are, we are looking for people to:
- **Write Code** (C#, Rust, Lua, TS)
- **Create Models** (3D assets, new hardware... or more Steves!)
- **Write Docs** (Wiki, documentation, maintaining guides)
- **Or just play the game** (Testing, finding bugs, and giving feedback)

Everyone is absolutely welcome! If you want to be a part of the project, just shoot me a quick DM on **Discord** or visit [gregframework.eu](https://gregframework.eu).

---
*© 2026 teamGreg | [gregframework.eu](https://gregframework.eu)*
