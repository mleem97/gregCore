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

    public IDisposable On(string hookName, Action<EventPayload> handler)
    {
        _internalBus.Subscribe(hookName, handler);
        return new Subscription(() => _internalBus.Unsubscribe(hookName, handler));
    }
    public IDisposable Once(string hookName, Action<EventPayload> handler)
    {
        Action<EventPayload>? wrapper = null;
        wrapper = payload => { try { handler(payload); } finally { if (wrapper != null) _internalBus.Unsubscribe(hookName, wrapper); } };
        return On(hookName, wrapper);
    }
    public void Subscribe(string hookName, Action<EventPayload> handler) => _ = On(hookName, handler);
    public void Unsubscribe(string hookName, Action<EventPayload> handler) => _internalBus.Unsubscribe(hookName, handler);

    private sealed class Subscription : IDisposable
    {
        private Action? _dispose;
        public Subscription(Action dispose) => _dispose = dispose;
        public void Dispose() => Interlocked.Exchange(ref _dispose, null)?.Invoke();
    }
}
