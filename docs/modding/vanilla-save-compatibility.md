---
title: "Vanilla Save Compatibility"
description: "How gregCore handles vanilla save files"
---

# Vanilla Save Compatibility

## Was ist ein "Vanilla Save"?
Ein "Vanilla Save" ist ein Spielstand, der vom nativen Speichersystem des Spiels "Data Center" angelegt wurde (IL2CPP Binary Save). Er enthält keine `greg_meta`-Dokumente.

## Wie greg.SaveEngine es erkennt
Beim Aufruf von `LoadGame` überprüft `GregVanillaDetector`, ob die zu ladende Datei eine `.greg.db` mit validem Header ist. Wenn nicht, wird sie als Vanilla eingestuft.

## Was passiert bei einem Vanilla Save?
`GregFeatureGuard.IsVanillaSave` wird auf `true` gesetzt.
Folgende Features werden zum Schutz des Spielstands deaktiviert:
- `GridPlacement` (keine Racks übers Grid platzieren)
- `SaveEngine.Write` (keine LiteDB-Updates)

## Was Spieler tun müssen
Um alle Modding-Features voll auszunutzen, muss ein neues Spiel gestartet werden, das ausschließlich über die `greg.SaveEngine` gespeichert wird.

## Migration (WIP)
Gibt es einen Konverter? Nein, aktuell nicht. (Siehe `MISSING.md` für zukünftige Entwicklungen im Save-Parser).

## GregFeatureGuard API
```csharp
// Vanilla-Save-Status prüfen
if (GregFeatureGuard.IsVanillaSave) {
    LoggerInstance.Warning("Vanilla save — custom save disabled");
    return;
}
```
