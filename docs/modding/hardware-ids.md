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

## Deterministic derivation (no random GUIDs)

`GenerateStableGregId(prefix, legacyId)` derives the gregID as
`SHA-256(legacyId)` truncated to 12 uppercase hex chars. The same legacy
ID yields the same gregID on **live objects and in save healing**, so both
sides correlate by construction — regardless of how vanilla binds save
entries to live objects (by ID or by position+overwrite). IDs are stable
across sessions and machines (48-bit space; uniqueness follows from
vanilla's per-session device-ID uniqueness). Empty input falls back to a
random ID (nothing can reference it yet). Previously healed saves (random
GUIDs, still `gregID:`-prefixed) keep working: they are skipped by the
prefix check on both sides.

Background: random per-load GUIDs required vanilla to overwrite live IDs
from save data during `LoadNetworkState`. On game builds where that
assumption breaks, endpoints dangle and route evaluation finds no routes
(customers disconnected) — see `docs/troubleshooting/doctor.md`
(UNSUPPORTED_GAME_BUILD). Deterministic IDs remove the assumption.

## Safe-mode gate (route-safe passthrough)

All ID rewrites (assign patches + save healing + the extra route
evaluation) run only on **supported game builds**
(`GregGameCompat.HwIdRewritesAllowed`, latched from the GregDoctor
fingerprint verdict at boot; default unknown = disabled). On unknown or
unsupported builds the system passes vanilla data through untouched and
logs one loud warning (`[gregCore][HwId] DISABLED ...`). Vanilla behaviour
is then exactly as without gregCore — degraded IDs, but working routes.
The suffix-cleanup and screen-scrub patches stay active: they provably
no-op without `gregID:` tokens present.

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
