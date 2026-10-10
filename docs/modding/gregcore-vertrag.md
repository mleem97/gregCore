# Der Greg-Vertrag: Registry, HUD, Menüs, Settings, Save, Logging

> Was ein Mod an gregCore meldet — und was er dafür bekommt (F1-Hub, Tasten-HUD,
> Input-Locks, Settings-Hub, Save-Sidecars, Dev-Console). Namespaces/Methoden exakt,
> Stand gregCore `1.2.3`.

## 1. Grundprinzip: Soft-Dependency + JIT-Trennung

gregCore ist **optional zur Laufzeit**: Der Mod läuft auch standalone (nur Basisfunktionen).
Regel: `GregHost.HasCore` (Typname-Probe, siehe [Getting Started](getting-started.md))
prüfen, und **allen** gregCore-berührenden Code in eigene Methoden packen, die nur bei
`HasCore == true` laufen. Sonst: `JIT-TypeLoad`-Crash ohne gregCore-DLL.

## 2. Mod-Registrierung (`gregCore.Core.Mods.GregModRegistry`)

```csharp
gregCore.Core.Mods.GregModRegistry.Register(
    "gregMod.Hello",   // stabile Mod-ID (Konvention: gregMod.<Name>)
    "Hello",           // Anzeigename (F1-Hub gruppiert danach)
    "1.0.0",           // Version
    new string[] { "hello" });  // Menü-IDs dieses Mods (F1-Hub ordnet danach zu)
```

- `All()` liefert alle Einträge (F1-Hub nutzt das).
- Die Menü-IDs sollten zu den `menuId`s aus Abschnitt 4 passen.

## 3. Tasten-HUD (`gregCore.UI.GregHudRegistry`)

Rechte Tastenleiste (`F6 Backplanes`, `F2 Hex`, …):

```csharp
gregCore.UI.GregHudRegistry.Register("hello", _toggleKey.ToString(), "Hello");
```

Ein Eintrag pro `modId` (erneutes Registrieren überschreibt). `Unregister(modId)` beim
Entladen. Die Liste ist nach Taste sortiert (`All()`).

## 4. Menü-Registry: Opener, Closer, Offen-Status (`gregCore.UI.GregMenuRegistry`)

Das Herz von F1-Hub + Input-Locks. Drei Dinge getrennt melden:

```csharp
// "Öffnen"-Button im F1-Hub (Pflicht für Sichtbarkeit dort):
gregCore.UI.GregMenuRegistry.RegisterOpener("hello", () => HelloPanel.Toggle());

// "Schließen"-Button — NUR mit Closer zeigt der Hub "Schließen" an.
// Ohne Closer bleibt es ehrlich bei "Öffnen" (One-Shot-Aktionen wie Export!).
gregCore.UI.GregMenuRegistry.RegisterCloser("hello",
    () => { try { if (HelloPanel.IsVisible) HelloPanel.Toggle(); } catch { } });
```

Offen-Status (Pflicht für korrekte `[Offen]/[Zu]`-Anzeige) — nach jedem Toggle,
in separater Methode (JIT-Trennung):

```csharp
private static void ReportOpenState()
{
    try { gregCore.UI.GregMenuRegistry.SetOpen("hello", HelloPanel.IsVisible); } catch { }
}
// Aufrufstelle: try { if (GregHost.HasCore) ReportOpenState(); } catch { }
```

Optionen pro Menü (`GregMenuOptions`, Defaults alle `true`): `LockCamera`,
`LockMovement`, `LockInteract`, `ShowCursor` (plus `Draggable`, `SlideFromRight`,
`PanelWidth`). Registrieren via `RegisterMenu(menuId, options)` — ohne Registrierung
gelten die Defaults (sperrend + Cursor). Abfrage: `IsOpen(menuId)`, `TryOpen/TryClose`.

## 5. Settings-Hub-Tab (`greg.UI.Settings.GregSettingsHub`)

Zweite Oberfläche neben dem eigenen Panel (F-Taste). Tab mit Builder-Pattern:

```csharp
greg.UI.Settings.GregSettingsHub.RegisterTab("hello.settings", "Hello",
    (Action<gregCore.UI.GregPanelBuilder>)(b =>
    {
        b.AddToggle("Feature X", current, v => { /* Pref setzen + anwenden */ });
        b.AddSlider("Stärke", 0f, 10f, current, v => { /* ... */ });
        b.AddSecondaryButton("Jetzt reparieren", () => { /* ... */ });
    }));
```

Verfügbare Builder-Methoden (`GregPanelBuilder`, alle fluent): `AddHeadline`,
`AddLabel`, `AddButton`/`AddSecondaryButton`, `AddToggle`/`AddSwitch`,
`AddSlider(min, max, ...)`, `AddDropdown`, `AddInputField`, `AddSpacer`, `AddSeparator`.
Prefs selbst in `MelonPreferences` halten und im Callback schreiben
(`MelonPreferences.Save()` nicht vergessen).

## 6. Save-Sidecar (`gregCore.Infrastructure.Persistence.GregSaveGuard`)

Mod-Daten reisen als `greg_<modId>.<save>.tsv` neben dem Savegame mit — ohne Mod inert:

```csharp
gregCore.Infrastructure.Persistence.GregSaveGuard.RegisterSidecar(
    "hello",
    () => Serialize(),      // Func<string>: Marker/Text liefern
    content => Deserialize(content));  // Action<string>: einlesen
```

Atomar (`.tmp` + `.bak`), spalten-tolerant lesen, Legacy-IDs migrieren (Vorbild:
Backplanes-Registry). Prefix-Mapping für PrefabIDs: `RegisterVanillaModuleMap`.

## 7. Logging

- Standard: `MelonLogger.Msg/Warning/Error` ( landet in `MelonLoader/Latest.log`).
- gregCore-API: `gregCore.API.GregAPI.Log/LogInfo/LogWarning/LogError` (fällt ohne
  Verdrahtung auf MelonLogger zurück; mit Dev-Console auch dort sichtbar).
- Regel: Kauffrequenz-Logs (Spawns!) immer, Sweeps nur bei Änderung/verbose,
  Diagnose-Details hinter eigenem Verbose-Schalter.

## 8. Dev-Console (F1-Nachbar)

- Taste: **Backquote** (`` ` ``) — **nicht** F12 (gehört dem Export).
- Menü-ID `greg.console` (intern, erscheint nicht im Hub).
- Befehle: `help`, `clear`, `mods`, `menus`, `keys` (alle belegten Tasten!), `version`.
- Unity-Fehler/Warnungen aus Spiel + allen Mods laufen dort ein.
