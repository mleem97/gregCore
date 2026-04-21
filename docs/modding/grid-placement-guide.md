---
title: "Grid Placement Guide"
description: "How to use greg.GridPlacement and integrate it with your mods"
---

# Grid Placement Guide

## Was ist greg.GridPlacement?
greg.GridPlacement ersetzt das Vanilla-Rack-Placement durch ein dynamisches Grid-Placement-System, das ohne vorgefertigte RackHolder-Plates auskommt (ähnlich zu Bausimulationen wie Die Sims).

## Vanilla RackHolder vs. Grid-System
| Feature | Vanilla RackHolder | greg.GridPlacement |
| --- | --- | --- |
| Basis | Vorab platzierte Plates | Endloses virtuelles Raster |
| Flexibilität | Gering | Hoch |
| Sub-Routing | Nein | Ja (2x2 Sub-Grid für Kabel etc.) |

## Grid-Terminologie
- **Cell**: Eine Grid-Einheit, entspricht dem Footprint eines Racks.
- **SubCell**: Eine Cell ist in 4 SubCells (2x2) unterteilt.
- **Snap**: Einrasten auf die Grid-Koordinaten.
- **Origin**: Der 0,0,0 Punkt des virtuellen Grids.

## Wie Racks platziert werden
1. Modus aktivieren via F8 (oder B-Taste).
2. Raster erscheint.
3. Hover zeigt Preview.
4. Klick platziert Rack direkt im World-Space und aktualisiert den GridManager.

## Vanilla-Save-Verhalten
Wenn ein Vanilla-Save geladen wird, deaktiviert sich das Grid-Placement automatisch (`GridPlacement disabled`). 
Vanilla speichert Racks in ihrem echten Positionen und Plates. Unser System speichert sie via `greg.SaveEngine`. Beides mischen zerstört Spielstände.

## Modder-API
```csharp
// Prüfen ob Grid-Feature aktiv (kein Vanilla-Save)
bool active = GregFeatureGuard.IsEnabled("GridPlacement");

// Rack-Placement-Event subscriben
GregEventDispatcher.On(GregNativeEventHooks.WorldRackPlaced, (payload) => {
    string rackId  = GregPayload.Get<string>(payload, "rackId", null);
    string coord   = GregPayload.Get<string>(payload, "gridCoord", null);
}, modId: "myMod");

// Freie Zelle prüfen
GregGridManager grid = GregGridManager.Instance;
bool free = !grid.IsCellOccupied(new Vector2Int(3, 5));

// Rack programmatisch platzieren
grid.PlaceRack(new Vector2Int(3, 5), myRack);
```

## Bekannte Einschränkungen
⚠️ Siehe `MISSING.md` im `greg.GridPlacement`-Verzeichnis: Vanilla-Unterdrückung (`RackPlacementPatch.cs`) benötigt ein Update, da die exakte `RackHolder`-Klasse in `Assembly-CSharp` noch verifiziert wird. Standalone-Modus (via Controller) wird derzeit genutzt.
