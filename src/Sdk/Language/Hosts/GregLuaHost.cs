using System;
using MoonSharp.Interpreter;
using gregCore.Bridge.LuaFFI;

namespace gregCore.Sdk.Language.Hosts;

public sealed class GregLuaHost : IGregLanguageHost
{
    public Language Language => Language.Lua;
    public string HostName => nameof(GregLuaHost);
    public bool IsActive { get; private set; }

    public bool IsDependencyAvailable(out string detail)
    {
        detail = "MoonSharp 2.0.0";
        return typeof(Script) != null;
    }

    public void Activate(string modsScriptsDir)
    {
        if (IsActive)
        {
            return;
        }

        LuaFFIBridge.Initialize();
        IsActive = true;
    }

    public void OnUpdate(float dt)
    {
        if (!IsActive)
        {
            return;
        }

        LuaFFIBridge.OnUpdate(dt);
    }
 
    public void OnGUI()
    {
        if (!IsActive)
        {
            return;
        }
 
        LuaFFIBridge.OnGUI();
    }

    public void OnSceneLoaded(string sceneName)
    {
        if (!IsActive)
        {
            return;
        }

        LuaFFIBridge.OnSceneLoaded(sceneName);
    }

    public void Shutdown()
    {
        if (!IsActive)
        {
            return;
        }

        LuaFFIBridge.Shutdown();
        IsActive = false;
    }
}
