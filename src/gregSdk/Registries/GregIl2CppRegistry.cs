using System;
using MelonLoader;
using UnityEngine;

namespace greg.Sdk.Registries;

/// <summary>
/// Registry for custom IL2CPP types to bypass [AddComponent] limits.
/// </summary>
public static class GregIl2CppRegistry
{
    /// <summary>
    /// Registers a custom type in IL2CPP.
    /// Note: This must be called in OnInitializeMelon or earlier.
    /// </summary>
    public static void RegisterType<T>() where T : MonoBehaviour
    {
        try
        {
            // MelonLoader provides RegisterTypeInIl2Cpp for this
            // We're just providing a clean SDK wrapper
            Il2CppInterop.Runtime.Injection.ClassInjector.RegisterTypeInIl2Cpp<T>();
            MelonLogger.Msg($"[GregIl2CppRegistry] Registered type: {typeof(T).FullName}");
        }
        catch (Exception ex)
        {
            MelonLogger.Error($"[GregIl2CppRegistry] Failed to register type {typeof(T).FullName}: {ex.Message}");
        }
    }

    /// <summary>
    /// Safely adds a component to a GameObject, ensuring it's registered.
    /// </summary>
    public static T AddCustomComponent<T>(GameObject go) where T : MonoBehaviour
    {
        if (go == null) return null;
        return go.AddComponent<T>();
    }
}
