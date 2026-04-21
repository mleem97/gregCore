using System;
using MelonLoader;

namespace gregCore.Sdk.Language.Hosts;

public sealed class GregCSharpScriptHost : IGregLanguageHost
{
    public Language Language => Language.CSharpScript;
    public string HostName => nameof(GregCSharpScriptHost);
    public bool IsActive { get; private set; }

    public bool IsDependencyAvailable(out string detail)
    {
        var roslynType = Type.GetType("Microsoft.CodeAnalysis.CSharp.CSharpCompilation, Microsoft.CodeAnalysis.CSharp");
        if (roslynType == null)
        {
            detail = "Roslyn (Microsoft.CodeAnalysis.CSharp) not found";
            return false;
        }

        detail = "Roslyn / C# script runtime";
        return true;
    }

    public void Activate(string modsScriptsDir)
    {
        if (IsActive)
        {
            return;
        }

        MelonLogger.Msg("[gregCore] GregCSharpScriptHost initialized (runtime execution layer is [UNVERIFIED]).");
        IsActive = true;
    }

    public void OnUpdate(float dt)
    {
    }

    public void OnSceneLoaded(string sceneName)
    {
    }

    public void Shutdown()
    {
        IsActive = false;
    }
}
