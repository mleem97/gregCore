using System.Text.Json;

namespace gregCore.Core.Diagnostics;

public static class GregDoctor
{
    public static GregDoctorReport Create(string gameRoot, string manifestPath, string logPath,
        IEnumerable<string>? loadedMods = null, IEnumerable<string>? activeHosts = null)
    {
        var fingerprint = GameFingerprint.Capture(gameRoot);
        var manifestVersion = "UNKNOWN";
        try
        {
            using var document = JsonDocument.Parse(File.ReadAllText(manifestPath));
            if (document.RootElement.TryGetProperty("manifestVersion", out var version))
                manifestVersion = version.ToString();
        }
        catch { }

        var manifestFingerprint = "";
        var manifestUnity = "UNKNOWN";
        var manifestMelon = "UNKNOWN";
        var manifestInterop = "UNKNOWN";
        try
        {
            using var document = JsonDocument.Parse(File.ReadAllText(manifestPath));
            var root = document.RootElement;
            if (root.TryGetProperty("assemblyFingerprint", out var fp)) manifestFingerprint = fp.GetString() ?? fp.ToString();
            if (root.TryGetProperty("unityVersion", out var unity)) manifestUnity = unity.GetString() ?? unity.ToString();
            if (root.TryGetProperty("melonLoaderVersion", out var melon)) manifestMelon = melon.GetString() ?? melon.ToString();
            if (root.TryGetProperty("il2CppInteropVersion", out var interop)) manifestInterop = interop.GetString() ?? interop.ToString();
        }
        catch { }
        var fingerprintMatch = manifestFingerprint is "" or "UNKNOWN" ? "unknown" :
            string.Equals(manifestFingerprint, fingerprint.CombinedSha256, StringComparison.OrdinalIgnoreCase) ? "match" : "mismatch";
        var knownBuild = fingerprintMatch == "match";
        return new GregDoctorReport
        {
            Status = knownBuild ? "SUPPORTED_GAME_BUILD" : "UNSUPPORTED_GAME_BUILD",
            SafeMode = !knownBuild,
            ErrorCode = knownBuild ? "" : "UNSUPPORTED_GAME_BUILD",
            GregCoreVersion = typeof(GregDoctor).Assembly.GetName().Version?.ToString() ?? "UNKNOWN",
            ManifestVersion = manifestVersion,
            FingerprintMatch = fingerprintMatch,
            UnityVersion = fingerprint.UnityVersion == "UNKNOWN" ? manifestUnity : fingerprint.UnityVersion,
            MelonLoaderVersion = fingerprint.MelonLoaderVersion == "UNKNOWN" ? manifestMelon : fingerprint.MelonLoaderVersion,
            Il2CppInteropVersion = fingerprint.Il2CppInteropVersion == "UNKNOWN" ? manifestInterop : fingerprint.Il2CppInteropVersion,
            Fingerprint = fingerprint,
            LoadedMods = loadedMods?.OrderBy(x => x, StringComparer.Ordinal).ToArray() ?? Array.Empty<string>(),
            ActiveLanguageHosts = activeHosts?.OrderBy(x => x, StringComparer.Ordinal).ToArray() ?? Array.Empty<string>(),
            DisabledComponents = knownBuild ? Array.Empty<string>() : new[] { "risk-bearing game hooks" },
            Recommendations = knownBuild
                ? new[] { "Keep the committed manifest and runtime versions aligned." }
                : new[] { "Install the supported game build or add this fingerprint to a reviewed manifest.", "Keep risky hooks disabled until compatibility is reviewed." },
            LogPath = logPath
        };
    }

    public static void Write(string path, GregDoctorReport report)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(path) ?? ".");
        File.WriteAllText(path, JsonSerializer.Serialize(report, new JsonSerializerOptions { WriteIndented = true }));
    }
}
