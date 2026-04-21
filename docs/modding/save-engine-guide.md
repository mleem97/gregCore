---
title: "Save Engine Guide"
description: "Documentation for the LiteDB-based greg.SaveEngine"
---

# Save Engine Guide

## Was ist greg.SaveEngine?
Eine hochperformante, embedded BSON-Datenbank (LiteDB 5.x) zum Speichern von Spielständen. Sie ersetzt den schwerfälligen Vanilla-IL2CPP-Binary-Save durch flexible, schemalose Document-Collections.

## Vanilla-Save vs. greg.SaveEngine
| Feature | Vanilla Save | greg.SaveEngine |
| --- | --- | --- |
| Format | Binary IL2CPP | LiteDB (BSON) |
| Mod-Kompatibilität | Schlecht (Runtime-only states) | Perfekt (Collections) |
| Erweiterbarkeit | Keine | Beliebig viele eigene Collections |

## LiteDB & ILRepack
LiteDB ist via ILRepack vollständig in die `gregCore.dll` eingebettet. Es gibt keine externe `LiteDB.dll` und keine Abhängigkeit in UserLibs.

## Vanilla-Kompatibilität
**NOT COMPATIBLE by design.**
Wenn ein Vanilla-Save geladen wird, deaktiviert sich `greg.SaveEngine.Write`.

## Modder-API
```csharp
// Vanilla-Save-Status prüfen
if (GregFeatureGuard.IsVanillaSave) {
    LoggerInstance.Warning("Vanilla save — custom save disabled");
    return;
}

// Eigene Daten im greg.SaveEngine speichern
GregSaveEngine.Instance.GetCollection<MyData>("mymod_data")
    .Upsert(new MyData { Id = "key1", Value = 42 });

// Eigene Daten laden
var data = GregSaveEngine.Instance.GetCollection<MyData>("mymod_data")
    .FindById("key1");

// Feature-State-Change subscriben
GregFeatureGuard.OnFeatureStateChanged += (featureKey) => {
    if (featureKey == "GridPlacement") RefreshUI();
};
```
