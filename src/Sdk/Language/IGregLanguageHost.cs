using System;

namespace gregCore.Sdk.Language;

public interface IGregLanguageHost
{
    Language Language { get; }
    string HostName { get; }
    bool IsActive { get; }
    bool IsDependencyAvailable(out string detail);
    void Activate(string modsScriptsDir);
    void OnUpdate(float dt);
    void OnGUI();
    void OnSceneLoaded(string sceneName);
    void Shutdown();
}
