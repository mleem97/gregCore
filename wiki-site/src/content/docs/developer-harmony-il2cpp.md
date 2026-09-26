---
title: "Developer Harmony IL2CPP"
---
# Developer Harmony IL2CPP

Patch style, performance rules, and IL2CPP pitfalls. (Oxide equivalent: "Publicizer" + "Using Decompiler" + "Best Practices", adapted to Unity 6000 IL2CPP.)

## Patch style

- One task per patch class, deriving from `SafePatch`; explicit `PatchAll`, never blanket-apply.
- `try/catch` inside every patch; `true/false` prefix semantics (return `false` skips the original — use deliberately).
- Small defensive Prefix/Postfix pairs; emit events (`greg.RACK.*`, `greg.SYSTEM.ButtonCheckOut`, …) instead of duplicating logic.
- Representative classes (`src/gregCore.Patches/`): `HardwareIdPersistencePatch`, `IncompatibleModGuard`, `RackPatch`, `SaveSystemPatch`, `SaveManagerPatch`, `NetworkMapPatch`, `CablePositionsPatch`, `TimePatch`, `LoadingScreenPatch`, `PlayerPatch`, `ShopPatch`, `ServerPatch`, `KeybindPatches`, `SettingsUiBridgePatch`, `InputControllerPatch`, `RackPlacementPatch`.

## Performance rules (measured, not stylistic)

- **No per-frame reflection.** Cache `Type.GetType` / `MethodInfo` once.
- **No `FindObjectsOfType` in `OnUpdate`.** Cache holders; throttle scans (tiers: 0.1 s / 1 s / 2 s / 30 s).
- **Event-state, not polling** where possible (`PortSpeedMemory` pattern); visual animators (`RgbAnimator`) track slots instead of rescanning.
- Route bursts through `GregOperationQueue`; background budgets via `GregPerformanceGovernor` / `GregResourceMonitor` / `GregPerformancePatches` (incl. `WorldCanvasCuller`, technician/footstep/indicator throttles).

## IL2CPP pitfalls

- `Il2CppReferenceArray`: always copy/extend properly (shop rows, dropdown items via `Il2CppSystem.Collections.Generic.List<string>`).
- `Nullable<Color>` needs explicit handling; `renderer.materials` clones — mutate the clone.
- Never cache Il2Cpp objects long-term (GC moves them); re-resolve via inventory UIDs.
- Inactive `DontDestroyOnLoad` holders for must-keep state; `DelegateSupport.ConvertDelegate` for managed→Il2Cpp callbacks.
- `OnGUI` is stripped and there is no `EventSystem` — UIToolkit + `GregClickRouter` only ([Developer UI Panels HUD](./developer-ui-panels-hud/)).

## After a game update

1. Run the game once with MelonLoader; re-copy `Il2CppAssemblies` + `net6` into `references/`.
2. `tools/GameApiGenerator/regenerate.sh` → re-check `src/gregCore.GameApi/Generated/`.
3. `scripts/Generate-GregHooksFromIl2CppDump.ps1` → review `game_hooks.json` → curate `framework/greg_hooks.json`.
4. `scripts/validate_contracts.py` + `scripts/check-coverage.sh` + `dotnet test`.
5. Vanilla-first test: no mods → game loads → enable yours.
