using System;
using MelonLoader;
using UnityEngine;
using Il2Cpp;

namespace greg.Sdk.Services;

/// <summary>
/// Bridge service for interacting with Technicians, hiring, firing, and job dispatching.
/// </summary>
public static class GregTechnicianService
{
    private static TechnicianManager GetInstance()
    {
        var instance = UnityEngine.Object.FindObjectOfType<TechnicianManager>();
        if (instance == null)
            MelonLogger.Warning("[GregTechnicianService] TechnicianManager instance not found!");
        return instance;
    }

    public static void FireTechnician(int technicianId)
    {
        GetInstance()?.FireTechnician(technicianId);
    }

    public static void SendTechnician(NetworkSwitch netSwitch, Server server)
    {
        GetInstance()?.SendTechnician(netSwitch, server);
    }

    public static int GetQueuedJobCount()
    {
        return GetInstance()?.QueuedJobCount ?? 0;
    }

    public static Il2CppSystem.Collections.Generic.List<TechnicianManager.RepairJob> GetQueuedJobs()
    {
        return GetInstance()?.GetQueuedJobs();
    }

    public static Il2CppSystem.Collections.Generic.List<TechnicianManager.RepairJob> GetActiveJobs()
    {
        return GetInstance()?.GetActiveJobs();
    }
}

