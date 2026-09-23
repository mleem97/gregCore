using MelonLoader;

namespace gregCore.Infrastructure.Logging;

/// <summary>
/// Erweitertes Mod-Entwickler-Logging, zur Laufzeit umschaltbar (Umgebungsschalter).
///
/// Verhalten:
///  * Default folgt dem Build-Flavor: DEBUG-Build (Dev) -&gt; AN, Release-Build -&gt; AUS.
///    So liefert der deployte Release ruhige Logs, die Dev-Variante volle Diagnose.
///  * Über den Settings-Hub (Switch "Verbose Console Output") lässt sich das erweiterte
///    Logging zur Laufzeit OHNE Rebuild umschalten — auch auf einem Release-Build, wenn
///    man gezielt nach Mod-Problemen sucht (Mod-Entwicklung am deployten Spielstand).
///  * BEWUSST KEIN [Conditional("DEBUG")]: Der Switch muss in jedem Build wirken. Die
///    Kosten pro Aufruf sind ein einzelner bool-Check; der teure String-Aufbau erfolgt
///    erst nach dem Gate. Aufrufstellen sind bewusst selten (Bootstrap, SaveGuard).
/// </summary>
public static class DevLog
{
    // Default folgt dem Flavor; wird zur Laufzeit über den Settings-Hub geändert.
    private static bool _verbose =
#if DEBUG
        true;
#else
        false;
#endif

    /// <summary>Master-Schalter für das erweiterte Dev-Logging (Laufzeit).</summary>
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
