# GregCore Conventions

- Framework/public types use the `Greg` or `greg` prefix and are grouped by layer.
- C# mods use `[GregMod]` and derive from `GregMod`.
- Dependencies use `[GregDependsOn]`; plugin IDs and setting IDs are case-insensitive at registry boundaries.
- Public mod subscriptions should use `GregMod.On(...)` so disposal removes them automatically.
- Unity/Il2Cpp work must be dispatched through `IGregMainThreadDispatcher` when initiated from another thread.
- Hook contracts use canonical `gregMod.*`, `gregExt.*` or `gregPlugin.*` names; legacy names are migration aliases.
- Callback and patch failures are caught and logged at framework boundaries.

[TODO] A repository-wide formatter/analyzer policy was not verified in the inspected files.

## Evidence

- `src/PublicApi/Attributes/`
- `src/PublicApi/GregMod.cs`
- `src/PublicApi/IGregMainThreadDispatcher.cs`
- `framework/greg_hooks.json`
- `src/Core/Events/`

