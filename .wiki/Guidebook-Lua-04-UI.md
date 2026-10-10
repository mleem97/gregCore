# Guidebook Lua 04 UI

Show things: toasts, log lines, tablets, widgets, and a settings tab. (C# panels: [[Guidebook CSharp 03 UI]].)

## Notify + log

```lua
greg.ui.notify("ShiftHelper ready.", 4)
greg.ui.log("plain line")            -- type optional
greg.ui.log_info("info")
greg.ui.log_warning("check this")
greg.ui.log_error("something broke")
```

Output lands in the DevConsole + log file. Batch notifications; never notify per frame.

## Tablets and widgets (handles)

Flat globals (not `greg.ui.*`) returning string handles (`pnl_<guid12>`; `""` means failed):

```lua
local id = tablet_open("ShiftHelper")
panel_add_label(id, "Balance: $" .. string.format("%.2f", greg.player.money()))
panel_add_section(id, "Actions")
panel_add_button(id, "Repair all servers", function()
    local fixed = greg.server.repair_all()
    greg.ui.notify("Repaired " .. tostring(fixed) .. " servers.")
end)
panel_add_toggle(id, "Auto-repair", true, function(v)
    greg.config.set("auto_repair", tostring(v))
end)
panel_add_slider(id, "Interval", 10, 300, 60, function(v)
    greg.config.set("interval", tostring(v))
end)
panel_add_spacer(id, 8)

local mini = widget_open("ShiftMini", 100, 200)
panel_add_label(mini, "Broken: " .. tostring(greg.server.broken_count()))

panel_toggle(id)     -- -> new visibility (bool)
panel_visible(id)    -- -> bool
panel_close(id)      -- -> bool
```

Refresh content on open (rebuild labels from live state) rather than caching stale numbers.

## Settings tab + dependency check

```lua
greg.ui.register_mod_config_tab("shift_helper", "ShiftHelper", function()
    -- builder_fn: add your toggles/sliders here
end)

-- Dependency gating (GregModDeps surface):
for _, m in ipairs(greg.mods.list()) do
    greg.ui.log_info(m.id .. " " .. m.version)
end
if not greg.mods.is_loaded("some_helper_lib") then
    greg.ui.log_warning("Optional helper lib not loaded; continuing standalone.")
end
greg.mods.declare({{ mod = "other_mod", min_version = "1.0.0", required = false }})
local problems = greg.mods.check()  -- empty table = ok
```

## Exercises

1. Build the tablet above; confirm open/toggle/close from both the handle calls and the F1 Hub.
2. Wire the toggle + slider to `greg.config` and read them back in your timer from chapter 03.
3. Register the config tab and find it from the Mod Hub's Settings button.

## Checkpoint

- [ ] Tablet with label, section, button, toggle, slider works; widget shows live data.
- [ ] Settings persist across reload; missing optional deps degrade with a warning, not a crash.

Next: [[Guidebook Lua 05 Systems]] — the domain APIs (player, servers, shop, world).
