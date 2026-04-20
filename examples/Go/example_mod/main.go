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
import "unsafe"

var api *C.GregCoreAPI

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

//export greg_mod_init
func greg_mod_init(api_ptr *C.GregCoreAPI) bool {
    api = api_ptr
    msg := C.CString("Go Mod Initialized!")
    defer C.free(unsafe.Pointer(msg))
    C.bridge_log_info(api.log_info, msg)
    return true
}

// Helper to call C function pointers
//go:uintptrescapes
func callLog(fn unsafe.Pointer, msg *C.char) {
    // This requires cgo bridge helpers usually
}

func main() {}
