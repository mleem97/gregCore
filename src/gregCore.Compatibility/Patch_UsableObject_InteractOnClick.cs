using System;
using HarmonyLib;
using Il2Cpp;
using Il2CppInterop.Runtime.InteropTypes;
using UnityEngine;
using gregCore.API;

namespace DataCenterModLoader;

/// <summary>
/// Detects when the local player picks up or drops a UsableObject (Server, Switch, etc.)
/// by comparing PlayerManager state before/after InteractOnClick.
/// Fires ObjectPickedUp / ObjectDropped events for native co-op-aware mods.
/// </summary>
[HarmonyPatch(typeof(UsableObject), nameof(UsableObject.InteractOnClick))]
internal static class Patch_UsableObject_InteractOnClick
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "S2386:Mutable fields should not be declared public", Justification = "Harmony cross-patch coordination flag: written and read by cooperating patch classes in the same assembly (see usages). Must stay mutable.")]
    internal static bool SuppressEvents = false;

    [ThreadStatic] private static int _prevNumObjects;
    [ThreadStatic] private static int _prevObjectInHand;

    private static string? _heldObjectId = null;
    private static byte _heldObjectType = 0;
    private static UsableObject? _heldObjectRef = null;
    private static bool _pickupFiredThisInteraction = false;

    internal static void Prefix(UsableObject __instance)
    {
        _pickupFiredThisInteraction = false;
        try
        {
            var pm = PlayerManager.instance;
            if (pm == null) return;
            _prevNumObjects = pm.numberOfObjectsInHand;
            _prevObjectInHand = (int)pm.objectInHand;
        }
        catch (Exception ex) { CrashLog.Log($"[WorldSync] InteractOnClick Prefix error: {ex.Message}"); }
    }

    internal static void Postfix(UsableObject? __instance)
    {
        if (SuppressEvents || __instance == null) return;
        try
        {
            var pm = PlayerManager.instance;
            if (pm == null) return;
            int newNumObjects = pm.numberOfObjectsInHand;
            int newObjectInHand = (int)pm.objectInHand;
            if (IsPickup(newNumObjects, newObjectInHand)) HandlePickup(__instance);
            else if (IsDrop(newNumObjects, newObjectInHand)) HandleDrop();
        }
        catch (Exception ex) { CrashLog.Log($"[WorldSync] InteractOnClick Postfix error: {ex.Message}"); }
    }

    private static bool IsPickup(int newNumObjects, int newObjectInHand)
    {
        return _prevNumObjects == 0 && newNumObjects > 0 && _prevObjectInHand == 0 && newObjectInHand != 0;
    }

    private static bool IsDrop(int newNumObjects, int newObjectInHand)
    {
        return _prevNumObjects > 0 && newNumObjects == 0 && _prevObjectInHand != 0 && newObjectInHand == 0;
    }

    private static void HandlePickup(UsableObject __instance)
    {
        var (objectId, objectType) = ResolvePickedIdentity(__instance);
        if (string.IsNullOrEmpty(objectId)) return;
        objectId = ResolveStablePickupId(__instance, objectId, objectType);
        if (_pickupFiredThisInteraction) return;
        _pickupFiredThisInteraction = true;
        _heldObjectId = objectId;
        _heldObjectType = objectType;
        _heldObjectRef = __instance;
        CrashLog.Log($"[WorldSync] Pickup tracked: '{objectId}' type={objectType}");
    }

    private static void HandleDrop()
    {
        if (string.IsNullOrEmpty(_heldObjectId)) return;
        CrashLog.Log($"[WorldSync] Drop tracked: '{_heldObjectId}' type={_heldObjectType}");
        ClearHeldObject();
    }

    private static (string objectId, byte objectType) ResolvePickedIdentity(UsableObject __instance)
    {
        var server = __instance.TryCast<Server>();
        if (server != null) return (server.ServerID ?? "", (byte)server.serverType);
        var netSwitch = __instance.TryCast<NetworkSwitch>();
        if (netSwitch != null) return (netSwitch.switchId ?? "", (byte)(int)__instance.objectInHandType);
        var patchPanel = __instance.TryCast<PatchPanel>();
        if (patchPanel != null) return ResolvePatchPanelIdentity(patchPanel, __instance);
        return ResolveFallbackIdentity(__instance);
    }

    private static (string objectId, byte objectType) ResolvePatchPanelIdentity(PatchPanel patchPanel, UsableObject __instance)
    {
        string objectId = patchPanel.patchPanelId ?? "";
        if (!string.IsNullOrEmpty(objectId)) return (objectId, (byte)(int)__instance.objectInHandType);
        string objName = StripCloneSuffix(patchPanel.gameObject?.name ?? "PatchPanel");
        objectId = GenerateDeterministicId(objName, patchPanel.transform.position);
        patchPanel.patchPanelId = objectId;
        CrashLog.Log($"[WorldSync] InteractOnClick: assigned patchPanelId '{objectId}' (position-based)");
        return (objectId, (byte)(int)__instance.objectInHandType);
    }

    private static (string objectId, byte objectType) ResolveFallbackIdentity(UsableObject __instance)
    {
        string objName = StripCloneSuffix(__instance.gameObject.name);
        var p = __instance.transform.position;
        int posHash = ((int)(p.x * 100)) ^ ((int)(p.y * 100) << 10) ^ ((int)(p.z * 100) << 20);
        return ($"{objName}_{posHash}", (byte)(int)__instance.objectInHandType);
    }

    private static string StripCloneSuffix(string name)
    {
        try
        {
            if (name.EndsWith("(Clone)")) name = name.Substring(0, name.Length - 7);
            return name;
        }
        catch { /* ignored: defensive best-effort (CONVENTIONS.md) */  return name; }
    }

    private static string ResolveStablePickupId(UsableObject __instance, string objectId, byte objectType)
    {
        int pickupRackUid = FindPickupRackUid(__instance, objectType);
        if (pickupRackUid <= 0) return objectId;
        try
        {
            var stableEntry = Patch_Rack_MarkPositionAsUsed.LookupInstalledObject(pickupRackUid, objectType);
            if (stableEntry.HasValue && !string.IsNullOrEmpty(stableEntry.Value.objectId) && stableEntry.Value.objectId != objectId)
            {
                CrashLog.Log($"[WorldSync] Pickup: resolved clone ID '{objectId}' → stable '{stableEntry.Value.objectId}' (rackUid={pickupRackUid})");
                objectId = stableEntry.Value.objectId;
                try { ApplyStableIdentity(__instance, objectType, objectId); } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */  }
            }
            Patch_Rack_MarkPositionAsUsed.RemoveInstalledObject(pickupRackUid);
        }
        catch { /* ignored: defensive best-effort (CONVENTIONS.md) */  }
        return objectId;
    }

    private static int FindPickupRackUid(UsableObject __instance, byte objectType)
    {
        try
        {
            if (IsServerType(__instance, objectType)) return CoalesceRackUid(__instance);
            if (IsSwitchType(__instance, objectType)) return CoalesceRackUid(__instance);
            if (IsPanelType(__instance, objectType)) return CoalesceRackUid(__instance);
        }
        catch { /* ignored: defensive best-effort (CONVENTIONS.md) */  }
        return -1;
    }

    private static bool IsServerType(UsableObject instance, byte objectType)
    {
        try { return objectType <= 3 && instance.TryCast<Server>() != null; } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */  return false; }
    }

    private static bool IsSwitchType(UsableObject instance, byte objectType)
    {
        try { return objectType == 4 && instance.TryCast<NetworkSwitch>() != null; } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */  return false; }
    }

    private static bool IsPanelType(UsableObject instance, byte objectType)
    {
        try { return objectType == 7 && instance.TryCast<PatchPanel>() != null; } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */  return false; }
    }

    // Prefer the live-linked rack position, otherwise the stored one.
    private static int CoalesceRackUid(UsableObject device)
    {
        try
        {
            var pos = device.currentRackPosition;
            if (pos != null && pos.rackPosGlobalUID > 0) return pos.rackPosGlobalUID;
            return device.rackPositionUID;
        }
        catch { /* ignored: defensive best-effort (CONVENTIONS.md) */  }
        return -1;
    }

    private static void ApplyStableIdentity(UsableObject __instance, byte objectType, string objectId)
    {
        if (objectType <= 3) ApplyServerIdentity(__instance, objectId);
        else if (objectType == 4) ApplySwitchIdentity(__instance, objectId);
        else if (objectType == 7) ApplyPanelIdentity(__instance, objectId);
    }

    private static void ApplyServerIdentity(UsableObject instance, string objectId)
    {
        try
        {
            var srv = instance.TryCast<Server>();
            if (srv != null) srv.ServerID = objectId;
        }
        catch { /* ignored: defensive best-effort (CONVENTIONS.md) */  }
    }

    private static void ApplySwitchIdentity(UsableObject instance, string objectId)
    {
        try
        {
            var sw = instance.TryCast<NetworkSwitch>();
            if (sw != null) sw.switchId = objectId;
        }
        catch { /* ignored: defensive best-effort (CONVENTIONS.md) */  }
    }

    private static void ApplyPanelIdentity(UsableObject instance, string objectId)
    {
        try
        {
            var pp = instance.TryCast<PatchPanel>();
            if (pp != null) pp.patchPanelId = objectId;
        }
        catch { /* ignored: defensive best-effort (CONVENTIONS.md) */  }
    }

    /// <summary>
    /// Generates a deterministic object ID based on position instead of Unity instance ID.
    /// </summary>
    internal static string GenerateDeterministicId(string baseName, UnityEngine.Vector3 pos)
    {
        int px = Mathf.RoundToInt(pos.x * 100);
        int py = Mathf.RoundToInt(pos.y * 100);
        int pz = Mathf.RoundToInt(pos.z * 100);
        uint hash = (uint)(px * 73856093 ^ py * 19349663 ^ pz * 83492791);
        return $"{baseName}_{hash}";
    }

    /// <summary>
    /// Called from remote world actions to suppress local event firing
    /// </summary>
    internal static void SetHeldObject(string? objectId, byte objectType, UsableObject? obj)
    {
        _heldObjectId = objectId;
        _heldObjectType = objectType;
        _heldObjectRef = obj;
    }

    internal static void ClearHeldObject()
    {
        _heldObjectId = null;
        _heldObjectType = 0;
        _heldObjectRef = null;
    }

    internal static string? GetHeldObjectId() => _heldObjectId;
    internal static byte GetHeldObjectType() => _heldObjectType;
}
