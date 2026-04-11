---
id: hexviewer-csharp
title: HexViewer Mod Tutorial (C#)
sidebar_label: HexViewer in C#
slug: /development/tutorials/hexviewer-csharp
description: Technical writing tutorial for the HexViewer C# logic layer in gregCore, including structure, build pipeline, and in-game validation.
---

HexViewer is a hybrid gregCore mod that renders a JADE-like top-center HUD with target information for racks, cables, and cable spinners.

This page implements the **C# Core Logic layer** in the architecture:

- **C# (Core Logic):** raycast + entity identity resolution
- **Rust (Data Layer):** fast metadata query (color, owner, type)
- **TypeScript (UI Layer):** HUD rendering (`Anchor.TopCenter`)
- **Lua (Config Layer):** filter profile and thresholds

> Layer ownership: this page belongs to **GregFramework Core SDK layer** and communicates with bridges/mod layers through canonical `greg.*` events.

## Introduction

HexViewer continuously resolves the currently targeted object and emits a normalized payload event:

- Event name: `greg.HEXVIEWER.TargetUpdated`
- Event payload: `{ entityId, entityType, colorHex, ownerName, distanceMeters }`

UI and scripting layers must never call Unity internals directly; they subscribe to this event.

## Project structure (hybrid mod)

Use one repository folder for the mod package, with explicit language boundaries:

```text
gregMod.HexViewer/
├─ csharp/
│  ├─ gregMod.HexViewer.csproj
│  └─ HexViewerMod.cs
├─ rust/
│  ├─ Cargo.toml
│  └─ src/lib.rs
├─ ts/
│  ├─ package.json
│  ├─ tsconfig.json
│  └─ src/hud.ts
├─ lua/
│  └─ hexviewer_config.lua
├─ dist/
│  ├─ gregMod.HexViewer.dll
│  ├─ hexviewer_rust.dll
│  └─ hexviewer_hud.js
└─ manifest/
   └─ hexviewer.mod.json
```

## Build pipeline in gregCore ecosystem

1. Build C# logic against `gregCore.dll`.
2. Build Rust `cdylib` for metadata adapter.
3. Build TypeScript HUD bundle.
4. Copy artifacts to game mod folders.
5. Start game, verify runtime registration and event flow.

Recommended command sequence:

```powershell
dotnet build .\csharp\gregMod.HexViewer.csproj -c Release
cargo build --manifest-path .\rust\Cargo.toml --release
pnpm --dir .\ts install
pnpm --dir .\ts run build
```

## C# implementation (logic layer)

Create `csharp/gregMod.HexViewer.csproj`:

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net6.0</TargetFramework>
    <Nullable>enable</Nullable>
    <LangVersion>latest</LangVersion>
  </PropertyGroup>

  <ItemGroup>
    <Reference Include="gregCore">
      <HintPath>..\..\gregCore\framework\bin\Release\net6.0\gregCore.dll</HintPath>
    </Reference>
    <Reference Include="MelonLoader">
      <HintPath>..\..\gregCore\lib\references\MelonLoader\MelonLoader.dll</HintPath>
    </Reference>
  </ItemGroup>
</Project>
```

Create `csharp/HexViewerMod.cs`:

```csharp
using System;
using MelonLoader;
using GregFramework.Hooks;
using gregFramework.Core;

namespace GregMod.HexViewer;

public sealed class HexViewerMod : MelonMod
{
    private const string HudEventName = "greg.HEXVIEWER.TargetUpdated";

    public override void OnInitializeMelon()
    {
        MelonLogger.Msg("[HexViewer] Init");

        HookBinder.OnAfter("greg.PLAYER.InteractOnClick", OnTargetProbe);
        HookBinder.OnAfter("greg.UI.OnHoverOver", OnTargetProbe);
    }

    private static void OnTargetProbe(HookContext ctx)
    {
        try
        {
            // gregCore-specific: physics target acquisition (placeholder if absent in current branch)
            var hit = GregSystems.Physics.TryRaycastFromCrosshair();
            if (!hit.HasValue)
            {
                return;
            }

            // gregCore-specific: entity id extraction via gregCore EntityManager
            var entityId = GregSystems.EntityManager.GetEntityId(hit.Value.GameObject);
            if (entityId == 0)
            {
                return;
            }

            var entityType = GregSystems.EntityManager.GetEntityType(entityId);
            var distanceMeters = hit.Value.DistanceMeters;

            var payload = new
            {
                entityId,
                entityType,
                distanceMeters,
                colorHex = "#000000",
                ownerName = "unknown"
            };

            // gregCore-specific: central event bus emission to bridges (TS/Lua/Rust consumers)
            GregEventDispatcher.Emit(HudEventName, payload);
        }
        catch (Exception ex)
        {
            MelonLogger.Warning($"[HexViewer] Probe failed: {ex.Message}");
        }
    }

    public override void OnDeinitializeMelon()
    {
        HookBinder.Unregister("greg.PLAYER.InteractOnClick");
        HookBinder.Unregister("greg.UI.OnHoverOver");
    }
}
```

### Placeholder policy (mandatory)

If `GregSystems.Physics` or `GregSystems.EntityManager` are not present in your current gregCore branch, do **not** replace them with direct Unity APIs. Instead add gregCore-compatible adapters:

- `gregFramework.Core.Systems.GregPhysics`
- `gregFramework.Core.Systems.GregEntityManager`

## Integration contract

C# publishes the canonical payload; other language layers consume it:

```text
C# target probe -> greg.HEXVIEWER.TargetUpdated -> Rust enrich -> TS render -> Lua filter profile
```

See companion pages:

- `HexViewer in Rust`
- `HexViewer in TypeScript`
- `HexViewer in LUA`

## Deploy and test in game

Copy C# output:

```powershell
Copy-Item .\csharp\bin\Release\net6.0\gregMod.HexViewer.dll "C:\Program Files (x86)\Steam\steamapps\common\Data Center\Mods\" -Force
```

Validation checklist:

1. Game starts without loader exceptions.
2. `MelonLoader\Latest.log` contains `[HexViewer] Init`.
3. Hovering rack/cable/cablespinner triggers `greg.HEXVIEWER.TargetUpdated` events.
4. TS HUD updates top-center line accordingly.

## Troubleshooting

- No events: confirm hook names in your `greg_hooks.json`.
- No entity id: ensure object class is mapped in gregCore entity registry.
- No HUD update: verify TS listener subscribes to exact event name.

## Build quality gates

- Keep logic non-blocking and exception-safe.
- Use `HookBinder` and `GregEventDispatcher` only (no direct Unity API in mods).
- Ensure unload unregisters all hooks.

- Data Center + MelonLoader installed
- `gregCore.dll` working in game `Mods/`
- .NET SDK 6.x installed
- Existing workspace with `gregCore/` checkout

## 2) Create project

Create `gregMod.HexViewer/gregMod.HexViewer.csproj`:

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net6.0</TargetFramework>
    <LangVersion>latest</LangVersion>
    <Nullable>enable</Nullable>
  </PropertyGroup>

  <ItemGroup>
    <Reference Include="gregCore">
      <HintPath>..\gregCore\framework\bin\Release\net6.0\gregCore.dll</HintPath>
    </Reference>
    <Reference Include="MelonLoader">
      <HintPath>..\gregCore\lib\references\MelonLoader\MelonLoader.dll</HintPath>
    </Reference>
  </ItemGroup>
</Project>
```

## 3) Implement the mod

Create `HexViewerMod.cs`:

```csharp
using System;
using MelonLoader;
using GregFramework.Hooks;

namespace GregMod.HexViewer;

public sealed class HexViewerMod : MelonMod
{
    private static readonly string[] HookCandidates =
    {
        "greg.UI.OnHoverOver",
        "greg.SYSTEM.OnHoverOver",
        "greg.PLAYER.InteractOnClick"
    };

    public override void OnInitializeMelon()
    {
        MelonLogger.Msg("[HexViewer] Initializing...");

        foreach (var hook in HookCandidates)
        {
            HookBinder.OnAfter(hook, OnAfterHook);
        }

        MelonLogger.Msg("[HexViewer] Hooks registered.");
    }

    private static void OnAfterHook(HookContext ctx)
    {
        try
        {
            var instanceName = ctx.Instance?.GetType().Name ?? "<null>";
            var instanceHash = ctx.Instance?.GetHashCode() ?? 0;
            var hexId = $"0x{instanceHash:X8}";
            MelonLogger.Msg($"[HexViewer] Hook={ctx.HookName} Type={instanceName} Hex={hexId}");
        }
        catch (Exception ex)
        {
            MelonLogger.Warning($"[HexViewer] Hook processing failed: {ex.Message}");
        }
    }

    public override void OnDeinitializeMelon()
    {
        foreach (var hook in HookCandidates)
        {
            HookBinder.Unregister(hook);
        }

        MelonLogger.Msg("[HexViewer] Hooks unregistered.");
    }
}
```

## 4) Build

```powershell
dotnet build .\gregMod.HexViewer\gregMod.HexViewer.csproj -c Release
```

Expected output DLL:

- `gregMod.HexViewer\bin\Release\net6.0\gregMod.HexViewer.dll`

## 5) Deploy to game

Copy DLL to Data Center mods folder:

```powershell
Copy-Item .\gregMod.HexViewer\bin\Release\net6.0\gregMod.HexViewer.dll "C:\Program Files (x86)\Steam\steamapps\common\Data Center\Mods\" -Force
```

## 6) Test in game

1. Start game.
2. Open `MelonLoader\Latest.log`.
3. Hover/click rack/server/world objects.
4. Verify entries like:

```text
[HexViewer] Hook=greg.UI.OnHoverOver Type=RackController Hex=0x01AF3B22
```

## 7) Troubleshooting

- No logs: ensure hook alias exists in hook catalog loaded by gregCore.
- Type missing: fallback to `ctx.Method.Name` and inspect `ctx.Arguments`.
- Crashes: keep all hook handlers wrapped in `try/catch`.

## 8) Next improvements

- Render on-screen UI instead of log-only output.
- Add config file to enable/disable selected hook groups.
- Map hex ID to domain labels (Rack/Server/Customer).
