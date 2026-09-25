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
    ulong ObjFindByIdImpl(byte typeId, ushort fieldId, IntPtr id, uint idLen)
    {
        try
        {
            string targetId = ReadFindTargetId(id, idLen);
            if (string.IsNullOrEmpty(targetId)) return 0;

            switch (typeId)
            {
                case 0: // Server
                    return FindServerById(targetId, fieldId);
                case 4: // NetworkSwitch
                    return FindSwitchById(targetId, fieldId);
                case 7: // PatchPanel
                    return FindPatchPanelById(targetId, fieldId);
                default:
                    return 0;
            }
        }
        catch (Exception ex)
        {
            CrashLog.LogException("ObjFindByIdImpl", ex);
            return 0;
        }
    }

    private static string ReadFindTargetId(IntPtr id, uint idLen)
    {
        if (id == IntPtr.Zero || idLen == 0) return "";
        byte[] buf = new byte[idLen];
        Marshal.Copy(id, buf, 0, (int)idLen);
        int end = Array.IndexOf(buf, (byte)0);
        if (end < 0) end = (int)idLen;
        return System.Text.Encoding.UTF8.GetString(buf, 0, end).Trim();
    }

    private static ulong FindServerById(string targetId, ushort fieldId)
    {
        foreach (var srv in UnityEngine.Resources.FindObjectsOfTypeAll<Server>())
        {
            try
            {
                if (srv.gameObject.scene.name == null) continue;
                string val = ReadServerFindField(srv, fieldId);
                if (val == targetId) return (ulong)srv.Pointer.ToInt64();
            }
            catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
        }
        // Lookup failed — dump all known servers so we can see if ID mismatch
        LogServerLookupMiss(targetId);
        return 0;
    }

    private static string ReadServerFindField(Server srv, ushort fieldId)
    {
        return fieldId switch
        {
            0 => srv.ServerID ?? "",
            2 => srv.rackPositionUID.ToString(),
            3 => srv.gameObject.name ?? "",
            _ => ""
        };
    }

    private static void LogServerLookupMiss(string targetId)
    {
        try
        {
            var all = UnityEngine.Resources.FindObjectsOfTypeAll<Server>();
            var sb = new System.Text.StringBuilder();
            sb.Append($"[FindById] Server '{targetId}' not found. Scene servers ({all.Count}): ");
            foreach (var srv in all)
            {
                try
                {
                    bool inScene = srv.gameObject.scene.name != null;
                    string sid = srv.ServerID ?? "<null>";
                    bool active = srv.gameObject.activeInHierarchy;
                    sb.Append($"[id={sid} active={active} inScene={inScene}] ");
                }
                catch { sb.Append("[err] "); }
            }
            CrashLog.Log(sb.ToString());
        }
        catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
    }

    private static ulong FindSwitchById(string targetId, ushort fieldId)
    {
        foreach (var sw in UnityEngine.Resources.FindObjectsOfTypeAll<NetworkSwitch>())
        {
            try
            {
                if (sw.gameObject.scene.name == null) continue;
                string val = ReadSwitchFindField(sw, fieldId);
                if (val == targetId) return (ulong)sw.Pointer.ToInt64();
            }
            catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
        }
        LogSwitchLookupMiss(targetId);
        return 0;
    }

    private static string ReadSwitchFindField(NetworkSwitch sw, ushort fieldId)
    {
        return fieldId switch
        {
            1 => sw.switchId ?? "",
            3 => sw.gameObject.name ?? "",
            _ => ""
        };
    }

    private static void LogSwitchLookupMiss(string targetId)
    {
        try
        {
            var all = UnityEngine.Resources.FindObjectsOfTypeAll<NetworkSwitch>();
            var sb = new System.Text.StringBuilder();
            sb.Append($"[FindById] NetworkSwitch '{targetId}' not found. Scene switches ({all.Count}): ");
            foreach (var sw in all)
            {
                try
                {
                    bool inScene = sw.gameObject.scene.name != null;
                    string sid = sw.switchId ?? "<null>";
                    bool active = sw.gameObject.activeInHierarchy;
                    sb.Append($"[id={sid} active={active} inScene={inScene}] ");
                }
                catch { sb.Append("[err] "); }
            }
            CrashLog.Log(sb.ToString());
        }
        catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
    }

    private static ulong FindPatchPanelById(string targetId, ushort fieldId)
    {
        foreach (var pp in UnityEngine.Resources.FindObjectsOfTypeAll<PatchPanel>())
        {
            try
            {
                if (pp.gameObject.scene.name == null) continue;
                string val = ReadPatchPanelFindField(pp, fieldId);
                if (val == targetId) return (ulong)pp.Pointer.ToInt64();
            }
            catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
        }
        // Lookup failed dump
        LogPatchPanelLookupMiss(targetId);
        return 0;
    }

    private static string ReadPatchPanelFindField(PatchPanel pp, ushort fieldId)
    {
        return fieldId switch
        {
            4 => pp.patchPanelId ?? "",
            3 => pp.gameObject.name ?? "",
            _ => ""
        };
    }

    private static void LogPatchPanelLookupMiss(string targetId)
    {
        try
        {
            var all = UnityEngine.Resources.FindObjectsOfTypeAll<PatchPanel>();
            var sb = new System.Text.StringBuilder();
            sb.Append($"[FindById] PatchPanel '{targetId}' not found. Scene panels ({all.Count}): ");
            foreach (var pp in all)
            {
                try
                {
                    bool inScene = pp.gameObject.scene.name != null;
                    string pid = pp.patchPanelId ?? "<null>";
                    bool active = pp.gameObject.activeInHierarchy;
                    sb.Append($"[id={pid} active={active} inScene={inScene}] ");
                }
                catch { sb.Append("[err] "); }
            }
            CrashLog.Log(sb.ToString());
        }
        catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
    }

    int GetHeldObjectImpl(IntPtr outId, uint idMax, IntPtr outType)
    {
        try
        {
            string id = Patch_UsableObject_InteractOnClick.GetHeldObjectId();
            byte objType = Patch_UsableObject_InteractOnClick.GetHeldObjectType();
            if (string.IsNullOrEmpty(id) || outId == IntPtr.Zero || idMax == 0) return 0;
            if (outType != IntPtr.Zero) Marshal.WriteByte(outType, objType);
            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(id);
            int len = System.Math.Min(bytes.Length, (int)idMax);
            Marshal.Copy(bytes, 0, outId, len);
            return len;
        }
        catch (Exception ex)
        {
            CrashLog.LogException("GetHeldObjectImpl", ex);
            return 0;
        }
    }

    int ObjGetRotationImpl(ulong handle, IntPtr outX, IntPtr outY, IntPtr outZ, IntPtr outW)
    {
        try
        {
            var comp = ResolveComponent(handle);
            if (comp == null) return 0;
            var rot = comp.transform.rotation;
            Marshal.Copy(new float[] { rot.x }, 0, outX, 1);
            Marshal.Copy(new float[] { rot.y }, 0, outY, 1);
            Marshal.Copy(new float[] { rot.z }, 0, outZ, 1);
            Marshal.Copy(new float[] { rot.w }, 0, outW, 1);
            return 1;
        }
        catch { return 0; }
    }

    // ── v17: handle-based rack operations + generic transform primitives ──

    int ObjSetParentImpl(ulong child, ulong parent)
    {
        try
        {
            var childComp = ResolveComponent(child);
            var parentComp = ResolveComponent(parent);
            if (childComp == null || parentComp == null) return 0;
            childComp.transform.SetParent(parentComp.transform, false);
            return 1;
        }
        catch { return 0; }
    }

    int ObjSetLocalPositionImpl(ulong handle, float x, float y, float z)
    {
        try
        {
            var comp = ResolveComponent(handle);
            if (comp == null) return 0;
            comp.transform.localPosition = new UnityEngine.Vector3(x, y, z);
            return 1;
        }
        catch { return 0; }
    }

    int ObjSetLocalRotationImpl(ulong handle, float x, float y, float z, float w)
    {
        try
        {
            var comp = ResolveComponent(handle);
            if (comp == null) return 0;
            comp.transform.localRotation = new UnityEngine.Quaternion(x, y, z, w);
            return 1;
        }
        catch { return 0; }
    }

    ulong RackFindPositionImpl(int rackUid)
    {
        try
        {
            var rackPos = FindRackPosition(rackUid);
            if (rackPos == null) return 0;
            return (ulong)rackPos.Pointer.ToInt64();
        }
        catch (Exception ex)
        {
            CrashLog.LogException("RackFindPositionImpl", ex);
            return 0;
        }
    }

    int RackGameInstallImpl(ulong objHandle, ulong rackPosHandle, byte objectType)
    {
        try
        {
            var rackPos = new Il2Cpp.RackPosition(new IntPtr((long)rackPosHandle));
            if (rackPos == null) return 0;

            var rack = rackPos.rack;
            if (rack == null)
            {
                CrashLog.Log($"[WorldSync] RackGameInstall: rackPos has no parent Rack");
                return 0;
            }

            var ptr = new IntPtr((long)objHandle);

            Patch_Rack_MarkPositionAsUsed.SuppressEvents = true;
            try
            {
                // Read the current object ID BEFORE calling InsertedInRack
                // so the Harmony postfix can restore it if the game renames it
                string objectId = ReadRackInstallObjectId(ptr, objectType);
                int rackPosUid = rackPos.rackPosGlobalUID;
                if (!string.IsNullOrEmpty(objectId))
                {
                    Patch_Rack_MarkPositionAsUsed.PendingCloneRestore = (objectId, objectType, rackPosUid);
                }

                int sizeInU = RackInstallBookkeeping(ptr, rackPos, objectType, "RackGameInstall");

                // Mark position as used
                rack.MarkPositionAsUsed(rackPos.positionIndex, sizeInU);

                CrashLog.Log($"[WorldSync] RackGameInstall: installed at uid={rackPos.rackPosGlobalUID} OK");
                return 1;
            }
            catch (Exception ex)
            {
                CrashLog.Log($"[WorldSync] RackGameInstall: failed: {ex.Message}");
                return 0;
            }
            finally
            {
                Patch_Rack_MarkPositionAsUsed.SuppressEvents = false;
            }
        }
        catch (Exception ex)
        {
            Patch_Rack_MarkPositionAsUsed.SuppressEvents = false;
            CrashLog.LogException("RackGameInstallImpl", ex);
            return 0;
        }
    }

    private static string ReadRackInstallObjectId(IntPtr ptr, byte objectType)
    {
        if (objectType == 0 || objectType == 1 || objectType == 2 || objectType == 3)
            return ReadInstallServerId(ptr);
        if (objectType == 4)
            return ReadInstallSwitchId(ptr);
        if (objectType == 7)
            return ReadInstallPatchPanelId(ptr);
        return "";
    }

    private static string ReadInstallServerId(IntPtr ptr)
    {
        try { var srv = new Il2Cpp.Server(ptr); return srv.ServerID ?? ""; } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
        return "";
    }

    private static string ReadInstallSwitchId(IntPtr ptr)
    {
        try { var sw = new Il2Cpp.NetworkSwitch(ptr); return sw.switchId ?? ""; } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
        return "";
    }

    private static string ReadInstallPatchPanelId(IntPtr ptr)
    {
        try { var pp = new Il2Cpp.PatchPanel(ptr); return pp.patchPanelId ?? ""; } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
        return "";
    }

    int RackGameUninstallImpl(ulong objHandle, byte objectType)
    {
        try
        {
            var ptr = new IntPtr((long)objHandle);

            Il2Cpp.RackPosition savedRackPos;
            int savedSizeInU;
            SaveUninstallRackState(ptr, objectType, out savedRackPos, out savedSizeInU);

            RackUninstallBookkeeping(ptr, objectType, "RackGameUninstall");

            ClearUninstallTracking(savedRackPos);
            FreeUninstallRackPosition(savedRackPos, savedSizeInU);

            CrashLog.Log($"[WorldSync] RackGameUninstall: cleared rack fields OK");
            return 1;
        }
        catch (Exception ex)
        {
            CrashLog.LogException("RackGameUninstallImpl", ex);
            return 0;
        }
    }

    private static void SaveUninstallRackState(IntPtr ptr, byte objectType, out Il2Cpp.RackPosition savedRackPos, out int savedSizeInU)
    {
        savedRackPos = null;
        savedSizeInU = 1;
        try
        {
            switch (objectType)
            {
                case 0:
                case 1:
                case 2:
                case 3: // Server types
                    {
                        var srv = new Il2Cpp.Server(ptr);
                        savedRackPos = srv.currentRackPosition;
                        savedSizeInU = srv.sizeInU > 0 ? srv.sizeInU : 1;
                    }
                    break;
                case 4: // NetworkSwitch
                    {
                        var sw = new Il2Cpp.NetworkSwitch(ptr);
                        savedRackPos = sw.currentRackPosition;
                        savedSizeInU = sw.sizeInU > 0 ? sw.sizeInU : 1;
                    }
                    break;
                case 7: // PatchPanel
                    {
                        var pp = new Il2Cpp.PatchPanel(ptr);
                        savedRackPos = pp.currentRackPosition;
                        savedSizeInU = pp.sizeInU > 0 ? pp.sizeInU : 1;
                    }
                    break;
                default:
                    break;
            }
        }
        catch (Exception ex) { CrashLog.Log($"[WorldSync] RackGameUninstall: failed to save rack pos: {ex.Message}"); }
    }

    private static void ClearUninstallTracking(Il2Cpp.RackPosition savedRackPos)
    {
        // Clear the installed objects tracking for this rack position
        if (savedRackPos != null)
        {
            try
            {
                int posUid = savedRackPos.rackPosGlobalUID;
                if (posUid > 0)
                    Patch_Rack_MarkPositionAsUsed.RemoveInstalledObject(posUid);
            }
            catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
        }
    }

    private static void FreeUninstallRackPosition(Il2Cpp.RackPosition savedRackPos, int savedSizeInU)
    {
        if (savedRackPos == null) return;
        try
        {
            var rack = savedRackPos.rack;
            if (rack != null)
            {
                int startIndex = savedRackPos.positionIndex;
                CrashLog.Log($"[WorldSync] RackGameUninstall: IsPositionAvailable({startIndex}, {savedSizeInU}) = {rack.IsPositionAvailable(startIndex, savedSizeInU)} before freeing");
                rack.MarkPositionAsUnused(startIndex, savedSizeInU);
                CrashLog.Log($"[WorldSync] RackGameUninstall: freed {savedSizeInU} position(s) starting at index {startIndex}");
            }
        }
        catch (Exception ex) { CrashLog.Log($"[WorldSync] RackGameUninstall: rack position free failed: {ex.Message}"); }
    }
}
