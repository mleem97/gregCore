---
title: "Guidebook Lua 05 Systems"
---
# Guidebook Lua 05 Systems

The domain APIs that read and drive the game. Exact signatures: [Hooks Reference](./hooks-reference/). Identity model: [Developer Hardware IDs Inventory](./developer-hardware-ids-inventory/). Shop deep-dive: [Developer Shop Items](./developer-shop-items/).

## Player, world, technicians

```lua
local pos = greg.player.position()       -- {x, y, z}
greg.player.teleport(10, 0, 20)
greg.player.add_money(500)
greg.ui.log_info("XP: " .. tostring(greg.player.xp()))

greg.world.time_of_day()                 -- 0-24
greg.world.day()
greg.world.set_time_scale(2)
greg.world.pause() / greg.world.resume()

greg.tech.free_count() / greg.tech.total_count()
greg.tech.dispatch_server()              -- -> 1/0, sends a tech to one broken server
```

## Servers, switches, racks, cables, patch panels

```lua
greg.server.count() / greg.server.broken_count()
for _, s in ipairs(greg.server.get_all()) do  -- {id, hash, is_on, is_broken, size_u?, x, y, z}
    if s.is_broken then greg.server.repair(s.id) end
end
greg.server.repair_all()                 -- -> number fixed
greg.server.set_ip(someId, "10.0.0.5")

greg.switch.count() / greg.switch.broken_count()
greg.switch.repair_all()

greg.rack.count()
greg.rack.is_position_available(rackId, pos)  -- -> bool
greg.rack.get_used_count(rackId)

greg.cable.count() / greg.cable.get_next_id()
greg.patch.count()
greg.patch.has_cable(patchId)            -- -> bool
```

Address devices by inventory UID / `gregID` (stable, invisible) — never by screen label. No Lua API spawns racks/devices or performs physical cable ops; that is C# territory by design.

## Customers, economy, shop, network, requests, subnets, items

```lua
greg.customer.bases()                    -- [{base_id, customer_id, money_speed, all_met, ...}]
greg.economy.sheet()                     -- {total_salary, months} (read-only)
greg.economy.history()                   -- [{month, day, salary, repair, shop}]

local items = greg.shop.items()          -- [{idx (1-based), name, price, xp, type, id, unlocked}]
greg.shop.unlock(1) / greg.shop.buy(1)
greg.shop.cart()                         -- [{ref, name, price, qty, total}]
greg.shop.cart_add(ref) / greg.shop.cart_remove(ref)

greg.net.routers() / greg.net.firewalls() / greg.net.sfps()  -- read-only save data
greg.requests.list()                     -- [{number, state, short, long, rewarded, done, progress}]
greg.subnet.mask_from_cidr("10.0.0.0/24") -- pure math, no game needed
greg.subnet.first_usable("10.0.0.0/24")

greg.items.register_shop_item("my_cooler", {
    name = "Ice Cooler", price = 499, xp = 10, size_u = 2,
    mass = 5, scale = 1, model = "cooler.obj",
    texture = "cooler.png", icon = "cooler_icon.png", type = "ServerCooler",
})
-- spec keys accept snake_case or PascalCase; meshes ship as files in the mod folder
```

## Exercises

1. Write a `status()` function returning one line: money, broken servers, free techs, day/time — show it via `notify` and a widget label.
2. Repair loop: timer + `broken_count` + `repair_all`, gated by your `auto_repair` config from chapter 03.
3. Inspect `greg.shop.items()` in the REPL; buy or unlock one entry and watch `CoinChanged` fire.

## Checkpoint

- [ ] You can read every domain area and drive server/switch/tech/shop actions.
- [ ] You know the Lua boundaries (no spawning, no cable ops, no key capture).

Next: [Guidebook Lua Complete Project](./guidebook-lua-complete-project/) — the full ShiftHelper, annotated.
