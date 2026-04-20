/// <file-summary>
/// Schicht:      PublicApi
/// Zweck:        Öffentlicher Wrapper für den EventBus.
/// Maintainer:   Verhindert unautorisierte Zugriffe (z.B. ClearAll).
/// </file-summary>

namespace gregCore.PublicApi;

public sealed class GregEventBusPublic
{
    private readonly IGregEventBus _internalBus;

    public GregEventBusPublic(IGregEventBus internalBus)
    {
        _internalBus = internalBus;
    }

    public void Subscribe(string hookName, Action<EventPayload> handler) => _internalBus.Subscribe(hookName, handler);
    public void Unsubscribe(string hookName, Action<EventPayload> handler) => _internalBus.Unsubscribe(hookName, handler);
}
