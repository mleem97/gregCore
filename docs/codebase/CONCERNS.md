# GregCore Concerns and Implementation Findings

- The active worktree contains pre-existing uncommitted changes; they were preserved and are not attributable to this analysis alone. Inspect `git status` before committing.
- The canonical hook manifest currently has `assemblyFingerprint: UNKNOWN`; runtime therefore correctly enters safe mode until a reviewed game fingerprint is supplied.
- The compatibility README advertises broad support, but the release smoke test is explicitly external and not passed by repository tests.
- `GregDependencyResolver` was a placeholder in the base commit and is now implemented in the existing worktree changes; cycle and missing-dependency tests pass.
- The GregCore bootstrap was present but not called by the Melon entry point; it is now wired into `GregCoreMod.OnInitializeMelon`.
- Notification APIs had disconnected/empty paths; the public and legacy notification facades now use the bounded `GregNotificationManager`.
- Settings updates are marked dirty and persisted after a short debounce instead of writing synchronously on every update.
- Operation queues are bounded by `PerformanceProfile.MaxQueuedOperations`; event dispatch and UI notifications also have hard limits.
- `GregPerformanceGovernor.OnUpdate` is now called from the real Melon update path, and quality profiles control throttle intervals.
- `GregPerformanceModule.OnResourceUpdate` now removes the exact delegate registered by the caller.
- Remaining TODOs include external in-game verification and validation of native
  co-op callbacks against the installed game build. gregCore deliberately does
  not define or synchronize multiplayer state itself.

## Evidence

- `git status --short --branch`
- `src/Core/GregCoreMod.cs`
- `src/GameLayer/Bootstrap/GregBootstrapper.cs`
- `src/Infrastructure/Plugins/GregDependencyResolver.cs`
- `src/PublicApi/Modules/GregUIModule.cs`
- `src/API/GregAPI.cs`
- `src/Infrastructure/Settings/GregModSettingsService.cs`
- `src/Infrastructure/Performance/GregOperationQueue.cs`
- `src/Infrastructure/Performance/GregPerformanceGovernor.cs`
- `src/PublicApi/Modules/GregPerformanceModule.cs`
