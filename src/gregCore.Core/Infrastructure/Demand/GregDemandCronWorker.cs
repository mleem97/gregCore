/// <file-summary>
/// Layer:       Infrastructure (Demand)
/// Purpose:     CronWorker for demand events. Runs as MonoBehaviour
///               and listens to TimeController (game time) to fire
///               demand events at fixed intervals.
///               Host-only in multiplayer: clients only receive NetMsgs.
/// Maintainer:  No game logic here: only timer + event dispatch.
///               The actual customer manipulation lives in GregDemandModule.
/// </file-summary>

using System;
using System.Diagnostics.CodeAnalysis;
using gregCore.Core.Models;
using gregCore.PublicApi.Modules;
using MelonLoader;
using UnityEngine;

namespace gregCore.Infrastructure.Demand;

[ExcludeFromCodeCoverage(Justification = "MonoBehaviour over live game runtime; pure timing logic tested separately.")]
public sealed class GregDemandCronWorker : MonoBehaviour
{
    public GregDemandCronWorker(IntPtr ptr) : base(ptr) { }

    // Configuration
    private float _intervalSeconds = 180f; // 3-minute default
    private float _speedScaleFactor = 1.3f; // 30% demand increase (Surge)
    private float _dipAmount = 0.3f; // 30% bonus feed-in (Dip)
    private float _timeoutScaleFactor = 1.0f;
    private int _maxAppsPerEvent = 3;
    private bool _allowDip = true;
    private bool _allowNewService = true;
    private int[] _productAppIds = System.Array.Empty<int>();
    private int _productDifficulty = 1;
    private float _ratePerSecond;

    // State
    private float _timer;
    private float _nextPoissonDelay;
    private bool _initialized;
    private bool _active;
    private readonly System.Random _rng = new();

    // Events
    public event Action<DemandEvent>? OnDemandEvent;

    /// <summary>
    /// Interval in seconds between demand events.
    /// Set by DemandIntegration (gregMod.Economics).
    /// </summary>
    public float IntervalSeconds
    {
        get => _intervalSeconds;
        set
        {
            _intervalSeconds = Math.Max(30f, value);
            _ratePerSecond = 1f / _intervalSeconds;
            _nextPoissonDelay = DemandPlanner.SamplePoissonInterval(_ratePerSecond, _rng);
        }
    }

    public float SpeedScaleFactor
    {
        get => _speedScaleFactor;
        set => _speedScaleFactor = Math.Max(1.01f, value);
    }

    // Dip strength: share of the demand granted as bonus (0 = off).
    public float DipAmount
    {
        get => _dipAmount;
        set => _dipAmount = Math.Max(0f, value);
    }

    public bool AllowDip
    {
        get => _allowDip;
        set => _allowDip = value;
    }

    public bool AllowNewService
    {
        get => _allowNewService;
        set => _allowNewService = value;
    }

    public int[] ProductAppIds
    {
        get => _productAppIds;
        set => _productAppIds = value ?? System.Array.Empty<int>();
    }

    public int ProductDifficulty
    {
        get => _productDifficulty;
        set => _productDifficulty = Math.Max(1, value);
    }

    public float TimeoutScaleFactor
    {
        get => _timeoutScaleFactor;
        set => _timeoutScaleFactor = Math.Max(0.1f, value);
    }

    public int MaxAppsPerEvent
    {
        get => _maxAppsPerEvent;
        set => _maxAppsPerEvent = Math.Max(0, value);
    }

    public bool IsActive => _active;

    public void Activate()
    {
        _active = true;
        _timer = 0f;
        _ratePerSecond = 1f / _intervalSeconds;
        _nextPoissonDelay = DemandPlanner.SamplePoissonInterval(_ratePerSecond, _rng);
        _initialized = true;
        MelonLogger.Msg($"[gregCore][Demand] CronWorker enabled: interval {_intervalSeconds:F0}s, Poisson mode.");
    }

    public void Deactivate()
    {
        _active = false;
        MelonLogger.Msg("[gregCore][Demand] CronWorker disabled.");
    }

    // Called by Unity (every frame).
    // Bound to TimeController (game time), not real time.
    void Update()
    {
        if (!_active || !_initialized) return;

        // Only run on the host (single-player always counts as host).
        try
        {
            if (!gregCore.Infrastructure.Networking.GregNetSession.CanMutateWorld) return;
        }
        catch { return; }

        // Respect game slow-motion/pause.
        float gameSpeed;
        try
        {
            var tc = UnityEngine.Object.FindObjectOfType<global::Il2Cpp.TimeController>();
            if (tc == null) return;
            gameSpeed = tc.timeMultiplier;
        }
        catch { return; }

        if (gameSpeed <= 0f) return; // Pause

        float dt = Time.deltaTime * gameSpeed;
        _timer += dt;

        if (_timer < _nextPoissonDelay) return;

        // Fire event
        _timer = 0f;
        _nextPoissonDelay = DemandPlanner.SamplePoissonInterval(_ratePerSecond, _rng);
        FireDemandEvent();
    }

    private void FireDemandEvent()
    {
        try
        {
            var customers = UnityEngine.Object.FindObjectsOfType<global::Il2Cpp.CustomerBase>();
            if (customers == null || customers.Length == 0) return;

            // Pick a random customer
            int customerIdx = _rng.Next(customers.Length);
            var customer = customers[customerIdx];
            if (customer == null) return;

            int customerId = -1;
            try { customerId = customer.customerID; } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }

            // Roll the shift kind (Surge/Dip/NewService)
            bool hasPool = _productAppIds != null && _productAppIds.Length > 0;
            var kind = DemandPlanner.PickShiftKind(_rng, _allowDip, _allowNewService, hasPool);

            if (kind == DemandPlanner.DemandShiftKind.NewService)
            {
                FireNewService(customer, customerId, customerIdx);
                return;
            }

            // Collect apps
            int appIdCount;
            try
            {
                var req = customer.GetAppsSpeedRequirements();
                if (req == null || req.Length == 0) return;
                appIdCount = req.Length;
            }
            catch { return; }

            // Select random apps
            var targets = DemandPlanner.SelectDemandTargets(appIdCount, _maxAppsPerEvent, _rng);
            if (targets.Count == 0) return;

            if (kind == DemandPlanner.DemandShiftKind.Dip)
                FireDip(customer, customerId, customerIdx, targets);
            else
                FireSurge(customer, customerId, customerIdx, targets);
        }
        catch (Exception ex)
        {
            MelonLogger.Error($"[gregCore][Demand] CronWorker error: {ex.GetBaseException().Message}");
        }
    }

    // Surge: create a deficit (lower speed) = demand up.
    private void FireSurge(global::Il2Cpp.CustomerBase customer, int customerId, int customerIdx,
        System.Collections.Generic.List<int> targets)
    {
        float totalIncrease = 0f;
        try
        {
            var req = customer.GetAppsSpeedRequirements();
            if (req == null) return;

            foreach (int appId in targets)
            {
                if (appId < 0 || appId >= req.Length) continue;
                float current = req[appId];
                float scaled = DemandPlanner.ScaleSpeedRequirement(current, _speedScaleFactor);
                float increase = scaled - current;
                if (increase > 0f)
                {
                    customer.AddAppPerformance(appId, -increase); // Lowering speed = creating demand
                    totalIncrease += increase;
                }
            }
        }
        catch (Exception ex)
        {
            MelonLogger.Warning($"[gregCore][Demand] Error in speed manipulation: {ex.GetBaseException().Message}");
            return;
        }

        if (totalIncrease <= 0f) return;

        var evt = new DemandEvent
        {
            Kind = DemandEventKind.Surge,
            CustomerId = customerId,
            CustomerIndex = customerIdx,
            AffectedAppIds = targets,
            SpeedIncrease = totalIncrease,
            TimeoutSeconds = ReadTimeout(customer),
            FiredAtUtc = DateTime.UtcNow,
        };

        MelonLogger.Msg($"[gregCore][Demand] Surge: Kunde {customerId}, " +
            $"{targets.Count} Apps, Bedarf +{totalIncrease:F1}, Timeout {evt.TimeoutSeconds}s.");

        OnDemandEvent?.Invoke(evt);
    }

    // Dip: feed in surplus (demand down, customer relaxes).
    private void FireDip(global::Il2Cpp.CustomerBase customer, int customerId, int customerIdx,
        System.Collections.Generic.List<int> targets)
    {
        float totalBonus = 0f;
        try
        {
            var req = customer.GetAppsSpeedRequirements();
            if (req == null) return;
            var required = new float[req.Length];
            for (int i = 0; i < req.Length; i++) required[i] = req[i];
            foreach (var (appId, amount) in DemandPlanner.ComputeDipFeed(required, _dipAmount))
            {
                if (!targets.Contains(appId)) continue;
                customer.AddAppPerformance(appId, amount);
                totalBonus += amount;
            }
        }
        catch (Exception ex)
        {
            MelonLogger.Warning($"[gregCore][Demand] Error in dip feed: {ex.GetBaseException().Message}");
            return;
        }

        if (totalBonus <= 0f) return;

        var evt = new DemandEvent
        {
            Kind = DemandEventKind.Dip,
            CustomerId = customerId,
            CustomerIndex = customerIdx,
            AffectedAppIds = targets,
            SpeedIncrease = -totalBonus,
            TimeoutSeconds = ReadTimeout(customer),
            FiredAtUtc = DateTime.UtcNow,
        };

        MelonLogger.Msg($"[gregCore][Demand] Dip: Kunde {customerId}, " +
            $"{targets.Count} Apps, Bedarf -{totalBonus:F1}.");

        OnDemandEvent?.Invoke(evt);
    }

    // NewService: set up a new product (new app + subnet).
    // Cabling/routing is handled by the demand scan (GregDemandModule).
    private void FireNewService(global::Il2Cpp.CustomerBase customer, int customerId, int customerIdx)
    {
        try
        {
            var existing = new System.Collections.Generic.List<int>();
            try
            {
                var subnets = customer.GetSubnetsPerApp();
                if (subnets != null)
                {
                    foreach (var kv in subnets) existing.Add(kv.Key);
                }
            }
            catch { return; }

            var fresh = DemandPlanner.ComputeNewServiceIds(existing, _productAppIds);
            if (fresh.Count == 0) return;
            int appId = fresh[_rng.Next(fresh.Count)];

            customer.SetUpApp(appId, _productDifficulty);

            var evt = new DemandEvent
            {
                Kind = DemandEventKind.NewService,
                CustomerId = customerId,
                CustomerIndex = customerIdx,
                AffectedAppIds = new System.Collections.Generic.List<int> { appId },
                NewAppId = appId,
                TimeoutSeconds = ReadTimeout(customer),
                FiredAtUtc = DateTime.UtcNow,
            };

            MelonLogger.Msg($"[gregCore][Demand] NewService: Kunde {customerId}, " +
                $"neues Produkt App {appId} (Schwierigkeit {_productDifficulty}).");

            OnDemandEvent?.Invoke(evt);
        }
        catch (Exception ex)
        {
            MelonLogger.Warning($"[gregCore][Demand] Error in new service: {ex.GetBaseException().Message}");
        }
    }

    private int ReadTimeout(global::Il2Cpp.CustomerBase customer)
    {
        try
        {
            int baseTimeout = customer.howLongToWaitBeforeFine;
            return DemandPlanner.ComputeDemandTimeout(baseTimeout, _timeoutScaleFactor);
        }
        catch
        {
            return 30;
        }
    }
}

/// <summary>
/// Kind of customer change (shift).
/// </summary>
public enum DemandEventKind
{
    Surge = 0,
    Dip = 1,
    NewService = 2,
}

/// <summary>
/// Data model for a demand event fired by the CronWorker.
/// </summary>
public sealed class DemandEvent
{
    public DemandEventKind Kind { get; init; } = DemandEventKind.Surge;
    public int CustomerId { get; init; }
    public int CustomerIndex { get; init; }
    public List<int> AffectedAppIds { get; init; } = new();
    public float SpeedIncrease { get; init; }
    public int NewAppId { get; init; } = -1;
    public int TimeoutSeconds { get; init; }
    public DateTime FiredAtUtc { get; init; }
}
