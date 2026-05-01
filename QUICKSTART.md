# gregCore QuickStart Guide

> **Version:** 1.1.0  
> **Target:** Unity 6.4+ IL2CPP | MelonLoader 0.7+  
> **Status:** Production Ready

---

## 1. Installation (Player / Mod-User)

1. Installiere [MelonLoader](https://melonwiki.xyz) in dein Spielverzeichnis.
2. Kopiere `gregCore.dll` in den `Mods/` Ordner.
3. Kopiere `game_hooks.json` und `greg_hooks.json` in den `Mods/` Ordner (oder lasse sie im gleichen Verzeichnis wie `gregCore.dll`).
4. Starte das Spiel einmal. gregCore erstellt automatisch:
   - `UserData/gregCore/Mods/Lua/` – Lua-Mod-Verzeichnis
   - `UserData/gregCore/Mods/Scripts/` – Allgemeines Script-Verzeichnis

---

## 2. Dein Erster Mod (Lua – Empfohlen)

### 2.1 Ordnerstruktur

```
UserData/gregCore/Mods/Lua/
└── my_first_mod/
    ├── mod.json
    └── main.lua
```

### 2.2 mod.json (Manifest)

```json
{
  "id": "my_first_mod",
  "name": "My First Mod",
  "version": "1.0.0",
  "author": "Your Name",
  "description": "A starter template for gregCore Lua modding.",
  "entry": "main.lua",
  "min_framework_version": "1.1.0"
}
```

### 2.3 main.lua

```lua
-- Lifecycle: Called when the mod is first loaded
function on_init()
    greg.ui.log_info("Hello from My First Mod!")
    
    -- Read player money
    local money = greg.player.money()
    greg.ui.log_info("Current balance: $" .. string.format("%.2f", money))
    
    -- Subscribe to a game event
    greg.on("greg.PLAYER.CoinChanged", function(payload)
        local amount = payload.data["Amount"]
        greg.ui.log_info("Money changed by: " .. tostring(amount))
    end)
end

-- Lifecycle: Called every frame
function on_update(dt)
    -- Be careful with logging here!
end

-- Lifecycle: Called when scene changes
function on_scene_loaded(name)
    greg.ui.log_info("Scene loaded: " .. name)
end

-- Lifecycle: Called on mod shutdown
function on_shutdown()
    greg.ui.log_info("My First Mod shutting down...")
end
```

### 2.4 In-Game REPL (F12)

Drücke `F12` im Spiel für die Lua-Konsole:

```lua
-- Direkte API-Aufrufe
print(greg.player.money())
greg.player.add_money(1000)
greg.ui.notify("Hello from REPL!")
```

---

## 3. Verfügbare Lua-API (Auswahl)

### Player (`greg.player`)

| Funktion | Beschreibung |
|----------|-------------|
| `greg.player.money()` | Aktuelles Geld |
| `greg.player.add_money(n)` | Geld hinzufügen |
| `greg.player.set_money(n)` | Geld setzen |
| `greg.player.xp()` | Aktuelle XP |
| `greg.player.reputation()` | Reputation |
| `greg.player.teleport(x, y, z)` | Spieler teleportieren |
| `greg.player.position()` | `{x, y, z}` Position |

### Server / Hardware (`greg.server`)

| Funktion | Beschreibung |
|----------|-------------|
| `greg.server.count()` | Anzahl Server |
| `greg.server.broken_count()` | Anzahl defekter Server |
| `greg.server.get_all()` | Tabelle aller Server |
| `greg.server.repair(hash)` | Einen Server reparieren |
| `greg.server.repair_all()` | Alle defekten Server reparieren |

### Welt / Zeit (`greg.world`)

| Funktion | Beschreibung |
|----------|-------------|
| `greg.world.time_of_day()` | Tageszeit (0-24) |
| `greg.world.day()` | Aktueller Tag |
| `greg.world.pause()` | Spiel pausieren |
| `greg.world.resume()` | Spiel fortsetzen |
| `greg.world.set_time_scale(n)` | Zeitfaktor setzen |

### Events (`greg`)

| Funktion | Beschreibung |
|----------|-------------|
| `greg.on(hook, callback)` | Auf Event hören |
| `greg.once(hook, callback)` | Einmalig auf Event hören |
| `greg.fire(hook, data)` | Eigenes Event feuern |

### UI (`greg.ui`)

| Funktion | Beschreibung |
|----------|-------------|
| `greg.ui.notify(msg, seconds?)` | In-Game Notification |
| `greg.ui.log_info(msg)` | Info-Log (DevConsole) |
| `greg.ui.log_warning(msg)` | Warn-Log |
| `greg.ui.log_error(msg)` | Error-Log |

### IO (`greg.io`) – Sandboxed auf `mod/data/`

| Funktion | Beschreibung |
|----------|-------------|
| `greg.io.read_file(path)` | Datei lesen |
| `greg.io.write_file(path, content)` | Datei schreiben |
| `greg.io.file_exists(path)` | Prüfen ob Datei existiert |
| `greg.io.list_files(pattern?)` | Dateien auflisten |

### Coroutinen / Timer

| Funktion | Beschreibung |
|----------|-------------|
| `greg.start_coroutine(fn)` | Coroutine starten |
| `greg.wait(seconds)` | In Coroutine warten |
| `greg.every(seconds, fn)` | Wiederholender Timer |

---

## 4. Hook-System (1771+ Hooks)

gregCore patched automatisch alle Methoden aus `game_hooks.json`. Du kannst auf sie hören:

```lua
greg.on("greg.Audio.AudioManager.SetEffectsVolume", function(payload)
    greg.ui.log_info("Volume changed!")
end)

-- Alle verfügbaren Hooks anzeigen
greg.hooks.groups()          -- Gruppenliste
greg.hooks.audio.list()      -- Hooks in Gruppe "Audio"
```

---

## 5. Andere Sprachen

| Sprache | Dateiendung | Status | Hinweis |
|---------|------------|--------|---------|
| **Lua** | `.lua` | Production Ready | Vollständige API |
| **JavaScript** | `.js` | Beta | Jint-Runtime, basic |
| **Python** | `.py` | Beta | Python.NET, basic |
| **Rust** | `.rs` | Alpha | FFI-Bridge |
| **C# Scripts** | `.cs` | Alpha | Roslyn-Runtime (unverified) |

---

## 6. Troubleshooting

### "No hooks found" / Events funktionieren nicht
- Prüfe dass `game_hooks.json` im gleichen Verzeichnis wie `gregCore.dll` liegt.
- Prüfe die MelonLoader-Logs auf `[DynamicPatcher]` Meldungen.

### "Lua mod not loading"
- Ordner muss unter `UserData/gregCore/Mods/Lua/` liegen.
- `main.lua` muss existieren.
- `mod.json` ist optional aber empfohlen.

### "Dependency missing" für JS/Python
- Stelle sicher dass die entsprechenden NuGet-Pakete (Jint, pythonnet) in gregCore eingebunden sind.
- Bei selbst-kompilierten Builds: Prüfe `gregCore.csproj` PackageReferences.

---

## 7. Weiterführende Links

- `examples/Lua/starter_template/` – Minimaler Start
- `examples/Lua/example_mod/` – Event-Beispiel
- `examples/Lua/advanced_automation/` – Coroutinen + Timer
- `docs/FrameworkAPI.md` – Vollständige Hook-Referenz (autogeneriert)

---

**Happy Modding!** – TeamGreg
