---
id: modding-melonloader-gregcore
title: MelonLoader + gregCore Modding Tutorial
slug: /guides/modding-melonloader-gregcore
description: Beginner tutorial for building a first Data Center mod with MelonLoader and gregCore.
---

## 1. Prerequisites (5 min)

Before you start, make sure the full toolchain is ready:

- ✅ Visual Studio 2022 Community
- ✅ .NET 6 SDK
- ✅ Data Center (Steam)
- ✅ MelonLoader installer
- ✅ `gregCore.dll` (Release artifact or local build)

Recommended quick checks:

```powershell
dotnet --version
```

```powershell
Test-Path "C:\Program Files (x86)\Steam\steamapps\common\Data Center"
```

---

## 2. MelonLoader installation (5 min)

1. Run `MelonLoader-Installer.exe`.
2. Select the `Data Center` game folder.
3. Complete installation.
4. Copy `gregCore.dll` to `Data Center/Mods/`.
5. Start game once and verify log output.

[image:1]

Expected verification file:

- `Data Center/MelonLoader/Latest.log`

---

## 3. First standalone mod (10 min)

Create a beginner template folder in your repo/workspace:

```text
Templates/
└── MyFirstDataCenterMod/
    ├── MyFirstDataCenterMod.csproj
    ├── Main.cs
    └── README.md
```

### 3.1 `MyFirstDataCenterMod.csproj`

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net6.0</TargetFramework>
    <LangVersion>latest</LangVersion>
    <Nullable>enable</Nullable>
  </PropertyGroup>

  <ItemGroup>
    <Reference Include="MelonLoader">
      <HintPath>..\..\lib\references\MelonLoader\MelonLoader.dll</HintPath>
    </Reference>
    <Reference Include="UnityEngine.CoreModule">
      <HintPath>..\..\lib\references\MelonLoader\net6\UnityEngine.CoreModule.dll</HintPath>
    </Reference>
    <Reference Include="UnityEngine.IMGUIModule">
      <HintPath>..\..\lib\references\MelonLoader\net6\UnityEngine.IMGUIModule.dll</HintPath>
    </Reference>
  </ItemGroup>
</Project>
```

### 3.2 `Main.cs` (standalone MelonLoader)

```csharp
using MelonLoader;
using UnityEngine;

[assembly: MelonInfo(typeof(MyFirstDataCenterMod.Main), "MyFirstDataCenterMod", "1.0.0", "teamGreg")]
[assembly: MelonGame("Waseku", "Data Center")]

namespace MyFirstDataCenterMod;

public sealed class Main : MelonMod
{
    private float _fps;
    private float _accumulator;
    private int _frames;

    public override void OnInitializeMelon()
    {
        LoggerInstance.Msg("MyFirstDataCenterMod loaded");
    }

    public override void OnUpdate()
    {
        _accumulator += Time.unscaledDeltaTime;
        _frames += 1;

        if (_accumulator >= 0.5f)
        {
            _fps = _frames / _accumulator;
            _accumulator = 0f;
            _frames = 0;
        }
    }

    public override void OnGUI()
    {
        GUI.Box(new Rect(12f, 12f, 240f, 60f), "MyFirstDataCenterMod");
        GUI.Label(new Rect(24f, 38f, 220f, 20f), $"Hello World | FPS: {_fps:0.0}");
    }
}
```

### 3.3 `README.md`

```markdown
# MyFirstDataCenterMod

Standalone MelonLoader beginner mod for Data Center.
```

---

## 4. gregCore integration (10 min)

Now extend the same mod with gregCore APIs.

### 4.1 Update project references

Add `gregCore.dll` reference to `.csproj`:

```xml
<ItemGroup>
  <Reference Include="gregCore">
    <HintPath>..\..\framework\bin\Release\net6.0\gregCore.dll</HintPath>
  </Reference>
</ItemGroup>
```

### 4.2 Update `Main.cs` to gregCore-aware version

```csharp
using MelonLoader;
using UnityEngine;
using gregFramework.Core;

[assembly: MelonInfo(typeof(MyFirstDataCenterMod.Main), "MyFirstDataCenterMod", "1.0.0", "teamGreg")]
[assembly: MelonGame("Waseku", "Data Center")]

namespace MyFirstDataCenterMod;

public sealed class Main : MelonMod
{
    private const string ModId = "MyFirstDataCenterMod";

    private bool _frameworkReady;
    private string _lastServerType = "none";

    public override void OnInitializeMelon()
    {
        LoggerInstance.Msg("MyFirstDataCenterMod initializing...");

        GregEventDispatcher.On(
            GregNativeEventHooks.ServerInsertedInRack,
            OnServerInserted,
            ModId
        );

        GregEventDispatcher.On(
            GregNativeEventHooks.NetworkCreateNewCable,
            OnCableCreated,
            ModId
        );

        GregEventDispatcher.On(
            GregNativeEventHooks.ServerCustomerChanged,
            OnCustomerChanged,
            ModId
        );
    }

    public override void OnUpdate()
    {
        if (Core.Instance == null)
        {
            _frameworkReady = false;
            return;
        }

        if (!_frameworkReady)
        {
            _frameworkReady = true;
            LoggerInstance.Msg("gregCore ready");
        }

        Core.Instance.Publish(new ModTickEvent(System.DateTime.UtcNow));
    }

    public override void OnGUI()
    {
        GUI.Box(new Rect(12f, 12f, 360f, 70f), "MyFirstDataCenterMod (gregCore)");
        GUI.Label(new Rect(24f, 38f, 330f, 20f), $"Framework: {(_frameworkReady ? "ready" : "waiting")}");
        GUI.Label(new Rect(24f, 56f, 330f, 20f), $"Last ServerType: {_lastServerType}");
    }

    public override void OnApplicationQuit()
    {
        GregEventDispatcher.UnregisterAll(ModId);
    }

    private void OnServerInserted(object payload)
    {
        _lastServerType = GregPayload.Get(payload, "ServerType", "Unbekannt");
        LoggerInstance.Msg($"ServerInsertedInRack -> {_lastServerType}");
    }

    private void OnCableCreated(object payload)
    {
        LoggerInstance.Msg($"CreateNewCable -> {GregPayload.Dump(payload)}");
    }

    private void OnCustomerChanged(object payload)
    {
        var customer = GregPayload.Get(payload, "CustomerId", "N/A");
        LoggerInstance.Msg($"ServerCustomerChanged -> {customer}");
    }
}
```

---

## 5. Build & deploy (5 min)

Build:

```powershell
dotnet build .\Templates\MyFirstDataCenterMod\MyFirstDataCenterMod.csproj -c Release
```

Deploy DLL to game:

```powershell
Copy-Item .\Templates\MyFirstDataCenterMod\bin\Release\net6.0\MyFirstDataCenterMod.dll "C:\Program Files (x86)\Steam\steamapps\common\Data Center\Mods\" -Force
```

Run game and verify in log:

- `MyFirstDataCenterMod loaded`

---

## 6. Debugging & logs (5 min)

Primary diagnostics:

- `MelonLoader/Latest.log`
- `LoggerInstance.Msg(...)`
- `LoggerInstance.Warning(...)`
- `LoggerInstance.Error(...)`

[image:2]

Quick checks:

- Mod appears in startup list
- No missing assembly errors
- Hook callbacks trigger when gameplay events occur

---

## 7. Next steps (advanced)

After first success, continue with:

- 🔥 Custom IMGUI HUD in `OnGUI()`
- 🔥 `Physics.Raycast()` targeting overlays
- 🔥 Harmony patches for game methods
- 🔥 gregCore plugin architecture (`Templates/greg.PluginTemplate`)
- 🔥 Workshop packaging and release workflow

---

## MISSING.md rule (with .gitignore header)

If a required API abstraction is missing during extension work, create `MISSING.md` in that module folder.

Template:

```markdown
# .gitignore
!MISSING.md

# Missing API report

## Missing signature
- `public interface INewExtensionPoint { void Register(); }`

## Expected location
- `framework/ModLoader/INewExtensionPoint.cs`

## Compatibility impact
- Affects plugin registration and runtime initialization order.

## greg-convention pseudo code
- `ModLoader.RegisterExtension(INewExtensionPoint extension)`
```

This keeps extension work traceable and avoids undocumented ad-hoc APIs.
