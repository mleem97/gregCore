---
title: "Guidebook JS 02 Project"
---
# Guidebook JS 02 Project

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

Next: [Guidebook Porting Matrix](./guidebook-porting-matrix/) — every feature side-by-side in three languages.
