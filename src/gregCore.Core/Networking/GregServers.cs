/// <file-summary>
/// Schicht:      Core (Networking)
/// Zweck:        Server-Brücke: Save-DTO (ServerSaveData) plus Erzeugen/
///               Fuellen/Lesen/Upsert in der spieleigenen Liste
///               (NetworkSaveData.servers, Key serverID) sowie sichere
///               Runtime-Helfer (Find, Capture, InsertIntoRack, SetIP, Power,
///               Customer/App, Repair, Validate). Alles best-effort.
/// </file-summary>

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using MelonLoader;
using UnityEngine;

namespace gregCore.Core.Networking;

[ExcludeFromCodeCoverage(Justification = "Live Il2Cpp interop against game assemblies; needs running game.")]
public static class GregServers
{
    // ── Verwaltetes DTO ──────────────────────────────────────────────────────

    public sealed class ServerSave
    {
        public string ServerID = "";
        public int CustomerID;
        public string Ip = "";
        public int ServerType;
        public Vector3 Position;
        public Quaternion Rotation;
        public int RackPositionUID = -1;
        public int PrefabID;
        public bool IsOn = true;
        public bool IsBroken;
        public int TimeToBrake;
        public int EolTime;
        public bool IsWarningCleared;
        public string Label = "";
    }

    // ── Erzeugen / Fuellen / Lesen ───────────────────────────────────────────

    public static global::Il2Cpp.ServerSaveData Create(ServerSave dto)
    {
        global::Il2Cpp.ServerSaveData entry = null;
        try { entry = new global::Il2Cpp.ServerSaveData(); } catch { return null; }
        Fill(entry, dto);
        return entry;
    }

    public static void Fill(global::Il2Cpp.ServerSaveData entry, ServerSave dto)
    {
        if (entry == null || dto == null) return;
        Try(() => entry.serverID = dto.ServerID ?? "");
        Try(() => entry.customerID = dto.CustomerID);
        Try(() => entry.ip = dto.Ip ?? "");
        Try(() => entry.serverType = dto.ServerType);
        Try(() => entry.position = dto.Position);
        Try(() => entry.rotation = dto.Rotation);
        Try(() => entry.rackPositionUID = dto.RackPositionUID);
        Try(() => entry.prefabID = dto.PrefabID);
        Try(() => entry.isOn = dto.IsOn);
        Try(() => entry.isBroken = dto.IsBroken);
        Try(() => entry.timeToBrake = dto.TimeToBrake);
        Try(() => entry.eolTime = dto.EolTime);
        Try(() => entry.isWarningCleared = dto.IsWarningCleared);
        Try(() => entry.label = dto.Label ?? "");
    }

    public static ServerSave Read(global::Il2Cpp.ServerSaveData entry)
    {
        var dto = new ServerSave();
        if (entry == null) return dto;
        Try(() => dto.ServerID = entry.serverID ?? "");
        Try(() => dto.CustomerID = entry.customerID);
        Try(() => dto.Ip = entry.ip ?? "");
        Try(() => dto.ServerType = entry.serverType);
        Try(() => dto.Position = entry.position);
        Try(() => dto.Rotation = entry.rotation);
        Try(() => dto.RackPositionUID = entry.rackPositionUID);
        Try(() => dto.PrefabID = entry.prefabID);
        Try(() => dto.IsOn = entry.isOn);
        Try(() => dto.IsBroken = entry.isBroken);
        Try(() => dto.TimeToBrake = entry.timeToBrake);
        Try(() => dto.EolTime = entry.eolTime);
        Try(() => dto.IsWarningCleared = entry.isWarningCleared);
        Try(() => dto.Label = entry.label ?? "");
        return dto;
    }

    public static List<ServerSave> ReadAll(Il2CppSystem.Collections.Generic.List<global::Il2Cpp.ServerSaveData> list)
    {
        var result = new List<ServerSave>();
        if (list == null) return result;
        Try(() =>
        {
            foreach (var entry in list)
            {
                try { result.Add(Read(entry)); } catch { }
            }
        });
        return result;
    }

    // ── Upsert / Remove (Key: serverID; leere ID -> immer Anhang) ────────────

    public static global::Il2Cpp.ServerSaveData Upsert(
        Il2CppSystem.Collections.Generic.List<global::Il2Cpp.ServerSaveData> list, ServerSave dto)
    {
        if (list == null || dto == null) return null;
        string key = dto.ServerID ?? "";
        global::Il2Cpp.ServerSaveData found = null;
        if (!string.IsNullOrEmpty(key))
        {
            Try(() =>
            {
                foreach (var entry in list)
                {
                    if (entry == null) continue;
                    string id = null;
                    try { id = entry.serverID; } catch { continue; }
                    if (string.Equals(id, key, StringComparison.OrdinalIgnoreCase))
                    {
                        found = entry;
                        break;
                    }
                }
            });
        }
        if (found != null)
        {
            Fill(found, dto);
            return found;
        }
        var created = Create(dto);
        if (created != null)
        {
            try { list.Add(created); } catch { return null; }
        }
        return created;
    }

    public static bool Remove(
        Il2CppSystem.Collections.Generic.List<global::Il2Cpp.ServerSaveData> list, string serverID)
    {
        if (list == null || string.IsNullOrEmpty(serverID)) return false;
        bool removed = false;
        Try(() =>
        {
            for (int i = list.Count - 1; i >= 0; i--)
            {
                global::Il2Cpp.ServerSaveData entry = null;
                try { entry = list[i]; } catch { continue; }
                if (entry == null) continue;
                string id = null;
                try { id = entry.serverID; } catch { continue; }
                if (string.Equals(id, serverID, StringComparison.OrdinalIgnoreCase))
                {
                    try { list.RemoveAt(i); removed = true; } catch { }
                }
            }
        });
        return removed;
    }

    // ── Runtime-Brücke (live Server) ─────────────────────────────────────────

    public static List<global::Il2Cpp.Server> FindAll()
    {
        var result = new List<global::Il2Cpp.Server>();
        Try(() =>
        {
            var all = Resources.FindObjectsOfTypeAll<global::Il2Cpp.Server>();
            if (all == null) return;
            foreach (var s in all)
            {
                if (s == null) continue;
                try
                {
                    var go = s.gameObject;
                    if (go != null && go.scene.IsValid() && go.scene.isLoaded)
                        result.Add(s);
                }
                catch { }
            }
        });
        return result;
    }

    public static global::Il2Cpp.Server FindById(string serverID)
    {
        if (string.IsNullOrEmpty(serverID)) return null;
        global::Il2Cpp.Server found = null;
        Try(() =>
        {
            foreach (var s in FindAll())
            {
                string id = null;
                try { id = s.ServerID; } catch { continue; }
                if (string.Equals(id, serverID, StringComparison.OrdinalIgnoreCase))
                {
                    found = s;
                    break;
                }
            }
        });
        return found;
    }

    public static global::Il2Cpp.Server FindByIp(string ip)
    {
        if (string.IsNullOrEmpty(ip)) return null;
        global::Il2Cpp.Server found = null;
        Try(() =>
        {
            foreach (var s in FindAll())
            {
                string cur = null;
                try { cur = s.IP; } catch { continue; }
                if (string.Equals(cur, ip, StringComparison.OrdinalIgnoreCase))
                {
                    found = s;
                    break;
                }
            }
        });
        return found;
    }

    // Live-Server -> DTO (RackPositionUID runtime-seitig nicht lesbar -> -1).
    public static ServerSave Capture(global::Il2Cpp.Server server)
    {
        var dto = new ServerSave();
        if (server == null) return dto;
        Try(() => dto.ServerID = server.ServerID ?? "");
        Try(() => dto.Ip = server.IP ?? "");
        Try(() => dto.ServerType = server.serverType);
        Try(() => dto.IsOn = server.isOn);
        Try(() => dto.IsBroken = server.isBroken);
        Try(() => dto.EolTime = server.eolTime);
        Try(() => dto.Label = server.lastDisplayedLabel ?? "");
        Try(() => dto.Position = server.transform.position);
        Try(() => dto.Rotation = server.transform.rotation);
        Try(() => dto.CustomerID = server.GetCustomerID());
        return dto;
    }

    // Vanilla-Restore-Pfad: Server per Save-Eintrag ins Rack einsetzen.
    public static bool InsertIntoRack(global::Il2Cpp.Server server, global::Il2Cpp.ServerSaveData entry)
    {
        if (server == null || entry == null) return false;
        try
        {
            var _ = server.gameObject; // liveness
            server.ServerInsertedInRack(entry);
            return true;
        }
        catch (Exception ex)
        {
            Warn($"ServerInsertedInRack fehlgeschlagen: {Base(ex)}");
            return false;
        }
    }

    public static bool InsertIntoRack(global::Il2Cpp.Server server, ServerSave dto)
    {
        if (server == null || dto == null) return false;
        var entry = Create(dto);
        if (entry == null) return false;
        return InsertIntoRack(server, entry);
    }

    // Sichere Aktionen (Vanilla-Pfade, bool statt Exception).
    public static bool SetIP(global::Il2Cpp.Server server, string ip)
    {
        if (server == null || string.IsNullOrWhiteSpace(ip)) return false;
        try
        {
            var _ = server.gameObject; // liveness
            server.SetIP(ip);
            return true;
        }
        catch (Exception ex)
        {
            Warn($"SetIP fehlgeschlagen: {Base(ex)}");
            return false;
        }
    }

    public static bool SetPower(global::Il2Cpp.Server server, bool forceState)
    {
        if (server == null) return false;
        try
        {
            var _ = server.gameObject; // liveness
            server.PowerButton(forceState);
            return true;
        }
        catch (Exception ex)
        {
            Warn($"PowerButton fehlgeschlagen: {Base(ex)}");
            return false;
        }
    }

    public static bool UpdateCustomer(global::Il2Cpp.Server server, int customerID)
    {
        if (server == null) return false;
        try
        {
            var _ = server.gameObject; // liveness
            server.UpdateCustomer(customerID);
            return true;
        }
        catch (Exception ex)
        {
            Warn($"UpdateCustomer fehlgeschlagen: {Base(ex)}");
            return false;
        }
    }

    public static bool UpdateAppID(global::Il2Cpp.Server server, int appID)
    {
        if (server == null) return false;
        try
        {
            var _ = server.gameObject; // liveness
            server.UpdateAppID(appID);
            return true;
        }
        catch (Exception ex)
        {
            Warn($"UpdateAppID fehlgeschlagen: {Base(ex)}");
            return false;
        }
    }

    public static bool Repair(global::Il2Cpp.Server server)
    {
        if (server == null) return false;
        try
        {
            var _ = server.gameObject; // liveness
            server.RepairDevice();
            return true;
        }
        catch (Exception ex)
        {
            Warn($"RepairDevice fehlgeschlagen: {Base(ex)}");
            return false;
        }
    }

    public static bool ClearWarning(global::Il2Cpp.Server server, bool preserved)
    {
        if (server == null) return false;
        try
        {
            var _ = server.gameObject; // liveness
            server.ClearWarningSign(preserved);
            return true;
        }
        catch (Exception ex)
        {
            Warn($"ClearWarningSign fehlgeschlagen: {Base(ex)}");
            return false;
        }
    }

    public static bool IsAnyCableConnected(global::Il2Cpp.Server server)
    {
        if (server == null) return false;
        try { return server.IsAnyCableConnected(); } catch { return false; }
    }

    public static bool ValidateRackPosition(global::Il2Cpp.Server server)
    {
        if (server == null) return false;
        try { return server.ValidateRackPosition(); } catch { return false; }
    }

    private static string Base(Exception ex)
    {
        try { return ex != null ? ex.GetBaseException().Message : "?"; } catch { return "?"; }
    }

    private static void Warn(string message)
    {
        try { MelonLogger.Warning($"[gregCore][Net] Servers: {message}"); } catch { }
    }

    private static void Try(Action action)
    {
        try { action?.Invoke(); }
        catch (Exception ex)
        {
            try { MelonLogger.Warning($"[gregCore][Net] Servers-Feld fehlgeschlagen: {ex.GetBaseException().Message}"); } catch { }
        }
    }
}
