# Eigene Mods mit Greg schreiben — Getting Started

> Stand: gregCore `1.2.3` · Spiel: Data Center (Waseku) · MelonLoader `0.7.x` · Unity `6000.4.x` · Target: `net6.0`
>
> Diese Anleitung beschreibt den **verifizierten** Weg (Stand der Mod-Repos).
> Weiterführend: [Greg-Vertrag](gregcore-vertrag.md) · [UI-Panels](ui-panels.md) ·
> [Harmony + IL2CPP](harmony-il2cpp.md) · [Shop-Items](shop-items.md) · [Events](api/events.md) · [FAQ](faq.md)

## 1. Voraussetzungen

- .NET SDK (net6.0-fähig), MelonLoader im Spiel installiert.
- Data-Center-Installation, z. B. `~/.local/share/Steam/steamapps/common/Data Center`.
- Einen Mod-Ordner nach Team-Konvention: `gregMod.<Name>/` mit `.csproj`, `src/`, `references/`.
- Nach Fresh Clone **immer zuerst**: `ModRepositories/tools/sync-melon-assemblies.sh`
  (legt `references/`-Symlinks auf `MelonLoader/net6/*.dll` +
  `MelonLoader/Il2CppAssemblies/*.dll` + `gregCore.dll`). **Nie DLLs committen.**

## 2. Minimales Projekt

`gregMod.Hello/gregMod.Hello.csproj` (Muster aus den Team-Mods):

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

`src/HelloMod.cs` — das kleinste lebensfähige Mod-Gerüst:

```csharp
using System;
using MelonLoader;
using UnityEngine;
using UnityEngine.InputSystem;

[assembly: MelonInfo(typeof(GregMod.Hello.HelloMod), "gregMod.Hello", "1.0.0", "DeinName")]
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
                var keyEntry = cat.CreateEntry("ToggleKey", "F8", "Hotkey für das Hello-Panel.");
                if (Enum.TryParse<Key>(keyEntry.Value, true, out var k) && k != Key.None)
                    _toggleKey = k;
                else
                    MelonLogger.Warning($"[Hello] Unknown ToggleKey '{keyEntry.Value}', defaulting to F8.");
            }
            catch { /* Standalone weiter */ }

            MelonLogger.Msg($"[Hello] geladen. {_toggleKey} = Panel.");
            if (GregHost.HasCore)
            {
                try { RegisterCoreExtras(); } catch { /* gregCore fehlt zur Laufzeit */ }
            }
        }

        // NUR mit gregCore aufrufen (eigene Methode = JIT-Trennung, siehe Greg-Vertrag).
        private static void RegisterCoreExtras()
        {
            try
            {
                gregCore.Core.Mods.GregModRegistry.Register(
                    "gregMod.Hello", "Hello", "1.0.0", new string[] { "hello" });
                gregCore.UI.GregHudRegistry.Register("hello", _toggleKey.ToString(), "Hello");
                gregCore.UI.GregMenuRegistry.RegisterOpener("hello", () => HelloPanel.Toggle());
                gregCore.UI.GregMenuRegistry.RegisterCloser("hello",
                    () => { try { if (HelloPanel.IsVisible) HelloPanel.Toggle(); } catch { } });
            }
            catch (Exception ex)
            {
                MelonLogger.Warning("[Hello] Core-Registrierung fehlgeschlagen: " + ex.GetBaseException().Message);
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
                    HelloPanel.Toggle();
            }
            catch { /* Input best-effort */ }
        }
    }
}
```

`src/GregHost.cs` — Soft-Dependency-Probe (Pflicht, wenn gregCore optional bleiben soll):

```csharp
using System;

namespace GregMod.Hello;

// Reiner Typname-Lookup, kein direkter Typzugriff: Methoden, die gregCore-Typen
// beruehren, duerfen NUR laufen, wenn HasCore true ist (sonst JIT-TypeLoad).
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

## 3. Bauen, deployen, prüfen

```bash
dotnet build gregMod.Hello.csproj -c Release
cp bin/Release/net6.0/gregMod.Hello.dll \
  ~/.local/share/Steam/steamapps/common/Data\ Center/Mods/
```

Spiel starten (ggf. neu starten, DLLs werden beim Start geladen) und prüfen:

1. `MelonLoader/Latest.log` enthält `[Hello] geladen` ohne Exceptions.
2. Taste (F8) öffnet/schließt das Panel (siehe [UI-Panels](ui-panels.md)).
3. Mit gregCore: F1-Hub listet den Mod, Tasten-HUD rechts zeigt `F8 Hello`.

## 4. Konventionen (kurz)

- pacing: kein per-frame Reflection, keine Scans in `OnUpdate` (Details: [Harmony + IL2CPP](harmony-il2cpp.md)).
- Defensive Patches: `try/catch` + Null-Checks in jedem Prefix/Postfix.
- `Private=false` bei allen Spiel-Referenzen; `references/*.dll` steht in `.gitignore`.
- Version einpflegen: `MelonInfo` + ggf. Workshop-`metadata.json` synchron halten.
