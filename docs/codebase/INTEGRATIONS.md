# GregCore Integrations

- MelonLoader 0.7.3 and Unity 6000.4.12f1 are the reviewed contract values in `framework/greg_hooks.json`.
- Unity IL2CPP and Il2CppInterop are required runtime references.
- Harmony provides Prefix/Postfix patching; the canonical manifest is fingerprint-gated.
- Data Center integration is under `src/Compatibility/DataCenterModLoader` and `src/GameLayer`.
- Native Data Center co-op is treated as a game-owned subsystem; gregCore does
  not ship a relay, FishNet, lobby, or replacement synchronization layer.
- Legacy Rust FFI v7 Steam/lobby/P2P slots remain ABI-compatible no-ops only;
  they do not call Steam or create sessions.
- The reviewed game references expose Unity multiplayer roles, game-owned
  network-save/network-map types, SteamManager and Steamworks lobby/P2P APIs;
  see `docs/codebase/native-coop-assembly-audit.md` for hashes and commands.
- Lua, JavaScript, Python, C#, Go and Rust bridges are represented under `src/Bridge` and `src/Sdk/Language`.
- Settings and framework persistence use JSON services; LiteDB is declared for the save-engine layer.

[TODO] In-game Windows/Linux smoke-test artifacts were not available in this repository inspection.

## Evidence

- `framework/greg_hooks.json`
- `gregCore.csproj`
- `src/Compatibility/`
- `src/Bridge/`
- `src/Infrastructure/Config/`
- `src/Infrastructure/Settings/Services/`
