/// <file-summary>
/// Schicht:      PublicApi (Modules)
/// Zweck:        Customer-Demand-Automatisierung ("Kunden on demand"):
///               liest pro App Bedarf aus CustomerBase (subnetsPerApp,
///               vlanIdsPerApp, usableIpsPerApp, Speed-Anforderungen - alles
///               public API, keine Harmony-Patches) und erfuellt ihn:
///               Routing via TryRegisterRoutedSubnet, Performance via
///               AddAppPerformance bis AreAllAppRequirementsMet().
/// Maintainer:   Nur lesen + fuettern, keine Spielregeln aendern.
///               Fehler werden abgefangen (API-Drift), UI gibt es hier keine -
///               Consumer (Mods) abonnieren OnToast.
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

    // customerId -> war beim letzten Scan voll versorgt (fuer "neu versorgt"-Events)
    private readonly Dictionary<int, bool> _satisfiedByCustomer = new();
    private bool _memberFailureLogged;

    public event Action<string>? OnToast;

    public DemandScanResult Diagnose() => Scan(new DemandOptions { DryRun = true });

    // Nach Peer-Wechsel (Join): Gedächtnis verwerfen, damit alle Kunden
    // neu bewertet und Routen für die aktuelle Welt sichergestellt werden.
    public void ResetMemory()
    {
        _satisfiedByCustomer.Clear();
    }

    // Braucht das laufende Spiel (FindObjectsOfType + Il2Cpp-Calls) und ist
    // darum vom Unit-Coverage ausgenommen; die Entscheidungslogik steckt
    // getestet in DemandPlanner.
    public DemandScanResult Scan(DemandOptions options)
    {
        var result = new DemandScanResult { DryRun = options.DryRun };
        var (doRoute, doFeed, doProducts) = DemandPlanner.ResolveActions(options);
        bool observe = options.DryRun;
        // HighEnd-Upkeep: nur Host, nie DryRun, nur wenn eingeschaltet.
        bool doHighEnd = !options.DryRun && options.CanWriteWorld && options.HighEndEnabled;

        global::Il2Cpp.CustomerBase[]? customers;
        try
        {
            customers = UnityEngine.Object.FindObjectsOfType<global::Il2Cpp.CustomerBase>();
        }
        catch (Exception ex)
        {
            MelonLogger.Error("[gregCore][Demand] Kundenscan fehlgeschlagen: " + ex.GetBaseException().Message);
            return result;
        }

        if (customers == null) return result;

        foreach (var cb in customers)
        {
            if (cb == null) continue;
            int customerId;
            try { customerId = cb.customerID; }
            catch { continue; } // destroyed object

            result.Customers++;
            try
            {
                bool metBefore = ReadSatisfied(cb);
                var subnets = ReadSubnets(cb);
                if (subnets != null)
                {
                    foreach (var kv in subnets)
                    {
                        result.Apps++;
                        result.Routed += TryAutoRoute(cb, customerId, kv.Key, kv.Value, options.DryRun, doRoute, options.Quiet);
                    }
                }
                if (doFeed || observe) result.FedTotal += TryAutoFeed(cb, customerId, options, doFeed);

                if (doProducts || observe)
                    result.ProductsAdded += TryEnsureProducts(cb, customerId, options, doProducts);

                // HighEnd-Upkeep NACH dem Feed: Marge abziehen, damit der
                // Kunde dauerhaft (Multiplier - 1) mehr braucht. Nur Host.
                if (doHighEnd)
                {
                    float drained = TryHighEndUpkeep(cb, customerId, options);
                    if (drained > 0f)
                    {
                        result.HighEndDrained += drained;
                        result.HighEndCustomers++;
                    }
                }

                bool metAfter = doFeed ? ReadSatisfied(cb) : metBefore;
                bool? wasMet = _satisfiedByCustomer.TryGetValue(customerId, out bool prev) ? prev : null;
                _satisfiedByCustomer[customerId] = metAfter;
                if (DemandPlanner.IsNewlySatisfied(wasMet, metAfter, doFeed || doRoute || doProducts))
                {
                    result.NewlySatisfied.Add(customerId);
                    EmitToast($"Kunde {customerId} vollstaendig versorgt.");
                    MelonLogger.Msg($"[gregCore][Demand] Kunde {customerId} ist jetzt vollstaendig versorgt.");
                }
            }
            catch (Exception ex)
            {
                LogMemberFailure(ex);
            }
        }

        if ((doRoute || doFeed || doProducts || options.DryRun) && !options.Quiet)
            MelonLogger.Msg("[gregCore][Demand] Scan: " + result.Summary);
        return result;
    }

    // Neue Produkte: richtet fehlende Wunsch-Apps per SetUpApp ein.
    // Niemals raten - nur IDs aus den Optionen, nur wenn noch nicht vorhanden.
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

    private int TryAutoRoute(global::Il2Cpp.CustomerBase cb, int customerId, int appId, string subnet, bool dryRun, bool write, bool quiet)
    {
        try
        {
            int vlanId = -1;
            try
            {
                var vlans = cb.GetVlanIdsPerApp();
                if (vlans == null || !vlans.TryGetValue(appId, out vlanId)) return 0;
            }
            catch (Exception ex) { LogMemberFailure(ex); return 0; }

            string[]? ips = ReadUsableIps(cb, appId);
            if (ips == null || ips.Length == 0) return 0;

            string routeKey = DemandPlanner.BuildRouteKey(customerId, appId);
            if (IsRouteRegistered(cb, routeKey)) return 1;
            if (!write)
            {
                if (!quiet)
                    MelonLogger.Msg(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                        "[gregCore][Demand][DRY] Kunde {0} App {1}: wuerde Subnetz '{2}' auf VLAN {3} routen ({4} IPs, Key '{5}').",
                        customerId, appId, subnet, vlanId, ips.Length, routeKey));
                return 1;
            }

            var il2cppIps = new Il2CppStringArray(ips.Length);
            for (int i = 0; i < ips.Length; i++) il2cppIps[i] = ips[i];
            if (cb.TryRegisterRoutedSubnet(vlanId, routeKey, il2cppIps))
            {
                MelonLogger.Msg(string.Format(System.Globalization.CultureInfo.InvariantCulture, 
                    "[gregCore][Demand] Kunde {0} App {1}: Subnetz '{2}' auf VLAN {3} geroutet.",
                    customerId, appId, subnet, vlanId));
                return 1;
            }
            return 0;
        }
        catch (Exception ex) { LogMemberFailure(ex); return 0; }
    }

    // Idempotenz: Bereits registrierte Routen nicht erneut schreiben.
    // Schützt vor Doppel-Anwendung bei Rescans und nach Peer-Wechseln.
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
            catch { return null; }
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
        catch (Exception ex) { MelonLogger.Warning($"[gregCore][Demand] Toast-Handler fehlgeschlagen: {ex.Message}"); }
    }

    // HighEnd-Upkeep: pro App die Marge (Req * (Mult - 1)) abziehen, max. Bestand.
    // Gibt die Summe zurueck (0 = nichts zu tun / Vanilla-Kunde).
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
        MelonLogger.Error("[gregCore][Demand] API-Zugriff fehlgeschlagen (Spielupdate? Namen gedriftet): "
            + ex.GetBaseException().Message);
        EmitToast("Demand-Automatisierung: API-Fehler, siehe Log.");
    }
}
