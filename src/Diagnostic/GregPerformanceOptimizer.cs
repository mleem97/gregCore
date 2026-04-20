using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using HarmonyLib;
using Il2Cpp;
using MelonLoader;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

namespace greg.Diagnostic;

// ── Main-thread dispatch queue ─────────────────────────────────────────────
internal static class MainThreadDispatch
{
    private static readonly ConcurrentQueue<Action> _queue = new();
    public static void Enqueue(Action action) => _queue.Enqueue(action);
    public static void Drain()
    {
        while (_queue.TryDequeue(out var action))
        {
            try { action(); }
            catch (Exception ex) { MelonLogger.Warning($"[gregCore.PerfFix] MainThreadDispatch error: {ex.Message}"); }
        }
    }
}

public static class GregPerformanceOptimizer
{
    // ── Preferences ───────────────────────────────────────────────────────
    public static MelonPreferences_Entry<bool>  CanvasThrottleEnabled;
    public static MelonPreferences_Entry<float> CanvasUpdateInterval;

    public static MelonPreferences_Entry<bool>  IndicatorThrottleEnabled;
    public static MelonPreferences_Entry<float> IndicatorUpdateInterval;

    public static MelonPreferences_Entry<bool>  ThrottlePulsating;
    public static MelonPreferences_Entry<float> PulsatingUpdateInterval;

    public static MelonPreferences_Entry<float> RouteEvalCooldown;
    public static MelonPreferences_Entry<bool>  AsyncRouteEval;
    public static MelonPreferences_Entry<float> AutoSaveMinutes;

    // NPCs
    public static MelonPreferences_Entry<bool>  NpcEnabled;
    public static MelonPreferences_Entry<float> NpcThrottleDistance;
    public static MelonPreferences_Entry<float> NpcThrottleInterval;

    // Memory
    public static MelonPreferences_Entry<bool>  MemoryEnabled;
    public static MelonPreferences_Entry<int>   TextureMipmapLimit;
    public static MelonPreferences_Entry<bool>  StreamingMipmaps;
    public static MelonPreferences_Entry<float> StreamingMipmapsBudgetMB;
    public static MelonPreferences_Entry<float> PeriodicGCIntervalSeconds;

    // Graphics
    public static MelonPreferences_Entry<bool>  GraphicsEnabled;
    public static MelonPreferences_Entry<float> ShadowDistance;
    public static MelonPreferences_Entry<float> CameraFarClip;
    public static MelonPreferences_Entry<float> LodBias;
    public static MelonPreferences_Entry<bool>  DisableSSAO;
    public static MelonPreferences_Entry<bool>  DisableContactShadows;
    public static MelonPreferences_Entry<bool>  DisableGlobalIllumination;
    public static MelonPreferences_Entry<bool>  DisableSSR;
    public static MelonPreferences_Entry<bool>  DisableVolumetricFog;

    // ── OLD TWEAKS ────────────────────────────────────────────────────────
    public static MelonPreferences_Entry<int> TargetFPS;

    public static void ApplySettings()
    {
        Initialize();
    }

    public static void Initialize()
    {
        var cat = MelonPreferences.CreateCategory("PerfFix");

        TargetFPS = cat.CreateEntry("TargetFPS", 120, "TargetFPS", "Target framerate. 0 = uncap.");

        CanvasThrottleEnabled = cat.CreateEntry("CanvasThrottle", true, "CanvasThrottle",
            "Throttle WorldCanvasCuller.Update() to CanvasUpdateInterval Hz instead of every frame.");

        CanvasUpdateInterval = cat.CreateEntry("CanvasUpdateInterval", 0.1f, "CanvasUpdateInterval",
            "Seconds between WorldCanvasCuller distance checks (0.1 = 10 Hz). Vanilla runs at full framerate.");

        IndicatorThrottleEnabled = cat.CreateEntry("IndicatorThrottle", true, "IndicatorThrottle",
            "Throttle PositionIndicator.Update() (warning/error triangles) — re-projects world→screen every frame.");

        IndicatorUpdateInterval = cat.CreateEntry("IndicatorUpdateInterval", 0.1f, "IndicatorUpdateInterval",
            "Seconds between PositionIndicator screen-position updates (0.1 = 10 Hz).");

        ThrottlePulsating = cat.CreateEntry("ThrottlePulsating", true, "ThrottlePulsating",
            "Throttle PulsatingImageColor and PulsatingText Update() calls.");

        PulsatingUpdateInterval = cat.CreateEntry("PulsatingUpdateInterval", 0.05f, "PulsatingUpdateInterval",
            "Seconds between pulsating effect updates (0.05 = 20 Hz).");

        RouteEvalCooldown = cat.CreateEntry("RouteEvalCooldown", 2.0f, "RouteEvalCooldown",
            "Minimum seconds between full ECS cable-route re-evaluations.");

        AsyncRouteEval = cat.CreateEntry("AsyncRouteEval", false, "AsyncRouteEval",
            "EXPERIMENTAL (DISABLED!): Run EvaluateAllRoutes on a background thread. CAUSES IL2CPP CRASHES.");

        AutoSaveMinutes = cat.CreateEntry("AutoSaveMinutes", 10.0f, "AutoSaveMinutes",
            "Minutes between auto-saves. Large saves cause frame hitches. Set to 0 to disable.");

        GraphicsEnabled = cat.CreateEntry("GraphicsEnabled", true, "GraphicsEnabled",
            "Master toggle for the graphics fixes below.");

        ShadowDistance = cat.CreateEntry("ShadowDistance", 20f, "ShadowDistance",
            "HDRP shadow cull distance in metres. Vanilla is ~150 m.");

        CameraFarClip = cat.CreateEntry("CameraFarClip", 80f, "CameraFarClip",
            "Player camera far clip plane in metres. Vanilla is ~1000 m.");

        LodBias = cat.CreateEntry("LodBias", 0.4f, "LodBias",
            "Unity LOD bias. Lower = switch to cheaper LOD meshes sooner.");

        DisableSSAO = cat.CreateEntry("DisableSSAO", true, "DisableSSAO",
            "Disable HDRP Screen-Space Ambient Occlusion.");

        DisableContactShadows = cat.CreateEntry("DisableContactShadows", true, "DisableContactShadows",
            "Disable HDRP Contact Shadows.");

        DisableGlobalIllumination = cat.CreateEntry("DisableGlobalIllumination", true, "DisableGlobalIllumination",
            "Disable HDRP Screen-Space Global Illumination.");

        DisableSSR = cat.CreateEntry("DisableSSR", true, "DisableSSR",
            "Disable HDRP Screen-Space Reflections on floors/racks.");

        DisableVolumetricFog = cat.CreateEntry("DisableVolumetricFog", false, "DisableVolumetricFog",
            "Disable HDRP Volumetric Fog.");

        NpcEnabled = cat.CreateEntry("NpcEnabled", true, "NpcEnabled",
            "Master toggle for NPC/Technician optimizations.");

        NpcThrottleDistance = cat.CreateEntry("NpcThrottleDistance", 15f, "NpcThrottleDistance",
            "Distance in metres beyond which Technician FixedUpdate and LateUpdate are throttled.");

        NpcThrottleInterval = cat.CreateEntry("NpcThrottleInterval", 0.2f, "NpcThrottleInterval",
            "Seconds between FixedUpdate/LateUpdate ticks for distant technicians.");

        MemoryEnabled = cat.CreateEntry("MemoryEnabled", true, "MemoryEnabled",
            "Master toggle for memory reduction settings below.");

        TextureMipmapLimit = cat.CreateEntry("TextureMipmapLimit", 1, "TextureMipmapLimit",
            "Global texture mipmap skip level. 0=full resolution, 1=half resolution, 2=quarter resolution.");

        StreamingMipmaps = cat.CreateEntry("StreamingMipmaps", true, "StreamingMipmaps",
            "Enable Unity mipmap streaming.");

        StreamingMipmapsBudgetMB = cat.CreateEntry("StreamingMipmapsBudgetMB", 512f, "StreamingMipmapsBudgetMB",
            "Memory budget for streamed mipmaps in megabytes.");

        PeriodicGCIntervalSeconds = cat.CreateEntry("PeriodicGCIntervalSeconds", 0f, "PeriodicGCIntervalSeconds",
            "Seconds between forced garbage collection passes. Set to 0 to disable. (WARNING: Enabled GC has been known to crash Il2CppInterop)");

        MelonLogger.Msg($"[gregCore.PerfFix] Loaded. " +
                           $"Canvas={CanvasUpdateInterval.Value}s  " +
                           $"RouteEval={RouteEvalCooldown.Value}s  " +
                           $"AutoSave={AutoSaveMinutes.Value}min  " +
                           $"FarClip={CameraFarClip.Value}m");

        ApplyMemorySettings();

        // Base Performance Tweaks
        int targetFPS = TargetFPS.Value > 0 ? TargetFPS.Value : Screen.currentResolution.refreshRate;
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = targetFPS > 0 ? targetFPS : 120;
    }

    private static float _nextGC = 0f;

    public static void OnUpdate()
    {
        MainThreadDispatch.Drain();

        float interval = PeriodicGCIntervalSeconds?.Value ?? 0f;
        if (interval > 0f && Time.realtimeSinceStartup >= _nextGC && _nextGC > 0f)
        {
            _nextGC = Time.realtimeSinceStartup + interval;
            MelonCoroutines.Start(RunPeriodicGC());
        }
    }

    private static System.Collections.IEnumerator RunPeriodicGC()
    {
        yield return null;
        // GC forcing removed - Causes Il2CppInterop/Unity Finalizer NullReferenceException
        MelonLogger.Msg($"[gregCore.PerfFix] Periodic GC skipped to prevent Il2CppInterop crashes.");
    }

    public static void OnSceneLoaded()
    {
        ApplySimulationFixes();
        MelonCoroutines.Start(ApplyGraphicsFixesNextFrame());
        if (PeriodicGCIntervalSeconds != null)
        {
            float interval = PeriodicGCIntervalSeconds.Value;
            _nextGC = interval > 0f ? Time.realtimeSinceStartup + interval : 0f;
        }
    }

    private static void ApplySimulationFixes()
    {
        try
        {
            var wis = WaypointInitializationSystem.Instance;
            if (wis != null)
            {
                wis.SetEvaluationCooldown(RouteEvalCooldown.Value);
                MelonLogger.Msg($"[gregCore.PerfFix] RouteEvalCooldown → {RouteEvalCooldown.Value}s");
            }
        }
        catch (Exception ex) { MelonLogger.Warning($"[gregCore.PerfFix] RouteEvalCooldown: {ex.Message}"); }

        try
        {
            var mgr = MainGameManager.instance;
            if (mgr != null)
            {
                if (AutoSaveMinutes.Value <= 0f)
                {
                    mgr.SetAutoSaveEnabled(false);
                    MelonLogger.Msg("[gregCore.PerfFix] AutoSave disabled.");
                }
                else
                {
                    mgr.SetAutoSaveEnabled(true);
                    mgr.SetAutoSaveInterval(AutoSaveMinutes.Value);
                    MelonLogger.Msg($"[gregCore.PerfFix] AutoSave interval → {AutoSaveMinutes.Value} min");
                }
            }
        }
        catch (Exception ex) { MelonLogger.Warning($"[gregCore.PerfFix] AutoSave: {ex.Message}"); }
    }

    private static System.Collections.IEnumerator ApplyGraphicsFixesNextFrame()
    {
        yield return null;
        ApplyGraphicsFixes();
    }

    private static void ApplyMemorySettings()
    {
        if (!MemoryEnabled.Value) return;
        try
        {
            QualitySettings.globalTextureMipmapLimit = TextureMipmapLimit.Value;
        }
        catch (Exception ex) { MelonLogger.Warning($"[gregCore.PerfFix] TextureMipmapLimit: {ex.Message}"); }

        try
        {
            QualitySettings.streamingMipmapsActive = StreamingMipmaps.Value;
            if (StreamingMipmaps.Value)
            {
                QualitySettings.streamingMipmapsMemoryBudget = StreamingMipmapsBudgetMB.Value;
            }
        }
        catch (Exception ex) { MelonLogger.Warning($"[gregCore.PerfFix] StreamingMipmaps: {ex.Message}"); }
    }

    public static void ApplyGraphicsFixes()
    {
        QualitySettings.lodBias = LodBias.Value;

        if (!GraphicsEnabled.Value) return;

        try
        {
            var cam = MainGameManager.instance?.playerCamera;
            if (cam != null)
            {
                cam.farClipPlane = CameraFarClip.Value;
            }
        }
        catch (Exception ex) { MelonLogger.Warning($"[gregCore.PerfFix] CameraFarClip: {ex.Message}"); }

        try
        {
            var sg = SettingsSingleton.instance?.settingsGraphics;
            if (sg != null)
            {
                sg.SetShadowDistance(ShadowDistance.Value);
            }
        }
        catch (Exception ex) { MelonLogger.Warning($"[gregCore.PerfFix] ShadowDistance: {ex.Message}"); }

        try
        {
            var sg = SettingsSingleton.instance?.settingsGraphics;
            if (sg == null) return;
            var profile = sg.volumeProfile;
            if (profile == null) return;

            int disabled = 0;
            if (DisableSSAO.Value && profile.TryGet<ScreenSpaceAmbientOcclusion>(out var ssao))
            { ssao.active = false; disabled++; }
            if (DisableContactShadows.Value && profile.TryGet<ContactShadows>(out var cs))
            { cs.active = false; disabled++; }
            if (DisableGlobalIllumination.Value && profile.TryGet<GlobalIllumination>(out var gi))
            { gi.active = false; disabled++; }
            if (DisableSSR.Value && profile.TryGet<ScreenSpaceReflection>(out var ssr))
            { ssr.active = false; disabled++; }
            if (DisableVolumetricFog.Value && profile.TryGet<Fog>(out var fog))
            { fog.enableVolumetricFog.overrideState = true; fog.enableVolumetricFog.value = false; disabled++; }
        }
        catch (Exception ex) { MelonLogger.Warning($"[gregCore.PerfFix] HDRP volume overrides: {ex.Message}"); }
    }
}

// ── WorldCanvasCuller throttle ─────────────────────────────────────────────
[HarmonyPatch(typeof(WorldCanvasCuller), "Update")]
internal static class WorldCanvasCullerPatch
{
    private static readonly Dictionary<IntPtr, float> _nextRun = new();
    static bool Prefix(WorldCanvasCuller __instance)
    {
        if (GregPerformanceOptimizer.CanvasThrottleEnabled == null || !GregPerformanceOptimizer.CanvasThrottleEnabled.Value) return true;
        float now = Time.time;
        var ptr = __instance.Pointer;
        if (_nextRun.TryGetValue(ptr, out float next) && now < next) return false;
        _nextRun[ptr] = now + GregPerformanceOptimizer.CanvasUpdateInterval.Value;
        return true;
    }
}

// ── PositionIndicator throttle ─────────────────────────────────────────────
[HarmonyPatch(typeof(PositionIndicator), "Update")]
internal static class PositionIndicatorPatch
{
    private static readonly Dictionary<IntPtr, float> _nextRun = new();
    static bool Prefix(PositionIndicator __instance)
    {
        if (GregPerformanceOptimizer.IndicatorThrottleEnabled == null || !GregPerformanceOptimizer.IndicatorThrottleEnabled.Value) return true;
        float now = Time.time;
        var ptr = __instance.Pointer;
        if (_nextRun.TryGetValue(ptr, out float next) && now < next) return false;
        _nextRun[ptr] = now + GregPerformanceOptimizer.IndicatorUpdateInterval.Value;
        return true;
    }
}

// ── PulsatingImageColor throttle ───────────────────────────────────────────
[HarmonyPatch(typeof(PulsatingImageColor), "Update")]
internal static class PulsatingImageColorPatch
{
    private static readonly Dictionary<IntPtr, float> _nextRun = new();
    static bool Prefix(PulsatingImageColor __instance)
    {
        if (GregPerformanceOptimizer.ThrottlePulsating == null || !GregPerformanceOptimizer.ThrottlePulsating.Value) return true;
        float now = Time.time;
        var ptr = __instance.Pointer;
        if (_nextRun.TryGetValue(ptr, out float next) && now < next) return false;
        _nextRun[ptr] = now + GregPerformanceOptimizer.PulsatingUpdateInterval.Value;
        return true;
    }
}

// ── PulsatingText throttle ─────────────────────────────────────────────────
[HarmonyPatch(typeof(PulsatingText), "Update")]
internal static class PulsatingTextPatch
{
    private static readonly Dictionary<IntPtr, float> _nextRun = new();
    static bool Prefix(PulsatingText __instance)
    {
        if (GregPerformanceOptimizer.ThrottlePulsating == null || !GregPerformanceOptimizer.ThrottlePulsating.Value) return true;
        float now = Time.time;
        var ptr = __instance.Pointer;
        if (_nextRun.TryGetValue(ptr, out float next) && now < next) return false;
        _nextRun[ptr] = now + GregPerformanceOptimizer.PulsatingUpdateInterval.Value;
        return true;
    }
}

// ── Async EvaluateAllRoutes ────────────────────────────────────────────────
[HarmonyPatch(typeof(WaypointInitializationSystem), "EvaluateAllRoutes")]
internal static class AsyncRouteEvalPatch
{
    [ThreadStatic]
    private static bool _allowPassthrough;

    private static volatile bool _evaluationInFlight;

    static bool Prefix(WaypointInitializationSystem __instance)
    {
        // ── KILLS IL2CPP GC BECAUSE UNMANAGED THREAD IS NOT ATTACHED ──
        // Disabled fully
        return true;

        /*
        if (GregPerformanceOptimizer.AsyncRouteEval == null || !GregPerformanceOptimizer.AsyncRouteEval.Value) return true;

        if (_allowPassthrough) return true;

        if (_evaluationInFlight) return false;

        _evaluationInFlight = true;
        var wis = __instance;

        Task.Run(() =>
        {
            _allowPassthrough = true;
            try
            {
                wis.EvaluateAllRoutes();
            }
            catch (Exception ex)
            {
                MelonLogger.Warning($"[gregCore.PerfFix] Async EvaluateAllRoutes threw: {ex.GetType().Name}: {ex.Message}. " +
                                    "Falling back to main-thread execution.");

                MainThreadDispatch.Enqueue(() =>
                {
                    _allowPassthrough = true;
                    try   { wis.EvaluateAllRoutes(); }
                    catch (Exception ex2) { MelonLogger.Warning($"[gregCore.PerfFix] Sync fallback also failed: {ex2.Message}"); }
                    finally { _allowPassthrough = false; }
                });
            }
            finally
            {
                _allowPassthrough = false;
                _evaluationInFlight = false;
            }
        });

        return false;
        */
    }
}

// ── Technician Animator culling ────────────────────────────────────────────
[HarmonyPatch(typeof(TechnicianManager), "AddTechnician")]
internal static class TechnicianAnimatorCullingPatch
{
    static void Postfix(Technician technician)
    {
        if (GregPerformanceOptimizer.NpcEnabled == null || !GregPerformanceOptimizer.NpcEnabled.Value || technician == null) return;
        try
        {
            var anim = technician.GetComponent<Animator>();
            if (anim != null)
                anim.cullingMode = AnimatorCullingMode.CullCompletely;

            var agent = technician.GetComponent<NavMeshAgent>();
            if (agent != null)
                agent.obstacleAvoidanceType = ObstacleAvoidanceType.LowQualityObstacleAvoidance;
        }
        catch (Exception ex)
        {
            MelonLogger.Warning($"[gregCore.PerfFix] TechnicianAnimatorCulling: {ex.Message}");
        }
    }
}

// Removed Technician FixedUpdate, LateUpdate, and Footstep update patches as methods are stripped and cause Harmony init failures
