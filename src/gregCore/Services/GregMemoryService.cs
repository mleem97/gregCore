using System;
using System.Runtime;
using MelonLoader;
using UnityEngine;

namespace gregCore.Services;

public sealed class GregMemoryService
{
    public static GregMemoryService Instance { get; private set; } = null!;

    private float _gcTimer = 0f;
    private long _lastHeap = 0L;
    private const long GcTriggerBytes = 256 * 1024 * 1024;

    public void Initialize()
    {
        Instance = this;

        try
        {
            var latencyMode = GCLatencyMode.SustainedLowLatency;
            GCSettings.LatencyMode = latencyMode;
            MelonLogger.Msg($"[Memory] GC LatencyMode: {latencyMode}");
        }
        catch (Exception ex)
        {
            MelonLogger.Warning($"[Memory] GC LatencyMode config failed: {ex.Message}");
        }

        _lastHeap = GC.GetTotalMemory(false);
        MelonLogger.Msg($"[Memory] Initial heap: {_lastHeap / 1024 / 1024} MB");
    }

    public void Tick(bool isMenuOrIdle)
    {
        _gcTimer += Time.unscaledDeltaTime;

        long currentHeap = GC.GetTotalMemory(false);
        long growth = currentHeap - _lastHeap;

        bool heapPressure = growth > GcTriggerBytes;

        if (heapPressure && isMenuOrIdle)
        {
            MelonLogger.Msg($"[Memory] GC triggered: heap grew by {growth / 1024 / 1024} MB");
            GC.Collect(2, GCCollectionMode.Optimized, blocking: false);
            _lastHeap = GC.GetTotalMemory(false);
            _gcTimer = 0f;
        }

        if (_gcTimer > 300f)
        {
            GC.Collect(0, GCCollectionMode.Optimized, blocking: false);
            _gcTimer = 0f;
        }
    }

    public MemorySnapshot GetSnapshot() => new()
    {
        ManagedHeapMb = (float)GC.GetTotalMemory(false) / 1024f / 1024f,
        AllocatedMb = (float)GC.GetTotalAllocatedBytes(true) / 1024f / 1024f,
        SystemRamMb = SystemInfo.systemMemorySize,
        GpuVramMb = SystemInfo.graphicsMemorySize,
        GcLatencyMode = GCSettings.LatencyMode.ToString()
    };

    public struct MemorySnapshot
    {
        public float ManagedHeapMb { get; set; }
        public float AllocatedMb { get; set; }
        public int SystemRamMb { get; set; }
        public int GpuVramMb { get; set; }
        public string GcLatencyMode { get; set; }
    }
}