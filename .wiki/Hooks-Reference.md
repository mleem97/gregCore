# Hooks Reference

The complete hook/event catalog — the gregCore equivalent of Oxide's hook index. Source of truth: `game_hooks.json` (1,850 methods, 22 groups) + `framework/greg_hooks.json` (canonical registry, manifest v2) + generated `docs/FrameworkAPI.md`. Usage patterns: [[Developer Events Hooks Guide]]; concepts: [[Core Events]].

## How to read a hook

Each `game_hooks.json` row: `{ Group, Namespace, ClassName, MethodName, ReturnType, IsVoid, Parameters[] }`. Full ID form: `greg.{Group}.{ClassName}.{MethodName}`. Lua per-group sugar: `greg.hooks.<lowercase_group>.on_<snake_case_method>(cb)` + `greg.hooks.<group>.list()` + `greg.hooks.groups()`.

## The 22 groups (counts)

| Group | Hooks | Typical content |
|---|---|---|
| VisualUI | 605 | Screens, panels, HUD widgets, update loops |
| Networking | 180 | Cables, routes, switches, patch panels, maps |
| World | 163 | Time, scene, environment, placement |
| Economy | 137 | Money, XP, reputation, shop, checkout |
| Persistence | 121 | Save/load, serialization, waypoints |
| Lifecycle | 120 | Boot, scenes, mods, game state transitions |
| Character | 112 | Player controller, movement, interaction |
| Hardware | 95 | Servers, racks, devices, IDs, screens |
| Input | 74 | Keys, actions, keypads, locks |
| Uncategorized | 73 | Awaiting curation — prefer grouped equivalents |
| Settings | 34 | Options, config, difficulty |
| Interaction | 24 | Clicks, pickups, usable objects |
| Audio | 20 | Mixers, volumes, effects |
| UnityEngine | 20 | Engine callbacks |
| Serialization | 17 | DTOs, bytes, snapshots |
| Facility | 12 | Walls, rooms, furniture |
| Maintenance | 9 | Repair, technicians, jobs |
| CustomImport | 8 | ModLoader item/prefab injection |
| Tutorials | 8 | Onboarding flows |
| Steam | 7 | Lobbies, achievements (observe-only) |
| DevTools | 6 | Consoles, REPL, diagnostics |
| Ignored | 5 | Explicitly excluded from dispatch |

## Curated hook names (stable, prefer these)

| ID | Emitted from | Payload highlights |
|---|---|---|
| `greg.PLAYER.CoinChanged` | `Player.UpdateCoin` | `data["Amount"]` |
| `greg.PLAYER.XpChanged` / `ReputationChanged` | player patches | new totals |
| `greg.RACK.*` | `RackPatch` (position used/freed) | rack hash, position |
| `greg.SYSTEM.ButtonCheckOut` | `ShopPatch.OnCheckOut` | cart snapshot |
| `greg.lifecycle.scene-loaded` | `MelonMod.OnSceneWasLoaded` | scene name |
| `greg.lifecycle.update` | `MelonMod.OnUpdate` | `dt` |
| `SystemGameLoaded / SystemGameSaved`, `GameLoaded / GameSaved` | `gregNativeEventHooks` | save identity |
| `Money / Xp / ReputationChanged`, `Day / MonthEnded` | native event hooks | totals / time |

Numeric IDs 1001–4001 (`EventIds`, `GetByEventId`) exist for compat — prefer string IDs in new code.

## Lua domain API (the other half of "hooks")

Events tell you *when*; these tell you *what now* (all guarded — safe defaults outside the game):

- `greg.player` — `position() → {x,y,z}`, `money() / set_money(v) / add_money(n)`, `xp() / set_xp(v)`, `reputation() / set_reputation(v)`, `teleport(x,y,z)`, `is_crouching() / is_sitting()`
- `greg.server` — `get_all() → [{id, hash, is_on, is_broken, size_u?, x, y, z}]`, `get_list()`, `count()`, `broken_count()`, `find_by_id(id) / find_by_ip(ip)`, `repair(id)`, `repair_all() → number`, `power_on/off(id)`, `set_ip(id, ip)`, `set_customer(id, customerId)`
- `greg.switch` — `get_all()`, `get_list()`, `count()`, `broken_count()`, `find_by_id(id)`, `repair(id)`, `repair_all()`
- `greg.tech` — `free_count()`, `total_count()`, `dispatch_server() → 1/0`, `dispatch_switch() → 1/0`
- `greg.rack` — `get_all()`, `count()`, `is_position_available(rackId, position)`, `get_used_count(rackId)`, `mark_used / mark_free(rackId, position)`
- `greg.cable` — `get_all()`, `count()`, `get_next_id()`
- `greg.patch` — `get_all() → [{id, hash, x, y, z}]`, `get_list()`, `count()`, `find_by_id(id)`, `has_cable(id)`
- `greg.customer` — `bases() → [{base_id, customer_id, money_speed, all_met, wants_internet, satisfied}]`, `is_ip_present(baseId, ip)`, `app_id_for_ip(baseId, ip) → number (-1 unknown)`, `register_subnet / unregister_subnet`
- `greg.economy` (read-only) — `sheet() → {total_salary, months}`, `history() → [{month, day, salary, repair, shop}]`
- `greg.shop` — `items() → [{idx (1-based), name, price, xp, type, id, unlocked}]`, `unlock(idx)`, `buy(idx)`, `cart() → [{ref, name, price, qty, total}]`, `cart_add/ref`, `cart_remove(ref)`
- `greg.net` (read-only saves) — `routers() → [{asn, next_route_id, routes, owned}]`, `firewalls() → [{cluster_ip, rules}]`, `sfps() → [{prefab, x, y, z, inserted}]`, `lacps()`, `cables() → [{id, maxspeed}]`
- `greg.mods` — `list() → [{id, name, version}]`, `is_loaded(idOrName)`, `version(idOrName)`, `declare({{mod, min_version, required}})`, `ensure({mod, min_version?, required?}) → ok, detail`, `check() → problems (empty = ok)`
- `greg.requests` (read-only) — `list() → [{number, state, short, long, rewarded, done, progress}]`, `current_number()`
- `greg.subnet` — `mask_from_cidr(cidr)` (pure math), `usable_ips(subnet)` (game needed, capped 65,536), `first_usable(subnet)`
- `greg.items` — `register_shop_item(subfolder, spec)`, `register_static_item(subfolder, spec)`; spec keys `name, price, xp, size_u, mass, scale, model, texture, icon, type` (snake_case or PascalCase)
- `greg.world` — `time_of_day()`, `day()`, `seconds_in_day() / set_seconds_in_day(v)`, `time_scale() / set_time_scale(v)`, `pause() / resume() / is_paused()`, `scene()`, `difficulty()`, `save()`, `server_count() / rack_count() / switch_count()`, `open_all_walls()`
- `greg.ui` — `notify(msg, seconds?)`, `log / log_info / log_warning / log_error(msg)`, `register_mod_config_tab(tab_id, label, fn)`
- `greg.computer` — `register_shortcut(id, label, fn_or_appid)`, `unregister_shortcut(id)`, `register_app(appId, title, on_open_fn[, on_close_fn])`, `unregister_app(appId)`, `open_app(appId)`, `close_app()`, `current_app()`, `list_shortcuts()`, `list_apps()` (see [[Guidebook Computer UI]])
- Tablets/widgets — `tablet_open(title) / widget_open(title, x, y)` (x, y required) `→ id ("" = failed)`, `panel_add_label / panel_add_section / panel_add_spacer / panel_add_button(id, label, fn) / panel_add_toggle / panel_add_slider`, `panel_toggle(id) → visibility`, `panel_visible(id)`, `panel_close(id)`

Regenerating this catalog: `scripts/Generate-GregHooksFromIl2CppDump.ps1` → `game_hooks.json`; `scripts/generate_api_docs.py` → `docs/FrameworkAPI.md`; `scripts/validate_contracts.py` checks coverage claims (two denominators: raw dump vs curated registry — see `docs/modding/api/coverage.md`).
