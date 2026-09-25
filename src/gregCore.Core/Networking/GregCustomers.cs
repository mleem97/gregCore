/// <file-summary>
/// Layer:       Core (Networking)
/// Purpose:     Customer bridge (runtime): find/read CustomerBase
///               (IDs, money speed, requirements, IP lookup, routed subnets,
///               LoadData), read CustomerItem, fill CustomerCard.
///               All best-effort.
/// </file-summary>

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using MelonLoader;
using UnityEngine;

namespace gregCore.Core.Networking;

[ExcludeFromCodeCoverage(Justification = "Live Il2Cpp interop against game assemblies; needs running game.")]
public static class GregCustomers
{
    // ── DTO ──────────────────────────────────────────────────────────────────

    public sealed class BaseInfo
    {
        public int CustomerBaseID { get; set; } = -1;
        public int CustomerID { get; set; } = -1;
        public float EffectiveMoneySpeed { get; set; }
        public bool AllRequirementsMet { get; set; }
        public bool WantsInternet { get; set; }
        public bool WasFullySatisfied { get; set; }
    }

    public sealed class ItemInfo
    {
        public int CustomerID { get; set; } = -1;
        public string CustomerName { get; set; } = "";
        public int Difficulty { get; set; }
        public int Reputation { get; set; }
        public int[] AppTypes = Array.Empty<int>();
    }

    // ── Find ─────────────────────────────────────────────────────────────────

    public static List<global::Il2Cpp.CustomerBase> FindAllBases()
    {
        var result = new List<global::Il2Cpp.CustomerBase>();
        Try(() =>
        {
            var all = Resources.FindObjectsOfTypeAll<global::Il2Cpp.CustomerBase>();
            if (all == null) return;
            foreach (var cb in all)
            {
                if (cb == null) continue;
                try
                {
                    var go = cb.gameObject;
                    if (go != null && go.scene.IsValid() && go.scene.isLoaded)
                        result.Add(cb);
                }
                catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
            }
        });
        return result;
    }

    public static global::Il2Cpp.CustomerBase FindByCustomerID(int customerID)
    {
        global::Il2Cpp.CustomerBase found = null;
        Try(() =>
        {
            foreach (var cb in FindAllBases())
            {
                int id = -1;
                try { id = cb.customerID; } catch { continue; }
                if (id == customerID) { found = cb; break; }
            }
        });
        return found;
    }

    public static global::Il2Cpp.CustomerBase FindByBaseID(int customerBaseID)
    {
        global::Il2Cpp.CustomerBase found = null;
        Try(() =>
        {
            foreach (var cb in FindAllBases())
            {
                int id = -1;
                try { id = cb.customerBaseID; } catch { continue; }
                if (id == customerBaseID) { found = cb; break; }
            }
        });
        return found;
    }

    // ── Read ─────────────────────────────────────────────────────────────────

    public static BaseInfo ReadBase(global::Il2Cpp.CustomerBase cb)
    {
        var dto = new BaseInfo();
        if (cb == null) return dto;
        Try(() => dto.CustomerBaseID = cb.customerBaseID);
        Try(() => dto.CustomerID = cb.customerID);
        Try(() => dto.EffectiveMoneySpeed = cb.GetEffectiveMoneySpeed());
        Try(() => dto.AllRequirementsMet = cb.AreAllAppRequirementsMet());
        Try(() => dto.WantsInternet = cb.wantsInternet);
        Try(() => dto.WasFullySatisfied = cb.wasFullySatisfied);
        return dto;
    }

    public static List<BaseInfo> ReadAllBases()
    {
        var result = new List<BaseInfo>();
        Try(() =>
        {
            foreach (var cb in FindAllBases())
            {
                try { result.Add(ReadBase(cb)); } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
            }
        });
        return result;
    }

    public static ItemInfo ReadItem(global::Il2Cpp.CustomerItem item)
    {
        var dto = new ItemInfo();
        if (item == null) return dto;
        Try(() => dto.CustomerID = item.customerID);
        Try(() => dto.CustomerName = item.customerName ?? "");
        Try(() => dto.Difficulty = item.difficulty);
        Try(() => dto.Reputation = item.reputation);
        Try(() =>
        {
            var arr = item.appTypes;
            if (arr == null) return;
            int n = 0;
            try { n = arr.Length; } catch { return; }
            var dst = new int[Math.Max(0, n)];
            for (int i = 0; i < dst.Length; i++)
            {
                try { dst[i] = arr[i]; } catch { dst[i] = 0; }
            }
            dto.AppTypes = dst;
        });
        return dto;
    }

    // ── IP / app lookup (for IPAM & co.) ─────────────────────────────────────

    public static bool IsIPPresent(global::Il2Cpp.CustomerBase cb, string ip)
    {
        if (cb == null || string.IsNullOrWhiteSpace(ip)) return false;
        try
        {
            _ = cb.gameObject; // liveness
            return cb.IsIPPresent(ip);
        }
        catch { return false; }
    }

    public static int GetAppIDForIP(global::Il2Cpp.CustomerBase cb, string ip)
    {
        if (cb == null || string.IsNullOrWhiteSpace(ip)) return -1;
        try
        {
            _ = cb.gameObject; // liveness
            return cb.GetAppIDForIP(ip);
        }
        catch { return -1; }
    }

    // ── Routed Subnets ───────────────────────────────────────────────────────

    public static bool TryRegisterRoutedSubnet(global::Il2Cpp.CustomerBase cb, int targetVlanId,
        string routeKey, List<string> ips)
    {
        if (cb == null || string.IsNullOrEmpty(routeKey)) return false;
        Il2CppInterop.Runtime.InteropTypes.Arrays.Il2CppStringArray arr = null;
        try
        {
            var src = ips ?? new List<string>();
            arr = new Il2CppInterop.Runtime.InteropTypes.Arrays.Il2CppStringArray(src.Count);
            for (int i = 0; i < src.Count; i++)
            {
                try { arr[i] = src[i] ?? ""; } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
            }
        }
        catch { arr = null; }
        try
        {
            _ = cb.gameObject; // liveness
            return cb.TryRegisterRoutedSubnet(targetVlanId, routeKey, arr);
        }
        catch (Exception ex)
        {
            Warn($"TryRegisterRoutedSubnet failed: {Base(ex)}");
            return false;
        }
    }

    public static bool TryUnregisterRoutedSubnet(global::Il2Cpp.CustomerBase cb, string routeKey)
    {
        if (cb == null || string.IsNullOrEmpty(routeKey)) return false;
        try
        {
            _ = cb.gameObject; // liveness
            cb.TryUnregisterRoutedSubnet(routeKey);
            return true;
        }
        catch (Exception ex)
        {
            Warn($"TryUnregisterRoutedSubnet failed: {Base(ex)}");
            return false;
        }
    }

    // ── Save bridge (vanilla path, DTO from GregCustomerSaves) ────────────────

    public static bool LoadData(global::Il2Cpp.CustomerBase cb, GregCustomerSaves.CustomerBase dto)
    {
        if (cb == null || dto == null) return false;
        global::Il2Cpp.CustomerBaseSaveData data = null;
        try { data = GregCustomerSaves.CreateBase(dto); } catch { data = null; }
        if (data == null) return false;
        try
        {
            _ = cb.gameObject; // liveness
            cb.LoadData(data);
            return true;
        }
        catch (Exception ex)
        {
            Warn($"LoadData failed: {Base(ex)}");
            return false;
        }
    }

    // ── CustomerCard ─────────────────────────────────────────────────────────

    public static bool SetCardCustomer(global::Il2Cpp.CustomerCard card, global::Il2Cpp.CustomerItem item)
    {
        if (card == null || item == null) return false;
        try
        {
            _ = card.gameObject; // liveness
            card.SetCustomer(item);
            return true;
        }
        catch (Exception ex)
        {
            Warn($"SetCustomer failed: {Base(ex)}");
            return false;
        }
    }

    private static string Base(Exception ex)
    {
        try { return ex != null ? ex.GetBaseException().Message : "?"; } catch { return "?"; }
    }

    private static void Warn(string message)
    {
        try { MelonLogger.Warning($"[gregCore][Net] Customers: {message}"); } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
    }

    private static void Try(Action action)
    {
        try { action?.Invoke(); }
        catch (Exception ex)
        {
            try { MelonLogger.Warning($"[gregCore][Net] Customers field failed: {ex.GetBaseException().Message}"); } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
        }
    }
}
