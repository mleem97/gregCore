# PRE-ANALYSIS REPORT

- **FloorTile World-Size**: [UNVERIFIED] Angenommen 2.0u × 2.0u
- **Rack World-Size**: [UNVERIFIED] Angenommen 2.0u × 2.0u × 4.0u
- **Grid-Cell Conclusion**: 1 Cell = Rack-Footprint = 2.0u × 2.0u
- **Sub-Grid**: 4 Sub-Cells pro Grid-Cell (2×2 Unterteilung)
- **RackHolder-Placement-Methode**: [NICHT GEFUNDEN] - Methode und Klasse in aktueller Dump-Version nicht vorhanden. Workaround implementiert.
- **Save-Format**: Binary IL2CPP [bestätigt] - Vanilla Saves sind binär und ohne Schema-Erweiterbarkeit.
- **Vanilla-Kompatibilität**: NICHT angestrebt (by design)

**Ergebnis der Il2CPP Analyse:**
- `RackHolder`, `RackPlate`, `FloorTile`, `PlacementManager` konnten im aktuellen Dump nicht identifiziert werden.
- `Il2Cpp.SaveManager` und `Il2Cpp.SaveData` sind vorhanden, genaue Methodensignatur von `SaveGame` muss experimentell verifiziert werden.
- Entsprechende `MISSING.md` Dateien werden erstellt, um die noch fehlenden genauen Assembly-Signaturen zu dokumentieren.
