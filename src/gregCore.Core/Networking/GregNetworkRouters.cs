/// <file-summary>
/// Layer:       Core (Networking)
/// Purpose:     Live bridge for router + firewall (vanilla paths):
///               find by switchId, manage subnets/routes/rules,
///               sync clusters, check traffic. All best-effort (bool/int
///               instead of exceptions; -1/empty on errors).
/// </file-summary>

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using MelonLoader;
using UnityEngine;

namespace gregCore.Core.Networking;

[ExcludeFromCodeCoverage(Justification = "Live Il2Cpp interop against game assemblies; needs running game.")]
public static class GregNetworkRouters
{
    // ── Find ─────────────────────────────────────────────────────────────────

    public static List<global::Il2Cpp.Router> FindAllRouters()
    {
        var result = new List<global::Il2Cpp.Router>();
        Try(() =>
        {
            var all = Resources.FindObjectsOfTypeAll<global::Il2Cpp.Router>();
            if (all == null) return;
            foreach (var r in all)
            {
                if (r == null) continue;
                try
                {
                    var go = r.gameObject;
                    if (go != null && go.scene.IsValid() && go.scene.isLoaded)
                        result.Add(r);
                }
                catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
            }
        });
        return result;
    }

    public static List<global::Il2Cpp.Firewall> FindAllFirewalls()
    {
        var result = new List<global::Il2Cpp.Firewall>();
        Try(() =>
        {
            var all = Resources.FindObjectsOfTypeAll<global::Il2Cpp.Firewall>();
            if (all == null) return;
            foreach (var f in all)
            {
                if (f == null) continue;
                try
                {
                    var go = f.gameObject;
                    if (go != null && go.scene.IsValid() && go.scene.isLoaded)
                        result.Add(f);
                }
                catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
            }
        });
        return result;
    }

    public static global::Il2Cpp.Router FindRouterById(string switchId)
    {
        if (string.IsNullOrEmpty(switchId)) return null;
        global::Il2Cpp.Router found = null;
        Try(() =>
        {
            foreach (var r in FindAllRouters())
            {
                string id = null;
                try { id = r.switchId; } catch { continue; }
                if (string.Equals(id, switchId, StringComparison.OrdinalIgnoreCase))
                {
                    found = r;
                    break;
                }
            }
        });
        return found;
    }

    public static global::Il2Cpp.Firewall FindFirewallById(string switchId)
    {
        if (string.IsNullOrEmpty(switchId)) return null;
        global::Il2Cpp.Firewall found = null;
        Try(() =>
        {
            foreach (var f in FindAllFirewalls())
            {
                string id = null;
                try { id = f.switchId; } catch { continue; }
                if (string.Equals(id, switchId, StringComparison.OrdinalIgnoreCase))
                {
                    found = f;
                    break;
                }
            }
        });
        return found;
    }

    // ── Router: subnets ──────────────────────────────────────────────────────

    public static bool AddSubnet(global::Il2Cpp.Router router, int vlanId, string subnetCidr)
    {
        if (router == null || string.IsNullOrWhiteSpace(subnetCidr)) return false;
        try
        {
            var _ = router.gameObject; // liveness
            return router.AddSubnet(vlanId, subnetCidr);
        }
        catch (Exception ex)
        {
            Warn($"AddSubnet failed: {Base(ex)}");
            return false;
        }
    }

    public static bool RemoveSubnet(global::Il2Cpp.Router router, int vlanId)
    {
        if (router == null) return false;
        try
        {
            var _ = router.gameObject; // liveness
            router.RemoveSubnet(vlanId);
            return true;
        }
        catch (Exception ex)
        {
            Warn($"RemoveSubnet failed: {Base(ex)}");
            return false;
        }
    }

    // ── Router: routes ───────────────────────────────────────────────────────

    public static int AddRoute(global::Il2Cpp.Router router, int sourceVlanId, string sourceIp, int targetVlanId, string targetIp)
    {
        if (router == null) return -1;
        try
        {
            var _ = router.gameObject; // liveness
            return router.AddRoute(sourceVlanId, sourceIp ?? "", targetVlanId, targetIp ?? "");
        }
        catch (Exception ex)
        {
            Warn($"AddRoute failed: {Base(ex)}");
            return -1;
        }
    }

    public static bool RemoveRoute(global::Il2Cpp.Router router, int routeId)
    {
        if (router == null) return false;
        try
        {
            var _ = router.gameObject; // liveness
            router.RemoveRoute(routeId);
            return true;
        }
        catch (Exception ex)
        {
            Warn($"RemoveRoute failed: {Base(ex)}");
            return false;
        }
    }

    public static bool ReapplyAllRoutes(global::Il2Cpp.Router router)
    {
        if (router == null) return false;
        try
        {
            var _ = router.gameObject; // liveness
            router.ReapplyAllRoutes();
            return true;
        }
        catch (Exception ex)
        {
            Warn($"ReapplyAllRoutes failed: {Base(ex)}");
            return false;
        }
    }

    public static bool SyncRoutesWithSameAsn(global::Il2Cpp.Router router)
    {
        if (router == null) return false;
        try
        {
            var _ = router.gameObject; // liveness
            router.SyncRoutesWithSameAsn();
            return true;
        }
        catch (Exception ex)
        {
            Warn($"SyncRoutesWithSameAsn failed: {Base(ex)}");
            return false;
        }
    }

    // ── Firewall: rules ──────────────────────────────────────────────────────

    public static bool AddRule(global::Il2Cpp.Firewall firewall, int portIndex, string sourceIpCidr,
        string destIpCidr, int networkPort, string protocol, bool bidirectional, bool allow)
    {
        if (firewall == null) return false;
        global::Il2Cpp.Firewall.Protocol proto = global::Il2Cpp.Firewall.Protocol.TCP;
        try
        {
            if (!string.IsNullOrWhiteSpace(protocol)
                && Enum.TryParse<global::Il2Cpp.Firewall.Protocol>(protocol, true, out var p))
                proto = p;
        }
        catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
        try
        {
            var _ = firewall.gameObject; // liveness
            firewall.AddRule(portIndex, sourceIpCidr ?? "", destIpCidr ?? "",
                networkPort, proto, bidirectional, allow);
            return true;
        }
        catch (Exception ex)
        {
            Warn($"AddRule failed: {Base(ex)}");
            return false;
        }
    }

    public static bool RemoveRule(global::Il2Cpp.Firewall firewall, int portIndex, int vlanId,
        string sourceIpCidr, string destIpCidr, int networkPort)
    {
        if (firewall == null) return false;
        try
        {
            var _ = firewall.gameObject; // liveness
            firewall.RemoveRule(portIndex, vlanId, sourceIpCidr ?? "", destIpCidr ?? "", networkPort);
            return true;
        }
        catch (Exception ex)
        {
            Warn($"RemoveRule failed: {Base(ex)}");
            return false;
        }
    }

    public static bool IsTrafficAllowed(global::Il2Cpp.Firewall firewall, int portIndex, int vlanId,
        string sourceIp, string destIp, int networkPort, string protocol)
    {
        if (firewall == null) return false;
        global::Il2Cpp.Firewall.Protocol proto = global::Il2Cpp.Firewall.Protocol.TCP;
        try
        {
            if (!string.IsNullOrWhiteSpace(protocol)
                && Enum.TryParse<global::Il2Cpp.Firewall.Protocol>(protocol, true, out var p))
                proto = p;
        }
        catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
        try
        {
            var _ = firewall.gameObject; // liveness
            return firewall.IsTrafficAllowed(portIndex, vlanId, sourceIp ?? "", destIp ?? "", networkPort, proto);
        }
        catch (Exception ex)
        {
            Warn($"IsTrafficAllowed failed: {Base(ex)}");
            return false;
        }
    }

    public static bool SyncRulesFromCluster(global::Il2Cpp.Firewall firewall)
    {
        if (firewall == null) return false;
        try
        {
            var _ = firewall.gameObject; // liveness
            firewall.SyncRulesFromCluster();
            return true;
        }
        catch (Exception ex)
        {
            Warn($"SyncRulesFromCluster failed: {Base(ex)}");
            return false;
        }
    }

    public static bool BroadcastRulesToCluster(global::Il2Cpp.Firewall firewall)
    {
        if (firewall == null) return false;
        try
        {
            var _ = firewall.gameObject; // liveness
            firewall.BroadcastRulesToCluster();
            return true;
        }
        catch (Exception ex)
        {
            Warn($"BroadcastRulesToCluster failed: {Base(ex)}");
            return false;
        }
    }

    public static string GetClusterIP(global::Il2Cpp.Firewall firewall)
    {
        if (firewall == null) return "";
        try
        {
            var _ = firewall.gameObject; // liveness
            return firewall.clusterIP ?? "";
        }
        catch { return ""; }
    }

    private static string Base(Exception ex)
    {
        try { return ex != null ? ex.GetBaseException().Message : "?"; } catch { return "?"; }
    }

    private static void Warn(string message)
    {
        try { MelonLogger.Warning($"[gregCore][Net] Routers: {message}"); } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
    }

    private static void Try(Action action)
    {
        try { action?.Invoke(); }
        catch (Exception ex)
        {
            try { MelonLogger.Warning($"[gregCore][Net] Routers field failed: {ex.GetBaseException().Message}"); } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
        }
    }
}
