---
id: extending-gregcore
title: EXTENDING gregCore
slug: /reference/extending-gregcore
description: Framework extension tutorial for adding hooks, payload parsers, plugins, and Harmony patches in gregCore.
---

## 1. Introduction

Framework extensions are changes inside `gregCore` that expand the SDK/runtime capabilities for all mods and plugins.

Why extend instead of fork:

- Lower maintenance overhead across releases
- Better version compatibility for plugin authors
- Clear architecture boundaries (`Sdk`, `ModLoader`, `harmony`, `plugins`)

Prerequisites:

- Visual Studio 2022 (or `dotnet` CLI)
- .NET 6 SDK
- MelonLoader reference assemblies in `gregCore/lib/references/`

---

## 2. Framework structure overview

```text
gregCore/
├─ framework/
│  ├─ Sdk/
│  │  ├─ GregNativeEventHooks.cs
│  │  ├─ GregEventDispatcher.cs
│  │  └─ GregPayload.cs
│  ├─ ModLoader/
│  ├─ harmony/
│  ├─ Main.cs
│  └─ gregCore.csproj
├─ plugins/
│  └─ greg.Plugin.<Feature>/
├─ Templates/
│  ├─ greg.PluginTemplate/
│  └─ greg.BasedModTemplate/
└─ gregCore.sln
```

Extension ownership:

- `framework/Sdk/`: public API surfaces (`gregFramework.Core` namespace)
- `framework/ModLoader/`: runtime loading and integration plumbing
- `framework/harmony/`: game interception via Harmony patches
- `plugins/`: feature modules using SDK APIs

---

## 3. Extension #1: Add a new hook constant

Target file:

- `framework/Sdk/GregNativeEventHooks.cs`

### Step-by-step

1. Add a new canonical hook string constant.
2. Map event ID to hook in `ByEventId` lookup.
3. Rebuild and test subscription from a plugin/mod.

### Before

```csharp
namespace gregFramework.Core;

public static class GregNativeEventHooks
{
    public const string NetworkCreateNewCable = "greg.NETWORK.CreateNewCable";
}
```

### After

```csharp
namespace gregFramework.Core;

public static class GregNativeEventHooks
{
    public const string NetworkCreateNewCable = "greg.NETWORK.CreateNewCable";
    public const string CableColorChanged = "greg.NETWORK.ColorChanged";

    private static readonly Dictionary<uint, string> ByEventId = new()
    {
        { (uint)EventIds.CableCreated, NetworkCreateNewCable },
        { (uint)EventIds.NewEventId, CableColorChanged }
    };
}
```

### Subscription test

```csharp
using gregFramework.Core;

GregEventDispatcher.On(
    GregNativeEventHooks.CableColorChanged,
    payload => {
        var dump = GregPayload.Dump(payload);
        MelonLoader.MelonLogger.Msg($"CableColorChanged => {dump}");
    },
    "CableInspector"
);
```

---

## 4. Extension #2: Add a new payload parser

Target file:

- `framework/Sdk/GregPayload.cs`

Goal: add an explicit parser helper for color payload fields.

### Before

```csharp
namespace gregFramework.Core;

public static class GregPayload
{
    public static T Get<T>(object payload, string fieldName, T fallback)
    {
        // existing implementation
        return fallback;
    }
}
```

### After

```csharp
using UnityEngine;

namespace gregFramework.Core;

public static class GregPayload
{
    public static T Get<T>(object payload, string fieldName, T fallback)
    {
        // existing implementation
        return fallback;
    }

    public static Color GetColor(object payload, string fieldName, Color fallback)
    {
        var raw = Get(payload, fieldName, string.Empty);
        if (string.IsNullOrWhiteSpace(raw))
        {
            return fallback;
        }

        return ColorUtility.TryParseHtmlString(raw, out var parsed)
            ? parsed
            : fallback;
    }
}
```

### Integration usage

```csharp
var cableColor = GregPayload.GetColor(payload, "CableColor", Color.white);
```

---

## 5. Extension #3: Create a new plugin project

Target structure:

```text
plugins/greg.Plugin.CableInspector/
├─ greg.Plugin.CableInspector.csproj
├─ Main.cs
└─ CableInspector.cs
```

Use `Templates/greg.PluginTemplate/` as baseline.

### `greg.Plugin.CableInspector.csproj`

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net6.0</TargetFramework>
    <LangVersion>latest</LangVersion>
  </PropertyGroup>

  <ItemGroup>
    <ProjectReference Include="..\..\framework\gregCore.csproj" />
  </ItemGroup>
</Project>
```

### `Main.cs`

```csharp
using MelonLoader;
using gregFramework.Core;

[assembly: MelonInfo(typeof(greg.Plugin.CableInspector.Main), "CableInspector", "00.01.0001", "teamGreg")]
[assembly: MelonGame("Waseku", "Data Center")]

namespace greg.Plugin.CableInspector;

public sealed class Main : MelonMod
{
    public override void OnInitializeMelon()
    {
        GregEventDispatcher.On(
            GregNativeEventHooks.NetworkCreateNewCable,
            payload => MelonLogger.Msg(GregPayload.Dump(payload)),
            "CableInspector"
        );
    }

    public override void OnApplicationQuit()
    {
        GregEventDispatcher.UnregisterAll("CableInspector");
    }
}
```

---

## 6. Extension #4: Add a Harmony patch

Target file:

- `framework/harmony/NewPatches.cs`

```csharp
using HarmonyLib;
using gregFramework.Core;

namespace gregFramework.Harmony;

public static class NewPatches
{
    public static void Apply(Harmony harmony)
    {
        var targetType = AccessTools.TypeByName("CableManager");
        var targetMethod = AccessTools.Method(targetType, "SetColor");
        var postfix = AccessTools.Method(typeof(NewPatches), nameof(SetColorPostfix));

        if (targetMethod != null && postfix != null)
        {
            harmony.Patch(targetMethod, postfix: new HarmonyMethod(postfix));
        }
    }

    private static void SetColorPostfix(object __instance)
    {
        GregEventDispatcher.Emit(GregNativeEventHooks.CableColorChanged, new
        {
            Source = __instance?.GetType().Name ?? "unknown"
        });
    }
}
```

---

## 7. Build and deploy

```bash
dotnet restore gregCore.sln
dotnet build gregCore.sln --configuration Release
```

Copy outputs:

- `framework/bin/Release/net6.0/gregCore.dll` -> `Data Center/Mods/`
- `plugins/*/bin/Release/net6.0/*.dll` -> `Data Center/Mods/`

Validation:

- Launch game
- Inspect `MelonLoader/Latest.log`
- Confirm hook registrations and no startup exceptions

---

## 8. Best practices

- Use semantic versioning for newly introduced hooks.
- Keep backward compatibility via `GregCompatBridge` mappings.
- Document every new hook constant with clear event origin.
- Extend tests in `framework/gregCore.Tests/` for parser and dispatcher changes.
- Prefer additive API changes over signature-breaking edits.

---

## 9. Common issues and troubleshooting

- **`Core.Instance is null`**: framework bootstrap not ready; defer logic to next update tick.
- **`EventId not mapped`**: update `GregNativeEventHooks` event ID mapping table.
- **Build failures on references**: refresh MelonLoader references under `lib/references/` (for example via `tools/refresh_refs.py` in the gregCore repository).
- **No event callbacks**: verify `GregEventDispatcher.On(...)` uses canonical `greg.*` hook names.

---

## MISSING.md rule (required for absent abstractions)

If an extension depends on a non-existing abstraction (example: `IModExtension`), create `MISSING.md` in the affected folder.

Required content:

1. Missing API signature
2. Intended file/folder for implementation
3. Runtime compatibility impact (`ModLoader`, `plugins`, `runtime`)
4. greg-style pseudo code

Example:

```markdown
# MISSING API: IModExtension

- Signature:
  - `public interface IModExtension { void Initialize(); void Shutdown(); }`
- Target location:
  - `framework/ModLoader/IModExtension.cs`
- Impact:
  - Plugin load lifecycle and dependency registration
- Pseudo code:
  - `ModLoader.RegisterExtension(IModExtension extension)`
```

This rule prevents undocumented ad-hoc APIs and keeps extensions compatible with the core architecture.
