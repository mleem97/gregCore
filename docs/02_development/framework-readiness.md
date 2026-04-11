---
id: framework-readiness
title: FRAMEWORK READINESS — gregCore vs Community Feature Requests
slug: /developers/framework-readiness
description: Structured readiness analysis for gregCore against the full A-H community requirement catalog.
---

## 1. Executive Summary

Analysierter Umfang: **43** Community-Anforderungen (A-01 bis H-03).

| Status | Anzahl | Bedeutung |
| --- | ---: | --- |
| ✅ READY | 1 | Mit aktuell verifizierten Hooks/APIs direkt als Mod umsetzbar |
| ⚠️ PARTIAL | 24 | Teilweise umsetzbar, benötigt Framework-Erweiterung |
| ❌ MISSING | 14 | Framework-Komponente fehlt zwingend |
| 🚫 GAME-LEVEL | 4 | Erfordert Spielquellcode oder nicht freigelegte Spielsystme |

Kernaussage: gregCore ist als **Hook-/Event-Backbone** solide (`GregNativeEventHooks`, `GregEventDispatcher`, `GregPayload`, `GregCompatBridge`), aber nicht vollständig als **Content-Registry-Framework** (typed registries, model-override service, domain services).

## 2. Analyse-Methodik

- **Priorität angewendet:** Wiki zuerst, Repository als Verifikation.
- **Wiki-Sources (parallel geprüft):**
  - `docs/02_development/concepts/hooks-and-events.md`
  - `docs/02_development/api-reference/hooks-catalog.md`
  - `docs/content-creation/implementation-backlog.md`
- **Repository-Sources (parallel geprüft):**
  - `gregCore/gregSdk/gregNativeEventHooks.cs`
  - `gregCore/gregSdk/gregEventDispatcher.cs`
  - `gregCore/gregSdk/gregPayload.cs`
  - `gregCore/gregSdk/gregCompatBridge.cs`
  - `gregCore/gregMain.cs`
  - `gregCore/gregModLoader/Plugins/gregPluginBase.cs`
  - `gregCore/gregModLoader/Plugins/gregRegistry.cs`

Bewertungslogik:

- ✅ READY: Ohne neue Framework-Verträge realisierbar.
- ⚠️ PARTIAL: Technisch startbar, aber stabiler Betrieb braucht SDK/Plugin-Erweiterung.
- ❌ MISSING: Notwendige Framework-Fähigkeit existiert nicht.
- 🚫 GAME-LEVEL: Ohne tiefe, nicht dokumentierte Spielintegration nicht realistisch.

## 3. Readiness-Matrix

| ID | Feature | Status | Verfügbare Hooks/APIs | Fehlende Komponente | Layer |
| --- | --- | --- | --- | --- | --- |
| A-01 | Mitarbeiter tanzen bei Idle | ⚠️ PARTIAL | `greg.EMPLOYEE.CustomHired`, `greg.SYSTEM.ButtonConfirmHire` | Employee idle-state hooks/service | harmony + SDK |
| A-02 | Mitarbeiter-KI erweiterbar | ❌ MISSING | Hire/fire hooks, EventBus | AI state-machine extension API | harmony + SDK |
| A-03 | Neue Mitarbeiter-Typen | ⚠️ PARTIAL | `GregEventDispatcher`, `GregPayload` | `GregEmployeeRegistry` | SDK + Plugin |
| B-01 | Vorkonfigurierte Switches | ⚠️ PARTIAL | `greg.SYSTEM.ButtonBuyShopItem`, `greg.NETWORK.InsertSFP` | Purchase post-processor service | harmony + SDK |
| B-02 | Switch-Port-Konfigurator | ❌ MISSING | Network hooks vorhanden | Per-port config model/API | SDK |
| B-03 | Mehr Rear-Ports | ⚠️ PARTIAL | Cable/register hooks | `GregSwitchRegistry` | SDK |
| B-04 | 1HE High-Density-Switches | ⚠️ PARTIAL | Rack/server hooks | Rack geometry/constraints API | harmony + SDK |
| B-05 | Massenbestückung Modul-Box | ⚠️ PARTIAL | SFP hooks | Batch port operation service | SDK |
| B-06 | Kabel-Manager auto-routing | ❌ MISSING | Cable create/remove hooks | Topology routing service | SDK + Plugin |
| B-07 | Cable-Audit-Mode | ⚠️ PARTIAL | Cable/link hooks, NetWatch | Topology audit + highlight adapter | Plugin + UI |
| B-08 | Farbenblind-Modus Kabeltypen | ✅ READY | UI overlay + hook data | Optional helper only | Mod Layer |
| B-09 | WDM-Infrastruktur | 🚫 GAME-LEVEL | Generic network hooks only | Optical simulation model | game-level |
| B-10 | Managed Switch VLAN/Tagging | ❌ MISSING | Link/connect events | VLAN policy engine | SDK + gameplay |
| B-11 | DHCP-Server Feature | ❌ MISSING | Event surface limited | Lease/allocation service | SDK + gameplay |
| B-12 | Größere Port-Boxen | ⚠️ PARTIAL | Shop events | Inventory container registry | SDK |
| C-01 | Höhertier-Server | ⚠️ PARTIAL | `greg.SERVER.*`, shop hooks | `GregServerRegistry` + balancing | SDK |
| C-02 | 1HE-Server | ⚠️ PARTIAL | rack mount/unmount hooks | Rack-unit constraint API | harmony + SDK |
| C-03 | Server-IOPS-Upgrade | ⚠️ PARTIAL | server/customer hooks | Upgrade path registry | SDK |
| C-04 | Höhertier-Kabel/Ports | ⚠️ PARTIAL | cable speed/sfp hooks | Cable/SFP typed registries | SDK |
| C-05 | iPerf/Speedtest im Spiel | ⚠️ PARTIAL | `greg.NETWORK.NetWatchDispatched` | Test session orchestration API | Plugin + SDK |
| D-01 | Unterschiedliche Kundenanforderungen | ⚠️ PARTIAL | `greg.SYSTEM.ButtonCustomerChosen`, customer req hooks | `GregCustomerRegistry` + rule engine | SDK |
| D-02 | Zufällige Kundenanforderungen | ⚠️ PARTIAL | customer acceptance hooks | Requirement generator service | Plugin + SDK |
| D-03 | Dynamische Preisschwankungen | ⚠️ PARTIAL | shop hooks | Dynamic pricing service | Plugin + SDK |
| D-04 | Stromverbrauch/Stromkosten | ⚠️ PARTIAL | server power hooks | Energy simulation service | SDK + gameplay |
| D-05 | Auto-IP-Vergabe | ⚠️ PARTIAL | server app-id and network hooks | IP allocation service | SDK |
| E-01 | Beschriftungs-Tapes | ⚠️ PARTIAL | rack/network hooks | Persistent label store + UI input | Plugin + UI |
| E-02 | Rack-Kopieren inkl. Kabel | ⚠️ PARTIAL | rack/network hooks | Topology clone transaction API | SDK + harmony |
| E-03 | HexViewer-Panel | ⚠️ PARTIAL | verified event bus + hooks | Unified metadata adapter for all entities | Plugin + SDK |
| E-04 | Inventar-Slots / Item-Leiste | ❌ MISSING | no verified inventory UI API | Inventory UI extension API | UI + Plugin |
| E-05 | Kühlung/Abwärme Anzeige+Gameplay | 🚫 GAME-LEVEL | no thermal hook surface | Cooling simulation model | game-level |
| F-01 | Lichter / Deckenlampen | ❌ MISSING | wall purchase hook only | Lighting registry + runtime controls | harmony + SDK |
| F-02 | Mehrere Stockwerke/Türen/Wände | 🚫 GAME-LEVEL | no structural building API | world-building systems | game-level |
| F-03 | Transport-Automatisierung | ❌ MISSING | no logistics hooks verified | transport actor framework | SDK + gameplay |
| F-04 | Dustpan & Brush cleanup | ⚠️ PARTIAL | `greg.SYSTEM.oveSpawnedItemRemoved` | world item type/action API | Plugin + SDK |
| F-05 | Stromversorgung für Racks | ⚠️ PARTIAL | power hooks available | power topology graph service | SDK |
| F-06 | Kühlungsmanagement aktiv | 🚫 GAME-LEVEL | no cooling event surface | cooling system integration | game-level |
| G-01 | Dynamische Events | ⚠️ PARTIAL | day/save/load hooks | incident scheduler/orchestrator | Plugin + SDK |
| G-02 | Hacker/DDoS/Firewall | ❌ MISSING | NetWatch + generic hooks | security simulation framework | Plugin + SDK + gameplay |
| G-03 | Kabel-Audit Farb-Highlights | ⚠️ PARTIAL | cable/netwatch hooks | topology analyzer + highlight API | Plugin + UI |
| G-04 | Performance-Mod | ⚠️ PARTIAL | `gregMain` lifecycle proof (`OnUpdate`, `OnGUI`) | standardized performance profile service | Plugin + SDK |
| H-01 | Austausch bestehender 3D-Modelle | ❌ MISSING | no official model override API | `GregModelOverrideService` | Plugin + SDK |
| H-02 | Eigene Modelle für neue Inhalte | ⚠️ PARTIAL | data-driven conventions possible | official model binding contract | SDK + Plugin |
| H-03 | NPC-Modelle für neue Mitarbeiter | ❌ MISSING | employee events only | employee visual binding registry | SDK + harmony |

## 4. Detailanalyse pro Kategorie

### A — Employee / NPC
- Heute möglich: Reaktion auf Einstellungs-/Entlassungsereignisse.
- Fehlend: Idle-state und AI-state hooks; registrierbare employee classes.
- Ziel-Layer: `gregSdk` + `harmony`.

### B — Netzwerk / Switches / Ports
- Heute möglich: Beobachtung von Kabel-, SFP-, Link- und Speed-Ereignissen.
- Fehlend: Typed registries + compatibility matrix + route planner.
- Ziel-Layer: `gregSdk` zuerst, Plugin-Tools danach.

### C — Server / Hardware
- Heute möglich: Server lifecycle/power/broken/repair Observability.
- Fehlend: server types, density constraints, upgrade contracts.
- Ziel-Layer: `gregSdk` + selective `harmony`.

### D — Kunden / Wirtschaft
- Heute möglich: customer selection + requirement pass/fail events.
- Fehlend: customer policy engine, randomizer, dynamic pricing, power economy.
- Ziel-Layer: `gregSdk` + plugin policy modules.

### E — UI / QoL
- Heute möglich: native OnGUI overlays and hook-driven panels.
- Fehlend: unified metadata adapter, inventory UI extension, persistent labels.
- Ziel-Layer: Plugin + SDK contracts.

### F — Umgebung / Gebäude
- Heute möglich: begrenzte Reaktionen auf item remove / wall purchase.
- Fehlend: strukturelle Gebäude- und Klimasysteme.
- Ziel-Layer: überwiegend game-level, teils `harmony`-heavy.

### G — Events / Gameplay
- Heute möglich: event triggering via day/save/load + telemetry hooks.
- Fehlend: orchestrator for incidents and security scenario system.
- Ziel-Layer: Plugin orchestrator + SDK contracts.

### H — 3D / Visual
- Heute möglich: partielle model references in mod-side data.
- Fehlend: official model override/binding API with fallback/conflict handling.
- Ziel-Layer: Plugin runtime + SDK contract.

## 5. Fehlende Framework-Komponenten (konsolidiert)

### FEHLT: GregContentRegistry (typed categories)
**Betrifft Features:** A-03, B-03, B-12, C-01, C-03, C-04, D-01, H-02  
**Layer:** `framework/Sdk/`  
**Warum nötig:** Ohne typed registries bleiben Inhalte modlokal und nicht frameworkweit konsistent.

### FEHLT: GregNetworkCompatibilityService
**Betrifft Features:** B-01, B-02, B-05, B-06, B-10, B-11, C-04  
**Layer:** `framework/Sdk/` + plugins  
**Warum nötig:** Port-/SFP-/Kabel-Regeln brauchen zentralen, reproduzierbaren Resolver.

### FEHLT: GregEmployeeStateService
**Betrifft Features:** A-01, A-02, H-03  
**Layer:** `framework/harmony/` + `framework/Sdk/`  
**Warum nötig:** Idle/AI transitions sind aktuell nicht als stabile Hook-Oberfläche verfügbar.

### FEHLT: GregModelOverrideService
**Betrifft Features:** H-01, H-02, H-03, E-03  
**Layer:** plugins + SDK  
**Warum nötig:** Kein offizieller runtime override lifecycle mit fallback und Konfliktauflösung.

### FEHLT: GregCustomerPolicyEngine
**Betrifft Features:** D-01, D-02, D-05  
**Layer:** SDK  
**Warum nötig:** Kundenregeln/Budget/Assignment sind nicht als offizieller Evaluator modelliert.

### FEHLT: GregIncidentOrchestrator
**Betrifft Features:** G-01, G-02  
**Layer:** plugins + SDK hooks  
**Warum nötig:** Dynamische Ereignisse brauchen Scheduler + lifecycle/state persistence.

## 6. MISSING Status (Language Bridges)

### 6.1 Konsolidierung aller aktuellen MISSING-Dateien

| Datei | Fehlende Bereiche | Layer |
| --- | --- | --- |
| `plugins/greg.Plugin.HexViewer/lua/MISSING.md` | Lua event bridge, payload bridge, HUD bridge, targeting bridge | `framework/ModLoader`, `framework/Sdk` |
| `plugins/greg.Plugin.HexViewer/ts/MISSING.md` | TS event bridge, payload bridge, HUD bridge, targeting bridge | `framework/ModLoader`, `framework/Sdk` |
| `plugins/greg.Plugin.HexViewer/rs/MISSING.md` | Rust event bridge, payload bridge, HUD bridge, targeting bridge | `framework/ModLoader`, `framework/Sdk` |

### 6.2 Zusammengefasste Lücken aus allen MISSING-Dateien

- Es fehlt eine **dokumentierte LanguageBridge-Vertragsfläche** für Lua/TS/Rust (Subscribe, Update, GUI callbacks).
- Es fehlt eine **sprachübergreifende Payload-Bridge** mit stabilen Getter-Signaturen analog zu `GregPayload.Get<T>(...)`.
- Es fehlt eine **native HUD-Bridge** (IMGUI proxy API) für Nicht-C#-Sprachen.
- Es fehlt eine **Targeting-Bridge** (Camera-forward Raycast) in allen Nicht-C#-Bridges.
- Ergebnis: HexViewer in Lua/TS/Rust bleibt **UNVERIFIED** bis die Bridge-Verträge offiziell dokumentiert und implementiert sind.

## 7. Empfohlener Implementierungsplan

### Phase 1 – Framework-Fundament (BLOCKING)
- Typed content registries
- Schema validation + cross-reference checks
- Network compatibility service
- Employee state hook expansion

### Phase 2 – Feature-Layer
- Customer policy engine
- Model override service (with fallback/conflict)
- Topology audit service
- UI metadata adapter

### Phase 3 – Mod-Layer (externe Modder)
- Content packs für server/switch/customer/cable/sfp/furniture
- QoL plugins (labels, cable audit, performance modes)

### Phase 4 – Long-Term / GAME-LEVEL
- Features mit tiefer Spielsystem-Abhängigkeit (cooling simulation, advanced building, WDM, security minigame)
