using System.Text.Json.Serialization;

namespace greg.Diagnostic;

public sealed class GregPerfConfig
{
    public static GregPerfConfig Instance { get; set; } = new();

    public bool FrameCapEnabled { get; set; } = true;
    public int MenuFps { get; set; } = 30;
    public int GameplayFps { get; set; } = 144;
    public int BackgroundFps { get; set; } = 20;
    public int AfkFps { get; set; } = 15;
    public bool AfkEnabled { get; set; } = true;
    public float AfkSeconds { get; set; } = 60f;
    public int MaxAllowedFps { get; set; } = 240;

    [JsonIgnore]
    public int CurrentTarget { get; set; } = 30;

    public bool ThreadingEnabled { get; set; } = true;
    public int PhysicalCores { get; set; } = 0;

    public bool GcOptEnabled { get; set; } = true;
    public int GcTriggerMb { get; set; } = 256;
    public bool IncrementalGc { get; set; } = true;

    public bool RenderOptEnabled { get; set; } = true;
    public bool ReduceShadows { get; set; } = true;
    public float ShadowDistanceM { get; set; } = 50f;
    public int ShadowCascades { get; set; } = 2;
    public bool AggressiveLod { get; set; } = true;
    public float LodBias { get; set; } = 1.0f;
    public int MaxLodLevel { get; set; } = 0;
    public bool LimitPixelLights { get; set; } = true;
    public int MaxPixelLights { get; set; } = 2;
    public bool ReduceTextureQuality { get; set; } = false;
    public int TextureMipMapLimit { get; set; } = 0;
    public bool DisableSoftParticles { get; set; } = true;
    public bool DisableHeavyPostProcessing { get; set; } = true;
    public bool DisableMotionBlur { get; set; } = true;
    public bool DisableBloom { get; set; } = false;
    public bool DisableDoF { get; set; } = true;
    public bool DisableAO { get; set; } = false;
    public bool DisableSSR { get; set; } = true;
}