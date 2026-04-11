# gregCore Language Bridges (Plugin Layer)

Diese Komponenten leben im **Plugin Layer / Language Bridges** und kapseln Mehrsprachen-Modding hinter einer stabilen Core-Schnittstelle.

## Enthaltene Brücken

- `LuaLanguageBridge` (`.lua`) — **MoonSharp VM**, vollständiger Lifecycle
- `TypeScriptJavaScriptLanguageBridge` (`.ts`, `.js`, `.mjs`, `.cjs`)
- `RustLanguageBridgeAdapter` (delegiert an `FFIBridge` für native Mods)

## Aktueller Stand

- Core-Lifecycle ist angebunden (`Initialize`, `LoadAll`, `OnSceneLoaded`, `OnUpdate`, `OnGui`, `Shutdown`).
- Script-Dateien werden in den jeweiligen Mod-Ordnern entdeckt.
- Fehler sind pro Bridge isoliert (`try/catch` + `CrashLog`), damit ein Bridge-Fehler den Rest nicht stoppt.
- **Lua:** MoonSharp-VM kompiliert jede `.lua`-Datei; injiziert `greg`-Global mit allen API-Modulen via `IGregLuaModule`.
- **Rust:** Voller Lifecycle über `FFIBridge` — `LoadAllMods`, `OnUpdate`, `OnSceneLoaded`, `Shutdown`, `DispatchEvent`.

## Script-Ordner

- Lua: `Mods/ScriptMods/lua/`
- TypeScript/JavaScript: `Mods/ScriptMods/js/`
- Rust/native: `Mods/RustMods/` (über `FFIBridge`)

## Lua-API (`greg.*`)

### Logging & Paths

| Funktion | Beschreibung |
|----------|-------------|
| `greg.log(msg)` | MelonLogger Info |
| `greg.warn(msg)` | MelonLogger Warning |
| `greg.error(msg)` | MelonLogger Error + CrashLog |
| `greg.paths.scripts` | Lua-Skriptordner |
| `greg.paths.userdata` | UserData-Verzeichnis |
| `greg.paths.mods` | Mods-Verzeichnis |
| `greg.paths.game` | Spielinstallation |

### Events & Hooks (`greg.on`, `greg.hook`)

| Funktion | Beschreibung |
|----------|-------------|
| `greg.on(hookName, fn)` | GregEventDispatcher: Event-Callback |
| `greg.off(hookName)` | Alle Lua-Handler für diesen Hook entfernen |
| `greg.emit(hookName, payload)` | Custom Event emittieren |
| `greg.hook.before(hookName, fn)` | Harmony Prefix (HookBinder) |
| `greg.hook.after(hookName, fn)` | Harmony Postfix (HookBinder) |
| `greg.hook.off(hookName)` | Hook-Handler entfernen |

### Unity-Objekte (`greg.unity`)

| Funktion | Beschreibung |
|----------|-------------|
| `greg.unity.find(typeName)` | FindObjectsOfType → Handle-Array |
| `greg.unity.find_child(h, name)` | Transform.Find → Handle |
| `greg.unity.get_component(h, type)` | GetComponent → Handle |
| `greg.unity.add_component(h, type)` | AddComponent → Handle |
| `greg.unity.get_parent_component(h, type)` | GetComponentInParent → Handle |
| `greg.unity.get_children_components(h, type)` | GetComponentsInChildren → Handle-Array |
| `greg.unity.create_gameobject(name, parent)` | Neues GameObject → Handle |
| `greg.unity.instantiate(h, parent)` | Klonen → Handle |
| `greg.unity.destroy(h)` | Objekt zerstören |
| `greg.unity.get_name(h)` / `set_name(h, n)` | Name lesen/setzen |
| `greg.unity.handle_alive(h)` | Handle noch gültig? |
| `greg.unity.release(h)` | Handle freigeben |

### Properties (`greg.unity`)

| Funktion | Beschreibung |
|----------|-------------|
| `greg.unity.get_string(h, member)` | String-Property lesen |
| `greg.unity.get_number(h, member)` | Numerische Property lesen |
| `greg.unity.get_bool(h, member)` | Bool-Property lesen |
| `greg.unity.get_handle(h, member)` | Objekt-Property → Handle |
| `greg.unity.set_string(h, member, val)` | String setzen |
| `greg.unity.set_number(h, member, val)` | Nummer setzen |
| `greg.unity.set_bool(h, member, val)` | Bool setzen |

### Transform (`greg.unity`)

| Funktion | Beschreibung |
|----------|-------------|
| `greg.unity.position(h)` | World-Position → `{x, y, z}` |
| `greg.unity.set_position(h, x, y, z)` | Position setzen |
| `greg.unity.set_local_scale(h, x, y, z)` | Scale setzen |
| `greg.unity.set_rotation(h, x, y, z, w)` | Rotation (Quaternion) |
| `greg.unity.set_parent(h, parent)` | Parent setzen |

### Material & Color

| Funktion | Beschreibung |
|----------|-------------|
| `greg.unity.material_hex(h, prop)` | Material-Farbe → `"#RRGGBB"` |
| `greg.color.to_hex(r, g, b)` | RGB → Hex |
| `greg.color.normalize_hex(raw)` | Hex normalisieren |
| `greg.color.parse(hex)` | Hex → `{r, g, b}` |

### TMPro & TextMesh

| Funktion | Beschreibung |
|----------|-------------|
| `greg.unity.tmpro_set(h, text, size, min, max, auto, wrap, align, color)` | TMPro konfigurieren |
| `greg.unity.tmpro_get_text(h)` | TMPro-Text lesen |
| `greg.unity.tmpro_anchored_pos(h)` | Position → `{x, y}` |
| `greg.unity.tmpro_set_anchored_pos(h, x, y)` | Position setzen |
| `greg.unity.textmesh_set(h, text, size, charSize, color, anchor, align)` | TextMesh konfigurieren |

### Physics

| Funktion | Beschreibung |
|----------|-------------|
| `greg.unity.raycast(ox, oy, oz, dx, dy, dz, dist)` | Raycast → `{hit, handle, point}` |
| `greg.unity.camera_ray()` | Kamera-Ray → `{ox, oy, oz, dx, dy, dz}` |

### File I/O (`greg.io`)

| Funktion | Beschreibung |
|----------|-------------|
| `greg.io.read_file(path)` | Datei lesen → String |
| `greg.io.read_lines(path)` | Datei → Zeilen-Array |
| `greg.io.read_head(path, maxLines)` | Erste N Zeilen (Share-Read) |
| `greg.io.write_file(path, content)` | Datei schreiben |
| `greg.io.file_exists(path)` | Datei vorhanden? |
| `greg.io.dir_exists(path)` | Verzeichnis vorhanden? |
| `greg.io.list_files(path, pattern, recursive)` | Dateien auflisten |
| `greg.io.path_combine(a, b)` | Pfade zusammenfügen |
| `greg.io.path_filename(path)` | Dateiname extrahieren |
| `greg.io.path_ext(path)` | Dateiendung |

### Keyboard Input (`greg.input`)

| Funktion | Beschreibung |
|----------|-------------|
| `greg.input.key_pressed(key)` | Taste gedrückt (diesen Frame) |
| `greg.input.key_down(key)` | Taste gehalten |
| `greg.input.ctrl()` | Ctrl gehalten |
| `greg.input.shift()` | Shift gehalten |
| `greg.input.alt()` | Alt gehalten |

### Config (`greg.config`)

| Funktion | Beschreibung |
|----------|-------------|
| `greg.config.load(path)` | Key=Value Config → Table |
| `greg.config.save(path, table)` | Table → Config-Datei |
| `greg.config.userdata_path()` | UserData-Verzeichnis |

### IMGUI (`greg.gui`)

| Funktion | Beschreibung |
|----------|-------------|
| `greg.gui.box(x, y, w, h, title)` | GUI.Box |
| `greg.gui.label(x, y, w, h, text)` | GUI.Label |
| `greg.gui.button(x, y, w, h, text)` | GUI.Button → bool |
| `greg.gui.toggle(x, y, w, h, val, text)` | GUI.Toggle → bool |
| `greg.gui.screen_width()` / `screen_height()` | Bildschirmgröße |

## Lua-Lifecycle-Konvention

Lua-Scripts definieren optionale globale Funktionen:

```lua
function on_update(dt) end   -- pro Frame, dt = Time.deltaTime
function on_scene(name) end  -- wenn eine Szene geladen wird
function on_gui() end        -- Unity OnGUI für IMGUI
```

## Nächster Ausbau

- JS Runtime Host anbinden (z. B. Jint/Node sidecar)
- TypeScript-Transpile-Pipeline (`.ts` -> `.js`) mit Hot-Reload
