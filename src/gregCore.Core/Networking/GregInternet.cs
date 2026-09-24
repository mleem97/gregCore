/// <file-summary>
/// Schicht:      Core (Networking)
/// Zweck:        Internet- und Dienst-Brücke: Internet-Endpunkte lesen/
///               pflegen, CommandCenter (Level, AutoRepair, Upgrades),
///               Angriffs-Manager (Erlaubnis, Start). Alles best-effort.
/// </file-summary>

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using MelonLoader;
using UnityEngine;

namespace gregCore.Core.Networking;

[ExcludeFromCodeCoverage(Justification = "Live Il2Cpp interop against game assemblies; needs running game.")]
public static class GregInternet
{
    // ── DTO ──────────────────────────────────────────────────────────────────

    public sealed class InternetEndpointInfo
    {
        public string ServerID = "";
        public string IP = "";
        public int ServerType;
        public int AppID = -1;
        public float MaxProcessingSpeed;
        public float CurrentProcessingSpeed;
    }

    // ── Internet-Endpunkte ───────────────────────────────────────────────────

    public static List<global::Il2Cpp.Internet> FindAllEndpoints()
    {
        var result = new List<global::Il2Cpp.Internet>();
        Try(() =>
        {
            var all = Resources.FindObjectsOfTypeAll<global::Il2Cpp.Internet>();
            if (all == null) return;
            foreach (var ep in all)
            {
                if (ep == null) continue;
                try
                {
                    var go = ep.gameObject;
                    if (go != null && go.scene.IsValid() && go.scene.isLoaded)
                        result.Add(ep);
                }
                catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
            }
        });
        return result;
    }

    public static InternetEndpointInfo ReadEndpoint(global::Il2Cpp.Internet ep)
    {
        var dto = new InternetEndpointInfo();
        if (ep == null) return dto;
        Try(() => dto.ServerID = ep.ServerID ?? "");
        Try(() => dto.IP = ep.IP ?? "");
        Try(() => dto.ServerType = ep.serverType);
        Try(() => dto.AppID = ep.appID);
        Try(() => dto.MaxProcessingSpeed = ep.maxProcessingSpeed);
        Try(() => dto.CurrentProcessingSpeed = ep.currentProcessingSpeed);
        return dto;
    }

    public static List<InternetEndpointInfo> ReadAllEndpoints()
    {
        var result = new List<InternetEndpointInfo>();
        Try(() =>
        {
            foreach (var ep in FindAllEndpoints())
            {
                try { result.Add(ReadEndpoint(ep)); } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
            }
        });
        return result;
    }

    public static bool UpdateEndpointAppID(global::Il2Cpp.Internet ep, int appID)
    {
        if (ep == null) return false;
        try
        {
            var _ = ep.gameObject; // liveness
            ep.UpdateAppID(appID);
            return true;
        }
        catch (Exception ex)
        {
            Warn($"UpdateAppID fehlgeschlagen: {Base(ex)}");
            return false;
        }
    }

    // ── CommandCenter ────────────────────────────────────────────────────────

    public static global::Il2Cpp.CommandCenter GetCommandCenter()
    {
        try { return global::Il2Cpp.CommandCenter.instance; }
        catch { return null; }
    }

    public static int GetCommandCenterLevel()
    {
        var cc = GetCommandCenter();
        if (cc == null) return -1;
        try { return cc.CurrentLevel; } catch { return -1; }
    }

    public static int GetAutoRepairMode()
    {
        var cc = GetCommandCenter();
        if (cc == null) return -1;
        try { return cc.autoRepairMode; } catch { return -1; }
    }

    public static bool SetAutoRepairMode(int mode)
    {
        var cc = GetCommandCenter();
        if (cc == null) return false;
        try
        {
            var _ = cc.gameObject; // liveness
            cc.SetAutoRepairMode(mode);
            return true;
        }
        catch (Exception ex)
        {
            Warn($"SetAutoRepairMode fehlgeschlagen: {Base(ex)}");
            return false;
        }
    }

    public static bool ToggleAutoClearWarnings(bool isOn)
    {
        var cc = GetCommandCenter();
        if (cc == null) return false;
        try
        {
            var _ = cc.gameObject; // liveness
            cc.ToggleClearWarningAuto(isOn);
            return true;
        }
        catch (Exception ex)
        {
            Warn($"ToggleClearWarningAuto fehlgeschlagen: {Base(ex)}");
            return false;
        }
    }

    public static bool UpgradeCommandCenter()
    {
        var cc = GetCommandCenter();
        if (cc == null) return false;
        try
        {
            var _ = cc.gameObject; // liveness
            cc.ButtonUpgradeCommandCenter();
            return true;
        }
        catch (Exception ex)
        {
            Warn($"ButtonUpgradeCommandCenter fehlgeschlagen: {Base(ex)}");
            return false;
        }
    }

    public static bool DowngradeCommandCenter()
    {
        var cc = GetCommandCenter();
        if (cc == null) return false;
        try
        {
            var _ = cc.gameObject; // liveness
            cc.ButtonDowngradeCommandCenter();
            return true;
        }
        catch (Exception ex)
        {
            Warn($"ButtonDowngradeCommandCenter fehlgeschlagen: {Base(ex)}");
            return false;
        }
    }

    public static int[] GetCostPerLevel()
    {
        var cc = GetCommandCenter();
        if (cc == null) return Array.Empty<int>();
        int[] result = Array.Empty<int>();
        Try(() =>
        {
            var arr = cc.costPerLevel;
            if (arr == null) return;
            int n = 0;
            try { n = arr.Length; } catch { return; }
            var dst = new int[Math.Max(0, n)];
            for (int i = 0; i < dst.Length; i++)
            {
                try { dst[i] = arr[i]; } catch { dst[i] = 0; }
            }
            result = dst;
        });
        return result;
    }

    // ── Angriffs-Manager ─────────────────────────────────────────────────────

    public static global::Il2Cpp.MaliciousAttackManager GetAttackManager()
    {
        try { return global::Il2Cpp.MaliciousAttackManager.instance; }
        catch { return null; }
    }

    public static bool IsMaliciousAllowed(global::Il2Cpp.Firewall firewall)
    {
        var mgr = GetAttackManager();
        if (mgr == null || firewall == null) return false;
        try
        {
            var _ = mgr.gameObject; // liveness
            return mgr.MaliciousAllowed(firewall);
        }
        catch { return false; }
    }

    public static bool LaunchAttack()
    {
        var mgr = GetAttackManager();
        if (mgr == null) return false;
        try
        {
            var _ = mgr.gameObject; // liveness
            mgr.LaunchAttack();
            return true;
        }
        catch (Exception ex)
        {
            Warn($"LaunchAttack fehlgeschlagen: {Base(ex)}");
            return false;
        }
    }

    private static string Base(Exception ex)
    {
        try { return ex != null ? ex.GetBaseException().Message : "?"; } catch { return "?"; }
    }

    private static void Warn(string message)
    {
        try { MelonLogger.Warning($"[gregCore][Net] Internet: {message}"); } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
    }

    private static void Try(Action action)
    {
        try { action?.Invoke(); }
        catch (Exception ex)
        {
            try { MelonLogger.Warning($"[gregCore][Net] Internet-Feld fehlgeschlagen: {ex.GetBaseException().Message}"); } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
        }
    }
}
