# Go Hook API Tutorial

In Go mods, you'll use the `greg-go` package (or raw FFI via the `GregCoreAPI` struct) to interact with the Hook API.

## Subscribing to a Hook

To listen for an event, you'll use the `greg_mod_init` entry point to register your callbacks.

```go
// Example of a Go hook subscription
//export onHookCallback
func onHookCallback(hookName, trigger, jsonData *C.char) {
    hookNameStr := C.GoString(hookName)
    triggerStr := C.GoString(trigger)
    jsonDataStr := C.GoString(jsonData)

    fmt.Printf("Hook received: %s (Trigger: %s) - Data: %s\n", hookNameStr, triggerStr, jsonDataStr)
}

//export greg_mod_init
func greg_mod_init(api *GregCoreAPI) bool {
    // Subscribe to a hook
    hookName := C.CString("greg.PLAYER.CoinChanged")
    defer C.free(unsafe.Pointer(hookName))
    
    api.OnHook(hookName, (unsafe.Pointer)(C.onHookCallback))
    
    return true
}
```

## Firing a Hook

To fire a custom event from your Go mod, use the `FireHook` function pointer in the `GregCoreAPI` struct.

```go
// Fire a custom hook from Go
hookName := C.CString("greg.CUSTOM.MyEvent")
defer C.free(unsafe.Pointer(hookName))

jsonData := C.CString(`{"foo": "bar", "value": 42}`)
defer C.free(unsafe.Pointer(jsonData))

api.FireHook(hookName, jsonData)
```

## Troubleshooting

- Ensure your Go DLL is located in `Plugins/Go/`.
- Your Go mod must export `greg_mod_info` and `greg_mod_init`.
- Use the `greg-go` package for a more ergonomic API.
- Ensure the hook name starts with `greg.`.
