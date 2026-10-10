---
title: "Guidebook CSharp 02 Lifecycle"
---
# Guidebook CSharp 02 Lifecycle

The `GregMod` base class (`src/gregCore.Mod/GregMod.cs`), the registry, the dispatcher, and the threading rule.

## Overrides

```csharp
public override void OnLoad()     { /* register everything */ }
public override void OnReady()    { /* game is ready; heavy init here, not in OnLoad */ }
public override void OnUpdate(float deltaTime) { /* allocation-free per-frame work only */ }
public override void OnSceneLoaded(string sceneName) { /* scene changed */ }
public override void OnUnload()   { /* mirror OnLoad: dispose, unregister, close panels */ }
// OnShutdown() defaults to OnUnload(); override only if shutdown differs from unload.
```

`Initialize(GregApiContext)` injects `Logger, EventBus, HookBus, Config, Persist, MainThread, Resources, Events` — use `Api.*` / `Logger` / `MainThread` from there. Keep `OnLoad` to registration; expensive scans go to `OnReady` or throttled updates.

## Registry + dependencies

```csharp
GregModRegistry.Register("shift_helper_cs", "ShiftHelper C#", "1.0.0",
    menus: new[] { "shift_helper_cs.main" });

// Runtime dependency gating (GregModDeps.Dependency{ModId, MinVersion, Required}):
GregModDeps.Declare("shift_helper_cs",
    new GregModDeps.Dependency { ModId = "some_helper_lib", MinVersion = "1.0.0", Required = false });
if (!GregModDeps.EnsureLoaded(
        new GregModDeps.Dependency { ModId = "some_helper_lib", MinVersion = "1.0.0" }, out var detail))
    Logger.Warning($"Optional dep missing: {detail}");
```

`GregModRegistry.All()` feeds the F1 Hub; `menus[]` links your entries to `GregMenuRegistry` state (next chapter). Missing deps: log + Hub warning, never throw.

## Events: attributes vs manual

```csharp
[GregHook("greg.PLAYER.CoinChanged")]
public void OnCoins(EventPayload payload)
    => Logger.Info($"Coins changed: {payload.Data["Amount"]}");

IDisposable sub = On("greg.Group.Event", payload => { /* ... */ });
// base-class On() auto-tracks; DisposeSubscriptions() in OnUnload releases all.
```

`On()` is the protected `GregMod` helper (returns `IDisposable`, tracked for `DisposeSubscriptions()`). The static `GregEventDispatcher.On(hookName, handler, modId)` exists but returns void — prefer the base helper or `[GregHook]`.

Native lifecycle (`gregNativeEventHooks`): `SystemGameLoaded / SystemGameSaved`, `GameLoaded / GameSaved`, `Money / Xp / ReputationChanged`, `Day / MonthEnded`. Prefer string IDs; the numeric 1001–4001 range is compat.

## Logging + threading

- `Logger.Info / Warning / Error / Debug` (protected `IGregLogger`, already scoped to your mod); dev-only `DevLog`; what to grep: `[gregCore]`, `[gregCore][HwId]`, `[DynamicPatcher]`, `HWID SYSTEM ACTIVE`.
- Callbacks arrive on the main thread; when in doubt, `MainThread.Enqueue(...)` (`IGregMainThreadDispatcher.Enqueue`). Never touch game objects from background threads.
- High-level modules first: the static `greg` facade (`src/gregCore.Mod/PublicApi/greg.cs`: `Economy, Player, Server, …`) and the `Greg*` bridges (`GregServers.FindAll / Repair`, `GregShop`, `GregTechnicians`, …) cover whole workflows — call them before writing patches. (The coroutine engines under `Infrastructure/Automation` are `internal` framework machinery, not mod API.)

## Exercises

1. Log `greg.PLAYER.CoinChanged` via attribute **and** one manual subscription; dispose both on unload.
2. Move a scene-scan from `OnLoad` to `OnReady` + throttled `OnUpdate` (see [Guidebook CSharp 04 Patches](./guidebook-csharp-04-patches/) tiers).
3. Simulate missing gregCore (`GregHost.OverrideForTesting(false)`) and confirm degraded load.

## Checkpoint

- [ ] Lifecycle overrides placed correctly; registry entry visible in Hub grouping.
- [ ] Events flow; logging greppable; no game access off main thread.

Next: [Guidebook CSharp 03 UI](./guidebook-csharp-03-ui/) — Hub, HUD, panels.
