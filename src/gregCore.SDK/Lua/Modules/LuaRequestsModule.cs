/// <file-summary>
/// Schicht:      Infrastructure
/// Zweck:        Lua-API für Service-Requests (read-only).
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

        // greg.requests.list() → array of {number, state, short, long, rewarded, done, progress}
        requestsTable["list"] = (Func<Table>)(() =>
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

        // greg.requests.current_number() → number
        requestsTable["current_number"] = (Func<int>)(() =>
        {
            try { return gregCore.Core.Networking.GregServiceRequests.GetCurrentSRNumber(); }
            catch { return 0; }
        });

        greg["requests"] = requestsTable;
    }
}
