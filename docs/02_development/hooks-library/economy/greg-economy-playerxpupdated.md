---
title: greg.economy.PlayerXpUpdated
sidebar_label: greg.economy.PlayerXpUpdated
description: "gregCore Hook — Player XP Updated"
---

# `greg.economy.PlayerXpUpdated`

## Description

- Emitted when the player's XP changes.

## Payload Schema

| Field | Type / Note |
|------|----------------|
| `type` | `valueChange` |
| `key` | `xp` |
| `oldValue` | `float` |
| `newValue` | `float` |

## How to use this hook

### 1. Object Bus: `GregEventDispatcher` (Rust / FFI / Lua / TS)

This hook is fired from the core via the `gregCore` plugin bridge whenever the underlying IL2CPP method (`Player.UpdateXP`) executes successfully.

```csharp
using gregFramework.Core;

GregEventDispatcher.On("greg.economy.PlayerXpUpdated", payload =>
{
    var newValue = GregPayload.Get<float>(payload, "newValue", 0f);
    // Logic here
});
```

### 2. Usage in Lua

Use the Lua bridge and register against the canonical hook string:

~~~lua
greg.on("greg.economy.PlayerXpUpdated", function(payload)
  local newVal = payload and payload.newValue or 0
  greg.log("[ECONOMY] new xp=" .. tostring(newVal))
end)
~~~
