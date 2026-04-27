// gregCore Rust SDK – Example Mod (Extended)
//
// Demonstrates full hook-callback integration for Rack, Cable, Player, and Server events.
// Build with: cargo build --release --target x86_64-pc-windows-msvc
// Place the DLL in: Data Center/Mods/RustMods/

use std::ffi::{CStr, CString};
use std::os::raw::{c_char, c_void};

// ─── FFI Structs ─────────────────────────────────────────────────────

#[repr(C)]
pub struct GregModInfo {
    pub id:          *const c_char,
    pub name:        *const c_char,
    pub version:     *const c_char,
    pub author:      *const c_char,
    pub description: *const c_char,
    pub api_version: u32,
}

#[repr(C)]
pub struct GregCoreAPI {
    pub api_version:      u32,
    pub log_info:         extern "C" fn(*const c_char),
    pub log_warning:      extern "C" fn(*const c_char),
    pub log_error:        extern "C" fn(*const c_char),
    pub get_player_money: extern "C" fn() -> f64,
    pub get_player_xp:    extern "C" fn() -> f64,
    pub on_hook:          extern "C" fn(*const c_char, *const c_void),
    pub subscribe_event:  extern "C" fn(u32, extern "C" fn(u32, u64)),
    pub fire_event:       extern "C" fn(u32, u64),
}

static mut API: Option<&GregCoreAPI> = None;

// ─── Event IDs (must match gregCore.Core.Events.EventIds) ────────────

mod events {
    pub const COINS_CHANGED: u32      = 1001;
    pub const XP_CHANGED: u32         = 1002;
    pub const REPUTATION_CHANGED: u32 = 1003;
    pub const GAME_SAVED: u32         = 2001;
    pub const SERVER_STATUS: u32      = 3001;
    pub const RACK_POSITION: u32      = 3002;
    pub const RACK_USED: u32          = 3003;
    pub const RACK_FREED: u32         = 3004;
    pub const CABLE_CREATED: u32      = 4001;
}

// ─── Helper ──────────────────────────────────────────────────────────

fn log_info(msg: &str) {
    unsafe {
        if let Some(api) = API {
            if let Ok(cstr) = CString::new(msg) {
                (api.log_info)(cstr.as_ptr());
            }
        }
    }
}

fn log_warning(msg: &str) {
    unsafe {
        if let Some(api) = API {
            if let Ok(cstr) = CString::new(msg) {
                (api.log_warning)(cstr.as_ptr());
            }
        }
    }
}

// ─── Hook Callbacks ──────────────────────────────────────────────────

/// Legacy hook callback (string-based)
extern "C" fn on_hook_callback(hook_name: *const i8, trigger: *const i8, json_data: *const i8) {
    let hook_name = unsafe { CStr::from_ptr(hook_name).to_string_lossy() };
    let trigger = unsafe { CStr::from_ptr(trigger).to_string_lossy() };
    let json_data = unsafe { CStr::from_ptr(json_data).to_string_lossy() };

    log_info(&format!(
        "[RustMod] Hook: {} (Trigger: {}) - Data: {}",
        hook_name, trigger, json_data
    ));
}

/// Event callback for coin changes
extern "C" fn on_coins_changed(event_id: u32, data: u64) {
    log_info(&format!("[RustMod] Coins changed! EventID={}, Data={}", event_id, data));
}

/// Event callback for XP changes
extern "C" fn on_xp_changed(event_id: u32, data: u64) {
    log_info(&format!("[RustMod] XP changed! EventID={}, Data={}", event_id, data));
}

/// Event callback for rack position queries
extern "C" fn on_rack_position(event_id: u32, data: u64) {
    log_info(&format!("[RustMod] Rack position queried! RackID={}", data));
}

/// Event callback for rack position used
extern "C" fn on_rack_used(event_id: u32, data: u64) {
    log_info(&format!("[RustMod] Rack position marked used! Data={}", data));
}

/// Event callback for cable creation
extern "C" fn on_cable_created(event_id: u32, data: u64) {
    log_info(&format!("[RustMod] Cable created! CableID={}", data));
}

/// Event callback for game saved
extern "C" fn on_game_saved(event_id: u32, data: u64) {
    log_info(&format!("[RustMod] Game saved! Timestamp={}", data));
}

/// Event callback for server status changes
extern "C" fn on_server_status(event_id: u32, data: u64) {
    log_info(&format!("[RustMod] Server status changed! Data={}", data));
}

// ─── Mod Lifecycle ───────────────────────────────────────────────────

#[no_mangle]
pub extern "C" fn greg_mod_info() -> GregModInfo {
    GregModInfo {
        id:          b"rust_example\0".as_ptr() as *const c_char,
        name:        b"Rust Example Mod\0".as_ptr() as *const c_char,
        version:     b"1.1.0\0".as_ptr() as *const c_char,
        author:      b"teamGreg\0".as_ptr() as *const c_char,
        description: b"Full-featured Rust SDK example with Rack/Cable/Player hooks.\0".as_ptr() as *const c_char,
        api_version: 2,
    }
}

#[no_mangle]
pub extern "C" fn greg_mod_init(api: *const GregCoreAPI) -> bool {
    if api.is_null() {
        return false;
    }

    let api_ref = unsafe { &*api };
    unsafe { API = Some(api_ref); }

    log_info("[RustMod v1.1.0] Initializing...");

    // Subscribe to legacy string-based hooks
    let hook_coin = CString::new("greg.PLAYER.CoinChanged").unwrap();
    (api_ref.on_hook)(hook_coin.as_ptr(), on_hook_callback as *const c_void);

    // Subscribe to typed events (new in gregCore 1.1.0)
    (api_ref.subscribe_event)(events::COINS_CHANGED, on_coins_changed);
    (api_ref.subscribe_event)(events::XP_CHANGED, on_xp_changed);
    (api_ref.subscribe_event)(events::GAME_SAVED, on_game_saved);
    (api_ref.subscribe_event)(events::SERVER_STATUS, on_server_status);
    (api_ref.subscribe_event)(events::RACK_POSITION, on_rack_position);
    (api_ref.subscribe_event)(events::RACK_USED, on_rack_used);
    (api_ref.subscribe_event)(events::CABLE_CREATED, on_cable_created);

    log_info("[RustMod] All event handlers registered.");
    log_info("[RustMod] Mod initialized successfully!");

    true
}

#[no_mangle]
pub extern "C" fn greg_mod_update(dt: f32) {
    // Per-frame update logic
    // Example: could query player stats periodically
    // unsafe {
    //     if let Some(api) = API {
    //         let money = (api.get_player_money)();
    //         let xp = (api.get_player_xp)();
    //     }
    // }
}

#[no_mangle]
pub extern "C" fn greg_mod_shutdown() {
    log_info("[RustMod] Shutting down...");
    unsafe { API = None; }
}
