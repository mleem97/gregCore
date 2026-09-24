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

### `greg.switch`
- `get_all() -> table`: array of `{id, hash, is_on, is_broken, x, y, z}`.
- `get_list() -> table`: array of switch IDs.
- `count() -> number` / `broken_count() -> number`
- `find_by_id(id) -> table or nil`
- `repair(id) -> bool` / `repair_all() -> number`

### `greg.tech` (technicians)
- `free_count() -> number` / `total_count() -> number`
- `dispatch_server() -> 1/0` / `dispatch_switch() -> 1/0`: sends a technician to one broken device.

### `greg.rack`
- `get_all() -> table` / `count() -> number`
- `is_position_available(rackId, position) -> bool`
- `get_used_count(rackId) -> number`
- `mark_used(rackId, position)` / `mark_free(rackId, position)`

### `greg.cable`
- `get_all() -> table` / `count() -> number` / `get_next_id() -> number`

### `greg.world`
- `time_of_day() -> number` / `day() -> number` / `seconds_in_day() -> number`
- `set_seconds_in_day(val)` / `time_scale() -> number` / `set_time_scale(val)`
- `pause()` / `resume()` / `is_paused() -> bool`
- `scene() -> string` / `difficulty() -> number` / `save() -> bool` (triggers a game save)
- `server_count()` / `rack_count()` / `switch_count() -> number`

## 4. UI & Logging (`greg.ui.*`)
- `notify(message, duration?)`
- `log(message, type?)` / `log_info(message)` / `log_warning(message)` / `log_error(message)` (DevConsole + log file)
- `register_mod_config_tab(tab_id, label, builder_fn)`

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
- Placing/spawning racks, devices or cables from Lua (use C# mods for world editing).
- Keyboard input capture (input belongs to the game/mods, not scripts).
- Anything outside `<modId>/data/` (sandbox enforced, traversal rejected).
