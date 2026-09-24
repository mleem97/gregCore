# gregCore QuickStart Guide

> **Version:** 1.2.3  
> **Target:** Unity 6.4+ IL2CPP | MelonLoader 0.7+  
> **Status:** Production Ready

---

## 1. Installation (Player / Mod User)

1. Install [MelonLoader](https://melonwiki.xyz) into your game directory.
2. Copy `gregCore.dll` into the `Mods/` folder.
3. Copy `game_hooks.json` and `greg_hooks.json` into the `Mods/` folder (or leave them in the same directory as `gregCore.dll`).
4. Start the game once. gregCore automatically creates:
   - `UserData/gregCore/Mods/Lua/` – Lua mod directory
   - `UserData/gregCore/Mods/Scripts/` – general script directory

---

## 2. Building from Source (Developers)

Prerequisites: **.NET 6 SDK**, local Data Center / MelonLoader installation,
reference assemblies under `references/` (play once with MelonLoader,
then copy `MelonLoader/Il2CppAssemblies/` and `MelonLoader/net6/`).

```bash
git clone https://github.com/mleem97/gregCore.git
cd gregCore

# Release build (recommended)
dotnet build -c Release
# or both:
./build.sh --both

# Artifact: bin/Release/net6.0/gregCore.dll (or Releases/ after build.sh)
```

### Tests

```bash
DOTNET_ROLL_FORWARD=Major dotnet test
```

> `DOTNET_ROLL_FORWARD=Major` is only needed if no .NET 6 runtime
> is installed on the host (e.g. only .NET 8/10). Project targets `net6.0`.

### Check Version

```bash
python3 scripts/validate_version.py 1.2.3   # → exit 0
```

Source of truth: [`VERSION`](VERSION) · Changes: [`CHANGELOG.md`](CHANGELOG.md).

---

## 3. Your First Mod (Lua – Recommended)

### 3.1 Folder Structure

```
UserData/gregCore/Mods/Lua/
└── my_first_mod/
    ├── mod.json
    └── main.lua
```

### 3.2 mod.json (Manifest)

```json
{
  "id": "my_first_mod",
  "name": "My First Mod",
  "version": "1.0.0",
  "author": "Your Name",
  "description": "A starter template for gregCore Lua modding.",
  "entry": "main.lua",
  "min_framework_version": "1.2.3"
}
```

### 3.3 main.lua

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

### 3.4 In-Game REPL (F12)

Press `F12` in-game for the Lua console:

```lua
-- Direct API calls
print(greg.player.money())
greg.player.add_money(1000)
greg.ui.notify("Hello from REPL!")
```

---

## 4. Available Lua API (Selection)

### Player (`greg.player`)

| Function | Description |
|----------|-------------|
| `greg.player.money()` | Current money |
| `greg.player.add_money(n)` | Add money |
| `greg.player.set_money(n)` | Set money |
| `greg.player.xp()` | Current XP |
| `greg.player.reputation()` | Reputation |
| `greg.player.teleport(x, y, z)` | Teleport player |
| `greg.player.position()` | `{x, y, z}` position |

### Server / Hardware (`greg.server`)

| Function | Description |
|----------|-------------|
| `greg.server.count()` | Server count |
| `greg.server.broken_count()` | Number of broken servers |
| `greg.server.get_all()` | Table of all servers |
| `greg.server.repair(hash)` | Repair a server |
| `greg.server.repair_all()` | Repair all broken servers |

### World / Time (`greg.world`)

| Function | Description |
|----------|-------------|
| `greg.world.time_of_day()` | Time of day (0-24) |
| `greg.world.day()` | Current day |
| `greg.world.pause()` | Pause game |
| `greg.world.resume()` | Resume game |
| `greg.world.set_time_scale(n)` | Set time scale |

### Events (`greg`)

| Function | Description |
|----------|-------------|
| `greg.on(hook, callback)` | Listen for event |
| `greg.once(hook, callback)` | Listen for event once |
| `greg.fire(hook, data)` | Fire custom event |

### UI (`greg.ui`)

| Function | Description |
|----------|-------------|
| `greg.ui.notify(msg, seconds?)` | In-game notification |
| `greg.ui.log_info(msg)` | Info log (DevConsole) |
| `greg.ui.log_warning(msg)` | Warning log |
| `greg.ui.log_error(msg)` | Error log |

### IO (`greg.io`) – Sandboxed to `mod/data/`

| Function | Description |
|----------|-------------|
| `greg.io.read_file(path)` | Read file |
| `greg.io.write_file(path, content)` | Write file |
| `greg.io.file_exists(path)` | Check if file exists |
| `greg.io.list_files(pattern?)` | List files |

### Coroutines / Timers

| Function | Description |
|----------|-------------|
| `greg.start_coroutine(fn)` | Start coroutine |
| `greg.wait(seconds)` | Wait in coroutine |
| `greg.every(seconds, fn)` | Repeating timer |

---

## 5. Hook System (1850+ Hooks)

gregCore automatically patches all methods from `game_hooks.json`. You can listen to them:

```lua
greg.on("greg.Audio.AudioManager.SetEffectsVolume", function(payload)
    greg.ui.log_info("Volume changed!")
end)

-- Show all available hooks
greg.hooks.groups()          -- Group list
greg.hooks.audio.list()      -- Hooks in group "Audio"
```

---

## 6. Other Languages

| Language | File Extension | Status | Note |
|---------|------------|--------|---------|
| **Lua** | `.lua` | Production Ready | Full API |
| **JavaScript** | `.js` | Beta | Jint-Runtime, basic |
| **Python** | `.py` | Beta | Python.NET, basic |
| **Rust** | `.rs` | Alpha | FFI-Bridge |
| **C# Scripts** | `.cs` | Alpha | Roslyn-Runtime (unverified) |

---

## 7. Troubleshooting

### "No hooks found" / Events not working
- Check that `game_hooks.json` is in the same directory as `gregCore.dll`.
- Check the MelonLoader logs for `[DynamicPatcher]` messages.

### "Lua mod not loading"
- Folder must be under `UserData/gregCore/Mods/Lua/`.
- `main.lua` must exist.
- `mod.json` is optional but recommended.

### "Dependency missing" for JS/Python
- Make sure the corresponding NuGet packages (Jint, pythonnet) are included in gregCore.
- For self-compiled builds: check `gregCore.csproj` PackageReferences.

### `dotnet test` fails with missing .NET 6 runtime
- Set `DOTNET_ROLL_FORWARD=Major dotnet test` (see section 2).

---

## 8. Further Links

- `examples/Lua/` – minimal examples
- `templates/csharp/` / `templates/lua/` – starter templates
- `docs/FrameworkAPI.md` – complete hook reference (auto-generated)
- `docs/INDEX.md` – documentation index

---

**Happy Modding!** – TeamGreg
