---
title: greg.NETWORK.GenerateBentSegment
sidebar_label: greg.NETWORK.GenerateBentSegment
description: "gregCore Hook — Auto-generated from Il2Cpp unpack: CablePositions.GenerateBentSegment"
---

# `greg.NETWORK.GenerateBentSegment`

## Description

- Auto-generated from Il2Cpp unpack: CablePositions.GenerateBentSegment

## Patch Targets (Il2Cpp)

This hook name can be mapped to **1** Harmony target(s):

| Patch Target | Strategy | Description |
|------------|-----------|--------------|
| `Il2Cpp.CablePositions::GenerateBentSegment(Vector3, Vector3, Transform, bool)` | Postfix | Auto-generated from Il2Cpp unpack: CablePositions.GenerateBentSegment |

**Strategy:** Postfix — The catalog says **Postfix** → typically use `HookBinder.OnAfter(...)`.

## Payload Schema (from Registry)

| Field | Type / Note |
|------|----------------|
| `method` | `string` |

## How to use this hook

### 1. Harmony Pipeline: `HookBinder` (Main path for `greg_hooks.json`)

The framework code patches Il2Cpp methods and calls `HookBinder.DispatchBefore` / `DispatchAfter`. You register handlers with the **exact** string `greg.NETWORK.GenerateBentSegment`:

```csharp
using GregFramework.Hooks;

// For Postfix strategy (common):
HookBinder.OnAfter("greg.NETWORK.GenerateBentSegment", ctx =>
{
    // ctx.HookName, ctx.Method, ctx.Instance, ctx.Arguments, ctx.ReturnValue
});
```

Prerequisites:

- A hook **catalog** / alias file has been loaded (e.g., `HookBinder.LoadAliases(path)` or `Hooker.InstallFromCatalog(...)`), so the Il2Cpp signature is mapped to the canonical string `greg.NETWORK.GenerateBentSegment`.
- Namespace `GregFramework.Hooks`, type `HookContext` (includes `Arguments`, `Instance`, `ReturnValue`, etc.).

### 2. Object Bus: `GregEventDispatcher` (Rust / FFI / manual emits)

Some names in the form ``greg.<Domain>.<Action>`` are additionally fired from the core via `GregHookIntegration` / `GregEventDispatcher.Emit` (numeric `EventIds` → String). If your hook is only defined in `greg_hooks.json` as an Il2Cpp patch, **HookBinder** is the correct entry point; use `GregEventDispatcher` if you want to explicitly listen to the payload bus:

```csharp
using gregFramework.Core;

GregEventDispatcher.On("greg.NETWORK.GenerateBentSegment", payload =>
{
    // depending on the payload type; helpers: GregPayload.Get<T>(payload, "field", fallback)
});
```

Unregister: `GregEventDispatcher.Off("greg.NETWORK.GenerateBentSegment", handler)` (same delegate reference as for `On`).

## See also

- [Overview of all greg hooks](/wiki/_internal/README)
- Source: `gregCore/gregFramework/greg_hooks.json`
## Usage in Rust

If your Rust bridge exposes the event bus, subscribe to the exact hook name:

~~~rust
use greg_framework::events::EventBus;

fn register_hooks(bus: &mut EventBus) {
    bus.on("greg.NETWORK.GenerateBentSegment", |payload| {
        if let Some(method) = payload.get("method") {
            println!("[NETWORK] method={}", method);
        }
    });
}
~~~

> If your current Rust runtime only exposes numeric EventIds, map those IDs to canonical `greg.*` names in your bridge layer before dispatching to mod code.

## Usage in Lua

Use the Lua bridge and register against the same canonical hook string:

~~~lua
greg.on("greg.NETWORK.GenerateBentSegment", function(payload)
  local method = payload and payload.method or "unknown"
  greg.log("[NETWORK] method=" .. tostring(method))
end)
~~~

For Harmony-level interception use:

~~~lua
greg.hook.after("greg.NETWORK.GenerateBentSegment", function(ctx)
  greg.log("after hook: " .. tostring(ctx.method_name))
end)
~~~

## Usage in TypeScript

In a TS/JS bridge, subscribe via your event client using the same hook key:

~~~ts
import { gregBus } from './gregBus';

gregBus.on("greg.NETWORK.GenerateBentSegment", (payload) => {
  const method = payload?.method ?? 'unknown';
  console.log("[NETWORK] method=" + method);
});
~~~

When using RPC/WebSocket transport, keep `greg.NETWORK.GenerateBentSegment` as the canonical routing key end-to-end.

