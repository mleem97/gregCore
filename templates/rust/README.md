# gregCore Rust mod template

Minimal native mod: safe bindings (`src/greg.rs`) + example lifecycle (`src/lib.rs`).

## Build

```bash
cargo build --release --target x86_64-pc-windows-msvc
```

The DLL lands in `target/x86_64-pc-windows-msvc/release/<crate>.dll`.
Rename it to `<modId>.dll` and copy it to the game's `UserLibs/Rust/`
(`Plugins/Rust/` is ignored). Restart the game — no manifest needed.

## Files

| File | Purpose |
|---|---|
| `src/greg.rs` | Safe wrapper over the C ABI table. **Do not reorder fields** — layout must match `GregCoreAPI` in `src/gregCore.Bridge/RustFFI/RustFFIBridge.cs` exactly. Report `greg::TABLE_VERSION` in `greg_mod_info`. |
| `src/lib.rs` | Your mod: info/init/update/scene/shutdown/event exports. Copy this folder per mod, adjust ids + logic. |

## API rules

- Every table slot is `Option`: unbound slots (table v1: `unsubscribe_event`, all `config_*_int/float/string`) are safe no-ops/defaults — but check before relying on them.
- Strings are NUL-terminated ANSI, copied synchronously by the bridge.
- `get_current_scene()` allocates on the C# side per call — cache it.
- Keep `greg_mod_update` cheap. Never block, never panic across the boundary (`#[no_mangle] extern "C"` fns must not unwind).
- Event IDs live in `greg::events` (mirrors `gregCore.Core.Events.EventIds`).
- Enable/disable without deleting: move the DLL into/out of `UserLibs/Rust/.deactivated/` and restart (loaders never read `.deactivated`).

## Deploy layout

```
<game root>/UserLibs/Rust/<modId>.dll
```
