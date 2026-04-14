using System;
using UnityEngine;
using Il2CppInterop.Runtime;
using Il2CppInterop.Runtime.InteropTypes;

namespace greg.Core.UI;

public static class Il2CppSafe
{
    public static T AddComponent<T>(GameObject go) where T : Component
    {
        return go.AddComponent(Il2CppType.Of<T>()).TryCast<T>();
    }

    public static Component AddComponent(GameObject go, Type componentType)
    {
        try
        {
            return go.AddComponent(Il2CppType.From(componentType));
        }
        catch (Exception ex)
        {
            MelonLoader.MelonLogger.Warning($"[Il2CppSafe] AddComponent failed for {componentType?.Name}: {ex.Message}");
            return null;
        }
    }

    public static T GetComponent<T>(GameObject go) where T : Il2CppObjectBase
    {
        try
        {
            var comp = go?.GetComponent(Il2CppType.Of<T>());
            return comp?.TryCast<T>();
        }
        catch
        {
            return null;
        }
    }

    public static T GetComponentInChildren<T>(GameObject go) where T : Il2CppObjectBase
    {
        try
        {
            var comp = go?.GetComponentInChildren(Il2CppType.Of<T>());
            return comp?.TryCast<T>();
        }
        catch
        {
            return null;
        }
    }
}
