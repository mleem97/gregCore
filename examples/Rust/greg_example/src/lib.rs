// gregCore Rust Example Mod

use std::ffi::{CStr, CString};
use std::os::raw::{c_char, c_void};

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
    pub api_version: u32,
    pub log_info:    extern "C" fn(*const c_char),
    pub log_warning: extern "C" fn(*const c_char),
    pub log_error:   extern "C" fn(*const c_char),
    pub get_player_money: extern "C" fn() -> f64,
    pub on_hook:          extern "C" fn(*const c_char, *const c_void),
    // ... restliche Felder
}

static mut API: Option<&GregCoreAPI> = None;

#[no_mangle]
pub extern "C" fn greg_mod_info() -> GregModInfo {
    GregModInfo {
        id:          b"rust_example\0".as_ptr() as *const c_char,
        name:        b"Rust Example Mod\0".as_ptr() as *const c_char,
        version:     b"1.0.0\0".as_ptr() as *const c_char,
        author:      b"teamGreg\0".as_ptr() as *const c_char,
        description: b"Ein Beispiel-Mod in Rust.\0".as_ptr() as *const c_char,
        api_version: 1,
    }
}

// Example of a Rust hook subscription
extern "C" fn on_hook_callback(hook_name: *const i8, trigger: *const i8, json_data: *const i8) {
    let hook_name = unsafe { CStr::from_ptr(hook_name).to_string_lossy() };
    let trigger = unsafe { CStr::from_ptr(trigger).to_string_lossy() };
    let json_data = unsafe { CStr::from_ptr(json_data).to_string_lossy() };

    println!("Rust Hook received: {} (Trigger: {}) - Data: {}", hook_name, trigger, json_data);
}

#[no_mangle]
pub extern "C" fn greg_mod_init(api: *const GregCoreAPI) -> bool {
    let api = unsafe { &*api };
    
    // Subscribe to a hook
    let hook_name = CString::new("greg.PLAYER.CoinChanged").unwrap();
    (api.on_hook)(hook_name.as_ptr(), on_hook_callback as *const ());
    
    true
}

#[no_mangle]
pub extern "C" fn greg_mod_update(dt: f32) {
    // Update logic
}

#[no_mangle]
pub extern "C" fn greg_mod_shutdown() {
    println!("Rust Example Mod shutdown.");
}
