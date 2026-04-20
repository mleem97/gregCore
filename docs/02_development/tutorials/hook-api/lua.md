# Lua Hook API Tutorial

In Lua mods, the `greg` global object is automatically provided to interact with the gregCore Hook API.

## Subscribing to a Hook

To listen for an event, use `greg.on(hookName, callback)`.

```lua
-- Register a mod
greg.log_info("My Lua Mod initializing...")

-- Subscribe to the player's coin changed hook
greg.on("greg.PLAYER.CoinChanged", function(payload)
    local amount = payload.data["Amount"]
    local total = payload.data["Total"]
    greg.log_info("Money changed! Amount: " .. tostring(amount) .. ", Total: " .. tostring(total))
end)
```

## Firing a Hook

To fire a custom event from your Lua mod, use `greg.fire(hookName, dataTable)`.

```lua
-- Fire a custom hook
greg.fire("greg.CUSTOM.MyEvent", {
    foo = "bar",
    value = 42
})
```

## Troubleshooting

- Ensure your `main.lua` file is located in `Plugins/Lua/<ModId>/`.
- Check the `MelonLoader` console for any Lua errors or log messages from `LuaFFIBridge`.
- Ensure the hook name starts with `greg.`.
