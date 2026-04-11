using System;
using MelonLoader;
using UnityEngine;
using Il2Cpp;

namespace gregSdk.Services;

/// <summary>
/// Bridge service for interacting with in-game Rack objects.
/// </summary>
public static class GregRackInteractionService
{
    private static Rack GetRackInstance(string rackName)
    {
        var go = GameObject.Find(rackName);
        if (go == null) return null;
        return go.GetComponent<Rack>();
    }

    public static bool IsPositionAvailable(string rackName, int index, int sizeInU)
    {
        return GetRackInstance(rackName)?.IsPositionAvailable(index, sizeInU) ?? false;
    }

    public static void MarkPositionAsUsed(string rackName, int index, int sizeInU)
    {
        GetRackInstance(rackName)?.MarkPositionAsUsed(index, sizeInU);
    }

    public static void MarkPositionAsUnused(string rackName, int index, int sizeInU)
    {
        GetRackInstance(rackName)?.MarkPositionAsUnused(index, sizeInU);
    }

    public static void ButtonDisablePositionsInRack(string rackName)
    {
        GetRackInstance(rackName)?.ButtonDisablePositionsInRack();
    }

    public static void ButtonUnmountRack(string rackName)
    {
        GetRackInstance(rackName)?.ButtonUnmountRack();
    }
}
