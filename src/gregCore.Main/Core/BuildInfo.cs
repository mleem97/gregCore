namespace gregCore.Core;

/// <summary>
/// Build flavor + version, derived from the compile profile:
/// Dev = debug build with extended logging (DevLog, #if DEBUG),
/// Release = release build with normal logging.
/// BuildInfo.Version is used by <c>MelonInfo</c> so that MelonLoader
/// visibly shows which flavor is installed.
/// </summary>
public static class BuildInfo
{
#if DEBUG
    public const string Flavor = "DEV";
    public const string Version = "1.2.3-dev.0";
#else
#pragma warning disable // Justification: Flavor/Version must stay const (assembly-level MelonInfo attribute requires compile-time constants; see GregCoreMod.cs).
    public const string Flavor = "RELEASE";
    public const string Version = "1.2.3";
#pragma warning restore
#endif

    public const string Name = "gregCore";
    public const string Author = "TeamGreg";
    public const string ShortLabel = $"{Name} v{Version} [{Flavor}]";
}