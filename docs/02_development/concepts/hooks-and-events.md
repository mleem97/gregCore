---
title: Greg hooks & event runtime
sidebar_label: Greg hooks & events
description: greg.* hooks, GregEventDispatcher, GregNativeEventHooks, greg_hooks.json, Rust FFI event ids, and compat aliases.
---

# Greg hooks & event runtime

The **`gregCore.dll`** assembly is built from **`gregCore/framework/gregCore.csproj`** and combines Harmony patches, C# events (`GregEventDispatcher`), and the Rust/native bridge. Mod authors mainly care about **three surfaces**:

| Surface | Role | Typical entry |
|---------|------|----------------|
| **`greg.*` hook strings** | Canonical names for Harmony/mod code; registry in **`greg_hooks.json`**. | `GregEventDispatcher.On("greg....", handler, modId)` in **`gregFramework.Core`** |
| **Native pipeline (`EventIds` → `greg.*`)** | Same gameplay moments as FFI events; centralized in **`GregNativeEventHooks`**. | Constants / `Resolve(uint)` on **`gregFramework.Core.GregNativeEventHooks`** |
| **Legacy aliases** | Old spellings → canonical **`greg.*`**. | **`GregCompatBridge`** (+ optional **`legacy`** entries in **`greg_hooks.json`**) |

New **documentation** identifiers still follow **`FMF.<Domain>.*`** — see [FMF hook naming](/wiki/reference/fmf-hook-naming). **Runtime** strings for the native chain are **`greg.*`** as defined by **`GregNativeEventHooks`** and **`greg_hooks.json`**.

## `greg_hooks.json` (version 2)

| Path | Role |
|------|------|
| **Repo root** `greg_hooks.json` | Source of truth: `name`, `patchTarget`, `strategy`, `payloadSchema`, optional `legacy`. |
| **Next to `gregCore.dll`** | Copied from the monorepo root on build so **`GregCompatBridge`** can resolve legacy names. |

Regenerate: `gregCore/scripts/Generate-GregHooksFromIl2CppDump.ps1` when Il2Cpp/interop inputs change.

## `GregEventDispatcher` / SDK

Implementation: **`gregCore/framework/Sdk/GregEventDispatcher.cs`** (`namespace gregFramework.Core`). API: **`On` / `Once` / `Off` / `Emit`**, **`OnCancelable` / `InvokeCancelable`**, **`UnregisterAll(modId)`**.

Build stable hook strings with **`GregHookName.Create(GregDomain.*, "Action")`** or **`GregNativeEventHooks.*`** constants.

## Native events (`EventIds`)

- **`EventIds` and `EventDispatcher`:** `gregCore/framework/ModLoader/EventDispatcher.cs` (numeric ids aligned with Rust).
- **Mapping → `greg.*`:** **`GregNativeEventHooks`** (`gregCore/framework/Sdk/GregNativeEventHooks.cs`); emission via **`GregHookIntegration`** in the same ModLoader tree.
- **Wiki table:** [greg hooks catalog](/wiki/reference/greg-hooks-catalog) (generator: `gregCore/tools/Generate-GregHookCatalog.ps1`).

## Lua event & hook subscriptions

Lua scripts subscribe to the same event surfaces through the `greg.*` API injected by `GregHooksLuaModule`:

| Lua API | C# backend | Purpose |
|---------|-----------|---------|
| `greg.on(hookName, fn)` | `GregEventDispatcher.On()` | Subscribe to any `greg.*` event — receives payload as Lua table |
| `greg.off(hookName)` | `GregEventDispatcher.Off()` | Unsubscribe all Lua handlers for a hook |
| `greg.emit(hookName, payload)` | `GregEventDispatcher.Emit()` | Emit a custom event (other Lua/C# listeners receive it) |
| `greg.hook.before(hookName, fn)` | `HookBinder.OnBefore()` | Harmony **prefix** — fn receives `{hook_name, type_name, method_name, instance_handle, arg_count}` |
| `greg.hook.after(hookName, fn)` | `HookBinder.OnAfter()` | Harmony **postfix** — same context table |
| `greg.hook.off(hookName)` | `HookBinder.Unregister()` | Remove all handlers for a Harmony hook |

**Example** — react to CableSpinner.Start in Lua:

```lua
greg.hook.after("greg.Misc.OnStart", function(ctx)
    if ctx.type_name and ctx.type_name:find("CableSpinner") then
        greg.log("new spinner spawned, handle: " .. tostring(ctx.instance_handle))
    end
end)
```

**Example** — listen to game events:

```lua
greg.on("greg.PLAYER.CoinChanged", function(payload)
    greg.log("coins changed by " .. tostring(payload.coinChangeAmount))
end)
```

All hook names from `GregNativeEventHooks` (e.g. `greg.PLAYER.CoinChanged`, `greg.SERVER.PowerButton`) and `HookBinder` aliases (e.g. `greg.Misc.OnStart`) are available to Lua.

## Rust FFI

Rust/native mods receive **numeric** event ids; C# mirrors the same moments as **`greg.*`** through **`GregHookIntegration`** while the game runs. The `RustLanguageBridgeAdapter` fully delegates all lifecycle calls (`OnUpdate`, `OnSceneLoaded`, `Shutdown`, event dispatch) to `FFIBridge`. Bridge code: **`FfiBridge.cs`** / **`RustLanguageBridgeAdapter.cs`** under `framework/ModLoader/`.

## MelonLoader entry points (one DLL)

Depending on the build, multiple **`MelonMod`** types may ship in the same assembly (e.g. main framework plugin vs. AssetExporter paths) — check **`MelonInfo`** / **`MelonGame`** in source.

## Tooling

- **MCP:** `greg_hook_registry`, `greg_hook_search`, … with `dataRoot` → **`gregCore/`** — [MCP references](/wiki/developers).
- **[FMF hooks catalog](/wiki/reference/fmf-hook-naming)** is now a **short redirect** to **`GregNativeEventHooks`** / [greg hooks catalog](/wiki/reference/greg-hooks-catalog) (the old **`HookNames.cs`** table is gone).

## See also

- [Repository architecture](/wiki/getting-started/architecture)
- [FFI, hooks & Lua (hub)](/wiki/development/concepts/hooks-and-events)
- [Getting started](/wiki/getting-started/quickstart)

