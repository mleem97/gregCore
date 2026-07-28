using System;

namespace gregCore.Abstractions;

public enum GregLogLevel
{
    Trace,
    Debug,
    Information,
    Warning,
    Error,
    Critical
}

public interface IGregLogSink
{
    void Write(GregLogLevel level, string message, Exception? exception = null);
}

public sealed class LoaderRuntimeInfo
{
    public string LoaderId { get; set; } = string.Empty;
    public string LoaderVersion { get; set; } = string.Empty;
    public string RuntimeId { get; set; } = string.Empty;
    public string Platform { get; set; } = string.Empty;
    public string Architecture { get; set; } = string.Empty;
    public string GameRootDirectory { get; set; } = string.Empty;
    public string ModsDirectory { get; set; } = string.Empty;
}

/// <summary>
/// Lifecycle surface implemented by loader-specific hosts. Managed framework
/// services depend on this contract rather than MelonLoader or BepInEx types.
/// </summary>
public interface ILoaderHost
{
    LoaderRuntimeInfo Runtime { get; }
    IGregLogSink Log { get; }

    void RegisterUpdate(Action<float> callback);
    void RegisterSceneLoaded(Action<int, string> callback);
    void RegisterShutdown(Action callback);
}

public interface ICompatibilityContext
{
    string ProfileId { get; }
    bool SafeMode { get; }
    bool Supports(string capability);
}
