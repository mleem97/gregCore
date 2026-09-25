# Guidebook CSharp 04 Patches

Patch game methods with Harmony without hurting performance or the IL2CPP runtime. Rule source: `docs/modding/harmony-il2cpp.md`. Example neighbors: `src/gregCore.Patches/` (`RackPatch`, `ShopPatch`, `PlayerPatch`, `TimePatch`, …).

## Style: one task per class

```csharp
public sealed class RepairLogPatch : SafePatch
{
    // Explicit PatchAll from your OnLoad/OnReady; try/catch inside;
    // Prefix returns true (run original) or false (skip it) — deliberately.
}
```

- Derive from `SafePatch`; explicit `PatchAll`, never blanket-apply.
- Small defensive Prefix/Postfix pairs; emit events (`greg.RACK.*`, `greg.SYSTEM.ButtonCheckOut`) instead of duplicating logic.
- Dynamic bulk patching goes through `GregDynamicHookPatcher` + `HookIntegration` (driven by `game_hooks.json`); your hand-written patches cover what the dispatcher cannot.

## Performance rules

- **No per-frame reflection**: cache `Type.GetType` / `MethodInfo` once.
- **No `FindObjectsOfType` in `OnUpdate`**: cache holders; throttle scans in tiers (0.1 s / 1 s / 2 s / 30 s).
- **Event-state over polling** (`PortSpeedMemory` pattern); route bursts via `GregOperationQueue`; budgets via `GregPerformanceGovernor` / `GregResourceMonitor` / `GregPerformancePatches` (incl. `WorldCanvasCuller`, technician/footstep/indicator throttles).

## IL2CPP pitfalls

- `Il2CppReferenceArray`: copy/extend properly (shop rows, dropdowns via `Il2CppSystem.Collections.Generic.List<string>`).
- `Nullable<Color>` needs explicit handling; `renderer.materials` clones — mutate the clone.
- Never cache Il2Cpp objects long-term (GC moves them); re-resolve via inventory UIDs ([[Guidebook CSharp 05 Saves Shop]]).
- Inactive `DontDestroyOnLoad` holders for must-keep state; `DelegateSupport.ConvertDelegate` for managed→Il2Cpp callbacks.

## After a game update

1. Run the game once with the loader; re-copy `Il2CppAssemblies` + `net6` into `references/`.
2. `tools/GameApiGenerator/regenerate.sh` → review `src/gregCore.GameApi/Generated/`.
3. `scripts/Generate-GregHooksFromIl2CppDump.ps1` → review `game_hooks.json` → curate `framework/greg_hooks.json`.
4. `scripts/validate_contracts.py` + `scripts/check-coverage.sh` + `dotnet test`; vanilla-first test (no mods → loads → enable yours).

## Exercises

1. Write a postfix that logs one shop checkout (`greg.SYSTEM.ButtonCheckOut` exists — prefer subscribing; patch only what events cannot reach).
2. Convert a per-frame scan to a 1 s throttled cached scan; measure the log silence.

## Checkpoint

- [ ] One explicit patch class, applied + logged (`PatchApplied`), removable without residue.

Next: [[Guidebook CSharp 05 Saves Shop]] — sidecars, inventory, shop items.
