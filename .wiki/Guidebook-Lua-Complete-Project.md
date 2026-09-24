# Guidebook Lua Complete Project

The whole ShiftHelper in one annotated file. It combines chapters 01–05: lifecycle, events, timers, config/save, tablet UI, and domain APIs.

## `mod.json`

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

## `main.lua` (complete)

```lua
-- ShiftHelper: greets with balance, logs money changes, auto-repairs on a
-- timer, keeps settings, shows a tablet. One file, fully annotated.

local PANEL_ID = nil

local function auto_repair_enabled()
    return greg.config.get_or("auto_repair", "true") == "true"
end

local function interval_seconds()
    return tonumber(greg.config.get_or("interval", "60")) or 60
end

local function status_line()
    return string.format("Balance $%.2f | Broken servers %d | Free techs %d | Day %d",
        greg.player.money(), greg.server.broken_count(),
        greg.tech.free_count(), greg.world.day())
end

local function do_repair_cycle()
    if not auto_repair_enabled() then return end
    local broken = greg.server.broken_count()
    if broken > 0 then
        local fixed = greg.server.repair_all()
        greg.ui.notify("Repaired " .. tostring(fixed) .. " servers.", 4)
        greg.save.set("last_repair", tostring(greg.world.day()))
    end
end

local function build_panel()
    PANEL_ID = tablet_open("ShiftHelper")
    panel_add_label(PANEL_ID, status_line())
    panel_add_section(PANEL_ID, "Actions")
    panel_add_button(PANEL_ID, "Repair now", do_repair_cycle)
    panel_add_button(PANEL_ID, "Refresh status", function()
        greg.ui.notify(status_line(), 4)
    end)
    panel_add_toggle(PANEL_ID, "Auto-repair", auto_repair_enabled(), function(v)
        greg.config.set("auto_repair", tostring(v))
    end)
    panel_add_slider(PANEL_ID, "Interval (s)", 10, 300, interval_seconds(), function(v)
        greg.config.set("interval", tostring(math.floor(v)))
    end)
end

function on_init()
    greg.ui.log_info("ShiftHelper loaded. " .. status_line())

    greg.on("greg.PLAYER.CoinChanged", function(payload)
        local amount = tonumber(payload.data["Amount"]) or 0
        greg.save.set("last_seen_money", tostring(greg.player.money()))
        if math.abs(amount) >= 100 then
            greg.ui.notify("Money changed by " .. tostring(amount), 3)
        end
    end)

    build_panel()

    -- Note: greg.every has no cancel; the interval is read once here.
    -- Changing the slider applies after reload (by design, not a bug).
    greg.every(interval_seconds(), do_repair_cycle)

    greg.ui.register_mod_config_tab("shift_helper", "ShiftHelper", function()
        -- Settings Hub entry; toggles live on the tablet.
    end)
end

function on_update(dt)
    -- Intentionally empty: timers do the polling.
end

function on_scene_loaded(name)
    greg.ui.log_info("Scene loaded: " .. name)
end

function on_shutdown()
    if PANEL_ID ~= nil then panel_close(PANEL_ID) end
    greg.ui.log_info("ShiftHelper shutting down...")
end

function on_reload()
    greg.ui.log_info("ShiftHelper reloaded. " .. status_line())
end
```

## Why it is shaped this way

- **Config vs save**: user choices (`auto_repair`, `interval`) live in `config.json`; observations (`last_seen_money`, `last_repair`) in `save.json`. They survive different things and are edited by different actors.
- **No per-frame work**: `on_update` is empty; the timer polls. The 2 ms/frame profiler budget (`LuaProfiler`) is never touched.
- **Panel rebuilt, not cached**: labels read live state at build; refresh via explicit button + timer-driven notify.
- **Cleanup**: `on_shutdown` closes the panel; the bus auto-unregisters subscriptions.
- **Local-only**: no networking, no world spawning — inside the Lua contract and the co-op boundary.

## Ship checklist

- [ ] Runs from a cold start and after hot-reload; `mods` lists it; no errors in the log.
- [ ] Tablet opens/toggles/closes; settings persist; timer repairs only when enabled.
- [ ] Save → load round-trip; uninstall leaves the vanilla save loadable.
- [ ] README (install, settings, events, co-op scope) + version bump plan → [[Guidebook Release]].
