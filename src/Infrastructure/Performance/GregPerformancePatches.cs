using System;
using System.Collections.Generic;
using HarmonyLib;
using Il2Cpp;
using UnityEngine;
using UnityEngine.AI;

namespace gregCore.Infrastructure.Performance;

/// <summary>
/// Zentrale Performance-Optimierungen via Harmony-Patches.
/// Integriert in GregCore — kein separates Mod nötig.
/// </summary>
internal static class GregPerformancePatches
{
    private static bool _initialized = false;
    private static HarmonyLib.Harmony? _harmony;

    // Throttle-Konfiguration (wird vom Governor gesetzt)
    internal static bool CanvasThrottleEnabled { get; set; } = true;
    internal static float CanvasUpdateInterval { get; set; } = 0.1f;
    internal static bool IndicatorThrottleEnabled { get; set; } = true;
    internal static float IndicatorUpdateInterval { get; set; } = 0.1f;
    internal static bool PulsatingThrottleEnabled { get; set; } = true;
    internal static float PulsatingUpdateInterval { get; set; } = 0.05f;
    internal static bool NpcThrottleEnabled { get; set; } = true;
    internal static float NpcThrottleDistance { get; set; } = 15f;
    internal static float NpcThrottleInterval { get; set; } = 0.2f;
    internal static bool AsyncRouteEvalEnabled { get; set; } = false;

    internal static void Initialize()
    {
        if (_initialized) return;
        _initialized = true;

        _harmony = new HarmonyLib.Harmony("gregCore.performance");
        _harmony.PatchAll(typeof(GregPerformancePatches).Assembly);
    }

    // ── WorldCanvasCuller throttle ───────────────────────────────────────────
    [HarmonyPatch(typeof(WorldCanvasCuller), "Update")]
    internal static class WorldCanvasCullerPatch
    {
        internal static readonly Dictionary<IntPtr, float> _nextRun = new();
        static bool Prefix(WorldCanvasCuller __instance)
        {
            if (!CanvasThrottleEnabled) return true;
            float now = Time.time;
            var ptr = __instance.Pointer;
            if (_nextRun.TryGetValue(ptr, out float next) && now < next) return false;
            _nextRun[ptr] = now + CanvasUpdateInterval;
            return true;
        }
    }
    [HarmonyPatch(typeof(WorldCanvasCuller), "OnDestroy")]
    internal static class WorldCanvasCullerCleanupPatch
    {
        static void Postfix(WorldCanvasCuller __instance) => WorldCanvasCullerPatch._nextRun.Remove(__instance.Pointer);
    }

    // ── PositionIndicator throttle ───────────────────────────────────────────
    [HarmonyPatch(typeof(PositionIndicator), "Update")]
    internal static class PositionIndicatorPatch
    {
        internal static readonly Dictionary<IntPtr, float> _nextRun = new();
        static bool Prefix(PositionIndicator __instance)
        {
            if (!IndicatorThrottleEnabled) return true;
            float now = Time.time;
            var ptr = __instance.Pointer;
            if (_nextRun.TryGetValue(ptr, out float next) && now < next) return false;
            _nextRun[ptr] = now + IndicatorUpdateInterval;
            return true;
        }
    }
    [HarmonyPatch(typeof(PositionIndicator), "OnDestroy")]
    internal static class PositionIndicatorCleanupPatch
    {
        static void Postfix(PositionIndicator __instance) => PositionIndicatorPatch._nextRun.Remove(__instance.Pointer);
    }

    // ── PulsatingImageColor throttle ─────────────────────────────────────────
    [HarmonyPatch(typeof(PulsatingImageColor), "Update")]
    internal static class PulsatingImageColorPatch
    {
        internal static readonly Dictionary<IntPtr, float> _nextRun = new();
        static bool Prefix(PulsatingImageColor __instance)
        {
            if (!PulsatingThrottleEnabled) return true;
            float now = Time.time;
            var ptr = __instance.Pointer;
            if (_nextRun.TryGetValue(ptr, out float next) && now < next) return false;
            _nextRun[ptr] = now + PulsatingUpdateInterval;
            return true;
        }
    }
    [HarmonyPatch(typeof(PulsatingImageColor), "OnDestroy")]
    internal static class PulsatingImageColorCleanupPatch
    {
        static void Postfix(PulsatingImageColor __instance) => PulsatingImageColorPatch._nextRun.Remove(__instance.Pointer);
    }

    // ── PulsatingText throttle ───────────────────────────────────────────────
    [HarmonyPatch(typeof(PulsatingText), "Update")]
    internal static class PulsatingTextPatch
    {
        internal static readonly Dictionary<IntPtr, float> _nextRun = new();
        static bool Prefix(PulsatingText __instance)
        {
            if (!PulsatingThrottleEnabled) return true;
            float now = Time.time;
            var ptr = __instance.Pointer;
            if (_nextRun.TryGetValue(ptr, out float next) && now < next) return false;
            _nextRun[ptr] = now + PulsatingUpdateInterval;
            return true;
        }
    }
    [HarmonyPatch(typeof(PulsatingText), "OnDestroy")]
    internal static class PulsatingTextCleanupPatch
    {
        static void Postfix(PulsatingText __instance) => PulsatingTextPatch._nextRun.Remove(__instance.Pointer);
    }

    // ── Technician Animator culling ──────────────────────────────────────────
    [HarmonyPatch(typeof(TechnicianManager), "AddTechnician")]
    internal static class TechnicianAnimatorCullingPatch
    {
        static void Postfix(Technician technician)
        {
            if (!NpcThrottleEnabled || technician == null) return;
            try
            {
                var anim = technician.GetComponent<Animator>();
                if (anim != null) anim.cullingMode = AnimatorCullingMode.CullCompletely;
                var agent = technician.GetComponent<NavMeshAgent>();
                if (agent != null) agent.obstacleAvoidanceType = ObstacleAvoidanceType.LowQualityObstacleAvoidance;
            }
            catch (Exception ex) { UnityEngine.Debug.LogWarning($"[gregCore] AnimatorCulling: {ex.Message}"); }
        }
    }

    // ── Technician FixedUpdate throttle ──────────────────────────────────────
    [HarmonyPatch(typeof(Technician), "FixedUpdate")]
    internal static class TechnicianFixedUpdatePatch
    {
        internal static readonly Dictionary<IntPtr, float> _nextRun = new();
        static bool Prefix(Technician __instance)
        {
            if (!NpcThrottleEnabled) return true;
            float now = Time.time;
            var ptr = __instance.Pointer;
            if (_nextRun.TryGetValue(ptr, out float next) && now < next) return false;

            var cam = Camera.main;
            if (cam != null)
            {
                float dist = Vector3.Distance(cam.transform.position, __instance.transform.position);
                if (dist > NpcThrottleDistance)
                {
                    _nextRun[ptr] = now + NpcThrottleInterval;
                    return true;
                }
            }
            _nextRun.Remove(ptr);
            return true;
        }
    }
    [HarmonyPatch(typeof(Technician), "OnDestroy")]
    internal static class TechnicianFixedUpdateCleanupPatch
    {
        static void Postfix(Technician __instance) => TechnicianFixedUpdatePatch._nextRun.Remove(__instance.Pointer);
    }

    // ── Technician LateUpdate throttle ───────────────────────────────────────
    [HarmonyPatch(typeof(Technician), "LateUpdate")]
    internal static class TechnicianLateUpdatePatch
    {
        internal static readonly Dictionary<IntPtr, float> _nextRun = new();
        static bool Prefix(Technician __instance)
        {
            if (!NpcThrottleEnabled) return true;
            float now = Time.time;
            var ptr = __instance.Pointer;
            if (_nextRun.TryGetValue(ptr, out float next) && now < next) return false;

            var cam = Camera.main;
            if (cam != null)
            {
                float dist = Vector3.Distance(cam.transform.position, __instance.transform.position);
                if (dist > NpcThrottleDistance)
                {
                    _nextRun[ptr] = now + NpcThrottleInterval;
                    return true;
                }
            }
            _nextRun.Remove(ptr);
            return true;
        }
    }
    [HarmonyPatch(typeof(Technician), "OnDestroy")]
    internal static class TechnicianLateUpdateCleanupPatch
    {
        static void Postfix(Technician __instance) => TechnicianLateUpdatePatch._nextRun.Remove(__instance.Pointer);
    }

    // ── FootSteps throttle ───────────────────────────────────────────────────
    [HarmonyPatch(typeof(FootSteps), "Update")]
    internal static class FootStepsPatch
    {
        internal static readonly Dictionary<IntPtr, float> _nextRun = new();
        static bool Prefix(FootSteps __instance)
        {
            if (!NpcThrottleEnabled) return true;
            float now = Time.time;
            var ptr = __instance.Pointer;
            if (_nextRun.TryGetValue(ptr, out float next) && now < next) return false;

            var cam = Camera.main;
            if (cam != null)
            {
                float dist = Vector3.Distance(cam.transform.position, __instance.transform.position);
                if (dist > NpcThrottleDistance * 2f)
                {
                    _nextRun[ptr] = now + 1f;
                    return false;
                }
                if (dist > NpcThrottleDistance)
                {
                    _nextRun[ptr] = now + NpcThrottleInterval;
                    return true;
                }
            }
            _nextRun.Remove(ptr);
            return true;
        }
    }
    [HarmonyPatch(typeof(FootSteps), "OnDestroy")]
    internal static class FootStepsCleanupPatch
    {
        static void Postfix(FootSteps __instance) => FootStepsPatch._nextRun.Remove(__instance.Pointer);
    }
}
