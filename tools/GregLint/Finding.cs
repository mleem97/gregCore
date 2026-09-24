// GregLint finding model + severities. Deterministic ordering via CompareTo.

namespace GregLint;

public enum Severity
{
    Info = 0,
    Warning = 1,
    Error = 2,
}

public sealed class Finding : IComparable<Finding>
{
    public string RuleId { get; init; } = "";
    public Severity Severity { get; init; }
    public string File { get; init; } = "";
    public string Subject { get; init; } = "";
    public string Message { get; init; } = "";
    public string FixHint { get; init; } = "";

    public int CompareTo(Finding? other)
    {
        if (other is null) return 1;
        int c = string.Compare(RuleId, other.RuleId, StringComparison.Ordinal);
        if (c != 0) return c;
        c = string.Compare(File, other.File, StringComparison.OrdinalIgnoreCase);
        if (c != 0) return c;
        c = string.Compare(Subject, other.Subject, StringComparison.Ordinal);
        if (c != 0) return c;
        return string.Compare(Message, other.Message, StringComparison.Ordinal);
    }

    public override string ToString() =>
        $"{Severity.ToString().ToLowerInvariant()} {RuleId} {File} {Subject}: {Message}".Trim();
}

public sealed class LintContext
{
    /// <summary>Root under analysis (mod folder or game Mods dir).</summary>
    public string Root { get; init; } = "";
    /// <summary>When true, rules may repair (with .bak backup) and report AppliedFixes.</summary>
    public bool ApplyFix { get; init; }
    public List<string> AppliedFixes { get; } = new();
    public List<string> Warnings { get; } = new();

    public void Fixed(string what)
    {
        AppliedFixes.Add(what);
    }
}
