// gregCore Go SDK – Example Mod
//
// Demonstrates how to use the gregCore Go API via CGo FFI Bridge.
// Build with: go build -buildmode=c-shared -o example_mod.dll main.go
// Place the DLL in: Data Center/Mods/GoMods/
//
// The CGo bridge exposes the same API as the Rust SDK:
//   greg_subscribe(event_id, callback)
//   greg_fire_event(event_id, data)
//   greg_log(message)

package main

/*
#include <stdint.h>

// gregCore FFI function pointers (populated by the framework at load time)
typedef void (*greg_log_fn)(const char* msg);
typedef void (*greg_subscribe_fn)(uint32_t event_id, void (*callback)(uint32_t, uint64_t));
typedef void (*greg_fire_event_fn)(uint32_t event_id, uint64_t data);

extern greg_log_fn         greg_log;
extern greg_subscribe_fn   greg_subscribe;
extern greg_fire_event_fn  greg_fire_event;
*/
import "C"

import (
	"fmt"
	"unsafe"
)

const (
	ModName    = "GoExampleMod"
	ModVersion = "1.0.0"
)

// Event IDs matching gregCore.Core.Events.EventIds
const (
	EventCoinsChanged      = 1001
	EventXpChanged         = 1002
	EventReputationChanged = 1003
	EventGameSaved         = 2001
	EventServerStatus      = 3001
	EventRackPosition      = 3002
	EventCableCreated      = 4001
)

// logMsg sends a message to the gregCore logger
func logMsg(msg string) {
	cstr := C.CString(msg)
	defer C.free(unsafe.Pointer(cstr))
	C.greg_log(cstr)
}

//export on_coins_changed
func on_coins_changed(eventId C.uint32_t, data C.uint64_t) {
	logMsg(fmt.Sprintf("[%s] Coins changed! EventID=%d, Data=%d", ModName, eventId, data))
}

//export on_xp_changed
func on_xp_changed(eventId C.uint32_t, data C.uint64_t) {
	logMsg(fmt.Sprintf("[%s] XP changed! EventID=%d, Data=%d", ModName, eventId, data))
}

//export on_game_saved
func on_game_saved(eventId C.uint32_t, data C.uint64_t) {
	logMsg(fmt.Sprintf("[%s] Game saved! EventID=%d", ModName, eventId))
}

//export on_rack_position
func on_rack_position(eventId C.uint32_t, data C.uint64_t) {
	logMsg(fmt.Sprintf("[%s] Rack position queried! Data=%d", ModName, data))
}

//export on_cable_created
func on_cable_created(eventId C.uint32_t, data C.uint64_t) {
	logMsg(fmt.Sprintf("[%s] Cable created! ID=%d", ModName, data))
}

//export greg_mod_init
func greg_mod_init() {
	logMsg(fmt.Sprintf("[%s v%s] Initializing...", ModName, ModVersion))

	// Subscribe to events
	C.greg_subscribe(EventCoinsChanged, (C.greg_subscribe_fn)(unsafe.Pointer(C.on_coins_changed)))
	C.greg_subscribe(EventXpChanged, (C.greg_subscribe_fn)(unsafe.Pointer(C.on_xp_changed)))
	C.greg_subscribe(EventGameSaved, (C.greg_subscribe_fn)(unsafe.Pointer(C.on_game_saved)))
	C.greg_subscribe(EventRackPosition, (C.greg_subscribe_fn)(unsafe.Pointer(C.on_rack_position)))
	C.greg_subscribe(EventCableCreated, (C.greg_subscribe_fn)(unsafe.Pointer(C.on_cable_created)))

	logMsg(fmt.Sprintf("[%s] Mod initialized successfully!", ModName))
}

func main() {
	// Entry point when loaded by gregCore
	greg_mod_init()
}
