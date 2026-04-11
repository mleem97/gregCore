---
id: hexviewer-rust
title: HexViewer Mod Tutorial (Rust)
sidebar_label: HexViewer in Rust
slug: /development/tutorials/hexviewer-rust
description: Technical writing tutorial for the HexViewer Rust data layer in gregCore, including FFI contract, pipeline, and game validation.
---

This page implements the **Rust Low-Level Data layer** for HexViewer.

Role in architecture:

- Input event from C#: `greg.HEXVIEWER.TargetUpdated`
- Rust enrichment: resolve color/owner metadata at low overhead
- Output event to UI: `greg.HEXVIEWER.TargetEnriched`

> Layer ownership: Rust bridge is in **Language Bridge layer**, never direct Unity gameplay logic.

## Introduction

Rust is used for hot-path metadata operations where frequent target updates would otherwise allocate heavily on managed paths.

## Project structure (Rust segment)

```text
gregMod.HexViewer/rust/
├─ Cargo.toml
├─ src/
│  ├─ lib.rs
│  ├─ bridge.rs
│  └─ metadata.rs
└─ target/release/hexviewer_rust.dll
```

## FFI contract with gregCore

Required exported functions:

- `greg_mod_init` — register module and callbacks
- `greg_mod_shutdown` — release resources
- `greg_on_hexviewer_target_updated` — process event payload from C#

If your current bridge uses different names, keep semantics and align naming with `greg.*` conventions.

## Rust implementation

`rust/Cargo.toml`:

```toml
[package]
name = "hexviewer_rust"
version = "0.1.0"
edition = "2021"

[lib]
crate-type = ["cdylib"]
```

`rust/src/lib.rs`:

```rust
mod bridge;
mod metadata;

use std::ffi::CStr;
use std::os::raw::c_char;

#[no_mangle]
pub extern "C" fn greg_mod_init() {
    // gregCore-specific: register this Rust module with the bridge runtime
    bridge::register_module("hexviewer_rust");
}

#[no_mangle]
pub extern "C" fn greg_mod_shutdown() {
    bridge::unregister_module("hexviewer_rust");
}

#[no_mangle]
pub extern "C" fn greg_on_hexviewer_target_updated(
    entity_id: u64,
    entity_type_ptr: *const c_char,
) {
    let entity_type = unsafe {
        if entity_type_ptr.is_null() {
            "unknown"
        } else {
            CStr::from_ptr(entity_type_ptr).to_str().unwrap_or("unknown")
        }
    };

    let meta = metadata::query_entity_metadata(entity_id, entity_type);

    // gregCore-specific: emit enriched event back to the shared event bus
    bridge::emit_target_enriched(
        entity_id,
        entity_type,
        &meta.color_hex,
        &meta.owner_name,
    );
}
```

`rust/src/metadata.rs`:

```rust
pub struct EntityMeta {
    pub color_hex: String,
    pub owner_name: String,
}

pub fn query_entity_metadata(entity_id: u64, entity_type: &str) -> EntityMeta {
    // gregCore-specific placeholder:
    // Replace with gregCore::ffi metadata access when available in your branch.
    // Do NOT replace with direct engine pointers outside gregCore bridge.
    let _ = (entity_id, entity_type);

    EntityMeta {
        color_hex: "#55AAFF".to_string(),
        owner_name: "Unassigned".to_string(),
    }
}
```

## Build pipeline

```powershell
cargo build --manifest-path .\rust\Cargo.toml --release
```

Output:

- `rust\target\release\hexviewer_rust.dll`

## Integration with other languages

Data flow:

1. C# emits `greg.HEXVIEWER.TargetUpdated`
2. Rust callback receives entity id/type
3. Rust queries metadata and emits `greg.HEXVIEWER.TargetEnriched`
4. TypeScript HUD updates panel
5. Lua config filters what is displayed

## Deploy and test

```powershell
Copy-Item .\rust\target\release\hexviewer_rust.dll "C:\Program Files (x86)\Steam\steamapps\common\Data Center\Mods\RustMods\" -Force
```

Validation checklist:

- Bridge logs module registration
- No FFI crashes on hover/click
- `TargetEnriched` events contain color/owner fields

## Troubleshooting

- DLL not loaded: check `cdylib`, exports, and architecture (x64).
- Invalid strings: always guard pointer conversion.
- Missing metadata API: keep placeholder layer in `metadata.rs` until gregCore binding exists.

## 1) Prerequisites

- `rustup` + stable toolchain
- Working gregCore Rust bridge (`RustLanguageBridgeAdapter`/FFI)
- Data Center game setup with `Mods/RustMods/`

## 2) Create crate

```powershell
cargo new hexviewer --lib
```

`Cargo.toml`:

```toml
[package]
name = "hexviewer"
version = "0.1.0"
edition = "2021"

[lib]
crate-type = ["cdylib"]
```

## 3) Implement event hook

`src/lib.rs`:

```rust
use std::ffi::c_char;

#[no_mangle]
pub extern "C" fn greg_mod_init() {
    // Register callback through your bridge-specific API.
    // Example only: actual function names depend on bridge ABI.
    println!("[HexViewer/Rust] init");
}

#[no_mangle]
pub extern "C" fn greg_on_event(event_name: *const c_char, instance_id: u64) {
    // Convert pointers in your real bridge helper, then log a hex id:
    let hex = format!("0x{:08X}", instance_id as u32);
    let _ = event_name;
    println!("[HexViewer/Rust] hex={}", hex);
}
```

## 4) Build

```powershell
cargo build --release
```

Artifact example:

- `target/release/hexviewer.dll`

## 5) Deploy

Copy binary to Rust mods folder:

```powershell
Copy-Item .\target\release\hexviewer.dll "C:\Program Files (x86)\Steam\steamapps\common\Data Center\Mods\RustMods\" -Force
```

## 6) Test in game

1. Start Data Center.
2. Ensure bridge logs module load success.
3. Hover/click objects and verify Rust log lines with hex IDs.

## 7) Troubleshooting

- Module not loaded: confirm `cdylib` target and exported symbols.
- No events: validate FFI callback signatures against bridge ABI.
- Invalid strings/panic: guard all FFI boundaries and avoid `unwrap()`.

## 8) Next improvements

- Parse payload structs for richer labels.
- Add in-game toggle via shared config file.
- Emit events to C# side for UI rendering.
