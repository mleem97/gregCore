# Developer Hardware IDs Inventory

Stable device identity across save/load: the single-schema `gregID` system plus the `EntityInventory`.

## gregID: exactly one schema

Format: `gregID:<Type>:<12HEX>` (implementation: `HardwareIdPersistencePatch`, `Greg*IdAssignPatch` classes, `HWID SYSTEM ACTIVE` boot line, `[gregCore][HwId]` logs).

- **Assign live** at `Start`/`Awake`: `GregSwitchIdAssignPatch`, `GregPatchPanelIdAssignPatch`, `GregServerIdAssignPatch`. Any foreign ID converts exactly once — no coexistence logic.
- **`CleanId`** strips only numeric `GetInstanceID` suffixes; lettered user names (`Core_Switch_A`) survive.
- **Heal on load** (`GregNetworkIdHealing` on `WaypointInitializationSystem.LoadNetworkState`): rewrites `switchID / patchPanelID / serverID` **and cable endpoints**, per-entry null-guarded (one bad entry never aborts healing), dedups logs by ID string, then `RequestRouteEvaluation()`. Raw↔display mapping persists via `SaveSystem.displayToRawMap`.
- **Display separation**: `gameObject.name` stays vanilla; `Server.UpdateServerScreenUI` / `NetworkSwitch.UpdateScreenUI` scrub tokens. Players never see a `gregID`.
- **Guard**: `IncompatibleModGuard` scans `RegisteredMelons` at init + scene load and `UnpatchSelf`s the old standalone 404-PersistentID mod (log + one toast per session).

Legacy `GregHardwareID : MonoBehaviour` / `HardwareIDManager` (`UniqueID`, `PrefabID`) still exists in `gregCore.Core` but is superseded — target `gregID`.

## EntityInventory (`GregEntityInventory`)

On load, everything present in the save is inventoried: servers, switches, routers, firewalls, patch panels, cables, SFP modules, LACP groups. Each entry gets a stable, player-invisible UID:

- Servers / switches / patch panels reuse their `gregID`.
- Cables / LACP use deterministic UIDs derived from vanilla IDs.
- Routers / firewalls / SFP persist via sidecar (`greg_inventory.<save>.tsv`, index-shift repair via hint).

Drive things directly: `TryFindLive`, `TryGetUid`, `GetAll`, `Rebuilt` event. Control via MelonPreferences `gregCore.EntityInventory` (`Enabled=true`, `VerboseLogging=false`, `DumpOnRebuild=false`) + `Dump()` / `Verify()`.

## Rack positions (`RackPatch`)

Vanilla `Rack.IsPositionAvailable` is hollow (`false`) — the prefix keeps `_usedPositions<rackHash, HashSet<int>>` with `MarkPositionUsed / Free / ClearRack / GetUsedCount`, emitting `greg.RACK.*`. Lua:

```lua
greg.rack.get_all() / greg.rack.count()
greg.rack.is_position_available(rackId, position)
greg.rack.get_used_count(rackId)
greg.rack.mark_used(rackId, position) / greg.rack.mark_free(rackId, position)
```

## Rules for mod authors

1. Address devices by inventory UID / `gregID` — never by screen label or scene name.
2. Expect healing: your load path must tolerate rewritten IDs and re-request routing where needed.
3. Null-guard every entry you iterate — mirror the healer's discipline.
4. Never invent a second ID scheme; never ship the old PersistentID mod alongside gregCore.
5. Keep `gameObject.name` vanilla — put identity in ID fields + inventory, not display names.
