using System;
using gregCore.Bridge.PythonFFI;

namespace gregCore.Sdk.Language.Hosts;

public sealed class GregPythonHost : IGregLanguageHost
{
    public string HostId => "python";
    public string HostName => nameof(GregPythonHost);
    public bool IsActive { get; private set; }
    public string[] FileExtensions => new[] { ".py" };

    public bool IsDependencyAvailable(out string detail)
    {
        var pythonType = Type.GetType("Python.Runtime.PythonEngine, Python.Runtime");
        if (pythonType == null)
        {
            detail = "Python.Runtime.dll missing";
            return false;
        }

        detail = "Python-Host-Bindings";
        return true;
    }

    public void Activate(string modsScriptsDir)
    {
        if (IsActive) return;

        PythonFFIBridge.Initialize();
        IsActive = true;
    }

    public void OnUpdate(float dt)
    {
        if (!IsActive) return;
        PythonFFIBridge.OnUpdate(dt);
    }

    public void OnSceneLoaded(string sceneName)
    {
        if (!IsActive) return;
        PythonFFIBridge.OnSceneLoaded(sceneName);
    }

    public void Shutdown()
    {
        if (!IsActive) return;
        PythonFFIBridge.Shutdown();
        IsActive = false;
    }
}
