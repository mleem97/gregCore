# Changelog — gregCore
<!-- markdownlint-disable MD024 -- no-duplicate-heading does not apply to Keep-a-Changelog version sections -->

Format: [Keep a Changelog](https://keepachangelog.com/de/1.0.0/). Version: see [`VERSION`](VERSION).

## [Unreleased]

### Added

- Rust native SDK: `templates/rust/` (`Cargo.toml`, safe `src/greg.rs`
  bindings mirroring the ABI v1 table field-for-field with compile-time
  layout asserts, example `src/lib.rs`) and refreshed
  `examples/Rust/greg_example/` (full event/hook coverage on the new
  bindings, fixing the old example's table-order mismatch). Verified with
  `cargo check` + `clippy` + `rustfmt`. Guidebook track:
  `Guidebook Rust 01 Setup` + `Guidebook Rust 02 Project`.
- GregLint (own repo `github.com/mleem97/gregLint`): offline static
  analysis + auto-fix for mods (GL001–GL007 IL rules via Cecil,
  GL101–GL105 layout/manifest/mod.json rules, `--fix` with backup,
  text/JSON output, exit codes, custom-rule extensibility). 55 tests,
  99.05% line coverage. Guide: `.wiki/Developer-GregLint.md`.
- Wiki: GregCore Collection on Steam linked (`Home`, `Player Getting
  Started`, `Player Installation`, `Developer Publishing`).
- CommonShop completion (compatible, no breaking change):
  `ShopAPI.Initialize(Harmony)` (idempotent), persistent custom ID registry
  (`UserData/gregCore/CommonShop_CustomIDs.json`, legacy migration +
  format upgrade), real category/subcategory containers
  (`ShopUI.EnsureCategoryContainer/FixGridHeight/UpdateLayoutHeight`),
  custom cards with BCG tint, icon, `CustomPrefab`, cart stacking and
  `OnBuy` (`ShopCard`, with legacy `ShopItemSO` fallback incl.
  template button restore), `VanillaCategory`/`SetCategory`, live injection
  into the open shop.
- Custom color presets: purchases with a custom color are automatically stored as
  presets (`UserData/gregCore/CustomItemPresets.json`,
  legacy migration, dedup), appear in the shop category `Mods`
  (color, price, icon, unlock gating with grey-out) and create colored
  cart entries. Fix: plain purchase on a custom cart entry
  creates the correct plain line (ID masking); refresh on unlock.
- Modpack manifest (`Mods/manifest.json`: `Name`/`Mods`/`Library`/
  `Plugins`, relative to `Mods/`): validation (`.dll` only, no
  path traversal, never `.deactivated`), warn log, `Library` folders
  are additionally searched in the assembly resolver. Without a manifest
  no behavior change.
- ComputerUI extensions (`GregComputer`, `GregComputerPatch`,
  `greg.computer` Lua module): mods register custom shortcuts on the
  in-game computer main screen (`ComputerShop.ButtonReturnMainScreen` /
  `OpenShop` postfix injection, UGUI template cloning with
  `greg-computer-` dedup, cleanup on `CloseShop` / `HideCanvas`) and
  own pages/apps (`GregPanelBuilder` frame page with Back button +
  input lock for C#, tablet page with `panel_add_*` content for Lua).
  Events: `greg.COMPUTER.ShortcutClicked`, `greg.COMPUTER.AppOpened`,
  `greg.COMPUTER.AppClosed`. Docs: `docs/modding/computer-ui.md`.

### Changed

- English-only user-facing strings: remaining German `GregObjImport`
  warnings translated (`Import returned null`, `Import has no vertices`,
  `Path escapes the pack folder`, `Empty file path`, `File too large`);
  Rust legacy-dir warning translated; wiki log tables updated to the
  exact messages.
- SaveGuard backup guarantee: no framework write (sidecars, sanitize-gated
  paths) happens without a fresh vanilla backup first — if the backup
  fails, mod writes for that save are skipped while the game's own save
  proceeds untouched. Backups now also cover the `SaveGameData` /
  `SerializeToBytes` paths (previously only `SaveGame`). Backup location
  moved to `Documents/DatacenterBackups` (Windows `%USERPROFILE%`,
  Linux `~/Documents`, fallback `~`; legacy `~/GregFrameworkBackups`
  left untouched). `BackupEnabled=false` is now an explicit opt-out that
  also disables mod writes (logged once per session). Install logs the
  resolved `BackupRoot`.

- Hardware IDs (`HardwareIdPersistencePatch`) as standalone single-scheme
  system: exactly one stable schema (`gregID:...`); any other ID is converted
  exactly once — live at Start/Awake, save data on load incl. cable endpoints.
  No coexistence/adoption logic for foreign schemas.
- `CleanId` strips numeric GetInstanceID suffixes only; lettered user names
  (`Core_Switch_A`) survive.
- Healing with per-entry null-guards (lists, entries, cables, endpoints):
  one bad entry never aborts the whole healing.
- Log dedup by ID string instead of `GetHashCode`; `[gregCore][HwId]` logs;
  `NewPatchPanelIdpatch` typo fixed.
- Compatibility guard (`IncompatibleModGuard`): detects the old separate
  404-PersistentID mod (name/assembly) and unpatches it (`UnpatchSelf`) — at
  mod init and again on scene load — keeping gregID the only ID system.
  Log + toast warning, once per session.
- Clearly own implementation: `Greg*` patch class names
  (`GregSwitchIdAssignPatch`, `GregNetworkIdHealing`, …), `HWID SYSTEM ACTIVE`
  startup line in log, docs in `docs/modding/hardware-ids.md`.

### Added

- String save persistence (`GregModSaveStrings`): UTF-16 text chunked into
  `saveIntArray` (`Encode`/`Decode` purely managed + tested,
  `UpsertText`/`TryReadText`/`CombineTitleBody`/`SplitTitleBody` as
  best-effort glue like `GregModSave`). First user: gregMod.NotesHUD
  (note text travels with the savegame).
- Crash-safe text input (`GregKeyPump` + `GregSafeTextField` +
  `GregPanelBuilder.AddSafeInputField` + `SafeInputFields`): label rendering
  + manual keyboard pump instead of `TextField` (IL2CPP `TextEditor` is
  stripped → hard crash). `AddInputField` is marked `[Obsolete]`
  (pointer to `AddSafeInputField`). The owning mod calls
  `field.PumpFrame()` per frame when focused and manages focus itself.
  In-game verification still pending (build + review only).
- EntityInventory (`GregEntityInventory`): on load, everything
  present in the save is inventoried (servers, switches, routers, firewalls,
  patch panels, cables, SFP modules, LACP groups). Each entry gets a
  stable UID invisible to the player — mods address things directly
  (`TryFindLive`, `TryGetUid`, `GetAll`, `Rebuilt` event). Server/switch/
  patch panel use their `gregID`, cables/LACP deterministic UIDs from
  vanilla IDs, router/firewall/SFP persisted via sidecar
  (`greg_inventory.<save>.tsv`, index-shift repair via hint).
- Display separation: devices look vanilla (`gameObject.name` stays
  vanilla, screens show no `gregID` tokens — scrub postfixes on
  `Server.UpdateServerScreenUI` / `NetworkSwitch.UpdateScreenUI`), persistence
  hides invisibly in ID fields + inventory.
- Control: MelonPreferences `gregCore.EntityInventory`
  (`Enabled`/`VerboseLogging`/`DumpOnRebuild`) plus `Dump()` and `Verify()`.
  Docs: `docs/modding/api/entity-inventory.md`.

## [1.2.3] — 2026-09-22

### Added

- Mod dependencies (`GregModDeps`): Declare/EnsureLoaded/CheckAll with minimum versions (MelonLoader registry) plus manifest diff (GetLocalManifest/DiffManifests/FormatDiff) as ModSync prerequisite for coop.
- Save batch A (`gregCore.Core.Networking`): `GregNetworkDeviceSaves` (router/firewall/SFP/LACP/cable/label/VLAN filter, upsert where keyed), `GregCustomerSaves` + `GregEconomySaves` (CustomerBase/Record/Monthly/Balance, dict bridges), `GregJobSaves` (Technician/RepairJob/InteractObject, upsert per uid); bool arrays in `GregModPack`.
- Networking batch (`gregCore.Core.Networking`): `GregPatchPanels` (save DTO + runtime bridge for PatchPanel/PatchPanelSaveData), `GregSetIP` (keypad discovery, subnet math with fallback, Show/Cancel), `GregServiceRequests` (save snapshot, AddRequest/LoadFromSave/RebuildUI, row helpers), `GregServers` (ServerSaveData DTO + runtime actions: SetIP, Power, Customer/App, Repair).
- Custom items (`GregCustomItems` + `GregObjImport`): shop/static items from DTO + pack folder loaded via vanilla ModLoader (mesh pre-check, traversal protection, GetPrefab).
- ModPackConfig extension (`GregModPack`): builder (Create/EnsureLists/AddShopItem/AddStaticItem/AddDll) and reader (Snapshot/ReadShopItem/ReadStaticItem) for all pack types, incl. managed array bridges.
- Save persistence (`GregModSave`): ModItemSaveData DTO plus Create/Fill/Read/ReadAll/Upsert/Remove against the game's own list (SaveData.modItemData), keyed by modFolderName.
- Hotkey HUD (`GregHudRegistry`/`GregHud`): mods register hotkeys, displayed as key bar on the right edge (HUD layer).
- Central mod hub (`GregModHub`, F1 key): lists registered mods/menus with open/close buttons (opener via `GregMenuRegistry.RegisterOpener`/`TryOpen`/`Snapshot`, click routing with EventSystem fallback).
- Unified open-source layout (README, docs, badges) following the gregCore model.
- Codacy analysis configuration (`.codacy/`, CLI v2 like gregModmanager): `lizard`, `opengrep`, `pylint`, `trivy` for C#/Python/Shell; local without duplicate CI runs, `GH_TOKEN`/`CODACY_API_TOKEN` only as Forgejo secrets.

### Changed

- Source tree restructured into `gregCore.*` assemblies; `lib/MoonSharp` and `ci-stubs/` removed (MoonSharp via NuGet).
- Finalized public docs (README, QUICKSTART, ARCHITECTURE, SOURCE_LAYOUT, INDEX) for open-source release 1.2.3.
- Aligned `IGregAPI.Version`, hook counts in docs/comments and workshop descriptions to 1.2.3 / 1850+.
