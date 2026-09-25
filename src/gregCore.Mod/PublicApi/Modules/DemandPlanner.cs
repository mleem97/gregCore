/// <file-summary>
/// Layer:       PublicApi (Modules)
/// Purpose:     Pure, game-free decision logic for demand automation:
///               deficit calculation with demand multiplier (IOPS scaling),
///               route keys, product selection, transitions. Deliberately with
///               no Il2Cpp/Unity access so it is 100% covered by unit tests.
///               Il2Cpp access lives thinly in GregDemandModule.
/// </file-summary>

namespace gregCore.PublicApi.Modules;

public sealed class DemandOptions
{
    public bool AutoRouteSubnets { get; set; }
    public bool AutoFeedPerformance { get; set; }
    public bool AutoAddProducts { get; set; }
    public bool DryRun { get; set; } = true;
    // Quiet: timer scans log no details (no per-item spam).
    // Only diagnostics (forced) log verbosely.
    public bool Quiet { get; set; }
    // Demand multiplier from the economy (1.0 = normal): scales the
    // injected IOPS performance. < 1 = customers get less, > 1 = more.
    public float DemandMultiplier { get; set; } = 1f;
    // Single writer in multiplayer: only host (or solo) may change the world.
    // Clients observe. Default true = solo behavior.
    public bool CanWriteWorld { get; set; } = true;
    // App IDs that may be added as new products.
    public int[] ProductAppIds { get; set; } = Array.Empty<int>();
    public int ProductDifficulty { get; set; } = 1;
    // High-end customers: per-customer difficulty (1 + customerID * step).
    // Default off = vanilla behavior. Host only, never DryRun.
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
    public static float DefaultEpsilon { get; } = 0.001f;

    public static (bool DoRoute, bool DoFeed, bool DoProducts) ResolveActions(DemandOptions options)
    {
        bool write = !options.DryRun && options.CanWriteWorld;
        return (options.AutoRouteSubnets && write,
                options.AutoFeedPerformance && write,
                options.AutoAddProducts && write);
    }

    // Deficit scales with demand: multiplier 0 = no feed,
    // 1 = exact deficit, 2 = double deficit (oversupply for growth).
    public static List<(int AppId, float Amount)> ComputeFeed(
        float[] required, float[] current, float multiplier)
    {
        return ComputeFeed(required, current, multiplier, DefaultEpsilon);
    }

    public static List<(int AppId, float Amount)> ComputeFeed(
        float[] required, float[] current, float multiplier, float epsilon)
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

    // Which desired products is the customer still missing? Only IDs not yet
    // provisioned - the rest is game state, no guessing.
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

    // "1,2, 3" -> [1,2,3]. Empty/invalid parts are ignored.
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

    // True exactly on the unserved -> served transition while
    // automation is active. wasMet == null = customer first seen.
    public static bool IsNewlySatisfied(bool? wasMet, bool metAfter, bool automationActive)
    {
        if (!automationActive) return false;
        if (!metAfter) return false;
        return wasMet != true;
    }

    // --- Demand Generation (CronWorker) ---

    // Poisson distribution: inter-arrival time in seconds for
    // randomly distributed demand events.
    public static float SamplePoissonInterval(float ratePerSecond, System.Random? rng = null)
    {
        if (ratePerSecond <= 0f) return float.MaxValue;
        rng ??= System.Random.Shared;
        // Inverse Transform Sampling: -ln(U) / lambda
        double u = 1.0 - rng.NextDouble(); // (0,1]
        return (float)(-Math.Log(u) / ratePerSecond);
    }

    // Randomly selects apps that receive a demand increase.
    // maxApps: maximum affected apps per event (0 = all).
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

    // Scales a speed requirement for a given app.
    // factor > 1.0 = demand increase, < 1.0 = decrease.
    public static float ScaleSpeedRequirement(float currentRequired, float factor)
    {
        if (factor <= 0f) return 0f;
        return currentRequired * factor;
    }

    // Computes the deadline in seconds from now.
    // baseTimeout: the customer's CustomerBase.howLongToWaitBeforeFine.
    // scaleFactor: scaling factor (1.0 = identical to game default).
    public static int ComputeDemandTimeout(int baseTimeout, float scaleFactor)
    {
        if (baseTimeout <= 0) baseTimeout = 30; // Fallback
        if (scaleFactor <= 0f) scaleFactor = 1f;
        return Math.Max(5, (int)(baseTimeout * scaleFactor));
    }

    // --- DemandShift (customer changes) ---

    // Type of customer change: Surge = demand up, Dip = demand down,
    // NewService = new product (new app + subnet).
    public enum DemandShiftKind
    {
        Surge = 0,
        Dip = 1,
        NewService = 2,
    }

    // Weighted random selection: Surge 50%, Dip 30%, NewService 20%.
    // Disabled kinds drop out, the rest is renormalized.
    // With an empty product pool, NewService drops out as well.
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

    // Dip feed: inject surplus (requirement effectively reduced).
    // amount = share of the requirement as bonus (0.3 = 30% on top).
    public static List<(int AppId, float Amount)> ComputeDipFeed(
        float[] required, float amount)
    {
        return ComputeDipFeed(required, amount, DefaultEpsilon);
    }

    public static List<(int AppId, float Amount)> ComputeDipFeed(
        float[] required, float amount, float epsilon)
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

    // New desired products from the pool that the customer does not have yet.
    // Order = pool order (deterministic, no guessing).
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

    // --- High-end customers (community request) ---

    // Difficulty multiplier per customer: 1 + id * step.
    // id 0 -> 1.0 (vanilla), id 1 -> 1.1, ..., id 33 -> 4.3 (step 0.1).
    // Negative IDs/steps are clamped to 1.0 (never easier).
    public static float HighEndMultiplier(int customerId, float step)
    {
        if (customerId <= 0 || step <= 0f) return 1f;
        return 1f + customerId * step;
    }

    // Upkeep drain: keeps high-end customers permanently under pressure by subtracting
    // the margin (requirement * (multiplier - 1)) per scan.
    // The player must overprovision accordingly - exactly the
    // "high-end" feel. Never drain below 0, never more than available.
    public static List<(int AppId, float Amount)> ComputeHighEndDrain(
        float[] required, float[] current, float multiplier)
    {
        return ComputeHighEndDrain(required, current, multiplier, DefaultEpsilon);
    }

    public static List<(int AppId, float Amount)> ComputeHighEndDrain(
        float[] required, float[] current, float multiplier, float epsilon)
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
