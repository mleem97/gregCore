# GregCore Events for Mod Authors

> Verified status (gregCore `1.2.3`): two buses plus static native hooks.
> What is written here exists in code.

## 1. Which bus for what

- `gregCore.Core.Events.GregEventBus` (instance via service container): `Subscribe(hook, Action<EventPayload>)`,
  `SubscribeOnce`, `Unsubscribe`, `Publish` (stats via `GetStats()`).
- `gregCore.Core.Events.GregHookBus`: `On/Once/Off`, `Dispatch/TryDispatch`,
  `SetHookStatus/IsHookEnabled/Clear`, stats.
- `gregCore.Core.Events.GregEventDispatcher` (static, simplest entry):
  `On(hookName, Action<object> handler, modId)`, `Emit(hookName, data)`.
  Note: `UnregisterAll(modId)` is an **empty stub** — don't rely on it.

Hook names: `greg.{Domain}.{Event}` (`HookName.Create/Parse`, `HookName.Full`).
Event IDs (`EventIds`): economy 1001–1003, persistence 2001, hardware 3001–3004,
cables 4001, input overrides 5001–5003 (only few natively mapped, see
`NativeEventHooks.TryGetEventId`).

## 2. Native hooks for mods (`greg.Sdk.gregNativeEventHooks`)

Static `Action<object>?` fields for direct wiring (pattern from CableThrottle,
EolHider — with try/catch since gregCore may be absent):

```csharp
try
{
    greg.Sdk.gregEventDispatcher.On(gregNativeEventHooks.SystemGameLoaded,
        _ => OnGameLoaded(), "MyMod");
}
catch { /* standalone without gregCore */ }
```

Key hooks: `SystemGameLoaded/SystemGameSaved` (save lifecycle!),
`GameLoaded/GameSaved`, `MoneyChanged/XpChanged/ReputationChanged`
(+ `OnCoinsChanged/OnXpChanged/OnReputationChanged`), `DayEnded/MonthEnded`,
`NetworkCreateNewCable`. Plus `GetByEventId(eventId)` for string lookup.

## 3. Typical save/load subscription

```csharp
// Re-apply state after save load (e.g. suppressed spawners,
// hidden warnings): keep the handler small, work goes in dedicated methods.
greg.Sdk.gregEventDispatcher.On(gregNativeEventHooks.SystemGameLoaded,
    _ => ReapplyAfterLoad(), "MyMod");
```

 Rule of thumb: events notify, heavy work happens in typed methods with
their own error handling — never parse/search inside the handler itself.
