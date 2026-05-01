using UnityEngine;

namespace gregCore.Infrastructure.Performance;

internal sealed class GregFrameRateLimiter : IDisposable
{
    private readonly IGregLogger _logger;
    private PerformanceProfile _profile;
    private bool _isDisposed;
    private bool _wasFocused = true;

    internal GregFrameRateLimiter(IGregLogger logger, PerformanceProfile profile)
    {
        _logger = logger.ForContext(nameof(GregFrameRateLimiter));
        _profile = profile;
        Apply(profile);
    }

    internal void Apply(PerformanceProfile profile)
    {
        _profile = profile;
        global::UnityEngine.QualitySettings.vSyncCount = profile.EnableVSync ? 1 : 0;
        
        if (!profile.EnableVSync)
            global::UnityEngine.Application.targetFrameRate = profile.TargetFps;
        else
            global::UnityEngine.Application.targetFrameRate = -1;

        global::UnityEngine.QualitySettings.SetQualityLevel(profile.QualityLevel, false);
        global::UnityEngine.QualitySettings.shadowDistance = profile.ShadowDistance;
        global::UnityEngine.QualitySettings.globalTextureMipmapLimit = profile.TextureResolution;
        
        _logger.Info($"[FPS] VSync:{profile.EnableVSync} Target:{profile.TargetFps} Quality:{profile.QualityLevel}");
    }

    internal void OnUpdate()
    {
        if (_isDisposed) return;
        
        var isFocused = global::UnityEngine.Application.isFocused;
        if (isFocused == _wasFocused) return;
        
        _wasFocused = isFocused;
        if (!isFocused)
        {
            global::UnityEngine.Application.targetFrameRate = _profile.UnfocusedFps;
            global::UnityEngine.QualitySettings.vSyncCount = 0;
            _logger.Debug($"[FPS] Unfocused -> {_profile.UnfocusedFps} FPS");
        }
        else
        {
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
    }
}
