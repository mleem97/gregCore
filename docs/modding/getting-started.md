# Writing Mods with Greg — Getting Started

> Status: gregCore `1.2.3` · Game: Data Center (Waseku) · MelonLoader `0.7.x` · Unity `6000.4.x` · Target: `net6.0`
>
> This guide describes the **verified** path (as used by the mod repos).
> Next: [Greg Contract](greg-contract.md) · [UI Panels](ui-panels.md) ·
> [Harmony + IL2CPP](harmony-il2cpp.md) · [Shop Items](shop-items.md) · [Events](api/events.md) · [FAQ](faq.md)

## 1. Prerequisites

- .NET SDK (net6.0-capable), MelonLoader installed in the game.
- Data Center installation, e.g. `~/.local/share/Steam/steamapps/common/Data Center`.
- One mod folder per team convention: `gregMod.<Name>/` with `.csproj`, `src/`, `references/`.
- After a fresh clone **always run first**: `ModRepositories/tools/sync-melon-assemblies.sh`
  (symlinks `references/` to `MelonLoader/net6/*.dll` +
  `MelonLoader/Il2CppAssemblies/*.dll` + `gregCore.dll`). **Never commit DLLs.**

## 2. Minimal project

`gregMod.Hello/gregMod.Hello.csproj` (template used by the team mods):

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net6.0</TargetFramework>
    <Nullable>disable</Nullable>
    <AllowUnsafeBlocks>true</AllowUnsafeBlocks>
    <AssemblyName>gregMod.Hello</AssemblyName>
    <RootNamespace>GregMod.Hello</RootNamespace>
  </PropertyGroup>
  <ItemGroup>
    <Reference Include="MelonLoader"><HintPath>references/MelonLoader.dll</HintPath><Private>false</Private></Reference>
    <Reference Include="Il2CppInterop.Runtime"><HintPath>references/Il2CppInterop.Runtime.dll</HintPath><Private>false</Private></Reference>
    <Reference Include="0Harmony"><HintPath>references/0Harmony.dll</HintPath><Private>false</Private></Reference>
    <Reference Include="Il2Cppmscorlib"><HintPath>references/Il2Cppmscorlib.dll</HintPath><Private>false</Private></Reference>
    <Reference Include="Assembly-CSharp"><HintPath>references/Assembly-CSharp.dll</HintPath><Private>false</Private></Reference>
    <Reference Include="UnityEngine.CoreModule"><HintPath>references/UnityEngine.CoreModule.dll</HintPath><Private>false</Private></Reference>
    <Reference Include="UnityEngine.UI"><HintPath>references/UnityEngine.UI.dll</HintPath><Private>false</Private></Reference>
    <Reference Include="Unity.InputSystem"><HintPath>references/Unity.InputSystem.dll</HintPath><Private>false</Private></Reference>
  </ItemGroup>
</Project>
```

`src/HelloMod.cs` — the smallest viable mod skeleton:

```csharp
using System;
using MelonLoader;
using UnityEngine;
using UnityEngine.InputSystem;

[assembly: MelonInfo(typeof(GregMod.Hello.HelloMod), "gregMod.Hello", "1.0.0", "YourName")]
[assembly: MelonGame("Waseku", "Data Center")]

namespace GregMod.Hello
{
    public sealed class HelloMod : MelonMod
    {
        private static Key _toggleKey = Key.F8;

        public override void OnInitializeMelon()
        {
            try
            {
                var cat = MelonPreferences.CreateCategory("gregMod.Hello");
                var keyEntry = cat.CreateEntry("ToggleKey", "F8", "Hotkey for the Hello panel.");
                if (Enum.TryParse<Key>(keyEntry.Value, true, out var k) && k != Key.None)
                    _toggleKey = k;
                else
                    MelonLogger.Warning($"[Hello] Unknown ToggleKey '{keyEntry.Value}', defaulting to F8.");
            }
            catch { /* continue standalone */ }

            MelonLogger.Msg($"[Hello] loaded. {_toggleKey} = panel.");
            if (GregHost.HasCore)
            {
                try { RegisterCoreExtras(); } catch { /* gregCore missing at runtime */ }
            }
        }

        // Call ONLY with gregCore present (separate method = JIT separation, see Greg Contract).
        private static void RegisterCoreExtras()
        {
            try
            {
                gregCore.Core.Mods.GregModRegistry.Register(
                    "gregMod.Hello", "Hello", "1.0.0", new string[] { "hello" });
                gregCore.UI.GregHudRegistry.Register("hello", _toggleKey.ToString(), "Hello");
                gregCore.UI.GregMenuRegistry.RegisterOpener("hello",
                    () => MelonLogger.Msg("[Hello] Hook panel toggle here (see UI Panels)."));
                gregCore.UI.GregMenuRegistry.RegisterCloser("hello", () => { });
            }
            catch (Exception ex)
            {
                MelonLogger.Warning("[Hello] Core registration failed: " + ex.GetBaseException().Message);
            }
        }

        public override void OnUpdate()
        {
            try
            {
                var kb = Keyboard.current;
                if (kb == null) return;
                var ctrl = kb[_toggleKey];
                if (ctrl != null && ctrl.wasPressedThisFrame)
                    MelonLogger.Msg("[Hello] Key pressed (panel: see UI Panels).");
            }
            catch { /* input best-effort */ }
        }
    }
}
```

`src/GregHost.cs` — soft-dependency probe (required if gregCore stays optional):

```csharp
using System;

namespace GregMod.Hello;

// Pure type-name lookup, no direct type access: methods touching gregCore types
// must ONLY run when HasCore is true (otherwise JIT TypeLoad without the DLL).
internal static class GregHost
{
    private const string ProbeType = "gregCore.UI.GregNotificationManager, gregCore";
    private static bool? _hasCore;

    public static bool HasCore
    {
        get
        {
            if (_hasCore == null)
            {
                try { _hasCore = Type.GetType(ProbeType) != null; }
                catch { _hasCore = false; }
            }
            return _hasCore.Value;
        }
    }
}
```

This example deliberately has no panel — it compiles exactly as shown
(verified via `tools/new-mod.sh` scaffold + `dotnet build`). The panel comes
as step 2: [UI Panels](ui-panels.md) (hang Toggle/IsVisible/SetOpen/Closer
onto the marked spots, following the Backplanes template).

## 3. Build, deploy, verify

```bash
dotnet build gregMod.Hello.csproj -c Release
cp bin/Release/net6.0/gregMod.Hello.dll \
  ~/.local/share/Steam/steamapps/common/Data\ Center/Mods/
```

Start the game (restart required, DLLs load at startup) and check:

1. `MelonLoader/Latest.log` contains `[Hello] loaded` with no exceptions.
2. The key (F8) logs the press (panel: step 2).
3. With gregCore: F1 hub lists the mod, right-side key HUD shows `F8 Hello`.

## 4. Conventions (short)

- Pacing: no per-frame reflection, no scans in `OnUpdate` (details: [Harmony + IL2CPP](harmony-il2cpp.md)).
- Defensive patches: `try/catch` + null checks in every prefix/postfix.
- `Private=false` on all game references; `references/*.dll` belongs in `.gitignore`.
- Keep versions in sync: `MelonInfo` + workshop `metadata.json` if applicable.
