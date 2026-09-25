// greg.rs — safe Rust bindings for the gregCore native API table (ABI v1).
//
// Layout MUST match `GregCoreAPI` in
// `src/gregCore.Bridge/RustFFI/RustFFIBridge.cs` field for field:
// `#[repr(C)]` + `Option<extern "C" fn>` (null-pointer optimized, same ABI
// as a raw pointer). Never reorder, remove, or insert fields —
// append-only on the C# side, mirrored here.
//
// Slots that table v1 leaves null (`unsubscribe_event`, all
// `config_*_int/float/string`) are `None`: every wrapper checks first,
// so calling them is a safe no-op / default, never a crash.
//
// Strings cross the boundary as NUL-terminated ANSI (`*const c_char`).
// The bridge copies synchronously (`PtrToStringAnsi`), so temporaries
// created per call are safe to drop afterwards.
//
// NOTE: `get_current_scene` allocates on the C# side per call. Cache the
// result instead of polling it every frame.

// Bindings modules are use-what-you-need: silence dead-code noise.
#![allow(dead_code)]

use std::ffi::{CStr, CString};
use std::os::raw::{c_char, c_void};
use std::sync::OnceLock;

// ─── Raw table (ABI) ────────────────────────────────────────────────

/// Callback for typed events: `greg_mod_event(event_id, data)`.
pub type EventCallback = extern "C" fn(u32, u64);
/// Callback for string hooks: `on_hook(hook, trigger, json)`.
pub type HookCallback = extern "C" fn(*const c_char, *const c_char, *const c_char);

#[repr(C)]
pub struct GregModInfo {
    pub id: *const c_char,
    pub name: *const c_char,
    pub version: *const c_char,
    pub author: *const c_char,
    pub description: *const c_char,
    pub api_version: u32,
}

#[repr(C)]
pub struct GregCoreAPI {
    pub api_version: u32,
    // Logging
    pub log_info: Option<extern "C" fn(*const c_char)>,
    pub log_warning: Option<extern "C" fn(*const c_char)>,
    pub log_error: Option<extern "C" fn(*const c_char)>,
    // Economy
    pub get_player_money: Option<extern "C" fn() -> f64>, pub set_player_money: Option<extern "C" fn(f64)>,
    pub get_player_xp: Option<extern "C" fn() -> f64>, pub set_player_xp: Option<extern "C" fn(f64)>,
    pub get_player_reputation: Option<extern "C" fn() -> f64>, pub set_player_reputation: Option<extern "C" fn(f64)>,
    // World
    pub get_server_count: Option<extern "C" fn() -> u32>,
    pub get_rack_count: Option<extern "C" fn() -> u32>,
    pub get_switch_count: Option<extern "C" fn() -> u32>,
    pub get_broken_server_count: Option<extern "C" fn() -> u32>,
    pub get_broken_switch_count: Option<extern "C" fn() -> u32>,
    // Technicians
    pub get_free_technician_count: Option<extern "C" fn() -> u32>,
    pub get_total_technician_count: Option<extern "C" fn() -> u32>,
    pub dispatch_repair_server: Option<extern "C" fn() -> i32>,
    pub dispatch_repair_switch: Option<extern "C" fn() -> i32>,
    // Time
    pub get_time_of_day: Option<extern "C" fn() -> f32>,
    pub get_day: Option<extern "C" fn() -> u32>,
    pub get_seconds_in_full_day: Option<extern "C" fn() -> f32>,
    pub set_seconds_in_full_day: Option<extern "C" fn(f32)>,
    // Game
    pub get_current_scene: Option<extern "C" fn() -> *mut c_char>,
    pub is_game_paused: Option<extern "C" fn() -> u32>,
    pub set_game_paused: Option<extern "C" fn(f64)>,
    pub get_time_scale: Option<extern "C" fn() -> f32>,
    pub set_time_scale: Option<extern "C" fn(f32)>,
    pub trigger_save: Option<extern "C" fn() -> i32>,
    pub get_difficulty: Option<extern "C" fn() -> i32>,
    // Player + UI
    pub get_player_position: Option<extern "C" fn(*mut f32, *mut f32, *mut f32, *mut f32)>,
    pub show_notification: Option<extern "C" fn(*const c_char)>,
    // Typed events
    pub subscribe_event: Option<extern "C" fn(u32, EventCallback)>,
    pub unsubscribe_event: Option<extern "C" fn(u32, EventCallback)>,
    pub fire_event: Option<extern "C" fn(u32, u64)>,
    // String hooks
    pub on_hook: Option<extern "C" fn(*const c_char, *const c_void)>,
    pub fire_hook: Option<extern "C" fn(*const c_char, *const c_char)>,
    // Config (bool only on table v1 — the rest is reserved null)
    pub config_set_bool: Option<extern "C" fn(*const c_char, *const c_char, u32)>, pub config_get_bool: Option<extern "C" fn(*const c_char, *const c_char, u32) -> u32>,
    pub config_set_int: Option<extern "C" fn(*const c_char, *const c_char, i32)>, pub config_get_int: Option<extern "C" fn(*const c_char, *const c_char, i32) -> i32>,
    pub config_set_float: Option<extern "C" fn(*const c_char, *const c_char, f32)>, pub config_get_float: Option<extern "C" fn(*const c_char, *const c_char, f32) -> f32>,
    pub config_set_string: Option<extern "C" fn(*const c_char, *const c_char, *const c_char)>,
    pub config_get_string: Option<extern "C" fn(*const c_char, *const c_char, *const c_char) -> *mut c_char>,
}

/// Table version this file was written against (`RustFFIBridge` sets 1).
/// Report this version in `greg_mod_info`. If the bridge ever ships a
/// higher version, re-check every field before bumping.
pub const TABLE_VERSION: u32 = 1;

// Compile-time ABI guard (64-bit game builds): any field added, removed,
// or reordered on either side breaks these offsets immediately.
const _: () = {
    assert!(std::mem::size_of::<GregCoreAPI>() == 360);
    assert!(std::mem::offset_of!(GregCoreAPI, log_info) == 8);
    assert!(std::mem::offset_of!(GregCoreAPI, get_player_money) == 32);
    assert!(std::mem::offset_of!(GregCoreAPI, subscribe_event) == 256);
    assert!(std::mem::offset_of!(GregCoreAPI, on_hook) == 280);
    assert!(std::mem::offset_of!(GregCoreAPI, config_set_bool) == 296);
};

// ─── Event IDs (must match `gregCore.Core.Events.EventIds`) ─────────

pub mod events {
    pub const PLAYER_COIN_UPDATED: u32 = 1001;
    pub const PLAYER_XP_UPDATED: u32 = 1002;
    pub const PLAYER_REPUTATION_UPDATED: u32 = 1003;
    pub const GAME_SAVED: u32 = 2001;
    pub const SERVER_STATUS_CHANGED: u32 = 3001;
    pub const RACK_POSITION_QUERIED: u32 = 3002;
    pub const RACK_POSITION_USED: u32 = 3003;
    pub const RACK_POSITION_FREED: u32 = 3004;
    pub const CABLE_CREATED: u32 = 4001;
    pub const INPUT_MOVE_OVERRIDDEN: u32 = 5001;
    pub const INPUT_LOOK_OVERRIDDEN: u32 = 5002;
    pub const INPUT_INTERACT_OVERRIDDEN: u32 = 5003;
}

// ─── Safe wrapper ───────────────────────────────────────────────────

static TABLE: OnceLock<TablePtr> = OnceLock::new();

/// Raw table pointer. `Send`/`Sync` is sound: the bridge writes the table
/// once during `greg_mod_init` (game thread) and it lives forever.
#[derive(Copy, Clone)]
struct TablePtr(*const GregCoreAPI);
unsafe impl Send for TablePtr {} // nosemgrep: rust.lang.security.unsafe-usage.unsafe-usage -- raw FFI boundary: unsafe required to dereference native-provided pointers; no safe alternative exists at this layer.
unsafe impl Sync for TablePtr {} // nosemgrep: rust.lang.security.unsafe-usage.unsafe-usage -- raw FFI boundary: unsafe required to dereference native-provided pointers; no safe alternative exists at this layer.

/// Stores the table handed to `greg_mod_init`. Call once, before use.
pub fn init(table: *const GregCoreAPI) -> bool {
    if table.is_null() {
        return false;
    }
    TABLE.set(TablePtr(table)).is_ok()
}

fn table() -> Option<&'static GregCoreAPI> {
    TABLE.get().and_then(|p| unsafe { p.0.as_ref() }) // nosemgrep: rust.lang.security.unsafe-usage.unsafe-usage -- raw FFI boundary: unsafe required to dereference native-provided pointers; no safe alternative exists at this layer.
}

fn cstr(s: &str) -> Option<CString> {
    CString::new(s).ok()
}

fn lossy(ptr: *const c_char) -> String {
    if ptr.is_null() {
        return String::new();
    }
    unsafe { CStr::from_ptr(ptr).to_string_lossy().into_owned() } // nosemgrep: rust.lang.security.unsafe-usage.unsafe-usage -- raw FFI boundary: unsafe required to dereference native-provided pointers; no safe alternative exists at this layer.
}

/// Decodes a hook callback triple `(hook, trigger, json)` into Strings.
/// Use inside your `extern "C"` hook function.
pub fn decode_hook(
    hook: *const c_char,
    trigger: *const c_char,
    json: *const c_char,
) -> (String, String, String) {
    (lossy(hook), lossy(trigger), lossy(json))
}

pub fn log_info(msg: &str) {
    if let Some(f) = table().and_then(|t| t.log_info) {
        if let Some(m) = cstr(msg) {
            f(m.as_ptr());
        }
    }
}

pub fn log_warning(msg: &str) {
    if let Some(f) = table().and_then(|t| t.log_warning) {
        if let Some(m) = cstr(msg) {
            f(m.as_ptr());
        }
    }
}

pub fn log_error(msg: &str) {
    if let Some(f) = table().and_then(|t| t.log_error) {
        if let Some(m) = cstr(msg) {
            f(m.as_ptr());
        }
    }
}

pub fn notify(msg: &str) {
    if let Some(f) = table().and_then(|t| t.show_notification) {
        if let Some(m) = cstr(msg) {
            f(m.as_ptr());
        }
    }
}

// Economy
pub fn get_player_money() -> f64 {
    table()
        .and_then(|t| t.get_player_money)
        .map(|f| f())
        .unwrap_or(0.0)
}
pub fn set_player_money(v: f64) {
    if let Some(f) = table().and_then(|t| t.set_player_money) {
        f(v);
    }
}
pub fn get_player_xp() -> f64 {
    table()
        .and_then(|t| t.get_player_xp)
        .map(|f| f())
        .unwrap_or(0.0)
}
pub fn set_player_xp(v: f64) {
    if let Some(f) = table().and_then(|t| t.set_player_xp) {
        f(v);
    }
}
pub fn get_player_reputation() -> f64 {
    table()
        .and_then(|t| t.get_player_reputation)
        .map(|f| f())
        .unwrap_or(0.0)
}
pub fn set_player_reputation(v: f64) {
    if let Some(f) = table().and_then(|t| t.set_player_reputation) {
        f(v);
    }
}

// World
pub fn get_server_count() -> u32 {
    table()
        .and_then(|t| t.get_server_count)
        .map(|f| f())
        .unwrap_or(0)
}
pub fn get_rack_count() -> u32 {
    table()
        .and_then(|t| t.get_rack_count)
        .map(|f| f())
        .unwrap_or(0)
}
pub fn get_switch_count() -> u32 {
    table()
        .and_then(|t| t.get_switch_count)
        .map(|f| f())
        .unwrap_or(0)
}
pub fn get_broken_server_count() -> u32 {
    table()
        .and_then(|t| t.get_broken_server_count)
        .map(|f| f())
        .unwrap_or(0)
}
pub fn get_broken_switch_count() -> u32 {
    table()
        .and_then(|t| t.get_broken_switch_count)
        .map(|f| f())
        .unwrap_or(0)
}

// Technicians
pub fn get_free_technician_count() -> u32 {
    table()
        .and_then(|t| t.get_free_technician_count)
        .map(|f| f())
        .unwrap_or(0)
}
pub fn get_total_technician_count() -> u32 {
    table()
        .and_then(|t| t.get_total_technician_count)
        .map(|f| f())
        .unwrap_or(0)
}
/// Returns the bridge result (see `GregAPI.DispatchRepairServer`).
pub fn dispatch_repair_server() -> i32 {
    table()
        .and_then(|t| t.dispatch_repair_server)
        .map(|f| f())
        .unwrap_or(0)
}
pub fn dispatch_repair_switch() -> i32 {
    table()
        .and_then(|t| t.dispatch_repair_switch)
        .map(|f| f())
        .unwrap_or(0)
}

// Time
pub fn get_time_of_day() -> f32 {
    table()
        .and_then(|t| t.get_time_of_day)
        .map(|f| f())
        .unwrap_or(0.0)
}
pub fn get_day() -> u32 {
    table().and_then(|t| t.get_day).map(|f| f()).unwrap_or(0)
}
pub fn get_seconds_in_full_day() -> f32 {
    table()
        .and_then(|t| t.get_seconds_in_full_day)
        .map(|f| f())
        .unwrap_or(0.0)
}
pub fn set_seconds_in_full_day(v: f32) {
    if let Some(f) = table().and_then(|t| t.set_seconds_in_full_day) {
        f(v);
    }
}

// Game
/// Copies the scene name immediately; cache it instead of polling.
pub fn get_current_scene() -> String {
    table()
        .and_then(|t| t.get_current_scene)
        .map(|f| lossy(f()))
        .unwrap_or_default()
}
pub fn is_game_paused() -> bool {
    table()
        .and_then(|t| t.is_game_paused)
        .map(|f| f() != 0)
        .unwrap_or(false)
}
pub fn set_game_paused(paused: bool) {
    if let Some(f) = table().and_then(|t| t.set_game_paused) {
        f(if paused { 1.0 } else { 0.0 });
    }
}
pub fn get_time_scale() -> f32 {
    table()
        .and_then(|t| t.get_time_scale)
        .map(|f| f())
        .unwrap_or(1.0)
}
pub fn set_time_scale(v: f32) {
    if let Some(f) = table().and_then(|t| t.set_time_scale) {
        f(v);
    }
}
pub fn trigger_save() {
    if let Some(f) = table().and_then(|t| t.trigger_save) {
        f();
    }
}
pub fn get_difficulty() -> i32 {
    table()
        .and_then(|t| t.get_difficulty)
        .map(|f| f())
        .unwrap_or(0)
}

/// Player position `(x, y, z, ry)`. All zeros when unbound.
pub fn get_player_position() -> (f32, f32, f32, f32) {
    if let Some(f) = table().and_then(|t| t.get_player_position) {
        let (mut x, mut y, mut z, mut ry) = (0f32, 0f32, 0f32, 0f32);
        f(&mut x, &mut y, &mut z, &mut ry);
        (x, y, z, ry)
    } else {
        (0.0, 0.0, 0.0, 0.0)
    }
}

// Typed events
pub fn subscribe_event(event_id: u32, cb: EventCallback) -> bool {
    if let Some(f) = table().and_then(|t| t.subscribe_event) {
        f(event_id, cb);
        true
    } else {
        false
    }
}
pub fn fire_event(event_id: u32, data: u64) -> bool {
    if let Some(f) = table().and_then(|t| t.fire_event) {
        f(event_id, data);
        true
    } else {
        false
    }
}

// String hooks
/// Subscribes `cb` (a `HookCallback` fn item) to a hook like
/// `"greg.PLAYER.CoinChanged"`. The callback receives
/// `(hook, trigger, json)` — decode with [`decode_hook`].
pub fn on_hook(hook_name: &str, cb: HookCallback) -> bool {
    if let Some(f) = table().and_then(|t| t.on_hook) {
        if let Some(h) = cstr(hook_name) {
            f(h.as_ptr(), cb as *const c_void);
            return true;
        }
    }
    false
}
pub fn fire_hook(hook_name: &str, json: &str) -> bool {
    if let Some(f) = table().and_then(|t| t.fire_hook) {
        if let (Some(h), Some(j)) = (cstr(hook_name), cstr(json)) {
            f(h.as_ptr(), j.as_ptr());
            return true;
        }
    }
    false
}

// Config (bool only on table v1)
pub fn config_set_bool(mod_id: &str, key: &str, value: bool) -> bool {
    if let Some(f) = table().and_then(|t| t.config_set_bool) {
        if let (Some(m), Some(k)) = (cstr(mod_id), cstr(key)) {
            f(m.as_ptr(), k.as_ptr(), value as u32);
            return true;
        }
    }
    false
}
pub fn config_get_bool(mod_id: &str, key: &str, default: bool) -> bool {
    if let Some(f) = table().and_then(|t| t.config_get_bool) {
        if let (Some(m), Some(k)) = (cstr(mod_id), cstr(key)) {
            return f(m.as_ptr(), k.as_ptr(), default as u32) != 0;
        }
    }
    default
}
