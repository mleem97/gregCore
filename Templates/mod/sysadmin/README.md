# Sysadmin Multi-Language Templates

Diese Beispiele zeigen dasselbe Sysadmin-Verhalten in mehreren Sprachen:
- Service-Heartbeat loggen
- Event-Einstiegspunkt bereitstellen
- Hotload-freundliche Struktur

## Ordner

- `csharp/`: MelonMod-basierte Sysadmin-Mod (`.csproj` + `Main.cs`)
- `lua/`: Script-Variante für die Lua-Bridge
- `typescript/`: TS-Quelle + Build-Konfig (Transpile zu JS)
- `rust/`: Native FFI-Mod mit `mod_info`, `mod_init`, `mod_update`, `mod_shutdown`

## Test im Spiel (gregCore)

1. C#: DLL nach `Data Center/Mods/`.
2. Rust: Build-Artefakt nach `Data Center/Mods/RustMods/`.
3. Lua/TS(JS): Dateien nach `Data Center/Mods/ScriptMods/lua` bzw. `.../js`.
4. Spiel starten, im Main Menu `Mod Settings` öffnen und Runtime-Mods aktivieren/deaktivieren.
5. `Ctrl+Shift+R` lädt aktivierte Runtime-Units im Main Menu neu.
