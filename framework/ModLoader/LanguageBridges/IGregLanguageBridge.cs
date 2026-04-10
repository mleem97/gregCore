using System.Collections.Generic;

namespace DataCenterModLoader.LanguageBridges;

/// <summary>
/// Plugin-layer language bridge abstraction for script/native hosts.
/// </summary>
public interface IGregLanguageBridge
{
    string LanguageName { get; }

    IReadOnlyList<string> ScriptExtensions { get; }

    void Initialize();

    int LoadScripts();

    IReadOnlyList<GregRuntimeUnit> GetRuntimeUnits();

    bool SetUnitEnabled(string unitId, bool enabled);

    int ReloadEnabledUnits();

    void OnSceneLoaded(string sceneName);

    void OnUpdate(float deltaTime);

    void Shutdown();
}
