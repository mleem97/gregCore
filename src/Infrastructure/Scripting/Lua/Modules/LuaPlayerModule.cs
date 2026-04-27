/// <file-summary>
/// Schicht:      Infrastructure
/// Zweck:        Lua-API für Player-Interaktion.
/// Maintainer:   greg.player.position(), money(), xp(), teleport(), add_money()
///               Nutzt PlayerManager.instance und MainGameManager.instance.
/// </file-summary>

using System;
using MoonSharp.Interpreter;
using MelonLoader;

namespace gregCore.Infrastructure.Scripting.Lua.Modules;

public static class LuaPlayerModule
{
    public static void Register(Table greg, Script script, string modId)
    {
        var playerTable = new Table(script);

        // greg.player.position() → {x, y, z}
        playerTable["position"] = (Func<Table>)(() =>
        {
            try
            {
                var pos = API.GregAPI.GetPlayerPosition();
                var t = new Table(script);
                t["x"] = (double)pos.x;
                t["y"] = (double)pos.y;
                t["z"] = (double)pos.z;
                return t;
            }
            catch
            {
                var t = new Table(script);
                t["x"] = 0.0; t["y"] = 0.0; t["z"] = 0.0;
                return t;
            }
        });

        // greg.player.money() → number
        playerTable["money"] = (Func<double>)(() =>
        {
            try { return API.GregAPI.GetPlayerMoney(); }
            catch { return 0.0; }
        });

        // greg.player.set_money(amount)
        playerTable["set_money"] = (Action<double>)((val) =>
        {
            try { API.GregAPI.SetPlayerMoney(val); }
            catch (Exception ex) { MelonLogger.Error($"[LuaMod:{modId}] player.set_money failed: {ex.Message}"); }
        });

        // greg.player.add_money(amount)
        playerTable["add_money"] = (Action<double>)((amount) =>
        {
            try
            {
                double current = API.GregAPI.GetPlayerMoney();
                API.GregAPI.SetPlayerMoney(current + amount);
            }
            catch (Exception ex) { MelonLogger.Error($"[LuaMod:{modId}] player.add_money failed: {ex.Message}"); }
        });

        // greg.player.xp() → number
        playerTable["xp"] = (Func<double>)(() =>
        {
            try { return API.GregAPI.GetPlayerXp(); }
            catch { return 0.0; }
        });

        // greg.player.set_xp(amount)
        playerTable["set_xp"] = (Action<double>)((val) =>
        {
            try { API.GregAPI.SetPlayerXp(val); }
            catch (Exception ex) { MelonLogger.Error($"[LuaMod:{modId}] player.set_xp failed: {ex.Message}"); }
        });

        // greg.player.reputation() → number
        playerTable["reputation"] = (Func<double>)(() =>
        {
            try { return API.GregAPI.GetPlayerReputation(); }
            catch { return 0.0; }
        });

        // greg.player.set_reputation(amount)
        playerTable["set_reputation"] = (Action<double>)((val) =>
        {
            try { API.GregAPI.SetPlayerReputation(val); }
            catch (Exception ex) { MelonLogger.Error($"[LuaMod:{modId}] player.set_reputation failed: {ex.Message}"); }
        });

        // greg.player.teleport(x, y, z)
        playerTable["teleport"] = (Action<double, double, double>)((x, y, z) =>
        {
            try
            {
                var pm = Il2Cpp.PlayerManager.instance;
                if (pm?.playerClass == null || pm.playerGO == null) return;
                var pos = new UnityEngine.Vector3((float)x, (float)y, (float)z);
                var rot = pm.playerGO.transform.rotation;
                pm.playerClass.WarpPlayer(pos, rot);
            }
            catch (Exception ex) { MelonLogger.Error($"[LuaMod:{modId}] player.teleport failed: {ex.Message}"); }
        });

        // greg.player.is_crouching() → bool
        playerTable["is_crouching"] = (Func<bool>)(() =>
        {
            try
            {
                var pm = Il2Cpp.PlayerManager.instance;
                return pm?.fpc?.m_isCrouching ?? false;
            }
            catch { return false; }
        });

        // greg.player.is_sitting() → bool
        playerTable["is_sitting"] = (Func<bool>)(() =>
        {
            try
            {
                var pm = Il2Cpp.PlayerManager.instance;
                return pm?.fpc?.m_IsSitting ?? false;
            }
            catch { return false; }
        });

        greg["player"] = playerTable;
    }
}
