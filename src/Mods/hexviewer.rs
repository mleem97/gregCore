// hexviewer.rs
// Rust implementation of HexViewer using gregCore v11 FFI

use std::ffi::{CStr, CString};
use std::os::raw::{c_char, c_float, c_int};

#[repr(C)]
pub struct GameAPITable {
    pub api_version: u32,
    // ... [v1-v10 fields omitted for brevity] ...
    pub _reserved: [usize; 91], // Padding to match v11 layout 

    // v11 — New SDK Services
    pub hud_update_jade_box: extern "C" fn(*const c_char, *const c_char, *const c_char),
    pub hud_hide_jade_box: extern "C" fn(),
    pub get_target_info: extern "C" fn(c_float, *mut *const c_char, *mut *const c_char, *mut c_float, *mut c_float, *mut c_float, *mut c_float) -> c_int,
    pub get_metadata: extern "C" fn(c_float) -> *const c_char,
}

static mut API: Option<&'static GameAPITable> = None;

#[no_mangle]
pub extern "C" fn mod_init(api: &'static GameAPITable) -> bool {
    unsafe { API = Some(api) };
    true
}

#[no_mangle]
pub extern "C" fn mod_update(dt: f32) {
    let api = unsafe { API.as_ref().unwrap() };

    // 1. Get Metadata as a serialized string from SDK
    let meta_ptr = (api.get_metadata)(10.0);
    if meta_ptr.is_null() {
        (api.hud_hide_jade_box)();
        return;
    }

    let meta_str = unsafe { CStr::from_ptr(meta_ptr) }.to_str().unwrap_or("");
    if meta_str.is_empty() {
        (api.hud_hide_jade_box)();
        return;
    }

    // 2. The SDK returned a serialized string: "label|value|#hex;..."
    let title = CString::new("TARGET DETECTED").unwrap();
    let sub_header = CString::new("RUST TELEMETRY").unwrap();
    let entries_json = CString::new(meta_str).unwrap();

    // 3. Update the HUD
    (api.hud_update_jade_box)(title.as_ptr(), sub_header.as_ptr(), entries_json.as_ptr());
}

