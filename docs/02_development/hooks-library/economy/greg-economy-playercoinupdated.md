---
title: greg.economy.PlayerCoinUpdated
sidebar_label: greg.economy.PlayerCoinUpdated
description: "gregCore Hook — Player Coin Updated"
---

# `greg.economy.PlayerCoinUpdated`

## Description

- Emitted when the player's coin balance changes.

## Payload Schema

| Field | Type / Note |
|------|----------------|
| `type` | `valueChange` |
| `key` | `money` |
| `oldValue` | `float` |
| `newValue` | `float` |

## How to use this hook

### 1. Object Bus: `GregEventDispatcher` (Rust / FFI / Lua / TS)

This hook is fired from the core via the `gregCore` plugin bridge whenever the underlying IL2CPP method (`Player.UpdateCoin`) executes successfully.

```csharp
using gregFramework.Core;

GregEventDispatcher.On("greg.economy.PlayerCoinUpdated", payload =>
{
    var newValue = GregPayload.Get<float>(payload, "newValue", 0f);
    // Logic here
});
```

### 2. Usage in Lua

Use the Lua bridge and register against the canonical hook string:

~~~lua
greg.on("greg.economy.PlayerCoinUpdated", function(payload)
  local newVal = payload and payload.newValue or 0
  greg.log("[ECONOMY] new money=" .. tostring(newVal))
end)
~~~
