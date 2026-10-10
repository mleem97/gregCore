# Guidebook Rust 02 Project

A complete, shippable Rust mod — **full event + hook coverage** — mirroring `examples/Rust/greg_example/`. Drop-in, no companion needed.

## The project: `examples/Rust/greg_example/`

`Cargo.toml` (cdylib `greg_rust_example`) + `src/greg.rs` (bindings copy) + `src/lib.rs`:

```rust
extern "C" fn on_coins_changed(_event_id: u32, data: u64) {
    greg::log_info(&format!("[rust_example] coins changed: {data}"));
}

extern "C" fn on_any_hook(hook: *const c_char, trigger: *const c_char, json: *const c_char) {
    let (h, t, j) = greg::decode_hook(hook, trigger, json);
    greg::log_info(&format!("[rust_example] hook {h} via {t}: {j}"));
}

// in greg_mod_init, after greg::init(api):
greg::subscribe_event(greg::events::PLAYER_COIN_UPDATED, on_coins_changed);
greg::subscribe_event(greg::events::GAME_SAVED, on_game_saved);
greg::subscribe_event(greg::events::SERVER_STATUS_CHANGED, on_server_status);
greg::subscribe_event(greg::events::RACK_POSITION_QUERIED, on_rack_position);
greg::subscribe_event(greg::events::RACK_POSITION_USED, on_rack_used);
greg::subscribe_event(greg::events::CABLE_CREATED, on_cable_created);
greg::on_hook("greg.PLAYER.CoinChanged", on_any_hook);
```

The full example additionally covers XP, rack-freed, scene load, and shutdown logging — read it as the reference for every subscription shape (typed events take `(u32, u64)`; hooks take `(hook, trigger, json)`).

Verify: build → `UserLibs/Rust/rust_example.dll` → restart → `Rust Plugin loaded: rust_example` → change money (coin lines), save the game (save line), place hardware (rack/cable lines). No companion mod required.

## Reading game state (not just events)

```rust
let money = greg::get_player_money();
let (servers, broken) = (greg::get_server_count(), greg::get_broken_server_count());
let (x, y, z, _ry) = greg::get_player_position();
if greg::get_free_technician_count() > 0 {
    greg::dispatch_repair_server();
}
greg::notify("Rust says hi.");
```

Writes (`set_player_money`, `set_time_scale`, `trigger_save`, …) work the same way — still local-only: Rust never touches networking or saves directly.

## Config flags

```rust
let verbose = greg::config_get_bool("rust_example", "verbose", false);
greg::config_set_bool("rust_example", "verbose", true);
```

Only bool exists on table v1; int/float/string helpers exist in `greg.rs` but are inert until the bridge binds them — check before relying on them.

## Troubleshooting

| Symptom | Cause | Fix |
|---|---|---|
| `does not export core functions` | Missing `greg_mod_info`/`greg_mod_init` (name mangling) | `#[no_mangle] pub extern "C"`, `crate-type = ["cdylib"]` |
| Crash on first subscribe/hook | Hand-rolled table with wrong field order | Use `greg.rs` unmodified — layout asserts guard it |
| `api_version` mismatch confusion | Example reports table version, not framework version | Report `greg::TABLE_VERSION`; pin framework `VERSION` in your README |
| No events arriving | Wrong event ID | Use `greg::events::*` (mirrors `EventIds`), never magic numbers |
| Game stutters | Work in `greg_mod_update` | Move to event callbacks; update stays near-empty |

Checkpoint: DLL loads, all eight subscriptions log, state reads work, update is empty, no errors.
