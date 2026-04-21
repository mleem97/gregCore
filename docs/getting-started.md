---
title: Getting Started (Modding)
description: Einstieg in gregCore mit klarer Trennung zwischen MelonLoader-Plugins und Script-Mods.
slug: /getting-started
---

## Voraussetzungen

- Visual Studio 2022
- .NET 6 SDK
- MelonLoader
- `gregCore.dll`
- Steam-Spiel **Data Center**

## Grundregel: Plugin vs Script

- **Plugin**: MelonLoader-Assembly (`*.dll`) im Mods-Plugin-Kontext.
- **Script**: Datei in `Mods/Scripts` (`*.lua`, `*.py`, `*.rs`, `*.rmod`, `*.js`, `*.ts`, `*.cs`).

## Erstes C#-Plugin (kompilierbar)

```csharp
using MelonLoader;

[assembly: MelonInfo(typeof(HelloPlugin), "HelloPlugin", "1.0.0", "TeamGreg")]
[assembly: MelonGame("", "Data Center")]

public sealed class HelloPlugin : MelonMod
{
    public override void OnInitializeMelon()
    {
        MelonLogger.Msg("[HelloPlugin] loaded");
    }
}
```

## Erstes Lua-Script (funktional)

```lua
-- Datei: Mods/Scripts/hello.lua

greg.log_info("Hello from lua script")
greg.show_notification("Lua script is running")
```

## Framework-Guard-Pattern (REGEL 10)

```csharp
public static class Guard
{
    public static bool Ensure(bool condition, string message)
    {
        if (!condition)
        {
            MelonLoader.MelonLogger.Warning($"[gregCore] Guard failed: {message}");
            return false;
        }

        return true;
    }
}
```

## Deployment-Matrix

| Typ | Datei | Zielpfad |
|---|---|---|
| Plugin | `MyMod.dll` | `Data Center/Mods` |
| Lua Script | `*.lua` | `Data Center/Mods/Scripts` |
| Python Script | `*.py` | `Data Center/Mods/Scripts` |
| Rust Script/Mod Trigger | `*.rs`, `*.rmod` | `Data Center/Mods/Scripts` |
| JS/TS Script | `*.js`, `*.ts` | `Data Center/Mods/Scripts` |
| C# Script | `*.cs` | `Data Center/Mods/Scripts` |

## Häufige Fehler (Top 5)

1. `Python.Runtime` fehlt → Python-Host wird übersprungen.
2. Script liegt im falschen Ordner (`Plugins` statt `Mods/Scripts`).
3. TypeScript ohne Transpiler abgelegt.
4. C# Script ohne Roslyn-Runtime.
5. Lua-Fehler im Script stoppt das jeweilige Script (Framework bleibt aktiv).

## Nächste Schritte

- [Scripting Language Support](./scripting-language-support.md)
- [MoonSharp Lua Integration](./moonsharp-lua-integration.md)
- [Framework Dependencies](./framework-dependencies.md)
