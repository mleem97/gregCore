---
title: "Guidebook JS 01 Setup"
---
# Guidebook JS 01 Setup

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

Choose JS only when **all** are true: you need events + logging only; flat-file deployment is fine; no settings, timers, panels, or game-state reads. Anything else → Lua ([Guidebook Lua 01 First Mod](./guidebook-lua-01-first-mod/)) today; the JS surface grows with feedback, and this page tracks it.

## Checkpoint

- [ ] File in `UserLibs/Js/`, log lines observed, coin counter works, no bridge errors.
- [ ] You can state the five JS entry points and the four things JS cannot do (folders/manifest, timers, domain APIs, panels).

Next: [Guidebook JS 02 Project](./guidebook-js-02-project/) — a complete JS observer + its Lua/C# counterparts.
