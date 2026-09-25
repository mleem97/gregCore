using System;
using System.Collections.Generic;
using HarmonyLib;
using Il2Cpp;
using UnityEngine;
using gregCore.API;

namespace DataCenterModLoader;

[HarmonyPatch(typeof(Rack), nameof(Rack.MarkPositionAsUsed))]
internal static class Patch_Rack_MarkPositionAsUsed
{
    internal static bool SuppressEvents = false;
    internal static Dictionary<int, (string objectId, byte objectType)> PendingRestores = new();
    internal static Dictionary<int, (string objectId, byte objectType)> InstalledObjects = new();

    // Backward-compat property — GameAPI.cs still sets PendingCloneRestore = (...).
    internal static (string objectId, byte objectType, int rackPosUid)? PendingCloneRestore
    {
        get => null;
        set
        {
            if (value.HasValue && value.Value.rackPosUid > 0)
            {
                PendingRestores[value.Value.rackPosUid] = (value.Value.objectId, value.Value.objectType);
                InstalledObjects[value.Value.rackPosUid] = (value.Value.objectId, value.Value.objectType);
            }
        }
    }

    // Dedup guard — Il2Cpp fires every Harmony postfix twice for the same call.
    private static int _lastFiredUid = -1;
    private static long _lastFiredTick = 0;

    /// <summary>
    /// Consume and remove the pending restore entry for the given rack position.
    /// Returns null if no matching entry exists.
    /// </summary>
    internal static (string objectId, byte objectType)? ConsumePendingRestore(int rackPosUid, byte expectedType)
    {
        if (rackPosUid <= 0) return null;
        if (!PendingRestores.TryGetValue(rackPosUid, out var entry)) return null;
        if (!MatchesExpectedType(entry.objectType, expectedType)) return null;
        PendingRestores.Remove(rackPosUid);
        return entry;
    }

    /// <summary>
    /// Look up the stable ID for the object installed at a rack position.
    /// Does NOT remove the entry — this is a persistent fallback.
    /// </summary>
    internal static (string objectId, byte objectType)? LookupInstalledObject(int rackPosUid, byte expectedType)
    {
        if (rackPosUid <= 0) return null;
        if (!InstalledObjects.TryGetValue(rackPosUid, out var entry)) return null;
        if (!MatchesExpectedType(entry.objectType, expectedType)) return null;
        return entry;
    }

    private static bool MatchesExpectedType(byte actual, byte expected)
    {
        if (expected <= 3) return actual <= 3;
        if (expected == 4) return actual == 4;
        if (expected == 7) return actual == 7;
        return false;
    }

    /// <summary>
    /// Remove an entry from both dictionaries. Call on uninstall.
    /// </summary>
    internal static void RemoveInstalledObject(int rackPosUid)
    {
        InstalledObjects.Remove(rackPosUid);
        PendingRestores.Remove(rackPosUid);
    }

    internal static void Postfix(Rack? __instance, int __0, int __1)
    {
        try
        {
            if (SuppressEvents || __instance == null) return;
            var rackPos = ResolveRackPosition(__instance, __0);
            if (rackPos == null) return;
            int rackPosUid = rackPos.rackPosGlobalUID;
            if (!IdentifyInstalledObject(rackPosUid, out string objectId, out byte objectType)) return;
            if (string.IsNullOrEmpty(objectId)) return;
            if (rackPosUid < 0) return;
            if (IsDuplicateFire(rackPosUid)) return;
            RecordAndFire(rackPosUid, objectId, objectType, __0, __1);
        }
        catch (Exception ex) { EventDispatcher.LogError($"MarkPositionAsUsed: {ex.Message}"); }
    }

    private static RackPosition? ResolveRackPosition(Rack rack, int index)
    {
        try
        {
            var positions = rack.positions;
            if (positions == null || index < 0 || index >= positions.Count) return null;
            return positions[index];
        }
        catch { return null; }
    }

    private static bool IdentifyInstalledObject(int rackPosUid, out string objectId, out byte objectType)
    {
        if (TryIdentifyHeldObject(out objectId, out objectType)) return true;
        if (TryFindInstalled(rackPosUid, out objectId, out objectType)) return true;
        objectId = null!;
        objectType = 0;
        return false;
    }

    private static bool TryFindInstalled(int rackPosUid, out string objectId, out byte objectType)
    {
        if (TryFindInServers(rackPosUid, out objectId, out objectType)) return true;
        if (TryFindInSwitches(rackPosUid, out objectId, out objectType)) return true;
        if (TryFindInPatchPanels(rackPosUid, out objectId, out objectType)) return true;
        objectId = null!;
        objectType = 0;
        return false;
    }

    private static bool TryIdentifyHeldObject(out string objectId, out byte objectType)
    {
        string? heldId = Patch_UsableObject_InteractOnClick.GetHeldObjectId();
        byte heldType = Patch_UsableObject_InteractOnClick.GetHeldObjectType();
        if (string.IsNullOrEmpty(heldId))
        {
            objectId = null!;
            objectType = 0;
            return false;
        }
        objectId = heldId;
        objectType = heldType;
        return true;
    }

    private static bool TryFindInServers(int rackPosUid, out string objectId, out byte objectType)
    {
        var allServers = UnityEngine.Object.FindObjectsOfType<Server>();
        foreach (var srv in allServers)
        {
            try
            {
                if ((srv.currentRackPosition != null && srv.currentRackPosition.rackPosGlobalUID == rackPosUid)
                    || srv.rackPositionUID == rackPosUid)
                {
                    objectId = srv.ServerID ?? "";
                    objectType = (byte)srv.serverType;
                    return true;
                }
            }
            catch { }
        }
        objectId = null!;
        objectType = 0;
        return false;
    }

    private static bool TryFindInSwitches(int rackPosUid, out string objectId, out byte objectType)
    {
        foreach (var sw in UnityEngine.Object.FindObjectsOfType<NetworkSwitch>())
        {
            try
            {
                if ((sw.currentRackPosition != null && sw.currentRackPosition.rackPosGlobalUID == rackPosUid)
                    || sw.rackPositionUID == rackPosUid)
                {
                    objectId = sw.switchId ?? "";
                    objectType = 4;
                    return true;
                }
            }
            catch { }
        }
        objectId = null!;
        objectType = 0;
        return false;
    }

    private static bool TryFindInPatchPanels(int rackPosUid, out string objectId, out byte objectType)
    {
        foreach (var pp in UnityEngine.Object.FindObjectsOfType<PatchPanel>())
        {
            try
            {
                if ((pp.currentRackPosition != null && pp.currentRackPosition.rackPosGlobalUID == rackPosUid)
                    || pp.rackPositionUID == rackPosUid)
                {
                    objectId = pp.patchPanelId ?? "";
                    objectType = 7;
                    return true;
                }
            }
            catch { }
        }
        objectId = null!;
        objectType = 0;
        return false;
    }

    private static bool IsDuplicateFire(int rackPosUid)
    {
        long now = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        if (rackPosUid == _lastFiredUid && (now - _lastFiredTick) < 200)
            return true;
        _lastFiredUid = rackPosUid;
        _lastFiredTick = now;
        return false;
    }

    private static void RecordAndFire(int rackPosUid, string objectId, byte objectType, int index, int sizeInU)
    {
        PendingRestores[rackPosUid] = (objectId, objectType);
        InstalledObjects[rackPosUid] = (objectId, objectType);
        CrashLog.Log($"[WorldSync] MarkPositionAsUsed: '{objectId}' type={objectType} at rackUid={rackPosUid} (index={index}, sizeInU={sizeInU}) → firing event");
        EventDispatcher.FireServerInstalled(objectId, objectType, rackPosUid);
        CarryStateMonitor.SuppressNextDrop();
        Patch_UsableObject_InteractOnClick.ClearHeldObject();
    }
}
