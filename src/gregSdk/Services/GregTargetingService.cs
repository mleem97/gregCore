using System;
using UnityEngine;
using Il2Cpp;
using greg.Core;

namespace greg.Sdk.Services;

public enum GregTargetType
{
    None,
    Server,
    Rack,
    Switch,
    Cable,
    CableSpool,
    Furniture,
    NPC,
    Other
}

public struct GregTargetInfo
{
    public GregTargetType TargetType;
    public GameObject Entity;
    public string Name;
    public float Distance;
    public Vector3 HitPoint;
    public object NativeComponent; // Reference to the actual Rack/Server/Cable instance
}

/// <summary>
/// SDK Service for identifying world objects under the player's crosshair.
/// </summary>
public static class GregTargetingService
{
    private const float DefaultMaxDistance = 10.0f;

    public static GregTargetInfo GetTargetInfo(float maxDistance = DefaultMaxDistance)
    {
        var result = new GregTargetInfo
        {
            TargetType = GregTargetType.None,
            Distance = float.MaxValue
        };

        var hit = gregGameHooks.RaycastForward(maxDistance);
        if (hit == null || hit.Value.Entity == null)
            return result;

        result.Entity = hit.Value.Entity;
        result.Name = hit.Value.Name;
        result.Distance = hit.Value.Distance;
        result.HitPoint = hit.Value.Point;

        // Identification Logic
        if (TryIdentify(result.Entity, out var type, out var native))
        {
            result.TargetType = type;
            result.NativeComponent = native;
        }
        else
        {
            result.TargetType = GregTargetType.Other;
        }

        return result;
    }

    private static bool TryIdentify(GameObject entity, out GregTargetType type, out object native)
    {
        type = GregTargetType.None;
        native = null;

        if (entity == null) return false;

        // 1. Rack
        var rack = entity.GetComponentInParent<Rack>();
        if (rack != null)
        {
            type = GregTargetType.Rack;
            native = rack;
            return true;
        }

        // 2. Server
        var server = entity.GetComponentInParent<Server>();
        if (server != null)
        {
            type = GregTargetType.Server;
            native = server;
            return true;
        }

        // 3. Switch
        var sw = entity.GetComponentInParent<NetworkSwitch>();
        if (sw != null)
        {
            type = GregTargetType.Switch;
            native = sw;
            return true;
        }

        // 4. Cable
        var cable = entity.GetComponentInParent<CableLink>();
        if (cable != null)
        {
            type = GregTargetType.Cable;
            native = cable;
            return true;
        }

        // 5. Cable Spool / Spinner
        if (entity.name.ToLower().Contains("spool") || entity.name.ToLower().Contains("spinner"))
        {
            type = GregTargetType.CableSpool;
            return true;
        }

        // 6. NPC / Employee
        var npc = entity.GetComponent<Component>();
        if (npc != null && npc.GetType().Name == "EmployeeClass")
        {
            type = GregTargetType.NPC;
            native = npc;
            return true;
        }

        return false;
    }
}

