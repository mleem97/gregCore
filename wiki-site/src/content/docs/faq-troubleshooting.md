---
title: "FAQ Troubleshooting"
---
# FAQ Troubleshooting

Symptom → cause → fix. (Oxide equivalent: "Reporting Issues" + owner troubleshooting, adapted to a client framework.)

## Install / boot

| Symptom | Cause | Fix |
|---|---|---|
| "No hooks found" / no events fire | `game_hooks.json` not next to `gregCore.dll` | Place both JSON files alongside the DLL (`Mods/` or `BepInEx/plugins/gregCore/`); check `[DynamicPatcher]` log lines |
| F1 Hub never opens | Misplaced files, or another overlay locked input | Verify install; close Pause/Escape/Options overlays; check `GregMenuRegistry` lock state in log |
| `HWID SYSTEM ACTIVE` missing | Framework did not boot | Re-extract release ZIP; confirm loader version (MelonLoader 0.7.2+ / BepInEx 6+); read full loader log |
| Toast about incompatible ID mod | Old standalone 404-PersistentID mod present | Remove it — `IncompatibleModGuard` already unpatched it; gregID must be the only ID system |

## Lua mods

| Symptom | Cause | Fix |
|---|---|---|
| "Lua mod not loading" | Wrong folder / missing entry | Folder must be `UserData/gregCore/Mods/Lua/<modId>/` with `main.lua` (or `mod.json` → custom `entry`) |
| Silent failures, safe defaults (`0`/`""`/`nil`) | Called outside the game or on error (by design, guarded boundary) | Test in-game via F12 REPL; add `greg.ui.log_info` checkpoints |
| Per-frame log spam / stutter | Logging or heavy work in `on_update` | Move to `greg.every`; cache + throttle; see [Developer Timers Coroutines](./developer-timers-coroutines/) |
| `require` fails | Path outside mod dir / circular | Only `<modDir>/` (+`modules/`) and `@shared/`; check `*.lua` vs `*/init.lua` candidates |
| IO "access denied" | Escape from sandbox | Stay inside `<modId>/data/`; never `..`, `/`, `\` in paths |
| JS/Python example code fails | Examples are stale (`subscribe`/`fire_event` era) | Use current `JsBridge`/`PythonFFIBridge` surface per [Developer Scripting Bridges](./developer-scripting-bridges/) |

## UI

| Symptom | Cause | Fix |
|---|---|---|
| Invisible panel text | Missing game font | Apply `GregFontLoader.DefaultUGUIFont` (null-tolerant) |
| Clicks do nothing | Only `ClickEvent` callback registered | Add the `GregClickRouter` `worldBound` fallback + 500 ms dedup |
| Input stuck after closing panel | Lock not released | Pair every `SetOpen(id, true)` + `Report` with close-path release; re-check cursor/movement restore |
| Hub shows display-only row | No opener registered | `RegisterOpener` (or `BindToggle`) for that `menuId` |

## Saves

| Symptom | Cause | Fix |
|---|---|---|
| Mod data gone after update | Reused IDs / wrote vanilla structures | Use sidecars + stable high-range IDs; never write vanilla save fields directly |
| Save won't load vanilla after mod removal | Modded fields leaked into vanilla | Route through `GregModPack`/`GregModSave` + `RegisterVanillaModuleMap` fallback; test uninstall round-trip |
| `[HwId]` error spam | Corrupt entry in save | Healer null-guards per entry — identify the ID from the log, `EntityInventory.Verify()` / `Dump()` (MelonPreferences `gregCore.EntityInventory`) |

## Build / contrib

| Symptom | Cause | Fix |
|---|---|---|
| `dotnet test` fails: no .NET 6 runtime | Host has only .NET 8/10 | `DOTNET_ROLL_FORWARD=Major dotnet test` |
| Stale API after game update | Interop/dump outdated | Re-copy assemblies → `GameApiGenerator/regenerate` → re-dump hooks → `validate_contracts.py` → vanilla-first test |
| Coverage claim disputed | Wrong denominator | `game_hooks.json` (raw 1,850) vs `framework/greg_hooks.json` (curated) — see `docs/modding/api/coverage.md` |

## Still stuck?

1. Backquote console → `mods`, `menus`, `keys`, `version` — paste the output.
2. Loader log excerpt (`[gregCore]`, `[gregCore][HwId]`, `[DynamicPatcher]`, errors).
3. Game + framework + loader versions, mod list, single-player vs co-op.
4. Mod-specific → mod tracker; Hub/HUD/consoles/saves → gregCore tracker; security → `SECURITY.md` (never public).
