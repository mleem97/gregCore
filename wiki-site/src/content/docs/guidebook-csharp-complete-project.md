---
title: "Guidebook CSharp Complete Project"
---
# Guidebook CSharp Complete Project

ShiftHelper in C# — the Lua complete project ([Guidebook Lua Complete Project](./guidebook-lua-complete-project/)) mirrored with registry, menu binding, panel, event, repair pass, and sidecar. Every call below is verified against `src/` (namespaces in the `using` block).

## The mod (annotated)

```csharp
using System;
using gregCore.PublicApi;
using gregCore.PublicApi.Attributes;
using gregCore.Core.Models;
using gregCore.Core.Mods;
using gregCore.Core.Networking;
using gregCore.Infrastructure.Persistence;
using gregCore.UI;

namespace ShiftHelperCs;

// Requires GregHost.cs from templates/csharp/ in the same project,
// with its namespace adjusted to ShiftHelperCs (see Guidebook CSharp 01 Setup).

[GregMod("shift_helper_cs", "ShiftHelper C#", "1.0.0")]
[GregDependsOn("gregCore", "1.2.3")]
public sealed class ShiftHelper : GregMod
{
    private IDisposable? _coins;
    private bool _isOpen;
    private bool _autoRepair = true;

    public override void OnLoad()
    {
        // 1. Identity. Soft-dep: without gregCore we load degraded, never crash.
        if (!GregHost.HasCore) return;
        GregModRegistry.Register("shift_helper_cs", "ShiftHelper C#", "1.0.0",
            menus: new[] { "shift_helper_cs.main" });

        // 2. Hub + HUD wiring (one call each).
        GregMenuBinding.BindToggle("shift_helper_cs.main", ToggleMain, () => _isOpen);
        GregMenuRegistry.RegisterOpener("shift_helper_cs.main", OpenMain);
        GregMenuRegistry.RegisterCloser("shift_helper_cs.main", CloseMain);
        GregHudRegistry.Register("shift_helper_cs", "H", "ShiftHelper (H)");

        // 3. Events via the base-class helper (tracked; released by DisposeSubscriptions).
        _coins = On("greg.PLAYER.CoinChanged",
            p => Logger.Info($"Coins changed: {p.Data["Amount"]}"));

        // 4. Saves: sidecar round-trip (atomic .tmp + .bak; see GregSaveGuard).
        GregSaveGuard.RegisterSidecar("shift_helper_cs",
            save: () => _autoRepair ? "auto=1" : "auto=0",
            load: data => _autoRepair = data.Contains("auto=1"));

        // 5. Inventory: re-resolve after every rebuild (IDs may have healed).
        GregEntityInventory.Rebuilt += () =>
            Logger.Info($"Inventory rebuilt; servers: {GregEntityInventory.Count(GregEntityInventory.InventoryKind.Server)}");

        Logger.Info("ShiftHelper C# loaded.");
    }

    public override void OnReady()
    {
        // Heavy init belongs here, not in OnLoad.
    }

    private void ToggleMain() { if (_isOpen) CloseMain(); else OpenMain(); }

    private void OpenMain()
    {
        _isOpen = true;
        GregMenuRegistry.SetOpen("shift_helper_cs.main", true);
        GregMenuBinding.Report("shift_helper_cs.main", true);
        GregPanelBuilder.Create("ShiftHelper")
            .SetSize(500, 600).Build()
            .AddHeadline("ShiftHelper")
            .AddButton("Repair all servers", () => RepairAllServers())
            .AddToggle("Auto-repair", _autoRepair, v => _autoRepair = v)
            .Show();
    }

    private void CloseMain()
    {
        _isOpen = false;
        GregMenuRegistry.SetOpen("shift_helper_cs.main", false);
        GregMenuBinding.Report("shift_helper_cs.main", false);
    }

    private void RepairAllServers()
    {
        int fixed_ = 0;
        try
        {
            foreach (var server in GregServers.FindAll())
            {
                try { if (GregServers.Repair(server)) fixed_++; }
                catch { /* per-entry guard: one bad server never aborts the pass */ }
            }
        }
        catch (Exception ex) { Logger.Error("Repair pass failed", ex); }
        Logger.Info($"Repair pass: {fixed_} server(s) repaired.");
        GregNotificationManager.Show($"Repaired {fixed_} server(s).", 4f);
        GregMenuBinding.Report("shift_helper_cs.main", _isOpen);
    }

    public override void OnUnload()
    {
        _coins?.Dispose();
        DisposeSubscriptions();
        if (_isOpen) CloseMain();
    }
}
```

## Why it is shaped this way

- Same `shift_helper` idea, separate ID (`shift_helper_cs`) so both can coexist for comparison.
- Same layering: registry → Hub/HUD → events → saves → inventory → panel → cleanup (`OnUnload` mirrors Lua `on_shutdown`; `OnShutdown()` already forwards to it).
- Repair goes through the public bridge (`GregServers.FindAll / Repair`, per-entry guarded like the framework's own healer) — never cached Il2Cpp handles, never a second ID scheme.
- `GregMenuBinding.Report` keeps the Hub truthful after every state change; `GregNotificationManager.Show(message, duration)` toasts like Lua `greg.ui.notify`.

## Ship checklist

- [ ] Loads with and without gregCore (degraded, no crash); Hub Open/Close honest; HUD row live.
- [ ] Event logs; sidecar round-trips; inventory rebuild logged; uninstall leaves vanilla save loadable.
- [ ] README + version + changelog → [Guidebook Release](./guidebook-release/).
