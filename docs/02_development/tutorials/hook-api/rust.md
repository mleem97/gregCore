# Rust Hook API Tutorial

In Rust mods, you'll use the `greg-rs` crate (or raw FFI via the `GregCoreAPI` struct) to interact with the Hook API.

## Subscribing to a Hook

To listen for an event, you'll use the `greg_mod_init` entry point to register your callbacks.

```rust
// Example of a Rust hook subscription
extern "C" fn on_hook_callback(hook_name: *const i8, trigger: *const i8, json_data: *const i8) {
    let hook_name = unsafe { CStr::from_ptr(hook_name).to_string_lossy() };
    let trigger = unsafe { CStr::from_ptr(trigger).to_string_lossy() };
    let json_data = unsafe { CStr::from_ptr(json_data).to_string_lossy() };

    println!("Hook received: {} (Trigger: {}) - Data: {}", hook_name, trigger, json_data);
}

#[no_mangle]
pub extern "C" fn greg_mod_init(api: *const GregCoreAPI) -> bool {
    let api = unsafe { &*api };
    
    // Subscribe to a hook
    let hook_name = CString::new("greg.PLAYER.CoinChanged").unwrap();
    (api.on_hook)(hook_name.as_ptr(), on_hook_callback as *const ());
    
    true
}
```

## Firing a Hook

To fire a custom event from your Rust mod, use the `fire_hook` function pointer in the `GregCoreAPI` struct.

```rust
// Fire a custom hook from Rust
let hook_name = CString::new("greg.CUSTOM.MyEvent").unwrap();
let json_data = CString::new(r#"{"foo": "bar", "value": 42}"#).unwrap();
(api.fire_hook)(hook_name.as_ptr(), json_data.as_ptr());
```

## Troubleshooting

- Ensure your Rust DLL is located in `Plugins/Rust/`.
- Your Rust mod must export `greg_mod_info` and `greg_mod_init`.
- Use the `greg-rs` crate for a more ergonomic API.
- Ensure the hook name starts with `greg.`.
