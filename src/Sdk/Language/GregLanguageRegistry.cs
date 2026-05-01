using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MelonLoader;
using gregCore.Sdk.Language.Hosts;

namespace gregCore.Sdk.Language;

/// <summary>
/// Central registry for language hosts. Built-in hosts are pre-registered;
/// additional hosts can be registered at runtime via gregExt assemblies.
/// </summary>
public static class GregLanguageRegistry
{
    private static readonly Dictionary<string, IGregLanguageHost> ActiveHosts = new();
    private static readonly Dictionary<string, IGregLanguageHost> AvailableHosts = new();
    private static bool _scanCompleted;

    static GregLanguageRegistry()
    {
        // Register built-in hosts
        RegisterHostInternal(new GregLuaHost());
        RegisterHostInternal(new GregRustHost());
        RegisterHostInternal(new GregPythonHost());
        RegisterHostInternal(new GregJsHost());
        RegisterHostInternal(new GregCSharpScriptHost());
    }

    /// <summary>
    /// Registers a language host. Can be called by gregExt assemblies during discovery.
    /// </summary>
    public static void RegisterHost(string hostId, IGregLanguageHost host)
    {
        if (string.IsNullOrWhiteSpace(hostId))
            throw new ArgumentException("HostId must not be empty.", nameof(hostId));

        AvailableHosts[hostId.ToLowerInvariant()] = host;
    }

    private static void RegisterHostInternal(IGregLanguageHost host)
    {
        AvailableHosts[host.HostId.ToLowerInvariant()] = host;
    }

    public static bool IsActive(string hostId)
    {
        return ActiveHosts.ContainsKey(hostId.ToLowerInvariant());
    }

    public static IGregLanguageHost GetHost(string hostId)
    {
        if (!ActiveHosts.TryGetValue(hostId.ToLowerInvariant(), out var host))
        {
            throw new KeyNotFoundException($"Language host is not active: {hostId}");
        }
        return host;
    }

    public static void ScanAndActivate(string modsScriptsDir)
    {
        if (_scanCompleted)
        {
            MelonLogger.Msg("[gregCore] Language scan already completed. Skipping repeated activation.");
            return;
        }

        Directory.CreateDirectory(modsScriptsDir);

        MelonLogger.Msg("[gregCore] ── Language Host Activation ─────────────────");

        foreach (var kvp in AvailableHosts)
        {
            var host = kvp.Value;
            var extPatterns = host.FileExtensions.Select(ext => $"*{ext}").ToArray();
            int scriptCount = extPatterns.Sum(p => CountFiles(modsScriptsDir, p));

            if (scriptCount == 0)
            {
                MelonLogger.Msg($"[gregCore]   [{host.HostId}] {("SKIPPED").PadRight(10)} — no matching scripts found");
                continue;
            }

            if (!host.IsDependencyAvailable(out var detail))
            {
                MelonLogger.Warning($"[gregCore] {host.HostName} dependency missing: {detail}");
                MelonLogger.Msg($"[gregCore]   [{host.HostId}] {("SKIPPED").PadRight(10)} — dependency missing");
                continue;
            }

            try
            {
                host.Activate(modsScriptsDir);
                ActiveHosts[host.HostId.ToLowerInvariant()] = host;
                MelonLogger.Msg($"[gregCore]   [{host.HostId}] {("ACTIVE").PadRight(10)} — {host.HostName} ({scriptCount} scripts)");
            }
            catch (Exception ex)
            {
                MelonLogger.Error($"[gregCore] Host activation failed for {host.HostId}: {ex.Message}");
            }
        }

        MelonLogger.Msg("[gregCore] ────────────────────────────────────────────");
        MelonLogger.Msg($"[gregCore]   Active hosts: {ActiveHosts.Count} / {AvailableHosts.Count}");
        MelonLogger.Msg("[gregCore] ═══════════════════════════════════════════════════════════");
        MelonLogger.Msg("[gregCore]   Language host initialization complete.");
        MelonLogger.Msg("[gregCore] ═══════════════════════════════════════════════════════════");

        _scanCompleted = true;
    }

    public static void OnUpdate(float dt)
    {
        foreach (var host in ActiveHosts.Values)
        {
            try
            {
                host.OnUpdate(dt);
            }
            catch (Exception ex)
            {
                MelonLogger.Error($"[gregCore] Host update failed ({host.HostId}): {ex.Message}");
            }
        }
    }

    public static void OnSceneLoaded(string sceneName)
    {
        foreach (var host in ActiveHosts.Values)
        {
            try
            {
                host.OnSceneLoaded(sceneName);
            }
            catch (Exception ex)
            {
                MelonLogger.Error($"[gregCore] Host scene callback failed ({host.HostId}): {ex.Message}");
            }
        }
    }

    public static void Shutdown()
    {
        foreach (var host in ActiveHosts.Values.ToArray())
        {
            try
            {
                host.Shutdown();
            }
            catch (Exception ex)
            {
                MelonLogger.Error($"[gregCore] Host shutdown failed ({host.HostId}): {ex.Message}");
            }
        }

        ActiveHosts.Clear();
        _scanCompleted = false;
    }

    private static int CountFiles(string root, string pattern)
    {
        try
        {
            return Directory.EnumerateFiles(root, pattern, SearchOption.AllDirectories).Count();
        }
        catch
        {
            return 0;
        }
    }
}
