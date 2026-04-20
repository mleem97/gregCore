using gregCore.Infrastructure.Performance;

namespace gregCore.PublicApi.Modules;

public sealed class GregPerformanceModule
{
    private readonly GregPerformanceGovernor _governor;
    private readonly IGregLogger _logger;
    private readonly IGregEventBus _bus;

    internal GregPerformanceModule(GregApiContext ctx, GregPerformanceGovernor governor)
    {
        _governor = governor;
        _logger = ctx.Logger.ForContext(nameof(GregPerformanceModule));
        _bus = ctx.EventBus;
    }

    public void Configure(PerformanceProfile profile) => _governor.Configure(profile);
    public void UseProfile(PerformanceProfile profile) => _governor.Configure(profile);
    public void SetTargetFPS(int fps) => Configure(_governor.GetStats().Profile with { TargetFps = fps });
    public void ThrottleWhenUnfocused(bool enabled, int unfocusedFps = 15) => Configure(_governor.GetStats().Profile with { UnfocusedFps = enabled ? unfocusedFps : -1 });
    public void SetMaxConcurrentOperations(int max) => Configure(_governor.GetStats().Profile with { MaxConcurrentOps = max });

    public Task<T> QueueOperation<T>(string name, Func<Task<T>> operation, OperationPriority priority = OperationPriority.Normal)
        => _governor.QueueOperationAsync(name, operation, priority);

    public PerformanceStats GetStats() => _governor.GetStats();
    public ResourceSnapshot GetResourceSnapshot() => _governor.GetStats().Resources;

    public event Action<ResourceSnapshot>? OnResourceUpdate
    {
        add => _bus.Subscribe("greg.performance.ResourceSnapshot", p => value?.Invoke(_governor.GetStats().Resources));
        remove => _bus.Unsubscribe("greg.performance.ResourceSnapshot", p => value?.Invoke(_governor.GetStats().Resources));
    }

    public PerformanceProfile Balanced => PerformanceProfile.Balanced;
    public PerformanceProfile HighPerformance => PerformanceProfile.HighPerformance;
    public PerformanceProfile LowEnd => PerformanceProfile.LowEnd;
}