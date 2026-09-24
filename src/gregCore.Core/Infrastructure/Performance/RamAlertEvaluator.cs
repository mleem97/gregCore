/// <file-summary>
/// Schicht:      Infrastructure (Performance)
/// Zweck:        Reine, laufzeit-freie Entscheidungslogik fuer RAM-Warnungen:
///               Metrik-Auswahl (Proton-tauglich) + Edge-Trigger mit Hysterese,
///               damit Warnungen + GC-Stuerme nur bei echten Uebergaengen
///               feuern statt alle 5 Sekunden. Zu 100 % per Unit-Test abgedeckt.
/// </file-summary>

namespace gregCore.Infrastructure.Performance;

public enum RamAlertLevel
{
    None = 0,
    Warning = 1,
    Critical = 2,
}

public static class RamAlertEvaluator
{
    // Hysterese-Band: Re-Arm erst deutlich unter der Schwelle, damit der
    // Alert bei schwankendem Verbrauch nicht flattert.
    public static int RearmHysteresisMb { get; } = 256;

    // Unter Proton/Wine enthaelt WorkingSet das komplette Wine-Mapping und
    // liegt permanent ueber jeder sinnvollen Schwelle. Private Bytes messen
    // den tatsaechlich gehaltenen Speicher und sind dort die ehrliche Metrik.
    public static int SelectRamMetricMb(int workingSetMb, int privateMb)
        => privateMb > 0 ? privateMb : workingSetMb;

    public static RamAlertLevel Classify(int ramMb, int warnMb, int critMb)
    {
        if (ramMb >= critMb) return RamAlertLevel.Critical;
        if (ramMb >= warnMb) return RamAlertLevel.Warning;
        return RamAlertLevel.None;
    }

    public static bool IsRearmed(int ramMb, RamAlertLevel active, int warnMb, int critMb)
    {
        int rearmBelow = (active == RamAlertLevel.Critical ? critMb : warnMb) - RearmHysteresisMb;
        return ramMb < rearmBelow;
    }
}

// Edge-Trigger mit Hysterese: Feuert pro Uebergang genau einmal. Solange der
// Zustand anhaelt, kommt nichts mehr - kein Log-Spam, keine GC-Stuerme.
public sealed class RamAlertState
{
    private RamAlertLevel _active = RamAlertLevel.None;

    // Gibt den zu publizierenden Level zurueck, oder null wenn nichts zu tun.
    public RamAlertLevel? Evaluate(int ramMb, int warnMb, int critMb)
    {
        var current = RamAlertEvaluator.Classify(ramMb, warnMb, critMb);
        if (current == RamAlertLevel.None)
        {
            // Im Hysterese-Band: scharf bleiben (sonst wuerde Flattern um die
            // Schwelle bei jedem Anstieg neu feuern), aber still.
            if (_active != RamAlertLevel.None
                && !RamAlertEvaluator.IsRearmed(ramMb, _active, warnMb, critMb))
                return null;
            _active = RamAlertLevel.None;
            return null;
        }
        if (current > _active)
        {
            _active = current;
            return current;
        }
        if (current < _active && RamAlertEvaluator.IsRearmed(ramMb, _active, warnMb, critMb))
        {
            _active = current;
            return current;
        }
        return null;
    }

    public RamAlertLevel Active => _active;
}
