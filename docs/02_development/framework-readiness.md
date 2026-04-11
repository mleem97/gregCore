---
id: framework-readiness
title: gregCore Framework Readiness Analysis
description: Structured readiness analysis for community feature requests against current gregCore wiki and repository evidence.
slug: /developers/framework-readiness
---

## 1. Executive Summary

Assessment scope: **43** community requirements (A-01 .. H-03).

| Classification | Count | Meaning |
| --- | ---: | --- |
| ✅ READY | 1 | Can be built now as external mod with currently verified hooks/APIs |
| ⚠️ PARTIAL | 24 | Partly possible; framework extension needed for robust/official support |
| ❌ MISSING | 14 | Required framework capability not present/documented |
| 🚫 GAME-LEVEL | 4 | Requires game-level systems/source behavior outside documented gregCore control |

**High-level conclusion:** gregCore currently provides a strong **event/hook foundation** (`gregNativeEventHooks`, `gregEventDispatcher`, `gregPayload`, `gregCompatBridge`) but lacks first-class **typed content registries**, **model override service**, and several **domain-specific runtime services** needed for full custom-content ecosystem support.

## 2. Analyse-Methodik

- **Source priority applied:** Wiki first, repository second.
- **Wiki evidence used:**
  - `docs/02_development/concepts/hooks-and-events.md`
  - `docs/02_development/api-reference/hooks-catalog.md`
  - `docs/content-creation/implementation-backlog.md`
- **Repository evidence used (parallel check):**
  - `gregSdk/gregNativeEventHooks.cs`
  - `gregSdk/gregEventDispatcher.cs`
  - `gregSdk/gregPayload.cs`
  - `gregSdk/gregCompatBridge.cs`
  - `gregMain.cs`
  - `gregModLoader/Plugins/gregPluginBase.cs`
  - `gregModLoader/Plugins/gregRegistry.cs`
- **Status criteria:**
  - **✅ READY:** required behavior can be expressed with verified hooks/APIs and no new framework contract.
  - **⚠️ PARTIAL:** foundation exists, but stable/maintainable delivery needs SDK/plugin additions.
  - **❌ MISSING:** no documented/verified framework primitive for required capability.
  - **🚫 GAME-LEVEL:** needs game-internal systems beyond documented modding surface.

## 3. Readiness-Matrix

| ID | Feature | Status | Verfügbare Hooks/APIs | Fehlende Framework-Komponente | Layer |
| --- | --- | --- | --- | --- | --- |
| A-01 | Employee idle dance | ⚠️ PARTIAL | `greg.SYSTEM.ButtonConfirmHire`, `greg.EMPLOYEE.CustomHired` | `GregEmployeeStateService` idle-state exposure | harmony + SDK |
| A-02 | Extend employee AI states/behaviors | ❌ MISSING | Hire/fire related events only | AI behavior registry + state machine extension API | harmony + SDK |
| A-03 | New employee types with skills/specialization | ⚠️ PARTIAL | Event bus + payload helpers | Typed employee-class registry | SDK + Plugin |
| B-01 | Preconfigured switches at purchase | ⚠️ PARTIAL | `greg.SYSTEM.ButtonBuyShopItem`, `greg.NETWORK.InsertSFP` | Purchase-time postprocessor for auto-port provisioning | harmony + SDK |
| B-02 | Port-profile switch configurator | ❌ MISSING | Cable/SFP events available | Port profile model + per-port config API | SDK |
| B-03 | High rear-port-count switches | ⚠️ PARTIAL | Network cable/connect events | Switch type/content registry | SDK + Plugin |
| B-04 | 1HE density + matching high-port switches | ⚠️ PARTIAL | Rack/server/switch hooks | Rack-unit constraints + SKU integration API | harmony + SDK |
| B-05 | Bulk populate ports from module box | ⚠️ PARTIAL | `greg.NETWORK.InsertSFP`, `greg.SYSTEM.ButtonBuyShopItem` | Batch operations service for ports | SDK |
| B-06 | Cable manager (auto-route tool) | ❌ MISSING | `greg.NETWORK.CreateNewCable`, remove/speed hooks | Route planner + safe auto-connect API | SDK + Plugin |
| B-07 | Cable audit mode with wrong-link highlight | ⚠️ PARTIAL | Cable created/removed/link hooks | Topology validation + visual highlight bridge | Plugin + UI |
| B-08 | Colorblind cable mode | ✅ READY | Existing UI/mod event surface + model/UI overrides possible | None strictly required (optional UI helper service) | Mod Layer + UI |
| B-09 | WDM infrastructure | 🚫 GAME-LEVEL | Generic cable events only | Optical/wavelength simulation systems not exposed | game-level + harmony |
| B-10 | Managed switch with VLAN/tagging/trunk | ❌ MISSING | Link/register hooks | L2 config model + packet policy simulation API | SDK + gameplay systems |
| B-11 | DHCP server feature | ❌ MISSING | Limited network hooks | IP lease service + client stack integration | SDK + gameplay systems |
| B-12 | Larger port boxes | ⚠️ PARTIAL | Shop and SFP insertion events | Inventory/container definition registry | SDK |
| C-01 | Higher-tier configurable servers | ⚠️ PARTIAL | `greg.SERVER.*`, shop/customer hooks | Server type registry + balancing service | SDK |
| C-02 | 1HE servers | ⚠️ PARTIAL | Rack/server install/unmount hooks | Rack slot geometry constraints API | harmony + SDK |
| C-03 | Server IOPS upgrade tiers | ⚠️ PARTIAL | Server/customer events | Upgrade path registry + stats recomputation contract | SDK |
| C-04 | Higher-tier cables/ports configurable | ⚠️ PARTIAL | Cable create/remove/speed/sfp hooks | Typed cable/SFP registries + compatibility matrix | SDK |
| C-05 | iPerf/speedtest server+client | ⚠️ PARTIAL | `greg.NETWORK.NetWatchDispatched`, speed hooks | Test-session service + per-link telemetry API | Plugin + SDK |
| D-01 | Different customers with richer requirements | ⚠️ PARTIAL | `greg.SYSTEM.ButtonCustomerChosen`, customer req satisfied/failed | Customer definition registry + rule engine | SDK |
| D-02 | Random configurable customer requirements | ⚠️ PARTIAL | Customer acceptance/satisfaction hooks | Requirement generator service + seeding config | Plugin + SDK |
| D-03 | Dynamic hardware price fluctuations | ⚠️ PARTIAL | Shop hooks (`ButtonBuyShopItem`, checkout) | Pricing policy service with lifecycle hooks | Plugin + SDK |
| D-04 | Power usage and electricity cost gameplay | ⚠️ PARTIAL | Server/switch power/broken hooks | Global energy simulation and billing API | SDK + gameplay layer |
| D-05 | Auto IP assignment | ⚠️ PARTIAL | `greg.SERVER.AppIDChanged`, network link hooks | IP allocation service and conflict resolver | SDK |
| E-01 | Label tapes for racks/cables (free text) | ⚠️ PARTIAL | Rack/network hooks, payload API | Persistent labeling store + UI input bridge | Plugin + UI |
| E-02 | Duplicate full rack including cables | ⚠️ PARTIAL | Rack unmount + cable events | Topology clone transaction API | SDK + harmony |
| E-03 | HexViewer-like universal info panel | ⚠️ PARTIAL | `gregEventDispatcher`, hooks/events, `gregMain` OnGUI proof | Unified object metadata adapter for all entities | Plugin + SDK |
| E-04 | Bottom inventory slots/item bar | ❌ MISSING | Shop/cart hooks only | Inventory UI extension points + input binding service | UI + Plugin |
| E-05 | Cooling/heat management display + gameplay | 🚫 GAME-LEVEL | No verified thermal hooks | Thermal simulation systems not exposed | game-level |
| F-01 | Lights off / buyable ceiling lamps | ❌ MISSING | Wall/shop events only | Lighting object registry + world-light controls | harmony + SDK |
| F-02 | Multi floors, doors, windows, chairs | 🚫 GAME-LEVEL | No verified structural build API | Structural world generation/building systems | game-level |
| F-03 | Wall rail transport automation | ❌ MISSING | No logistics hooks verified | Conveyor/transport actor framework | SDK + game logic hooks |
| F-04 | Dustpan & brush cleanup trays | ⚠️ PARTIAL | `greg.SYSTEM.oveSpawnedItemRemoved` | World-item classification + cleanup action API | Plugin + SDK |
| F-05 | Rack power-supply mechanics | ⚠️ PARTIAL | Server/switch power hooks | Power topology model + failure propagation | SDK |
| F-06 | Active cooling management and failures | 🚫 GAME-LEVEL | No cooling event surface verified | Cooling simulation + environment model | game-level |
| G-01 | Dynamic events (power/cooling/traffic spikes) | ⚠️ PARTIAL | `SystemGameDayAdvanced`, power hooks, NetWatch | Event scheduler + incident framework | Plugin + SDK |
| G-02 | Cyber events (hacker, DDoS, antivirus minigame) | ❌ MISSING | NetWatch and basic network hooks | Security scenario framework + threat simulation APIs | Plugin + SDK + gameplay |
| G-03 | Cable audit with color highlights | ⚠️ PARTIAL | Cable/link hooks + NetWatch | Topology analyzer + highlight rendering adapter | Plugin + UI |
| G-04 | Performance mode (disable effects/reduce textures) | ⚠️ PARTIAL | `gregMain` update/gui lifecycle proves runtime toggle pattern | Standardized performance profile service | Plugin + SDK |
| H-01 | Replace existing 3D models | ❌ MISSING | No official model override API documented | `GregModelOverrideService` (registry + fallback) | Plugin + SDK |
| H-02 | Custom models for new content | ⚠️ PARTIAL | Content files + mod loading possible | Official model binding contract per content type | SDK + Plugin |
| H-03 | Visual NPC models for new employee types | ❌ MISSING | Employee custom hire/fire events only | Employee visual binding/skin registry | SDK + harmony |

## 4. Zusammenfassung für Beitrag-Entwickler

**Highest-impact Top 3 extensions:**

1. **Typed content registries** (`GregContentRegistry` family)
2. **Model override service** (official replacement + fallback)
3. **Network compatibility kernel** (switch/SFP/cable matrix)

These three unlock the broadest part of A–H requests at once.
