using System;
using System.IO;
using System.IO.Pipes;
using System.Text;
using Newtonsoft.Json;
using System.Threading;
using MelonLoader;
using UnityEngine;

namespace gregCore.Infrastructure.Social
{
    /// <summary>
    /// Lightweight native Discord Rich Presence implementation using Named Pipes.
    /// Avoids external DLL dependencies and works reliably in IL2CPP.
    /// </summary>
    public static class DiscordService
    {
        private static NamedPipeClientStream? _pipe;
        private static bool _isEnabled = true;
        private static bool _connected;
        private static long _startTime;
        private static string _lastDetails = "";
        private static string _lastState = "";

        public static void Initialize(string clientId = "1218206233285165108")
        {
            if (!_isEnabled) return;
            if (_connected) return;
            
            _startTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            
            new Thread(() => {
                try { Connect(clientId); }
                catch (Exception ex) { MelonLogger.Error($"[Discord] Connection failed: {ex.Message}"); }
            }).Start();
        }

        private static void Connect(string clientId)
        {
            for (int i = 0; i < 10; i++)
            {
                try
                {
                    _pipe = new NamedPipeClientStream(".", $"discord-ipc-{i}", PipeDirection.InOut);
                    _pipe.Connect(500);
                    _connected = true;
                    
                    SendHandshake(clientId);
                    UpdatePresence("Starting Data Center...", "Main Menu");
                    MelonLogger.Msg($"[Discord] Connected to local client (pipe {i}).");
                    return;
                }
                catch { /* try next pipe */ }
            }
        }

        public static void UpdatePresence(string details, string state, string largeImage = "logo_main", string largeText = "Data Center Modded")
        {
            if (!_connected || _pipe == null || !_pipe.IsConnected) return;
            if (details == _lastDetails && state == _lastState) return;

            _lastDetails = details;
            _lastState = state;

            try
            {
                var payload = new
                {
                    cmd = "SET_ACTIVITY",
                    args = new
                    {
                        pid = System.Diagnostics.Process.GetCurrentProcess().Id,
                        activity = new
                        {
                            details = details,
                            state = state,
                            timestamps = new { start = _startTime },
                            assets = new
                            {
                                large_image = largeImage,
                                large_text = largeText,
                                small_image = "greg_icon",
                                small_text = "GregCore Framework"
                            }
                        }
                    },
                    nonce = Guid.NewGuid().ToString()
                };

                SendFrame(1, JsonConvert.SerializeObject(payload));
            }
            catch (Exception ex)
            {
                MelonLogger.Warning($"[Discord] Failed to send update: {ex.Message}");
                _connected = false;
            }
        }

        private static void SendHandshake(string clientId)
        {
            var handshake = new { v = 1, client_id = clientId };
            SendFrame(0, JsonConvert.SerializeObject(handshake));
        }

        private static void SendFrame(int op, string json)
        {
            if (_pipe == null || !_pipe.IsConnected) return;

            byte[] content = Encoding.UTF8.GetBytes(json);
            byte[] header = new byte[8];
            Array.Copy(BitConverter.GetBytes(op), 0, header, 0, 4);
            Array.Copy(BitConverter.GetBytes(content.Length), 0, header, 4, 4);

            _pipe.Write(header, 0, header.Length);
            _pipe.Write(content, 0, content.Length);
            _pipe.Flush();
        }

        public static void Shutdown()
        {
            try
            {
                _connected = false;
                _pipe?.Close();
                _pipe = null;
                MelonLogger.Msg("[Discord] Disconnected.");
            }
            catch { }
        }
    }
}
