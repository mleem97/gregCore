# GregCore Lua mod template

Install the `example-mod` directory below the GregCore Lua mods directory. The manifest is the preferred format. Legacy single `.lua` files directly in the Lua directory remain supported and use the filename as their mod ID.

The event API returns a subscription token:

```lua
local handle = greg.on("gregMod.lifecycle.sceneLoaded", callback)
greg.off(handle)
```

All subscriptions are removed automatically during shutdown/reload.
