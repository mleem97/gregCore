// lib.rs — minimal gregCore Rust mod built on `greg.rs`.
// Copy this whole folder, rename the crate + mod id, build, ship the DLL.

mod greg;

use std::ffi::c_char;

const MOD_ID: &str = "rust_example";
const MOD_NAME: &str = "Rust Example Mod";

extern "C" fn on_coins_changed(_event_id: u32, data: u64) {
    greg::log_info(&format!("[{MOD_ID}] coins changed, data={data}"));
}

extern "C" fn on_game_saved(_event_id: u32, data: u64) {
    greg::log_info(&format!("[{MOD_ID}] game saved, data={data}"));
}

extern "C" fn on_any_hook(hook: *const c_char, trigger: *const c_char, json: *const c_char) {
    let (h, t, j) = greg::decode_hook(hook, trigger, json);
    greg::log_info(&format!("[{MOD_ID}] hook {h} via {t}: {j}"));
}

#[no_mangle]
pub extern "C" fn greg_mod_info() -> greg::GregModInfo {
    greg::GregModInfo {
        id: c"rust_example".as_ptr(),
        name: c"Rust Example Mod".as_ptr(),
        version: c"1.0.0".as_ptr(),
        author: c"you".as_ptr(),
        description: c"Minimal Rust mod template.".as_ptr(),
        api_version: greg::TABLE_VERSION,
    }
}

#[no_mangle]
pub extern "C" fn greg_mod_init(api: *const greg::GregCoreAPI) -> bool {
    if !greg::init(api) {
        return false;
    }
    greg::log_info(&format!(
        "[{MOD_ID}] init (money={})",
        greg::get_player_money()
    ));
    greg::subscribe_event(greg::events::PLAYER_COIN_UPDATED, on_coins_changed);
    greg::subscribe_event(greg::events::GAME_SAVED, on_game_saved);
    greg::on_hook("greg.PLAYER.CoinChanged", on_any_hook);
    greg::notify(&format!("{MOD_NAME} loaded."));
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
    if scene.is_null() {
        return;
    }
    let name = unsafe { std::ffi::CStr::from_ptr(scene).to_string_lossy() }; // nosemgrep: rust.lang.security.unsafe-usage.unsafe-usage -- raw FFI boundary: unsafe required to dereference native-provided pointers; no safe alternative exists at this layer.
    greg::log_info(&format!("[{MOD_ID}] scene: {name}"));
}

#[no_mangle]
pub extern "C" fn greg_mod_shutdown() {
    greg::log_info(&format!("[{MOD_ID}] shutdown."));
}
