/// <file-summary>
/// Schicht:      Infrastructure (Networking)
/// Zweck:        Sicherer, null-toleranter Zugriff auf die spieleigene
///               Coop-Session (Il2Cpp.CoopSession + Il2CppCoop.Net-Transport).
///               Single-Player (keine/inaktive Session) zaehlt als Host:
///               Alles laeuft wie bisher. Im Multiplayer darf nur der Host
///               Welt schreiben (Single-Writer) - Clients beobachten.
/// Maintainer:   Keine Frames ins Spiel-Netz injizieren: Das Spiel besitzt
///               den Draht (NetMsg/Chunking/Snapshots). Das Framework reitet
///               auf Host-Autoritaet + Spiel-Opcodes und haengt sich nur an
///               Transport-Events fuer Join-Konvergenz.
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
    // True, sobald MelonLoader das erste Szenen-Laden gemeldet hat.
    // Davor ist die Unity-Objektliste im Umbau: FindObjectsOfType haengt
    // dann den Main-Thread (White-Screen unter IL2CPP/Proton).
    private static bool _sceneReady;
    private static float _initRealtime;

    public static event Action? PeerJoined;
    public static event Action? PeerLeft;

    // true, wenn eine Session laeuft (Host oder Client).
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

    // Single-Player (inaktiv/fehlend) == Host: kein Verhalten aendern.
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

    // Das Spiel wendet gerade einen Remote-Befehl an: keine neuen
    // Framework-Aktionen ausloesen (Echo-Schutz).
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

    // Darf diese Instanz die Welt veraendern? Host-only im MP.
    public static bool CanMutateWorld => IsHost && !IsApplyingRemote;

    public static ulong PeerCount
    {
        get
        {
            try
            {
                // Direkt aufs Feld, NICHT ueber Session: vermeidet Rekursion
                // (Session -> Refresh -> PollPeerChanges -> PeerCount).
                var peers = _cached?.peers;
                return peers != null ? (ulong)peers.Count : 0;
            }
            catch { return 0; }
        }
    }

    // Szenen-Snapshot guenstig halten: CoopSession-Lookup max. 1x/2s cachen -
    // AUCH negativ (keine Session gefunden): Sonst wuerde jeder Frame bei
    // fehlender Session eine Vollsuche fahren und den Main-Thread verhungern
    // lassen (White-Screen). Wird vom Framework-Tick und bei Bedarf aufgerufen.
    // Vor dem ersten Szenen-Laden (und waehrend des Ladens) kein Lookup:
    // die Objektliste ist dann im Umbau. Es gilt Solo-Default (IsHost == true).
    public static void Refresh()
    {
        try
        {
            if (!_sceneReady)
            {
                // Fallback, falls der Szenen-Callback nie feuert: nach 120s
                // Echtzeit ist jeder Ladevorgang sicher vorbei.
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
            MelonLogger.Warning($"[gregCore][Net] Session-Refresh fehlgeschlagen: {ex.GetBaseException().Message}");
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

    // Wird von GregCoreMod.OnSceneWasLoaded aufgerufen (MelonLoader-Callback,
    // feuert NACH abgeschlossenem Laden): ab jetzt sind Lookups sicher.
    public static void NotifySceneLoaded()
    {
        _sceneReady = true;
        _cached = null;
        _nextRefreshRealtime = 0f;
    }

    // Wird beim Entladen aufgerufen: waehrend des naechsten Ladens wieder
    // Solo-Default, kein Lookup.
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
            MelonLogger.Warning($"[gregCore][Net] Peer-Hooks fehlgeschlagen: {ex.GetBaseException().Message}");
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
        catch { }
    }

    private static void OnPeersChanged()
    {
        try { PollPeerChanges(); }
        catch { }
    }
}
