using gregCore.Infrastructure.Automation;
using gregCore.PublicApi.Types;
using System.Collections;
using MelonLoader;

namespace gregCore.PublicApi.Modules;

public sealed class GregAutomationModule
{
    private readonly AutomationEngine _engine;
    private readonly IGregLogger _logger;
    private readonly List<GregScheduledTask> _tasks = new();

    internal GregAutomationModule(GregApiContext ctx)
    {
        _engine = new AutomationEngine(ctx);
        _logger = ctx.Logger.ForContext(nameof(GregAutomationModule));
        ctx.EventBus.Subscribe("greg.lifecycle.OnEndOfTheDay", _ => TickTasks());
    }

    public Task<AutomationResult> ProcessDeliveryZoneAsync(IProgress<AutomationProgress>? progress = null)
        => RunCoroutine(c => _engine.Delivery.ProcessAllCoroutine(progress, c));

    public Task<AutomationResult> LayCableAsync(string src, string tgt, CableType type = CableType.Cat6)
        => RunCoroutine(c => _engine.Cabling.LayCableCoroutine(src, tgt, type, c));

    public Task<AutomationResult> BuildRackAsync(RackBuildConfig config, IProgress<AutomationProgress>? progress = null)
        => RunCoroutine(c => _engine.RackBuild.BuildRackCoroutine(config, progress, c));

    public Task<AutomationResult> SetupNetworkSegmentAsync(NetworkSegmentConfig config, IProgress<AutomationProgress>? progress = null)
        => RunCoroutine(c => _engine.Network.SetupNetworkSegmentCoroutine(config, progress, c));

    public Task<AutomationResult> RepairBrokenDevicesAsync(IProgress<AutomationProgress>? progress = null)
        => RunCoroutine(c => _engine.Repair.RepairAllCoroutine(progress, c));

    public GregEventHandle EveryNDays(int days, Action action)
    {
        var task = new GregScheduledTask(days, action);
        _tasks.Add(task);
        return new GregEventHandle(() => _tasks.Remove(task));
    }

    public GregEventHandle EveryNDays(int days, Func<Task> asyncAction)
        => EveryNDays(days, () => MelonCoroutines.Start(RunAsync(asyncAction)));

    private static Task<AutomationResult> RunCoroutine(Func<Action<AutomationResult>, IEnumerator> factory)
    {
        var tcs = new TaskCompletionSource<AutomationResult>();
        MelonCoroutines.Start(factory(res => tcs.SetResult(res)));
        return tcs.Task;
    }

    private static IEnumerator RunAsync(Func<Task> action)
    {
        var t = action();
        while (!t.IsCompleted) yield return null;
    }

    private void TickTasks() { foreach (var t in _tasks.ToList()) t.Tick(); }
}

internal sealed class GregScheduledTask
{
    private readonly int _interval;
    private readonly Action _action;
    private int _count;
    public GregScheduledTask(int interval, Action action) { _interval = interval; _action = action; }
    public void Tick() { if (++_count >= _interval) { _count = 0; _action(); } }
}