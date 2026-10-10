# Guidebook Lua 02 Events

React to the game and talk to other mods. Concepts: [[Core Events]]. Full catalog: [[Hooks Reference]].

## Subscribe, once, unsubscribe, emit

`GregEventLuaModule` gives you four calls (unsubscriptions auto-clean at shutdown/reload, but explicit is better):

```lua
local token = greg.on("greg.Audio.AudioManager.SetEffectsVolume", function(payload)
    greg.ui.log_info("Volume changed!")
end)

greg.once("greg.PLAYER.CoinChanged", function(payload)
    greg.ui.log_info("First coin change seen.")
end)

greg.off(token)

-- Your own mod-to-mod event (prefix with your mod id):
greg.fire("shift_helper.job_done", { result = "ok", count = 3 })
greg.on("shift_helper.job_done", function(payload)
    greg.ui.log_info("Jobs done: " .. tostring(payload.data["count"]))
end)
```

## Discover hooks in-game (F12)

```lua
greg.hooks.groups()          -- all 22 groups
greg.hooks.audio.list()      -- hook IDs inside Audio
greg.hooks.networking.list()
```

Per-group sugar is generated at boot by `LuaHookBindingGenerator` from `game_hooks.json`:

```lua
greg.hooks.<group>.on_<snake_case_method>(callback)
-- e.g. greg.hooks.audio.on_set_effects_volume(fn)
-- full ID form: "greg.Audio.AudioManager.SetEffectsVolume"
```

The canonical registry (`framework/greg_hooks.json`, manifest v2) pins two fully-specified hooks — `greg.lifecycle.scene-loaded` and `greg.lifecycle.update` (dispatcher strategy, main thread). Everything else dispatches dynamically from the 1,850-row dump.

## Curated names to memorize

| ID | Meaning |
|---|---|
| `greg.PLAYER.CoinChanged` | Money changed; `payload.data["Amount"]` |
| `greg.RACK.*` | Rack position used/freed |
| `greg.SYSTEM.ButtonCheckOut` | Shop checkout |
| `greg.lifecycle.scene-loaded` / `greg.lifecycle.update` | Scene / frame |
| `SystemGameLoaded / SystemGameSaved`, `GameLoaded / GameSaved` | Save lifecycle (native hooks) |
| `Money / Xp / ReputationChanged`, `Day / MonthEnded` | Economy + time (native hooks) |

Prefer string IDs over the numeric 1001–4001 range (`EventIds`, `GetByEventId`) in new code.

## Exercises

1. Log `greg.PLAYER.CoinChanged` with amount, then `greg.fire` your own `shift_helper.ping` and handle it.
2. List two hook groups relevant to your mod idea and subscribe to one hook each from the REPL before putting them in `main.lua`.
3. If nothing fires: confirm `game_hooks.json` sits next to `gregCore.dll`, check `[DynamicPatcher]` log lines, then [[FAQ Troubleshooting]].

## Checkpoint

- [ ] You can subscribe/emit/unsubscribe and read `payload.data`.
- [ ] You can discover any hook from the REPL without leaving the game.

Next: [[Guidebook Lua 03 Timers Storage]] — scheduling plus settings, state, and files.
