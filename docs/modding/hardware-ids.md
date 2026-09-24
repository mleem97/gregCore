# Hardware IDs — own GregCore system (`gregID:`)

> Own GregCore implementation. Code:
> `src/gregCore.Patches/Hardware/HardwareIdPersistencePatch.cs`,
> compatibility guard: `src/gregCore.Patches/Hardware/IncompatibleModGuard.cs`.

## Problem

Vanilla assigns device IDs with Unity `GetInstanceID` suffixes
(e.g. `Switch_123456`). Suffixes differ every session — after
save/load, cable endpoints point at dead IDs and topologies break.

## Design (single-scheme)

Exactly **one** stable schema: `gregID:<Type>:<12 HEX>`, e.g.
`gregID:Switch:A3F9C41B2E77`. Any ID without the `gregID:` prefix is
converted exactly once:

- **Live objects** at `Start`/`Awake` (`GregSwitchIdAssignPatch`,
  `GregPatchPanelIdAssignPatch`, `GregServerIdAssignPatch`).
  `gameObject.name` stays vanilla (display separation); scrub patches
  (`GregServerScreenScrubPatch`, `GregSwitchScreenScrubPatch`) keep
  screens ID-token-free.
- **Save data** on load (`GregNetworkIdHealing` on
  `WaypointInitializationSystem.LoadNetworkState`): legacy IDs in
  `NetworkSaveData` are rewritten, cable endpoints follow, then
  `RequestRouteEvaluation()`.
- **Suffix cleanup** (`Greg*CleanPatch` on `GenerateUnique*`):
  only numeric suffixes are stripped (`Switch_123` → `Switch`);
  lettered user names (`Core_Switch_A`) survive.
- **ID persistence** via `SaveSystem.displayToRawMap`.

All best-effort (null/pointer checks, per-entry guards in healing,
`HookIntegration.LogPatchError`) — the ID system can never break
saves or startup. Status in log: `[gregCore][HwId]`.

## Incompatible third-party mods

A second ID system must never run alongside (ID churn, broken
cables). `IncompatibleModGuard` detects the old separate
404-PersistentID mod (mod/assembly name) and unpatches it via
`HarmonyInstance.UnpatchSelf()` — at mod init and again on scene
load (once per session, log + toast warning).
After that, `gregID` is guaranteed the only ID system.
