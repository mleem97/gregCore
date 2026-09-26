# gregCore Wiki

> Modular .NET 6 IL2CPP mod framework for **Data Center** — Harmony patching, UI overlays, save engine, scripting, and multi-mod architecture.

**Current version:** 1.2.3 · **Game:** Data Center 1.0.50.15 · **Engine:** Unity 6000.5 (IL2CPP) · **Loaders:** MelonLoader 0.7+ / BepInEx 6+ (Windows + Linux x64) · **License:** Apache-2.0

This wiki is the complete guidebook for gregCore, structured after the Oxide docs model:

- **Guides** — task-oriented walkthroughs for players and mod developers.
- **Core** — what the framework itself provides (architecture, events, config, saves).
- **Hooks** — the full event/hook reference (1,850 game hooks in 22 groups).
- **Glossary** — every term used across this wiki, defined once.

## Start here

| I want to… | Read |
|---|---|
| Build my own mod (C#, Lua, JS) — step by step | [[Guidebook]] → [[Guidebook Prerequisites]] → your track (Lua / C# / JS) |
| Play with mods, install gregCore, use the Mod Hub | [[Player Getting Started]] → [[Player Installation]] → [[Player Mod Users Guide]] |
| Write my first Lua mod | [[Developer First Lua Mod]] (then [[Developer Events Hooks Guide]]) |
| Write a C# mod | [[Developer First CSharp Mod]] (then [[Developer Harmony IL2CPP]]) |
| Write a Rust mod | [[Guidebook Rust 01 Setup]] (then [[Guidebook Rust 02 Project]]) |
| Understand the framework internals | [[Core Overview]] → [[Core Events]] → [[Core Save Engine]] |
| Look up an event or API call | [[Hooks Reference]] + [[Glossary]] |
| Fix something broken | [[FAQ Troubleshooting]] |

## 5-minute orientation

1. **Players** install a loader (MelonLoader or BepInEx), drop `gregCore.dll` plus `game_hooks.json` / `greg_hooks.json` next to it, and launch the game once. Details: [[Player Installation]].
2. **In game**, press **F1** for the central Mod Hub, look at the key bar on the right edge (the HUD) for mod hotkeys, and press **F12** for the Lua console. Details: [[Player Mod Users Guide]].
3. **Mod developers** start with Lua: one folder under `UserData/gregCore/Mods/Lua/<modId>/` with `main.lua` and an optional `mod.json`. Lifecycle functions (`on_init`, `on_update`, `on_scene_loaded`, `on_shutdown`, `on_reload`) are all optional. Details: [[Developer First Lua Mod]].
4. **Events** are the heart of gregCore: `greg.on("greg.Group.Method", callback)` subscribes to any of the 1,850 harmonized game methods, and `greg.fire` sends your own mod-to-mod events. Details: [[Core Events]] and [[Hooks Reference]].
5. **Saves are safe by design**: the SaveGuard backs up vanilla saves, stores mod data in inert sidecars (`greg_<modId>.<save>.tsv`), and every device carries a stable invisible `gregID`. Details: [[Core Save Engine]] and [[Developer Hardware IDs Inventory]].
6. **Co-op stays vanilla**: Data Center owns lobby, transport, and sessions. Mods are local-only and must never add a second networking stack. Details: [[Developer Native Coop]].

## Repository map

- Main repo: `https://github.com/mleem97/gregCore`
- GregCore Collection on Steam (mods + framework, one click): `https://steamcommunity.com/sharedfiles/filedetails/?id=3701575419`
- Framework source: `src/` (`gregCore.Main`, `gregCore.Core`, `gregCore.UI`, `gregCore.Bridge`, `gregCore.Hooks`, `gregCore.Patches`, `gregCore.Mod`, `gregCore.Abstractions`, `gregCore.Compatibility`, `gregCore.GameApi`, `gregCore.Shared`, `gregCore.SDK`)
- Canonical hook registry: `framework/greg_hooks.json` · patchable method dump: `game_hooks.json`
- Examples (6 languages): `examples/` · starter templates: `templates/csharp`, `templates/lua/example-mod`
- Docs index in-repo: `docs/INDEX.md` · quick start: `QUICKSTART.md` · changelog: `CHANGELOG.md`

## Contributing to this wiki

This wiki lives in the `.wiki/` folder of the repository and is synced to the GitHub wiki via `scripts/Sync-Wiki.ps1`. Fix a typo or extend a page with a pull request that edits the corresponding `.wiki/*.md` file, following `CONTRIBUTING.md` (Conventional Commits, docs + `CHANGELOG.md` Unreleased entry for features).
