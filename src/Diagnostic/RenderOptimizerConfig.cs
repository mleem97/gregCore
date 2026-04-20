namespace greg.Diagnostic;

public sealed class RenderOptimizerConfig
{
    public bool Enabled { get; set; } = true;
    public bool ReduceShadowDistance { get; set; } = true;
    public float ShadowDistance { get; set; } = 50f;
    public int ShadowCascades { get; set; } = 2;
    public bool AdjustLodBias { get; set; } = true;
    public float LodBias { get; set; } = 1.0f;
    public bool LimitPixelLights { get; set; } = true;
    public int PixelLightCount { get; set; } = 2;
    public bool SetAnisotropicFiltering { get; set; } = true;
    public bool AnisotropicFiltering { get; set; } = false;
    public bool SetAntiAliasing { get; set; } = true;
    public int AntiAliasingLevel { get; set; } = 0;
    public bool DisableSoftParticles { get; set; } = true;
}