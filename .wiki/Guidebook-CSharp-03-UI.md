# Guidebook CSharp 03 UI

Hub entries, HUD rows, and UIToolkit panels — the one-call wiring plus the three IL2CPP companions. Layers: `GregUILayerManager` (`Background < HUD < Panel < Dialog < Overlay < Tooltip < Notification`); Hub in Dialog, your panels in Panel, key bar in HUD.

## Hub + HUD (one call each)

```csharp
GregMenuBinding.BindToggle("shift_helper_cs.main", ToggleMain, () => _isOpen);
GregMenuBinding.Report("shift_helper_cs.main", _isOpen);   // keep Hub truthful
GregMenuRegistry.RegisterOpener("shift_helper_cs.main", OpenMain);
GregMenuRegistry.RegisterCloser("shift_helper_cs.main", CloseMain);
GregMenuRegistry.RegisterMenu("shift_helper_cs.main", new GregMenuOptions
{
    LockCamera = true, LockMovement = true, LockInteract = true,
    ShowCursor = true, PanelWidth = 420f,
});
GregHudRegistry.Register("shift_helper_cs", "H", "ShiftHelper (H)");
```

The F1 Hub (`GregModHub`) groups `GregModRegistry.All()` by `mod.Menus[]` vs `GregMenuRegistry.Snapshot()` and renders honest buttons (Close only iff `HasCloser`). Settings jump: `GregSettingsHub.FindTabForMenu(menuId, owner)->ShowTab()`.

## Panels: the canonical recipe

`OnGUI` is stripped in IL2CPP — always `GregPanelBuilder`:

```csharp
GregPanelBuilder.Create("ShiftHelper")
    .SetSize(500, 600)
    .Build()
    .AddHeadline("ShiftHelper")
    .AddLabel($"Broken servers: {broken}")
    .AddButton("Repair all", () => { RepairAll(); GregMenuBinding.Report("shift_helper_cs.main", _isOpen); })
    .AddToggle("Auto-repair", autoRepair, v => autoRepair = v)
    .AddSlider("Interval", 10, 300, interval, v => interval = (int)v)
    .AddSpacer(8).AddSeparator()
    .Show();   // .Hide() / .Toggle() / .Destroy()
```

Three mandatory companions (see `docs/modding/ui-panels.md`):

1. **Clicks**: `Button.RegisterCallback<ClickEvent>` **and** the per-frame `worldBound` fallback (`GregClickRouter.RouteClicks`, 500 ms dedup, `Screen.height - y` flip) — there is no `EventSystem`.
2. **Fonts**: `GregFontLoader.DefaultUGUIFont` (null-tolerant; Toolkit default renders invisible otherwise).
3. **Input lock**: `GregMenuRegistry.SetOpen(menuId, open)` drives `GregInputLock` (cursor, `PlayerManager` movement/look flags, `PlayerInput.actions`); ignore toggles while Pause/Escape/Options canvases are open.

Extras: `GregPanel.GetOrCreate / RebuildContent / TickAll` (0.28 s slide-in, right-docked, draggable), `GregUIStack`, theme, tooltips, `GregNotificationManager`, settings tabs via `GregSettingsHub.RegisterTab` (MelonPreferences-backed).

## Exercises

1. Open/close your panel from hotkey **and** Hub; confirm Close appears (closer registered).
2. Click every control at two resolutions; confirm font visibility and cursor restore.
3. Add a Settings tab and reach it from the Hub's Settings button.

## Checkpoint

- [ ] Panel + Hub + HUD wired; clicks work without `EventSystem`; input locks only while open.

Next: [[Guidebook CSharp 04 Patches]] — Harmony the safe way.
