# Guidebook CSharp 01 Setup

Scaffold, reference, and build your first C# mod. Template source: `templates/csharp/` (`ExampleMod.cs`, `GregHost.cs`, `GregMod.Template.csproj`).

## 1. Copy the template

Copy the three files into your mod project and rename the namespace. The template mod is minimal on purpose:

```csharp
using gregCore.PublicApi;
using gregCore.PublicApi.Attributes;
using gregCore.Core.Models;

namespace ExampleMod;

[GregMod("example.mod", "Example Mod", "1.0.0")]
public sealed class Example : GregMod
{
    private IDisposable? _subscription;

    public override void OnLoad()
    {
        Logger.Info("Example mod loaded.");
        _subscription = On("gregMod.lifecycle.sceneLoaded", OnScene);
    }

    private void OnScene(EventPayload payload)
    {
        MainThread.Enqueue(() => Logger.Info("Scene callback handled on the main thread."));
    }

    public override void OnShutdown() => _subscription?.Dispose();
}
```

Two patterns to internalize: subscriptions are `IDisposable` (dispose in `OnShutdown`); game-touching callbacks marshal via `MainThread.Enqueue`.

## 2. Reference gregCore (soft dependency)

Reference `gregCore.dll` with `Private=false` and copy `GregHost.cs` into your mod (any folder under `src/`). It probes by type-name string (`gregCore.UI.GregNotificationManager, gregCore`) — no hard load:

```csharp
if (GregHost.HasCore)
{
    try { RegisterCoreExtras(); } catch { /* best-effort */ }
}
```

Rule from `GregHost.cs`: methods touching `gregCore.*` types may **only** run when `HasCore` is true **and** must live in their own methods — otherwise the JIT throws `TypeLoad` when the DLL is missing. Your mod then loads degraded (base features) instead of crashing. Going hard-dep is a conscious choice (the HexViewer playbook deleted 351 lines that way) — see `docs/modding/hard-dependency.md`.

## 3. Declare identity + dependencies

```csharp
[GregMod("shift_helper_cs", "ShiftHelper C#", "1.0.0")]
[GregDependsOn("gregCore", "1.2.3")]
```

IDs are lowercase-dotted by convention (`gregMod.<Name>` appears in the registry). `GregModDeps` (`Declare / EnsureLoaded / CheckAll`) plus manifest diff (`GetLocalManifest / DiffManifests / FormatDiff`) handle runtime gating and co-op mod-sync.

## 4. Build and deploy

```bash
dotnet build -c Release
# deploy the DLL per scripts/Deploy-Release-ToDataCenter.ps1 conventions, launch the game
```

Verify: F1 Hub lists your mod; loader log shows your `Logger.Info` line; backquote console → `mods` includes it.

## Checkpoint

- [ ] Template builds; soft-dep probe in place; mod loads with **and** without gregCore (degraded, no crash).
- [ ] `[GregMod]` + `[GregDependsOn]` set; subscription disposed on shutdown.

Next: [[Guidebook CSharp 02 Lifecycle]] — overrides, registry, events, logging, threading.
