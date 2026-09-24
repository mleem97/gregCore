# Architecture — gregCore

gregCore is a **modular .NET 6 / IL2CPP mod framework** for *Data Center*.
MelonLoader loads the entry point `GregCoreMod` (`src/gregCore.Main/Core/GregCoreMod.cs`);
from there the service graph, UI root, SaveGuard, dynamic hooks and the mod registry boot.

## Components (src/)

| Assembly | Role |
|---|---|
| `gregCore.Main` | MelonLoader entry (`GregCoreMod`), `BuildInfo` (Flavor/Version) |
| `gregCore.Core` | Config, Diagnostics, Events, Networking save DTOs, Persistence, Services |
| `gregCore.Shared` | Shared utilities (incl. `DevLog`) |
| `gregCore.Abstractions` | Public API surface for mod developers (attributes, interfaces, models) |
| `gregCore.UI` | Overlays, widgets, button HUD (`GregHud`), mod hub (F1), click routing |
| `gregCore.Bridge` | Scripting bridges: Lua/JS/Python/Rust/Go/C# (`GregAPI`, FFI) |
| `gregCore.Hooks` | Dynamic Harmony patcher, native mod loader hooks |
| `gregCore.Patches` | Game-specific Harmony patches |
| `gregCore.Mod` | Multi-mod runtime / registry |
| `gregCore.SDK` | SDK packs for external tools |
| `gregCore.Compatibility` | Built-in QoL/compat modules |
| `gregCore.GameApi` | Generated game API surface |

Additionally: root helpers (`GlobalUsings`, polyfills) and `tests/` (`gregCore.Tests`).

## Data flows

```text
MelonLoader
  → GregCoreMod.OnInitializeMelon
      → GregDirectoryPolicy / GregDoctor
      → Service-Graph (gregCore.Core / Shared)
      → UI Toolkit Root (gregCore.UI)
      → SaveGuard (Persistence)
      → Dynamic Hooks (gregCore.Hooks ← framework/greg_hooks.json + game_hooks.json)
      → Mod Registry (gregCore.Mod) + Scripting Bridges (gregCore.Bridge)
```

- **Patching:** `game_hooks.json` (IL2CPP dump) → dynamic patches + events.
- **Saves:** Save DTOs in `gregCore.Core.Networking` / `Persistence`, versioned via LiteDB.
- **UI:** HUD/HUB in `gregCore.UI`, menu opener via `GregMenuRegistry.RegisterOpener`.
- **CI/Mirror:** identical workflows under `.forgejo/workflows/` and `.gitea/workflows/`
  (Forgejo = push source, GitHub = passive mirror; `scripts/gregMirror.Sync.sh`).
- **Quality:** `.codacy/` (local CLI, no duplicate CI run).

## Further information

- Getting started: [QUICKSTART.md](../QUICKSTART.md)
- Layout details: [SOURCE_LAYOUT.md](SOURCE_LAYOUT.md)
- Scripts: [`scripts/`](../scripts/)
- Tests: [`tests/`](../tests/)
- Examples: [`examples/`](../examples/)

Record changes here + [`CHANGELOG.md`](../CHANGELOG.md) (Unreleased).
