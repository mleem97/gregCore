using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MelonLoader;
using UnityEngine;
using System.Text.Json;

namespace greg.Sdk.Services.MCP;

/// <summary>
/// Model Context Protocol (MCP) Server for Development Telemetry.
/// Allows LLMs and IDEs to query live game state via HTTP SSE (Server-Sent Events) and REST endpoints.
/// Default port: 10420
/// </summary>
public static class GregMCPServer
{
    private static HttpListener _listener;
    private static Thread _serverThread;
    private static bool _isRunning;
    public static int Port { get; set; } = 10420;

    public static void Start()
    {
        if (_isRunning) return;

        try
        {
            _listener = new HttpListener();
            _listener.Prefixes.Add($"http://localhost:{Port}/mcp/");
            _listener.Start();

            _isRunning = true;
            _serverThread = new Thread(ListenLoop) { IsBackground = true };
            _serverThread.Start();
            
            MelonLogger.Msg($"[MCP Server] Listening on http://localhost:{Port}/mcp/");
        }
        catch (Exception ex)
        {
            MelonLogger.Error($"[MCP Server] Failed to start: {ex.Message}");
        }
    }

    public static void Stop()
    {
        if (!_isRunning) return;
        _isRunning = false;
        
        try
        {
            if (_listener != null && _listener.IsListening)
            {
                _listener.Stop();
                _listener.Close();
            }
        }
        catch { }
    }

    private static void ListenLoop()
    {
        while (_isRunning)
        {
            try
            {
                var context = _listener.GetContext();
                Task.Run(() => HandleRequest(context));
            }
            catch (HttpListenerException) { break; }
            catch (InvalidOperationException) { break; }
            catch (Exception ex)
            {
                if (_isRunning) MelonLogger.Error($"[MCP Server] Loop error: {ex.Message}");
            }
        }
    }

    private static async Task HandleRequest(HttpListenerContext context)
    {
        var request = context.Request;
        var response = context.Response;

        // CORS generic headers
        response.AppendHeader("Access-Control-Allow-Origin", "*");
        response.AppendHeader("Access-Control-Allow-Methods", "GET, POST, OPTIONS");
        
        if (request.HttpMethod == "OPTIONS")
        {
            response.StatusCode = 200;
            response.Close();
            return;
        }

        try
        {
            if (request.Url.AbsolutePath.EndsWith("/mcp/status"))
            {
                var stats = new
                {
                    status = "online",
                    framework = "gregCore",
                    uptime = Time.time,
                    multiplayerConnected = Multiplayer.GregMultiplayerService.IsConnected
                };
                await SendJson(response, stats);
            }
            else if (request.Url.AbsolutePath.EndsWith("/mcp/tools"))
            {
                // Return schema of available tools for LLM standard calling
                var tools = new
                {
                    tools = new[]
                    {
                        new { name = "get_system_stats", description = "Fetch game uptime and core loaded status." },
                        new { name = "dispatch_greg", description = "Mock dispatch a tech to a broken switch." },
                    }
                };
                await SendJson(response, tools);
            }
            else
            {
                response.StatusCode = 404;
                await SendJson(response, new { error = "Endpoint not found" });
            }
        }
        catch (Exception ex)
        {
            response.StatusCode = 500;
            await SendJson(response, new { error = ex.Message });
        }
    }

    private static async Task SendJson(HttpListenerResponse response, object payload)
    {
        var json = JsonSerializer.Serialize(payload, new JsonSerializerOptions { WriteIndented = true, PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
        var buffer = Encoding.UTF8.GetBytes(json);
        
        response.ContentType = "application/json";
        response.ContentLength64 = buffer.Length;
        await response.OutputStream.WriteAsync(buffer, 0, buffer.Length);
        response.Close();
    }
}
