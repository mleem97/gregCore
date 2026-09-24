# Glossary

Definitions for every term used across this wiki — the gregCore equivalent of Oxide's glossary. Terms are grouped like Oxide's (core concepts, development, data, management) but describe this framework, not a game server.

## Core concepts

### gregCore

Modular .NET 6 IL2CPP mod framework for Data Center (v1.2.3). Provides Harmony patching, UI overlays, save engine, multi-mod registry, and scripting bridges. Loaded by MelonLoader 0.7+ or BepInEx 6+.

### Data Center

The game being modded (v1.0.50.15, Unity 6000.5 IL2CPP). Owns lobby, transport, sessions, and saves in co-op — gregCore never replaces these.

### MelonLoader / BepInEx

Supported mod loaders (MelonLoader 0.7.2+, BepInEx 6+, Windows/Linux x64). Exactly one is used; both place `gregCore.dll` + `game_hooks.json` + `greg_hooks.json` where the framework expects them.

### GregMod / Mod

A C# class deriving from `GregMod` (attribute `[GregMod]`) or a Lua folder (`main.lua` + `mod.json`). Registered in `GregModRegistry` and shown in the F1 Hub.

### Mod Hub (F1)

Central menu (`GregModHub`, `MenuId "greg.hub"`, Dialog layer) listing all registered mods and their menus with honest Open/Close buttons.

### HUD key bar

Right-edge overlay (`GregHud` + `GregHudRegistry`, HUD layer) showing one `key + label` row per mod. Display-only and click-through.

### Hooks

Harmonized game methods (1,850 in `game_hooks.json`, 22 groups) dispatched through `GregHookBus` / `GregEventDispatcher` to `greg.on` (Lua) or `[GregHook]` (C#).

### Events

Mod-to-mod and lifecycle messages on `GregEventBus` (`Subscribe / Publish`, Lua `greg.fire`). Payloads: `{ hook_name, timestamp, cancelable, cancelled, data }`.

### gregID

The single stable device-identity schema: `gregID:<Type>:<12HEX>`. Assigned at `Start`/`Awake`, healed on load, invisible to players (screens/names stay vanilla).

### EntityInventory

Boot-rebuilt registry (`GregEntityInventory`) giving every saved entity (servers, switches, routers, firewalls, patch panels, cables, SFP, LACP) a stable invisible UID (`TryFindLive`, `TryGetUid`, `GetAll`, `Rebuilt`).

### SaveGuard

Save-protection layer (`GregSaveGuard`): pre-touch backups, atomic sidecar writes (`greg_<modId>.<save>.tsv`), vanilla sanitizing, idempotent load.

### Sidecar

A per-mod save file next to the vanilla save that the game ignores (only `*.save` is listed). Includes the inventory sidecar (`greg_inventory.<save>.tsv`).

### Native co-op

The boundary rule: Data Center owns multiplayer; mods are local-only and add no transport, lobby, or relay.

### Computer shortcut / app

A button injected into the in-game computer main screen (`GregComputer.RegisterShortcut`, Lua `greg.computer.register_shortcut`) and an own page opened from it (`RegisterApp` / `register_app`; C# frame page with Back button, Lua tablet page). Events: `greg.COMPUTER.ShortcutClicked / AppOpened / AppClosed`.

### Modelling (OBJ, static only)

Custom meshes as shop or static items: `.obj` files (≤ 64 MB, no traversal escapes) validated by `GregObjImport`, loaded via the vanilla `ModLoader` (`GregCustomItems`), described by `GregModPack` DTOs (`ShopItem` / `StaticItem`) or Lua `greg.items.register_shop_item / register_static_item`. No animation, rigging, or skinning — triangle soup plus data-driven colliders.

### Compatibility layer

`gregCore.Compatibility`: legacy `DataCenterModLoader` shim (`GameAPI`, `API_VERSION = 19`, `EventSystem`, `ModConfigSystem` F8 panel) + bundled QoL mods, forwarding to current Core.

## Development

### GregModAttribute / GregHookAttribute / GregDependsOnAttribute

C# attributes marking the mod class, subscribing a method to a hook, and declaring a dependency (`gregCore.Abstractions`).

### SafePatch

Abstract base for Harmony patch classes: one task per class, explicit `PatchAll`, `try/catch`, `true/false` prefix semantics.

### GregPanelBuilder

Fluent UIToolkit panel builder (`Create → SetSize → Build → Add* → Show`); the only supported way to build UI (`OnGUI` is stripped).

### GregMenuRegistry / GregMenuBinding / GregHudRegistry

UI truth layer: menus + openers/closers + input-lock state; one-call Hub wiring (`BindToggle`/`Report`); HUD key rows.

### GregClickRouter

Per-frame `worldBound` click fallback (with 500 ms dedup) compensating for the missing `EventSystem`.

### Lua REPL (F12)

In-game Lua console (`LuaRepl`) for live `greg.*` calls; complements the backquote console (`greg.console`: `help/clear/mods/menus/keys/version`).

### GregModDeps

Dependency API (`Declare / EnsureLoaded / CheckAll`, manifest `GetLocalManifest / DiffManifests / FormatDiff`) — the ModSync precondition for co-op.

### GameApi / GameApiTable

Generated Il2Cpp surface (`src/gregCore.GameApi/Generated/`, `tools/GameApiGenerator`) and the versioned C-ABI struct (`ApiTableVersion`) for Rust/Go/Python bridges.

### GregDoctor

Boot self-diagnostics (directories, hook files, incompatible mods). Related: `IncompatibleModGuard` (unpatches the legacy 404-PersistentID mod).

## Data & configuration

### JSON (Newtonsoft.Json 13.0.3)

Structured-data format for configs, manifests (`mod.json`), hook registries, and DTOs. Lua helpers: `greg.json.parse / stringify`.

### config.json / greg.config

User-facing per-mod settings (Lua `greg.config.*`: `get / get_or / set / delete / has / keys`).

### save.json / greg.save

Per-mod runtime state (same shape + `save_now()` force-flush).

### data/ sandbox (greg.io)

Per-mod file area (`<modId>/data/`, `read/write/append/delete/list`, `read_json/write_json`, `data_dir`); traversal rejected; `require` resolves only the mod dir + `@shared`.

### ModPack / ModSave

C# DTO layers: `GregModPack` (shop/static/DLL snapshots + Il2Cpp array bridges) and `GregModSave` (vanilla-list items keyed by `modFolderName`).

### LiteDB (5.0.21)

Embedded DB used **only** by `GregSaveEngine` (`gregSave_<guid>.greg.db`: `greg_meta`, `grid_state`) and wall-save integration — not a general store.

## Management

### Soft dependency (GregHost.HasCore)

Pattern where a C# mod probes for gregCore with a JIT-split and degrades gracefully instead of crashing when absent. Opposite: hard dependency.

### HL Mods

Auto-created shop section for custom items (`FindSection(shop, "HL Mods")`, GUID-deduped buttons).

### Version pinning

`VERSION` file (framework truth) + `min_framework_version` (Lua) / `[GregDependsOn]` (C#); checked by `scripts/validate_version.py`.
