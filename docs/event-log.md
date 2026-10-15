# Event-Log — alles sehen, nichts doppelt

`GregEventLog` (`src/gregCore.Core/Diagnostics/`) protokolliert alles, was durch
das Framework fließt: Menü-Navigation, Hook-Fires, Saves/Loads, Sidecars,
Techniker-Jobs. Gleicher Schlüssel zweimal = Zähler++, niemals eine
Doppel-Zeile (`Server A → Switch B: 1204 Pakete x312 (4/s)`).

## Sinks

- **Datei (verbose, vollständig):** `UserData/gregCore/logs/events-YYYY-MM-DD.log`,
  eine Zeile pro Emission mit Key, Zähler und Zeitstempel. Rotation: 7 Tage.
- **Konsole (clean):** DevConsole-Toggle **Logs/Events** — eine Zeile pro Key
  mit Zähler und Rate, farbcodiert (Hook/Menu/Save/Job).

## Erweitern

`GregEventLog.Emit("bereich:id", "Text", "kind")` — wirft nie, capped auf
300 Keys (älteste ruhige fliegen zuerst). Neue Quellen nur an bestehenden
Framework-Chokepoints (HookBus, Menüs, SaveGuard, Tech-Actions) einhängen,
niemals per Polling.
