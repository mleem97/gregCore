using System;
using System.Collections.Generic;
using Il2Cpp;
using Il2CppInterop.Runtime.InteropTypes;
using UnityEngine;
using gregCore.API;

namespace DataCenterModLoader;

internal static class SpawnedObjectTracker
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "S2386:Mutable fields should not be declared public", Justification = "Harmony cross-patch coordination flag: written and read by cooperating patch classes in the same assembly (see usages). Must stay mutable.")]
    internal static bool SuppressEvents = false;

    private static readonly HashSet<int> _knownInstances = new();
    private static readonly HashSet<string> _knownIds = new();

    internal static void DetectNewObjects()
    {
        if (SuppressEvents) return;
        try
        {
            foreach (var uo in UnityEngine.Object.FindObjectsOfType<UsableObject>())
            {
                int instId = uo.GetInstanceID();
                if (!_knownInstances.Add(instId)) continue;
                ProcessNewObject(uo, instId);
            }
        }
        catch (Exception ex) { CrashLog.Log($"[WorldSync] DetectNewObjects error: {ex.Message}"); }
    }

    private static void ProcessNewObject(UsableObject uo, int instId)
    {
        string objectId;
        byte objectType;
        int prefabId;
        if (!TryResolveIdentity(uo, instId, out objectId, out objectType, out prefabId)) return;
        if (string.IsNullOrEmpty(objectId)) return;
        if (!_knownIds.Add(objectId))
        {
            CrashLog.Log($"[WorldSync] DetectNewObjects: skipping '{objectId}' (already known by ID)");
            return;
        }
        FireSpawned(uo, objectId, objectType, prefabId);
    }

    private static bool TryResolveIdentity(UsableObject uo, int instId, out string objectId, out byte objectType, out int prefabId)
    {
        if (TryResolveNewServer(uo, instId, out objectId, out objectType, out prefabId)) return true;
        if (TryResolveNewSwitch(uo, out objectId, out objectType, out prefabId)) return true;
        if (TryResolveNewPatchPanel(uo, out objectId, out objectType, out prefabId)) return true;
        return false;
    }

    private static void FireSpawned(UsableObject uo, string objectId, byte objectType, int prefabId)
    {
        try
        {
            var pos = uo.transform.position;
            var rot = uo.transform.rotation;
            CrashLog.Log($"[WorldSync] DetectNewObjects: new object '{objectId}' type={objectType} prefab={prefabId} pos=({pos.x:F1},{pos.y:F1},{pos.z:F1})");
            EventDispatcher.FireObjectSpawned(objectId, objectType, prefabId, pos, rot);
        }
        catch { }
    }

    private static bool TryResolveNewServer(UsableObject uo, int instId,
        out string objectId, out byte objectType, out int prefabId)
    {
        objectId = null!;
        objectType = 0;
        prefabId = 0;
        var server = uo.TryCast<Server>();
        if (server == null) return false;
        objectId = ResolveServerId(server, instId);
        objectType = ReadServerType(server);
        prefabId = Patch_Server_ServerInsertedInRack.FindServerPrefabIndex(server);
        return true;
    }

    private static string ResolveServerId(Server server, int instId)
    {
        try
        {
            string objectId = server.ServerID ?? "";
            if (!string.IsNullOrEmpty(objectId)) return objectId;
            string n = server.gameObject?.name ?? "Server";
            if (n.EndsWith("(Clone)")) n = n.Substring(0, n.Length - 7);
            objectId = $"{n}_{instId}";
            server.ServerID = objectId;
            return objectId;
        }
        catch { return ""; }
    }

    private static byte ReadServerType(Server server)
    {
        try { return (byte)server.serverType; } catch { return 0; }
    }

    private static bool TryResolveNewSwitch(UsableObject uo,
        out string objectId, out byte objectType, out int prefabId)
    {
        objectId = null!;
        objectType = 0;
        prefabId = 0;
        var sw = uo.TryCast<NetworkSwitch>();
        if (sw == null) return false;
        objectId = ResolveSwitchId(sw);
        objectType = 4;
        prefabId = FindSwitchPrefabIndex(sw);
        return true;
    }

    private static string ResolveSwitchId(NetworkSwitch sw)
    {
        try
        {
            string objectId = sw.switchId ?? "";
            if (!string.IsNullOrEmpty(objectId)) return objectId;
            string n = sw.gameObject?.name ?? "Switch";
            if (n.EndsWith("(Clone)")) n = n.Substring(0, n.Length - 7);
            objectId = Patch_UsableObject_InteractOnClick.GenerateDeterministicId(n, sw.transform.position);
            sw.switchId = objectId;
            return objectId;
        }
        catch { return ""; }
    }

    private static int FindSwitchPrefabIndex(NetworkSwitch sw)
    {
        try
        {
            var mgr = MainGameManager.instance;
            if (mgr?.switchesPrefabs == null) return 0;
            string swName = StripCloneSuffix(sw.gameObject?.name ?? "");
            for (int i = 0; i < mgr.switchesPrefabs.Count; i++)
            {
                try { if (mgr.switchesPrefabs[i]?.name == swName) return i; } catch { }
            }
        }
        catch { }
        return 0;
    }

    private static string StripCloneSuffix(string name)
    {
        try
        {
            if (name.EndsWith("(Clone)")) name = name.Substring(0, name.Length - 7);
            return name;
        }
        catch { return name; }
    }

    private static bool TryResolveNewPatchPanel(UsableObject uo,
        out string objectId, out byte objectType, out int prefabId)
    {
        objectId = null!;
        objectType = 0;
        prefabId = 0;
        var pp = uo.TryCast<PatchPanel>();
        if (pp == null) return false;
        objectId = ResolvePanelId(pp);
        objectType = 7;
        prefabId = ResolvePanelPrefab(pp);
        return true;
    }

    private static string ResolvePanelId(PatchPanel pp)
    {
        try
        {
            string objectId = pp.patchPanelId ?? "";
            if (!string.IsNullOrEmpty(objectId)) return objectId;
            string n = pp.gameObject?.name ?? "PatchPanel";
            if (n.EndsWith("(Clone)")) n = n.Substring(0, n.Length - 7);
            objectId = Patch_UsableObject_InteractOnClick.GenerateDeterministicId(n, pp.transform.position);
            pp.patchPanelId = objectId;
            return objectId;
        }
        catch { return ""; }
    }

    private static int ResolvePanelPrefab(PatchPanel pp)
    {
        try
        {
            var mgr = MainGameManager.instance;
            if (mgr == null) return 0;
            var prefabGo = mgr.GetPatchPanelPrefab(pp.patchPanelType);
            if (prefabGo != null) return pp.patchPanelType;
        }
        catch { }
        return 0;
    }

    /// <summary>
    /// Populate known instances from the current scene so we don't re-fire ObjectSpawned
    /// for objects that came from the save file. Call this after scene load / save load.
    /// </summary>
    internal static void PopulateKnownServers()
    {
        _knownInstances.Clear();
        _knownIds.Clear();
        try
        {
            foreach (var uo in UnityEngine.Object.FindObjectsOfType<UsableObject>())
                RememberKnownObject(uo);
            CrashLog.Log($"[WorldSync] PopulateKnownServers: {_knownInstances.Count} instances, {_knownIds.Count} IDs");
        }
        catch (Exception ex) { CrashLog.Log($"[WorldSync] PopulateKnownServers error: {ex.Message}"); }
    }

    private static void RememberKnownObject(UsableObject uo)
    {
        try
        {
            _knownInstances.Add(uo.GetInstanceID());
            string id = ReadKnownId(uo);
            if (!string.IsNullOrEmpty(id)) _knownIds.Add(id);
        }
        catch { }
    }

    private static string ReadKnownId(UsableObject uo)
    {
        try
        {
            var srv = uo.TryCast<Server>(); if (srv != null) return srv.ServerID ?? "";
            var sw = uo.TryCast<NetworkSwitch>(); if (sw != null) return sw.switchId ?? "";
            var pp = uo.TryCast<PatchPanel>(); if (pp != null) return pp.patchPanelId ?? "";
        }
        catch { }
        return "";
    }

    /// <summary>
    /// Register a remotely-spawned object so we don't re-detect it.
    /// Called from WorldSpawnObjectImpl after creating an object.
    /// </summary>
    internal static void RegisterRemoteSpawn(int instanceId, string? serverId = null)
    {
        _knownInstances.Add(instanceId);
        if (!string.IsNullOrEmpty(serverId))
            _knownIds.Add(serverId);
    }
}
