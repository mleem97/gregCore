using System;
using System.Collections.Generic;
using UnityEngine;
using Il2Cpp;

namespace greg.Sdk.Services;

/// <summary>
/// Skeleton for first-class custom content support in gregCore.
/// Will eventually handle asset injection for Racks, Servers, and Cables.
/// </summary>
public static class GregContentRegistry
{
    private static readonly Dictionary<string, GameObject> _registeredContent = new();

    public static void RegisterContent(string id, GameObject prefab)
    {
        if (prefab == null) return;
        _registeredContent[id] = prefab;
        MelonLoader.MelonLogger.Msg($"[gregCore] Content registered: {id}");
    }

    public static GameObject GetContent(string id)
    {
        return _registeredContent.TryGetValue(id, out var prefab) ? prefab : null;
    }
}

