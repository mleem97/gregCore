/// <file-summary>
/// Layer:       Bridge (IPC)
/// Purpose:     Universal out-of-process adapter: anything (MCP server, scripts,
///               tools, agents) talks to the running game over localhost TCP/NDJSON.
/// Maintainer:  Wraps existing GregAPI surfaces only (mods, hooks, toast, reload,
///               logs). No new game logic. Every handler is try-caught with safe
///               defaults; everything runs on the main thread via the dispatcher.
///               Binds 127.0.0.1 only. Disabled unless Start() is called.
/// Protocol:    one JSON object per line: {"id":1,"method":"ping","params":{...}}
///              -> {"ok":true,"id":1,"result":...} | {"ok":false,"id":1,"error":"..."}
/// </file-summary>

using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using gregCore.PublicApi;

namespace gregCore.Bridge.Ipc
{
    /// <summary>
    /// Universal localhost bridge. Start once (e.g. from GregCoreMod when
    /// GREG_IPC_PORT is set); Stop on shutdown. Thread-safe.
    /// </summary>
    public static class GregIpcBridge
    {
        public const int DefaultPort = 27960;

        private static readonly object _gate = new();
        private static TcpListener? _listener;
        private static Thread? _acceptThread;
        private static IGregMainThreadDispatcher? _dispatcher;
        private static string? _token;
        private static volatile bool _running;

        public static bool IsRunning => _running;
        public static int ActualPort { get; private set; }

        public static bool Start(IGregMainThreadDispatcher dispatcher, int port = DefaultPort, string? token = null)
        {
            lock (_gate)
            {
                if (_running) return true;
                try
                {
                    _dispatcher = dispatcher ?? throw new ArgumentNullException(nameof(dispatcher));
                    _listener = new TcpListener(IPAddress.Loopback, port);
                    _listener.Start();
                    ActualPort = ((IPEndPoint)_listener.LocalEndpoint).Port;
                    _token = string.IsNullOrEmpty(token) ? null : token;
                    _running = true;
                    _acceptThread = new Thread(AcceptLoop) { IsBackground = true, Name = "GregIpcBridge" };
                    _acceptThread.Start();
                    return true;
                }
                catch (Exception ex)
                {
                    try { MelonLoader.MelonLogger.Warning($"[GregIpc] Start failed on port {port}: {ex.GetBaseException().Message}"); } catch { }
                    _running = false;
                    _listener = null;
                    return false;
                }
            }
        }

        public static void Stop()
        {
            lock (_gate)
            {
                _running = false;
                try { _listener?.Stop(); } catch { }
                _listener = null;
                _dispatcher = null;
            }
        }

        private static void AcceptLoop()
        {
            while (_running)
            {
                try
                {
                    var client = _listener!.AcceptTcpClient();
                    var t = new Thread(() => Serve(client)) { IsBackground = true };
                    t.Start();
                }
                catch { Thread.Sleep(50); }
            }
        }

        private static void Serve(TcpClient client)
        {
            using (client)
            try
            {
                using var stream = client.GetStream();
                using var reader = new StreamReader(stream, Encoding.UTF8);
                using var writer = new StreamWriter(stream, Encoding.UTF8) { AutoFlush = true, NewLine = "\n" };
                string? line;
                while (_running && (line = reader.ReadLine()) != null)
                {
                    if (string.IsNullOrWhiteSpace(line)) continue;
                    writer.WriteLine(DispatchOnMainThread(line));
                }
            }
            catch { }
        }

        private static string DispatchOnMainThread(string line)
        {
            string result = "{\"ok\":false,\"error\":\"dispatcher unavailable\"}";
            var done = new ManualResetEventSlim(false);
            try
            {
                var d = _dispatcher;
                if (d == null) return result;
                d.Enqueue(() =>
                {
                    try { result = Handle(line); }
                    catch (Exception ex) { result = "{\"ok\":false,\"error\":" + Quote("handler: " + ex.GetBaseException().Message) + "}"; }
                    finally { done.Set(); }
                });
            }
            catch { return result; }
            if (!done.Wait(TimeSpan.FromSeconds(5)))
                return "{\"ok\":false,\"error\":\"main-thread timeout\"}";
            return result;
        }

        internal static string Handle(string line)
        {
            string? id = ExtractRaw(line, "id");
            string? method = ExtractString(line, "method");
            if (string.IsNullOrEmpty(method))
                return "{\"ok\":false,\"error\":\"missing method\"}";
            if (_token != null)
            {
                string? tok = ExtractString(line, "token");
                if (tok != _token)
                    return Fail(id, "bad token");
            }
            try
            {
                switch (method)
                {
                    case "ping":
                        return Ok(id, "\"pong\"");
                    case "api.version":
                        return Ok(id, Quote(typeof(GregIpcBridge).Assembly.GetName().Version?.ToString() ?? "0.0.0"));
                    case "mods.list":
                        return Ok(id, ModsJson());
                    case "hook.fire":
                        {
                            string? hook = ExtractString(line, "hook");
                            if (string.IsNullOrEmpty(hook)) return Fail(id, "missing params.hook");
                            API.GregAPI.FireEvent(hook);
                            return Ok(id, "true");
                        }
                    case "ui.toast":
                        {
                            string text = ExtractString(line, "text") ?? "";
                            float dur = ExtractFloat(line, "duration", 3f);
                            API.GregAPI.ShowNotification(text, dur);
                            return Ok(id, "true");
                        }
                    case "ui.notify":
                        {
                            string text = ExtractString(line, "text") ?? "";
                            float dur = ExtractFloat(line, "duration", 3f);
                            API.GregAPI.ShowNotification(text, dur);
                            return Ok(id, "true");
                        }
                    case "scripts.reload":
                        {
                            bool ok = gregCore.Bridge.CSharpScript.GregCSharpScriptBridge.TryReloadNow();
                            return Ok(id, ok ? "true" : "false");
                        }
                    case "log.tail":
                        {
                            int n = ExtractInt(line, "n", 50);
                            return Ok(id, LogJson(Math.Max(1, Math.Min(n, 200))));
                        }
                    default:
                        return Fail(id, "unknown method: " + method);
                }
            }
            catch (Exception ex)
            {
                return Fail(id, ex.GetBaseException().Message);
            }
        }

        private static string Ok(string? id, string resultJson)
            => "{\"ok\":true,\"id\":" + (id ?? "null") + ",\"result\":" + resultJson + "}";

        private static string Fail(string? id, string error)
            => "{\"ok\":false,\"id\":" + (id ?? "null") + ",\"error\":" + Quote(error) + "}";

        private static string ModsJson()
        {
            var sb = new StringBuilder("[");
            bool first = true;
            try
            {
                foreach (var m in gregCore.Core.Mods.GregModRegistry.All())
                {
                    if (!first) sb.Append(',');
                    first = false;
                    sb.Append("{\"id\":").Append(Quote(m.Id ?? ""))
                      .Append(",\"name\":").Append(Quote(m.Name ?? ""))
                      .Append(",\"version\":").Append(Quote(m.Version ?? "")).Append('}');
                }
            }
            catch { }
            sb.Append(']');
            return sb.ToString();
        }

        private static string LogJson(int n)
        {
            List<string> lines = new();
            try
            {
                var inst = gregCore.Infrastructure.UI.GregDevConsole.Instance;
                if (inst != null) lines = inst.Tail(n);
            }
            catch { }
            var sb = new StringBuilder("[");
            for (int i = 0; i < lines.Count; i++)
            {
                if (i > 0) sb.Append(',');
                sb.Append(Quote(lines[i] ?? ""));
            }
            sb.Append(']');
            return sb.ToString();
        }

        // ---------- minimal JSON helpers (no reflection: IL2CPP-safe) ----------

        internal static string Quote(string s)
        {
            var sb = new StringBuilder("\"");
            foreach (char c in s)
            {
                switch (c)
                {
                    case '"': sb.Append("\\\""); break;
                    case '\\': sb.Append("\\\\"); break;
                    case '\n': sb.Append("\\n"); break;
                    case '\r': sb.Append("\\r"); break;
                    case '\t': sb.Append("\\t"); break;
                    default:
                        if (c < 0x20) sb.Append("\\u").Append(((int)c).ToString("x4"));
                        else sb.Append(c);
                        break;
                }
            }
            sb.Append('"');
            return sb.ToString();
        }

        internal static string? ExtractString(string json, string key)
        {
            string? raw = ExtractRaw(json, key);
            if (raw == null || raw.Length < 2 || raw[0] != '"') return null;
            var sb = new StringBuilder();
            for (int i = 1; i < raw.Length; i++)
            {
                char c = raw[i];
                if (c == '\\' && i + 1 < raw.Length)
                {
                    char n = raw[++i];
                    sb.Append(n switch { '"' => '"', '\\' => '\\', 'n' => '\n', 'r' => '\r', 't' => '\t', _ => n });
                }
                else if (c == '"') break;
                else sb.Append(c);
            }
            return sb.ToString();
        }

        internal static string? ExtractRaw(string json, string key)
        {
            string pat = "\"" + key + "\"";
            int i = json.IndexOf(pat, StringComparison.Ordinal);
            if (i < 0) return null;
            i += pat.Length;
            while (i < json.Length && char.IsWhiteSpace(json[i])) i++;
            if (i >= json.Length || json[i] != ':') return null;
            i++;
            while (i < json.Length && char.IsWhiteSpace(json[i])) i++;
            if (i >= json.Length) return null;
            if (json[i] == '"')
            {
                int j = i + 1;
                while (j < json.Length)
                {
                    if (json[j] == '\\') { j += 2; continue; }
                    if (json[j] == '"') return json.Substring(i, j - i + 1);
                    j++;
                }
                return null;
            }
            int k = i;
            while (k < json.Length && ",}]".IndexOf(json[k]) < 0) k++;
            return json.Substring(i, k - i).Trim();
        }

        internal static int ExtractInt(string json, string key, int fallback)
        {
            string? raw = ExtractRaw(json, key);
            return int.TryParse(raw, out int v) ? v : fallback;
        }

        internal static float ExtractFloat(string json, string key, float fallback)
        {
            string? raw = ExtractRaw(json, key);
            return float.TryParse(raw, System.Globalization.NumberStyles.Float,
                System.Globalization.CultureInfo.InvariantCulture, out float v) ? v : fallback;
        }
    }
}
