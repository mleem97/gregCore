namespace gregCore.PublicApi;

public sealed class GregResourceRegistry : IDisposable
{
    private readonly List<IDisposable> _resources = new();
    private bool _disposed;

    public T Track<T>(T resource) where T : IDisposable
    {
        if (_disposed) throw new ObjectDisposedException(nameof(GregResourceRegistry));
        _resources.Add(resource ?? throw new ArgumentNullException(nameof(resource)));
        return resource;
    }

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
        foreach (var resource in Enumerable.Reverse(_resources.ToArray()))
        {
            try { resource.Dispose(); } catch { }
        }
        _resources.Clear();
    }
}
