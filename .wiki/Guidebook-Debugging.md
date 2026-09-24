# Guidebook Debugging

Debug any mod in any language: logs, consoles, reload, and the per-language error paths. Framework internals: [[Core Overview]]; symptom tables: [[FAQ Troubleshooting]].

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

- No hot-reload: rebuild → overwrite `<game>/Mods/YourMod.dll` → restart (see [[Guidebook CSharp 06 Deploy Debug]]).
- `Logger.Info / Warning / Error` (with exception) is your printf; `GregDoctor` covers boot-level causes; `DisposeSubscriptions()` + `OnUnload` hygiene prevents ghost callbacks after reload.
- After a **game update**: vanilla-first test, re-copy interop assemblies, regenerate `GameApi` + hooks, `validate_contracts.py`, full test run.

## JS/TS

- Errors surface as `[gregCore][JS] <modId>/<file>: …` in the loader log — check there first (mod-tagged, per file).
- No restart needed: edit + save, return to the **main menu**, watch for `Reloaded`. Mid-game edits stay queued. Raw `.ts` warns and skips — compile with `tsc` first.

## The debugging loop

1. Reproduce once, note versions (game, framework via `` ` `` → `version`, loader, mod list via `mods`).
2. Read the log excerpt top-down: registration → patcher → your lines → first error.
3. Shrink: REPL (Lua) or minimal subscription until the failure reappears.
4. Fix, reload (Lua HotLoad, JS main-menu HotLoad, C# restart), re-run your chapter checkpoint.
5. Still stuck → [[FAQ Troubleshooting]] → mod tracker (single-mod bug) or gregCore tracker (Hub/HUD/saves) with versions + log excerpt.
