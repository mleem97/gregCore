---
title: "Guidebook Computer UI"
---
# Guidebook Computer UI

Add custom shortcuts to the in-game computer and ship your own pages/apps — in C# and Lua. Internals: `docs/modding/computer-ui.md`. Registry: `gregCore.UI.GregComputer`.

## What you get

The computer (`Il2Cpp.ComputerShop`) has a main screen with vanilla buttons (Shop, Balance Sheet, Hire, Network Map, …). gregCore lets you:

1. **Inject shortcut buttons** next to them (cloned from the first labeled vanilla button, deduped, re-synced on every main-screen visit).
2. **Open your own app page**: C# gets a frame page (title + your content + Back button, input-locked); Lua gets a tablet page you fill with `panel_add_*`.
3. **React to events**: `greg.COMPUTER.ShortcutClicked`, `greg.COMPUTER.AppOpened`, `greg.COMPUTER.AppClosed`.

## C# (5 minutes)

```csharp
public override void OnLoad()
{
    if (!GregHost.HasCore) return;
    GregComputer.RegisterApp("my_mod", "fleet", "Fleet Manager",
        builder => builder.AddHeadline("Fleet").AddLabel("All systems nominal."));
    GregComputer.RegisterShortcut("my_mod", "fleet", "Fleet Manager",
        onClick: null, appId: "fleet");
}

public override void OnUnload()
{
    GregComputer.UnregisterAll("my_mod");
}
```

Open the computer in-game → main screen → your “Fleet Manager” button → your page with a Back button. Full API (`TryOpenApp`, `CloseApp`, `Shortcuts()`, `Apps()`, ordering via `order`): `docs/modding/computer-ui.md`.

## Lua (5 minutes)

```lua
function on_init()
    greg.computer.register_app("fleet", "Fleet Manager", function(handle)
        panel_add_label(handle, "Broken: " .. tostring(greg.server.broken_count()))
        panel_add_button(handle, "Repair all", function()
            local fixed = greg.server.repair_all()
            greg.ui.notify("Repaired " .. tostring(fixed) .. " servers.")
        end)
        panel_add_button(handle, "Close", function()
            greg.computer.close_app()
        end)
    end)
    greg.computer.register_shortcut("fleet", "Fleet Manager", "fleet")
end
```

`register_shortcut(id, label, fn_or_appid)`: a function runs on click, a string opens that app. Introspect with `list_shortcuts()` / `list_apps()` / `current_app()`.

## Checklist

- [ ] Button appears on the computer main screen (open computer → main screen; if not, check the loader log for patch errors).
- [ ] Click opens your page; Back (C#) or your Close button (Lua) returns; closing the computer releases everything.
- [ ] Uninstall/reload removes buttons and pages (no stale `greg-computer-*` clones).
- [ ] One shortcut per idea; `order` under 100 floats yours to the top.

Limits (v1): main screen only, labels only (no icons yet), Lua pages carry no input lock — details in `docs/modding/computer-ui.md`.
