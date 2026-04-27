using MelonLoader;
using FishNet;
using FishNet.Managing;
using FishNet.Transporting;
using Il2CppInterop.Runtime.Attributes;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace greg.Multiplayer
{
    /// <summary>
    /// Core NetworkBehaviour singleton managing FishNet Listen-Server lifecycle,
    /// invite generation, approval queue, and state-sync orchestration.
    /// IL2CPP: registered via ClassInjector in GregMultiplayerMod.
    /// </summary>
    [Il2CppImplements(typeof(FishNet.Object.NetworkBehaviour))]
    public class GregRelayService : MonoBehaviour
    {
        public static GregRelayService Instance { get; private set; }
        private static MultiplayerConfig _cfg;
        private NetworkManager _fishNet;

        // Pending join approvals: sessionKey -> playerName
        private readonly Dictionary<string, string> _pendingApprovals = new();

        // Events for other greg mods to hook
        public static event Action<string> OnPlayerJoined;
        public static event Action<string> OnPlayerLeft;
        public static event Action<RackSyncPayload> OnRackStateReceived;
        public static event Action<CableSyncPayload> OnCableStateReceived;

        public static void Initialize(MultiplayerConfig cfg)
        {
            _cfg = cfg;
            var go = new GameObject("GregRelayService");
            Instance = go.AddComponent<GregRelayService>();
            UnityEngine.Object.DontDestroyOnLoad(go);
        }

        private void Awake()
        {
            // FishNet NetworkManager is placed by FishNet package – find or create
            _fishNet = InstanceFinder.NetworkManager;
            if (_fishNet == null)
            {
                var nmGo = new GameObject("FishNet_NetworkManager");
                _fishNet = nmGo.AddComponent<NetworkManager>();
                UnityEngine.Object.DontDestroyOnLoad(nmGo);
            }
        }

        // ─── Host API ────────────────────────────────────────────────────────────

        public bool StartListenServer()
        {
            try
            {
                _fishNet.ServerManager.StartConnection((ushort)_cfg.Port);
                _fishNet.ClientManager.StartConnection("localhost", (ushort)_cfg.Port);
                MelonLogger.Msg($"[greg.Relay] Listen-Server started on :{_cfg.Port}");
                return true;
            }
            catch (Exception ex)
            {
                MelonLogger.Error($"[greg.Relay] StartListenServer failed: {ex.Message}");
                return false;
            }
        }

        public string GenerateInviteLink()
        {
            string sessionKey = Guid.NewGuid().ToString("N")[..8].ToUpper();
            string localIp = GetLocalIp();
            // Format: gregmp://ip:port#SESSIONKEY
            return $"gregmp://{localIp}:{_cfg.Port}#{sessionKey}";
        }

        public string GenerateRelayInviteLink()
        {
            // For FishBait relay (Docker) usage
            string sessionKey = Guid.NewGuid().ToString("N")[..8].ToUpper();
            return $"gregmp://{_cfg.RelayHost}:{_cfg.RelayPort}#{sessionKey}";
        }

        // ─── Client API ──────────────────────────────────────────────────────────

        public bool JoinServer(string host, int port)
        {
            try
            {
                _fishNet.ClientManager.StartConnection(host, (ushort)port);
                MelonLogger.Msg($"[greg.Relay] Connecting to {host}:{port}");
                return true;
            }
            catch (Exception ex)
            {
                MelonLogger.Error($"[greg.Relay] JoinServer failed: {ex.Message}");
                return false;
            }
        }

        public static bool ParseInviteLink(string link, out string host, out int port, out string key)
        {
            host = string.Empty; port = 7777; key = string.Empty;
            if (!link.StartsWith("gregmp://")) return false;
            try
            {
                var body = link[9..]; // strip gregmp://
                var parts = body.Split('#');
                key = parts.Length > 1 ? parts[1] : string.Empty;
                var hostPort = parts[0].Split(':');
                host = hostPort[0];
                port = hostPort.Length > 1 ? int.Parse(hostPort[1]) : 7777;
                return true;
            }
            catch { return false; }
        }

        // ─── Sync Methods ────────────────────────────────────────────────────────

        [HideFromIl2Cpp]
        public void BroadcastRackSync(RackSyncPayload payload)
        {
            // Called from RackPatch after PlaceDevice on server
            // Serialise to bytes and send via FishNet ObserversRpc
            OnRackStateReceived?.Invoke(payload);
        }

        [HideFromIl2Cpp]
        public void BroadcastCableSync(CableSyncPayload payload)
        {
            OnCableStateReceived?.Invoke(payload);
        }

        // ─── Helpers ─────────────────────────────────────────────────────────────

        private static string GetLocalIp()
        {
            foreach (var ip in System.Net.Dns.GetHostAddresses(System.Net.Dns.GetHostName()))
                if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                    return ip.ToString();
            return "127.0.0.1";
        }

        public void Shutdown()
        {
            _fishNet?.ServerManager?.StopConnection(true);
            _fishNet?.ClientManager?.StopConnection();
            MelonLogger.Msg("[greg.Relay] Shutdown complete.");
        }
    }
}
