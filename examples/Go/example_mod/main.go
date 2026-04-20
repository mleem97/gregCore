// example_mod/main.go
package main

/*
#include <stdint.h>
#include <stdbool.h>

typedef struct {
    uint32_t api_version;
    void (*log_info)(const char*);
    void (*log_warning)(const char*);
    void (*log_error)(const char*);
    double (*get_player_money)();
    // ... rest of fields
} GregCoreAPI;

typedef struct {
    const char* id;
    const char* name;
    const char* version;
    const char* author;
    const char* description;
    uint32_t api_version;
} GregModInfo;
*/
import "C"
import (
    "fmt"
    "unsafe"
)

//export greg_mod_info
func greg_mod_info() C.GregModInfo {
    return C.GregModInfo{
        id:          C.CString("go_example"),
        name:        C.CString("Go Example Mod"),
        version:     C.CString("1.0.0"),
        author:      C.CString("teamGreg"),
        description: C.CString("A sample mod in Go."),
        api_version: 1,
    }
}

//export onHookCallback
func onHookCallback(hookName, trigger, jsonData *C.char) {
    hookNameStr := C.GoString(hookName)
    triggerStr := C.GoString(trigger)
    jsonDataStr := C.GoString(jsonData)

    fmt.Printf("Go Hook received: %s (Trigger: %s) - Data: %s\n", hookNameStr, triggerStr, jsonDataStr)
}

//export greg_mod_init
func greg_mod_init(api *GregCoreAPI) bool {
    // Subscribe to a hook
    hookName := C.CString("greg.PLAYER.CoinChanged")
    defer C.free(unsafe.Pointer(hookName))
    
    api.OnHook(hookName, (unsafe.Pointer)(C.onHookCallback))
    
    return true
}

//export greg_mod_update
func greg_mod_update(dt float32) {
    // Update logic
}

//export greg_mod_shutdown
func greg_mod_shutdown() {
    fmt.Println("Go Example Mod shutdown.")
}

func main() {}
