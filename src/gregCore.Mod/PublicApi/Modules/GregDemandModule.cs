/// <file-summary>
/// Layer:       PublicApi (Modules)
/// Purpose:     Customer demand automation ("customers on demand"):
///               reads per-app demand from CustomerBase (subnetsPerApp,
///               vlanIdsPerApp, usableIpsPerApp, speed requirements - all
///               public API, no Harmony patches) and fulfills it:
///               routing via TryRegisterRoutedSubnet, performance via
///               AddAppPerformance until AreAllAppRequirementsMet().
/// Maintainer:  Read + feed only, do not change game rules.
///               Errors are caught (API drift), there is no UI here -
///               consumers (mods) subscribe to OnToast.
/// </file-summary>

using System.Diagnostics.CodeAnalysis;
using MelonLoader;
using UnityEngine;
using Il2CppInterop.Runtime.InteropTypes.Arrays;

namespace gregCore.PublicApi.Modules;

[ExcludeFromCodeCoverage(Justification = "Thin shell over live game runtime; logic in DemandPlanner.")]
public sealed class GregDemandModule
{
    private readonly GregApiContext _ctx;
    internal GregDemandModule(GregApiContext ctx) => _ctx = ctx;

    // customerId -> was fully served at last scan (for "newly served" events)
    private readonly Dictionary<int, bool> _satisfiedByCustomer = new();
    private bool _memberFailureLogged;

    public event Action<string>? OnToast;

    public DemandScanResult Diagnose() => Scan(new DemandOptions { DryRun = true });

    // After peer change (join): discard memory so all customers
    // are re-evaluated and routes are ensured for the current world.
    public void ResetMemory()
    {
        _satisfiedByCustomer.Clear();
    }

    // Needs the running game (FindObjectsOfType + Il2Cpp calls) and is
    // therefore excluded from unit coverage; the decision logic lives
    // tested in DemandPlanner.
    public DemandScanResult Scan(DemandOptions options)
    {
        var result = new DemandScanResult { DryRun = options.DryRun };
        var customers = FindCustomers();
        if (customers == null) return result;
        foreach (var cb in customers)
        {
            try { ScanOne(cb, options, result); }
            catch (Exception ex) { LogMemberFailure(ex); }
        }
        LogSummary(options, result);
        return result;
    }

    private static global::Il2Cpp.CustomerBase[] FindCustomers()
    {
        try
        {
            return UnityEngine.Object.FindObjectsOfType<global::Il2Cpp.CustomerBase>();
        }
        catch (Exception ex)
        {
            MelonLogger.Error("[gregCore][Demand] Customer scan failed: " + ex.GetBaseException().Message);
            return null;
        }
    }

    private void ScanOne(global::Il2Cpp.CustomerBase cb, DemandOptions options, DemandScanResult result)
    {
        try
        {
            if (cb == null) return;
            int customerId;
            try { customerId = cb.customerID; }
            catch { /* ignored: defensive best-effort (CONVENTIONS.md) */  return; }
            result.Customers++;
            var (doRoute, doFeed, doProducts) = DemandPlanner.ResolveActions(options);
            bool observe = options.DryRun;
            bool doHighEnd = !options.DryRun && options.CanWriteWorld && options.HighEndEnabled;
            bool metBefore = ReadSatisfied(cb);
            RouteAll(cb, customerId, options, result, doRoute);
            FeedAll(cb, customerId, options, result, doFeed, observe);
            EnsureProducts(cb, customerId, options, result, doProducts, observe);
            DrainHighEnd(cb, customerId, options, result, doHighEnd);
            TrackSatisfaction(cb, customerId, options, result, metBefore, doRoute, doFeed, doProducts);
        }
        catch (Exception ex) { LogMemberFailure(ex); }
    }

    private void RouteAll(global::Il2Cpp.CustomerBase cb, int customerId, DemandOptions options, DemandScanResult result, bool doRoute)
    {
        try
        {
            var subnets = ReadSubnets(cb);
            if (subnets == null) return;
            foreach (var kv in subnets)
            {
                result.Apps++;
                result.Routed += TryAutoRoute(cb, customerId, kv.Key, kv.Value, doRoute, options.Quiet);
            }
        }
        catch (Exception ex) { LogMemberFailure(ex); }
    }

    private void FeedAll(global::Il2Cpp.CustomerBase cb, int customerId, DemandOptions options, DemandScanResult result, bool doFeed, bool observe)
    {
        try
        {
            if (doFeed || observe) result.FedTotal += TryAutoFeed(cb, customerId, options, doFeed);
        }
        catch (Exception ex) { LogMemberFailure(ex); }
    }

    private void EnsureProducts(global::Il2Cpp.CustomerBase cb, int customerId, DemandOptions options, DemandScanResult result, bool doProducts, bool observe)
    {
        try
        {
            if (doProducts || observe)
                result.ProductsAdded += TryEnsureProducts(cb, customerId, options, doProducts);
        }
        catch (Exception ex) { LogMemberFailure(ex); }
    }

    private void DrainHighEnd(global::Il2Cpp.CustomerBase cb, int customerId, DemandOptions options, DemandScanResult result, bool doHighEnd)
    {
        try
        {
            if (!doHighEnd) return;
            float drained = TryHighEndUpkeep(cb, customerId, options);
            if (drained <= 0f) return;
            result.HighEndDrained += drained;
            result.HighEndCustomers++;
        }
        catch (Exception ex) { LogMemberFailure(ex); }
    }

    private void TrackSatisfaction(global::Il2Cpp.CustomerBase cb, int customerId, DemandOptions options, DemandScanResult result, bool metBefore, bool doRoute, bool doFeed, bool doProducts)
    {
        try
        {
            bool metAfter = doFeed ? ReadSatisfied(cb) : metBefore;
            bool? wasMet = _satisfiedByCustomer.TryGetValue(customerId, out bool prev) ? prev : null;
            _satisfiedByCustomer[customerId] = metAfter;
            if (!DemandPlanner.IsNewlySatisfied(wasMet, metAfter, doFeed || doRoute || doProducts)) return;
            result.NewlySatisfied.Add(customerId);
            EmitToast($"Customer {customerId} fully supplied.");
            MelonLogger.Msg($"[gregCore][Demand] Customer {customerId} is now fully supplied.");
        }
        catch (Exception ex) { LogMemberFailure(ex); }
    }

    private static void LogSummary(DemandOptions options, DemandScanResult result)
    {
        try
        {
            var (doRoute, doFeed, doProducts) = DemandPlanner.ResolveActions(options);
            if ((doRoute || doFeed || doProducts || options.DryRun) && !options.Quiet)
                MelonLogger.Msg("[gregCore][Demand] Scan: " + result.Summary);
        }
        catch { /* ignored: defensive best-effort (CONVENTIONS.md) */  }
    }

    // New products: provision missing desired apps via SetUpApp.
    // Never guess - only IDs from the options, only if not present yet.
    private int TryEnsureProducts(global::Il2Cpp.CustomerBase cb, int customerId, DemandOptions options, bool write)
    {
        try
        {
            var existing = ReadSubnets(cb);
            var existingIds = existing != null
                ? (System.Collections.Generic.ICollection<int>)existing.Keys
                : System.Array.Empty<int>();
            var missing = DemandPlanner.ComputeMissingProducts(existingIds, options.ProductAppIds);
            if (missing.Count == 0) return 0;
            int added = 0;
            foreach (int appId in missing)
            {
                if (!write && !options.Quiet)
                {
                    MelonLogger.Msg(string.Format(
                        System.Globalization.CultureInfo.InvariantCulture,
                        "[gregCore][Demand][DRY] Kunde {0}: wuerde neues Produkt App {1} einrichten (Schwierigkeit {2}).",
                        customerId, appId, options.ProductDifficulty));
                }
                else
                {
                    cb.SetUpApp(appId, options.ProductDifficulty);
                    MelonLogger.Msg(string.Format(
                        System.Globalization.CultureInfo.InvariantCulture,
                        "[gregCore][Demand] Kunde {0}: neues Produkt App {1} eingerichtet.",
                        customerId, appId));
                }
                added++;
            }
            return added;
        }
        catch (Exception ex) { LogMemberFailure(ex); return 0; }
    }

    private Dictionary<int, string>? ReadSubnets(global::Il2Cpp.CustomerBase cb)
    {
        try
        {
            var raw = cb.GetSubnetsPerApp();
            if (raw == null) return null;
            var managed = new Dictionary<int, string>();
            foreach (var kv in raw) managed[kv.Key] = kv.Value;
            return managed;
        }
        catch (Exception ex) { LogMemberFailure(ex); return null; }
    }

    private bool ReadSatisfied(global::Il2Cpp.CustomerBase cb)
    {
        try { return cb.AreAllAppRequirementsMet(); }
        catch (Exception ex) { LogMemberFailure(ex); return false; }
    }

    private int TryAutoRoute(global::Il2Cpp.CustomerBase cb, int customerId, int appId, string subnet, bool write, bool quiet)
    {
        try
        {
            if (!TryReadVlan(cb, appId, out int vlanId)) return 0;
            string[] ips = ReadUsableIps(cb, appId);
            if (ips == null || ips.Length == 0) return 0;
            string routeKey = DemandPlanner.BuildRouteKey(customerId, appId);
            if (IsRouteRegistered(cb, routeKey)) return 1;
            if (!write) return LogDryRoute(customerId, appId, subnet, vlanId, ips.Length, routeKey, quiet);
            return TryRegisterRoute(cb, customerId, appId, subnet, vlanId, routeKey, ips);
        }
        catch (Exception ex) { LogMemberFailure(ex); return 0; }
    }

    private bool TryReadVlan(global::Il2Cpp.CustomerBase cb, int appId, out int vlanId)
    {
        vlanId = -1;
        try
        {
            var vlans = cb.GetVlanIdsPerApp();
            if (vlans == null || !vlans.TryGetValue(appId, out vlanId)) return false;
            return true;
        }
        catch (Exception ex) { LogMemberFailure(ex); return false; }
    }

    private static int LogDryRoute(int customerId, int appId, string subnet, int vlanId, int ipCount, string routeKey, bool quiet)
    {
        try
        {
            if (!quiet)
                MelonLogger.Msg(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                    "[gregCore][Demand][DRY] Customer {0} App {1}: would route subnet '{2}' on VLAN {3} ({4} IPs, Key '{5}').",
                    customerId, appId, subnet, vlanId, ipCount, routeKey));
            return 1;
        }
        catch { /* ignored: defensive best-effort (CONVENTIONS.md) */  return 1; }
    }

    private int TryRegisterRoute(global::Il2Cpp.CustomerBase cb, int customerId, int appId, string subnet, int vlanId, string routeKey, string[] ips)
    {
        try
        {
            var il2cppIps = new Il2CppStringArray(ips.Length);
            for (int i = 0; i < ips.Length; i++) il2cppIps[i] = ips[i];
            if (!cb.TryRegisterRoutedSubnet(vlanId, routeKey, il2cppIps)) return 0;
            MelonLogger.Msg(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                "[gregCore][Demand] Customer {0} App {1}: routed subnet '{2}' on VLAN {3}.",
                customerId, appId, subnet, vlanId));
            return 1;
        }
        catch (Exception ex) { LogMemberFailure(ex); return 0; }
    }

    // Idempotency: do not write already registered routes again.
    // Protects against double application on rescans and after peer changes.
    private bool IsRouteRegistered(global::Il2Cpp.CustomerBase cb, string routeKey)
    {
        try
        {
            var routed = cb.routedSubnets;
            if (routed == null) return false;
            return routed.ContainsKey(routeKey);
        }
        catch (Exception ex) { LogMemberFailure(ex); return false; }
    }

    private string[]? ReadUsableIps(global::Il2Cpp.CustomerBase cb, int appId)
    {
        try
        {
            var perApp = cb.usableIpsPerApp;
            if (perApp == null) return null;
            Il2CppStringArray? arr;
            try { if (!perApp.TryGetValue(appId, out arr)) return null; }
            catch { /* ignored: defensive best-effort (CONVENTIONS.md) */  return null; }
            if (arr == null) return null;
            var managed = new string[arr.Length];
            for (int i = 0; i < arr.Length; i++) managed[i] = arr[i];
            return managed;
        }
        catch (Exception ex) { LogMemberFailure(ex); return null; }
    }

    private float TryAutoFeed(global::Il2Cpp.CustomerBase cb, int customerId, DemandOptions options, bool write)
    {
        try
        {
            var req = cb.GetAppsSpeedRequirements();
            var cur = cb.appsSpeedCurrent;
            if (req == null || cur == null) return 0f;
            int n = Math.Min(req.Length, cur.Length);
            var required = new float[n];
            var current = new float[n];
            for (int i = 0; i < n; i++) { required[i] = req[i]; current[i] = cur[i]; }
            float fed = 0f;
            foreach (var (appId, amount) in DemandPlanner.ComputeFeed(required, current, options.DemandMultiplier))
            {
                if (!write && !options.Quiet)
                {
                    MelonLogger.Msg(string.Format(System.Globalization.CultureInfo.InvariantCulture, 
                        "[gregCore][Demand][DRY] Kunde {0} App {1}: wuerde {2:F1} Speed einspeisen (Soll {3:F1}, Ist {4:F1}, Nachfrage x{5:F2}).",
                        customerId, appId, amount, required[appId], current[appId], options.DemandMultiplier));
                }
                else
                {
                    cb.AddAppPerformance(appId, amount);
                }
                fed += amount;
            }
            return fed;
        }
        catch (Exception ex) { LogMemberFailure(ex); return 0f; }
    }

    private void EmitToast(string message)
    {
        try { OnToast?.Invoke(message); }
        catch (Exception ex) { MelonLogger.Warning($"[gregCore][Demand] Toast handler failed: {ex.Message}"); }
    }

    // High-end upkeep: subtract the margin (req * (mult - 1)) per app, max. stock.
    // Returns the total (0 = nothing to do / vanilla customer).
    private float TryHighEndUpkeep(global::Il2Cpp.CustomerBase cb, int customerId, DemandOptions options)
    {
        try
        {
            float mult = DemandPlanner.HighEndMultiplier(customerId, options.HighEndStep);
            if (mult <= 1f) return 0f;
            var req = cb.GetAppsSpeedRequirements();
            var cur = cb.appsSpeedCurrent;
            if (req == null || cur == null) return 0f;
            int n = Math.Min(req.Length, cur.Length);
            var required = new float[n];
            var current = new float[n];
            for (int i = 0; i < n; i++) { required[i] = req[i]; current[i] = cur[i]; }
            float drained = 0f;
            foreach (var (appId, amount) in DemandPlanner.ComputeHighEndDrain(required, current, mult))
            {
                cb.AddAppPerformance(appId, -amount);
                drained += amount;
            }
            return drained;
        }
        catch (Exception ex) { LogMemberFailure(ex); return 0f; }
    }

    private void LogMemberFailure(Exception ex)
    {
        if (_memberFailureLogged) return;
        _memberFailureLogged = true;
        MelonLogger.Error("[gregCore][Demand] API access failed (Game update? Names drifted): "
            + ex.GetBaseException().Message);
        EmitToast("Demand automation: API error, see log.");
    }
}
