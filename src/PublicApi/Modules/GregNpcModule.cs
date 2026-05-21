using UnityEngine;
using System.Reflection;

using System;

namespace gregCore.PublicApi.Modules;

public interface INpcSubsystem
{
    void UpdateState(string npcId, string state);
}

public sealed class GregNpcModule
{
    private readonly GregApiContext _ctx;
    internal readonly INpcSubsystem _internal;

    internal GregNpcModule(GregApiContext ctx, INpcSubsystem subsystem = null)
    {
        _ctx = ctx;
        _internal = subsystem;
    }

    public void UpdateNpcState(string npcId, string state)
    {
        try {
            if (_internal != null) {
                _internal.UpdateState(npcId, state);
            }
        }
        catch (Exception ex) {
            _ctx.Logger.Error($"NPC update failed: {ex.Message}");
        }
    }

    public int GetFreeTechnicianCount() {
        try {
            var tm = global::Il2Cpp.TechnicianManager.instance;
            if (tm == null || tm.technicians == null) return 0;
            int count = 0;
            foreach (var t in tm.technicians) {
                if (t != null && !t.isBusy) count++;
            }
            return count;
        } catch { return 0; }
    }
    public int GetTotalTechnicianCount() {
        try {
            var tm = global::Il2Cpp.TechnicianManager.instance;
            return tm != null && tm.technicians != null ? tm.technicians.Count : 0;
        } catch { return 0; }
    }
    
    public bool DispatchRepairServer(global::Il2Cpp.Server? server) {
        try {
            var tm = global::Il2Cpp.TechnicianManager.instance;
            if (tm == null || server == null) return false;
            // Use reflection if direct call fails
            tm.GetType().GetMethod("DispatchRepairServer")?.Invoke(tm, new object[] { server });
            return true;
        } catch { return false; }
    }

    public bool DispatchRepairSwitch(global::Il2Cpp.NetworkSwitch? sw) {
        try {
            var tm = global::Il2Cpp.TechnicianManager.instance;
            if (tm == null || sw == null) return false;
            tm.GetType().GetMethod("DispatchRepairSwitch")?.Invoke(tm, new object[] { sw });
            return true;
        } catch { return false; }
    }
}
