/// <file-summary>
/// Schicht:      Infrastructure
/// Zweck:        Lua-API für Spiel-Lautstärken und Settings-Reload.
/// Maintainer:   greg.settings.set_master_volume(), set_music_volume(),
///               set_effect_volume(), set_racks_volume(), reload()
/// </file-summary>

using System;
using MoonSharp.Interpreter;

namespace gregCore.Infrastructure.Scripting.Lua.Modules;

public static class LuaSettingsModule
{
    public static void Register(Table greg, Script script, string modId)
    {
        var settings = new Table(script);

        // greg.settings.set_master_volume(v) → bool (0..1)
        settings["set_master_volume"] = (Func<double, bool>)((v) =>
        {
            try { return gregCore.Core.Networking.GregGameSettings.SetMasterVolume((float)v); }
            catch { return false; }
        });

        // greg.settings.set_music_volume(v) → bool
        settings["set_music_volume"] = (Func<double, bool>)((v) =>
        {
            try { return gregCore.Core.Networking.GregGameSettings.SetMusicVolume((float)v); }
            catch { return false; }
        });

        // greg.settings.set_effect_volume(v) → bool
        settings["set_effect_volume"] = (Func<double, bool>)((v) =>
        {
            try { return gregCore.Core.Networking.GregGameSettings.SetEffectVolume((float)v); }
            catch { return false; }
        });

        // greg.settings.set_racks_volume(v) → bool
        settings["set_racks_volume"] = (Func<double, bool>)((v) =>
        {
            try { return gregCore.Core.Networking.GregGameSettings.SetRacksVolume((float)v); }
            catch { return false; }
        });

        // greg.settings.reload() → bool
        settings["reload"] = (Func<bool>)(() =>
        {
            try { return gregCore.Core.Networking.GregGameSettings.ReloadSettings(); }
            catch (Exception ex)
            {
                LuaLog.Error($"[LuaMod:{modId}] settings.reload() failed: {ex.Message}");
                return false;
            }
        });

        greg["settings"] = settings;
    }
}
