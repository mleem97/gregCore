---
id: hexviewer-lua
title: HexViewer Mod Tutorial (Lua)
sidebar_label: HexViewer in LUA
slug: /development/tutorials/hexviewer-lua
description: Technical writing tutorial for the HexViewer Lua config and hook layer in gregCore.
---

This page implements the **Lua Config + Hook layer** for HexViewer.

Role in architecture:

- Register mod profile in gregCore registry
- Define target filters and thresholds
- Consume enriched events and apply display policy

> Layer ownership: Lua runs in **Language Bridge layer** and only uses gregCore runtime APIs.

## Introduction

Lua determines whether a detected object should be shown in HUD output.

Filtering criteria in this tutorial:

- accepted object types: `Rack`, `Cable`, `CableSpinner`
- max distance threshold
- owner blacklist

## Project structure (Lua segment)

```text
gregMod.HexViewer/lua/
├─ init.lua
├─ hexviewer_config.lua
└─ filters.lua
```

Deploy location:

- `Data Center/Mods/ScriptMods/lua/hexviewer/`

## Lua implementation

`lua/hexviewer_config.lua`:

```lua
local config = {
  max_distance_m = 14.0,
  allowed_types = {
    Rack = true,
    Cable = true,
    CableSpinner = true
  },
  owner_blacklist = {
    ["Unknown"] = true
  }
}

return config
```

`lua/filters.lua`:

```lua
local cfg = require("hexviewer.hexviewer_config")

local M = {}

function M.should_display(payload)
  if not payload then return false end

  if not cfg.allowed_types[payload.entityType] then
    return false
  end

  if (payload.distanceMeters or 9999) > cfg.max_distance_m then
    return false
  end

  if cfg.owner_blacklist[payload.ownerName or ""] then
    return false
  end

  return true
end

return M
```

`lua/init.lua`:

```lua
local filters = require("hexviewer.filters")

-- gregCore-specific: mod registration in greg runtime registry
greg.registry.register_mod({
  id = "hexviewer.lua",
  name = "HexViewer Lua Profile",
  version = "1.0.0"
})

-- gregCore-specific: subscribe to enriched cross-language payload event
greg.on("greg.HEXVIEWER.TargetEnriched", function(payload)
  if not filters.should_display(payload) then
    return
  end

  greg.emit("greg.HEXVIEWER.TargetFiltered", payload)
end)

greg.log("[HexViewer/Lua] Filter profile active")
```

## Placeholder policy

If `greg.registry.register_mod` is not available in your current branch, do not replace it with custom global state. Keep a placeholder following greg naming conventions, for example:

- `greg.Registry.RegisterMod`
- `greg.registry.register_mod`

## Integration with C#/Rust/TS

Event chain:

1. C# emits `greg.HEXVIEWER.TargetUpdated`
2. Rust enriches and emits `greg.HEXVIEWER.TargetEnriched`
3. Lua filters and emits `greg.HEXVIEWER.TargetFiltered`
4. TypeScript HUD renders filtered payload

## Deploy and test in game

Copy Lua folder:

```powershell
Copy-Item .\lua\* "C:\Program Files (x86)\Steam\steamapps\common\Data Center\Mods\ScriptMods\lua\hexviewer\" -Recurse -Force
```

Validation checklist:

- Registry log confirms module registration
- Blacklisted owners are hidden
- Distance threshold limits output as configured

## Troubleshooting

- `require` fails: check folder/module naming (`hexviewer/...`).
- No filtering effect: ensure UI consumes `greg.HEXVIEWER.TargetFiltered` event.
- Nil payload fields: verify Rust enrichment event schema.

## 1) Prerequisites

- Data Center + MelonLoader + `gregCore.dll`
- Lua bridge enabled by your gregCore runtime
- Access to `Data Center/Mods/ScriptMods/lua/`

## 2) Create mod folder

Create:

- `Data Center/Mods/ScriptMods/lua/hexviewer/init.lua`

## 3) Implement HexViewer script

```lua
local hooks = {
  "greg.UI.OnHoverOver",
  "greg.SYSTEM.OnHoverOver",
  "greg.PLAYER.InteractOnClick"
}

local function to_hex(num)
  if num == nil then return "0x00000000" end
  return string.format("0x%08X", tonumber(num) or 0)
end

local function on_event(payload)
  local method = payload and payload.method or "unknown"
  local instance_handle = payload and payload.instance_handle or 0
  local hex = to_hex(instance_handle)
  greg.log("[HexViewer/Lua] method=" .. tostring(method) .. " hex=" .. hex)
end

for _, hook in ipairs(hooks) do
  greg.on(hook, on_event)
end

greg.log("[HexViewer/Lua] Registered " .. tostring(#hooks) .. " hooks")
```

## 4) Deploy

No compile step required. Save file under Lua mods folder and start the game.

## 5) Test in game

1. Launch game.
2. Trigger hover/click interactions.
3. Check runtime log output for HexViewer lines.

Expected line:

```text
[HexViewer/Lua] method=OnHoverOver hex=0x00F1A22B
```

## 6) Troubleshooting

- `greg` is nil: Lua bridge not loaded or wrong folder.
- No events: hook names differ in your catalog; test one known hook first.
- Performance issues: throttle logs (e.g. every 100ms).

## 7) Next improvements

- Add on-screen label via UI bridge API.
- Persist latest hovered hex in a Lua table for quick recall.
- Add command toggle (`/hexviewer on|off`) if your command bridge is enabled.
