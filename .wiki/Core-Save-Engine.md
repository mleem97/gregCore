# Core Save Engine

How gregCore persists mod data without ever endangering vanilla saves. Three layers: **SaveGuard** (backups + sidecars), **mod save APIs** (per-mod state), **EntityInventory + gregID** (stable device identity).

## Design principles

1. **Vanilla saves stay vanilla.** The game only lists `*.save`. All mod data goes to sidecars the game ignores.
2. **Back up before first touch.** Every save gets a permanent `pre-greg` backup plus rolling timestamped backups (max 3 per save).
3. **One stable identity.** Every device gets exactly one invisible `gregID`; anything else is converted exactly once. Mods address devices through the inventory, never by screen label.

## Layer 1 — SaveGuard (`GregSaveGuard`)

Patches installed via Harmony on `SaveSystem.SaveGame / SerializeToBytes / SaveGameData` (prefix) and `SaveSystem.LoadGame / LoadFromBytes / LoadGameData` + `WaypointInitializationSystem.LoadNetworkState` (postfix):

- **On save prefix:** `BackupVanillaSave(dir, name)` → `WriteSidecars` (atomic `.tmp` + `.bak` swap) → `SanitizeSaveData(data, phase)` (e.g. clamps `networkData.sfpModules[].prefabID` to a valid vanilla index or a registered map, so a modded save still loads vanilla).
- **On load postfix:** `LoadSidecarsForCurrentSave()` (idempotent per `_loadedKey`) → inventory rebuild → cable-endpoint healing.
- **Guarantee:** no framework write without a fresh backup first. `SaveGamePrefix` writes sidecars only when the backup succeeded; `SaveGameData`/`SerializeToBytes` prefixes back up before the (lossy-for-mods) sanitize mutation. Backup failure → mod writes skipped, vanilla save proceeds. `BackupEnabled=false` is an explicit opt-out that also disables mod writes.
- **Paths:** sidecars at `<saveDir>/greg_<modId>.<sanitizedSave>.tsv`; backups at `Documents/DatacenterBackups/` (`pre-greg/` permanent + `yyyy-MM-dd_HH-mm-ss/` pruned, max 3).
- **Registration for C# mods:** `RegisterSidecar(modId, saveFunc, loadFunc)` and `RegisterVanillaModuleMap(modId, toVanillaFunc, fallbackId)`.

## Layer 2 — mod save APIs

| API | Scope | Details |
|---|---|---|
| `greg.config.*` (Lua, `config.json`) | User-facing settings | `get / get_or / set / delete / has / keys` |
| `greg.save.*` (Lua, `save.json`) | Runtime state | Same methods + `save_now()` (force flush; `set`/`delete` already write through) |
| `greg.io.*` (Lua, `<modId>/data/`) | Files | `read_file / write_file / append_file / delete_file / file_exists / list_files / read_json / write_json` (+ `read_text`/`write_text` aliases, `data_dir`); traversal rejected |
| `GregConfigService` (C#) | Typed JSON configs | `LoadConfig<T> / SaveConfig<T>`, auto-creates directories |
| `GregPersistenceService` (C#) | Key/value JSON | Files in `%AppData%/gregCore/Saves/<key>.json`, filename-traversal guard |
| `GregModSave` (C#) | Vanilla-list items | `ItemSave{ModFolderName, Position, Rotation, SaveValue, SaveIntArray, …}` with `Create / Fill / Read / ReadAll / Upsert / Remove` on `SaveData.modItemData`, keyed by `modFolderName` |
| `GregModPack` (C#) | Pack snapshots | `ShopItem / StaticItem / DllRef / Snapshot` DTOs with builder (`Create / EnsureLists / AddShopItem / AddStaticItem / AddDll`) and reader, incl. managed↔Il2Cpp array bridges |
| `GregSaveEngine` (C#, LiteDB) | Grid/wall state | `LiteDatabase` at `<saveDir>/gregSave_<guid>.greg.db`, collections `greg_meta` + `grid_state`; writes gated by `GregFeatureGuard.SaveEngine.Write`; `IsGregSave` checks the `.greg.db` header |

LiteDB (5.0.21) is used **only** here (`GregSaveEngine`, `WallSaveIntegration`) — not as a general store.

## Layer 3 — gregID + EntityInventory

- **Format:** exactly one schema — `gregID:<Type>:<12HEX>` (e.g. `gregID:Switch:…`). Assigned live at `Start`/`Awake` (`GregSwitchIdAssignPatch`, `GregPatchPanelIdAssignPatch`, `GregServerIdAssignPatch`); `CleanId` strips only numeric `GetInstanceID` suffixes, so lettered user names (`Core_Switch_A`) survive.
- **Healing on load** (`GregNetworkIdHealing`, hooked on `WaypointInitializationSystem.LoadNetworkState`): rewrites `switchID / patchPanelID / serverID` plus cable endpoints, per-entry null-guarded (one bad entry never aborts healing), then `RequestRouteEvaluation()`. Raw↔display mapping persists via `SaveSystem.displayToRawMap`.
- **Display separation:** `gameObject.name` stays vanilla; `Server.UpdateServerScreenUI` / `NetworkSwitch.UpdateScreenUI` scrub any `gregID` token from screens. IDs live in hidden fields + inventory.
- **EntityInventory** (`GregEntityInventory`): on load, everything in the save is inventoried — servers, switches, routers, firewalls, patch panels, cables, SFP modules, LACP groups. Each entry gets a stable, player-invisible UID; mods drive things directly (`TryFindLive`, `TryGetUid`, `GetAll`, `Rebuilt` event). Servers/switches/patch panels reuse their `gregID`; cables/LACP use deterministic UIDs from vanilla IDs; routers/firewalls/SFP persist via sidecar (`greg_inventory.<save>.tsv`, index-shift repair via hint).
- **Control:** MelonPreferences `gregCore.EntityInventory` (`Enabled`, `VerboseLogging`, `DumpOnRebuild`) + `Dump()` / `Verify()`.
- **Guard:** `IncompatibleModGuard` unpatches the old standalone 404-PersistentID mod (name/assembly match, `UnpatchSelf` at init + scene load) so gregID is the only ID system. Log + one toast per session.

## What mod authors must (not) do

- **Do:** keep user settings in `greg.config`, runtime state in `greg.save`, bulk files in `greg.io` sandbox; address devices via inventory UIDs; register sidecars for C# world edits.
- **Do not:** write into vanilla save structures directly, reuse shop/item IDs or GUIDs (see [[Developer Shop Items]]), hand-edit sidecars, or cache Il2Cpp objects long-term (GC moves them — see [[Developer Harmony IL2CPP]]).
- Deep dives: [[Developer Data Storage]], [[Developer Hardware IDs Inventory]].
