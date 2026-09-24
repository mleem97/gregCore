# Core Events

Events are the heart of gregCore — the equivalent of Oxide's hooks + timers + libraries, unified in one bus design. Two buses exist; one static dispatcher fronts them.

## The two buses

| Bus | Type | Methods | Purpose |
|---|---|---|---|
| `GregEventBus` | mod-to-mod + lifecycle | `Subscribe / Unsubscribe / Publish` (`IGregEventBus`) | Your own events, cross-mod messages, save lifecycle |
| `GregHookBus` | game-method hooks | `On / Once / Off / Dispatch` | The 1,850 harmonized game methods from `game_hooks.json` |

Hook names look like `greg.{Domain}.{Event}` (e.g. `greg.PLAYER.CoinChanged`, `greg.RACK.*`, `greg.SYSTEM.ButtonCheckOut`). Native game-lifecycle hooks live in `gregNativeEventHooks`: `SystemGameLoaded / SystemGameSaved`, `GameLoaded / GameSaved`, `Money / Xp / ReputationChanged`, `Day / MonthEnded`, plus `GetByEventId` numeric lookup (IDs 1001–4001, see `examples/`).

Static front door for fire-and-forget: `GregEventDispatcher.Emit(hookName, data)`. For subscribing from C#, use the `GregMod.On()` helper (tracked `IDisposable`) or `[GregHook]` — the static `On(hookName, handler, modId)` returns void and is not the mod-author path.

Flow: `game_hooks.json` (IL2CPP dump: `Group / Namespace / ClassName / MethodName / ReturnType / Parameters`) → `GregDynamicHookPatcher` installs Prefix/Postfix via `HookIntegration` → each hit dispatches through `GregEventDispatcher` → Lua `greg.on` callbacks / C# subscribers run on the main thread.

## Lua: `greg.on / once / fire`

```lua
function on_init()
    greg.on("greg.PLAYER.CoinChanged", function(payload)
        greg.ui.log_info("Money changed by: " .. tostring(payload.data["Amount"]))
    end)
end
```

- `greg.on(hookName, callback)` → subscription token; `greg.off(token)` unsubscribes.
- `greg.once(hookName, callback)` → fires once, then auto-removes.
- `greg.fire(hookName, dataTable)` → emits your own event; payloads arrive as `{ hook_name, timestamp, cancelable, cancelled, data }`.
- `UnregisterAll(modId)` runs automatically at shutdown/reload — no leaks across hot-reloads.

## Lua: generated per-group bindings

`LuaHookBindingGenerator` builds these from `game_hooks.json` at boot:

```lua
greg.hooks.groups()        -- list all 22 groups
greg.hooks.audio.list()    -- hooks in group "Audio"
greg.hooks.audio.on_set_effects_volume(function(payload) ... end)
-- pattern: greg.hooks.<group>.on_<snake_case_method>(callback)
```

Canonical hook registry (`framework/greg_hooks.json`, `manifestVersion 2`) currently pins two fully-specified hooks — `greg.lifecycle.scene-loaded` (`MelonMod.OnSceneWasLoaded`) and `greg.lifecycle.update` (`MelonMod.OnUpdate`), both `strategy: dispatcher`, `threading: main-thread`, `status: implemented`, `supportedLanguages: [CSharp, Lua]`. Everything else dispatches dynamically from `game_hooks.json`. Coverage semantics (which denominator a "covered" claim uses) are defined in `docs/modding/api/coverage.md` and checked by `scripts/validate_contracts.py`.

## C#: attributes + dispatcher

- `[GregHook("greg.Group.Event")]` on a method subscribes it; `[GregMod]` marks the mod class; `[GregDependsOn]` declares dependencies.
- `GregMod.On()` / `DisposeSubscriptions()` for manual subscribe/publish (`GregEventDispatcher.Emit` for fire-and-forget); `HookName` constants + `EventIds` for the numeric range.
- Harmony patches derive from `SafePatch`; one task per patch class, `try/catch` inside, explicit `PatchAll` — never per-frame reflection (see [[Developer Harmony IL2CPP]]).

## Lua lifecycle + `require`

Optional globals in `main.lua` (define only what you need):

| Function | When |
|---|---|
| `on_init()` | Mod loaded |
| `on_update(dt)` | Every frame (`dt` = delta seconds — never log here per-frame) |
| `on_scene_loaded(name)` | Scene changed |
| `on_shutdown()` | Before reload / game exit |
| `on_reload()` | After a successful hot-reload |

`require("name")` loads `<modDir>/<name>.lua`; `require("@shared/name")` loads from the shared folder. Results are cached per path; avoid circular requires.

## Payload and threading rules

- Numbers cross as Lua numbers, strings as strings, lists as **1-indexed** Lua tables. Every boundary call is guarded: outside the game or on error you get safe defaults (`0`, `false`, `""`, empty table, `nil`) — never an exception.
- Callbacks run on the **main thread** (`GregMainThreadDispatcher`). Timers follow the same rule — see [[Developer Timers Coroutines]].
- Out of scope by design (use C# instead): spawning racks/devices, physical cable ops, keyboard capture, anything outside `<modId>/data/`.

## Where to go next

- Full hook catalog by group: [[Hooks Reference]]
- Timers/coroutines: [[Developer Timers Coroutines]]
- Persistence events + save lifecycle: [[Core Save Engine]]
- Hands-on subscribing: [[Developer Events Hooks Guide]]
