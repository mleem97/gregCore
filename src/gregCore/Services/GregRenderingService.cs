using System;
using MelonLoader;
using UnityEngine;
using UnityEngine.Rendering;
using greg.Diagnostic;

namespace gregCore.Services;

public sealed class GregRenderingService
{
    public static GregRenderingService Instance { get; private set; } = null!;

    public void Initialize(GregPerfConfig cfg)
    {
        Instance = this;
        if (!cfg.RenderOptEnabled) return;

        ApplyQualitySettings(cfg);
        ApplyPostProcessing(cfg);
        MelonLogger.Msg("[Rendering] Optimizations applied.");
    }

    static void ApplyQualitySettings(GregPerfConfig cfg)
    {
        try
        {
            if (cfg.ReduceShadows)
            {
                QualitySettings.shadows = ShadowQuality.HardOnly;
                QualitySettings.shadowDistance = cfg.ShadowDistanceM;
                QualitySettings.shadowCascades = cfg.ShadowCascades;
                QualitySettings.shadowResolution = ShadowResolution.Medium;
                Log($"Shadows: {cfg.ShadowDistanceM}m, {cfg.ShadowCascades} cascades, HardOnly");
            }

            if (cfg.AggressiveLod)
            {
                QualitySettings.lodBias = cfg.LodBias;
                QualitySettings.maximumLODLevel = cfg.MaxLodLevel;
                Log($"LOD: bias={cfg.LodBias} maxLevel={cfg.MaxLodLevel}");
            }

            if (cfg.LimitPixelLights)
            {
                QualitySettings.pixelLightCount = cfg.MaxPixelLights;
                Log($"Pixel lights: {cfg.MaxPixelLights}");
            }

            if (cfg.ReduceTextureQuality)
            {
                QualitySettings.masterTextureLimit = cfg.TextureMipMapLimit;
                Log($"Texture mip limit: {cfg.TextureMipMapLimit}");
            }

            if (cfg.DisableSoftParticles)
            {
                QualitySettings.softParticles = false;
                Log("Soft particles: OFF");
            }

            QualitySettings.skinWeights = SkinWeights.TwoBones;
            Log("Skin weights: 2 bones");

            QualitySettings.asyncUploadTimeSlice = 4;
            QualitySettings.asyncUploadBufferSize = 64;
            QualitySettings.asyncUploadPersistentBuffer = true;
            Log("AsyncUpload: 4ms/frame, 64MB buffer");
        }
        catch (Exception ex)
        {
            MelonLogger.Warning($"[Rendering] QualitySettings failed: {ex.Message}");
        }
    }

    static void ApplyPostProcessing(GregPerfConfig cfg)
    {
        if (!cfg.DisableHeavyPostProcessing) return;

        try
        {
            var volumes = Resources.FindObjectsOfTypeAll<Volume>();
            int disabled = 0;

            for (int i = 0; i < volumes.Length; i++)
            {
                var vol = volumes[i];
                if (vol == null || vol.profile == null) continue;

                var components = vol.profile.components;
                for (int j = 0; j < components.Count; j++)
                {
                    var comp = components[j];
                    if (comp == null) continue;

                    string typeName = comp.GetType().Name;
                    bool shouldDisable = typeName switch
                    {
                        "MotionBlur" => cfg.DisableMotionBlur,
                        "Bloom" => cfg.DisableBloom,
                        "DepthOfField" => cfg.DisableDoF,
                        "AmbientOcclusion" => cfg.DisableAO,
                        "ScreenSpaceReflections" => cfg.DisableSSR,
                        "FilmGrain" => true,
                        "ChromaticAberration" => true,
                        _ => false
                    };

                    if (shouldDisable && comp.active)
                    {
                        comp.active = false;
                        disabled++;
                    }
                }
            }

            Log($"Post processing: {disabled} effects disabled");
        }
        catch (Exception ex)
        {
            MelonLogger.Warning($"[Rendering] PostFX optimization failed: {ex.Message}");
        }
    }

    static void Log(string msg) => MelonLogger.Msg($"[Rendering] {msg}");
}