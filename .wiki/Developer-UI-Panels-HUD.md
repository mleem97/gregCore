# Developer UI Panels HUD

Build panels, HUD rows, and Hub entries the supported way. (Oxide equivalent: "CUI library".)

## Layer model (`GregUILayerManager`)

`Background(-1000) < HUD(0) < Panel(1000) < Dialog(2000) < Overlay(3000) < Tooltip(4000) < Notification(5000)`. The Mod Hub lives in Dialog; your panels in Panel; the key bar in HUD. Resolve roots via `GetLayerRoot(type)` / `AddToLayer()` — never parent to game canvases directly.

## Panels: the canonical recipe

IMGUI `OnGUI` is stripped — use UIToolkit via `GregPanelBuilder` (`src/gregCore.UI/GregPanelBuilder.cs`):

```csharp
GregPanelBuilder.Create("Title")
    .SetSize(500, 600)
    .Build()                       // Layer.Panel by default
    .AddHeadline("…") .AddLabel("…")
    .AddButton("Do it", OnDo)      // + AddSecondaryButton / AddToggle / AddSwitch
    .AddSlider("…", 0, 100, 50, OnSlide)   // + AddDropdown / AddInputField
    .AddSpacer(8) .AddSeparator()
    .Show();                       // .Hide() / .Toggle() / .Destroy()
```

Three mandatory companions (IL2CPP realities, see `docs/modding/ui-panels.md`):

1. **Clicks** — register `Button.RegisterCallback<ClickEvent>` **and** the per-frame `worldBound` fallback (`GregClickRouter.RouteClicks(clickables, lastClickUtc)` returns `bool`, no `ref`; `MarkRealClick()` returns the new timestamp — assign it: `_lastClick = GregClickRouter.MarkRealClick()`), 500 ms dedup, `Screen.height - y` flip. There is no `EventSystem`.
2. **Fonts** — apply `GregFontLoader.DefaultUGUIFont` (null-tolerant; Toolkit default renders invisible otherwise).
3. **Input lock** — `GregMenuOptions{LockCamera, LockMovement, LockInteract, ShowCursor, Draggable, SlideFromRight, PanelWidth}` + `GregMenuRegistry.SetOpen(menuId, open)`; ignore toggles while Pause/Escape/Options canvases are open.

`GregPanel.GetOrCreate / RebuildContent / TickAll` (0.28 s slide-in, docked right, draggable handle) + `GregUIStack`, animations, theme, tooltips, and `GregNotificationManager` cover the rest.

## Hub + HUD wiring (one call each)

```csharp
GregMenuBinding.BindToggle("my_mod.main", ToggleFn, () => isOpen);
GregMenuBinding.Report("my_mod.main", isOpen);   // keep Hub state truthful
GregMenuRegistry.RegisterOpener("my_mod.main", OpenFn);
GregMenuRegistry.RegisterCloser("my_mod.main", CloseFn);
GregHudRegistry.Register("my_mod", "M", "My Mod (M)");  // right-edge key bar
```

The F1 Hub (`GregModHub`) groups `GregModRegistry.All()` by `mod.Menus[]` vs `GregMenuRegistry.Snapshot()`; honest buttons only (Close iff `HasCloser`). Settings jump via `GregSettingsHub.FindTabForMenu(...)`.

## Lua tablets / widgets (handles)

Flat globals returning string handles (`pnl_<guid12>`), not `greg.ui.*`:

```lua
local id = tablet_open("My Tablet")        -- "" = failed
local w = widget_open("Mini", 100, 200)
panel_add_label(id, "Hello")
panel_add_section(id, "Group")
panel_add_spacer(id, 8)
panel_add_button(id, "Repair", function() greg.server.repair_all() end)
panel_add_toggle(id, "Enabled", true, function(v) ... end)
panel_add_slider(id, "Speed", 0, 10, 5, function(v) ... end)
panel_toggle(id)      -- -> new visibility
panel_visible(id)     -- -> bool
panel_close(id)       -- -> bool
```

Also: `greg.ui.notify(msg, seconds?)`, `greg.ui.log_*`, `greg.ui.register_mod_config_tab(tab_id, label, builder_fn)`.
C# settings tabs: `GregSettingsHub.RegisterTab` with `AddToggle / AddSlider / …` (MelonPreferences-backed).

## Checklist

- [ ] Panel opens/closes from both its hotkey **and** the F1 Hub.
- [ ] Clicks work (callback + router fallback) at multiple resolutions.
- [ ] Fonts visible; input locks only while open; cursor restored after.
- [ ] HUD row registered once per mod; Hub shows Open *or* Close truthfully.
