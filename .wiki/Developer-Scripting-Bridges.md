# Developer Scripting Bridges

Multi-language reality check: what Lua / JS / Python / Rust / Go / C# can actually do today.

## Lua — full API (MoonSharp 2.0.0, production)

Host: `GregLuaHost` (`HostId "lua"`, `.lua`) → `LuaFFIBridge` (`UserData/gregCore/Mods/Lua`, `@shared`, `Preset_SoftSandbox`, `LuaModuleLoader`, hot-reload, `LuaCoroutineScheduler`, `LuaHookBindingGenerator`, `LuaRepl`, `LuaProfiler`, `LuaErrorOverlay`).

Namespaces: `greg.player / greg.server / greg.switch / greg.tech / greg.rack / greg.cable / greg.patch / greg.customer / greg.economy (read-only) / greg.shop / greg.net (read-only) / greg.mods / greg.requests (read-only) / greg.subnet / greg.items / greg.world / greg.ui / greg.config / greg.save / greg.io / greg.json`, events (`on / once / off / fire`, `greg.hooks.<group>.*`), timers (`wait / every / start_coroutine`), tablets/widgets (`tablet_open / widget_open / panel_add_*`). Exact signatures: [[Hooks Reference]].

## JavaScript/TypeScript — UI-focused SDK (Jint 4.8.0)

Host: `GregJsHost` (`HostId "javascript"`). Layout: `UserData/gregCore/Mods/JS/<modId>/*.js` (one isolated engine per mod, 16 MB cap). Write TS against `templates/js/greg.d.ts`, compile with `tsc`, ship `.js` (raw `.ts` warns and skips). API: `log/warn/error`, `toast/toastRich/notify`, `createPanel` (chainable Toolkit builder), `bindMenu/reportMenu`, `registerToggle/registerSlider/registerKey`, `on` (hook bus), `onUpdate/onSceneLoaded` callbacks. HotLoad: edit + save, applied on next main-menu entry (queued otherwise). Full guide: [[Guidebook JS 01 Setup]] + [[Guidebook JS 02 Project]].

> Legacy flat path `UserLibs/Js/*.js` (`JsBridge`: `logInfo/logWarning/logError/on/fire`) still loads untouched. New mods use the per-mod layout above.

## Python — beta (pythonnet 3.0.5)

`PythonFFIBridge`: `log_info / warning / error`, `get/set_player_money`, `get/set_player_xp`, `get_server_count / get_broken_server_count / dispatch_repair_server`, `get_time_of_day / get_day / trigger_save`, `get_player_position → {x,y,z}`, `subscribe_event(id, cb)`, `on(hook, cb) → {hook_name, trigger}`; lifecycle methods are no-ops.

> Same staleness warning: `examples/Python/example_mod/main.py` mirrors the old JS example shape, not the current snake_case bridge.

## Rust / Go / C# scripts — alpha

- **Rust** (`RustFFIBridge`) / **Go** (`GoFFIBridge`): C ABI via `GameApiTable` (`ApiTableVersion`, `API_VERSION = 19` compat table v1–v13). v7 Steam/P2P slots are inert no-ops for ABI stability ([[Developer Native Coop]]). Examples: `examples/Rust/greg_example/src/lib.rs`, `examples/Go/example_mod/main.go`.
- **C# scripts** (`GregCSharpScriptBridge`, `GregCSharpCompiler`, `IGregCSharpMod`): Roslyn runtime, unverified — prefer compiled C# mods ([[Developer First CSharp Mod]]) for anything serious.
- **Native mods** (`NativeModLoader`, `Win32FfiBridge`, `GregNativeModService`, `IGregNativeModService` / `IGregNativePlugin`): discovered via `AssemblyScanner` + `GregDependencyResolver`; `sdk/packs/greg-*-sdk.zip` ship the per-language SDKs.

## Choosing

Need world edits, spawning, cable ops, or custom patches → **C#**. Need full game API with fast iteration → **Lua**. Need JS → use the UI SDK (`greg.d.ts`) and verify every call in the log before shipping. Need Python/Rust/Go → prototype against the small current surface first and pin `min_framework_version`; expect alpha/beta edges.
