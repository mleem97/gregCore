/// <file-summary>
/// Schicht:      PublicApi
/// Zweck:        Vertrag für DLL-Plugins, die GregCore selbst lädt.
/// Maintainer:   Managed Gegenstück zu Il2Cpp.IModPlugin (OnModLoad/OnModUnload).
/// </file-summary>

namespace gregCore.PublicApi;

/// <summary>
/// Host, den GregCore jedem nativen Plugin beim Laden übergibt.
/// </summary>
public interface IGregNativeHost
{
    IGregLogger Logger { get; }
    string ModFolderPath { get; }
    void PublishEvent(string hookName, IReadOnlyDictionary<string, object> data);
}

/// <summary>
/// Vertrag für ein GregCore-natives DLL-Plugin.
/// Implementierung muss einen parameterlosen Konstruktor besitzen.
/// </summary>
public interface IGregNativePlugin : IDisposable
{
    string Id { get; }
    string Name { get; }
    string Version { get; }
    void OnNativeLoad(IGregNativeHost host);
    void OnNativeUnload();
}
