# Guidebook Rust 01 Setup

Native Rust mods on gregCore: compiled cdylib DLLs loaded through `RustFFIBridge`. Read this page fully before writing Rust — the ABI rules are load-bearing.

## Layout

One DLL per mod, built from its own crate:

```
<crate>/Cargo.toml          (crate-type = ["cdylib"])
<crate>/src/greg.rs         (safe bindings — copy from templates/rust/)
<crate>/src/lib.rs          (your mod)
<game root>/UserLibs/Rust/<modId>.dll
```

Legacy `Plugins/Rust/` is **ignored** with a warning — use `UserLibs/Rust/`. Disable without deleting via `UserLibs/Rust/.deactivated/` (loaders never read it; restart after moving).

## The template

Copy `templates/rust/` per mod. `src/greg.rs` is the whole SDK surface:

- Mirrors the `GregCoreAPI` table in `src/gregCore.Bridge/RustFFI/RustFFIBridge.cs` **field for field** (ABI v1). Compile-time layout asserts fail the build on any drift — never reorder fields.
- Every slot is `Option`: unbound slots on table v1 (`unsubscribe_event`, all `config_*_int/float/string`) are safe no-ops/defaults.
- Safe wrappers: `log_info/warning/error`, `notify`, economy/world/tech/time/game getters + setters, `get_player_position`, `subscribe_event/fire_event`, `on_hook/fire_hook`, `config_set/get_bool`, `decode_hook`, event IDs in `greg::events` (mirrors `gregCore.Core.Events.EventIds`).
- Report `greg::TABLE_VERSION` in `greg_mod_info` and re-check every field before ever bumping it.

## Build

```bash
cargo build --release --target x86_64-pc-windows-msvc
```

Current stable Rust via rustup (the bindings use modern FFI affordances). The DLL lands in `target/x86_64-pc-windows-msvc/release/<crate>.dll` — rename to `<modId>.dll`, copy to `UserLibs/Rust/`, restart the game once (first discovery).

## ABI rules (all mandatory)

1. **Exports**: required `greg_mod_info` + `greg_mod_init`; optional `greg_mod_update(dt)`, `greg_mod_scene_loaded(name)`, `greg_mod_event(id, data)`, `greg_mod_shutdown`. Missing core exports → the bridge logs `does not export core functions` and skips the DLL.
2. **Strings** are NUL-terminated ANSI, copied synchronously by the bridge. Never store the pointers.
3. **`get_current_scene` allocates per call** on the C# side — cache it, never poll per frame.
4. **Never unwind across the boundary**: `#[no_mangle] extern "C"` fns must not panic. Keep `greg_mod_update` cheap.
5. **No saves, no UI, no networking** from Rust — observe, compute, notify; persist via fired events to Lua/C#.

## Your first Rust mod

`templates/rust/src/lib.rs` subscribes to coins + save events, hooks `greg.PLAYER.CoinChanged`, and notifies on load. Build → rename → `UserLibs/Rust/` → restart. Verify in the loader log: `Rust Plugin loaded: rust_example`, toast on start, coin lines on money change.

## Checkpoint

- [ ] DLL loads (`Rust Plugin loaded`), no `does not export core functions` warning.
- [ ] You can state the five ABI rules and the three things Rust never does (saves, UI, networking).
- [ ] You know where the table lives on both sides (`RustFFIBridge.cs` ↔ `greg.rs`).

Next: [[Guidebook Rust 02 Project]] — full event/hook coverage sample mod.
