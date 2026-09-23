/// <file-summary>
/// Schicht:      PublicApi (Modules)
/// Zweck:        Reine, spiel-freie Entscheidungslogik der Demand-Automatisierung:
///               Defizit-Berechnung mit Nachfrage-Multiplier (IOPS-Skalierung),
///               Route-Keys, Produkt-Auswahl, Transitions. Absichtlich ohne
///               jeden Il2Cpp-/Unity-Zugriff, damit sie zu 100 % per Unit-Test
///               abgedeckt ist. Der Il2Cpp-Zugriff lebt duenn in GregDemandModule.
/// </file-summary>

namespace gregCore.PublicApi.Modules;

public sealed class DemandOptions
{
    public bool AutoRouteSubnets { get; set; }
    public bool AutoFeedPerformance { get; set; }
    public bool AutoAddProducts { get; set; }
    public bool DryRun { get; set; } = true;
    // Quiet: Timer-Scans loggen keine Einzelheiten (kein Per-Item-Spam).
    // Nur Diagnose (forced) loggt ausfuehrlich.
    public bool Quiet { get; set; }
    // Nachfrage-Multiplier aus der Economy (1.0 = normal): skaliert die
    // eingespeiste IOPS-Leistung. < 1 = Kunden bekommen weniger, > 1 = mehr.
    public float DemandMultiplier { get; set; } = 1f;
    // Single-Writer im Multiplayer: nur Host (bzw. Solo) darf die Welt
    // veraendern. Clients beobachten. Default true = Solo-Verhalten.
    public bool CanWriteWorld { get; set; } = true;
    // App-IDs, die als neue Produkte hinzugefuegt werden duerfen.
    public int[] ProductAppIds { get; set; } = Array.Empty<int>();
    public int ProductDifficulty { get; set; } = 1;
    // HighEnd-Kunden: pro-Kunde-Schwierigkeit (1 + customerID * step).
    // Default aus = Vanilla-Verhalten. Nur Host, nie DryRun.
    public bool HighEndEnabled { get; set; }
    public float HighEndStep { get; set; } = 0.1f;
}

public sealed class DemandScanResult
{
    public int Customers { get; internal set; }
    public int Apps { get; internal set; }
    public int Routed { get; internal set; }
    public float FedTotal { get; internal set; }
    public int ProductsAdded { get; internal set; }
    public float HighEndDrained { get; internal set; }
    public int HighEndCustomers { get; internal set; }
    public List<int> NewlySatisfied { get; } = new();
    public bool DryRun { get; internal set; }

    public string Summary => string.Format(
        System.Globalization.CultureInfo.InvariantCulture,
        "{0} Kunden, {1} Apps | Route: {2} | Speed: {3:F1} | Produkte: {4} | HighEnd: {5:F1} ({6}) | Neu versorgt: {7}{8}",
        Customers, Apps, Routed, FedTotal, ProductsAdded, HighEndDrained, HighEndCustomers,
        NewlySatisfied.Count, DryRun ? " | DRY-RUN" : "");
}

public static class DemandPlanner
{
    public const float DefaultEpsilon = 0.001f;

    public static (bool DoRoute, bool DoFeed, bool DoProducts) ResolveActions(DemandOptions options)
    {
        bool write = !options.DryRun && options.CanWriteWorld;
        return (options.AutoRouteSubnets && write,
                options.AutoFeedPerformance && write,
                options.AutoAddProducts && write);
    }

    // Defizit skaliert mit der Nachfrage: Multiplier 0 = kein Feed,
    // 1 = exaktes Defizit, 2 = doppeltes Defizit (Übererfüllung für Wachstum).
    public static List<(int AppId, float Amount)> ComputeFeed(
        float[] required, float[] current, float multiplier, float epsilon = DefaultEpsilon)
    {
        var feed = new List<(int, float)>();
        if (multiplier <= 0f) return feed;
        int n = Math.Min(required.Length, current.Length);
        for (int appId = 0; appId < n; appId++)
        {
            float deficit = required[appId] - current[appId];
            if (deficit > epsilon) feed.Add((appId, deficit * multiplier));
        }
        return feed;
    }

    public static string BuildRouteKey(int customerId, int appId)
        => "greg-demand-" + customerId + "-" + appId;

    // Welche Wunsch-Produkte fehlen dem Kunden noch? Nur IDs, die noch
    // nicht eingerichtet sind - Rest ist Spielzustand, kein Raten.
    public static List<int> ComputeMissingProducts(
        System.Collections.Generic.ICollection<int> existingAppIds, int[]? productAppIds)
    {
        var missing = new List<int>();
        if (productAppIds == null) return missing;
        foreach (int appId in productAppIds)
        {
            if (!existingAppIds.Contains(appId) && !missing.Contains(appId))
                missing.Add(appId);
        }
        return missing;
    }

    // "1,2, 3" -> [1,2,3]. Leere/ungueltige Teile werden ignoriert.
    public static int[] ParseAppIds(string? text)
    {
        var ids = new List<int>();
        if (string.IsNullOrWhiteSpace(text)) return ids.ToArray();
        foreach (string part in text.Split(','))
        {
            if (int.TryParse(part.Trim(), out int id) && id >= 0 && !ids.Contains(id))
                ids.Add(id);
        }
        return ids.ToArray();
    }

    // True genau beim Uebergang nicht-versorgt -> versorgt, waehrend die
    // Automatisierung aktiv ist. wasMet == null = Kunde erstmals gesehen.
    public static bool IsNewlySatisfied(bool? wasMet, bool metAfter, bool automationActive)
    {
        if (!automationActive) return false;
        if (!metAfter) return false;
        return wasMet != true;
    }

    // --- Demand Generation (CronWorker) ---

    // Poisson-Verteilung: Inter-Arrival-Time in Sekunden fuer
    // zufaellig verteilte Nachfrage-Ereignisse.
    public static float SamplePoissonInterval(float ratePerSecond, System.Random? rng = null)
    {
        if (ratePerSecond <= 0f) return float.MaxValue;
        rng ??= System.Random.Shared;
        // Inverse Transform Sampling: -ln(U) / lambda
        double u = 1.0 - rng.NextDouble(); // (0,1]
        return (float)(-Math.Log(u) / ratePerSecond);
    }

    // Waehlt zufaellig Apps aus, die eine Nachfrage-Erhoehung bekommen.
    // maxApps: Maximum der betroffenen Apps pro Event (0 = alle).
    public static List<int> SelectDemandTargets(int totalApps, int maxApps, System.Random? rng = null)
    {
        rng ??= System.Random.Shared;
        var targets = new List<int>();
        if (totalApps <= 0) return targets;
        int count = maxApps <= 0 || maxApps >= totalApps
            ? totalApps
            : rng.Next(1, Math.Min(maxApps, totalApps) + 1);
        var available = new List<int>(totalApps);
        for (int i = 0; i < totalApps; i++) available.Add(i);
        for (int i = 0; i < count && available.Count > 0; i++)
        {
            int idx = rng.Next(available.Count);
            targets.Add(available[idx]);
            available.RemoveAt(idx);
        }
        return targets;
    }

    // Skaliert eine Speed-Anforderung fuer ein bestimmtes App.
    // factor > 1.0 = Nachfrage-Erhoehung, < 1.0 = Senkung.
    public static float ScaleSpeedRequirement(float currentRequired, float factor)
    {
        if (factor <= 0f) return 0f;
        return currentRequired * factor;
    }

    // Berechnet die Deadline in Sekunden ab jetzt.
    // baseTimeout: das CustomerBase.howLongToWaitBeforeFine des Kunden.
    // scaleFactor: Skalierungsfaktor (1.0 = identisch zum Spiel-Default).
    public static int ComputeDemandTimeout(int baseTimeout, float scaleFactor)
    {
        if (baseTimeout <= 0) baseTimeout = 30; // Fallback
        if (scaleFactor <= 0f) scaleFactor = 1f;
        return Math.Max(5, (int)(baseTimeout * scaleFactor));
    }

    // --- DemandShift (Kunden-Aenderungen) ---

    // Art der Kunden-Aenderung: Surge = Bedarf rauf, Dip = Bedarf runter,
    // NewService = neues Produkt (neue App + Subnetz).
    public enum DemandShiftKind
    {
        Surge = 0,
        Dip = 1,
        NewService = 2,
    }

    // Gewichtete Zufallsauswahl: Surge 50 %, Dip 30 %, NewService 20 %.
    // Deaktivierte Arten fallen raus, Rest wird renormalisiert.
    // Bei leerem Produktpool faellt NewService ebenfalls raus.
    public static DemandShiftKind PickShiftKind(
        System.Random rng, bool allowDip, bool allowService, bool hasProductPool)
    {
        if (rng == null) throw new ArgumentNullException(nameof(rng));
        double surge = 50.0;
        double dip = allowDip ? 30.0 : 0.0;
        double service = (allowService && hasProductPool) ? 20.0 : 0.0;
        double total = surge + dip + service;
        if (total <= 0.0) return DemandShiftKind.Surge;
        double roll = rng.NextDouble() * total;
        if (roll < surge) return DemandShiftKind.Surge;
        if (roll < surge + dip) return DemandShiftKind.Dip;
        return DemandShiftKind.NewService;
    }

    // Dip-Feed: Ueberschuss einspeisen (Anforderung effektiv gesenkt).
    // amount = Anteil der Anforderung als Bonus (0.3 = 30 % on top).
    public static List<(int AppId, float Amount)> ComputeDipFeed(
        float[] required, float amount, float epsilon = DefaultEpsilon)
    {
        var feed = new List<(int, float)>();
        if (required == null || amount <= 0f) return feed;
        for (int appId = 0; appId < required.Length; appId++)
        {
            float bonus = required[appId] * amount;
            if (bonus > epsilon) feed.Add((appId, bonus));
        }
        return feed;
    }

    // Neue Wunsch-Produkte aus dem Pool, die der Kunde noch nicht hat.
    // Reihenfolge = Pool-Reihenfolge (deterministisch, kein Raten).
    public static List<int> ComputeNewServiceIds(
        System.Collections.Generic.ICollection<int> existingAppIds, int[]? productPool)
    {
        var fresh = new List<int>();
        if (productPool == null) return fresh;
        foreach (int appId in productPool)
        {
            if (!existingAppIds.Contains(appId) && !fresh.Contains(appId))
                fresh.Add(appId);
        }
        return fresh;
    }

    // --- HighEnd-Kunden (Community-Wunsch) ---

    // Schwierigkeits-Multiplier pro Kunde: 1 + id * step.
    // id 0 -> 1.0 (Vanilla), id 1 -> 1.1, ..., id 33 -> 4.3 (step 0.1).
    // Negative IDs/Steps werden auf 1.0 geklemmt (niemals leichter).
    public static float HighEndMultiplier(int customerId, float step)
    {
        if (customerId <= 0 || step <= 0f) return 1f;
        return 1f + customerId * step;
    }

    // Upkeep-Drain: haelt HighEnd-Kunden dauerhaft unter Druck, indem pro
    // Scan die Marge (Anforderung * (Multiplier - 1)) abgezogen wird.
    // Der Spieler muss entsprechend ueberprovisionieren - exakt das
    // "HighEnd"-Gefuehl. Nie unter 0 ziehen, nie mehr als vorhanden.
    public static List<(int AppId, float Amount)> ComputeHighEndDrain(
        float[] required, float[] current, float multiplier, float epsilon = DefaultEpsilon)
    {
        var drain = new List<(int, float)>();
        if (required == null || current == null) return drain;
        if (multiplier <= 1f) return drain;
        int n = Math.Min(required.Length, current.Length);
        for (int appId = 0; appId < n; appId++)
        {
            float margin = required[appId] * (multiplier - 1f);
            if (margin <= epsilon) continue;
            float amount = Math.Min(margin, current[appId]);
            if (amount > epsilon) drain.Add((appId, amount));
        }
        return drain;
    }
}
