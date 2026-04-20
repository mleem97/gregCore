using System;
using MelonLoader;
using UnityEngine;
using UnityEngine.Rendering;

namespace greg.Diagnostic;

public sealed class GregRenderOptimizer
{
    public static GregRenderOptimizer Instance { get; private set; } = null!;

    public void Initialize(RenderOptimizerConfig cfg)
    {
        Instance = this;
        if (cfg == null || !cfg.Enabled) return;

        Apply(cfg);
    }

    private void Apply(RenderOptimizerConfig cfg)
    {
        if (cfg == null) return;

        try
        {
            if (cfg.ReduceShadowDistance)
            {
                QualitySettings.shadowDistance = cfg.ShadowDistance;
                QualitySettings.shadowCascades = cfg.ShadowCascades;
                Log($"Shadows: distance={cfg.ShadowDistance} cascades={cfg.ShadowCascades}");
            }

            if (cfg.AdjustLodBias)
            {
                QualitySettings.lodBias = cfg.LodBias;
                Log($"LOD bias: {cfg.LodBias}");
            }

            if (cfg.LimitPixelLights)
            {
                QualitySettings.pixelLightCount = cfg.PixelLightCount;
                Log($"Pixel lights: {cfg.PixelLightCount}");
            }

            if (cfg.SetAnisotropicFiltering)
            {
                QualitySettings.anisotropicFiltering =
                    cfg.AnisotropicFiltering ? AnisotropicFiltering.Enable
                                         : AnisotropicFiltering.Disable;
                Log($"Anisotropic: {cfg.AnisotropicFiltering}");
            }

            if (cfg.SetAntiAliasing)
            {
                QualitySettings.antiAliasing = cfg.AntiAliasingLevel;
                Log($"AA: {cfg.AntiAliasingLevel}x MSAA");
            }

            if (cfg.DisableSoftParticles)
            {
                QualitySettings.softParticles = false;
                Log("Soft particles: disabled");
            }

            QualitySettings.vSyncCount = 0;

            Log("Render optimizations applied.");
        }
        catch (Exception ex)
        {
            MelonLogger.Warning($"[RenderOptimizer] Apply failed: {ex.Message}");
        }
    }

    private static void Log(string msg) =>
        MelonLogger.Msg($"[RenderOptimizer] {msg}");
}