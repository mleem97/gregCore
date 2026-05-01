namespace gregCore.Core.Models;

public sealed record AutomationProgress
{
    public string CurrentTask { get; init; } = string.Empty;
    public int Current { get; init; }
    public int Total { get; init; }
    public double PercentDone => Total == 0 ? 0 : (Current / (double)Total) * 100;
}
