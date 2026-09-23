namespace gregCore.Core;

/// <summary>
/// Build-Flavor + Version, abgeleitet aus dem Compile-Profil:
/// Dev = Debug-Build mit erweitertem Logging (DevLog, #if DEBUG),
/// Release = Release-Build mit normalem Logging.
/// BuildInfo.Version wird von <c>MelonInfo</c> genutzt, damit MelonLoader
/// sichtbar anzeigt, welcher Flavor installiert ist.
/// </summary>
public static class BuildInfo
{
#if DEBUG
    public const string Flavor = "DEV";
    public const string Version = "1.2.3-dev.0";
#else
    public const string Flavor = "RELEASE";
    public const string Version = "1.2.3";
#endif

    public const string Name = "gregCore";
    public const string Author = "TeamGreg";
    public const string ShortLabel = $"{Name} v{Version} [{Flavor}]";
}