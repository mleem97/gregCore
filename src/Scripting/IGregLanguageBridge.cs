using System.Collections.Generic;

namespace greg.Core.Scripting;

/// <summary>
/// Plugin-layer language bridge abstraction for script/native hosts.
/// </summary>
public interface iGregLanguageBridge
{
    string LanguageName { get; }

    IReadOnlyList<string> ScriptExtensions { get; }

    void Initialize();

    int LoadScripts();

    IReadOnlyList<gregRuntimeUnit> GetRuntimeUnits();

    bool SetUnitEnabled(string unitId, bool enabled);

    int ReloadEnabledUnits();

    void OnSceneLoaded(string sceneName);

    void OnUpdate(float deltaTime);

    void OnGui();

    void Shutdown();
}

