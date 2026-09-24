# gregCore Lua API Reference

This document describes EXACTLY what the `greg.*` table provides in Lua mods.
Function names here match the implementation 1:1 — if a name is not listed,
it does not exist. Numbers cross the boundary as Lua numbers, strings as
Lua strings, lists as 1-indexed Lua tables. Every call is guarded: outside
the game (or on errors) functions return safe defaults (`0`, `false`, `""`,
empty table or `nil`) instead of throwing.

Mod layout: a folder under `UserData/gregCore/Mods/Lua/<modId>/` with
`main.lua` (or `mod.json` with a custom `entrypoint`). Per-mod files live
under `<modId>/data/` (sandboxed — no access outside).

## 1. Core Lifecycle
Define these functions globally in `main.lua` (all optional):

| Function | Description |
| :--- | :--- |
| `on_init()` | Called when the mod is loaded. |
| `on_update(dt)` | Called every frame. `dt` is frame delta time. |
| `on_scene_loaded(name)` | Called when a new scene is loaded. |
| `on_shutdown()` | Called before mod reload or game exit. |
| `on_reload()` | Called after a successful hot-reload. |

## 2. Coroutines & Timers (`greg.*`)
- `greg.wait(seconds, callback)`: Calls `callback` once after `n` seconds.
- `greg.every(seconds, callback)`: Calls `callback` every `n` seconds.
- `greg.start_coroutine(func)`: Runs `func` as a coroutine (use `coroutine.yield()` / return-wait values inside).

## 3. Domain Modules

### `greg.player`
- `position() -> table`: `{x, y, z}` of the player.
- `money() -> number` / `set_money(val)` / `add_money(amount)`
- `xp() -> number` / `set_xp(val)`
- `reputation() -> number` / `set_reputation(val)`
- `teleport(x, y, z)`
- `is_crouching() -> bool` / `is_sitting() -> bool`

### `greg.server`
- `get_all() -> table`: array of `{id, hash, is_on, is_broken, size_u?, x, y, z}`.
- `get_list() -> table`: array of server IDs.
- `count() -> number` / `broken_count() -> number`
- `find_by_id(id) -> table or nil` / `find_by_ip(ip) -> table or nil`
- `repair(id) -> bool` / `repair_all() -> number`
- `power_on(id) -> bool` / `power_off(id) -> bool`
- `set_ip(id, ip) -> bool` / `set_customer(id, customerId) -> bool`
- `set_app(id, appId) -> bool` / `clear_warning(id) -> bool`
- `has_cable(id) -> bool` / `valid_position(id) -> bool`
- `capture(id) -> table or nil` (full snapshot)
- `insert_into_rack(id, spec) -> bool` (spec: `rack_uid`, `prefab`, `is_on`, ...)

### `greg.switch`
- `get_all() -> table`: array of `{id, hash, is_on, is_broken, x, y, z}`.
- `get_list() -> table`: array of switch IDs.
- `count() -> number` / `broken_count() -> number`
- `find_by_id(id) -> table or nil`
- `repair(id) -> bool` / `repair_all() -> number`

### `greg.tech` (technicians)
- `free_count() -> number` / `total_count() -> number`
- `dispatch_server() -> 1/0` / `dispatch_switch() -> 1/0`: sends a technician to one broken device.
- `list() -> table`: array of `{id, name, salary, state, busy}`.
- `send_to_server(technicianId, serverId) -> bool` / `send_to_switch(technicianId, switchId) -> bool`
- `hire(index) -> bool` / `fire(technicianId) -> bool`
- `request_next_job(technicianId) -> bool`

### `greg.rack`
- `get_all() -> table` / `count() -> number`
- `is_position_available(rackId, position) -> bool`
- `get_used_count(rackId) -> number`
- `mark_used(rackId, position)` / `mark_free(rackId, position)`

### `greg.cable`
- `get_all() -> table` / `count() -> number` / `get_next_id() -> number`

### `greg.patch`
- `get_all() -> table`: array of `{id, hash, x, y, z}`.
- `get_list() -> table`: array of patch panel IDs.
- `count() -> number`
- `find_by_id(id) -> table or nil`
- `has_cable(id) -> bool` / `valid_position(id) -> bool`
- `capture(id) -> table or nil` / `insert_into_rack(id, spec) -> bool`

### `greg.customer`
- `bases() -> table`: array of `{base_id, customer_id, money_speed, all_met, wants_internet, satisfied}`.
- `is_ip_present(baseId, ip) -> bool` / `app_id_for_ip(baseId, ip) -> number` (-1 unknown)
- `register_subnet(baseId, vlanId, routeKey, ips) -> bool` / `unregister_subnet(baseId, routeKey) -> bool`

### `greg.economy` (read-only)
- `sheet() -> table or nil`: `{total_salary, months}`.
- `history() -> table`: array of `{month, day, salary, repair, shop}`.

### `greg.shop`
- `items() -> table`: array of `{idx, name, price, xp, type, id, unlocked}` (1-based `idx`, order is best-effort).
- `unlock(idx) -> bool` / `buy(idx) -> bool`
- `cart() -> table`: array of `{ref, name, price, qty, total}`.
- `cart_add(ref) -> bool` / `cart_remove(ref) -> bool`
- `mod_items() -> table`: array of `{ref, name, price, mod_id}`.
- `buy_mod_item(ref) -> bool`
- `picker_is_open() -> bool` / `picker_open() -> bool` / `picker_cancel() -> bool`
- `picker_color() -> {r,g,b,a} or nil` / `picker_set_color(r,g,b,a) -> bool`

### `greg.net` (read-only save data)
- `routers() -> table`: array of `{asn, next_route_id, routes, owned}`.
- `firewalls() -> table`: array of `{cluster_ip, rules}`.
- `sfps() -> table`: array of `{prefab, x, y, z, inserted}`.
- `lacps() -> table`: array of group ids.
- `cables() -> table`: array of `{id, maxspeed}`.

### `greg.mods`
- `list() -> table`: array of `{id, name, version}` (gregCore registry).
- `is_loaded(modIdOrName) -> bool` / `version(modIdOrName) -> string`
- `declare({{mod=, min_version=, required=}, ...}) -> bool`
- `ensure({mod=, min_version=?, required=?}) -> ok, detail`
- `check() -> table`: array of `{owner, mod, detail}` problems (empty = ok).

### `greg.requests` (read-only)
- `list() -> table`: array of `{number, state, short, long, rewarded, done, progress}`.
- `current_number() -> number`

### `greg.subnet`
- `mask_from_cidr(cidr) -> string` (pure math, no game needed)
- `usable_ips(subnet) -> table` (needs a running game; hard-capped at 65536 entries — mind large subnets)
- `first_usable(subnet) -> string` (needs a running game, "" when none)

### `greg.setip` (vanilla keypad UI flow)
- `show_for(serverId) -> bool` / `cancel() -> bool`

### `greg.modsave` (per-save mod data, travels with the savegame)
- `list(folder) -> table`: array of `{folder, position{x,y,z}}`.
- `upsert(folder, spec) -> bool` (spec: `position{x,y,z}`, `rotation{x,y,z}`, `values[]`, `ints[]`, `ints2[]`).
- `remove(folder) -> bool`

### `greg.items` (register custom items; meshes ship as files in the mod folder)
- `register_shop_item(subfolder, spec) -> bool` — spec keys: `name, price, xp, size_u, mass, scale, model, texture, icon, type` (snake_case or PascalCase).
- `register_static_item(subfolder, spec) -> bool`

### `greg.internet`
- `endpoints() -> table`: array of `{server, ip, type, app, max, current}`.
- `command_center_level() -> number`
- `auto_repair_mode() -> number` / `set_auto_repair_mode(mode) -> bool`

### `greg.settings` (game audio)
- `set_master_volume(v) -> bool` / `set_music_volume(v) -> bool`
- `set_effect_volume(v) -> bool` / `set_racks_volume(v) -> bool` (0..1)
- `reload() -> bool`

### `greg.objectives`
- `show(index) -> bool` / `stop() -> bool` / `skip() -> bool`
- `active() -> table` (objective UIDs) / `tutorial_in_progress() -> bool`
- `create({loc=, uid=, x=, y=, z=, xp=?, rep=?, sub=?}) -> bool`
- `start(uid, x, y, z) -> bool` / `clear() -> bool`

### `greg.tooltip`
- `overlay(text, x, y, z, xOffset?) -> bool` / `hide() -> bool`
- `interact(text) -> bool` / `hide_interact() -> bool`

### `greg.coop`
- `ensure() -> bool` / `shutdown() -> bool`
- `peers() -> table`: array of `{id, x, y, z, yaw}`.
- `remove_avatar(peerId) -> bool` / `resend() -> bool` / `peer_timeout() -> number`

### Misc (`greg.steam`, `greg.locale`, `greg.numpad`, `greg.pause`)
- `steam.parse_lobby(connect) -> number` (0 when unparsable)
- `locale.text(uid, fallback) -> string` / `locale.change(uid) -> bool` / `locale.current() -> number`
- `numpad.is_active() -> bool` / `numpad.written()`, `numpad.copied() -> string`
- `numpad.press(digit) -> bool` / `numpad.press_ok() -> bool` / `numpad.press_delete() -> bool`
- `pause.is_paused() -> bool` / `pause.on_open(fn) -> bool` / `pause.on_close(fn) -> bool`

### Tablets & widgets (handles, `greg.tablet_*` / `greg.widget_*` / `greg.panel_*`)
- `tablet_open(title) -> id` / `widget_open(title, x?, y?) -> id` ("" = failed)
- `panel_add_label(id, text)` / `panel_add_section(id, title)` / `panel_add_spacer(id, height?)` → bool
- `panel_add_button(id, label, fn)` / `panel_add_toggle(id, label, value, fn)` / `panel_add_slider(id, label, min, max, value, fn)` → bool
- `panel_toggle(id) -> bool` (new visibility) / `panel_visible(id) -> bool` / `panel_close(id) -> bool`

### `greg.world`
- `time_of_day() -> number` / `day() -> number` / `seconds_in_day() -> number`
- `set_seconds_in_day(val)` / `time_scale() -> number` / `set_time_scale(val)`
- `pause()` / `resume()` / `is_paused() -> bool`
- `scene() -> string` / `difficulty() -> number` / `save() -> bool` (triggers a game save)
- `server_count()` / `rack_count()` / `switch_count() -> number`
- `open_all_walls() -> bool`

## 4. UI & Logging (`greg.ui.*`)
- `notify(message, duration?)`
- `log(message, type?)` / `log_info(message)` / `log_warning(message)` / `log_error(message)` (DevConsole + log file)
- `register_mod_config_tab(tab_id, label, builder_fn)`

### `greg.computer` (in-game computer shortcuts & apps)
- `register_shortcut(id, label, target) -> bool` / `unregister_shortcut(id) -> bool`
- `register_app(appId, title, onOpen, onClose) -> bool` / `unregister_app(appId) -> bool`
- `open_app(appId) -> bool` / `close_app() -> bool` / `current_app() -> string`
- `list_shortcuts() -> table` / `list_apps() -> table`

## 5. Config & Save Data
Per-mod JSON files under `<modId>/data/` (created on demand, write-through).

`greg.config.*` (`config.json` — user-facing settings):
`greg.save.*` (`save.json` — runtime state):
- `get(key) -> string or nil` / `get_or(key, default) -> string`
- `set(key, value)` / `delete(key) -> bool` / `has(key) -> bool` / `keys() -> table`
- `greg.save.save_now() -> bool` (force flush; `set`/`delete` already write through)

## 6. Files (`greg.io.*`, sandboxed to `<modId>/data/`)
- `read_file(path) -> string` (`read_text` is an alias)
- `write_file(path, content)` (`write_text` is an alias)
- `append_file(path, content)` / `delete_file(path)` / `file_exists(path) -> bool`
- `list_files(pattern?) -> table` / `data_dir -> string` (read-only)
- `read_json(path) -> table or nil` / `write_json(path, table) -> bool`

## 7. JSON (`greg.json.*`)
- `parse(text) -> table/string/number/boolean or nil`
- `stringify(value) -> string` ("" on failure)

## 8. Events & Hooks
Mod-to-mod and game events:
- `greg.on(hookName, callback)` / `greg.once(hookName, callback)` / `greg.fire(hookName, dataTable)`
- `greg.hooks.<group>.on_<event>(callback)` — auto-generated per game-hook group, plus `greg.hooks.<group>.list()` to discover available hooks.

## 9. Modules (`require`)
`require("name")` loads `<modDir>/<name>.lua`; `require("@shared/name")` loads from the shared folder. Results are cached per path; avoid circular requires.

## 10. Out of scope (by design)
- Spawning racks/devices into the world from Lua (shop buying and item registration are covered; direct spawning needs C#).
- Physical cable operations from Lua (no stable link identity; use C# for world editing).
- Keyboard input capture (input belongs to the game/mods, not scripts).
- Anything outside `<modId>/data/` (sandbox enforced, traversal rejected).
- Keyboard input capture (input belongs to the game/mods, not scripts).
- Anything outside `<modId>/data/` (sandbox enforced, traversal rejected).
