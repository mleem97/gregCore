# Python Hook API Tutorial

In Python mods, the `greg` global object is automatically provided to interact with the gregCore Hook API.

## Subscribing to a Hook

To listen for an event, use `greg.on(hookName, callback)`.

```python
# My Python Mod initializing...
greg.log_info("My Python Mod initializing...")

# Define a callback function
def on_coin_changed(payload):
    amount = payload["data"]["Amount"]
    total = payload["data"]["Total"]
    greg.log_info(f"Money changed! Amount: {amount}, Total: {total}")

# Subscribe to the player's coin changed hook
greg.on("greg.PLAYER.CoinChanged", on_coin_changed)
```

## Firing a Hook

To fire a custom event from your Python mod, use `greg.fire(hookName, dataDict)`.

```python
# Fire a custom hook
greg.fire("greg.CUSTOM.MyEvent", {
    "foo": "bar",
    "value": 42
})
```

## Troubleshooting

- Ensure your `main.py` file is located in `Plugins/Python/<ModId>/`.
- Check the `MelonLoader` console for any Python errors or log messages from `PythonFFIBridge`.
- Ensure the hook name starts with `greg.`.
- Make sure `python310.dll` is available (configurable in `gregCore` config).
