# greg.PluginTemplate

Use this template to create a standalone **greg Plugin** based on `GregPluginBase`.

## What this template includes

- `greg.PluginTemplate.csproj`: Build setup with game directory auto-detection.
- `Main.cs`: Minimal plugin implementation with `PluginId`, version gate, and framework-ready callback.

## Requirements

- MelonLoader installed in the game.
- Generated IL2CPP assemblies (`MelonLoader/Il2CppAssemblies`).
- `gregCore.dll` installed in the game `Mods` folder.

## Quick usage

1. Copy this folder and rename it (for example `greg.Plugin.MyFeature`).
1. Rename `greg.PluginTemplate.csproj` and update metadata (`AssemblyName`, `RootNamespace`, `PluginId`).
1. Build:

```powershell
dotnet build .\Templates\greg.PluginTemplate\greg.PluginTemplate.csproj /p:GameDir="C:\Path\To\Data Center"
```

1. The built DLL is copied to `Data Center/Mods` automatically (local builds).
