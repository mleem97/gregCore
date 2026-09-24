/// <file-summary>
/// Schicht:      Infrastructure
/// Zweck:        Lua-API für Coop-Sessions (Peers lesen, Session steuern).
/// Maintainer:   greg.coop.ensure(), shutdown(), peers(), remove_avatar(),
///               resend(), peer_timeout()
/// </file-summary>

using System;
using MoonSharp.Interpreter;

namespace gregCore.Infrastructure.Scripting.Lua.Modules;

public static class LuaCoopModule
{
    public static void Register(Table greg, Script script, string modId)
    {
        var coop = new Table(script);

        // greg.coop.ensure() → bool
        coop["ensure"] = (Func<bool>)(() =>
        {
            try { return gregCore.Core.Networking.GregCoop.EnsureSession(); }
            catch (Exception ex)
            {
                LuaLog.Error($"[LuaMod:{modId}] coop.ensure() failed: {ex.Message}");
                return false;
            }
        });

        // greg.coop.shutdown() → bool
        coop["shutdown"] = (Func<bool>)(() =>
        {
            try { return gregCore.Core.Networking.GregCoop.ShutdownSession(); }
            catch { return false; }
        });

        // greg.coop.peers() → array of {id, x, y, z, yaw}
        coop["peers"] = (Func<Table>)(() =>
        {
            try
            {
                var result = new Table(script);
                int i = 1;
                foreach (var p in gregCore.Core.Networking.GregCoop.GetPeers())
                {
                    try
                    {
                        if (p == null) continue;
                        var t = new Table(script);
                        t["id"] = (double)p.PeerId;
                        t["x"] = (double)p.Position.x;
                        t["y"] = (double)p.Position.y;
                        t["z"] = (double)p.Position.z;
                        t["yaw"] = (double)p.Yaw;
                        result[i++] = t;
                    }
                    catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
                }
                return result;
            }
            catch (Exception ex)
            {
                LuaLog.Error($"[LuaMod:{modId}] coop.peers() failed: {ex.Message}");
                return new Table(script);
            }
        });

        // greg.coop.remove_avatar(peerId) → bool
        coop["remove_avatar"] = (Func<double, bool>)((peerId) =>
        {
            try { return gregCore.Core.Networking.GregCoop.RemoveAvatar((ulong)peerId); }
            catch { return false; }
        });

        // greg.coop.resend() → bool
        coop["resend"] = (Func<bool>)(() =>
        {
            try { return gregCore.Core.Networking.GregCoop.ForceResend(); }
            catch { return false; }
        });

        // greg.coop.peer_timeout() → number
        coop["peer_timeout"] = (Func<double>)(() =>
        {
            try { return (double)gregCore.Core.Networking.GregCoop.GetPeerTimeout(); }
            catch { return 0.0; }
        });

        greg["coop"] = coop;
    }
}
