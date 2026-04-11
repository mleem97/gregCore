---
title: greg.NETWORK.ComponentInitialized
sidebar_label: greg.NETWORK.ComponentInitialized
description: "gregCore Hook — Auto-generated from Il2Cpp unpack: CableLink.Start"
---

# `greg.NETWORK.ComponentInitialized`

## Description

- Auto-generated from Il2Cpp unpack: CableLink.Start
- Auto-generated from Il2Cpp unpack: CablePositions.Awake
- Auto-generated from Il2Cpp unpack: CablePositions.Start
- Auto-generated from Il2Cpp unpack: NetworkMap.Awake
- Auto-generated from Il2Cpp unpack: NetworkSwitch.Awake
- Auto-generated from Il2Cpp unpack: NetworkSwitch.Start
- Auto-generated from Il2Cpp unpack: SFPBox.Awake
- Auto-generated from Il2Cpp unpack: SFPModule.Awake

## Patch Targets (Il2Cpp)

This hook name can be mapped to **8** Harmony target(s):

| Patch Target | Strategy | Description |
|------------|-----------|--------------|
| `Il2Cpp.CableLink::Start()` | Postfix | Auto-generated from Il2Cpp unpack: CableLink.Start |
| `Il2Cpp.CablePositions::Awake()` | Postfix | Auto-generated from Il2Cpp unpack: CablePositions.Awake |
| `Il2Cpp.CablePositions::Start()` | Postfix | Auto-generated from Il2Cpp unpack: CablePositions.Start |
| `Il2Cpp.NetworkMap::Awake()` | Postfix | Auto-generated from Il2Cpp unpack: NetworkMap.Awake |
| `Il2Cpp.NetworkSwitch::Awake()` | Postfix | Auto-generated from Il2Cpp unpack: NetworkSwitch.Awake |
| `Il2Cpp.NetworkSwitch::Start()` | Postfix | Auto-generated from Il2Cpp unpack: NetworkSwitch.Start |
| `Il2Cpp.SFPBox::Awake()` | Postfix | Auto-generated from Il2Cpp unpack: SFPBox.Awake |
| `Il2Cpp.SFPModule::Awake()` | Postfix | Auto-generated from Il2Cpp unpack: SFPModule.Awake |

**Strategy:** Postfix — The catalog says **Postfix** → typically use `HookBinder.OnAfter(...)`.

## Payload Schema (from Registry)

| Field | Type / Note |
|------|----------------|
| `method` | `string` |

## How to use this hook

### 1. Harmony Pipeline: `HookBinder` (Main path for `greg_hooks.json`)

The framework code patches Il2Cpp methods and calls `HookBinder.DispatchBefore` / `DispatchAfter`. You register handlers with the **exact** string `greg.NETWORK.ComponentInitialized`:

```csharp
using GregFramework.Hooks;

// For Postfix strategy (common):
HookBinder.OnAfter("greg.NETWORK.ComponentInitialized", ctx =>
{
    // ctx.HookName, ctx.Method, ctx.Instance, ctx.Arguments, ctx.ReturnValue
});
```

Prerequisites:

- A hook **catalog** / alias file has been loaded (e.g., `HookBinder.LoadAliases(path)` or `Hooker.InstallFromCatalog(...)`), so the Il2Cpp signature is mapped to the canonical string `greg.NETWORK.ComponentInitialized`.
- Namespace `GregFramework.Hooks`, type `HookContext` (includes `Arguments`, `Instance`, `ReturnValue`, etc.).

### 2. Object Bus: `GregEventDispatcher` (Rust / FFI / manual emits)

Some names in the form ``greg.<Domain>.<Action>`` are additionally fired from the core via `GregHookIntegration` / `GregEventDispatcher.Emit` (numeric `EventIds` → String). If your hook is only defined in `greg_hooks.json` as an Il2Cpp patch, **HookBinder** is the correct entry point; use `GregEventDispatcher` if you want to explicitly listen to the payload bus:

```csharp
using gregFramework.Core;

GregEventDispatcher.On("greg.NETWORK.ComponentInitialized", payload =>
{
    // depending on the payload type; helpers: GregPayload.Get<T>(payload, "field", fallback)
});
```

Unregister: `GregEventDispatcher.Off("greg.NETWORK.ComponentInitialized", handler)` (same delegate reference as for `On`).

## See also

- [Overview of all greg hooks](/wiki/_internal/README)
- Source: `gregCore/gregFramework/greg_hooks.json`
## Usage in Rust

If your Rust bridge exposes the event bus, subscribe to the exact hook name:

~~~rust
use greg_framework::events::EventBus;

fn register_hooks(bus: &mut EventBus) {
    bus.on("greg.NETWORK.ComponentInitialized", |payload| {
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
greg.on("greg.NETWORK.ComponentInitialized", function(payload)
  local method = payload and payload.method or "unknown"
  greg.log("[NETWORK] method=" .. tostring(method))
end)
~~~

For Harmony-level interception use:

~~~lua
greg.hook.after("greg.NETWORK.ComponentInitialized", function(ctx)
  greg.log("after hook: " .. tostring(ctx.method_name))
end)
~~~

## Usage in TypeScript

In a TS/JS bridge, subscribe via your event client using the same hook key:

~~~ts
import { gregBus } from './gregBus';

gregBus.on("greg.NETWORK.ComponentInitialized", (payload) => {
  const method = payload?.method ?? 'unknown';
  console.log("[NETWORK] method=" + method);
});
~~~

When using RPC/WebSocket transport, keep `greg.NETWORK.ComponentInitialized` as the canonical routing key end-to-end.

