# Guidebook Lua 01 First Mod

Create, load, and verify your first Lua mod. No build step — save the files, reload, done.

## 1. Scaffold the folder

Each mod is one folder; the folder name is the fallback mod ID:

```
UserData/gregCore/Mods/Lua/shift_helper/
├── mod.json
└── main.lua
```

Copy `templates/lua/example-mod/` or `examples/Lua/starter_template/` as your starting point.

## 2. Write `mod.json`

The loader (`LuaFFIBridge.ReadManifest`) binds these fields (`ModManifest` record: `Id`, `Name`, `Version`, `PersistentId`, `Author`, `Entrypoint`, `ApiVersion`, `Loader`, `Dependencies`). Use this exact casing — the manifest reader is case-sensitive, and `Id` is required. `Entrypoint` defaults to `main.lua`; unknown extra keys are ignored, so document compatibility in your README instead of inventing fields.

```json
{
  "Id": "shift_helper",
  "Name": "ShiftHelper",
  "Version": "1.0.0",
  "Author": "Your Name",
  "Entrypoint": "main.lua",
  "ApiVersion": "1.0.0",
  "Loader": "Lua",
  "Dependencies": []
}
```

Without `mod.json`, the folder still loads (ID = folder name, entry = `main.lua`) — the manifest adds registry metadata and dependency info (`Dependencies` is a list of mod-ID strings, e.g. `"Dependencies": ["some_helper_lib"]`).

## 3. Write `main.lua`

Lifecycle functions are all optional globals (`on_init`, `on_update(dt)`, `on_scene_loaded(name)`, `on_shutdown`, `on_reload`). Define only what you need:

```lua
function on_init()
    greg.ui.log_info("ShiftHelper loaded.")

    local money = greg.player.money()
    greg.ui.log_info("Current balance: $" .. string.format("%.2f", money))

    greg.on("greg.PLAYER.CoinChanged", function(payload)
        greg.ui.log_info("Money changed by: " .. tostring(payload.data["Amount"]))
    end)
end

function on_update(dt)
    -- Keep frame work tiny. Never log here per-frame.
end

function on_scene_loaded(name)
    greg.ui.log_info("Scene loaded: " .. name)
end

function on_shutdown()
    greg.ui.log_info("ShiftHelper shutting down...")
end

function on_reload()
    greg.ui.log_info("ShiftHelper reloaded.")
end
```

Notes that bite beginners: `on_update` takes `dt` (delta seconds); payloads arrive as `{ hook_name, timestamp, cancelable, cancelled, data }` — always read arguments from `payload.data`; numbers cross as Lua numbers, lists as **1-indexed** tables; every boundary call is guarded (outside the game you get `0`/`false`/`""`/empty table/`nil`, never an exception).

## 4. Load and verify

Restart the game (or hot-reload — `on_reload()` runs after a successful reload). Check:

1. DevConsole shows the greeting + balance.
2. Changing money in-game logs the delta (event works).
3. Backquote console → `mods` lists `shift_helper`; `version` prints the framework version.

Try the F12 REPL next: `print(greg.player.money())`, `greg.player.add_money(1000)`, `greg.ui.notify("Hi!")`, `greg.hooks.groups()`.

## Checkpoint

- [ ] Folder + manifest + `main.lua` in place; mod listed in `mods`.
- [ ] Greeting, balance, coin-change log, scene log all observed.
- [ ] You know the five lifecycle functions and the payload shape.

Next: [[Guidebook Lua 02 Events]] — subscribe, emit, discover all 1,850 hooks.
