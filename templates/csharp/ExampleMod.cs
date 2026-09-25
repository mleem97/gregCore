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

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "S1172:Unused method parameters should be removed", Justification = "Event-callback signature is fixed by the On() subscription API; the template shows the canonical handler shape.")]
    private void OnScene(EventPayload _)
    {
        MainThread.Enqueue(() => Logger.Info("Scene callback handled on the main thread."));
    }

    public override void OnShutdown() => _subscription?.Dispose();
}
