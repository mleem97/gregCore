---
title: "Core Overview"
---
# Core Overview

The Oxide equivalent of this section is "Core": commands + libraries. gregCore's core is different — it is a **client-side framework** (no server, no chat commands, no permission groups). This page maps the framework's architecture, services, logging, and configuration. Events and saves have their own pages: [Core Events](./core-events/), [Core Save Engine](./core-save-engine/).

## Boot sequence

```
MelonLoader
  → GregCoreMod.OnInitializeMelon          (src/gregCore.Main/Core/GregCoreMod.cs)
      → GregBootstrapper.Build(...)        (src/gregCore.Main/GameLayer/Bootstrap/GregBootstrapper.cs)
      → GregDirectoryPolicy / GregDoctor   (directories + self-diagnostics)
      → Service graph                      (GregServiceContainer, gregCore.Core)
      → UI Toolkit root                    (GregCanvasManager, GregUILayerManager)
      → SaveGuard install                  (GregSaveGuard, Harmony prefixes/postfixes)
      → Dynamic hooks                      (GregDynamicHookPatcher ← framework/greg_hooks.json + game_hooks.json)
      → Mod registry + language hosts      (GregModRegistry, GregLanguageRegistry: Lua/JS/Python/Rust/C#)
```

`GregCoreMod` (a `MelonMod` subclass) is deliberately thin: it exposes statics (`Instance`, `PublicAPI: IGregAPI`, `HarmonyInstance`, `EventBus: GregEventBus`, `HookBus: GregHookBus`) and forwards `OnUpdate` / `OnSceneWasLoaded` / `OnApplicationQuit` to the owners. All logic lives in the assemblies below.

## Assemblies (`src/`)

| Assembly | Role | Key types |
|---|---|---|
| `gregCore.Main` | Loader entry + composition root | `GregCoreMod`, `BuildInfo`, `GregBootstrapper` |
| `gregCore.Core` | Config, diagnostics, events, networking save DTOs, persistence, services, automation | `GregApiContext`, `GregServiceContainer`, `GregEventDispatcher`, `GregShop`, `GregServers`, `GregCoop`, `GregSaveGuard`, `GregEntityInventory` |
| `gregCore.Shared` | Cross-cutting utilities | `GregLogger`, `GregBanner`, `DevLog`, `GregFeatureGuard` |
| `gregCore.Abstractions` | **Contracts only** — the public API surface every other assembly references | `IGregEventBus`, `IGregLogger`, `IGregConfigService`, `IGregPersistenceService`, `GregModAttribute`, `GregHookAttribute`, `GregDependsOnAttribute`, `ModManifest`, `EventPayload` |
| `gregCore.UI` | Overlays, HUD, Mod Hub, click routing, panels, settings, keybinds | `GregPanel`, `GregPanelBuilder`, `GregMenuRegistry`, `GregMenuBinding`, `GregModHub`, `GregHud`, `GregHudRegistry`, `GregClickRouter`, `GregUILayerManager`, `GregSettingsHub`, `GregKeybindRegistry` |
| `gregCore.Bridge` | Stable static facade + native FFI (Go/Rust/Python/C#) | `GregAPI`, `GregPublicAPI`, `GameApiTable`, `NativeModLoader`, `GoFFIBridge`, `RustFFIBridge`, `PythonFFIBridge` |
| `gregCore.Hooks` | Dynamic Harmony patching + hook integration | `GregDynamicHookPatcher`, `HookIntegration`, `GregHookRegistry`, `SafePatch` |
| `gregCore.Patches` | Concrete game-specific Harmony patches | `HardwareIdPersistencePatch`, `IncompatibleModGuard`, `RackPatch`, `SaveSystemPatch`, `ShopPatch`, `PlayerPatch`, `TimePatch` |
| `gregCore.Mod` | C# mod base class + high-level modules + save/grid/wall systems | `GregMod` (abstract), `greg` (static: `Economy`, `Player`, `Server`, `UI`, `Save`, …), `GregGridManager`, `GregWallRegistry`, `GregSaveEngine` |
| `gregCore.SDK` | Author-facing multi-language SDK + Lua/JS hosts | `LuaFFIBridge`, `JsBridge`, `GregLuaHost`, `GregJsHost`, `Lua*Module` (~20 domain modules), `LuaRepl`, `LuaHotReload`, `GregHookCatalog` |
| `gregCore.Compatibility` | Legacy `DataCenterModLoader` shim + bundled QoL mods | `GameAPI` (`API_VERSION = 19`), `EventSystem`, `EntityManager`, `ModConfigSystem`, `greg.CommonShop`, `greg.QoL`, `greg.WallRack.Integration` |
| `gregCore.GameApi` | Generated Il2Cpp surface (~200 wrappers) | `GregGameModuleHost`, `*Module` wrappers, `Generated/Il2Cpp/*.g.cs` (via `tools/GameApiGenerator`) |

Supporting pieces: `src/GlobalUsings.cs`, `src/NullablePolyfills.cs`, root `gregCore.csproj` (`net6.0`, deploys to the game's `Mods` path), `tests/gregCore.Tests`, `tools/`, `scripts/`.

## Services and context

- `GregApiContext` bundles what a mod needs: `Logger`, `EventBus`, `HookBus`, `Config`, `Persist`, `MainThread`, `Resources`, `Events`.
- `GregServiceContainer` wires the graph at boot; `GregPluginRegistry` + `AssemblyScanner` + `GregDependencyResolver` discover native (C#) mods.
- `GregPerformanceGovernor` + `GregOperationQueue` + `GregResourceMonitor` throttle expensive work (scans, per-frame reflection) — see [Developer Harmony IL2CPP](./developer-harmony-il2cpp/) and [Developer Best Practices](./developer-best-practices/).
- Automation engines (`Automation`, `CableLaying`, `DeliveryZone`, `NetworkConfig`, `RackBuild`, `Repair`) encapsulate repeatable in-game workflows for mods to call instead of reimplementing.

## Logging and diagnostics

- `GregLogger` (`gregCore.Shared`): `Msg / Warn / Error / Debug / Section / PatchApplied / HookFired`, plus per-mod `GregModLogger`. MoonSharp/Lua surface: `greg.ui.log_info / log_warning / log_error / log`.
- `DevLog` for development-only chatter; `MelonLoggerAdapter` bridges to the loader log.
- `GregDoctor` runs boot self-checks (directories, hook files present, incompatible mods). `IncompatibleModGuard` detects the old standalone 404-PersistentID mod and unpatches it so `gregID` stays the only ID system (log + one toast per session).
- What to grep in logs: `[gregCore]`, `[gregCore][HwId]`, `[DynamicPatcher]`, `HWID SYSTEM ACTIVE`.

## Configuration

- Framework config: `GregConfigService.LoadConfig<T> / SaveConfig<T>` (Newtonsoft.Json, `MissingMemberHandling.Ignore`, auto-creates directories). Debug flag: `GregCoreConfig.DebugMode` (default true in dev).
- Mod settings (modern): `GregModSettingsService` + `GregSettingsHub.RegisterTab` with `AddToggle / AddSlider / …` builders; persisted in MelonPreferences.
- Legacy mod configs: `ModConfigSystem` (`RegisterBool / Int / Float`, F8 panel, files under `UserData/ModConfigs`).
- Entity inventory tuning: MelonPreferences category `gregCore.EntityInventory` (`Enabled=true`, `VerboseLogging=false`, `DumpOnRebuild=false`) with `Dump()` / `Verify()` — see [Developer Hardware IDs Inventory](./developer-hardware-ids-inventory/).
- Mod dependency declarations: `GregModDeps` (`Declare / EnsureLoaded / CheckAll` with minimum versions against the MelonLoader registry) + manifest diff (`GetLocalManifest / DiffManifests / FormatDiff`) for co-op mod-sync — see [Developer Best Practices](./developer-best-practices/).

## Compatibility notes

- The old `DataCenterModLoader` API keeps working through `gregCore.Compatibility` (same `GameAPI` table, `API_VERSION = 19`, events re-emitted). New code should target the `gregCore.Abstractions` contracts / `greg.*` Lua API instead.
- Rust FFI v7 Steam/P2P slots exist only as inert no-ops for ABI stability — the game owns networking ([Developer Native Coop](./developer-native-coop/)).
- UI: IMGUI `OnGUI` is stripped in the IL2CPP build — always use UIToolkit via `GregPanelBuilder` ([Developer UI Panels HUD](./developer-ui-panels-hud/)).
