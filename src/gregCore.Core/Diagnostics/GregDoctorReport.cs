namespace gregCore.Core.Diagnostics;

public sealed record GregDoctorReport
{
    public string Status { get; init; } = "UNKNOWN";
    public bool SafeMode { get; init; }
    public string ErrorCode { get; init; } = string.Empty;
    public string GregCoreVersion { get; init; } = string.Empty;
    public string ManifestVersion { get; init; } = string.Empty;
    public string FingerprintMatch { get; init; } = "unknown";
    public string UnityVersion { get; init; } = "UNKNOWN";
    public string MelonLoaderVersion { get; init; } = "UNKNOWN";
    public string Il2CppInteropVersion { get; init; } = "UNKNOWN";
    public GameFingerprint Fingerprint { get; init; } = new();
    public IReadOnlyList<string> LoadedMods { get; init; } = Array.Empty<string>();
    public IReadOnlyList<string> ActiveLanguageHosts { get; init; } = Array.Empty<string>();
    public IReadOnlyList<string> InstalledHooks { get; init; } = Array.Empty<string>();
    public IReadOnlyList<string> FailedHooks { get; init; } = Array.Empty<string>();
    public IReadOnlyList<string> DisabledComponents { get; init; } = Array.Empty<string>();
    public IReadOnlyList<string> ModLoadErrors { get; init; } = Array.Empty<string>();
    public string SelfTestResult { get; init; } = "not-run";
    public IReadOnlyList<string> Recommendations { get; init; } = Array.Empty<string>();
    public string LogPath { get; init; } = string.Empty;
    public DateTime GeneratedAtUtc { get; init; } = DateTime.UtcNow;
}
