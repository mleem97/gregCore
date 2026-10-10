using System;
using MelonLoader;
using gregCore.Bridge.CSharpScript;

namespace gregCore.Sdk.Language.Hosts;

public sealed class GregCSharpScriptHost : IGregLanguageHost
{
    public string HostId => "csharp";
    public string HostName => nameof(GregCSharpScriptHost);
    public bool IsActive { get; private set; }
    public string[] FileExtensions => new[] { ".cs" };

    public (bool available, string detail) IsDependencyAvailable()
    {
        if (!GregCSharpCompiler.IsAvailable)
            return (false, "Roslyn (Microsoft.CodeAnalysis.CSharp) not found in runtime");
        return (true, "Roslyn / C# script runtime");
    }

    public void Activate(string modsScriptsDir)
    {
        if (IsActive) return;
        GregCSharpScriptBridge.Initialize();
        IsActive = true;
    }

    public void OnUpdate(float dt)
    {
        if (!IsActive) return;
        GregCSharpScriptBridge.OnUpdate(dt);
    }

    public void OnSceneLoaded(string sceneName)
    {
        if (!IsActive) return;
        GregCSharpScriptBridge.OnSceneLoaded(sceneName);
    }

    public void Shutdown()
    {
        if (!IsActive) return;
        GregCSharpScriptBridge.Shutdown();
        IsActive = false;
    }
}
