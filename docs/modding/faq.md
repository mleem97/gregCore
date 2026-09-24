# FAQ: typische Mod-Fehler

> Symptome aus echten Mod-Sessions — mit Ursache und Fix.

## Panel öffnet, aber Text unsichtbar

Toolkit-Default-Font ohne Spiel-Font. Fix: `GregFontLoader.DefaultUGUIFont`
(null-tolerant) auf alle Labels/Buttons anwenden, mod-lokaler Fallback ohne Core
(siehe [UI-Panels](ui-panels.md)).

## Klicks auf Panel-Buttons gehen verloren

Kein EventSystem im Spiel. Fix: explizite `RegisterCallback<ClickEvent>` **plus**
manuelles `RouteClicks()`-Fallback pro Frame (500-ms-Doppelschutz).

## `Method unstripping failed` im Log

IMGUI-/gestrippte Methoden im IL2CPP-Build aufgerufen (betraf HexViewer).
Fix: auf UIToolkit umstellen, keine IMGUI-Fenster für Spielfunktionen.

## Crash ohne gregCore (JIT-TypeLoad)

gregCore-Typen in Methoden, die auch ohne Core laufen. Fix: `GregHost.HasCore`-Probe,
gregCore-Code nur in separaten Methoden, nur bei `true` aufrufen.

## Stack-Overflow beim Save-Laden (0xC00000FD)

Re-Entrancy-Guard mit `HashSet<object>` über **geboxte** Il2Cpp-Wrapper trifft nie
(jeder Zugriff boxt neu). Fix: Guard per nativem `IntPtr` + Depth-Cap.

## Gekaufte Custom-Items kommen weiß / nur 1 von N farbig

- Shop-Overlay versteckt den Vanilla-Farbpicker → eigenen Picker oder Vanilla-Flow
  wiederherstellen (kein Direktkauf ohne Farbe).
- Vanilla ruft `ApplyColorToSpawnedItem` gern mit stale UID (z. B. immer 1):
  auf frischesten Checkout-Spawn umleiten; `spawnedItems` wird nie geleert
  (stale Keys nicht umfärben!). Mengen: Zeilen-Quantity auf aufeinanderfolgende
  Spawns verteilen, nicht nur ersten.
- Module/Ports: nie `sfpTypeInserted` ohne reales Modul schreiben (Phantom-Modul
  blockiert echte Module + rendert nichts).

## Ports kleben auf 1 Gbps

Effektivrate = `min(Server-Port, Modul, Kabel, Switch-Port)`. Leere Ports früh per
`CableLink.Start`-Postfix auf Tier-Cap heben; nach Modul-Einstecken Cap einmalig
nachziehen (`InsertSFP`-Postfix, nur anheben). Belegte Ports sonst nie anfassen.
Messen statt raten: belegte Ports lesend auditieren (Cap/Modul/Kabel/Gegenseite loggen).

## Doppelte Shop-Buttons / doppelte Käufe

Zwei Mods besitzen dieselben ID-Ranges (MoreModules vs. MoreServers vs.
RealisticModules). Fix: `RegisteredMelons`-Check, Verlierer bleibt inert
(`s_disabledBySibling`, Fehler loggen).

## Spiel-Update hat Hooks zerschossen

Totstellen: Harmony skippt fehlende Ziele still. Fix-Routine: generierte Caches neu,
einmal ohne optionale Mods starten, Hook-Signaturen gegen Live-Assembly prüfen,
tote Hooks löschen (siehe [Harmony + IL2CPP](harmony-il2cpp.md)).

## Trolley fliegt beladen durch die Gegend

Masse zu klein. Fix: `Rigidbody.mass`-Multiplikator + `angularDrag`-Multiplikator
(LargerCart-`CartStabilizer`-Muster, konfigurierbar, einmal pro Szene).
