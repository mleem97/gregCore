---
title: "Guidebook Lua 03 Timers Storage"
---
# Guidebook Lua 03 Timers Storage

Do work later, remember things, and share code between files. Scheduler: `LuaCoroutineScheduler` (pumped in `OnUpdate`). Stores: `LuaConfigModule` (`config.json`), `LuaSaveModule` (`save.json`), `GregIoLuaModule` (`<modId>/data/`).

## Timers and coroutines

```lua
greg.wait(5, function()
    greg.ui.log_info("Once, after 5 seconds.")
end)

greg.every(60, function()
    local broken = greg.server.broken_count()
    if broken > 0 then
        local fixed = greg.server.repair_all()
        greg.ui.notify("Repaired " .. tostring(fixed) .. " servers.")
    end
end)

greg.start_coroutine(function()
    greg.ui.log_info("Step 1")
    coroutine.yield(greg.WAIT)
    greg.ui.log_info("Step 2")
end)
```

Rules: callbacks run on the main thread (game-API calls are safe); never `while true` without yielding; never log per-frame in `on_update` — poll with `greg.every` instead.

## Settings vs state vs files

```lua
-- User-facing settings (config.json):
greg.config.set("auto_repair", "true")
local auto = greg.config.get_or("auto_repair", "true")
greg.config.has("auto_repair")       -- -> bool
greg.config.delete("auto_repair")    -- -> bool
greg.config.keys()                   -- -> table

-- Runtime state (save.json, write-through):
greg.save.set("last_seen_money", tostring(greg.player.money()))
greg.save.save_now()                 -- -> bool, force flush

-- Files, sandboxed to <modId>/data/:
greg.io.write_file("notes.txt", "hello")
greg.io.read_file("notes.txt")       -- alias: read_text
greg.io.append_file("notes.txt", "more")
greg.io.file_exists("notes.txt")     -- -> bool
greg.io.delete_file("notes.txt")
greg.io.list_files(".json")          -- -> table
greg.io.write_json("snapshot.json", { money = greg.player.money() })
local snap = greg.io.read_json("snapshot.json")  -- -> table or nil

-- JSON anywhere:
local t = greg.json.parse('{"a":1}') -- -> table
local s = greg.json.stringify(t)     -- -> string ("" on failure)
```

Sandbox: the VM runs `Preset_SoftSandbox`; `ResolveSafe` rejects `..`, `/`, `\` escapes — anything outside `<modId>/data/` is refused by design (world spawning, cable ops, and keyboard capture are C#-only on purpose).

## Sharing code (`require`)

```lua
-- <modDir>/helpers.lua:
local helpers = {}
function helpers.tag(msg) return "[ShiftHelper] " .. msg end
return helpers

-- main.lua:
local helpers = require("helpers")
greg.ui.log_info(helpers.tag("ready"))
require("@shared/common")  -- shared folder across mods
```

Cached per path; avoid circular requires.

## Exercises

1. Add an `auto_repair` config flag; a 60 s timer repairs only when it is `"true"`.
2. Persist `last_seen_money` in `greg.save` and notify on `CoinChanged` only when the delta exceeds 100.
3. Move one helper to its own file and `require` it.

## Checkpoint

- [ ] `wait` / `every` / coroutine each used once and observed.
- [ ] Config, save, and one sandbox file round-trip verified.

Next: [Guidebook Lua 04 UI](./guidebook-lua-04-ui/) — notifications, tablets, widgets, settings tabs.
