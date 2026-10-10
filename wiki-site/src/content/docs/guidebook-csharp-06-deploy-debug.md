---
title: "Guidebook CSharp 06 Deploy Debug"
---
# Guidebook CSharp 06 Deploy Debug

Get your DLL into the game and diagnose it when it misbehaves. Discovery path verified in `GregPluginRegistry.LoadAll` + `AssemblyScanner` (Mono.Cecil scan, no assembly load).

## Where the DLL goes

- **MelonLoader:** `<game root>/Mods/YourMod.dll` — the same folder as `gregCore.dll`. (`GregPluginRegistry` scans `MelonEnvironment.ModsDirectory`.)
- **BepInEx:** use your loader's equivalent mod folder; the framework is supported on both, but the scan path above is the MelonLoader directory — verify placement against your loader layout before reporting issues.
- The framework's own deploy script (`scripts/Deploy-Release-ToDataCenter.ps1`) shows the convention: `gregCore.dll → {GameRoot}/Mods`, plugins → `greg/Plugins`, Lua → `UserData/gregCore/Mods/Lua`, hooks → `UserData/gregCore/game_hooks.json`.

## What makes your DLL load

1. Exactly one class carries `[GregMod("your.id", "Name", "1.0.0")]` — the Cecil scan reads this attribute **without loading** your assembly. No attribute (`NO_MOD_ATTRIBUTE`) or empty ID → silently skipped.
2. Duplicate IDs: the first loaded wins; later DLLs with the same manifest ID are skipped.
3. Dependencies resolve via `GregDependencyResolver` before `LoadRuntimeMod` instantiates your class.
4. No `MelonInfo` needed and no `MelonMod` inheritance — `GregMod` is managed by gregCore (`OnUpdate` per frame, `OnSceneLoaded` per scene, `OnShutdown` on unload).

## Update loop

Edit → `dotnet build -c Release` → copy the DLL over `<game>/Mods/YourMod.dll` → restart the game (C# has no hot-reload; Lua does — see [Guidebook Debugging](./guidebook-debugging/)). Keep `Private=false` on the `gregCore.dll` reference so you never ship the framework inside your mod.

## Debugging

- **Loader log** (MelonLoader `Latest.log` in the game root): look for `Mod registriert: <Name> (<Version>) [ID: ...]`, `[DynamicPatcher]` lines, and your `Logger.Info` output. No registration line → attribute/scan problem (§ above).
- **Backquote console**: `mods` (registry?), `menus`, `keys`, `version` — confirms wiring without reading files.
- **`GregDoctor`**: boot self-checks (directories, hook files, game-build fingerprint, safe mode on unsupported builds).
- **Common failures**: silent skip (attribute missing/empty ID) → add `[GregMod]`; duplicate ID → rename; crash without framework → `GregHost.HasCore` guard (see [Guidebook CSharp 01 Setup](./guidebook-csharp-01-setup/)); stale behavior after game update → re-copy interop, regenerate `GameApi` + hooks (see [Guidebook CSharp 04 Patches](./guidebook-csharp-04-patches/)).

## Checkpoint

- [ ] DLL in `Mods/`, registration line in the log, Hub entry visible.
- [ ] Rebuild → overwrite → restart picks up changes; uninstall (delete DLL) leaves saves loadable.

Next: [Guidebook Release](./guidebook-release/) — packaging and publishing.
