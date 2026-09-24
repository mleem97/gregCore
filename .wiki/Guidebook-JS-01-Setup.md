# Guidebook JS 01 Setup

<<<<<<< HEAD
JavaScript on gregCore is **beta**: a small, honest surface via Jint 4.8.0. Read this page fully before writing JS — it tells you exactly what works today.

## The actual bridge (not the stale example)

`JsBridge` (`src/gregCore.SDK/Js/JsBridge.cs`) exposes exactly five entry points on the global `greg` object — nothing else:

```js
greg.logInfo("hello");      // -> GregAPI.LogInfo
greg.logWarning("check this");
greg.logError("broken");
greg.on("greg.PLAYER.CoinChanged", function (payload) {
    greg.logInfo("Coins changed.");
});
greg.fire("my_js_mod.ping", { count: 3 });
```

Host facts (`GregJsHost`): `HostId "javascript"`, files `*.js` (`.ts` files warn — **not** transpiled), 4 MB memory limit, `OnUpdate`/`OnSceneLoaded` empty (no per-frame JS callbacks).

> Warning: `examples/Js/example_mod/main.js` uses the retired `greg.subscribe / fire_event / log` + numeric `Events` form and does **not** run on the current bridge. Copy this guidebook's snippets instead.

## Where JS files live

Flat files (no per-mod folders, no manifest) under `<game root>/UserLibs/Js/*.js` — created at boot; every `*.js` file executes once at init. The legacy `./Plugins/Js` directory is **ignored** with a warning — move files to `./UserLibs/Js/`.

There is no `mod.json`, no sandbox, no `require`, no timers, no domain APIs (`player`, `server`, `shop`… do not exist in JS). JS observes events, logs, and fires custom events that Lua/C# mods can consume.

## Your first JS mod

Create `UserLibs/Js/shift_helper.js`:

```js
// ShiftHelper JS: observe + log + announce. Beta surface only.
greg.logInfo("[shift_helper_js] loading...");

var coinEvents = 0;

greg.on("greg.PLAYER.CoinChanged", function (payload) {
    coinEvents += 1;
    var amount = payload && payload.Data ? payload.Data["Amount"] : "?";
    greg.logInfo("[shift_helper_js] Money changed by: " + amount);
    if (coinEvents % 10 === 0) {
        greg.logInfo("[shift_helper_js] " + coinEvents + " coin events seen.");
    }
});

greg.on("greg.lifecycle.scene-loaded", function (payload) {
    greg.logInfo("[shift_helper_js] Scene loaded.");
});

greg.fire("shift_helper_js.ready", { version: "1.0.0" });
greg.logInfo("[shift_helper_js] ready.");
```

Restart the game; verify in the loader log: your lines appear, coin changes increment the counter, no `[JsBridge] JS-Fehler` errors.

## Decide: JS or Lua?

Choose JS only when **all** are true: you need events + logging only; flat-file deployment is fine; no settings, timers, panels, or game-state reads. Anything else → Lua ([[Guidebook Lua 01 First Mod]]) today; the JS surface grows with feedback, and this page tracks it.

## Checkpoint

- [ ] File in `UserLibs/Js/`, log lines observed, coin counter works, no bridge errors.
- [ ] You can state the five JS entry points and the four things JS cannot do (folders/manifest, timers, domain APIs, panels).

Next: [[Guidebook JS 02 Project]] — a complete JS observer + its Lua/C# counterparts.
=======
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
>>>>>>> agent/gregcore-integration
