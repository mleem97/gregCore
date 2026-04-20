/// <file-summary>
/// Schicht:      GameLayer
/// Zweck:        Minimaler DI-Container für das Framework.
/// Maintainer:   Kein Microsoft.Extensions.DI (zu schwer für IL2CPP).
/// </file-summary>

namespace gregCore.GameLayer.Bootstrap;

public sealed class GregServiceContainer : IDisposable
{
    private readonly Dictionary<string, object> _services = new();
    private static GregServiceContainer? _instance;

    public static GregServiceContainer? Instance => _instance;

    public GregServiceContainer()
    {
        _instance = this;
    }

    public void Register<T>(T instance) where T : notnull
        => _services[typeof(T).FullName!] = instance;

    public void Register<T>(string key, T instance) where T : notnull
        => _services[$"{typeof(T).FullName}:{key}"] = instance;

    public static T? Get<T>() where T : class
        => Instance?._services.TryGetValue(typeof(T).FullName!, out var s) == true ? (T)s : null;

    public T GetRequired<T>() where T : notnull
        => _services.TryGetValue(typeof(T).FullName!, out var s)
           ? (T)s
           : throw new GregInitException($"Service {typeof(T).Name} nicht registriert!");

    public void Dispose()
    {
        if (_instance == this) _instance = null;
        foreach (var s in _services.Values.OfType<IDisposable>())
            s.Dispose();
        _services.Clear();
    }
}
