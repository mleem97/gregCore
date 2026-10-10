/// <file-summary>
/// Layer:       Core (Networking)
/// Purpose:     Steam leaderboards bridge (init, upload score, request own
///               entry, read best score/rank). Static
///               vanilla API, all best-effort.
/// </file-summary>

using System;
using System.Diagnostics.CodeAnalysis;
using MelonLoader;

namespace gregCore.Core.Networking;

[ExcludeFromCodeCoverage(Justification = "Live Steamworks interop; needs running game with Steam.")]
public static class GregSteam
{
    public static bool IsLeaderboardInitialized()
    {
        try { return global::Il2Cpp.SteamLeaderboards.s_initialized; }
        catch { return false; }
    }

    public static bool InitLeaderboards()
    {
        try
        {
            global::Il2Cpp.SteamLeaderboards.Init();
            return true;
        }
        catch (Exception ex)
        {
            Warn($"Init failed: {Base(ex)}");
            return false;
        }
    }

    public static bool UploadScore(float moneyPerSecond)
    {
        try
        {
            global::Il2Cpp.SteamLeaderboards.UploadScore(moneyPerSecond);
            return true;
        }
        catch (Exception ex)
        {
            Warn($"UploadScore failed: {Base(ex)}");
            return false;
        }
    }

    public static bool RequestUserEntry()
    {
        try
        {
            global::Il2Cpp.SteamLeaderboards.RequestUserEntry();
            return true;
        }
        catch (Exception ex)
        {
            Warn($"RequestUserEntry failed: {Base(ex)}");
            return false;
        }
    }

    public static int GetUserBestScore()
    {
        try { return global::Il2Cpp.SteamLeaderboards.UserBestScore; }
        catch { return -1; }
    }

    public static int GetUserGlobalRank()
    {
        try { return global::Il2Cpp.SteamLeaderboards.UserGlobalRank; }
        catch { return -1; }
    }

    private static string Base(Exception ex)
    {
        try { return ex != null ? ex.GetBaseException().Message : "?"; } catch { return "?"; }
    }

    private static void Warn(string message)
    {
        try { MelonLogger.Warning($"[gregCore][Net] Steam: {message}"); } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
    }
}
