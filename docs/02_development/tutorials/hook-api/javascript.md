# JavaScript/TypeScript Hook API Tutorial

In JS mods, the `greg` global object is automatically provided to interact with the gregCore Hook API.

## Subscribing to a Hook

To listen for an event, use `greg.on(hookName, callback)`.

```javascript
// Register a mod
greg.logInfo("My JS Mod initializing...");

// Subscribe to the player's coin changed hook
greg.on("greg.PLAYER.CoinChanged", (payload) => {
    const amount = payload.Data.Amount;
    const total = payload.Data.Total;
    greg.logInfo(`Money changed! Amount: ${amount}, Total: ${total}`);
});
```

## Firing a Hook

To fire a custom event from your JS mod, use `greg.fire(hookName, dataObject)`.

```javascript
// Fire a custom hook from JS
greg.fire("greg.CUSTOM.MyEvent", {
    foo: "bar",
    value: 42
});
```

## Troubleshooting

- Ensure your JS file is located in `Plugins/Js/`.
- Check the `MelonLoader` console for any JS errors or log messages from `JsBridge`.
- Ensure the hook name starts with `greg.`.
- Use Jint for a performant JS engine.
