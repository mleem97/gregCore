/// <file-summary>
/// Layer:       Core (Networking)
/// Purpose:     Steam lobby bridge: parse connect string, load avatars
///               (for coop peer display). All best-effort.
/// </file-summary>

using System;
using System.Diagnostics.CodeAnalysis;
using MelonLoader;
using UnityEngine;

namespace gregCore.Core.Networking;

[ExcludeFromCodeCoverage(Justification = "Live Steamworks interop; needs running game with Steam.")]
public static class GregSteamLobby
{
    public static ulong ParseLobbyFromConnect(string connect)
    {
        if (string.IsNullOrWhiteSpace(connect)) return 0;
        try { return global::Il2Cpp.SteamFriendLobbies.ParseLobbyFromConnect(connect); }
        catch { return 0; }
    }

    public static Texture2D LoadAvatar(ulong friendId)
    {
        if (friendId == 0) return null;
        try { return global::Il2Cpp.SteamFriendLobbies.LoadSteamAvatar(friendId); }
        catch (Exception ex)
        {
            try { MelonLogger.Warning($"[gregCore][Net] LoadSteamAvatar failed: {ex.GetBaseException().Message}"); } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
            return null;
        }
    }

    public static Texture2D BuildAvatarTexture(int handle)
    {
        if (handle == 0) return null;
        try { return global::Il2Cpp.SteamFriendLobbies.BuildTextureFromHandle(handle); }
        catch (Exception ex)
        {
            try { MelonLogger.Warning($"[gregCore][Net] BuildTextureFromHandle failed: {ex.GetBaseException().Message}"); } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
            return null;
        }
    }
}
