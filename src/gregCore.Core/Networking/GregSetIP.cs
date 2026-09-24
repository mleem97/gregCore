/// <file-summary>
/// Layer:       Core (Networking)
/// Purpose:     SetIP bridge (vanilla IP keypad): find instance, subnet
///               math (mask, usable IPs, first usable IP) via vanilla
///               with managed fallback, plus Show/Cancel passthroughs.
///               All best-effort.
/// </file-summary>

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using MelonLoader;
using UnityEngine;

namespace gregCore.Core.Networking;

[ExcludeFromCodeCoverage(Justification = "Live Il2Cpp interop against game assemblies; needs running game.")]
public static class GregSetIP
{
    // Find the live keypad (prefer active, in loaded scene).
    public static global::Il2Cpp.SetIP FindInstance()
    {
        global::Il2Cpp.SetIP found = null;
        Try(() =>
        {
            var all = Resources.FindObjectsOfTypeAll<global::Il2Cpp.SetIP>();
            if (all == null) return;
            foreach (var s in all)
            {
                if (s == null) continue;
                try
                {
                    var go = s.gameObject;
                    if (go == null || !go.scene.IsValid() || !go.scene.isLoaded) continue;
                    if (found == null) found = s;
                    try
                    {
                        if (s.isActive) { found = s; break; }
                    }
                    catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
                }
                catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
            }
        });
        return found;
    }

    public static bool IsActive()
    {
        var inst = FindInstance();
        if (inst == null) return false;
        try { return inst.isActive; } catch { return false; }
    }

    public static string GetCurrentIp()
    {
        var inst = FindInstance();
        if (inst == null) return "";
        try { return inst.ipAddress ?? ""; } catch { return ""; }
    }

    // "255.255.255.0" from 24 — vanilla preferred, managed fallback.
    public static string GetMaskFromCidr(int cidr)
    {
        var inst = FindInstance();
        if (inst != null)
        {
            try
            {
                var _ = inst.gameObject; // liveness
                string mask = inst.GetMaskFromCidr(cidr);
                if (!string.IsNullOrEmpty(mask)) return mask;
            }
            catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
        }
        return MaskFromCidrManaged(cidr);
    }

    public static List<string> GetUsableIPs(string subnet)
    {
        var result = new List<string>();
        if (string.IsNullOrWhiteSpace(subnet)) return result;
        var inst = FindInstance();
        if (inst == null) return result;
        Try(() =>
        {
            var _ = inst.gameObject; // liveness
            var arr = inst.GetUsableIPsFromSubnet(subnet);
            if (arr == null) return;
            int n = 0;
            try { n = arr.Length; } catch { return; }
            for (int i = 0; i < n; i++)
            {
                try
                {
                    string ip = arr[i];
                    if (!string.IsNullOrEmpty(ip)) result.Add(ip);
                }
                catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
            }
        });
        return result;
    }

    public static string GetFirstUsableIP(string subnet)
    {
        if (string.IsNullOrWhiteSpace(subnet)) return "";
        var inst = FindInstance();
        if (inst == null) return "";
        try
        {
            var _ = inst.gameObject; // liveness
            return inst.GetFirstUsableIPFromSubnet(subnet) ?? "";
        }
        catch { return ""; }
    }

    // Open the keypad for a server (vanilla flow).
    public static bool ShowFor(global::Il2Cpp.Server server)
    {
        var inst = FindInstance();
        if (inst == null || server == null) return false;
        try
        {
            var _ = inst.gameObject; // liveness
            inst.ShowCanvas(server);
            return true;
        }
        catch (Exception ex)
        {
            Warn($"ShowCanvas failed: {Base(ex)}");
            return false;
        }
    }

    public static bool Cancel()
    {
        var inst = FindInstance();
        if (inst == null) return false;
        try
        {
            var _ = inst.gameObject; // liveness
            inst.ClickButtonCancel();
            return true;
        }
        catch (Exception ex)
        {
            Warn($"Cancel failed: {Base(ex)}");
            return false;
        }
    }

    // Managed fallback (deterministic, without game instance).
    public static string MaskFromCidrManaged(int cidr)
    {
        if (cidr < 0 || cidr > 32) return "";
        try
        {
            uint mask = cidr == 0 ? 0u : 0xFFFFFFFFu << (32 - cidr);
            return string.Format("{0}.{1}.{2}.{3}",
                (mask >> 24) & 0xFF, (mask >> 16) & 0xFF, (mask >> 8) & 0xFF, mask & 0xFF);
        }
        catch { return ""; }
    }

    private static string Base(Exception ex)
    {
        try { return ex != null ? ex.GetBaseException().Message : "?"; } catch { return "?"; }
    }

    private static void Warn(string message)
    {
        try { MelonLogger.Warning($"[gregCore][Net] SetIP: {message}"); } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
    }

    private static void Try(Action action)
    {
        try { action?.Invoke(); }
        catch (Exception ex)
        {
            try { MelonLogger.Warning($"[gregCore][Net] SetIP field failed: {ex.GetBaseException().Message}"); } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
        }
    }
}
