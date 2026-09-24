# Guidebook JS 01 Setup

JavaScript/TypeScript on gregCore is a **full UI-focused SDK** (Jint): per-mod
engines, toasts, Toolkit panels, F1-hub binding, settings, hooks — plus
HotLoad in the main menu. Read this page, then build [[Guidebook JS 02 Project]].

## Layout

One folder per mod (folder name = mod ID):

```
<game root>/UserData/gregCore/Mods/JS/<modId>/*.js
```

Write TypeScript against `templates/js/greg.d.ts`, compile with `tsc`,
ship the emitted `.js`. Raw `.ts` files are **skipped** with a warning
(no transpiler in the game). Each mod gets an isolated engine (16 MB cap).

## The `greg` API

```js
greg.log("hi"); greg.warn("careful"); greg.error("broken");
greg.toast("Done.", 3);
greg.toastRich("MYMOD", "Title", "sub line", 5);
greg.notify("Title", "Message", 5);

var panel = greg.createPanel("My Mod");   // chainable Toolkit builder
panel.AddHeadline("Hello");
panel.AddLabel("Status: ok.");
panel.AddButton("Ping", function () { greg.toast("pong", 2); });
panel.AddSecondaryButton("Close", toggle);
panel.Show(); // .Hide() / .Toggle() / ClearContent() for refresh

greg.bindMenu("mymod", toggle, isOpen);   // F1-hub wiring (one call)
greg.reportMenu("mymod", open);           // live state from hotkey paths

greg.registerToggle("enabled", "Enabled", true);
greg.registerSlider("rate", "Rate", 1.0);
greg.registerKey("toggle", "Toggle", toggle);

greg.on("greg.PLAYER.CoinChanged", function (p) { /* p.* */ });
```

Lifecycle (define when needed): `onUpdate(dt)`, `onSceneLoaded(name)`.
A throwing callback is disabled after its first failure (no log spam).

## HotLoad (main menu only)

Edit + save → queued (500 ms debounce) → applied on next **main-menu**
entry (engine dropped, files re-executed). Never mid-game. Deleted mod
folders unload. Manual trigger from any script context is not exposed —
return to the menu.

Limits: only `greg` is a stable contract (no CLR patching, no Harmony
from scripts — use hooks). No compiler on the machine means no scripts
at all (check the bridge warning in the log).

## Your first JS mod

`UserData/gregCore/Mods/JS/hello/hello.js`:

```js
var count = 0;
greg.toast("hello loaded.", 3);
greg.on("greg.PLAYER.CoinChanged", function () {
    count += 1;
    if (count === 1 || count % 25 === 0)
        greg.toast("Coins changed x" + count + ".", 3);
});
```

Restart once (first discovery), then iterate without restarts via HotLoad.
Verify: toast on start, toast every 25 coin events, no `[gregCore][JS]` errors.

## Checkpoint

- [ ] Folder mod ID correct, toasts observed, coin counter works, no errors.
- [ ] You can name the UI calls (`toast`, `createPanel`, `bindMenu`) and the
      HotLoad rule (menu only, queued otherwise).

Next: [[Guidebook JS 02 Project]] — panel + settings + menu sample mod.
