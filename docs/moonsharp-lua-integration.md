---
title: MoonSharp Lua Integration
description: Technische Referenz zur Lua-Integration in gregCore über MoonSharp 2.0.0.
slug: /moonsharp-lua-integration
---

## Was ist MoonSharp und warum 2.0.0?

MoonSharp ist eine reine .NET-Lua-Implementierung ohne native Lua-DLL. Für `gregCore` bedeutet das:

- kompatibel mit IL2CPP/MelonLoader-Setup
- leichtes Embedding in `gregCore.dll`
- klare Sandbox-Steuerung

`gregCore` verwendet MoonSharp **2.0.0** als feste Framework-Dependency.

## Lua Lifecycle in gregCore

1. **Scan**: Registry sucht `*.lua` in `Mods/Scripts`.
2. **Activate**: `GregLuaHost` wird nur bei Treffer aktiviert.
3. **Execute**: `LuaFFIBridge` lädt Lua-Entrypoints.
4. **Unload**: `Shutdown()` leert Host-Zustand.

```csharp
// Signatur
public sealed class GregLuaHost : IGregLanguageHost
```

## Minimalbeispiel (funktionsfähig)

```lua
-- Datei: Mods/Scripts/demo.lua

greg.log_info("[demo.lua] init")

greg.on("greg.lifecycle.GameLoaded", function(payload)
    greg.log_info("Game loaded hook fired")
    greg.show_notification("Lua hook active")
end)
```

## Exponierte Lua-API (aktuell)

> Quelle: `LuaFFIBridge.RegisterApi`.

- Logging: `log_info`, `log_warning`, `log_error`
- Economy: `get_player_money`, `set_player_money`, `get_player_xp`, `set_player_xp`, `get_player_reputation`, `set_player_reputation`
- World: `get_server_count`, `get_rack_count`, `get_switch_count`, `get_broken_server_count`, `get_broken_switch_count`
- Technicians: `get_free_technician_count`, `get_total_technician_count`, `dispatch_repair_server`, `dispatch_repair_switch`
- Time: `get_time_of_day`, `get_day`, `get_seconds_in_full_day`, `set_seconds_in_full_day`
- Game: `get_current_scene`, `is_game_paused`, `set_game_paused`, `get_time_scale`, `set_time_scale`, `trigger_save`, `get_difficulty`
- Player/UI: `get_player_position`, `show_notification`
- Events/Hooks: `subscribe_event`, `fire_event`, `on`, `fire`
- Config: `config_set_bool`, `config_get_bool`, `config_set_int`, `config_get_int`, `config_set_string`, `config_get_string`

## Fehlerbehandlung

Lua-Syntax- oder Laufzeitfehler werden abgefangen und im MelonLoader-Log gemeldet.

```csharp
// Signatur
private static void SafeCall(LuaPlugin p, Closure? func, params object[] args)
```

## Debugging

- Primärer Kanal: `MelonLoader/Latest.log`
- Prefixe:
  - `[gregCore] ...` (Framework-Level)
  - `[LuaMod:<id>] ...` (Plugin-/Script-Layer)

## Manifest- und Config-Anforderungen

- Manifest: [UNVERIFIED] aktuelles Lua-Manifest-Schema im Repo nicht eindeutig versioniert.
- `config.json`: [UNVERIFIED] modulabhängig; empfohlen sind einfache Key/Value-Paare.

## Deployment

- Lege Lua-Dateien unter `Mods/Scripts` ab.
- Stelle sicher, dass mindestens eine `*.lua` vorhanden ist, damit `GregLuaHost` startet.

## Fehlende Bausteine

Siehe `MISSING.md` für offene Integrationspunkte (z. B. standardisiertes Lua-Manifest-Schema).
