/// <file-summary>
/// Layer:       Core (Networking)
/// Purpose:     Coop bridge: session (Ensure/Shutdown), peers/avatars
///               (find, read, remove), held items, loose items
///               (register, announce), trolley ghosts, send chat.
///               All best-effort.
/// </file-summary>

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using MelonLoader;
using UnityEngine;

namespace gregCore.Core.Networking;

[ExcludeFromCodeCoverage(Justification = "Live Il2Cpp interop against game assemblies; needs running game.")]
public static class GregCoop
{
    // ── DTO ──────────────────────────────────────────────────────────────────

    public sealed class PeerInfo
    {
        public ulong PeerId { get; set; }
        public Vector3 Position { get; set; }
        public float Yaw { get; set; }
        public string Hand { get; set; } = "";
        public int SubType { get; set; }
        public bool HasHeldColor { get; set; }
        public float LastSeenTime { get; set; }
    }

    // ── Session ──────────────────────────────────────────────────────────────

    public static bool EnsureSession()
    {
        try
        {
            global::Il2Cpp.CoopBootstrap.EnsureSession();
            return true;
        }
        catch (Exception ex)
        {
            Warn($"EnsureSession failed: {Base(ex)}");
            return false;
        }
    }

    public static bool ShutdownSession()
    {
        try
        {
            global::Il2Cpp.CoopBootstrap.ShutdownSession();
            return true;
        }
        catch (Exception ex)
        {
            Warn($"ShutdownSession failed: {Base(ex)}");
            return false;
        }
    }

    // ── PlayerSync / Peers ───────────────────────────────────────────────────

    public static global::Il2Cpp.CoopPlayerSync FindPlayerSync()
    {
        global::Il2Cpp.CoopPlayerSync found = null;
        Try(() =>
        {
            var all = Resources.FindObjectsOfTypeAll<global::Il2Cpp.CoopPlayerSync>();
            if (all == null) return;
            foreach (var s in all)
            {
                if (s == null) continue;
                try
                {
                    var go = s.gameObject;
                    if (go != null && go.scene.IsValid() && go.scene.isLoaded)
                    {
                        found = s;
                        break;
                    }
                }
                catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
            }
        });
        return found;
    }

    public static List<PeerInfo> GetPeers()
    {
        var result = new List<PeerInfo>();
        var sync = FindPlayerSync();
        if (sync == null) return result;
        Try(() =>
        {
            _ = sync.gameObject; // liveness
            var dict = sync.avatars;
            if (dict == null) return;
            foreach (var kv in dict)
            {
                try
                {
                    var avatar = kv.Value;
                    if (avatar == null) continue;
                    var info = new PeerInfo { PeerId = kv.Key };
                    try { info.Position = avatar.targetPos; } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
                    try { info.Yaw = avatar.targetYaw; } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
                    try { info.Hand = avatar.currentHand.ToString(); } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
                    try { info.SubType = avatar.currentSubType; } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
                    try { info.HasHeldColor = avatar.currentHasHeldColor; } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
                    try { info.LastSeenTime = avatar.LastSeenTime; } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
                    result.Add(info);
                }
                catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
            }
        });
        return result;
    }

    public static bool RemoveAvatar(ulong peerId)
    {
        var sync = FindPlayerSync();
        if (sync == null) return false;
        try
        {
            _ = sync.gameObject; // liveness
            sync.RemoveAvatar(peerId);
            return true;
        }
        catch (Exception ex)
        {
            Warn($"RemoveAvatar failed: {Base(ex)}");
            return false;
        }
    }

    public static bool ForceResend()
    {
        var sync = FindPlayerSync();
        if (sync == null) return false;
        try
        {
            _ = sync.gameObject; // liveness
            sync.ForceResend();
            return true;
        }
        catch (Exception ex)
        {
            Warn($"ForceResend failed: {Base(ex)}");
            return false;
        }
    }

    public static float GetPeerTimeout()
    {
        try { return global::Il2Cpp.CoopPlayerSync.PeerTimeout; }
        catch { return -1f; }
    }

    // ── Held items (local player) ────────────────────────────────────────────

    private static global::Il2Cpp.PlayerManager LocalPlayer()
    {
        try { return global::Il2Cpp.PlayerManager.instance; }
        catch { return null; }
    }

    public static string GetHeldHandType()
    {
        var pm = LocalPlayer();
        if (pm == null) return "";
        try { return global::Il2Cpp.CoopPlayerSync.GetHeldHandType(pm).ToString(); }
        catch { return ""; }
    }

    public static int GetHeldSubType()
    {
        var pm = LocalPlayer();
        if (pm == null) return -1;
        try { return global::Il2Cpp.CoopPlayerSync.GetHeldSubType(pm); }
        catch { return -1; }
    }

    public static Color GetHeldColor(Color fallback)
    {
        var pm = LocalPlayer();
        if (pm == null) return fallback;
        try
        {
            Color color = fallback;
            if (global::Il2Cpp.CoopPlayerSync.TryGetHeldColor(pm, out color))
                return color;
            return fallback;
        }
        catch { return fallback; }
    }

    // ── Loose-Items ──────────────────────────────────────────────────────────

    public static bool RegisterLoose(global::Il2Cpp.UsableObject uo)
    {
        if (uo == null) return false;
        try
        {
            global::Il2Cpp.CoopLooseItems.Register(uo);
            return true;
        }
        catch (Exception ex)
        {
            Warn($"Loose-Register failed: {Base(ex)}");
            return false;
        }
    }

    public static bool UnregisterLoose(global::Il2Cpp.UsableObject uo)
    {
        if (uo == null) return false;
        try
        {
            global::Il2Cpp.CoopLooseItems.Unregister(uo);
            return true;
        }
        catch (Exception ex)
        {
            Warn($"Loose-Unregister failed: {Base(ex)}");
            return false;
        }
    }

    public static global::Il2Cpp.UsableObject GetLooseById(string coopLooseId)
    {
        if (string.IsNullOrEmpty(coopLooseId)) return null;
        try { return global::Il2Cpp.CoopLooseItems.GetById(coopLooseId); }
        catch { return null; }
    }

    public static bool AnnounceDropped(global::Il2Cpp.UsableObject uo)
    {
        if (uo == null) return false;
        try
        {
            global::Il2Cpp.CoopLooseItems.AnnounceDropped(uo);
            return true;
        }
        catch (Exception ex)
        {
            Warn($"AnnounceDropped failed: {Base(ex)}");
            return false;
        }
    }

    public static bool AnnouncePickedUp(global::Il2Cpp.UsableObject uo)
    {
        if (uo == null) return false;
        try
        {
            global::Il2Cpp.CoopLooseItems.AnnouncePickedUp(uo);
            return true;
        }
        catch (Exception ex)
        {
            Warn($"AnnouncePickedUp failed: {Base(ex)}");
            return false;
        }
    }

    public static bool AnnounceTrolleyItem(global::Il2Cpp.UsableObject uo)
    {
        if (uo == null) return false;
        try
        {
            global::Il2Cpp.CoopLooseItems.AnnounceTrolleyItem(uo);
            return true;
        }
        catch (Exception ex)
        {
            Warn($"AnnounceTrolleyItem failed: {Base(ex)}");
            return false;
        }
    }

    // ── Trolley-Ghosts ───────────────────────────────────────────────────────

    public static global::Il2Cpp.CoopTrolleySync FindTrolleySync()
    {
        global::Il2Cpp.CoopTrolleySync found = null;
        Try(() =>
        {
            var all = Resources.FindObjectsOfTypeAll<global::Il2Cpp.CoopTrolleySync>();
            if (all == null) return;
            foreach (var s in all)
            {
                if (s == null) continue;
                try
                {
                    var go = s.gameObject;
                    if (go != null && go.scene.IsValid() && go.scene.isLoaded)
                    {
                        found = s;
                        break;
                    }
                }
                catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
            }
        });
        return found;
    }

    public static List<ulong> GetTrolleyGhostIds()
    {
        var result = new List<ulong>();
        var sync = FindTrolleySync();
        if (sync == null) return result;
        Try(() =>
        {
            _ = sync.gameObject; // liveness
            var dict = sync.ghosts;
            if (dict == null) return;
            foreach (var kv in dict)
            {
                try { result.Add(kv.Key); } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
            }
        });
        return result;
    }

    public static bool RemoveTrolleyGhost(ulong peerId)
    {
        var sync = FindTrolleySync();
        if (sync == null) return false;
        try
        {
            _ = sync.gameObject; // liveness
            sync.RemoveGhost(peerId);
            return true;
        }
        catch (Exception ex)
        {
            Warn($"RemoveGhost failed: {Base(ex)}");
            return false;
        }
    }

    // ── Chat ─────────────────────────────────────────────────────────────────

    public static global::Il2Cpp.ChatController FindChat()
    {
        global::Il2Cpp.ChatController found = null;
        Try(() =>
        {
            var all = Resources.FindObjectsOfTypeAll<global::Il2Cpp.ChatController>();
            if (all == null) return;
            foreach (var c in all)
            {
                if (c == null) continue;
                try
                {
                    var go = c.gameObject;
                    if (go != null && go.scene.IsValid() && go.scene.isLoaded)
                    {
                        found = c;
                        break;
                    }
                }
                catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
            }
        });
        return found;
    }

    public static bool SendToChat(string text)
    {
        if (string.IsNullOrEmpty(text)) return false;
        var chat = FindChat();
        if (chat == null) return false;
        try
        {
            _ = chat.gameObject; // liveness
            chat.AddToChatOutput(text);
            return true;
        }
        catch (Exception ex)
        {
            Warn($"SendToChat failed: {Base(ex)}");
            return false;
        }
    }

    private static string Base(Exception ex)
    {
        try { return ex != null ? ex.GetBaseException().Message : "?"; } catch { return "?"; }
    }

    private static void Warn(string message)
    {
        try { MelonLogger.Warning($"[gregCore][Net] Coop: {message}"); } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
    }

    private static void Try(Action action)
    {
        try { action?.Invoke(); }
        catch (Exception ex)
        {
            try { MelonLogger.Warning($"[gregCore][Net] Coop field failed: {ex.GetBaseException().Message}"); } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
        }
    }
}
