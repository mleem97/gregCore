/// <file-summary>
/// Layer:       Infrastructure (Performance)
/// Purpose:     Pure, runtime-free decision logic for RAM warnings:
///               metric selection (Proton-safe) + edge trigger with hysteresis,
///               so warnings + GC storms only fire on real transitions
///               instead of every 5 seconds. 100% covered by unit tests.
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
    // Hysteresis band: re-arm only well below the threshold, so the
    // alert does not flutter when consumption fluctuates.
    public static int RearmHysteresisMb { get; } = 256;

    // Under Proton/Wine the working set contains the entire Wine mapping and
    // stays permanently above any meaningful threshold. Private bytes measure
    // the actually held memory and are the honest metric there.
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

// Edge trigger with hysteresis: fires exactly once per transition. While the
// state persists, nothing more arrives - no log spam, no GC storms.
public sealed class RamAlertState
{
    private RamAlertLevel _active = RamAlertLevel.None;

    // Returns the level to publish, or null if there is nothing to do.
    public RamAlertLevel? Evaluate(int ramMb, int warnMb, int critMb)
    {
        var current = RamAlertEvaluator.Classify(ramMb, warnMb, critMb);
        if (current == RamAlertLevel.None)
        {
            // Inside the hysteresis band: stay armed (otherwise flutter around
            // the threshold would re-fire on every rise), but stay silent.
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
