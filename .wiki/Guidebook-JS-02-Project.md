# Guidebook JS 02 Project

<<<<<<< HEAD
A complete, shippable JS mod — an **event observer** — plus how Lua and C# consume what it fires.

## The project: `shift_observer.js`

Drop-in file for `UserLibs/Js/shift_observer.js`:

```js
// shift_observer: counts economy + lifecycle events, logs milestones,
// and announces itself so Lua/C# mods can react.
var MOD_ID = "shift_observer";
var counts = { coins: 0, scenes: 0 };

function milestone(kind, n) {
    return "[" + MOD_ID + "] " + n + " " + kind + " events seen.";
}

greg.on("greg.PLAYER.CoinChanged", function (payload) {
    counts.coins += 1;
    if (counts.coins === 1 || counts.coins % 25 === 0) {
        greg.logInfo(milestone("coin", counts.coins));
    }
});

greg.on("greg.lifecycle.scene-loaded", function () {
    counts.scenes += 1;
    greg.logInfo("[" + MOD_ID + "] scene #" + counts.scenes);
});

greg.fire("shift_observer.ready", { version: "1.0.0" });
greg.logInfo("[shift_observer] ready. Waiting for events.");
```

Verify: cold start logs `ready`; change money → coin milestone at 1 and every 25; change scene → scene line; no bridge errors.

## Consuming it from Lua

```lua
greg.on("shift_observer.ready", function(payload)
    greg.ui.log_info("JS observer online, v" .. tostring(payload.data["version"]))
end)
```

## Consuming it from C#

```csharp
On("shift_observer.ready",
    p => Logger.Info($"JS observer online, v{p.Data["version"]}"));
```

This is the intended beta pattern: JS observes and announces; Lua/C# do the heavy lifting (state, UI, saves).

## Limits to design around (beta)

- No timers: count events instead of polling; ask a Lua/C# companion for periodic work via your custom events.
- No game-state reads: log what payloads carry; request enrichment from a companion mod.
- No UI/saves: notify and persist on the Lua/C# side.
- 4 MB memory: keep counters small, never accumulate payloads.
- Errors surface as `[JsBridge] JS-Fehler` in the loader log — check there first.

## Checkpoint

- [ ] Observer runs standalone; Lua or C# companion reacts to `shift_observer.ready`.
- [ ] You documented the beta limits in your README so users know what JS does and does not do.

Next: [[Guidebook Porting Matrix]] — every feature side-by-side in three languages.
=======
A complete, shippable JS mod — **panel + settings + F1 menu + toasts** —
mirroring `templates/js/example-mod.ts`. Drop-in, no companion needed.

## The project: `UserData/gregCore/Mods/JS/panel_demo/panel_demo.js`

```js
var open = false;
var panel = null;

function toggle() {
    open = !open;
    if (open) {
        if (!panel) panel = greg.createPanel("Panel Demo");
        panel.ClearContent();
        panel.AddHeadline("Status");
        panel.AddLabel("Panel driven fully by script.");
        panel.AddButton("Cheer", function () { greg.toast("Cheers!", 2); });
        panel.AddSecondaryButton("Close", toggle);
        panel.AddSeparator();
        panel.AddHeadline("Scene");
        panel.AddLabel("Reopen to refresh this list.");
        panel.Show();
    } else if (panel) {
        panel.Hide();
    }
    greg.reportMenu("panel_demo", open);
}

greg.registerToggle("enabled", "Enabled", true);
greg.registerSlider("volume", "Volume", 0.8);
greg.registerKey("toggle", "Toggle panel", toggle);
greg.bindMenu("panel_demo", toggle, function () { return open; });
greg.toast("panel_demo loaded.", 3);

function onSceneLoaded(scene) {
    if (open) greg.reportMenu("panel_demo", true);
}
```

Verify: restart once → toast on start → F1 hub lists `panel_demo` →
open shows the panel → Cheer toasts → key toggles → settings show the
toggle/slider. Then edit the file, return to the **main menu**, and watch
`[gregCore][JS] Reloaded 'panel_demo'` — no restart.

## TypeScript version

Same file as `.ts` with types (`/// <reference path="../greg.d.ts" />`,
annotated functions). Compile (`tsc panel_demo.ts --target es2020`),
ship only the `.js`. Keep the `.ts` next to it in your repo, not in the
game folder (raw `.ts` in the game folder warns and skips).

## Consuming it from Lua / C#

Fire hooks others consume, as usual:

```js
greg.on("panel_demo.cheer", function () { /* ... */ });
// Lua: greg.on("panel_demo.cheer", fn) — C#: On("panel_demo.cheer", p => ...)
```

Checkpoint: panel opens from F1 **and** key, settings persist in core
settings UI, HotLoad round-trip works, no errors.
>>>>>>> agent/gregcore-integration
