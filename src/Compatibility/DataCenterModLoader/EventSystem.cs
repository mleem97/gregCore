using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using MelonLoader;
using UnityEngine;

namespace DataCenterModLoader;

public static class EventIds
{
    public const uint MoneyChanged = 100;
    public const uint XPChanged = 101;
    public const uint ReputationChanged = 102;

    public const uint ServerPowered = 200;
    public const uint ServerBroken = 201;
    public const uint ServerRepaired = 202;
    public const uint ServerInstalled = 203;
    public const uint CableConnected = 204;
    public const uint CableDisconnected = 205;
    public const uint ServerCustomerChanged = 206;
    public const uint ServerAppChanged = 207;
    public const uint RackUnmounted = 208;
    public const uint SwitchBroken = 209;
    public const uint SwitchRepaired = 210;
    public const uint ObjectSpawned = 211;
    public const uint ObjectPickedUp = 212;
    public const uint ObjectDropped = 213;

    public const uint DayEnded = 300;
    public const uint MonthEnded = 301;

    public const uint CustomerAccepted = 400;
    public const uint CustomerSatisfied = 401;
    public const uint CustomerUnsatisfied = 402;

    public const uint ShopCheckout = 500;
    public const uint ShopItemAdded = 501;
    public const uint ShopCartCleared = 502;
    public const uint ShopItemRemoved = 503;

    public const uint EmployeeHired = 600;
    public const uint EmployeeFired = 601;

    public const uint GameSaved = 700;
    public const uint GameLoaded = 701;
    public const uint GameAutoSaved = 702;

    public const uint WallPurchased = 800;

    public const uint NetWatchDispatched = 900; // 9xx = mod systems

    // mod systems (10xx)
    public const uint CustomEmployeeHired = 1000;
    public const uint CustomEmployeeFired = 1001;
}

public class EventDispatcher
{
    private static FFIBridge? _ffiBridge;
    private static MelonLogger.Instance? _logger;

    public static void Initialize(FFIBridge bridge, MelonLogger.Instance logger)
    {
        _ffiBridge = bridge;
        _logger = logger;
    }

    public static void FireValueChanged(uint eventId, float oldValue, float newValue, float delta)
    {
        _ffiBridge?.OnEvent(eventId, $"{oldValue}|{newValue}|{delta}");
    }

    public static void FireSimple(uint eventId)
    {
        _ffiBridge?.OnEvent(eventId, "");
    }

    public static void FireServerPowered(bool isOn)
    {
        _ffiBridge?.OnEvent(EventIds.ServerPowered, isOn ? "1" : "0");
    }

    public static void FireCustomEmployeeHired(string employeeId)
    {
        _ffiBridge?.OnEvent(EventIds.CustomEmployeeHired, employeeId);
    }

    public static void FireCustomEmployeeFired(string employeeId)
    {
        _ffiBridge?.OnEvent(EventIds.CustomEmployeeFired, employeeId);
    }

    public static void FireDayEnded(uint day)
    {
        _ffiBridge?.OnEvent(EventIds.DayEnded, day.ToString());
    }

    public static void FireMonthEnded(int month)
    {
        _ffiBridge?.OnEvent(EventIds.MonthEnded, month.ToString());
    }

    public static void FireCustomerAccepted(int customerId)
    {
        _ffiBridge?.OnEvent(EventIds.CustomerAccepted, customerId.ToString());
    }

    public static void FireObjectSpawned(string objectId, byte objectType, int prefabId, Vector3 pos, Quaternion rot)
    {
        _ffiBridge?.OnEvent(EventIds.ObjectSpawned, $"{objectId}|{objectType}|{prefabId}|{pos.x:F3}|{pos.y:F3}|{pos.z:F3}|{rot.x:F4}|{rot.y:F4}|{rot.z:F4}|{rot.w:F4}");
    }

    public static void FireServerInstalled(string serverId, byte objectType, int rackUid)
    {
        _ffiBridge?.OnEvent(EventIds.ServerInstalled, $"{serverId}|{objectType}|{rackUid}");
    }

    public static void FireCustomerSatisfied(int customerId)
    {
        _ffiBridge?.OnEvent(EventIds.CustomerSatisfied, customerId.ToString());
    }

    public static void FireCustomerUnsatisfied(int customerId)
    {
        _ffiBridge?.OnEvent(EventIds.CustomerUnsatisfied, customerId.ToString());
    }

    public static void FireCableConnected()
    {
        _ffiBridge?.OnEvent(EventIds.CableConnected, "");
    }

    public static void FireCableDisconnected()
    {
        _ffiBridge?.OnEvent(EventIds.CableDisconnected, "");
    }

    public static void FireServerCustomerChanged(int customerId)
    {
        _ffiBridge?.OnEvent(EventIds.ServerCustomerChanged, customerId.ToString());
    }

    public static void FireServerAppChanged(int appId)
    {
        _ffiBridge?.OnEvent(EventIds.ServerAppChanged, appId.ToString());
    }

    public static void FireRackUnmounted()
    {
        _ffiBridge?.OnEvent(EventIds.RackUnmounted, "");
    }

    public static void FireSwitchBroken()
    {
        _ffiBridge?.OnEvent(EventIds.SwitchBroken, "");
    }

    public static void FireSwitchRepaired()
    {
        _ffiBridge?.OnEvent(EventIds.SwitchRepaired, "");
    }

    public static void FireShopItemAdded(int item0, int item1, int item2)
    {
        _ffiBridge?.OnEvent(EventIds.ShopItemAdded, $"{item0}|{item1}|{item2}");
    }

    public static void FireWallPurchased()
    {
        _ffiBridge?.OnEvent(EventIds.WallPurchased, "");
    }

    public static void FireGameAutoSaved()
    {
        _ffiBridge?.OnEvent(EventIds.GameAutoSaved, "");
    }

    public static void LogError(string message)
    {
        _logger?.Error(message);
        CrashLog.Log("ERROR: " + message);
    }
}
