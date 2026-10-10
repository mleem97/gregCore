---
title: "Developer Data Storage"
---
# Developer Data Storage

Where each kind of data belongs. (Oxide equivalent: "Data Storage" + "Data Files" + "Configuring Plugins".)

## Decision table

| Data | Lua API | C# API | File | Notes |
|---|---|---|---|---|
| User settings | `greg.config.*` | `GregModSettingsService` / `GregConfigService` | `config.json` / MelonPreferences | Survives updates; shown in Settings Hub |
| Runtime state | `greg.save.*` | `GregModSave`, `GregSaveEngine` | `save.json` / sidecars | Write-through; `save_now()` forces flush |
| Bulk files | `greg.io.*` | — | `<modId>/data/…` | Sandboxed; JSON helpers included |
| Structured data | `greg.json.*` | Newtonsoft.Json | — | `parse` / `stringify` (Lua) |
| World/grid state | — | `GregSaveEngine` (LiteDB), `GregModPack` | `gregSave_<guid>.greg.db`, pack DTOs | Gated writes; array bridges for Il2Cpp |

## Lua APIs (exact)

```lua
-- config (config.json) and save (save.json): same shape
greg.config.get(key) / greg.config.get_or(key, default)
greg.config.set(key, value)            -- write-through
greg.config.delete(key)                -- -> bool
greg.config.has(key)                   -- -> bool
greg.config.keys()                     -- -> table
greg.save.save_now()                   -- -> bool (force flush)

-- files, sandboxed to <modId>/data/
greg.io.read_file(path)                -- alias: read_text
greg.io.write_file(path, content)      -- alias: write_text
greg.io.append_file(path, content)
greg.io.delete_file(path)
greg.io.file_exists(path)              -- -> bool
greg.io.list_files(pattern?)           -- -> table
greg.io.read_json(path)                -- -> table or nil
greg.io.write_json(path, table)        -- -> bool
greg.io.data_dir                       -- read-only string

-- json
greg.json.parse(text)                  -- -> table/string/number/boolean or nil
greg.json.stringify(value)             -- -> string ("" on failure)

-- modules
require("name")                        -- <modDir>/<name>.lua, cached
require("@shared/name")                -- shared folder
```

Sandbox: `CoreModules.Preset_SoftSandbox`; `ResolveSafe` rejects traversal (`..`, `/`, `\`); search patterns are sanitized; migrated legacy `data/data` nesting is handled automatically. Anything outside `<modId>/data/` is rejected by design.

## C# APIs

- `GregConfigService.LoadConfig<T> / SaveConfig<T>` — typed JSON, directories auto-created.
- `GregPersistenceService.Set<T> / Get<T> / Has / Delete` — `%AppData%/gregCore/Saves/<key>.json`, traversal-guarded names.
- `GregModSave.Create / Fill / Read / ReadAll / Upsert / Remove` — vanilla-list items keyed by `modFolderName` (`ItemSave` DTO with position/rotation/float+int arrays).
- `GregModPack` builder + reader — `ShopItem / StaticItem / DllRef / Snapshot` DTOs with Il2Cpp array bridges (`To/FromFloat/Int/BoolArray`).
- `GregSaveEngine` (LiteDB) — `Initialize / SaveAll (gated) / LoadAll / SaveGridState / LoadGridState / GetCollection<T>`; `IsGregSave` validates the header. Only grid/wall state belongs here.
- `GregSaveGuard.RegisterSidecar / RegisterVanillaModuleMap` — hook your world edits into backup → write → sanitize → load (see [Core Save Engine](./core-save-engine/)).

## Rules

1. Never write vanilla save structures directly — go through sidecars/DTOs.
2. Never reuse shop/item IDs or GUIDs (ranges: `100, 1000/2000/3000, 9001+` — see [Developer Shop Items](./developer-shop-items/)).
3. Keep settings (user edits) separate from state (game writes).
4. Validate on load: one bad entry must never abort the whole load (per-entry null-guards, like `GregNetworkIdHealing`).
