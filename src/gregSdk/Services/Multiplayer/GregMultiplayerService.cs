using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MelonLoader;

namespace greg.Sdk.Services.Multiplayer;

public class LobbyPlayer
{
    public string PlayerId { get; set; }
    public string DisplayName { get; set; }
    public bool IsHost { get; set; }
    public bool IsReady { get; set; }
    public DateTime JoinedAt { get; set; }
}

/// <summary>
/// Core SDK service for multiplayer connection, lobby management, and messaging.
/// Uses native ClientWebSocket to connect to the dedicated NodeJS relay server.
/// </summary>
public static class GregMultiplayerService
{
    public static bool IsConnected { get; private set; }
    public static int ConnectedPlayers { get; private set; }
    public static string LobbyCode { get; private set; }
    public static string HostName { get; private set; }
    public static bool IsHost { get; private set; }
    public static int MaxPlayers { get; set; } = 4;

    private static readonly List<LobbyPlayer> _players = new();
    public static IReadOnlyList<LobbyPlayer> Players => _players;

    private static readonly List<string> _messageLog = new();
    
    private static ClientWebSocket _ws;
    private static CancellationTokenSource _cts;
    private static string _relayUrl = "ws://localhost:3000";

    public static void SetRelayServer(string url)
    {
        _relayUrl = url;
    }

    private static async Task ConnectToServerAsync(string lobbyCode, bool asHost, string displayName)
    {
        if (_ws != null && _ws.State == WebSocketState.Open)
        {
            await _ws.CloseAsync(WebSocketCloseStatus.NormalClosure, "Reconnecting", CancellationToken.None);
            _ws.Dispose();
        }

        _cts = new CancellationTokenSource();
        _ws = new ClientWebSocket();

        MelonLogger.Msg($"[Multiplayer] Connecting to Relay: {_relayUrl} for Lobby {lobbyCode}");
        
        try 
        {
            await _ws.ConnectAsync(new Uri(_relayUrl), _cts.Token);
            IsConnected = true;
            LobbyCode = lobbyCode;
            IsHost = asHost;
            _messageLog.Add($"[{DateTime.Now:HH:mm:ss}] Connected to relay server.");

            var joinMsg = $"{{\"type\":\"join\",\"lobby\":\"{lobbyCode}\",\"host\":{asHost.ToString().ToLower()},\"name\":\"{displayName}\"}}";
            await BroadcastRawAsync(joinMsg);

            _ = Task.Run(ReceiveLoop);
        }
        catch (Exception ex)
        {
            MelonLogger.Error($"[Multiplayer] Connection Failed: {ex.Message}");
            IsConnected = false;
        }
    }

    private static async Task ReceiveLoop()
    {
        var buffer = new byte[1024 * 4];
        while (IsConnected && _ws.State == WebSocketState.Open && !_cts.IsCancellationRequested)
        {
            try 
            {
                var result = await _ws.ReceiveAsync(new ArraySegment<byte>(buffer), _cts.Token);
                if (result.MessageType == WebSocketMessageType.Close)
                {
                    IsConnected = false;
                    _messageLog.Add($"[{DateTime.Now:HH:mm:ss}] Disconnected from relay server.");
                    break;
                }
                else
                {
                    string message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                    _messageLog.Add($"[{DateTime.Now:HH:mm:ss}] RX: {message}");
                    
                    if (message.Contains("\"type\":\"sync\""))
                    {
                        MelonLogger.Msg($"[Multiplayer] Received sync payload");
                    }
                }
            }
            catch (Exception ex)
            {
                if (_cts.IsCancellationRequested) break;
                MelonLogger.Error($"[Multiplayer] Receive Error: {ex.Message}");
                break;
            }
        }
    }

    private static async Task BroadcastRawAsync(string message)
    {
        if (!IsConnected || _ws == null || _ws.State != WebSocketState.Open) return;
        var bytes = Encoding.UTF8.GetBytes(message);
        await _ws.SendAsync(new ArraySegment<byte>(bytes), WebSocketMessageType.Text, true, _cts.Token);
    }

    public static bool CreateLobby(string lobbyName, int maxPlayers = 4, string password = null)
    {
        MaxPlayers = maxPlayers;
        HostName = lobbyName;
        string newLobbyCode = GenerateLobbyCode();
        
        _ = ConnectToServerAsync(newLobbyCode, true, lobbyName);

        _players.Clear();
        _players.Add(new LobbyPlayer
        {
            PlayerId = "host",
            DisplayName = lobbyName,
            IsHost = true,
            IsReady = true,
            JoinedAt = DateTime.UtcNow
        });

        return true;
    }

    public static bool JoinLobby(string lobbyCode, string password = null)
    {
        _ = ConnectToServerAsync(lobbyCode, false, "ClientPlayer");

        _players.Clear();
        _players.Add(new LobbyPlayer
        {
            PlayerId = "local",
            DisplayName = "ClientPlayer",
            IsHost = false,
            IsReady = false,
            JoinedAt = DateTime.UtcNow
        });

        return true;
    }

    public static void LeaveLobby()
    {
        if (_ws != null && _ws.State == WebSocketState.Open)
        {
            _cts.Cancel();
            _ws.CloseAsync(WebSocketCloseStatus.NormalClosure, "Left Lobby", CancellationToken.None).Wait(1000);
            _ws.Dispose();
        }
        IsConnected = false;
        IsHost = false;
        ConnectedPlayers = 0;
        LobbyCode = null;
        _players.Clear();
        _messageLog.Add($"[{DateTime.Now:HH:mm:ss}] Left lobby");
    }

    public static void Broadcast(string eventName, string json)
    {
        if (!IsConnected || _ws == null || _ws.State != WebSocketState.Open) return;

        string payload = $"{{\"type\":\"event\",\"event\":\"{eventName}\",\"data\":{json}}}";
        _ = BroadcastRawAsync(payload);
        
        _messageLog.Add($"[{DateTime.Now:HH:mm:ss}] TX: {eventName}");
    }

    public static void Tick(float deltaTime) { }

    public static void OnSceneLoaded(string sceneName)
    {
        if (!IsConnected) return;
        Broadcast("SceneLoaded", $"{{\"scene\":\"{sceneName}\"}}");
    }

    public static IReadOnlyList<string> GetMessageLog() => _messageLog;

    private static string GenerateLobbyCode()
    {
        const string chars = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789";
        var random = new Random();
        char[] code = new char[6];
        for (int i = 0; i < 6; i++)
            code[i] = chars[random.Next(chars.Length)];
        return new string(code);
    }
}

