# GregCore Events für Mod-Autoren

> Verifizierter Stand (gregCore `1.2.3`): zwei Busse plus statische Native-Hooks.
> Was hier steht, existiert im Code — Manifest-/Stub-Doku mit abweichenden Namen
> (`GregMod`-Basisklasse, `context.Events`) ist überholt.

## 1. Welcher Bus wofür

- `gregCore.Core.Events.GregEventBus` (Instanz via Service-Container): `Subscribe(hook, Action<EventPayload>)`,
  `SubscribeOnce`, `Unsubscribe`, `Publish` (mit Stats via `GetStats()`).
- `gregCore.Core.Events.GregHookBus`: `On/Once/Off`, `Dispatch/TryDispatch`,
  `SetHookStatus/IsHookEnabled/Clear`, Stats.
- `gregCore.Core.Events.GregEventDispatcher` (statisch, einfachster Einstieg):
  `On(hookName, Action<object> handler, modId)`, `Emit(hookName, data)`.
  Achtung: `UnregisterAll(modId)` ist ein **leerer Stub** — kein Verlass darauf.

Hook-Namen: `greg.{Domain}.{Event}` (`HookName.Create/Parse`, `HookName.Full`).
Event-IDs (`EventIds`): Economy 1001–1003, Persistenz 2001, Hardware 3001–3004,
Kabel 4001, Input-Overrides 5001–5003 (nativ gemappt sind erst wenige, siehe
`NativeEventHooks.TryGetEventId`).

## 2. Native Hooks für Mods (`greg.Sdk.gregNativeEventHooks`)

Statische `Action<object>?`-Felder zum Direktverdrahten (Muster aus CableThrottle,
EolHider — mit try/catch, da gregCore fehlen kann):

```csharp
try
{
    greg.Sdk.gregEventDispatcher.On(gregNativeEventHooks.SystemGameLoaded,
        _ => OnGameLoaded(), "MeinMod");
}
catch { /* ohne gregCore: Standalone */ }
```

Wichtigste Hooks: `SystemGameLoaded/SystemGameSaved` (Save-Lifecycle!),
`GameLoaded/GameSaved`, `MoneyChanged/XpChanged/ReputationChanged`
(+ `OnCoinsChanged/OnXpChanged/OnReputationChanged`), `DayEnded/MonthEnded`,
`NetworkCreateNewCable`. Dazu `GetByEventId(eventId)` für String-Lookup.

## 3. Typisches Save-Load-Abo

```csharp
// Nach Save-Load Zustand neu anwenden (z. B. unterdrückte Spawner,
// versteckte Warnungen): Handler klein halten, Arbeit in eigene Methoden.
greg.Sdk.gregEventDispatcher.On(gregNativeEventHooks.SystemGameLoaded,
    _ => ReapplyAfterLoad(), "MeinMod");
```

 Faustregel: Events melden, schwere Arbeit passiert in typisierten Methoden mit
eigener Fehlerbehandlung — nie im Handler selbst parsen/suchen.
