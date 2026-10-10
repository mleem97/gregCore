# Developer Timers Coroutines

Scheduling work without freezing the game. (Oxide equivalent: "Timers" + "Coroutines".)

## Lua

Registered once in `LuaCoroutineScheduler` (`src/gregCore.SDK/Lua/LuaCoroutineScheduler.cs`), pumped every `OnUpdate(dt)`:

```lua
greg.wait(5, function()
    greg.ui.log_info("Once, after 5 seconds")
end)

greg.every(60, function()
    greg.ui.log_info("Every 60 seconds")
end)

greg.start_coroutine(function()
    greg.ui.log_info("Step 1")
    coroutine.yield(greg.WAIT)  -- return-wait values cooperate with the scheduler
    greg.ui.log_info("Step 2")
end)
```

Rules:

- `greg.wait(seconds, callback)` — one-shot. `greg.every(seconds, callback)` — repeating. `greg.start_coroutine(fn)` — coroutine with `coroutine.yield()` / return-wait values.
- **Never** `while true do … end` without yielding — you block the main thread. Never log per-frame in `on_update`.
- Timer callbacks run on the main thread, so game-API calls are safe inside them.

## C#

- Per-frame: override `OnUpdate(dt)` in `GregMod`; keep it allocation-free (no reflection, no `FindObjectsOfType` — cache and throttle: 1 s / 2 s / 0.1 s / 30 s tiers per `docs/modding/harmony-il2cpp.md`).
- Deferred/queued: `GregOperationQueue` for bursts; `GregMainThreadDispatcher` to marshal back to the main thread.
- Governed: `GregPerformanceGovernor` + `GregResourceMonitor` + `GregPerformancePatches` (culling `WorldCanvasCuller`, technician/footstep/indicator throttles) keep background work bounded.

## Anti-patterns

| Do | Do not |
|---|---|
| `greg.every(30, poll)` for polling | `on_update` with a log line |
| Cache `Type`/`MethodInfo`, throttle scans | Reflect or scan the scene every frame |
| Yield in coroutines | Busy-wait or sleep the main thread |
| `wait` for one-shots | A repeating timer you never cancel |
