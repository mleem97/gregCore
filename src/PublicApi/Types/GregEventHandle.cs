namespace gregCore.PublicApi.Types;

public sealed class GregEventHandle : IDisposable
{
    private readonly Action _unsubscribe;
    private bool _disposed;

    public GregEventHandle(Action unsubscribe) => _unsubscribe = unsubscribe;

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
        _unsubscribe();
    }
}