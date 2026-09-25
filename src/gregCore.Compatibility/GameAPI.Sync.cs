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
    int WorldSetPowerImpl(IntPtr id, uint idLen, byte isOn)
    {
        // Phase 3 stub
        string objId = ReadUtf8(id, idLen);
        CrashLog.Log($"[WorldSync] SetPower stub: id={objId}, on={isOn}");
        return 0;
    }

    int WorldSetPropertyImpl(IntPtr id, uint idLen, IntPtr key, uint keyLen, IntPtr val, uint valLen)
    {
        // Phase 3 stub
        return 0;
    }


    int WorldDisconnectCableImpl(int cableId)
    {
        // Phase 3 stub
        CrashLog.Log($"[WorldSync] DisconnectCable stub: cableId={cableId}");
        return 0;
    }

    int WorldPickupObjectImpl(IntPtr id, uint idLen)
    {
        try
        {
            string objId = ReadUtf8(id, idLen);
            CrashLog.Log($"[WorldSync] PickupObject: id={objId}");

            ulong handle = FindHandleByStableId(objId);
            if (handle == 0)
            {
                CrashLog.Log($"[WorldSync] PickupObject: '{objId}' not found");
                return 0;
            }

            int result = ObjSetActiveImpl(handle, 0);
            CrashLog.Log($"[WorldSync] PickupObject: deactivated '{objId}' → {result}");
            return result;
        }
        catch (Exception ex)
        {
            CrashLog.LogException("WorldPickupObjectImpl", ex);
            return 0;
        }
    }


    private static UnityEngine.Rigidbody LockDropRigidBody(UnityEngine.Component comp)
    {
        // GetComponent works on inactive GameObjects in Unity/IL2CPP.
        // Setting isKinematic=true before SetActive() means the Rigidbody
        // enters the physics world as kinematic on the very first tick —
        // no velocity, no forces, no depenetration.
        var rb = comp.GetComponent<UnityEngine.Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
            rb.velocity = UnityEngine.Vector3.zero;
            rb.angularVelocity = UnityEngine.Vector3.zero;
        }
        return rb;
    }

    private static void TeleportDropToPose(UnityEngine.Component comp, float x, float y, float z, float rotX, float rotY, float rotZ, float rotW)
    {
        // Moving the transform before SetActive() is the critical fix:
        //   a) The game's own OnEnable callbacks fire at the DROP position,
        //      not at the stale rack-slot position.
        //   b) PhysX never sees the body at the rack slot — it goes straight
        //      from "not in simulation" to "at drop position, kinematic".
        var mgr = MainGameManager.instance;
        if (mgr != null && mgr.parentUsableObjects != null)
        {
            try { comp.transform.SetParent(mgr.parentUsableObjects, true); }
            catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
        }
        comp.transform.position = new UnityEngine.Vector3(x, y, z);
        comp.transform.rotation = new UnityEngine.Quaternion(rotX, rotY, rotZ, rotW);
    }

    private static void ActivateDropComponent(UnityEngine.Component comp)
    {
        // OnEnable fires here; transform is already at the drop target.
        comp.gameObject.SetActive(true);
    }

    private static void ReleaseDropRigidBody(UnityEngine.Rigidbody rb, ulong handle)
    {
        // Ensures the engine's AABB / contact cache reflects the new
        // world position before we release kinematic.
        if (rb != null)
        {
            rb.velocity = UnityEngine.Vector3.zero;
            rb.angularVelocity = UnityEngine.Vector3.zero;
            rb.isKinematic = false;
            rb.useGravity = true;
            rb.WakeUp();
        }
        else
        {
            ReleaseDropWithoutRigidBody(handle);
        }
    }

    private static void ReleaseDropWithoutRigidBody(ulong handle)
    {
        try
        {
            var comp = ResolveComponent(handle);
            if (comp == null) return;
            var rb2 = comp.GetComponent<UnityEngine.Rigidbody>();
            if (rb2 == null) return;
            rb2.isKinematic = false;
            rb2.useGravity = true;
            rb2.WakeUp();
        }
        catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
    }

    /// <summary>
    /// Clears the <c>objectInHands</c> flag that
    /// <see cref="RackUninstallBookkeeping"/> sets to <c>true</c> when an
    /// object is removed from a rack.  On the remote client this flag is
    /// never cleared by the native drop logic, so we must clear it here
    /// before calling SetActive(true).  If left as <c>true</c>, the game's
    /// own MonoBehaviour scripts apply carry-follow forces to the object on
    /// the very first Update tick after activation, flinging it away.
    /// </summary>
    private static void TryResetObjectInHands(ulong handle)
    {
        if (TryResetServerInHands(handle)) return;
        if (TryResetSwitchInHands(handle)) return;
        TryResetPatchPanelInHands(handle);
    }

    private static bool TryResetServerInHands(ulong handle)
    {
        var ptr = new IntPtr((long)handle);

        // Try Server types first
        try
        {
            var srv = new Il2Cpp.Server(ptr);
            if (!string.IsNullOrEmpty(srv.ServerID))
            {
                srv.objectInHands = false;
                CrashLog.Log($"[WorldSync] TryResetObjectInHands: cleared Server '{srv.ServerID}'");
                return true;
            }
        }
        catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
        return false;
    }

    private static bool TryResetSwitchInHands(ulong handle)
    {
        var ptr = new IntPtr((long)handle);

        // Try NetworkSwitch
        try
        {
            var sw = new Il2Cpp.NetworkSwitch(ptr);
            if (!string.IsNullOrEmpty(sw.switchId))
            {
                sw.objectInHands = false;
                CrashLog.Log($"[WorldSync] TryResetObjectInHands: cleared NetworkSwitch '{sw.switchId}'");
                return true;
            }
        }
        catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
        return false;
    }

    private static void TryResetPatchPanelInHands(ulong handle)
    {
        var ptr = new IntPtr((long)handle);

        // Try PatchPanel
        try
        {
            var pp = new Il2Cpp.PatchPanel(ptr);
            if (!string.IsNullOrEmpty(pp.patchPanelId))
            {
                pp.objectInHands = false;
                CrashLog.Log($"[WorldSync] TryResetObjectInHands: cleared PatchPanel '{pp.patchPanelId}'");
            }
        }
        catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
    }

    int WorldEnsureRackUIDsImpl()
    {
        try
        {
            return GameHooks.EnsureAllRackPositionUIDs();
        }
        catch (Exception ex)
        {
            CrashLog.LogException("WorldEnsureRackUIDsImpl", ex);
            return 0;
        }
    }
}
