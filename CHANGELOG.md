# Changelog

## [Unreleased] — 1.2.2-dev.2026-08-13

> Arbeitsstand `1.2.2-dev.0`; noch nicht als Release veröffentlicht. Die Einträge werden bis zur Freigabe commitweise ergänzt.

### Added

- GregCore-Codebase-Analyse unter `docs/codebase/` mit Stack, Struktur, Architektur, Integrationen, Tests und Befunden.
- Aktivierter gemeinsamer Bootstrap für Plugin-Lifecycle, Settings, Performance-Governor, Main-Thread-Dispatcher und Ressourcenverwaltung.
- Gebundene Plugin-Abhängigkeiten mit deterministischer Reihenfolge sowie Fehlern für fehlende und zyklische Dependencies.
- Begrenzte Operation-Queues, Quality-abhängige Throttling-Intervalle und `Medium` als öffentliche Standardprofil-Bezeichnung.
- Begrenzte Notification-Warteschlange und maximale Anzahl aktiver UI-Toasts.
- Chronologischer Audit der 28 offenen PRs; redundante Sentinel-/Bolt-Varianten
  werden in eine geprüfte Integrationsänderung zusammengeführt.
- Security-Härtung der Portrait-Pfade und O(1)-Zugriff auf die autoritativen
  `NetworkMap`-Serverregister.
- Branch-Policy und Release-Dokumentation für `dev`, `pre-release`, `main` und
  unveränderliche `release/vX.Y.Z`-Snapshots.
- Open-PR- und Remote-Branch-Audit mit Konsolidierung auf einen Integrations-PR;
  222 redundante, nicht geschützte Branches wurden nach Prüfung entfernt.

### Fixed

- GregCore-Bootstrap wird jetzt vom tatsächlichen MelonLoader-Einstiegspunkt aufgerufen.
- Performance-Governor wird im echten Update-Pfad ausgeführt.
- `GregPerformanceModule.OnResourceUpdate` entfernt exakt den registrierten Handler.
- Legacy- und Public-Notification-APIs verwenden nun den vorhandenen UI-Manager statt leerer Implementierungen.
- Settings-Änderungen schreiben nicht mehr synchron bei jeder Änderung, sondern werden gedrosselt persistiert.
- Testprojekt schließt generierte `bin/`-/`obj/`-Quellen aus.
- Erzeugte Template-Artefakte werden nicht mehr als Quellcode eingecheckt.
- Nicht übernommene Nebenänderungen (unbegrenzte Steam-Queue, reflektive
  FishNet-RPCs und optionale Testvarianten) sind mit Begründung dokumentiert.
- Die eigene FishNet-/Relay-/`greg.Multiplayer`-Schicht und die zugehörigen
  Synchronisationswrapper wurden entfernt; Koop bleibt vollständig beim
  nativen Data-Center-Spiel.
- Die aktiven Steam-/P2P-Aufrufe aus dem alten FFI-Multiplayerpfad wurden
  entfernt; die v7-ABI-Slots bleiben ausschließlich als stabile No-op-Slots.
- Native Unity-/Steamworks-/Data-Center-Netzwerktypen wurden gegen die lokalen
  Referenz-Assemblies geprüft und mit Hashes und reproduzierbaren Befehlen in
  `docs/codebase/native-coop-assembly-audit.md` festgehalten.

### Verification

- Release-Build ohne Deployment: erfolgreich.
- Hook-Vertragsprüfung: 2 kanonische Hooks aus Manifest v2 validiert.
- Tests: 26/26 bestanden.
- In-Game-Smoke-Test gegen eine reale Data-Center-Installation: noch offen.
- Test-Build und VSTest-Lauf nach Entfernung der Eigenimplementierung:
  26/26 bestanden.

## Committed history since v1.2.1

### c6192214 — 2026-08-13 — `chore: remove obsolete FishNet agent instructions`

- Veraltete `.gemini`-Anweisungen für den nicht mehr zulässigen FishNet-Eigenstack
  entfernt.

### 77d4de32 — 2026-08-13 — `refactor(multiplayer): remove custom networking stack`

- Eigene FishNet-, Relay-, `dc_multiplayer.dll`- und Steam/P2P-Synchronisation
  entfernt; native Data-Center-Co-op-Grenze gegen die lokalen Assemblies
  dokumentiert.

### aed6b58c — 2026-08-13 — `docs: record branch consolidation and cleanup`

- Branch-, PR- und Release-Bereinigung sowie die Maintainer-Dokumentation
  festgehalten.

### f9837f15 — 2026-08-13 — `feat: consolidate GregCore integration and release flow`

- GregCore-Integration, Release-Flows, Versionierung, CI-Gates und die
  konsolidierte Integrationsbasis umgesetzt.

### 9366b777 — 2026-07-28 — `docs: add macOS support notice`

- Dokumentation um den macOS-Support-Hinweis ergänzt.

### 4d9f222e — 2026-07-28 — `security: migrate Lua hot-reload sandbox PRs [skip ci]`

- Sicherheitsänderungen aus den Lua-Hot-Reload-Sandbox-PRs übernommen.

### 2539edb8 — 2026-07-28 — `security: migrate Lua module sandbox PRs [skip ci]`

- Sicherheitsänderungen für die Lua-Modul-Sandbox übernommen.

### 618fea58 — 2026-07-28 — `perf: optimize legacy GameHooks rack counting [skip ci]`

- Rack-Zählung in den Legacy-GameHooks performance-optimiert.

### e7c346f6 — 2026-07-28 — `perf: remove global scene scans from public and Lua APIs [skip ci]`

- Globale Scene-Scans aus Public- und Lua-APIs entfernt.

### b4d43682 — 2026-07-28 — `perf: use cached device counts for facility metrics [skip ci]`

- Facility-Metriken auf gecachte Geräteanzahlen umgestellt.

### 5916a2d7 — 2026-07-28 — `security: harden Lua sandbox and employee identifiers [skip ci]`

- Lua-Sandbox und Employee-Identifier abgesichert.

## [1.2.1] - 2026-06-28

### Changed

- Auto-release: chore: sync all versions to 1.2.0


<!-- markdownlint-disable MD024 -->

All notable changes to this project are documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/).
Versioning follows Semantic Versioning 2.0.0. Development uses
`X.Y.Z-dev.N`, release candidates use `X.Y.Z-rc.N`, and stable releases use
`X.Y.Z`; stable versions are never auto-incremented on ordinary pushes.

## [Unreleased]

### Changed

- Initial unreleased section.

## [1.1.0] - 2026-04-28

### Added
- Full CI/CD pipeline: auto version bump, win+linux × MelonLoader+BepInEx releases
- API docs auto-generation from `game_hooks.json` and `framework/greg_hooks.json`
- `scripts/generate_api_docs.py` — generates `docs/FrameworkAPI.md`
- Grid placement system (`greg.GridPlacement`)
- Multiplayer HUD (`src/UI`)
- Lua REPL integration
- Rust FFI host bridge

### Fixed
- Resolved merge conflict in `GregPersistenceService.cs`
- Fixed `sponsor-tier-sync.yml` (`core` identifier conflict in github-script)
- Fixed workflow failures (incorrect project paths, hardcoded version strings)

## [1.0.0] - 2026-01-01

### Added
- gregCore mod framework initial release
- Multiple mods: WallRack, GridPlacement, UI, CommonShop, etc.
- Harmony hooking system (Prefix/Postfix)
- Save engine with versioning (LiteDB)
- Unit tests

[Unreleased]: https://github.com/mleem97/gregCore/compare/v1.1.0...HEAD
[1.1.0]: https://github.com/mleem97/gregCore/compare/v1.0.0...v1.1.0
[1.0.0]: https://github.com/mleem97/gregCore/releases/tag/v1.0.0
