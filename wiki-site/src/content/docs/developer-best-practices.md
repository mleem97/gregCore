---
title: "Developer Best Practices"
---
# Developer Best Practices

The rules that keep multi-mod setups stable. (Oxide equivalent: "Plugin Guidelines" + "Best Practices" + "Permissions".)

## 1. Dependencies, not assumptions

```lua
greg.mods.list()                 -- {{id, name, version}}
greg.mods.is_loaded("other_mod") -- -> bool
greg.mods.version("other_mod")   -- -> string
greg.mods.declare({{mod = "other_mod", min_version = "1.0.0", required = true}})
local ok, detail = greg.mods.ensure({mod = "other_mod", min_version = "1.0.0"})
greg.mods.check()                -- {{owner, mod, detail}}; empty = ok
```

C#: `[GregDependsOn("gregCore", "1.2.3")]` + `GregModDeps.Declare / EnsureLoaded / CheckAll`. Report missing deps in the log + Hub instead of throwing. There is no Oxide-style permission/group system — gating is per-mod via these declarations and your own settings.

## 2. Performance

- No per-frame reflection or `FindObjectsOfType`; cache + throttle (0.1 s / 1 s / 2 s / 30 s).
- No per-frame logging; batch notifications (`greg.ui.notify`).
- Prefer event-state over polling; prefer automation engines over reimplemented loops.
- Full rules: [Developer Harmony IL2CPP](./developer-harmony-il2cpp/), [Developer Timers Coroutines](./developer-timers-coroutines/).

## 3. Saves and identity

- Settings → config, state → save/sidecar, bulk → `data/` sandbox ([Developer Data Storage](./developer-data-storage/)).
- Address devices via inventory UIDs, never display names ([Developer Hardware IDs Inventory](./developer-hardware-ids-inventory/)).
- Never reuse IDs/GUIDs; one bad entry must never abort a load; test save→load→uninstall→load-vanilla.

## 4. UI honesty

- Every Hub entry needs a truthful Open/Close affordance (`HasOpener`/`HasCloser`); every HUD row maps to a real hotkey ([Developer UI Panels HUD](./developer-ui-panels-hud/)).
- Lock input only while open; restore cursor/movement after; ignore toggles during Pause/Escape/Options.

## 5. Co-op + sandbox discipline

- Local-only; no second transport ([Developer Native Coop](./developer-native-coop/)).
- Lua: no world spawning, no cable ops, no keyboard capture, no escapes from `<modId>/data/` — move those to C#.
- `require` only your folder + `@shared`; no circular requires.

## 6. Diagnostics before release

- `GregDoctor` clean; `[DynamicPatcher]` shows your patches applied; no `[gregCore][HwId]` error spam.
- `scripts/validate_contracts.py` for hook coverage claims; `python3 scripts/validate_version.py <VERSION>`; `dotnet test` green.
- `.codacy/` local scan (`lizard`, `opengrep`, `pylint`, `trivy`) before the PR — no duplicate CI run exists.
- Ship with: unique mod ID, `min_framework_version`, README (events you emit, settings, co-op scope), CHANGELOG entry.
