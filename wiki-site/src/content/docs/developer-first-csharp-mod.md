---
title: "Developer First CSharp Mod"
---
# Developer First CSharp Mod

Build your first C# mod with the template: soft dependency, registry, menu binding, one Harmony patch, one event subscription.

## 1. Scaffold from the template

Copy `templates/csharp/` (`ExampleMod.cs`, `GregHost.cs`, `GregMod.Template.csproj`, `README.md`):

- `GregHost.HasCore` — JIT-split probe. Your mod loads with or without gregCore; with it, you get the full API.
- `ExampleMod : GregMod` — override `OnLoad / OnReady / OnUpdate / OnSceneLoaded / OnUnload`; `Initialize(GregApiContext)` injects logger, buses, config, persistence.
- Reference `gregCore.dll` with `Private=false` (soft-dep). Only go hard-dep when it pays (see `docs/modding/hard-dependency.md`).

## 2. Minimal mod

```csharp
[GregMod("my_csharp_mod", "My C# Mod", "1.0.0")]
[GregDependsOn("gregCore", "1.2.3")]
public sealed class MyMod : GregMod
{
    public override void OnLoad()
    {
        if (!GregHost.HasCore) return; // degraded mode: no framework, no crash

        Logger.Info("Hello from My C# Mod!");
        GregModRegistry.Register("my_csharp_mod", "My C# Mod", "1.0.0", menus: new[] { "my_csharp_mod.main" });

        // One-call F1 Hub wiring (no hand-rolled opener)
        GregMenuBinding.BindToggle("my_csharp_mod.main", ToggleMainMenu, () => IsMainOpen);
        GregHudRegistry.Register("my_csharp_mod", "M", "My Mod (M)");

        // Subscribe to a game event (base helper: tracked, IDisposable)
        On("greg.PLAYER.CoinChanged", OnCoinsChanged);
    }

    void OnCoinsChanged(EventPayload p) => Logger.Info($"Coins changed: {p.Data["Amount"]}");
}
```

## 3. Panel (UIToolkit, not IMGUI)

`OnGUI` is stripped in the IL2CPP build — always build with `GregPanelBuilder`:

```csharp
GregPanelBuilder.Create("My Mod")
    .SetSize(500, 600)
    .Build()
    .AddHeadline("My Mod")
    .AddLabel("Balance: ...")
    .AddButton("Repair all", () => { /* ... */ GregMenuBinding.Report("my_csharp_mod.main", IsMainOpen); })
    .Show();
```

Clicks need both `RegisterCallback<ClickEvent>` and the per-frame `worldBound` fallback via `GregClickRouter` (no `EventSystem` in this build); fonts via `GregFontLoader.DefaultUGUIFont` (null-tolerant). Full recipe: [Developer UI Panels HUD](./developer-ui-panels-hud/).

## 4. Harmony patch (one task per class)

```csharp
public sealed class MyPatch : SafePatch
{
    // explicit PatchAll, try/catch inside, true/false prefix semantics
}
```

Rules (full: [Developer Harmony IL2CPP](./developer-harmony-il2cpp/)): explicit patch classes, cached `Type` lookups, no `FindObjectsOfType` in `OnUpdate`, throttle scans, `Il2CppReferenceArray` copies, never cache Il2Cpp objects long-term.

## 5. High-level modules (prefer these over raw patches)

The static `greg` facade (`src/gregCore.Mod/PublicApi/greg.cs`) exposes modules (`GregEconomyModule`, `GregPlayerModule`, `GregServerModule`, …), plus the `Greg*` bridges (`GregServers.FindAll / Repair`, `GregShop`, `GregTechnicians`, `GregEntityInventory`, …), grid placement, wall racks, and the save engine. Prefer these over raw patches. (The coroutine engines under `Infrastructure/Automation` are `internal` framework machinery, not mod API.)

## 6. Build, deploy, verify

```bash
dotnet build -c Release
# deploy DLL per scripts/Deploy-Release-ToDataCenter.ps1 conventions, launch game
```

Verify: F1 Hub lists your mod with working Open/Close; HUD key row appears; log shows patch-applied lines; uninstall leaves the save loadable (sidecars only — [Core Save Engine](./core-save-engine/)).
