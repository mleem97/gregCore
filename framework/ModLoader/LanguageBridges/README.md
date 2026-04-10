# gregCore Language Bridges (Plugin Layer)

Diese Komponenten leben im **Plugin Layer / Language Bridges** und kapseln Mehrsprachen-Modding hinter einer stabilen Core-Schnittstelle.

## Enthaltene Brücken

- `LuaLanguageBridge` (`.lua`)
- `TypeScriptJavaScriptLanguageBridge` (`.ts`, `.js`, `.mjs`, `.cjs`)
- `RustLanguageBridgeAdapter` (delegiert an `FFIBridge` für native Mods)

## Aktueller Stand

- Core-Lifecycle ist angebunden (`Initialize`, `LoadAll`, `OnSceneLoaded`, `OnUpdate`, `Shutdown`).
- Script-Dateien werden in den jeweiligen Mod-Ordnern entdeckt.
- Fehler sind pro Bridge isoliert (`try/catch` + `CrashLog`), damit ein Bridge-Fehler den Rest nicht stoppt.

## Script-Ordner

- Lua: `Mods/ScriptMods/lua/`
- TypeScript/JavaScript: `Mods/ScriptMods/js/`
- Rust/native: `Mods/RustMods/` (über `FFIBridge`)

## Nächster Ausbau

- Lua VM Host anbinden (z. B. MoonSharp/KeraLua)
- JS Runtime Host anbinden (z. B. Jint/Node sidecar)
- TypeScript-Transpile-Pipeline (`.ts` -> `.js`) mit Hot-Reload
- `greg.*` API-Context als einheitliches Bridge-Objekt pro Sprache
