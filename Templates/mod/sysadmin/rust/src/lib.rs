use std::ffi::c_char;
use std::sync::atomic::{AtomicBool, Ordering};

#[repr(C)]
pub struct ModInfo {
    pub id: *const c_char,
    pub name: *const c_char,
    pub version: *const c_char,
    pub author: *const c_char,
    pub description: *const c_char,
}

static MOD_ID: &[u8] = b"native:sysadmin-rust\0";
static MOD_NAME: &[u8] = b"Sysadmin Rust Example\0";
static MOD_VERSION: &[u8] = b"0.1.0\0";
static MOD_AUTHOR: &[u8] = b"gregCore\0";
static MOD_DESCRIPTION: &[u8] = b"Sysadmin sample for gregCore native bridge\0";

static INITIALIZED: AtomicBool = AtomicBool::new(false);

#[no_mangle]
pub extern "C" fn mod_info() -> ModInfo {
    ModInfo {
        id: MOD_ID.as_ptr() as *const c_char,
        name: MOD_NAME.as_ptr() as *const c_char,
        version: MOD_VERSION.as_ptr() as *const c_char,
        author: MOD_AUTHOR.as_ptr() as *const c_char,
        description: MOD_DESCRIPTION.as_ptr() as *const c_char,
    }
}

#[no_mangle]
pub extern "C" fn mod_init(_api_table: *mut core::ffi::c_void) -> bool {
    INITIALIZED.store(true, Ordering::SeqCst);
    true
}

#[no_mangle]
pub extern "C" fn mod_update(_delta_time: f32) {
    if !INITIALIZED.load(Ordering::SeqCst) {
        return;
    }
}

#[no_mangle]
pub extern "C" fn mod_shutdown() {
    INITIALIZED.store(false, Ordering::SeqCst);
}
