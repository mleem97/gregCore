/// <file-summary>
/// Layer:       PublicApi
/// Purpose:     Contract for DLL plugins loaded by GregCore itself.
/// Maintainer:  Managed counterpart to Il2Cpp.IModPlugin (OnModLoad/OnModUnload).
/// </file-summary>

namespace gregCore.PublicApi;

/// <summary>
/// Host that GregCore passes to each native plugin on load.
/// </summary>
public interface IGregNativeHost
{
    IGregLogger Logger { get; }
    string ModFolderPath { get; }
    void PublishEvent(string hookName, IReadOnlyDictionary<string, object> data);
}

/// <summary>
/// Contract for a GregCore-native DLL plugin.
/// Implementation must have a parameterless constructor.
/// </summary>
public interface IGregNativePlugin : IDisposable
{
    string Id { get; }
    string Name { get; }
    string Version { get; }
    void OnNativeLoad(IGregNativeHost host);
    void OnNativeUnload();
}
