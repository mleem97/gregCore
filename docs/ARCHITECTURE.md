# Architektur — gregCore

gregCore ist ein **modulares .NET 6 / IL2CPP-Mod-Framework** für *Data Center*.
MelonLoader lädt den Einstiegspunkt `GregCoreMod` (`src/gregCore.Main/Core/GregCoreMod.cs`);
von dort booten Service-Graph, UI-Root, SaveGuard, dynamische Hooks und der Mod-Registry.

## Komponenten (src/)

| Assembly | Rolle |
|---|---|
| `gregCore.Main` | MelonLoader-Entry (`GregCoreMod`), `BuildInfo` (Flavor/Version) |
| `gregCore.Core` | Config, Diagnostics, Events, Networking-Save-DTOs, Persistence, Services |
| `gregCore.Shared` | Gemeinsame Utilities (u. a. `DevLog`) |
| `gregCore.Abstractions` | Öffentliche API-Fläche für Mod-Entwickler (Attribute, Interfaces, Modelle) |
| `gregCore.UI` | Overlays, Widgets, Tasten-HUD (`GregHud`), Mod-Hub (F1), Click-Routing |
| `gregCore.Bridge` | Scripting-Bridges: Lua/JS/Python/Rust/Go/C# (`GregAPI`, FFI) |
| `gregCore.Hooks` | Dynamischer Harmony-Patcher, Native-Mod-Loader-Hooks |
| `gregCore.Patches` | Game-spezifische Harmony-Patches |
| `gregCore.Mod` | Multi-Mod-Runtime / Registry |
| `gregCore.SDK` | SDK-Packs für externe Tools |
| `gregCore.Compatibility` | Eingebaute QoL-/Compat-Module |
| `gregCore.GameApi` | Generierte Game-API-Oberfläche |

Zusätzlich: Root-Helper (`GlobalUsings`, Polyfills) und `tests/` (`gregCore.Tests`).

## Datenflüsse

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

- **Patching:** `game_hooks.json` (IL2CPP-Dump) → dynamische Patches + Events.
- **Speicherstand:** Save-DTOs in `gregCore.Core.Networking` / `Persistence`, versioniert über LiteDB.
- **UI:** HUD/HUB in `gregCore.UI`, Menü-Opener via `GregMenuRegistry.RegisterOpener`.
- **CI/Spiegel:** identische Workflows unter `.forgejo/workflows/` und `.gitea/workflows/`
  (Forgejo = Push-Quelle, GitHub = passiver Mirror; `scripts/gregMirror.Sync.sh`).
- **Qualität:** `.codacy/` (CLI lokal, ohne CI-Doppel-Lauf).

## Weiteres

- Einstieg: [QUICKSTART.md](../QUICKSTART.md)
- Layout-Detail: [SOURCE_LAYOUT.md](SOURCE_LAYOUT.md)
- Skripte: [`scripts/`](../scripts/)
- Tests: [`tests/`](../tests/)
- Beispiele: [`examples/`](../examples/)

Änderungen hier + [`CHANGELOG.md`](../CHANGELOG.md) (Unreleased) nachtragen.
