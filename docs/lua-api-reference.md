# gregCore Lua API Reference (v1.1.0)

This document covers the high-level `greg.*` API provided by the modernized gregCore framework.

## 1. Core Lifecycle
Mods should define these functions globally in `main.lua`:

| Function | Description |
| :--- | :--- |
| `on_init()` | Called when the mod is loaded. |
| `on_update(dt)` | Called every frame. `dt` is frame delta time. |
| `on_scene_loaded(name)` | Called when a new scene is loaded. |
| `on_shutdown()` | Called before mod reload or game exit. |
| `on_reload()` | Called after a successful hot-reload. |

---

## 2. Global Services

### `greg.log_*`
- `greg.log_info(msg)`: Standard log.
- `greg.log_warning(msg)`: Yellow log in console.
- `greg.log_error(msg)`: Red log, shows in error overlay.

### `greg.wait / every / coroutine`
- `greg.wait(seconds)`: Yields the current coroutine for `n` seconds.
- `greg.every(seconds, callback)`: Executes `callback` every `n` seconds.
- `greg.start_coroutine(func)`: Spawns a new non-blocking execution thread.

---

## 3. Domain Modules

### `greg.player`
- `get_money() -> number`: Returns current balance.
- `set_money(val)`: Sets player money.
- `get_xp() -> number`: Returns total XP.
- `get_reputation() -> number`: Returns reputation level.

### `greg.server`
- `get_count() -> int`: Total number of servers in the world.
- `get_broken_count() -> int`: Number of servers with `isBroken = true`.
- `dispatch_repair_all() -> bool`: Sends technicians to fix all broken servers.
- `get_list() -> table`: Returns list of all server IDs.

### `greg.rack`
- `get_count() -> int`: Total number of racks.
- `is_pos_free(x, y, z) -> bool`: Checks if a position is available for placement.

---

## 4. Hook System (`greg.hooks.*`)
The framework automatically binds all game-side Harmony patches.

**Syntax:** `greg.hooks.[system]_[event](callback)`

**Examples:**
- `greg.hooks.rack_placed(function(rackId) ... end)`
- `greg.hooks.player_xp_gain(function(payload) ... end)`

---

## 5. Persistence (`greg.io.*`)
All I/O is sandboxed to `UserData/gregCore/Mods/Lua/[ModID]/data/`.

- `read_text(filename) -> string`
- `write_text(filename, content)`
- `read_json(filename) -> table`
- `write_json(filename, data_table)`
- `delete_file(filename)`

---

## 6. Events (`greg.events.*`)
Used for mod-to-mod communication.

- `on(eventName, callback)`: Listen to an event.
- `fire(eventName, data)`: Dispatch an event.
- `once(eventName, callback)`: Listen once.
