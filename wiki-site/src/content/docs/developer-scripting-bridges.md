---
title: "Developer Scripting Bridges"
---
# Developer Scripting Bridges

Multi-language reality check: what Lua / JS / Python / Rust / Go / C# can actually do today.

## Lua — full API (MoonSharp 2.0.0, production)

Host: `GregLuaHost` (`HostId "lua"`, `.lua`) → `LuaFFIBridge` (`UserData/gregCore/Mods/Lua`, `@shared`, `Preset_SoftSandbox`, `LuaModuleLoader`, hot-reload, `LuaCoroutineScheduler`, `LuaHookBindingGenerator`, `LuaRepl`, `LuaProfiler`, `LuaErrorOverlay`).

Namespaces: `greg.player / greg.server / greg.switch / greg.tech / greg.rack / greg.cable / greg.patch / greg.customer / greg.economy (read-only) / greg.shop / greg.net (read-only) / greg.mods / greg.requests (read-only) / greg.subnet / greg.items / greg.world / greg.ui / greg.config / greg.save / greg.io / greg.json`, events (`on / once / off / fire`, `greg.hooks.<group>.*`), timers (`wait / every / start_coroutine`), tablets/widgets (`tablet_open / widget_open / panel_add_*`). Exact signatures: [Hooks Reference](./hooks-reference/).

## JavaScript — beta (Jint 4.8.0)

Host: `GregJsHost` (`HostId "javascript"`, `.js`/`.ts` — `.ts` is **not** transpiled, warning only; 4 MB memory limit). `JsBridge` exposes `greg.logInfo / logWarning / logError`, `greg.on(hook, cb)`, `greg.fire(hook, dict)`; loads `./UserLibs/Js/*.js` (legacy `./Plugins/Js` is ignored with a warning).

> Caution: `examples/Js/example_mod/main.js` still uses the stale `greg.subscribe / fire_event / log` + numeric `Events` form and does **not** match the current bridge. Treat [Hooks Reference](./hooks-reference/) + `JsBridge.cs` as truth, not the example.

## Python — beta (pythonnet 3.0.5)

`PythonFFIBridge`: `log_info / warning / error`, `get/set_player_money`, `get/set_player_xp`, `get_server_count / get_broken_server_count / dispatch_repair_server`, `get_time_of_day / get_day / trigger_save`, `get_player_position → {x,y,z}`, `subscribe_event(id, cb)`, `on(hook, cb) → {hook_name, trigger}`; lifecycle methods are no-ops.

> Same staleness warning: `examples/Python/example_mod/main.py` mirrors the old JS example shape, not the current snake_case bridge.

## Rust / Go / C# scripts — alpha

- **Rust** (`RustFFIBridge`) / **Go** (`GoFFIBridge`): C ABI via `GameApiTable` (`ApiTableVersion`, `API_VERSION = 19` compat table v1–v13). v7 Steam/P2P slots are inert no-ops for ABI stability ([Developer Native Coop](./developer-native-coop/)). Examples: `examples/Rust/greg_example/src/lib.rs`, `examples/Go/example_mod/main.go`.
- **C# scripts** (`GregCSharpScriptBridge`, `GregCSharpCompiler`, `IGregCSharpMod`): Roslyn runtime, unverified — prefer compiled C# mods ([Developer First CSharp Mod](./developer-first-csharp-mod/)) for anything serious.
- **Native mods** (`NativeModLoader`, `Win32FfiBridge`, `GregNativeModService`, `IGregNativeModService` / `IGregNativePlugin`): discovered via `AssemblyScanner` + `GregDependencyResolver`; `sdk/packs/greg-*-sdk.zip` ship the per-language SDKs.

## Choosing

Need world edits, spawning, cable ops, or custom patches → **C#**. Need full game API with fast iteration → **Lua**. Need JS/Python/Rust/Go → prototype against the small current surface first and pin `min_framework_version`; expect alpha/beta edges and verify every call in the REPL/log before shipping.
