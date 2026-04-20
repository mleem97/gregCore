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

#[no_mangle]
pub extern "C" fn greg_mod_init(api: *const GregCoreAPI) -> bool {
    unsafe {
        API = Some(&*api);
        let msg = CString::new("Rust Mod Initialisiert!").unwrap();
        ((*api).log_info)(msg.as_ptr());
    }
    true
}

#[no_mangle]
pub extern "C" fn greg_mod_update(dt: f32) {
    // Logik pro Frame
}
