/// <file-summary>
/// Schicht:      Infrastructure
/// Zweck:        Lua-API für Tutorials und Objectives.
/// Maintainer:   greg.objectives.show(), stop(), skip(), active(),
///               tutorial_in_progress()
/// </file-summary>

using System;
using MoonSharp.Interpreter;

namespace gregCore.Infrastructure.Scripting.Lua.Modules;

public static class LuaObjectivesModule
{
    public static void Register(Table greg, Script script, string modId)
    {
        var objectives = new Table(script);

        // greg.objectives.show(index) → bool
        objectives["show"] = (Func<int, bool>)((index) =>
        {
            try { return gregCore.Core.Networking.GregTutorials.ShowTutorial(index); }
            catch { return false; }
        });

        // greg.objectives.stop() → bool
        objectives["stop"] = (Func<bool>)(() =>
        {
            try { return gregCore.Core.Networking.GregTutorials.StopTutorial(); }
            catch { return false; }
        });

        // greg.objectives.skip() → bool (alle Tutorials überspringen)
        objectives["skip"] = (Func<bool>)(() =>
        {
            try { return gregCore.Core.Networking.GregTutorials.SkipTutorials(); }
            catch { return false; }
        });

        // greg.objectives.active() → array of objective UIDs
        objectives["active"] = (Func<Table>)(() =>
        {
            try
            {
                var result = new Table(script);
                int i = 1;
                foreach (int uid in gregCore.Core.Networking.GregObjectives.GetActiveObjectiveUIDs())
                    result[i++] = uid;
                return result;
            }
            catch (Exception ex)
            {
                LuaLog.Error($"[LuaMod:{modId}] objectives.active() failed: {ex.Message}");
                return new Table(script);
            }
        });

        // greg.objectives.tutorial_in_progress() → bool
        objectives["tutorial_in_progress"] = (Func<bool>)(() =>
        {
            try { return gregCore.Core.Networking.GregObjectives.IsTutorialInProgress(); }
            catch { return false; }
        });

        // greg.objectives.create({loc=, uid=, x=, y=, z=, xp=?, rep=?, sub=?}) → bool
        objectives["create"] = (Func<DynValue, bool>)((spec) =>
        {
            try
            {
                if (spec == null || spec.Type != DataType.Table) return false;
                var t = spec.Table;
                int loc = Num(t, "loc");
                int uid = Num(t, "uid");
                float x = FNum(t, "x"), y = FNum(t, "y"), z = FNum(t, "z");
                int xp = Num(t, "xp"), rep = Num(t, "rep");
                bool sub = t.Get("sub").Type == DataType.Boolean && t.Get("sub").Boolean;
                return gregCore.Core.Networking.GregObjectives.CreateObjective(
                    loc, uid, new UnityEngine.Vector3(x, y, z), xp, rep, sub);
            }
            catch { return false; }
        });

        // greg.objectives.start(uid, x, y, z) → bool
        objectives["start"] = (Func<int, double, double, double, bool>)((uid, x, y, z) =>
        {
            try
            {
                return gregCore.Core.Networking.GregObjectives.StartObjective(
                    uid, new UnityEngine.Vector3((float)x, (float)y, (float)z));
            }
            catch { return false; }
        });

        // greg.objectives.clear() → bool
        objectives["clear"] = (Func<bool>)(() =>
        {
            try { return gregCore.Core.Networking.GregObjectives.ClearObjectives(); }
            catch { return false; }
        });

        greg["objectives"] = objectives;
    }

    internal static int Num(Table t, string key)
    {
        try
        {
            var v = t.Get(key);
            return v.Type == DataType.Number ? (int)v.Number : 0;
        }
        catch { return 0; }
    }

    internal static float FNum(Table t, string key)
    {
        try
        {
            var v = t.Get(key);
            return v.Type == DataType.Number ? (float)v.Number : 0f;
        }
        catch { return 0f; }
    }
}
