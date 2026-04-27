/// <file-summary>
/// Schicht:      Infrastructure
/// Zweck:        Frame-Budget-Tracking für Lua-Mods.
/// Maintainer:   Misst CPU-Zeit pro Lua-Mod pro Frame.
///               Warnt bei Überschreitung des Frame-Budgets.
/// </file-summary>

using System;
using System.Collections.Generic;
using System.Diagnostics;
using MelonLoader;

namespace gregCore.Infrastructure.Scripting.Lua.Dev;

public sealed class LuaProfiler
{
    private readonly Dictionary<string, ModProfile> _profiles = new();
    private readonly float _frameBudgetMs;
    private readonly Stopwatch _globalStopwatch = new();
    private float _lastTotalMs;
    private int _frameCount;
    private float _warningCooldown;

    /// <summary>
    /// Erstellt einen Profiler mit dem angegebenen Frame-Budget.
    /// </summary>
    /// <param name="frameBudgetMs">Max. Millisekunden pro Frame für alle Lua-Mods zusammen (Default: 2.0ms)</param>
    public LuaProfiler(float frameBudgetMs = 2.0f)
    {
        _frameBudgetMs = frameBudgetMs;
    }

    /// <summary>
    /// Startet eine Messung für einen Mod.
    /// </summary>
    public ProfileScope BeginScope(string modId)
    {
        if (!_profiles.TryGetValue(modId, out var profile))
        {
            profile = new ModProfile { ModId = modId };
            _profiles[modId] = profile;
        }

        return new ProfileScope(profile.CurrentFrameMsRef);
    }

    /// <summary>
    /// Wird am Ende jedes Frames aufgerufen.
    /// </summary>
    public void EndFrame()
    {
        _frameCount++;

        float totalMs = 0f;
        foreach (var profile in _profiles.Values)
        {
            totalMs += profile.CurrentFrameMs;
        }

        _lastTotalMs = totalMs;

        // Warn if over budget
        if (totalMs > _frameBudgetMs && _warningCooldown <= 0f)
        {
            MelonLogger.Warning($"[LuaProfiler] Frame budget exceeded: {totalMs:F2}ms / {_frameBudgetMs:F2}ms");
            foreach (var profile in _profiles.Values)
            {
                if (profile.CurrentFrameMs > 0.1f)
                {
                    MelonLogger.Warning($"  {profile.ModId}: {profile.CurrentFrameMs:F2}ms");
                }
            }
            _warningCooldown = 5f; // Don't spam warnings
        }

        if (_warningCooldown > 0f)
            _warningCooldown -= UnityEngine.Time.unscaledDeltaTime;

        // Reset frame counters
        foreach (var profile in _profiles.Values)
        {
            profile.AccumulatedMs += profile.CurrentFrameMs;
            profile.FrameCount++;
            profile.CurrentFrameMs = 0f;
        }
    }

    /// <summary>
    /// Gibt einen Profiling-Report als String zurück (für REPL/Console).
    /// </summary>
    public string GetReport()
    {
        var sb = new System.Text.StringBuilder();
        sb.AppendLine($"[LuaProfiler] Frame Budget: {_lastTotalMs:F2}ms / {_frameBudgetMs:F2}ms");
        sb.AppendLine($"  Total frames tracked: {_frameCount}");

        foreach (var profile in _profiles.Values)
        {
            float avgMs = profile.FrameCount > 0 ? profile.AccumulatedMs / profile.FrameCount : 0f;
            sb.AppendLine($"  {profile.ModId}: avg={avgMs:F3}ms, total={profile.AccumulatedMs:F1}ms over {profile.FrameCount} frames");
        }

        return sb.ToString();
    }

    public float LastFrameTotalMs => _lastTotalMs;
    public float BudgetMs => _frameBudgetMs;
    public bool IsOverBudget => _lastTotalMs > _frameBudgetMs;

    // ─── Inner Types ─────────────────────────────────────────────────

    private class ModProfile
    {
        public string ModId = "";
        /// <summary>Single-element array to allow struct-based writing via indirection.</summary>
        public readonly float[] CurrentFrameMsRef = new float[1];
        public float CurrentFrameMs { get => CurrentFrameMsRef[0]; set => CurrentFrameMsRef[0] = value; }
        public float AccumulatedMs;
        public int FrameCount;
    }

    public readonly struct ProfileScope : IDisposable
    {
        private readonly float[] _targetMs;
        private readonly long _startTicks;

        internal ProfileScope(float[] targetMs)
        {
            _targetMs = targetMs;
            _startTicks = Stopwatch.GetTimestamp();
        }

        public void Dispose()
        {
            long elapsed = Stopwatch.GetTimestamp() - _startTicks;
            float ms = (elapsed * 1000f) / Stopwatch.Frequency;
            _targetMs[0] += ms;
        }
    }
}
