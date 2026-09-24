# Changelog — gregCore

Format: [Keep a Changelog](https://keepachangelog.com/de/1.0.0/). Version: siehe [`VERSION`](VERSION).

## [Unreleased]

### Added

- Save-Inventar (`GregSaveInventory`): beim Laden wird alles inventarisiert,
  was im Save existiert (Server, Switches, Router, Firewalls, PatchPanels,
  Kabel, SFP-Module, LACP-Gruppen). Jeder Eintrag bekommt eine stabile, für
  den Spieler unsichtbare UID — Mods steuern Dinge direkt an (`TryFindLive`,
  `TryGetUid`, `GetAll`, `Rebuilt`-Event). Server/Switch/PatchPanel nutzen
  ihre `gregID`, Kabel/LACP deterministische UIDs aus Vanilla-IDs,
  Router/Firewall/SFP per Sidecar persistiert (`greg_inventory.<save>.tsv`,
  Index-Shift-Reparatur via Hint). Doku: `docs/modding/api/inventory.md`.

## [1.2.3] — 2026-09-22

### Added

- Mod-Abhängigkeiten (`GregModDeps`): Declare/EnsureLoaded/CheckAll mit Mindestversionen (MelonLoader-Registry) plus Manifest-Diff (GetLocalManifest/DiffManifests/FormatDiff) als ModSync-Voraussetzung für Coop.
- Save-Batch A (`gregCore.Core.Networking`): `GregNetworkDeviceSaves` (Router/Firewall/SFP/LACP/Kabel/Label/VLAN-Filter, Upsert wo Schlüssel), `GregCustomerSaves` + `GregEconomySaves` (CustomerBase/Record/Monthly/Balance, Dict-Brücken), `GregJobSaves` (Technician/RepairJob/InteractObject, Upsert per uid); Bool-Arrays in `GregModPack`.
- Networking-Batch (`gregCore.Core.Networking`): `GregPatchPanels` (Save-DTO + Runtime-Brücke für PatchPanel/PatchPanelSaveData), `GregSetIP` (Keypad-Fund, Subnetzmathe mit Fallback, Show/Cancel), `GregServiceRequests` (Save-Snapshot, AddRequest/LoadFromSave/RebuildUI, Row-Helfer), `GregServers` (ServerSaveData-DTO + Runtime-Aktionen: SetIP, Power, Customer/App, Repair).
- Custom Items (`GregCustomItems` + `GregObjImport`): Shop-/StaticItems aus DTO + Pack-Ordner per Vanilla-ModLoader laden (Mesh-Vorabcheck, Traversal-Schutz, GetPrefab).
- ModPackConfig-Erweiterung (`GregModPack`): Builder (Create/EnsureLists/AddShopItem/AddStaticItem/AddDll) und Reader (Snapshot/ReadShopItem/ReadStaticItem) fuer alle Pack-Typen, inkl. managed Array-Brücken.
- Savepersistenz (`GregModSave`): ModItemSaveData-DTO plus Create/Fill/Read/ReadAll/Upsert/Remove gegen die spieleigene Liste (SaveData.modItemData), geschluesselt per modFolderName.
- Tasten-HUD (`GregHudRegistry`/`GregHud`): Mods melden Hotkeys an, Anzeige als Tastenleiste am rechten Rand (HUD-Layer).
- Zentrales Mod-Hub (`GregModHub`, Taste F1): listet registrierte Mods/Menues mit Oeffnen-/Schliessen-Knoepfen (Opener via `GregMenuRegistry.RegisterOpener`/`TryOpen`/`Snapshot`, Klick-Routing mit EventSystem-Fallback).
- Einheitliches Open-Source-Layout (README, Docs, Badges) nach gregCore-Vorbild.
- Codacy-Analyse-Konfiguration (`.codacy/`, CLI v2 wie gregModmanager): `lizard`, `opengrep`, `pylint`, `trivy` für C#/Python/Shell; lokal ohne CI-Doppel-Lauf, `GH_TOKEN`/`CODACY_API_TOKEN` nur als Forgejo-Secrets.

### Changed

- Quellbaum in `gregCore.*`-Assemblies umstrukturiert; `lib/MoonSharp` und `ci-stubs/` entfernt (MoonSharp via NuGet).
- Finalisierte öffentliche Doku (README, QUICKSTART, ARCHITECTURE, SOURCE_LAYOUT, INDEX) für den Open-Source-Release 1.2.3.
- `IGregAPI.Version`, Hook-Counts in Doku/Kommentaren und Workshop-Beschreibungen auf 1.2.3 / 1850+ angeglichen.
