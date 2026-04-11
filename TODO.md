# 📋 gregCore Roadmap to 100% Coverage

Dieses Dokument ist der zentrale Masterplan für das Greg Framework. Es wird laufend aktualisiert, um mit der Entwicklung von *Data Center* Schritt zu halten.

---

## ✅ Phase 0: Baseline (Abgeschlossen)
- [x] ~~Initiales Standalone-Repository Setup~~
- [x] ~~Namespace-Migration zu `gregModLoader`~~
- [x] ~~Basis-Struktur für MelonLoader Mods~~

## ✅ Phase 1: Framework Foundation (Abgeschlossen)
- [x] ~~Typsicheres Registrierungssystem (`GregContentRegistry`)~~
- [x] ~~Basis-Definitionen für Hardware (Server, Switches)~~
- [x] ~~Zentraler Event-Bus (`gregEventDispatcher`)~~

## ✅ Phase 2: Runtime Integration (Abgeschlossen)
- [x] ~~Implementierung der `gregSdk` Services~~
- [x] ~~Kunden- und Mitarbeiter-Registries~~
- [x] ~~Netzwerk-Kompatibilitäts-Prüfung (`GregNetworkCompatibilityService`)~~

## ✅ Phase 3: Visual Layer & Overrides (Abgeschlossen)
- [x] ~~Offizieller Modell-Ersetzungs-Dienst (`GregModelOverrideService`)~~
- [x] ~~Prioritätsbasiertes Konflikt-Management für Mods~~
- [x] ~~JADE-Style HUD Referenz-Implementierung~~

## ✅ Phase 4: Quality & API Expansion (Abgeschlossen)
- [x] ~~30+ Normalisierte Unity-Signale (`GregNativeEventHooks`)~~
- [x] ~~Automatisierte Unit-Tests in `gregCore.Tests`~~
- [x] ~~9 System-Bridges (Shop, Time, Player etc.)~~
- [x] ~~Pre-Release v1.0.0-pre.5 veröffentlicht~~

## 🚧 Phase 5: Economy & Data (In Arbeit)
- [ ] **SaveSystem & SaveData**: Abstraktion des Speichervorgangs. Ermöglicht Custom-Save-Daten für Mods.
- [ ] **BalanceSheet & Finanzen**: Zugriff auf Einnahmen, Ausgaben und historische Daten.
- [ ] **Localisation**: Bridge zum internen Übersetzungssystem für mehrsprachige Mods.

## 📅 Phase 6: Deep Hardware & UI (Geplant)
- [ ] **CableLink & SFPModule**: Detailsteuerung für Kabeltypen und SFP-Zustände (Hitze, Verschleiß).
- [ ] **PatchPanel**: Interaktion mit passiver Netzwerk-Hardware.
- [ ] **Tooltip & UI_Section**: Erweiterte UI-Manipulation über das HUD hinaus.
- [ ] **AudioManager**: Steuerung von Soundeffekten und Umgebungsmusik.

## 📅 Phase 7: Environment & Simulation (Geplant)
- [ ] **HRSystem**: Tiefere Integration in das Mitarbeiter-Management (Boni, Level-Up).
- [ ] **SteamManager**: Zugriff auf Steam-Achievements und Leaderboards.
- [ ] **Objectives**: Manipulation von Tutorial- und Haupt-Zielen.

## 🔄 Phase 8: Unity Scripting API Integration (Laufend)
- [ ] **Unity API Abgleich**: Systematischer Abgleich mit der offiziellen Unity Scripting API.
- [ ] **Standard Module**: Integration von Physics, SceneManagement und NavMesh-Bridges.

---
> 💡 **Hinweis zur Roadmap:** Diese Roadmap ist ein **lebendes Dokument**. Da sich *Data Center* ständig weiterentwickelt, werden neue Systeme laufend in Phase 8 aufgenommen und bei Bedarf neue Phasen definiert.

## Technischer Status
- **Aktuelle Coverage**: ~65% der IL2CPP-Kernlogik bridged.
- **Letzter Snapshot-Abgleich**: 11. April 2026
