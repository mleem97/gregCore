/// <file-summary>
/// Schicht:      Infrastructure
/// Zweck:        Lua-API für World/Time-Management.
/// Maintainer:   greg.world.time_of_day(), day(), set_time_scale(), pause(), resume()
/// </file-summary>

using System;
using MoonSharp.Interpreter;
using MelonLoader;

namespace gregCore.Infrastructure.Scripting.Lua.Modules;

public static class LuaWorldModule
{
    public static void Register(Table greg, Script script, string modId)
    {
        var worldTable = new Table(script);

        // greg.world.time_of_day() → number (0.0 - 1.0)
        worldTable["time_of_day"] = (Func<double>)(() =>
        {
            try { return API.GregAPI.GetTimeOfDay(); }
            catch { return 0.0; }
        });

        // greg.world.day() → number
        worldTable["day"] = (Func<int>)(() =>
        {
            try { return (int)API.GregAPI.GetDay(); }
            catch { return 1; }
        });

        // greg.world.seconds_in_day() → number
        worldTable["seconds_in_day"] = (Func<double>)(() =>
        {
            try { return API.GregAPI.GetSecondsInFullDay(); }
            catch { return 1200.0; }
        });

        // greg.world.set_seconds_in_day(seconds)
        worldTable["set_seconds_in_day"] = (Action<double>)((val) =>
        {
            try { API.GregAPI.SetSecondsInFullDay((float)val); }
            catch (Exception ex) { MelonLogger.Error($"[LuaMod:{modId}] world.set_seconds_in_day failed: {ex.Message}"); }
        });

        // greg.world.time_scale() → number
        worldTable["time_scale"] = (Func<double>)(() =>
        {
            try { return API.GregAPI.GetTimeScale(); }
            catch { return 1.0; }
        });

        // greg.world.set_time_scale(scale)
        worldTable["set_time_scale"] = (Action<double>)((val) =>
        {
            try { API.GregAPI.SetTimeScale((float)val); }
            catch (Exception ex) { MelonLogger.Error($"[LuaMod:{modId}] world.set_time_scale failed: {ex.Message}"); }
        });

        // greg.world.pause()
        worldTable["pause"] = (Action)(() =>
        {
            try { API.GregAPI.SetGamePaused(true); }
            catch (Exception ex) { MelonLogger.Error($"[LuaMod:{modId}] world.pause failed: {ex.Message}"); }
        });

        // greg.world.resume()
        worldTable["resume"] = (Action)(() =>
        {
            try { API.GregAPI.SetGamePaused(false); }
            catch (Exception ex) { MelonLogger.Error($"[LuaMod:{modId}] world.resume failed: {ex.Message}"); }
        });

        // greg.world.is_paused() → bool
        worldTable["is_paused"] = (Func<bool>)(() =>
        {
            try { return API.GregAPI.IsGamePaused(); }
            catch { return false; }
        });

        // greg.world.scene() → string
        worldTable["scene"] = (Func<string>)(() =>
        {
            try { return API.GregAPI.GetCurrentScene(); }
            catch { return "None"; }
        });

        // greg.world.difficulty() → number
        worldTable["difficulty"] = (Func<int>)(() =>
        {
            try { return API.GregAPI.GetDifficulty(); }
            catch { return 1; }
        });

        // greg.world.save() → bool
        worldTable["save"] = (Func<bool>)(() =>
        {
            try { return API.GregAPI.TriggerSave() == 1; }
            catch { return false; }
        });

        // greg.world.server_count() → number
        worldTable["server_count"] = (Func<int>)(() =>
        {
            try { return (int)API.GregAPI.GetServerCount(); }
            catch { return 0; }
        });

        // greg.world.rack_count() → number
        worldTable["rack_count"] = (Func<int>)(() =>
        {
            try { return (int)API.GregAPI.GetRackCount(); }
            catch { return 0; }
        });

        // greg.world.switch_count() → number
        worldTable["switch_count"] = (Func<int>)(() =>
        {
            try { return (int)API.GregAPI.GetSwitchCount(); }
            catch { return 0; }
        });

        greg["world"] = worldTable;
    }
}
