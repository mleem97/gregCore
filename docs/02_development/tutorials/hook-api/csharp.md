# C# Hook API Tutorial

In C# mods, you'll use the `gregCore.Sdk.IGregAPI` interface to interact with the Hook API.

## Subscribing to a Hook

To listen for an event, you'll use the `On` method in your `MelonMod`.

```csharp
using gregCore.Sdk;
using gregCore.Sdk.Models;

namespace MyCsharpMod;

public class MyMod : MelonMod
{
    private IGregAPI _api;

    public override void OnInitializeMelon()
    {
        // Get the SDK API instance
        _api = GregAPI.GetSdkApi();

        // Subscribe to a hook
        _api.On("greg.PLAYER.CoinChanged", OnCoinChanged);
    }

    private void OnCoinChanged(GregPayload payload)
    {
        var amount = payload.GetValue<float>("Amount");
        var total = payload.GetValue<float>("Total");
        _api.LogInfo($"Money changed! Amount: {amount}, Total: {total}");
    }
}
```

## Firing a Hook

To fire a custom event from your C# mod, use the `Fire` method.

```csharp
// Fire a custom hook from C#
var payload = new GregPayload("greg.CUSTOM.MyEvent", "MyCsharpMod");
payload.Data["foo"] = "bar";
payload.Data["value"] = 42;

_api.Fire("greg.CUSTOM.MyEvent", payload);
```

## Troubleshooting

- Ensure your C# mod is a `MelonMod`.
- Reference the `gregCore.dll` and `gregCore.Sdk` namespaces.
- Use `GregAPI.GetSdkApi()` to get the API instance.
- Ensure the hook name starts with `greg.`.
