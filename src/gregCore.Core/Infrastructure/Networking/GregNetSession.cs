/// <file-summary>
/// Layer:       Infrastructure (Networking)
/// Purpose:     Safe, null-tolerant access to the game's own
///               coop session (Il2Cpp.CoopSession + Il2CppCoop.Net transport).
///               Single-player (no/inactive session) counts as host:
///               everything runs as before. In multiplayer only the host
///               may write the world (single-writer) - clients observe.
/// Maintainer:  Do not inject frames into the game network: the game owns
///               the wire (NetMsg/chunking/snapshots). The framework rides
///               on host authority + game opcodes and only hooks into
///               transport events for join convergence.
/// </file-summary>

using System.Diagnostics.CodeAnalysis;
using MelonLoader;
using UnityEngine;

namespace gregCore.Infrastructure.Networking;

[ExcludeFromCodeCoverage(Justification = "Thin shell over live game Coop session; needs running game.")]
public static class GregNetSession
{
    private static global::Il2Cpp.CoopSession? _cached;
    private static float _nextRefreshRealtime;
    private static bool _peerHooksInstalled;
    private static ulong _lastPeerCountSeen;
    // True once MelonLoader has reported the first scene load.
    // Before that the Unity object list is being rebuilt: FindObjectsOfType hangs
    // the main thread then (white screen under IL2CPP/Proton).
    private static bool _sceneReady;
    private static float _initRealtime;

    public static event Action? PeerJoined;
    public static event Action? PeerLeft;

    // True while a session is running (host or client).
    public static bool IsMultiplayerActive
    {
        get
        {
            try
            {
                var s = Session;
                return s != null && s.IsActive;
            }
            catch { return false; }
        }
    }

    // Single-player (inactive/missing) == host: don't change behavior.
    public static bool IsHost
    {
        get
        {
            try
            {
                var s = Session;
                if (s == null || !s.IsActive) return true;
                return s.IsHost;
            }
            catch { return true; }
        }
    }

    // The game is currently applying a remote command: don't trigger new
    // framework actions (echo protection).
    public static bool IsApplyingRemote
    {
        get
        {
            try
            {
                var s = Session;
                return s != null && s.IsActive && s.IsApplyingRemote;
            }
            catch { return false; }
        }
    }

    // May this instance modify the world? Host-only in MP.
    public static bool CanMutateWorld => IsHost && !IsApplyingRemote;

    public static ulong PeerCount
    {
        get
        {
            try
            {
                // Straight to the field, NOT via Session: avoids recursion
                // (Session -> Refresh -> PollPeerChanges -> PeerCount).
                var peers = _cached?.peers;
                return peers != null ? (ulong)peers.Count : 0;
            }
            catch { return 0; }
        }
    }

    // Keep scene snapshots cheap: cache CoopSession lookup at most 1x/2s -
    // INCLUDING negative results (no session found): otherwise every frame with
    // a missing session would run a full search and starve the main thread
    // (white screen). Called by the framework tick and on demand.
    // Before the first scene load (and while loading) no lookup:
    // the object list is being rebuilt then. Solo default applies (IsHost == true).
    public static void Refresh()
    {
        try
        {
            if (!_sceneReady)
            {
                // Fallback in case the scene callback never fires: after 120s
                // of real time every load is safely over.
                if (_initRealtime <= 0f) _initRealtime = Time.realtimeSinceStartup;
                if (Time.realtimeSinceStartup - _initRealtime < 120f) return;
            }

            float now = Time.realtimeSinceStartup;
            if (now < _nextRefreshRealtime)
            {
                PollPeerChanges();
                return;
            }
            _nextRefreshRealtime = now + 2f;
            var found = UnityEngine.Object.FindObjectsOfType<global::Il2Cpp.CoopSession>();
            _cached = (found != null && found.Length > 0) ? found[0] : null;
            EnsurePeerHooks();
            PollPeerChanges();
        }
        catch (Exception ex)
        {
            MelonLogger.Warning($"[gregCore][Net] Session refresh failed: {ex.GetBaseException().Message}");
        }
    }

    private static global::Il2Cpp.CoopSession? Session
    {
        get
        {
            try
            {
                if (_cached == null) Refresh();
                return _cached;
            }
            catch { return null; }
        }
    }

    // Called by GregCoreMod.OnSceneWasLoaded (MelonLoader callback,
    // fires AFTER loading completed): lookups are safe from here on.
    public static void NotifySceneLoaded()
    {
        _sceneReady = true;
        _cached = null;
        _nextRefreshRealtime = 0f;
    }

    // Called on unload: solo default again during the next load,
    // no lookup.
    public static void NotifySceneUnloading()
    {
        _sceneReady = false;
        _cached = null;
    }

    private static void EnsurePeerHooks()
    {
        if (_peerHooksInstalled) return;
        try
        {
            var transport = _cached?.transport;
            if (transport == null || !transport.IsRunning) return;
            transport.add_PeerConnected(new Action<global::Il2CppCoop.Net.NetPeer>(_ => OnPeersChanged()));
            transport.add_PeerDisconnected(new Action<global::Il2CppCoop.Net.NetPeer>(_ => OnPeersChanged()));
            _peerHooksInstalled = true;
            _lastPeerCountSeen = PeerCount;
        }
        catch (Exception ex)
        {
            MelonLogger.Warning($"[gregCore][Net] Peer hooks failed: {ex.GetBaseException().Message}");
        }
    }

    private static void PollPeerChanges()
    {
        try
        {
            ulong now = PeerCount;
            if (now == _lastPeerCountSeen) return;
            bool joined = now > _lastPeerCountSeen;
            _lastPeerCountSeen = now;
            if (joined) PeerJoined?.Invoke();
            else PeerLeft?.Invoke();
        }
        catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
    }

    private static void OnPeersChanged()
    {
        try { PollPeerChanges(); }
        catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
    }
}
