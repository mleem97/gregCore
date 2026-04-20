---
title: greg.economy.PlayerReputationUpdated
sidebar_label: greg.economy.PlayerReputationUpdated
description: "gregCore Hook — Player Reputation Updated"
---

# `greg.economy.PlayerReputationUpdated`

## Description

- Emitted when the player's reputation changes.

## Payload Schema

| Field | Type / Note |
|------|----------------|
| `type` | `valueChange` |
| `key` | `reputation` |
| `oldValue` | `float` |
| `newValue` | `float` |

## How to use this hook

### 1. Object Bus: `GregEventDispatcher` (Rust / FFI / Lua / TS)

This hook is fired from the core via the `gregCore` plugin bridge whenever the underlying IL2CPP method (`Player.UpdateReputation`) executes successfully.

```csharp
using gregFramework.Core;

GregEventDispatcher.On("greg.economy.PlayerReputationUpdated", payload =>
{
    var newValue = GregPayload.Get<float>(payload, "newValue", 0f);
    // Logic here
});
```

### 2. Usage in Lua

Use the Lua bridge and register against the canonical hook string:

~~~lua
greg.on("greg.economy.PlayerReputationUpdated", function(payload)
  local newVal = payload and payload.newValue or 0
  greg.log("[ECONOMY] new reputation=" .. tostring(newVal))
end)
~~~
