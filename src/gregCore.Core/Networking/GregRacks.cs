/// <file-summary>
/// Schicht:      Core (Networking)
/// Zweck:        Rack- + Trolley-Brücke: Racks finden, Positionen prüfen/
///               markieren, RackPosition per UID, Trolley-Bays, Wandcheck,
///               CarryModelPool, Trolley-Griffe. Alles best-effort.
/// </file-summary>

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using gregCore.Core.Mods;
using MelonLoader;
using UnityEngine;

namespace gregCore.Core.Networking;

[ExcludeFromCodeCoverage(Justification = "Live Il2Cpp interop against game assemblies; needs running game.")]
public static class GregRacks
{
    // ── Finden ───────────────────────────────────────────────────────────────

    public static List<global::Il2Cpp.Rack> FindAllRacks()
    {
        return FindAll<global::Il2Cpp.Rack>();
    }

    public static List<global::Il2Cpp.RackPosition> FindAllPositions()
    {
        return FindAll<global::Il2Cpp.RackPosition>();
    }

    public static List<global::Il2Cpp.TrolleyLoadingBay> FindAllLoadingBays()
    {
        return FindAll<global::Il2Cpp.TrolleyLoadingBay>();
    }

    public static List<global::Il2Cpp.PushTrolleyHandle> FindAllTrolleyHandles()
    {
        return FindAll<global::Il2Cpp.PushTrolleyHandle>();
    }

    private static List<T> FindAll<T>() where T : UnityEngine.Object
    {
        var result = new List<T>();
        Try(() =>
        {
            var all = Resources.FindObjectsOfTypeAll<T>();
            if (all == null) return;
            foreach (var o in all)
            {
                if (o == null) continue;
                try
                {
                    var go = (o as Component) != null
                        ? ((Component)(object)o).gameObject
                        : (o as GameObject);
                    if (go != null && go.scene.IsValid() && go.scene.isLoaded)
                        result.Add(o);
                }
                catch { }
            }
        });
        return result;
    }

    // ── Rack: Positionen ─────────────────────────────────────────────────────

    public static int GetPositionCount(global::Il2Cpp.Rack rack)
    {
        if (rack == null) return -1;
        try
        {
            var arr = rack.positions;
            if (arr == null) return -1;
            return arr.Length;
        }
        catch { return -1; }
    }

    public static bool[] GetPositionUsage(global::Il2Cpp.Rack rack)
    {
        if (rack == null) return Array.Empty<bool>();
        bool[] result = Array.Empty<bool>();
        Try(() =>
        {
            var arr = rack.isPositionUsed;
            if (arr == null) return;
            int n = 0;
            try { n = arr.Length; } catch { return; }
            var dst = new bool[Math.Max(0, n)];
            for (int i = 0; i < dst.Length; i++)
            {
                try { dst[i] = arr[i] != 0; } catch { dst[i] = false; }
            }
            result = dst;
        });
        return result;
    }

    public static bool IsPositionAvailable(global::Il2Cpp.Rack rack, int index, int sizeInU)
    {
        if (rack == null) return false;
        try
        {
            var _ = rack.gameObject; // liveness
            return rack.IsPositionAvailable(index, sizeInU);
        }
        catch { return false; }
    }

    public static bool MarkPosition(global::Il2Cpp.Rack rack, int index, int sizeInU, bool used)
    {
        if (rack == null) return false;
        try
        {
            var _ = rack.gameObject; // liveness
            if (used) rack.MarkPositionAsUsed(index, sizeInU);
            else rack.MarkPositionAsUnused(index, sizeInU);
            return true;
        }
        catch (Exception ex)
        {
            Warn($"MarkPosition fehlgeschlagen: {Base(ex)}");
            return false;
        }
    }

    public static bool UnmountRack(global::Il2Cpp.Rack rack)
    {
        if (rack == null) return false;
        try
        {
            var _ = rack.gameObject; // liveness
            rack.UnmountRack();
            return true;
        }
        catch (Exception ex)
        {
            Warn($"UnmountRack fehlgeschlagen: {Base(ex)}");
            return false;
        }
    }

    public static bool InitializeLoadedRack(global::Il2Cpp.Rack rack, int[] loadedPositions)
    {
        if (rack == null) return false;
        var arr = GregModPack.ToIntArray(loadedPositions ?? Array.Empty<int>());
        if (arr == null) return false;
        try
        {
            var _ = rack.gameObject; // liveness
            rack.InitializeLoadedRack(arr);
            return true;
        }
        catch (Exception ex)
        {
            Warn($"InitializeLoadedRack fehlgeschlagen: {Base(ex)}");
            return false;
        }
    }

    // ── RackPosition ─────────────────────────────────────────────────────────

    public static global::Il2Cpp.RackPosition GetPositionByUID(int uid)
    {
        try { return global::Il2Cpp.RackPosition.GetByUID(uid); }
        catch { return null; }
    }

    public static bool SetPositionUsed(global::Il2Cpp.RackPosition pos, bool used)
    {
        if (pos == null) return false;
        try
        {
            var _ = pos.gameObject; // liveness
            pos.SetUsed(used);
            return true;
        }
        catch (Exception ex)
        {
            Warn($"SetUsed fehlgeschlagen: {Base(ex)}");
            return false;
        }
    }

    public static bool IsAllowedItem(global::Il2Cpp.RackPosition pos, bool checkAvailability)
    {
        if (pos == null) return false;
        try
        {
            var _ = pos.gameObject; // liveness
            return pos.IsAllowedItem(checkAvailability);
        }
        catch { return false; }
    }

    public static bool BeginInsertItem(global::Il2Cpp.RackPosition pos)
    {
        if (pos == null) return false;
        try
        {
            var _ = pos.gameObject; // liveness
            var routine = pos.InsertItemInRack();
            return GregRackAndSfp.StartIl2CppRoutine(routine);
        }
        catch (Exception ex)
        {
            Warn($"InsertItemInRack fehlgeschlagen: {Base(ex)}");
            return false;
        }
    }

    // ── Trolley-Bays / Griffe / Wandcheck ────────────────────────────────────

    public static bool FreeTrolleySlot(global::Il2Cpp.TrolleyLoadingBay bay, int startIdx, int sizeInU)
    {
        if (bay == null) return false;
        try
        {
            var _ = bay.gameObject; // liveness
            bay.FreeTrolleySlot(startIdx, sizeInU);
            return true;
        }
        catch (Exception ex)
        {
            Warn($"FreeTrolleySlot fehlgeschlagen: {Base(ex)}");
            return false;
        }
    }

    public static bool ResetAllTrolleySlots(global::Il2Cpp.TrolleyLoadingBay bay)
    {
        if (bay == null) return false;
        try
        {
            var _ = bay.gameObject; // liveness
            bay.ResetAllSlots();
            return true;
        }
        catch (Exception ex)
        {
            Warn($"ResetAllSlots fehlgeschlagen: {Base(ex)}");
            return false;
        }
    }

    public static bool ClickTrolleyHandle(global::Il2Cpp.PushTrolleyHandle handle)
    {
        if (handle == null) return false;
        try
        {
            var _ = handle.gameObject; // liveness
            handle.InteractOnClick();
            return true;
        }
        catch (Exception ex)
        {
            Warn($"TrolleyHandle-Click fehlgeschlagen: {Base(ex)}");
            return false;
        }
    }

    public static bool TriggerOverlapCheck(global::Il2Cpp.CheckIfTouchingWall check)
    {
        if (check == null) return false;
        try
        {
            var _ = check.gameObject; // liveness
            check.PerformOverlapCheck();
            return true;
        }
        catch (Exception ex)
        {
            Warn($"PerformOverlapCheck fehlgeschlagen: {Base(ex)}");
            return false;
        }
    }

    public static bool SetWallCheckRenderers(global::Il2Cpp.CheckIfTouchingWall check, bool enabled)
    {
        if (check == null) return false;
        try
        {
            var _ = check.gameObject; // liveness
            check.SetRenderersEnabled(enabled);
            return true;
        }
        catch (Exception ex)
        {
            Warn($"SetRenderersEnabled fehlgeschlagen: {Base(ex)}");
            return false;
        }
    }

    // ── CarryModelPool ───────────────────────────────────────────────────────

    public static global::Il2Cpp.CarryModelPool GetCarryPool()
    {
        try { return global::Il2Cpp.CarryModelPool.instance; }
        catch { return null; }
    }

    public static GameObject PoolGet(GameObject prefab, int prefabID, Vector3 position, Quaternion rotation, Transform parent)
    {
        var pool = GetCarryPool();
        if (pool == null || prefab == null) return null;
        try { return pool.Get(prefab, prefabID, position, rotation, parent); }
        catch (Exception ex)
        {
            Warn($"Pool-Get fehlgeschlagen: {Base(ex)}");
            return null;
        }
    }

    public static bool PoolReturn(GameObject obj, int prefabID)
    {
        var pool = GetCarryPool();
        if (pool == null || obj == null) return false;
        try
        {
            pool.Return(obj, prefabID);
            return true;
        }
        catch (Exception ex)
        {
            Warn($"Pool-Return fehlgeschlagen: {Base(ex)}");
            return false;
        }
    }

    public static bool ClearPool()
    {
        var pool = GetCarryPool();
        if (pool == null) return false;
        try
        {
            pool.ClearPool();
            return true;
        }
        catch (Exception ex)
        {
            Warn($"Pool-Clear fehlgeschlagen: {Base(ex)}");
            return false;
        }
    }

    private static string Base(Exception ex)
    {
        try { return ex != null ? ex.GetBaseException().Message : "?"; } catch { return "?"; }
    }

    private static void Warn(string message)
    {
        try { MelonLogger.Warning($"[gregCore][Net] Racks: {message}"); } catch { }
    }

    private static void Try(Action action)
    {
        try { action?.Invoke(); }
        catch (Exception ex)
        {
            try { MelonLogger.Warning($"[gregCore][Net] Racks-Feld fehlgeschlagen: {ex.GetBaseException().Message}"); } catch { }
        }
    }
}
