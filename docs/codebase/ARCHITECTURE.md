# GregCore Architecture

`GregBootstrapper` builds the shared service graph: logger, event bus, hook bus, settings, persistence, plugin registry, notification service, public API, and performance governor. `GregApiContext` exposes the guarded subset to `GregMod` instances.

`GregPluginRegistry` scans DLLs with Mono.Cecil, resolves dependencies, creates `GregMod` entrypoints, and isolates lifecycle failures. The main-thread dispatcher is drained from the Melon update callback. Resource registrations are disposed with the mod.

The event bus caches handler arrays and defers events when the performance governor's per-frame budget is exhausted. The hook bus dispatches synchronous named hooks with handler isolation. The dynamic Harmony patcher only accepts the object-shaped reviewed manifest and enters safe mode for unknown or mismatched fingerprints.

Performance is centralized in `GregPerformanceGovernor`, which controls frame settings, memory monitoring, event budgets, operation concurrency and bounded operation queues.

## Evidence

- `src/GameLayer/Bootstrap/GregBootstrapper.cs`
- `src/PublicApi/GregApiContext.cs`
- `src/Infrastructure/Plugins/GregPluginRegistry.cs`
- `src/Core/Events/GregEventBus.cs`
- `src/GameLayer/Hooks/GregDynamicHookPatcher.cs`
- `src/Infrastructure/Performance/GregPerformanceGovernor.cs`

