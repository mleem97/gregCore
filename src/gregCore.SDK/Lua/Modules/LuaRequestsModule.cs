/// <file-summary>
/// Layer:       Infrastructure
/// Purpose:      Lua API for service requests (read-only).
/// Maintainer:   greg.requests.list(), current_number()
/// </file-summary>

using System;
using MoonSharp.Interpreter;

namespace gregCore.Infrastructure.Scripting.Lua.Modules;

public static class LuaRequestsModule
{
    public static void Register(Table greg, Script script, string modId)
    {
        var requestsTable = new Table(script);
        RegisterList(requestsTable, script, modId);
        RegisterCurrentNumber(requestsTable);
        RegisterAdd(requestsTable, modId);

        greg["requests"] = requestsTable;
    }

    private static void RegisterList(Table t, Script script, string modId)
    {

        // greg.requests.list() → array of {number, state, short, long, rewarded, done, progress}
        t["list"] = (Func<Table>)(() =>
        {
            try
            {
                var result = new Table(script);
                int i = 1;
                foreach (var r in gregCore.Core.Networking.GregServiceRequests.GetRequests())
                {
                    try
                    {
                        if (r == null) continue;
                        var t = new Table(script);
                        t["number"] = r.SrNumber;
                        t["state"] = r.State ?? "";
                        t["short"] = r.ShortDescription ?? "";
                        t["long"] = r.LongDescription ?? "";
                        t["rewarded"] = r.RewardGranted;
                        t["done"] = r.Completed;
                        t["progress"] = r.ProgressText ?? "";
                        result[i++] = t;
                    }
                    catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
                }
                return result;
            }
            catch (Exception ex)
            {
                LuaLog.Error($"[LuaMod:{modId}] requests.list() failed: {ex.Message}");
                return new Table(script);
            }
        });
    }

    private static void RegisterCurrentNumber(Table t)
    {

        // greg.requests.current_number() → number
        t["current_number"] = (Func<int>)(() =>
        {
            try { return gregCore.Core.Networking.GregServiceRequests.GetCurrentSRNumber(); }
            catch { return 0; }
        });
    }

    private static void RegisterAdd(Table t, string modId)
    {

        // greg.requests.add(spec) → bool (spec: number?, state?, short, long?)
        t["add"] = (Func<DynValue, bool>)((spec) =>
        {
            try
            {
                if (spec == null || spec.Type != DataType.Table) return false;
                var sr = new global::Il2Cpp.ServiceRequest();
                try
                {
                    var st = spec.Table;
                    string number = LuaServerModule.Str(st, "number");
                    int n;
                    if (!string.IsNullOrEmpty(number) && int.TryParse(number, out n)) sr.srNumber = n;
                    string state = LuaServerModule.Str(st, "state");
                    sr.state = string.Equals(state, "done", StringComparison.OrdinalIgnoreCase)
                        ? global::Il2Cpp.ServiceRequest.SRState.Resolved
                        : global::Il2Cpp.ServiceRequest.SRState.InProgress;
                    string sh = LuaServerModule.Str(st, "short");
                    if (sh != null) sr.shortDescription = sh;
                    string lo = LuaServerModule.Str(st, "long");
                    if (lo != null) sr.longDescription = lo;
                }
                catch { /* ignored: partial spec still usable */ }
                return gregCore.Core.Networking.GregServiceRequests.AddRequest(sr);
            }
            catch (Exception ex)
            {
                LuaLog.Error($"[LuaMod:{modId}] requests.add() failed: {ex.Message}");
                return false;
            }
        });
    }
}
