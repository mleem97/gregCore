namespace greg.Diagnostic;

public sealed class FrameLimiterConfig
{
    public bool Enabled { get; set; } = true;
    public FpsProfile Menu { get; set; } = new() { TargetFps = 30, VSync = 0 };
    public FpsProfile Gameplay { get; set; } = new() { TargetFps = 144, VSync = 0 };
    public AfkProfile Afk { get; set; } = new();
    public FpsProfile Minimized { get; set; } = new() { TargetFps = 5, VSync = 0 };
    public FpsProfile Background { get; set; } = new() { TargetFps = 20, VSync = 0 };
}

public class FpsProfile
{
    public int TargetFps { get; set; } = 60;
    public int VSync { get; set; } = 0;
}

public sealed class AfkProfile : FpsProfile
{
    public bool Enabled { get; set; } = true;
    public float AfkAfterSeconds { get; set; } = 60f;
    public AfkProfile() { TargetFps = 15; }
}