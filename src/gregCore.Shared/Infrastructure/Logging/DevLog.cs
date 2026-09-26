using MelonLoader;

namespace gregCore.Infrastructure.Logging;

/// <summary>
/// Extended mod-developer logging, switchable at runtime (environment switch).
///
/// Behavior:
///  * Default follows the build flavor: DEBUG build (dev) -&gt; ON, release build -&gt; OFF.
///    The deployed release therefore produces quiet logs, the dev variant full diagnostics.
///  * Via the settings hub (switch "Verbose Console Output") the extended
///    logging can be toggled at runtime WITHOUT a rebuild — even on a release build when
///    specifically hunting mod issues (mod development against a deployed save).
///  * DELIBERATELY NO [Conditional("DEBUG")]: the switch must work in every build. The
///    cost per call is a single bool check; the expensive string building happens
///    only after the gate. Call sites are deliberately rare (bootstrap, SaveGuard).
/// </summary>
public static class DevLog
{
    // Default follows the flavor; changed at runtime via the settings hub.
    private static bool _verbose =
#if DEBUG
        true;
#else
        false;
#endif

    /// <summary>Master switch for extended dev logging (runtime).</summary>
    public static bool Verbose
    {
        get => _verbose;
        set => _verbose = value;
    }

    public static void Msg(string message)
    {
        if (!_verbose) return;
        MelonLogger.Msg("[Dev] " + message);
    }

    public static void Warning(string message)
    {
        if (!_verbose) return;
        MelonLogger.Warning("[Dev] " + message);
    }

    public static void Error(string message)
    {
        if (!_verbose) return;
        MelonLogger.Error("[Dev] " + message);
    }
}
