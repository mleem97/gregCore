/// <file-summary>
/// Schicht:      Core (Networking)
/// Zweck:        Angriffs-Eskalation: Malicious-Attacken werden mit der Zeit
///               schlimmer (kürzere Intervalle, höhere Spawn-Rate/Speed pro
///               Stufe). Liest Vanilla-Basiswerte, skaliert sie stufenweise,
///               alles best-effort. Stufenmathe rein verwaltet (testbar).
/// </file-summary>

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using MelonLoader;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace gregCore.Core.Networking;

[ExcludeFromCodeCoverage(Justification = "Live Il2Cpp interop against game assemblies; needs running game.")]
public static class GregAttackEscalation
{
    // ── Konfiguration ────────────────────────────────────────────────────────

    public sealed class EscalationConfig
    {
        public float TierSeconds { get; set; } = 300f;
        public float IntervalFactor { get; set; } = 0.9f;
        public float MinIntervalSeconds { get; set; } = 20f;
        public float RateGrowthPerTier { get; set; } = 0.15f;
        public float SpeedGrowthPerTier { get; set; } = 0.10f;
        public int MaxTier { get; set; } = 10;
    }

    public sealed class TierStats
    {
        public float AttackInterval;
        public float SpawnRate;
        public float MoveSpeed;
    }

    private static EscalationConfig _config = new EscalationConfig();
    private static bool _running;
    private static float _elapsed;
    private static int _appliedTier = -1;
    private static float _baseInterval = -1f;
    private static float _baseRate = -1f;
    private static float _baseSpeed = -1f;

    public static int CurrentTier => Math.Max(0, _appliedTier);
    public static bool Running => _running;

    // ── Steuerung ────────────────────────────────────────────────────────────

    public static void Start(EscalationConfig config)
    {
        if (config != null) _config = config;
        if (_running) return;
        _running = true;
        _elapsed = 0f;
        _appliedTier = -1;
        _baseInterval = _baseRate = _baseSpeed = -1f;
        try { MelonCoroutines.Start(Pump()); } catch { _running = false; }
    }

    public static void Stop()
    {
        _running = false;
    }

    public static void Reset()
    {
        _elapsed = 0f;
        _appliedTier = -1;
    }

    public static float SecondsToNextTier()
    {
        if (!_running) return -1f;
        float tierLen = Math.Max(1f, _config.TierSeconds);
        int tier = (int)(_elapsed / tierLen);
        return Math.Max(0f, (tier + 1) * tierLen - _elapsed);
    }

    private static IEnumerator Pump()
    {
        var wait = new WaitForSecondsRealtime(1f);
        while (_running)
        {
            yield return wait;
            if (!_running) yield break;
            try { Tick(1f); } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
        }
    }

    private static void Tick(float dt)
    {
        if (IsMainMenu()) return;
        var mgr = GetManager();
        if (mgr == null) return;
        try
        {
            var _ = mgr.gameObject; // liveness
        }
        catch { return; }

        CaptureBase(mgr);
        _elapsed += Math.Max(0f, dt);
        float tierLen = Math.Max(1f, _config.TierSeconds);
        int tier = Math.Min(_config.MaxTier, (int)(_elapsed / tierLen));
        if (tier == _appliedTier) return;
        _appliedTier = tier;
        ApplyTier(mgr, tier);
    }

    private static void CaptureBase(global::Il2Cpp.MaliciousAttackManager mgr)
    {
        if (_baseInterval < 0f)
        {
            try { _baseInterval = mgr.attackInterval; } catch { _baseInterval = 0f; }
        }
        if (_baseRate < 0f)
        {
            try { _baseRate = mgr.spawnRate; } catch { _baseRate = 0f; }
        }
        if (_baseSpeed < 0f)
        {
            try { _baseSpeed = mgr.moveSpeed; } catch { _baseSpeed = 0f; }
        }
    }

    private static void ApplyTier(global::Il2Cpp.MaliciousAttackManager mgr, int tier)
    {
        var stats = ComputeTierStats(_baseInterval, _baseRate, _baseSpeed, tier, _config);
        try
        {
            if (stats.AttackInterval > 0f) mgr.attackInterval = stats.AttackInterval;
        }
        catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
        try
        {
            if (stats.SpawnRate > 0f) mgr.spawnRate = stats.SpawnRate;
        }
        catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
        try
        {
            if (stats.MoveSpeed > 0f) mgr.moveSpeed = stats.MoveSpeed;
        }
        catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
        try
        {
            MelonLogger.Msg($"[gregCore][Net] Angriffs-Eskalation: Stufe {tier} " +
                $"(Intervall {stats.AttackInterval:F0}s, Rate x{(stats.SpawnRate / Math.Max(0.001f, _baseRate)):F2}).");
        }
        catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
    }

    // ── Reine Stufenmathe (ohne Spiel testbar) ───────────────────────────────

    public static TierStats ComputeTierStats(float baseInterval, float baseRate, float baseSpeed,
        int tier, EscalationConfig config)
    {
        var stats = new TierStats();
        var cfg = config ?? new EscalationConfig();
        int t = Math.Max(0, tier);
        float factor = Math.Max(0.01f, Math.Min(1f, cfg.IntervalFactor));
        try { stats.AttackInterval = Math.Max(cfg.MinIntervalSeconds, baseInterval * (float)Math.Pow(factor, t)); }
        catch { stats.AttackInterval = baseInterval; }
        try { stats.SpawnRate = Math.Max(0f, baseRate * (1f + cfg.RateGrowthPerTier * t)); }
        catch { stats.SpawnRate = baseRate; }
        try { stats.MoveSpeed = Math.Max(0f, baseSpeed * (1f + cfg.SpeedGrowthPerTier * t)); }
        catch { stats.MoveSpeed = baseSpeed; }
        return stats;
    }

    private static global::Il2Cpp.MaliciousAttackManager GetManager()
    {
        try { return global::Il2Cpp.MaliciousAttackManager.instance; }
        catch { return null; }
    }

    private static bool IsMainMenu()
    {
        try
        {
            var scene = SceneManager.GetActiveScene();
            return string.Equals(scene.name, "MainMenu", StringComparison.OrdinalIgnoreCase);
        }
        catch { return false; }
    }
}
