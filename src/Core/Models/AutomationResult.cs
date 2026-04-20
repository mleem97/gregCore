namespace gregCore.Core.Models;

public sealed record AutomationResult
{
    public bool IsSuccess { get; init; }
    public string? Error { get; init; }
    public string? Detail { get; init; }
    public int ItemsProcessed { get; init; }

    public static AutomationResult Success(int items = 1, string? detail = null)
        => new AutomationResult { IsSuccess = true, ItemsProcessed = items, Detail = detail };

    public static AutomationResult Failure(string error, string? detail = null)
        => new AutomationResult { IsSuccess = false, Error = error, Detail = detail };

    public static AutomationResult Partial(int done, int total, string? detail = null)
        => new AutomationResult { IsSuccess = done > 0, ItemsProcessed = done, Detail = detail ?? $"{done}/{total} verarbeitet" };
}