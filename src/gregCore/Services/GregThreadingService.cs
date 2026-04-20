using System;
using System.Threading;
using MelonLoader;
using UnityEngine;

namespace gregCore.Services;

public sealed class GregThreadingService
{
    public static GregThreadingService Instance { get; private set; } = null!;

    public void Initialize(int physicalCores)
    {
        Instance = this;

        ThreadPool.GetMinThreads(out int minW, out int minIO);
        ThreadPool.GetMaxThreads(out int maxW, out int maxIO);

        MelonLogger.Msg($"[Threading] Before: min=({minW},{minIO}) max=({maxW},{maxIO})");
        MelonLogger.Msg($"[Threading] Physical cores: {physicalCores}, Logical: {SystemInfo.processorCount}");

        int targetWorkers = Math.Max(physicalCores - 2, 4);
        int targetCompletion = Math.Max(physicalCores / 2, 4);

        ThreadPool.SetMinThreads(targetWorkers, targetCompletion);
        ThreadPool.SetMaxThreads(
            Math.Max(targetWorkers * 2, maxW),
            Math.Max(targetCompletion * 2, maxIO)
        );

        ThreadPool.GetMinThreads(out int newMinW, out int newMinIO);
        ThreadPool.GetMaxThreads(out int newMaxW, out int newMaxIO);

        MelonLogger.Msg($"[Threading] After: min=({newMinW},{newMinIO}) max=({newMaxW},{newMaxIO})");

        TryConfigureUnityJobs(targetWorkers);
    }

    static void TryConfigureUnityJobs(int workerCount)
    {
        try
        {
            MelonLogger.Msg("[Threading] JobsUtility config skipped (API may be stripped in IL2CPP)");
        }
        catch (Exception ex)
        {
            MelonLogger.Warning($"[Threading] Job System config failed: {ex.Message}");
        }
    }
}