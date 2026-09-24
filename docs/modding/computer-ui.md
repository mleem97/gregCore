# ComputerUI — Custom Shortcuts & Apps

Custom buttons on the in-game computer main screen plus own pages/apps.
Registry: `src/gregCore.UI/GregComputer.cs` · injection:
`src/gregCore.Patches/GameLayer/Patches/UI/GregComputerPatch.cs` ·
Lua: `src/gregCore.SDK/Lua/Modules/LuaComputerModule.cs` ·
tests: `tests/UI/GregComputerTests.cs`.

## Concepts

- **Shortcut**: a button injected next to the vanilla screen buttons
  (Shop, Balance Sheet, Hire, Network Map, …) on the `ComputerShop`
  main screen. Clicking runs your callback — or opens your app when the
  shortcut carries an `appId`.
- **App (page)**: your own screen. C# apps get a `GregPanelBuilder`
  frame page (title + content + “← Back to computer”, input locked via
  `GregMenuRegistry` menu `computer.app.<appId>`). Lua apps get a
  tablet page built with the familiar `panel_add_*` calls.
- **Sync**: a Harmony postfix on `ButtonReturnMainScreen` and
  `OpenShop` re-injects missing buttons (idempotent, deduped by
  `greg-computer-` name prefix) and re-shows open pages;
  `CloseShop` / `HideCanvas` close pages and release locks.
- **Events**: `greg.COMPUTER.ShortcutClicked` (`ModId, Id, Label,
  AppId`), `greg.COMPUTER.AppOpened` (`AppId, ModId, Title`),
  `greg.COMPUTER.AppClosed` (`AppId`).

## C#

```csharp
// Shortcut that opens an app:
GregComputer.RegisterApp("my_mod", "fleet", "Fleet Manager",
    builder => builder.AddHeadline("Fleet").AddLabel("…"));
GregComputer.RegisterShortcut("my_mod", "fleet", "Fleet Manager",
    onClick: null, appId: "fleet");

// Shortcut with plain callback:
GregComputer.RegisterShortcut("my_mod", "ping", "Ping",
    onClick: () => Logger.Info("pong"), order: 50);

GregComputer.TryOpenApp("fleet");   // programmatic open
GregComputer.CloseApp();            // Back navigation
GregComputer.UnregisterAll("my_mod"); // cleanup on unload
```

Rules: one shortcut per `(modId, id)` (re-register replaces);
`order` sorts ascending (default 100); injection clones the first
labeled vanilla button — labels come from `Text`/`TextMeshProUGUI`,
missing template or missing computer means no buttons (never an
exception); never cache the cloned objects (re-sync heals them).

## Lua (`greg.computer`)

```lua
function on_init()
    greg.computer.register_app("fleet", "Fleet Manager", function(handle)
        panel_add_label(handle, "Broken: " .. tostring(greg.server.broken_count()))
        panel_add_button(handle, "Repair all", function()
            greg.server.repair_all()
        end)
        panel_add_button(handle, "Close", function()
            greg.computer.close_app()
        end)
    end)

    -- string target = open this app; function target = run on click:
    greg.computer.register_shortcut("fleet", "Fleet Manager", "fleet")
    greg.computer.register_shortcut("ping", "Ping", function()
        greg.ui.notify("pong")
    end)
end
```

API: `register_shortcut(id, label, fn_or_appid)`,
`unregister_shortcut(id)`, `register_app(appId, title, on_open_fn[,
on_close_fn])` (`on_open_fn(handleId)` builds with `panel_add_*`),
`unregister_app(appId)`, `open_app(appId)`, `close_app()`,
`current_app()`, `list_shortcuts()`, `list_apps()`. Shortcuts and
apps unregister automatically on mod shutdown/reload
(`LuaComputerModule.UnregisterAll`).

## Limits (v1)

- Buttons live on the **main screen only** (other computer screens are
  untouched); injection needs at least one labeled vanilla button as
  a clone template.
- No custom icons yet (labels only); icon support is a later step.
- Lua apps have no input lock (same as tablets today); C# frame pages
  lock movement/interact, not the camera.
- Local-only like everything else (see `native-coop.md`).
