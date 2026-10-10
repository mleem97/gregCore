---
title: "Guidebook Porting Matrix"
---
# Guidebook Porting Matrix

The same features in Lua, C#, and JS — use it to switch tracks or split work across languages.

## Events

| Task | Lua (full) | C# (full) | JS (beta) |
|---|---|---|---|
| Subscribe | `greg.on("greg.PLAYER.CoinChanged", fn)` → token | `On(id, fn)` (base helper) → `IDisposable`; or `[GregHook(id)]` | `greg.on("greg.PLAYER.CoinChanged", function (p) {...})` |
| Once | `greg.once(id, fn)` | Dispose inside the handler after first fire | n/a (count + ignore after first) |
| Unsubscribe | `greg.off(token)` (auto-clean at reload) | `Dispose()` / `DisposeSubscriptions()` in `OnUnload` | n/a (lives for the session) |
| Emit | `greg.fire("my_mod.evt", {...})` | `GregEventDispatcher.Emit(id, data)` | `greg.fire("my_mod.evt", {...})` |
| Discover | `greg.hooks.groups()` / `.audio.list()` / `on_<snake>` sugar | `HookName` constants, `EventIds` | Same string IDs for `on`/`fire` |

## Logging

| Task | Lua | C# | JS |
|---|---|---|---|
| Info/warn/error | `greg.ui.log_info / log_warning / log_error` | `Logger.Info / Warning / Error` (protected `IGregLogger`) | `greg.logInfo / logWarning / logError` |
| Toast | `greg.ui.notify(msg, secs)` | `GregNotificationManager.Show(msg, duration)` | n/a (fire an event; companion notifies) |

## Scheduling

| Task | Lua | C# | JS |
|---|---|---|---|
| Once / repeat | `greg.wait(s, fn)` / `greg.every(s, fn)` | Throttled `OnUpdate` + cached state (no per-frame scans) | n/a (event-driven counting) |
| Coroutine | `greg.start_coroutine(fn)` + `coroutine.yield(greg.WAIT)` | `MainThread.Enqueue` for marshaling | n/a |

## Storage

| Task | Lua | C# | JS |
|---|---|---|---|
| Settings | `greg.config.*` (`config.json`) | `GregModSettingsService` / `GregConfigService` | n/a |
| State | `greg.save.*` (`save.json`) + `save_now()` | `GregModSave`, `GregSaveEngine`, `RegisterSidecar` | n/a |
| Files/JSON | `greg.io.*` sandbox + `greg.json.*` | Newtonsoft (configs) / System.Text.Json (runtime) | n/a |

## UI

| Task | Lua | C# | JS |
|---|---|---|---|
| Panel | `tablet_open / widget_open / panel_add_*` handles | `GregPanelBuilder` + click-router + font + input-lock | n/a |
| Hub/HUD | automatic via tablet (Hub lists Lua mods) | `GregMenuBinding` + `GregHudRegistry` | n/a (announce via `fire`; companion shows UI) |
| Settings tab | `greg.ui.register_mod_config_tab` | `GregSettingsHub.RegisterTab` | n/a |

## Game systems

| Task | Lua | C# | JS |
|---|---|---|---|
| Read economy/hardware | `greg.player / server / switch / tech / rack / cable / patch / customer / economy / shop / net / requests / subnet / world` | `greg` facade modules + `GregServers / GregShop / GregTechnicians / …` bridges + `GregEntityInventory` | n/a (payloads only) |
| Drive repairs/shop | `repair_all()`, `dispatch_server()`, `shop.buy(idx)` | `GregServers.FindAll / Repair`, `GregShop.CartAddOne / BuyItem`, `GregTechnicians` | n/a |
| World edits, spawning, cable ops, patches | Not available (by design) | `GregMod` + `SafePatch` + grid/wall systems | Not available |

## Identity

| Task | Lua | C# |
|---|---|---|
| Mod ID | `mod.json` (`Id`, `Entrypoint`, …) / folder fallback | `[GregMod(id, name, version)]` + `GregModRegistry.Register` |
| Deps | `greg.mods.declare / ensure / check` | `[GregDependsOn]` + `GregModDeps` |

## Porting notes

- Lua → C#: move timers to throttled updates, tablets to `GregPanelBuilder`, `config/save` to services + sidecars; keep hook IDs identical.
- C# → Lua: replace patches with subscriptions where events exist; move world edits out (Lua cannot do them — keep a C# companion).
- Either → JS: keep only observe/log/announce; pair with a Lua/C# companion for the rest.
