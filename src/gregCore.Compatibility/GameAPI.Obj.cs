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
    private static UnityEngine.Component ResolveComponent(ulong handle)
    {
        try
        {
            if (handle == 0) return null;
            var ptr = new IntPtr((long)handle);
            var comp = new UnityEngine.Component(ptr);
            if (comp == null || comp.gameObject == null) return null;
            return comp;
        }
        catch { return null; }
    }

    uint ObjFindByTypeImpl(byte typeId, IntPtr outHandles, uint max)
    {
        try
        {
            switch (typeId)
            {
                case 0: // Server
                    return CollectServerHandles(outHandles, max);
                case 4: // NetworkSwitch
                    return CollectSwitchHandles(outHandles, max);
                default:
                    return 0;
            }
        }
        catch (Exception ex)
        {
            CrashLog.LogException("ObjFindByTypeImpl", ex);
            return 0;
        }
    }

    private static uint CollectServerHandles(IntPtr outHandles, uint max)
    {
        uint count = 0;
        var all = UnityEngine.Resources.FindObjectsOfTypeAll<Server>();
        foreach (var srv in all)
        {
            try
            {
                if (srv.gameObject.scene.name == null) continue;
                if (count >= max) break;
                Marshal.WriteInt64(outHandles, (int)(count * 8), srv.Pointer.ToInt64());
                count++;
            }
            catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
        }
        return count;
    }

    private static uint CollectSwitchHandles(IntPtr outHandles, uint max)
    {
        uint count = 0;
        var all = UnityEngine.Resources.FindObjectsOfTypeAll<NetworkSwitch>();
        foreach (var sw in all)
        {
            try
            {
                if (sw.gameObject.scene.name == null) continue;
                if (count >= max) break;
                Marshal.WriteInt64(outHandles, (int)(count * 8), sw.Pointer.ToInt64());
                count++;
            }
            catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
        }
        return count;
    }

    uint ObjGetStringFieldImpl(ulong handle, ushort fieldId, IntPtr outBuf, uint max)
    {
        try
        {
            if (handle == 0 || outBuf == IntPtr.Zero || max == 0) return 0;
            var ptr = new IntPtr((long)handle);
            string value = ReadObjStringField(ptr, fieldId);
            if (string.IsNullOrEmpty(value)) return 0;
            return WriteObjStringField(outBuf, max, value);
        }
        catch (Exception ex)
        {
            CrashLog.LogException("ObjGetStringFieldImpl", ex);
            return 0;
        }
    }

    private static string ReadObjStringField(IntPtr ptr, ushort fieldId)
    {
        if (fieldId == 0)
            return ReadServerIdField(ptr);
        if (fieldId == 1)
            return ReadSwitchIdField(ptr);
        if (fieldId == 2)
            return ReadRackPositionUidString(ptr);
        if (fieldId == 3)
            return ReadGameObjectNameField(ptr);
        if (fieldId == 4)
            return ReadPatchPanelIdField(ptr);
        return "";
    }

    private static string ReadServerIdField(IntPtr ptr)
    {
        try { var srv = new Server(ptr); return srv?.ServerID ?? ""; } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
        return "";
    }

    private static string ReadSwitchIdField(IntPtr ptr)
    {
        try { var sw = new NetworkSwitch(ptr); return sw?.switchId ?? ""; } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
        return "";
    }

    private static string ReadGameObjectNameField(IntPtr ptr)
    {
        try { var comp = new UnityEngine.Component(ptr); return comp?.gameObject?.name ?? ""; } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
        return "";
    }

    private static string ReadPatchPanelIdField(IntPtr ptr)
    {
        try { var pp = new PatchPanel(ptr); return pp?.patchPanelId ?? ""; } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
        return "";
    }

    private static string ReadRackPositionUidString(IntPtr ptr)
    {
        // Try each type, but filter negative values which indicate
        // we're reading the wrong Il2Cpp field offset (type confusion).
        int bestUid = ReadServerRackUid(ptr);
        if (bestUid == 0)
            bestUid = ReadSwitchRackUid(ptr);
        if (bestUid == 0)
            bestUid = ReadPatchPanelRackUid(ptr);
        return bestUid.ToString();
    }

    private static int ReadServerRackUid(IntPtr ptr)
    {
        try { var srv = new Server(ptr); int uid = srv.rackPositionUID; if (uid > 0) return uid; } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
        return 0;
    }

    private static int ReadSwitchRackUid(IntPtr ptr)
    {
        try { var sw = new NetworkSwitch(ptr); int uid = sw.rackPositionUID; if (uid > 0) return uid; } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
        return 0;
    }

    private static int ReadPatchPanelRackUid(IntPtr ptr)
    {
        try { var pp = new PatchPanel(ptr); int uid = pp.rackPositionUID; if (uid > 0) return uid; } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
        return 0;
    }

    private static uint WriteObjStringField(IntPtr outBuf, uint max, string value)
    {
        byte[] bytes = System.Text.Encoding.UTF8.GetBytes(value);
        int len = System.Math.Min(bytes.Length, (int)max);
        Marshal.Copy(bytes, 0, outBuf, len);
        return (uint)len;
    }

    int ObjSetStringFieldImpl(ulong handle, ushort fieldId, IntPtr value, uint valueLen)
    {
        try
        {
            if (handle == 0) return 0;
            var ptr = new IntPtr((long)handle);
            string newValue = ReadObjSetStringValue(value, valueLen);

            return ApplyObjStringField(ptr, fieldId, newValue);
        }
        catch (Exception ex)
        {
            CrashLog.LogException("ObjSetStringFieldImpl", ex);
            return 0;
        }
    }

    private static string ReadObjSetStringValue(IntPtr value, uint valueLen)
    {
        string newValue = "";
        if (value != IntPtr.Zero && valueLen > 0)
        {
            byte[] buf = new byte[valueLen];
            Marshal.Copy(value, buf, 0, (int)valueLen);
            newValue = System.Text.Encoding.UTF8.GetString(buf);
        }
        return newValue;
    }

    private static int ApplyObjStringField(IntPtr ptr, ushort fieldId, string newValue)
    {
        switch (fieldId)
        {
            case 0: // ServerID
                if (TrySetServerId(ptr, newValue)) return 1;
                break;
            case 1: // SwitchId
                if (TrySetSwitchId(ptr, newValue)) return 1;
                break;
            case 4: // PatchPanelId
                if (TrySetPatchPanelId(ptr, newValue)) return 1;
                break;
        }
        return 0;
    }

    private static bool TrySetServerId(IntPtr ptr, string newValue)
    {
        try { var srv = new Server(ptr); srv.ServerID = newValue; return true; } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
        return false;
    }

    private static bool TrySetSwitchId(IntPtr ptr, string newValue)
    {
        try { var sw = new NetworkSwitch(ptr); sw.switchId = newValue; return true; } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
        return false;
    }

    private static bool TrySetPatchPanelId(IntPtr ptr, string newValue)
    {
        try { var pp = new PatchPanel(ptr); pp.patchPanelId = newValue; return true; } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
        return false;
    }

    int ObjIsActiveImpl(ulong handle)
    {
        try
        {
            var comp = ResolveComponent(handle);
            return (comp != null && comp.gameObject.activeSelf) ? 1 : 0;
        }
        catch { return 0; }
    }

    int ObjSetActiveImpl(ulong handle, int active)
    {
        try
        {
            var comp = ResolveComponent(handle);
            if (comp == null) return 0;
            comp.gameObject.SetActive(active != 0);
            return 1;
        }
        catch { return 0; }
    }

    int ObjGetPositionImpl(ulong handle, IntPtr outX, IntPtr outY, IntPtr outZ)
    {
        try
        {
            var comp = ResolveComponent(handle);
            if (comp == null) return 0;
            var pos = comp.transform.position;
            Marshal.Copy(new float[] { pos.x }, 0, outX, 1);
            Marshal.Copy(new float[] { pos.y }, 0, outY, 1);
            Marshal.Copy(new float[] { pos.z }, 0, outZ, 1);
            return 1;
        }
        catch { return 0; }
    }

    int ObjSetPositionImpl(ulong handle, float x, float y, float z)
    {
        try
        {
            var comp = ResolveComponent(handle);
            if (comp == null) return 0;
            comp.transform.position = new UnityEngine.Vector3(x, y, z);
            return 1;
        }
        catch { return 0; }
    }

    int ObjSetRotationImpl(ulong handle, float x, float y, float z, float w)
    {
        try
        {
            var comp = ResolveComponent(handle);
            if (comp == null) return 0;
            comp.transform.rotation = new UnityEngine.Quaternion(x, y, z, w);
            return 1;
        }
        catch { return 0; }
    }

    int ObjSetParentToWorldImpl(ulong handle)
    {
        try
        {
            var comp = ResolveComponent(handle);
            if (comp == null) return 0;
            var mgr = MainGameManager.instance;
            if (mgr != null && mgr.parentUsableObjects != null)
            {
                comp.transform.SetParent(mgr.parentUsableObjects, true);
                return 1;
            }
            return 0;
        }
        catch { return 0; }
    }

    int RbSetKinematicImpl(ulong handle, int kinematic)
    {
        try
        {
            var comp = ResolveComponent(handle);
            if (comp == null) return 0;
            var rb = comp.GetComponent<UnityEngine.Rigidbody>();
            if (rb == null) return 0;
            rb.isKinematic = (kinematic != 0);
            return 1;
        }
        catch { return 0; }
    }

    int RbSetGravityImpl(ulong handle, int useGravity)
    {
        try
        {
            var comp = ResolveComponent(handle);
            if (comp == null) return 0;
            var rb = comp.GetComponent<UnityEngine.Rigidbody>();
            if (rb == null) return 0;
            rb.useGravity = (useGravity != 0);
            return 1;
        }
        catch { return 0; }
    }

    int RbWakeUpImpl(ulong handle)
    {
        try
        {
            var comp = ResolveComponent(handle);
            if (comp == null) return 0;
            var rb = comp.GetComponent<UnityEngine.Rigidbody>();
            if (rb == null) return 0;
            rb.velocity = UnityEngine.Vector3.zero;
            rb.angularVelocity = UnityEngine.Vector3.zero;
            rb.WakeUp();
            return 1;
        }
        catch { return 0; }
    }
}
