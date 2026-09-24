# Guidebook JS 02 Project

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
