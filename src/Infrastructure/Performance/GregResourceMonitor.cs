using System.Diagnostics;

namespace gregCore.Infrastructure.Performance;

internal sealed class GregResourceMonitor : IDisposable
{
    private readonly IGregLogger _logger;
    private readonly IGregEventBus _bus;
    private readonly PerformanceProfile _profile;
    private System.Timers.Timer? _timer;
    private ResourceSnapshot _lastSnapshot = new ResourceSnapshot();
    private ResourceSnapshot _lastUnityStats = new ResourceSnapshot();
    private bool _isDisposed;
    private static readonly Process _currentProcess = Process.GetCurrentProcess();

    internal GregResourceMonitor(IGregLogger logger, IGregEventBus bus, PerformanceProfile profile)
    {
        _logger = logger.ForContext(nameof(GregResourceMonitor));
        _bus = bus;
        _profile = profile;
    }

    internal void Start(int intervalMs = 5000)
    {
        _timer = new System.Timers.Timer(intervalMs);
        _timer.Elapsed += (_, _) => MeasureAndReport();
        _timer.AutoReset = true;
        _timer.Start();
        _logger.Info($"[Monitor] Gestartet ({intervalMs}ms)");
    }

    // UNITY MAIN THREAD REQUIRED
    internal void CacheUnityMemoryStats()
    {
        _lastUnityStats = _lastUnityStats with
        {
            UnityAllocatedMb = (int)(global::UnityEngine.Profiling.Profiler.GetTotalAllocatedMemoryLong() / 1024 / 1024),
            UnityReservedMb = (int)(global::UnityEngine.Profiling.Profiler.GetTotalReservedMemoryLong() / 1024 / 1024),
            UnityUnusedMb = (int)(global::UnityEngine.Profiling.Profiler.GetTotalUnusedReservedMemoryLong() / 1024 / 1024),
        };
    }

    private void MeasureAndReport()
    {
        if (_isDisposed) return;
        try {
            _currentProcess.Refresh();
            var snapshot = new ResourceSnapshot {
                TimestampUtc = DateTime.UtcNow,
                RamUsedMb = (int)(_currentProcess.WorkingSet64 / 1024 / 1024),
                PrivateMemoryMb = (int)(_currentProcess.PrivateMemorySize64 / 1024 / 1024),
                GcTotalMemoryMb = (int)(GC.GetTotalMemory(false) / 1024 / 1024),
                
                UnityAllocatedMb = _lastUnityStats.UnityAllocatedMb,
                UnityReservedMb = _lastUnityStats.UnityReservedMb,
                UnityUnusedMb = _lastUnityStats.UnityUnusedMb,

                GcGen0Collections = GC.CollectionCount(0),
                GcGen1Collections = GC.CollectionCount(1),
                GcGen2Collections = GC.CollectionCount(2),
                ThreadCount = _currentProcess.Threads.Count
            };
            _lastSnapshot = snapshot;
            
            _bus.Publish("greg.performance.ResourceSnapshot", new EventPayload { 
                OccurredAtUtc = snapshot.TimestampUtc, 
                Data = new Dictionary<string, object> { ["ramMb"] = snapshot.RamUsedMb, ["unityMb"] = snapshot.UnityAllocatedMb } 
            });
            
            CheckThresholds(snapshot);
        } catch (Exception ex) { _logger.Warning($"[Monitor] Messfehler: {ex.Message}"); }
    }

    private void CheckThresholds(ResourceSnapshot s)
    {
        if (s.RamUsedMb >= _profile.RamCriticalMb)
            _bus.Publish("greg.performance.RamCritical", new EventPayload { OccurredAtUtc = DateTime.UtcNow, Data = new Dictionary<string, object> { ["ramMb"] = s.RamUsedMb } });
        else if (s.RamUsedMb >= _profile.RamWarningMb)
            _bus.Publish("greg.performance.RamWarning", new EventPayload { OccurredAtUtc = DateTime.UtcNow, Data = new Dictionary<string, object> { ["ramMb"] = s.RamUsedMb } });
    }

    internal ResourceSnapshot GetLatest() => _lastSnapshot;

    public void Dispose()
    {
        if (_isDisposed) return;
        _isDisposed = true;
        _timer?.Stop();
        _timer?.Dispose();
    }
}
