# Player Installation

Step-by-step installation of gregCore for **MelonLoader** and **BepInEx**, on Windows and Linux.

## 1. Install a loader

Install **exactly one** loader into your Data Center game folder:

- **MelonLoader 0.7.2+** — follow the official MelonLoader setup, then run the game once so `Mods/` and `MelonLoader/` exist.
- **BepInEx 6+** — follow the official BepInEx setup, then run the game once so `BepInEx/plugins/` exists.

> gregCore targets .NET 6 (`net6.0`, Unity 6000.5 IL2CPP). The loader provides the `Il2CppAssemblies` and `net6` interop surface gregCore builds against.

## 2. Download the right gregCore ZIP

From the GitHub Releases page, pick the artifact matching your loader and OS:

- `gregCore-v1.2.3-melonloader-windows.zip` / `-linux.zip`
- `gregCore-v1.2.3-bepinex-windows.zip` / `-linux.zip`

NuGet dependencies (Jint 4.8.0, LiteDB 5.0.21, Mono.Cecil 0.11.6, MoonSharp 2.0.0, Newtonsoft.Json 13.0.3, pythonnet 3.0.5) are **bundled in the release** — you do not install them separately.

## 3. Extract into the game root

- **MelonLoader:** extract the ZIP into the game's root folder. `Mods/` will then contain `gregCore.dll`, with `game_hooks.json` and `greg_hooks.json` alongside it (same directory as the DLL).
- **BepInEx:** extract the ZIP into the game's root folder. `BepInEx/plugins/gregCore/gregCore.dll` is placed automatically.

The two JSON files are **required**: without `game_hooks.json` next to the DLL you get "No hooks found" and no events fire (see [[FAQ Troubleshooting]]).

## 4. First launch + verification

1. Launch the game normally (through Steam or your usual launcher).
2. gregCore creates `UserData/gregCore/Mods/Lua/`, `UserData/gregCore/Mods/Scripts/`, and the backup root automatically.
3. Check the MelonLoader/BepInEx log for:
   - `HWID SYSTEM ACTIVE` — the hardware-ID system booted.
   - `[DynamicPatcher]` lines — dynamic hooks installed from `game_hooks.json`.
   - `[gregCore][HwId]` lines — ID assignment/healing (normal on first load of an existing save).
4. In game, press **F1** — the Mod Hub (`GregModHub`) opens with a dimmed background and a `GregFramework — Mods (F1)` card. That confirms UI + menu registry + input routing all work.

## 5. Installing Lua mods (the common case)

1. Each mod is one folder: `UserData/gregCore/Mods/Lua/<modId>/` containing `main.lua` (entry point) and optionally `mod.json` (manifest with `id`, `name`, `version`, `entry`, `min_framework_version`).
2. Restart the game or hot-reload (the mod's `on_reload()` runs after a successful reload).
3. Verify with the backquote console: `` ` `` → `mods` lists the registry (`GregModRegistry.All()`), `version` prints the framework version.

## 6. Uninstalling

- Remove `gregCore.dll` (and the two JSON files) to uninstall the framework. Vanilla saves are untouched: mod data lives in sidecars (`<saveDir>/greg_<modId>.<save>.tsv`), which the game ignores because it only lists `*.save`.
- Save backups from before gregCore ever touched a save are kept permanently under `pre-greg/` inside `Documents/DatacenterBackups` (see [[Player Mod Users Guide]]).

## Next

- [[Player Mod Users Guide]] — daily use: Hub, HUD, settings, REPL, save hygiene.
- Something failed? [[FAQ Troubleshooting]] first, then Discord / GitHub issue with log excerpt.
