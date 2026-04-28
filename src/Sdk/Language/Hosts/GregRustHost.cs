using System;
using gregCore.Bridge.RustFFI;

namespace gregCore.Sdk.Language.Hosts;

public sealed class GregRustHost : IGregLanguageHost
{
    public Language Language => Language.Rust;
    public string HostName => nameof(GregRustHost);
    public bool IsActive { get; private set; }

    public bool IsDependencyAvailable(out string detail)
    {
        detail = "Rust-Bridge (FFI/ABI-Layer)";
        return typeof(RustFFIBridge) != null;
    }

    public void Activate(string modsScriptsDir)
    {
        if (IsActive)
        {
            return;
        }

        RustFFIBridge.Initialize();
        IsActive = true;
    }

    public void OnUpdate(float dt)
    {
        if (!IsActive)
        {
            return;
        }

        RustFFIBridge.OnUpdate(dt);
    }

    public void OnSceneLoaded(string sceneName)
    {
        if (!IsActive)
        {
            return;
        }

        RustFFIBridge.OnSceneLoaded(sceneName);
    }

    public void Shutdown()
    {
        if (!IsActive)
        {
            return;
        }

        RustFFIBridge.Shutdown();
        IsActive = false;
    }
}
