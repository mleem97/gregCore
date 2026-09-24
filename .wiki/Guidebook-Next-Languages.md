# Guidebook Next Languages

What "more languages coming" means concretely: Python, Rust, Go, and C# scripts exist as **preview** surfaces today. Build production mods in Lua/C#/JS; prototype here only, pinned and verified.

## Python — beta, small bridge (`PythonFFIBridge`, pythonnet 3.0.5)

Snake-case calls: `log_info / warning / error`, `get/set_player_money`, `get/set_player_xp`, `get_server_count / get_broken_server_count / dispatch_repair_server`, `get_time_of_day / get_day / trigger_save`, `get_player_position → {x,y,z}`, `subscribe_event(id, cb)`, `on(hook, cb) → {hook_name, trigger}`; lifecycle methods are no-ops. Host: `GregPythonHost` (`HostId "python"`, `.py`, requires `Python.Runtime`).

> Staleness warning: `examples/Python/example_mod/main.py` mirrors the retired subscribe/numeric-`Events` shape, not this bridge. Verify every call against `PythonFFIBridge.cs` before shipping anything.

## Rust / Go — alpha FFI (`RustFFIBridge` / `GoFFIBridge`)

C ABI via the versioned `GameApiTable` (`ApiTableVersion`; compat table `API_VERSION = 19`, v1–v13). v7 Steam/P2P slots are inert no-ops for ABI stability — the game owns networking ([[Developer Native Coop]]). Sketches: `examples/Rust/greg_example/src/lib.rs`, `examples/Go/example_mod/main.go`. Per-language SDKs: `sdk/packs/greg-*-sdk.zip`.

## C# scripts — alpha, unverified (`GregCSharpScriptBridge`, `GregCSharpCompiler`, `IGregCSharpMod`)

Roslyn runtime for `.cs` scripts. Prefer compiled C# mods ([[Guidebook CSharp 01 Setup]]) for anything serious until this graduates.

## How to prototype safely

1. Pin the framework version you tested (`VERSION` 1.2.3) and re-verify each call in-game — preview surfaces change.
2. Keep preview code read-only and local (no saves, no UI, no networking) until its storage story exists.
3. Watch the guidebook: tracks graduate here (JS did) with setup + project chapters when their surface stabilizes.
