using System.Security.Cryptography;
using System.Diagnostics;

namespace gregCore.Core.Diagnostics;

public sealed record GameFingerprint
{
    public string GameVersion { get; init; } = "UNKNOWN";
    public string AssemblyCSharpSha256 { get; init; } = string.Empty;
    public string GameAssemblySha256 { get; init; } = string.Empty;
    public string MetadataSha256 { get; init; } = string.Empty;
    public string UnityVersion { get; init; } = "UNKNOWN";
    public string MelonLoaderVersion { get; init; } = "UNKNOWN";
    public string Il2CppInteropVersion { get; init; } = "UNKNOWN";

    public string CombinedSha256 => ComputeCombinedSha256();

    public static GameFingerprint Capture(string gameRoot)
    {
        var assembly = Path.Combine(gameRoot, "MelonLoader", "Il2CppAssemblies", "Assembly-CSharp.dll");
        var gameAssembly = Path.Combine(gameRoot, "GameAssembly.dll");
        var metadata = Path.Combine(gameRoot, "Data", "Metadata", "global-metadata.dat");
        if (!File.Exists(metadata) && Directory.Exists(gameRoot))
            metadata = Directory.GetFiles(gameRoot, "global-metadata.dat", SearchOption.AllDirectories).OrderBy(x => x, StringComparer.Ordinal).FirstOrDefault() ?? metadata;
        return new GameFingerprint
        {
            AssemblyCSharpSha256 = HashIfPresent(assembly),
            GameAssemblySha256 = HashIfPresent(gameAssembly),
            MetadataSha256 = HashIfPresent(metadata),
            UnityVersion = ReadUnityVersion(gameRoot),
            GameVersion = ReadGameVersion(gameRoot),
            MelonLoaderVersion = ReadAssemblyVersion(gameRoot, "MelonLoader.dll"),
            Il2CppInteropVersion = ReadAssemblyVersion(gameRoot, "Il2CppInterop.Runtime.dll")
        };
    }

    private string ComputeCombinedSha256() {
        using var sha = SHA256.Create();
        var value = string.Join("\n", GameVersion, AssemblyCSharpSha256, GameAssemblySha256, MetadataSha256, UnityVersion, MelonLoaderVersion, Il2CppInteropVersion);
        return Convert.ToHexString(sha.ComputeHash(System.Text.Encoding.UTF8.GetBytes(value))).ToLowerInvariant();
    }

    private static string ReadUnityVersion(string root) {
        var path = Path.Combine(root, "UnityPlayer.dll");
        return File.Exists(path) ? FileVersionInfo.GetVersionInfo(path).ProductVersion ?? "UNKNOWN" : "UNKNOWN";
    }

    private static string ReadGameVersion(string root) {
        var candidates = new[] { Path.Combine(root, "version.txt"), Path.Combine(root, "VERSION") };
        foreach (var path in candidates) if (File.Exists(path)) return File.ReadAllText(path).Trim();
        return "UNKNOWN";
    }

    private static string ReadAssemblyVersion(string root, string fileName) {
        var path = Directory.Exists(root) ? Directory.GetFiles(root, fileName, SearchOption.AllDirectories).OrderBy(x => x, StringComparer.Ordinal).FirstOrDefault() : null;
        try { return path is null ? "UNKNOWN" : System.Reflection.AssemblyName.GetAssemblyName(path).Version?.ToString() ?? "UNKNOWN"; }
        catch { return "UNKNOWN"; }
    }

    private static string HashIfPresent(string path)
    {
        if (!File.Exists(path)) return string.Empty;
        using var stream = File.OpenRead(path);
        using var sha = SHA256.Create();
        return Convert.ToHexString(sha.ComputeHash(stream)).ToLowerInvariant();
    }
}
