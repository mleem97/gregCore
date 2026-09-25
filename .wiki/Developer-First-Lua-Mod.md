# Developer First Lua Mod

Build your first working Lua mod in ~10 minutes. (Oxide equivalent: "My First Plugin".)

## 1. Folder + manifest

Create one folder per mod (folder name = mod ID):

```
UserData/gregCore/Mods/Lua/
└── my_first_mod/
    ├── mod.json
    └── main.lua
```

`mod.json`:

```json
{
  "id": "my_first_mod",
  "name": "My First Mod",
  "version": "1.0.0",
  "author": "Your Name",
  "description": "A starter template for gregCore Lua modding.",
  "entry": "main.lua",
  "min_framework_version": "1.2.3"
}
```

Without `mod.json`, `main.lua` in the folder is still loaded; the manifest adds registry metadata + version gating (`GregModDeps`).

## 2. `main.lua` — lifecycle + first event

```lua
-- Called when the mod is first loaded
function on_init()
    greg.ui.log_info("Hello from My First Mod!")

    local money = greg.player.money()
    greg.ui.log_info("Current balance: $" .. string.format("%.2f", money))

    -- Subscribe to a game event
    greg.on("greg.PLAYER.CoinChanged", function(payload)
        local amount = payload.data["Amount"]
        greg.ui.log_info("Money changed by: " .. tostring(amount))
    end)
end

-- Called every frame (keep it cheap, never log per-frame)
function on_update(dt)
end

-- Called when the scene changes
function on_scene_loaded(name)
    greg.ui.log_info("Scene loaded: " .. name)
end

-- Called on mod shutdown / before reload
function on_shutdown()
    greg.ui.log_info("My First Mod shutting down...")
end
```

Reload (or restart the game). Verify: DevConsole shows the greeting + balance; changing money in-game logs the delta. The backquote console → `mods` lists `my_first_mod`.

## 3. Try the REPL (F12)

```lua
print(greg.player.money())
greg.player.add_money(1000)
greg.ui.notify("Hello from REPL!")
greg.hooks.groups()       -- discover the 22 hook groups
greg.hooks.audio.list()   -- hooks inside group "Audio"
```

## 4. What to learn next (in order)

1. **More API surface** — `greg.player / greg.server / greg.switch / greg.tech / greg.rack / greg.cable / greg.patch / greg.customer / greg.economy / greg.shop / greg.net / greg.mods / greg.requests / greg.subnet / greg.items / greg.world`, `greg.ui.*`, `greg.config / greg.save / greg.io / greg.json` — full signatures: [[Hooks Reference]].
2. **Timers** — `greg.wait / greg.every / greg.start_coroutine` ([[Developer Timers Coroutines]]).
3. **Events** — `greg.on / greg.once / greg.fire`, payload shape, per-group bindings ([[Developer Events Hooks Guide]]).
4. **Storage** — `config.json` vs `save.json` vs `data/` sandbox ([[Developer Data Storage]]).
5. **UI** — `tablet_open / widget_open / panel_add_*` ([[Developer UI Panels HUD]]).

## 5. Minimal checklist before sharing

- [ ] `mod.json` has a unique `id` and `min_framework_version`.
- [ ] No per-frame logging; no `while true` loops (use `greg.every`).
- [ ] Only writes inside `<modId>/data/` (sandbox enforced — traversal is rejected).
- [ ] `on_shutdown` cleans timers/subscriptions you created manually (bus auto-unregisters, but be explicit for clarity).
- [ ] Tested on a **copy** of a save first (SaveGuard backs up, but good hygiene is free).

Reference implementations: `examples/Lua/starter_template`, `examples/Lua/example_mod`, `examples/Lua/advanced_automation`, `examples/Lua/fleet_doctor`.
