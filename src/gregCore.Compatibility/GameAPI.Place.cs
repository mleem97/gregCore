using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Il2Cpp;
using MelonLoader;
using UnityEngine;
using gregCore.API;

namespace DataCenterModLoader;

public partial class GameAPIManager
{
    int WorldPlaceInRackImpl(IntPtr id, uint idLen, int rackUid)
    {
        try
        {
            string objId = ReadUtf8(id, idLen);
            CrashLog.Log($"[WorldSync] PlaceInRack: id={objId}, uid={rackUid}");

            if (!TryResolvePlaceTarget(objId, out ulong handle, out var comp)) return 0;
            if (!TryResolvePlaceRack(objId, rackUid, out var rackPos, out var rack)) return 0;

            if (!comp.gameObject.activeSelf)
                comp.gameObject.SetActive(true);

            var ptr = new IntPtr((long)handle);
            InstallIntoRack(objId, comp, rackPos, rack, ptr, rackUid);

            CrashLog.Log($"[WorldSync] PlaceInRack: '{objId}' installed at uid={rackUid} OK");
            return 1;
        }
        catch (Exception ex)
        {
            Patch_Rack_MarkPositionAsUsed.SuppressEvents = false;
            CrashLog.LogException("WorldPlaceInRackImpl", ex);
            return 0;
        }
    }

    private bool TryResolvePlaceTarget(string objId, out ulong handle, out UnityEngine.Component comp)
    {
        handle = FindHandleByStableId(objId);
        comp = null;
        if (handle == 0)
        {
            CrashLog.Log($"[WorldSync] PlaceInRack: '{objId}' not found");
            return false;
        }
        comp = ResolveComponent(handle);
        if (comp == null)
        {
            CrashLog.Log($"[WorldSync] PlaceInRack: handle for '{objId}' resolved to null");
            return false;
        }
        return true;
    }

    private bool TryResolvePlaceRack(string objId, int rackUid, out Il2Cpp.RackPosition rackPos, out Il2Cpp.Rack rack)
    {
        rackPos = FindRackPosition(rackUid);
        rack = null;
        if (rackPos == null)
        {
            CrashLog.Log($"[WorldSync] PlaceInRack: rack uid={rackUid} not found");
            return false;
        }
        rack = rackPos.rack;
        if (rack == null)
        {
            CrashLog.Log($"[WorldSync] PlaceInRack: rack uid={rackUid} has no parent Rack");
            return false;
        }
        return true;
    }

    private void InstallIntoRack(string objId, UnityEngine.Component comp,
        Il2Cpp.RackPosition rackPos, Il2Cpp.Rack rack, IntPtr ptr, int rackUid)
    {
        byte guessedType = GuessRackObjectType(ptr);
        PreserveRackInstallId(ptr, guessedType, rackUid);

        int sizeInU = RackInstallBookkeeping(ptr, rackPos, guessedType, "PlaceInRack");

        comp.transform.SetParent(rackPos.transform, false);
        comp.transform.localPosition = UnityEngine.Vector3.zero;
        comp.transform.localRotation = UnityEngine.Quaternion.identity;

        FreezeRackObjectPhysics(comp);

        Patch_Rack_MarkPositionAsUsed.SuppressEvents = true;
        try { rack.MarkPositionAsUsed(rackPos.positionIndex, sizeInU); }
        catch (Exception ex)
        {
            CrashLog.Log($"[WorldSync] PlaceInRack: MarkPositionAsUsed failed: {ex.Message}");
        }
        finally { Patch_Rack_MarkPositionAsUsed.SuppressEvents = false; }
    }

    private static byte GuessRackObjectType(IntPtr ptr)
    {
        byte guessedType = 0;
        try { var s = new Il2Cpp.Server(ptr); if (!string.IsNullOrEmpty(s.ServerID)) guessedType = (byte)s.serverType; } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
        if (guessedType == 0)
        {
            try { var sw = new Il2Cpp.NetworkSwitch(ptr); if (!string.IsNullOrEmpty(sw.switchId)) guessedType = 4; } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
        }
        if (guessedType == 0)
        {
            try { var pp = new Il2Cpp.PatchPanel(ptr); if (!string.IsNullOrEmpty(pp.patchPanelId)) guessedType = 7; } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
        }
        return guessedType;
    }

    private static void PreserveRackInstallId(IntPtr ptr, byte guessedType, int rackUid)
    {
        // Preserve the object ID through InsertedInRack callbacks
        string preserveId = ReadRackInstallPreserveId(ptr, guessedType);
        if (!string.IsNullOrEmpty(preserveId))
        {
            Patch_Rack_MarkPositionAsUsed.PendingCloneRestore = (preserveId, guessedType, rackUid);
        }
    }

    private static string ReadRackInstallPreserveId(IntPtr ptr, byte guessedType)
    {
        if (IsServerRackType(guessedType))
            return ReadServerPreserveId(ptr);
        if (guessedType == 4)
            return ReadSwitchPreserveId(ptr);
        if (guessedType == 7)
            return ReadPatchPanelPreserveId(ptr);
        return "";
    }

    private static bool IsServerRackType(byte guessedType)
    {
        return guessedType == 0 || guessedType == 1 || guessedType == 2 || guessedType == 3;
    }

    private static string ReadServerPreserveId(IntPtr ptr)
    {
        try { var srv = new Il2Cpp.Server(ptr); return srv.ServerID ?? ""; } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
        return "";
    }

    private static string ReadSwitchPreserveId(IntPtr ptr)
    {
        try { var sw = new Il2Cpp.NetworkSwitch(ptr); return sw.switchId ?? ""; } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
        return "";
    }

    private static string ReadPatchPanelPreserveId(IntPtr ptr)
    {
        try { var pp = new Il2Cpp.PatchPanel(ptr); return pp.patchPanelId ?? ""; } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
        return "";
    }

    private static void FreezeRackObjectPhysics(UnityEngine.Component comp)
    {
        try
        {
            var rb = comp.GetComponent<UnityEngine.Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = true;
                rb.velocity = UnityEngine.Vector3.zero;
                rb.angularVelocity = UnityEngine.Vector3.zero;
            }
        }
        catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
    }

    int WorldRemoveFromRackImpl(IntPtr id, uint idLen)
    {
        try
        {
            string objId = ReadUtf8(id, idLen);
            CrashLog.Log($"[WorldSync] RemoveFromRack: id={objId}");

            // ── 1. Find the object ──────────────────────────────────────────
            ulong handle = FindHandleByStableId(objId);
            if (handle == 0)
            {
                CrashLog.Log($"[WorldSync] RemoveFromRack: '{objId}' not found");
                return 0;
            }
            var comp = ResolveComponent(handle);
            if (comp == null) return 0;

            var ptr = new IntPtr((long)handle);
            byte guessedType = GuessRackObjectType(ptr);
            ClearInstalledObjectTracking(ptr, guessedType);

            RackUninstallBookkeeping(ptr, guessedType, "RemoveFromRack");

            ReparentToWorldParent(comp);
            ReleaseRemovedPhysics(comp);

            CrashLog.Log($"[WorldSync] RemoveFromRack: '{objId}' removed OK");
            return 1;
        }
        catch (Exception ex)
        {
            CrashLog.LogException("WorldRemoveFromRackImpl", ex);
            return 0;
        }
    }

    private static void ClearInstalledObjectTracking(IntPtr ptr, byte guessedType)
    {
        // Clear installed objects tracking before uninstall bookkeeping
        try
        {
            int removeUid = ReadRemoveRackUid(ptr, guessedType);
            if (removeUid > 0) Patch_Rack_MarkPositionAsUsed.RemoveInstalledObject(removeUid);
        }
        catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
    }

    private static int ReadRemoveRackUid(IntPtr ptr, byte guessedType)
    {
        if (IsServerRackType(guessedType))
            return ReadServerRemoveUid(ptr);
        if (guessedType == 4)
            return ReadSwitchRemoveUid(ptr);
        if (guessedType == 7)
            return ReadPatchPanelRemoveUid(ptr);
        return -1;
    }

    private static int ReadServerRemoveUid(IntPtr ptr)
    {
        try { var s2 = new Il2Cpp.Server(ptr); return s2.currentRackPosition != null ? s2.currentRackPosition.rackPosGlobalUID : s2.rackPositionUID; } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
        return -1;
    }

    private static int ReadSwitchRemoveUid(IntPtr ptr)
    {
        try { var sw2 = new Il2Cpp.NetworkSwitch(ptr); return sw2.currentRackPosition != null ? sw2.currentRackPosition.rackPosGlobalUID : sw2.rackPositionUID; } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
        return -1;
    }

    private static int ReadPatchPanelRemoveUid(IntPtr ptr)
    {
        try { var pp2 = new Il2Cpp.PatchPanel(ptr); return pp2.currentRackPosition != null ? pp2.currentRackPosition.rackPosGlobalUID : pp2.rackPositionUID; } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
        return -1;
    }

    private static void ReparentToWorldParent(UnityEngine.Component comp)
    {
        // ── 3. Reparent to world ────────────────────────────────────────
        try
        {
            var mgr = Il2Cpp.MainGameManager.instance;
            if (mgr != null && mgr.parentUsableObjects != null)
                comp.transform.SetParent(mgr.parentUsableObjects, true);
        }
        catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
    }

    private static void ReleaseRemovedPhysics(UnityEngine.Component comp)
    {
        // ── 4. Re-enable physics ────────────────────────────────────────
        try
        {
            var rb = comp.GetComponent<UnityEngine.Rigidbody>();
            if (rb != null)
            {
                rb.velocity = UnityEngine.Vector3.zero;
                rb.angularVelocity = UnityEngine.Vector3.zero;
                UnityEngine.Physics.SyncTransforms();
                rb.isKinematic = false;
                rb.useGravity = true;
                rb.WakeUp();
            }
        }
        catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
    }
}
