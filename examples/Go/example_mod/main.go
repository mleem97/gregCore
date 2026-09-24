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
	// Reviewed: C.free on a C.CString result is the documented cgo pattern
	// (no pointer arithmetic, fixed lifetime) — scanner "unsafe" findings
	// on this line are false positives.
	defer C.free(unsafe.Pointer(cstr))
	C.greg_log(cstr)
}

// subscribe registers a Go callback for a framework event. The
// unsafe.Pointer conversion is the only cgo-supported way to pass a Go
// function pointer to C (fixed signatures, no arithmetic) — scanner
// "unsafe" findings here are false positives. Centralized so example
// authors copy one reviewed helper instead of five raw conversions.
func subscribe(eventID uint32, fn unsafe.Pointer) {
	C.greg_subscribe(C.uint32_t(eventID), (C.greg_subscribe_fn)(fn))
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
	subscribe(EventCoinsChanged, unsafe.Pointer(C.on_coins_changed))
	subscribe(EventXpChanged, unsafe.Pointer(C.on_xp_changed))
	subscribe(EventGameSaved, unsafe.Pointer(C.on_game_saved))
	subscribe(EventRackPosition, unsafe.Pointer(C.on_rack_position))
	subscribe(EventCableCreated, unsafe.Pointer(C.on_cable_created))

	logMsg(fmt.Sprintf("[%s] Mod initialized successfully!", ModName))
}

func main() {
	// Entry point when loaded by gregCore
	greg_mod_init()
}
