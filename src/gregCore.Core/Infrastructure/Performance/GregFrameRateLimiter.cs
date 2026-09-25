using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

namespace gregCore.Infrastructure.Performance;

internal sealed class GregFrameRateLimiter : IDisposable
{
    private readonly IGregLogger _logger;
    private PerformanceProfile _profile;
    private bool _isDisposed;
    private bool _wasFocused = true;
    private int _lastAppliedProfileHash = 0;
    private bool _originalCaptured;
    private int _originalQualityLevel = -1;
    private float _originalShadowDistance;
    private int _originalMipmapLimit;
    private float _originalLodBias = -1f;
    private bool _originalStreamingMipmaps;
    private float _originalStreamingBudget;
    private float _originalFarClip = -1f;

    internal GregFrameRateLimiter(IGregLogger logger, PerformanceProfile profile)
    {
        _logger = logger.ForContext(nameof(GregFrameRateLimiter));
        CaptureOriginalGraphics();
        _profile = profile;
        Apply(profile);
    }

    internal void Apply(PerformanceProfile profile)
    {
        _profile = profile;
        var currentHash = profile.GetHashCode();
        if (currentHash == _lastAppliedProfileHash) return;
        _lastAppliedProfileHash = currentHash;

        // Frame cap — always safe, touches no meshes/LODs/materials.
        global::UnityEngine.QualitySettings.vSyncCount = profile.EnableVSync ? 1 : 0;
        if (!profile.EnableVSync)
            global::UnityEngine.Application.targetFrameRate = profile.TargetFps;
        else
            global::UnityEngine.Application.targetFrameRate = -1;

        if (!profile.ApplyGraphicsSettings)
        {
            _logger.Info($"[Perf] Applied FPS-only profile: VSync={profile.EnableVSync} TargetFPS={profile.TargetFps} (graphics untouched, game defaults kept).");
            return;
        }

        // Graphics — only with explicit opt-in (UseProfile(Balanced/LowEnd/...)).
        // May hide server LODs (RackMount.ApplyMaterialToLODs / AddCullOnlyLODGroup).
        CaptureOriginalGraphics();
        if (_originalFarClip <= 0f)
        {
            try
            {
                var earlyCam = global::Il2Cpp.MainGameManager.instance?.playerCamera;
                if (earlyCam != null) _originalFarClip = earlyCam.farClipPlane;
            }
            catch { /* best-effort */ }
        }
        global::UnityEngine.QualitySettings.SetQualityLevel(profile.QualityLevel, false);
        global::UnityEngine.QualitySettings.shadowDistance = profile.ShadowDistance;
        global::UnityEngine.QualitySettings.globalTextureMipmapLimit = profile.TextureResolution;
        global::UnityEngine.QualitySettings.lodBias = profile.LodBias;

        // Camera
        var cam = global::Il2Cpp.MainGameManager.instance?.playerCamera;
        if (cam != null)
            cam.farClipPlane = profile.CameraFarClip;

        // Streaming Mipmaps
        global::UnityEngine.QualitySettings.streamingMipmapsActive = profile.EnableStreamingMipmaps;
        if (profile.EnableStreamingMipmaps)
            global::UnityEngine.QualitySettings.streamingMipmapsMemoryBudget = profile.StreamingMipmapBudgetMb;

        // HDRP Volume Overrides
        ApplyHDRPVolumeOverrides(profile);

        // HDRP Pipeline Asset Overrides
        ApplyHDRPPipelineOverrides(profile);

        _logger.Info($"[Perf] Applied profile: VSync={profile.EnableVSync} TargetFPS={profile.TargetFps} " +
                     $"ShadowDist={profile.ShadowDistance} FarClip={profile.CameraFarClip} LOD={profile.LodBias} " +
                     $"Mipmaps={profile.TextureResolution}");
    }

    private void CaptureOriginalGraphics()
    {
        if (_originalCaptured) return;
        try
        {
            _originalQualityLevel = global::UnityEngine.QualitySettings.GetQualityLevel();
            _originalShadowDistance = global::UnityEngine.QualitySettings.shadowDistance;
            _originalMipmapLimit = global::UnityEngine.QualitySettings.globalTextureMipmapLimit;
            _originalLodBias = global::UnityEngine.QualitySettings.lodBias;
            _originalStreamingMipmaps = global::UnityEngine.QualitySettings.streamingMipmapsActive;
            _originalStreamingBudget = global::UnityEngine.QualitySettings.streamingMipmapsMemoryBudget;
            var cam = global::Il2Cpp.MainGameManager.instance?.playerCamera;
            if (cam != null) _originalFarClip = cam.farClipPlane;
            _originalCaptured = true;
        }
        catch { /* best-effort: game defaults remain unknown */ }
    }

    internal void RestoreGameGraphics()
    {
        if (!_originalCaptured) return;
        try
        {
            if (_originalQualityLevel >= 0)
                global::UnityEngine.QualitySettings.SetQualityLevel(_originalQualityLevel, false);
            global::UnityEngine.QualitySettings.shadowDistance = _originalShadowDistance;
            global::UnityEngine.QualitySettings.globalTextureMipmapLimit = _originalMipmapLimit;
            if (_originalLodBias > 0f)
                global::UnityEngine.QualitySettings.lodBias = _originalLodBias;
            global::UnityEngine.QualitySettings.streamingMipmapsActive = _originalStreamingMipmaps;
            if (_originalStreamingMipmaps)
                global::UnityEngine.QualitySettings.streamingMipmapsMemoryBudget = _originalStreamingBudget;
            if (_originalFarClip > 0f)
            {
                var cam = global::Il2Cpp.MainGameManager.instance?.playerCamera;
                if (cam != null) cam.farClipPlane = _originalFarClip;
            }
            _logger.Info("[Perf] Game graphics restored (Quality/LOD/Mipmaps/FarClip).");
        }
        catch (Exception ex)
        {
            _logger.Warning($"[Perf] RestoreGameGraphics failed: {ex.Message}");
        }
    }

    private void ApplyHDRPVolumeOverrides(PerformanceProfile profile)
    {
        try
        {
            var sg = global::Il2Cpp.SettingsSingleton.instance?.settingsGraphics;
            if (sg == null) return;
            var volProfile = sg.volumeProfile;
            if (volProfile == null) return;

            int disabled = 0;
            if (profile.DisableSSAO && volProfile.TryGet<ScreenSpaceAmbientOcclusion>(out var ssao))
            { ssao.active = false; disabled++; }
            if (profile.DisableContactShadows && volProfile.TryGet<ContactShadows>(out var cs))
            { cs.active = false; disabled++; }
            if (profile.DisableGlobalIllumination && volProfile.TryGet<GlobalIllumination>(out var gi))
            { gi.active = false; disabled++; }
            if (profile.DisableSSR && volProfile.TryGet<ScreenSpaceReflection>(out var ssr))
            { ssr.active = false; disabled++; }
            if (profile.DisableVolumetricFog && volProfile.TryGet<Fog>(out var fog))
            { fog.enableVolumetricFog.overrideState = true; fog.enableVolumetricFog.value = false; disabled++; }

            if (disabled > 0)
                _logger.Debug($"[Perf] Disabled {disabled} HDRP volume effect(s).");
        }
        catch (Exception ex)
        {
            _logger.Warning($"[Perf] HDRP volume overrides failed: {ex.Message}");
        }
    }

    private void ApplyHDRPPipelineOverrides(PerformanceProfile profile)
    {
        try
        {
            var hdrpAsset = GraphicsSettings.currentRenderPipeline as HDRenderPipelineAsset;
            if (hdrpAsset == null) return;

            var settings = hdrpAsset.currentPlatformRenderPipelineSettings;
            settings.hdShadowInitParams.maxShadowRequests = profile.MaxShadowRequests;
            settings.supportDecals = !profile.DisableDecals;
            settings.supportSSAO = !profile.DisableSSAO;
            settings.supportSSR = !profile.DisableSSR;

            hdrpAsset.currentPlatformRenderPipelineSettings = settings;
            _logger.Debug($"[Perf] HDRPAsset: MaxShadowRequests={profile.MaxShadowRequests} Decals={!profile.DisableDecals}");
        }
        catch (Exception ex)
        {
            _logger.Warning($"[Perf] HDRP pipeline overrides failed: {ex.Message}");
        }
    }

    internal void OnUpdate()
    {
        if (_isDisposed) return;
        if (!_profile.ThrottleWhenUnfocused) return;
        
        var isFocused = global::UnityEngine.Application.isFocused;
        if (isFocused == _wasFocused) return;
        
        _wasFocused = isFocused;
        if (!isFocused)
        {
            global::UnityEngine.Application.targetFrameRate = _profile.UnfocusedFps;
            global::UnityEngine.QualitySettings.vSyncCount = 0;
            _lastAppliedProfileHash = 0;
            _logger.Debug($"[FPS] Unfocused -> {_profile.UnfocusedFps} FPS");
        }
        else
        {
            _lastAppliedProfileHash = 0;
            Apply(_profile);
            _logger.Debug($"[FPS] Focused -> {_profile.TargetFps} FPS");
        }
    }

    public void Dispose()
    {
        if (_isDisposed) return;
        _isDisposed = true;
        global::UnityEngine.Application.targetFrameRate = -1;
        global::UnityEngine.QualitySettings.vSyncCount = 1;
        RestoreGameGraphics();
    }
}
