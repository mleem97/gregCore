# Guidebook Prerequisites

What every track needs before writing a line of mod code. Total: ~15 minutes.

## 1. Game + framework (all tracks)

1. Install Data Center (Steam) and run it once.
2. Install **one** loader: MelonLoader 0.7.2+ or BepInEx 6+.
3. Install gregCore 1.2.3: extract the release ZIP matching your loader + OS into the game root, so `gregCore.dll` sits next to `game_hooks.json` and `greg_hooks.json` (`Mods/` on MelonLoader, `BepInEx/plugins/gregCore/` on BepInEx).
4. Launch the game once. This creates `UserData/gregCore/Mods/Lua/`, `UserData/gregCore/Mods/Scripts/`, and `UserData/gregCore/Mods/Lua/@shared/`.

Verify: the loader log shows `HWID SYSTEM ACTIVE` and `[DynamicPatcher]` lines; in-game **F1** opens the Mod Hub (`GregModHub`), **F12** opens the Lua REPL, backquote (`` ` ``) opens the gregCore console (`help`, `mods`, `menus`, `keys`, `version`). Details: [[Player Installation]].

## 2. Per-track tools

| Track | Tools | Check |
|---|---|---|
| Lua | Any text editor (VS Code recommended). No SDK, no build. | You can create folders under `UserData/gregCore/Mods/Lua/`. |
<<<<<<< HEAD
| JS (beta) | Any text editor. No build. | `UserLibs/Js/` exists in the game root (created at boot by `JsBridge`; legacy `Plugins/Js` is ignored). |
=======
| JS/TS | Any text editor + `tsc` for TypeScript. No game build. | You can create folders under `UserData/gregCore/Mods/JS/`. Types: `templates/js/greg.d.ts`. |
>>>>>>> agent/gregcore-integration
| C# | .NET 6 SDK + local game + loader install. Copy `MelonLoader/Il2CppAssemblies/` and `MelonLoader/net6/` into the repo's `references/` once. | `dotnet build -c Release` succeeds; `python3 scripts/validate_version.py 1.2.3` exits 0. |

Full C# environment (GameApi regeneration, coverage, mirror/CI): [[Developer Environment]].

## 3. Folder layout you will use

```
<Game root>/
├── Mods/gregCore.dll + game_hooks.json + greg_hooks.json   (MelonLoader)
├── UserData/gregCore/Mods/Lua/<modId>/main.lua + mod.json  (Lua mods)
├── UserData/gregCore/Mods/Lua/@shared/                     (Lua shared modules)
├── UserData/gregCore/Mods/Scripts/                         (general scripts)
<<<<<<< HEAD
└── UserLibs/Js/*.js                                        (JS mods — flat files)
=======
├── UserData/gregCore/Mods/JS/<modId>/*.js                     (JS/TS mods — one folder per mod)
└── UserLibs/Js/*.js                                        (JS legacy — flat files, still loaded)
>>>>>>> agent/gregcore-integration
```

C# mods ship as compiled DLLs (built from `templates/csharp/`) and are deployed per `scripts/Deploy-Release-ToDataCenter.ps1` conventions.

## 4. The three rules (all languages)

1. **Local-only.** Data Center owns lobby, transport, and sessions. Mods never add networking ([[Developer Native Coop]]).
2. **Saves stay vanilla.** Settings → config, state → save/sidecar, bulk files → sandbox. Never write vanilla save structures ([[Core Save Engine]]).
3. **Report honestly.** Register what you are (manifest/registry), wire Hub openers/closers truthfully, gate missing dependencies instead of crashing ([[Developer Best Practices]]).

## Checkpoint

- [ ] F1 Hub opens, F12 REPL answers `print(greg.player.money())`, console `mods` + `version` work.
- [ ] (C# only) template builds with `dotnet build -c Release`.
- [ ] You picked a track: [[Guidebook Lua 01 First Mod]] · [[Guidebook CSharp 01 Setup]] · [[Guidebook JS 01 Setup]].
