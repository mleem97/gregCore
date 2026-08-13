# GregCore Structure

- `src/Core`: Melon entry point, models, events, persistence and exceptions.
- `src/PublicApi`: `GregMod`, public context, attributes, modules and facade.
- `src/Infrastructure`: logging, settings, plugins, performance, scripting and UI services.
- `src/GameLayer`: bootstrap, lifecycle integration, hooks and game patches.
- `src/Compatibility`: DataCenterModLoader and native game compatibility code.
- `src/UI`: UI Toolkit canvas, panels, overlays, themes and notifications.
- `src/greg.*`: feature modules such as SaveEngine, WallRack and QoL; no custom multiplayer module.
- `src/Bridge`: C#, Lua, JS, Python, Go and Rust bridges.
- `framework/`: canonical reviewed hook manifest and Harmony hooks.
- `tools/`, `templates/`, `examples/`: coverage scanner, mod templates and language examples.
- `tests/`: unit tests for events, patches, registry, diagnostics and public resources.

The MelonLoader entry point is `src/Core/GregCoreMod.cs`; C# mods derive from `src/PublicApi/GregMod.cs`.

## Evidence

- `README.md`
- `src/Core/GregCoreMod.cs`
- `src/PublicApi/GregMod.cs`
- `tests/`
