# The Greg Contract: Registry, HUD, Menus, Settings, Save, Logging

> What a mod reports to gregCore — and what it gets back (F1 hub, key HUD,
> input locks, settings hub, save sidecars, dev console). Namespaces/methods exact,
> status gregCore `1.2.3`.

## 1. Ground rule: soft dependency + JIT separation

gregCore is **optional at runtime**: the mod also runs standalone (base features only).
Rule: check `GregHost.HasCore` (type-name probe, see [Getting Started](getting-started.md)),
and put **all** gregCore-touching code in separate methods that only run when
`HasCore == true`. Otherwise: `JIT TypeLoad` crash without the gregCore DLL.

## 2. Mod registration (`gregCore.Core.Mods.GregModRegistry`)

```csharp
gregCore.Core.Mods.GregModRegistry.Register(
    "gregMod.Hello",   // stable mod ID (convention: gregMod.<Name>)
    "Hello",           // display name (F1 hub groups by it)
    "1.0.0",           // version
    new string[] { "hello" });  // menu IDs of this mod (F1 hub maps by them)
```

- `All()` returns all entries (used by the F1 hub).
- Menu IDs should match the `menuId`s from section 4.

## 3. Key HUD (`gregCore.UI.GregHudRegistry`)

Right-side key bar (`F6 Backplanes`, `F2 Hex`, …):

```csharp
gregCore.UI.GregHudRegistry.Register("hello", _toggleKey.ToString(), "Hello");
```

One entry per `modId` (re-registering overwrites). `Unregister(modId)` on unload.
The list is sorted by key (`All()`).

## 4. Menu registry: opener, closer, open state (`gregCore.UI.GregMenuRegistry`)

The heart of F1 hub + input locks. Report three things separately:

```csharp
// "Open" button in the F1 hub (required for visibility there):
gregCore.UI.GregMenuRegistry.RegisterOpener("hello", () => HelloPanel.Toggle());

// "Close" button — the hub only shows "Close" with a registered closer.
// Without one it honestly stays at "Open" (one-shot actions like Export!).
gregCore.UI.GregMenuRegistry.RegisterCloser("hello",
    () => { try { if (HelloPanel.IsVisible) HelloPanel.Toggle(); } catch { } });
```

Open state (required for correct `[Open]/[Closed]` display) — after every toggle,
in a separate method (JIT separation):

```csharp
private static void ReportOpenState()
{
    try { gregCore.UI.GregMenuRegistry.SetOpen("hello", HelloPanel.IsVisible); } catch { }
}
// Call site: try { if (GregHost.HasCore) ReportOpenState(); } catch { }
```

Per-menu options (`GregMenuOptions`, all defaults `true`): `LockCamera`,
`LockMovement`, `LockInteract`, `ShowCursor` (plus `Draggable`, `SlideFromRight`,
`PanelWidth`). Register via `RegisterMenu(menuId, options)` — without registration
the defaults apply (locking + cursor). Queries: `IsOpen(menuId)`, `TryOpen/TryClose`.

## 5. Settings hub tab (`greg.UI.Settings.GregSettingsHub`)

Second surface next to the mod's own panel (F-key). Tab with builder pattern:

```csharp
greg.UI.Settings.GregSettingsHub.RegisterTab("hello.settings", "Hello",
    (Action<gregCore.UI.GregPanelBuilder>)(b =>
    {
        b.AddToggle("Feature X", current, v => { /* set pref + apply */ });
        b.AddSlider("Strength", 0f, 10f, current, v => { /* ... */ });
        b.AddSecondaryButton("Repair now", () => { /* ... */ });
    }));
```

Available builder methods (`GregPanelBuilder`, all fluent): `AddHeadline`,
`AddLabel`, `AddButton`/`AddSecondaryButton`, `AddToggle`/`AddSwitch`,
`AddSlider(min, max, ...)`, `AddDropdown`, `AddInputField`, `AddSpacer`, `AddSeparator`.
Keep prefs in `MelonPreferences` yourself and write them in the callback
(don't forget `MelonPreferences.Save()`).

## 6. Save sidecar (`gregCore.Infrastructure.Persistence.GregSaveGuard`)

Mod data travels as `greg_<modId>.<save>.tsv` next to the savegame — inert without mod:

```csharp
gregCore.Infrastructure.Persistence.GregSaveGuard.RegisterSidecar(
    "hello",
    () => Serialize(),                // Func<string>: deliver markers/text
    content => Deserialize(content)); // Action<string>: read back
```

Atomic (`.tmp` + `.bak`), read column-tolerant, migrate legacy IDs (template:
Backplanes registry). PrefabID mapping: `RegisterVanillaModuleMap`.

## 7. Logging

- Standard: `MelonLogger.Msg/Warning/Error` (lands in `MelonLoader/Latest.log`).
- gregCore API: `gregCore.API.GregAPI.Log/LogInfo/LogWarning/LogError` (falls back
  to MelonLogger unwired; also visible in dev console when wired).
- Rule: high-frequency logs (spawns!) always on, sweeps only on change/verbose,
  diagnostics behind a dedicated verbose switch.

## 8. Dev console (F1 neighbor)

- Key: **backquote** (`` ` ``) — **not** F12 (belongs to Export).
- Menu ID `greg.console` (internal, hidden from hub).
- Commands: `help`, `clear`, `mods`, `menus`, `keys` (all bound keys!), `version`.
- Unity errors/warnings from game + all mods flow in.
