using System;
using MoonSharp.Interpreter;

namespace gregCore.Infrastructure.Scripting.Lua.Dev;

public sealed partial class LuaRepl
{
    // Registers basic logging functions in the REPL greg table.
    private void RegisterLoggingApi(Table gregTable)
    {
        try
        {
            gregTable["log_info"] = (Action<string>)(msg => AddOutput($"[INFO] {msg}"));
            gregTable["log_warning"] = (Action<string>)(msg => AddOutput($"[WARN] {msg}"));
            gregTable["log_error"] = (Action<string>)(msg => AddOutput($"[ERROR] {msg}"));
        }
        catch { /* ignored: defensive best-effort (CONVENTIONS.md) */  }
    }

    // Registers economy accessors in the REPL greg table.
    private static void RegisterEconomyApi(Table gregTable)
    {
        try
        {
            gregTable["get_money"] = (Func<double>)(() => API.GregAPI.GetPlayerMoney());
            gregTable["set_money"] = (Action<double>)(v => API.GregAPI.SetPlayerMoney(v));
            gregTable["get_xp"] = (Func<double>)(() => API.GregAPI.GetPlayerXp());
            gregTable["get_reputation"] = (Func<double>)(() => API.GregAPI.GetPlayerReputation());
        }
        catch { }
    }

    // Registers world query functions in the REPL greg table.
    private static void RegisterWorldApi(Table gregTable)
    {
        try
        {
            gregTable["server_count"] = (Func<int>)(() => (int)API.GregAPI.GetServerCount());
            gregTable["rack_count"] = (Func<int>)(() => (int)API.GregAPI.GetRackCount());
            gregTable["time_of_day"] = (Func<double>)(() => API.GregAPI.GetTimeOfDay());
            gregTable["day"] = (Func<int>)(() => (int)API.GregAPI.GetDay());
            gregTable["scene"] = (Func<string>)(() => API.GregAPI.GetCurrentScene());
            gregTable["is_paused"] = (Func<bool>)(() => API.GregAPI.IsGamePaused());
            gregTable["pause"] = (Action)(() => API.GregAPI.SetGamePaused(true));
            gregTable["resume"] = (Action)(() => API.GregAPI.SetGamePaused(false));
        }
        catch { }
    }

    // Redirects Lua print() into the REPL output pane.
    private void RegisterPrintHelper(Script replScript)
    {
        try
        {
            replScript.Globals["print"] = (Action<DynValue[]>)(args =>
            {
                try
                {
                    string line = string.Join("\t", Array.ConvertAll(args, a => a.ToPrintString()));
                    AddOutput(line);
                }
                catch { }
            });
        }
        catch { }
    }

    // Prints the REPL welcome banner.
    private void PrintWelcome()
    {
        try
        {
            AddOutput("─── gregCore Lua REPL v1.1.0 ───");
            AddOutput("Type Lua expressions. Press Enter to evaluate.");
            AddOutput("Use greg.* for API access. Type 'help()' for commands.");
        }
        catch { }
    }

    // Registers the help() command listing available calls.
    private void RegisterHelpCommand(Script replScript)
    {
        try
        {
            replScript.Globals["help"] = (Action)(() => PrintHelp());
        }
        catch { }
    }

    // Prints the help listing.
    private void PrintHelp()
    {
        try
        {
            AddOutput("── Available commands ──");
            AddOutput("  greg.get_money()       → Player money");
            AddOutput("  greg.set_money(1000)   → Set money");
            AddOutput("  greg.server_count()    → Active servers");
            AddOutput("  greg.rack_count()      → Active racks");
            AddOutput("  greg.time_of_day()     → Current time");
            AddOutput("  greg.day()             → Current day");
            AddOutput("  greg.scene()           → Active scene");
            AddOutput("  greg.pause() / resume()");
            AddOutput("  clear()                → Clear output");
        }
        catch { }
    }
}
