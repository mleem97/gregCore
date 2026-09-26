---
title: "Guidebook Debugging"
---
# Guidebook Debugging

Debug any mod in any language: logs, consoles, reload, and the per-language error paths. Framework internals: [Core Overview](./core-overview/); symptom tables: [FAQ Troubleshooting](./faq-troubleshooting/).

## The three observation points (all languages)

| Point | How | Shows |
|---|---|---|
| Loader log (`Latest.log`, game root) | `Mod registriert: … [ID: …]`, `[DynamicPatcher]`, `[gregCore][HwId]`, `HWID SYSTEM ACTIVE` | Boot, registration, patches, ID healing, all `Logger` + Lua `log_*` output |
| Backquote console (`` ` ``) | `help`, `clear`, `mods`, `menus`, `keys`, `version` | Registry, Hub menus, keybinds, framework version — no file reading |
| F1 Hub | Open/Close buttons, Settings jump | Whether your menu wiring (`RegisterOpener/Closer`, `BindToggle/Report`) is truthful |

## Lua

- **REPL (F12):** run `greg.*` calls live before putting them in `main.lua`; discover hooks via `greg.hooks.groups()` / `.list()`.
- **Hot-reload:** a `FileSystemWatcher` (`LuaHotReload`) watches `*.lua` under your mod dir — just save the file. `on_shutdown()` runs, the script reloads, `on_reload()` runs. No restart needed.
- **Errors:** `SafeCall` catches runtime errors per mod (`[LuaMod:<id>] Runtime error: …` in the log) and `LuaErrorOverlay.ReportError(modId, message, stackTrace?)` surfaces them in-game instead of killing other mods.
- **Budget:** `LuaProfiler` enforces ~2 ms/frame — empty `on_update`, poll with `greg.every`, never log per frame.

## C#

- No hot-reload: rebuild → overwrite `<game>/Mods/YourMod.dll` → restart (see [Guidebook CSharp 06 Deploy Debug](./guidebook-csharp-06-deploy-debug/)).
- `Logger.Info / Warning / Error` (with exception) is your printf; `GregDoctor` covers boot-level causes; `DisposeSubscriptions()` + `OnUnload` hygiene prevents ghost callbacks after reload.
- After a **game update**: vanilla-first test, re-copy interop assemblies, regenerate `GameApi` + hooks, `validate_contracts.py`, full test run.

## JS (beta)

- Errors surface as `[JsBridge] JS-Fehler: …` in the loader log — check there first; only five entry points exist (`logInfo/logWarning/logError/on/fire`), so an unknown call is always the bug.
- No REPL, no reload shortcut: restart the game after editing `UserLibs/Js/*.js`.

## The debugging loop

1. Reproduce once, note versions (game, framework via `` ` `` → `version`, loader, mod list via `mods`).
2. Read the log excerpt top-down: registration → patcher → your lines → first error.
3. Shrink: REPL (Lua) or minimal subscription until the failure reappears.
4. Fix, reload (Lua/JS-edit-then-restart), re-run your chapter checkpoint.
5. Still stuck → [FAQ Troubleshooting](./faq-troubleshooting/) → mod tracker (single-mod bug) or gregCore tracker (Hub/HUD/saves) with versions + log excerpt.
