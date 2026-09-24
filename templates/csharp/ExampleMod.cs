using gregCore.PublicApi;
using gregCore.PublicApi.Attributes;
using gregCore.Core.Models;

namespace ExampleMod;

[GregMod("example.mod", "Example Mod", "1.0.0")]
public sealed class Example : GregMod
{
    private IDisposable? _subscription;

    public override void OnLoad()
    {
        Logger.Info("Example mod loaded.");
        _subscription = On("gregMod.lifecycle.sceneLoaded", OnScene);
    }

    private void OnScene(EventPayload payload)
    {
        MainThread.Enqueue(() => Logger.Info("Scene callback handled on the main thread."));
    }

    public override void OnShutdown() => _subscription?.Dispose();
}
