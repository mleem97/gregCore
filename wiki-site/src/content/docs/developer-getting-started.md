---
title: "Developer Getting Started"
---
# Developer Getting Started

Who this is for: programmers who want to **write** Data Center mods on gregCore. Players start at [Player Getting Started](./player-getting-started/).

## Which language?

| Language | Extension | Runtime | Status | Pick it when… |
|---|---|---|---|---|
| **Lua** | `.lua` | MoonSharp 2.0.0 (soft sandbox) | Production Ready, full API | You start fresh — biggest API, examples, REPL, hot-reload |
| **JavaScript** | `.js` | Jint 4.8.0 | Beta, basic (`logInfo/on/fire`) | You prefer JS and need only events + logging so far |
| **Python** | `.py` | pythonnet 3.0.5 | Beta, basic | You prefer Python and need only the small bridge surface |
| **C#** | `.dll` / `.cs` | MelonLoader + Harmony | Production Ready | You edit the world (spawn, cables, patches), need max power |
| **Rust / Go** | FFI | C ABI (`GameApiTable`) | Alpha | You ship native logic via the FFI bridge |

Lua is the recommended default. C# is required for world editing (spawning, cable ops, custom Harmony patches) — Lua deliberately cannot do those (see [Core Events](./core-events/) “Out of scope”).

## The mod contract (all languages)

Every mod honors the same lifecycle + registry contract (`docs/modding/gregcore-vertrag.md`):

- **Register** what you are: `GregModRegistry.Register(id, name, version, menus)` (C#) or `mod.json` (`id`, `name`, `version`, `entry`, `min_framework_version`) for Lua.
- **Report UI honestly**: `GregMenuRegistry.RegisterOpener / RegisterCloser / SetOpen`, HUD rows via `GregHudRegistry.Register(modId, key, label)`. The F1 Hub renders exactly what you report — no opener, no Open button.
- **Respect saves**: user settings → config, runtime state → save/sidecar, never vanilla structures directly ([Core Save Engine](./core-save-engine/)).
- **Respect co-op**: local-only, no second transport ([Developer Native Coop](./developer-native-coop/)).
- **Soft dependency**: C# mods probe `GregHost.HasCore` with a JIT-split so they load (degraded, not crash) without gregCore.

## Learning path

1. [Developer Environment](./developer-environment/) — SDK, references, build, test (once).
2. [Developer First Lua Mod](./developer-first-lua-mod/) **or** [Developer First CSharp Mod](./developer-first-csharp-mod/) — your first working mod.
3. [Developer Events Hooks Guide](./developer-events-hooks-guide/) + [Developer Timers Coroutines](./developer-timers-coroutines/) — react to the game + schedule work.
4. [Developer Data Storage](./developer-data-storage/) + [Developer UI Panels HUD](./developer-ui-panels-hud/) — persist + show.
5. Specialty: [Developer Shop Items](./developer-shop-items/), [Developer Hardware IDs Inventory](./developer-hardware-ids-inventory/), [Developer Harmony IL2CPP](./developer-harmony-il2cpp/), [Developer Scripting Bridges](./developer-scripting-bridges/).
6. Ship: [Developer Best Practices](./developer-best-practices/) → [Developer Publishing](./developer-publishing/).

## Conventions you must follow from day one

- **Conventional Commits** (`feat:`, `fix:`, `docs:`, `chore:` …), one logical change per commit; never `push --force` or rewrite history unasked.
- **Docs sync**: new features update `README.md` + `docs/` + `CHANGELOG.md` (Unreleased).
- **No secrets** in repos (keys/tokens/`.env` — env vars only); no generated artifacts (`bin/`, `obj/`, `dist/`) in commits.
- **Verify before declaring done**: build + test what the repo offers (`QUICKSTART.md`: `dotnet build -c Release`, `DOTNET_ROLL_FORWARD=Major dotnet test`, `python3 scripts/validate_version.py <VERSION>`).
