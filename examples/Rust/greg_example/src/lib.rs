// gregCore Rust SDK — Example Mod.
//
// Full hook + event coverage: player, rack, cable, server, save.
// Build: cargo build --release --target x86_64-pc-windows-msvc
// Deploy: copy the DLL to <game>/UserLibs/Rust/ as <modId>.dll.
//
// Bindings live in `greg.rs` (mirrors `templates/rust/src/greg.rs`:
// exact ABI-table layout, null-safe wrappers, event IDs).

mod greg;

use std::ffi::c_char;

const MOD_ID: &str = "rust_example";

extern "C" fn on_hook_callback(hook: *const c_char, trigger: *const c_char, json: *const c_char) {
    let (h, t, j) = greg::decode_hook(hook, trigger, json);
    greg::log_info(&format!("[{MOD_ID}] hook {h} via {t}: {j}"));
}

macro_rules! on_event {
    ($name:ident, $label:literal) => {
        extern "C" fn $name(_event_id: u32, data: u64) {
            greg::log_info(&format!("[{MOD_ID}] {}: {data}", $label));
        }
    };
}

on_event!(on_coins_changed, "coins changed");
on_event!(on_xp_changed, "xp changed");
on_event!(on_rack_position, "rack position queried");
on_event!(on_rack_used, "rack position used");
on_event!(on_rack_freed, "rack position freed");
on_event!(on_cable_created, "cable created");
on_event!(on_game_saved, "game saved");
on_event!(on_server_status, "server status changed");

#[no_mangle]
pub extern "C" fn greg_mod_info() -> greg::GregModInfo {
    greg::GregModInfo {
        id: c"rust_example".as_ptr(),
        name: c"Rust Example Mod".as_ptr(),
        version: c"1.1.0".as_ptr(),
        author: c"teamGreg".as_ptr(),
        description: c"Full-featured Rust SDK example with player/rack/cable/server hooks."
            .as_ptr(),
        api_version: greg::TABLE_VERSION,
    }
}

#[no_mangle]
pub extern "C" fn greg_mod_init(api: *const greg::GregCoreAPI) -> bool {
    if !greg::init(api) {
        return false;
    }
    greg::log_info(&format!(
        "[{MOD_ID}] init (money={}, servers={})",
        greg::get_player_money(),
        greg::get_server_count()
    ));

    greg::on_hook("greg.PLAYER.CoinChanged", on_hook_callback);

    greg::subscribe_event(greg::events::PLAYER_COIN_UPDATED, on_coins_changed);
    greg::subscribe_event(greg::events::PLAYER_XP_UPDATED, on_xp_changed);
    greg::subscribe_event(greg::events::GAME_SAVED, on_game_saved);
    greg::subscribe_event(greg::events::SERVER_STATUS_CHANGED, on_server_status);
    greg::subscribe_event(greg::events::RACK_POSITION_QUERIED, on_rack_position);
    greg::subscribe_event(greg::events::RACK_POSITION_USED, on_rack_used);
    greg::subscribe_event(greg::events::RACK_POSITION_FREED, on_rack_freed);
    greg::subscribe_event(greg::events::CABLE_CREATED, on_cable_created);

    greg::log_info(&format!("[{MOD_ID}] all event handlers registered."));
    true
}

#[no_mangle]
pub extern "C" fn greg_mod_update(_dt: f32) {
    // Per-frame work. Keep it cheap — read, don't log, every frame.
}

#[no_mangle]
// SAFETY: fixed C ABI — the bridge always passes a valid scene pointer
// (or null, which is checked). Never call from Rust code directly.
#[allow(clippy::not_unsafe_ptr_arg_deref)] // nosemgrep: rust.lang.security.unsafe-usage.unsafe-usage -- raw FFI boundary: unsafe required to dereference native-provided pointers; no safe alternative exists at this layer.
pub extern "C" fn greg_mod_scene_loaded(scene: *const c_char) {
    if !scene.is_null() {
        let name = unsafe { std::ffi::CStr::from_ptr(scene).to_string_lossy() }; // nosemgrep: rust.lang.security.unsafe-usage.unsafe-usage -- raw FFI boundary: unsafe required to dereference native-provided pointers; no safe alternative exists at this layer.
        greg::log_info(&format!("[{MOD_ID}] scene: {name}"));
    }
}

#[no_mangle]
pub extern "C" fn greg_mod_shutdown() {
    greg::log_info(&format!("[{MOD_ID}] shutdown."));
}
