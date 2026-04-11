---
id: framework-addons
title: Framework Addons
sidebar_label: Framework Addons
slug: /community/framework-addons
description: Extension guide for gregCore framework addons, including install locations, load order, and validation steps.
---

Framework Addons are framework-level extensions that extend gregCore behavior without modifying the core runtime directly.

Typical addon categories:

- Additional hook providers
- Language bridge extensions
- Diagnostics and tooling modules
- Runtime integration helpers

## Where to install addons

Use the addon binary type to choose the correct location.

### Managed addons (`.dll`, C# / MelonLoader)

Install path:

- `Data Center/Mods/`

Examples:

- `FFM.Plugin.AssetExporter.dll`
- `FFM.Plugin.Multiplayer.dll`
- `FFM.Plugin.WebUIBridge.dll`

### Native addons (`.dll` / `.so`, Rust bridge modules)

Install path:

- `Data Center/Mods/RustMods/`

Use this path only for native bridge modules exposed through the gregCore Rust runtime.

## Installation procedure

1. Stop the game completely.
2. Copy addon files to the correct folder.
3. Start the game.
4. Open `MelonLoader/Latest.log` and verify addon load events.

Recommended PowerShell examples:

```powershell
Copy-Item .\FFM.Plugin.AssetExporter.dll "C:\Program Files (x86)\Steam\steamapps\common\Data Center\Mods\" -Force
Copy-Item .\hexviewer_rust.dll "C:\Program Files (x86)\Steam\steamapps\common\Data Center\Mods\RustMods\" -Force
```

## Load order and compatibility

- gregCore must initialize before dependent addons.
- Addons should fail soft (log warning, keep game running).
- Addons must not directly patch unrelated systems outside their declared scope.

If an addon depends on another addon, verify dependency order in `Latest.log` and document it in the addon README.

## How to validate a successful install

Checklist:

- Addon appears in `MelonLoader/Latest.log` without exception traces.
- Hook subscriptions are registered (if addon is event-driven).
- No repeated startup retries or null-reference loops.
- Addon behavior is visible in-game (feature-specific smoke test).

## Uninstall / rollback

1. Exit the game.
2. Remove addon binaries from `Mods/` or `RustMods/`.
3. Restart game and verify logs are clean.

## Best practices for addon authors

- Keep addon API surface aligned with `greg.*` naming conventions.
- Use `GregEventDispatcher` rather than direct cross-mod coupling.
- Guard all critical runtime paths with error handling.
- Log clear startup/shutdown messages for support diagnostics.
