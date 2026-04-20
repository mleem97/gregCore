using System;
using System.Runtime.InteropServices;
using greg.Sdk;
using MelonLoader;

namespace greg.Core;

// must match dc_api/src/events.rs
public static class EventIds
{
    public const uint MoneyChanged      = 100;
    public const uint XPChanged         = 101;
    public const uint ReputationChanged = 102;

    public const uint ServerPowered   = 200;
    public const uint ServerBroken    = 201;
    public const uint ServerRepaired  = 202;
    public const uint ServerInstalled = 203;
    public const uint CableConnected          = 204;
    public const uint CableDisconnected       = 205;
    public const uint ServerCustomerChanged   = 206;
    public const uint ServerAppChanged        = 207;
    public const uint RackUnmounted  = 208;
    public const uint SwitchBroken   = 209;
    public const uint SwitchRepaired = 210;
    public const uint CableCreated = 211;
    public const uint CableRemoved = 212;
    public const uint CableCleared = 213;
    public const uint CableSpeedChanged = 214;
    public const uint CableSfpInserted = 215;
    public const uint CableSfpRemoved = 216;

    public const uint DayEnded = 300;
    public const uint MonthEnded = 301;

    public const uint CustomerAccepted  = 400;
    public const uint CustomerSatisfied = 401;
    public const uint CustomerUnsatisfied = 402;

    public const uint ShopCheckout = 500;
    public const uint ShopItemAdded  = 501;
    public const uint ShopCartCleared = 502;
    public const uint ShopItemRemoved = 503;

    public const uint EmployeeHired = 600;
    public const uint EmployeeFired = 601;

    public const uint GameSaved  = 700;
    public const uint GameLoaded = 701;
    public const uint GameAutoSaved = 702;

    public const uint WallPurchased = 800;

    public const uint NetWatchDispatched = 900; // 9xx = mod systems

    // mod systems (10xx)
    public const uint CustomEmployeeHired = 1000;
    public const uint CustomEmployeeFired = 1001;

    // hook bridge introspection (11xx) — keep in sync with dc_api / gregCore.EventIds history
    public const uint HookBridgeInstalled = 1100;
    public const uint HookBridgeTriggered = 1101;

    // finance / balance sheet extensions (12xx)
    public const uint RatesCalculated = 1200;
    public const uint BalanceSheetScreenOpened = 1201;
    public const uint BalanceSheetRecordAccessed = 1202;
    public const uint BalanceSheetTrackFinances = 1203;
    public const uint BalanceSheetFilled = 1204;
    public const uint BalanceSheetTotalRowAdded = 1205;
    public const uint BalanceSheetSalaryRegistered = 1206;
    public const uint BalanceSheetRecordRestored = 1207;
    public const uint BalanceSheetDataSaved = 1208;
    public const uint BalanceSheetDataLoaded = 1209;
    public const uint BalanceSheetLatestSnapshotRequested = 1210;
    public const uint ShopCartTotalUpdated = 1211;
    public const uint ShopNewItemPurchased = 1212;
    public const uint ShopAnotherItemPurchased = 1213;
    public const uint ShopPhysicalItemSpawned = 1214;

    // customer extensions (13xx)
    public const uint CustomerComponentInitialized = 1300;
    public const uint CustomerServerCountAndSpeedChanged = 1301;
    public const uint CustomerAppPerformanceAdded = 1302;
    public const uint CustomerAppSpeedsReset = 1303;
    public const uint CustomerBaseSetup = 1304;
    public const uint CustomerAppSetup = 1305;
    public const uint CustomerSpeedOnAppChanged = 1306;
    public const uint CustomerDataLoaded = 1307;
    public const uint CustomerDoorClicked = 1308;
    public const uint CustomerDoorHovered = 1309;
    public const uint CustomerDoorOpenedAndSetup = 1310;
    public const uint CustomerDoorOpened = 1311;
    public const uint CustomerDoorLoaded = 1312;
    public const uint CustomerDoorDestroyed = 1313;
    public const uint CustomerCardSet = 1314;
    public const uint CustomerChoiceCanceled = 1315;
    public const uint CustomerCardsCanvasShown = 1316;
    public const uint CustomerFallbackCreated = 1317;
    public const uint CustomerTotalRequirementRequested = 1318;
    public const uint CustomerSuitabilityChecked = 1319;
    public const uint NetworkCustomerBaseRegistered = 1320;
    public const uint NetworkCustomerBaseRequested = 1321;
    public const uint NetworkDeviceCustomerIdChanged = 1322;
    public const uint ServerChangeCustomerClicked = 1323;
    public const uint ServerNextCustomerIdRequested = 1324;
    public const uint ServerCustomerIdRequested = 1325;
}

// must match rust repr(C) layouts

[StructLayout(LayoutKind.Sequential)]
public struct ValueChangedData
{
    public double OldValue;
    public double NewValue;
    public double Delta;
}

[StructLayout(LayoutKind.Sequential)]
public struct ServerPoweredData
{
    public uint PoweredOn; // 1 = on, 0 = off
}

[StructLayout(LayoutKind.Sequential)]
public struct DayEndedData
{
    public uint Day;
}

[StructLayout(LayoutKind.Sequential)]
public struct CustomerAcceptedData
{
    public int CustomerId;
}

[StructLayout(LayoutKind.Sequential)]
public struct CustomerSatisfiedData
{
    public int CustomerBaseId;
}

[StructLayout(LayoutKind.Sequential)]
public struct ServerCustomerChangedData
{
    public int NewCustomerId;
}

[StructLayout(LayoutKind.Sequential)]
public struct ServerAppChangedData
{
    public int NewAppId;
}

[StructLayout(LayoutKind.Sequential)]
public struct MonthEndedData
{
    public int Month;
}

[StructLayout(LayoutKind.Sequential)]
public struct ShopItemAddedData
{
    public int ItemId;
    public int Price;
    public int ItemType;
}

[StructLayout(LayoutKind.Sequential)]
public struct ShopItemRemovedData
{
    public int Uid;
}

[StructLayout(LayoutKind.Sequential)]
public struct NetWatchDispatchedData
{
    public int DeviceType; // 0 = server, 1 = switch
    public int Reason;     // 0 = broken, 1 = eol_warning
}

[StructLayout(LayoutKind.Sequential)]
public struct RatesCalculatedData
{
    public double MoneyPerSec;
    public double XpPerSec;
    public double ExpensesPerSec;
}

[StructLayout(LayoutKind.Sequential)]
public struct BalanceSheetTotalRowData
{
    public double Revenue;
    public double Penalties;
    public double Total;
}

[StructLayout(LayoutKind.Sequential)]
public struct BalanceSheetSalaryRegisteredData
{
    public int MonthlySalary;
}

[StructLayout(LayoutKind.Sequential)]
public struct CustomerBaseIdData
{
    public int CustomerBaseId;
}

[StructLayout(LayoutKind.Sequential)]
public struct CustomerAppSpeedData
{
    public int AppId;
    public float Speed;
}

[StructLayout(LayoutKind.Sequential)]
public struct CustomerCountSpeedData
{
    public int Count;
    public float Speed;
}

[StructLayout(LayoutKind.Sequential)]
public struct CustomerAppSetupData
{
    public int AppId;
    public int Difficulty;
}

[StructLayout(LayoutKind.Sequential)]
public struct CustomerBoolResultData
{
    public int Value;
}

[StructLayout(LayoutKind.Sequential)]
public struct CustomEmployeeEventData
{
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
    public byte[] EmployeeId;

    public static CustomEmployeeEventData Create(string employeeId)
    {
        var data = new CustomEmployeeEventData { EmployeeId = new byte[64] };
        if (!string.IsNullOrEmpty(employeeId))
        {
            var bytes = System.Text.Encoding.ASCII.GetBytes(employeeId);
            Array.Copy(bytes, data.EmployeeId, Math.Min(bytes.Length, 63));
        }
        return data;
    }
}

// dispatches events to rust mods
public static class EventDispatcher
{
    private static gregFfiBridge _bridge;
    private static MelonLogger.Instance _logger;

    // dedup: harmony + il2cpp can double-fire patches
    private static uint _lastEventId;
    private static long _lastEventTick;
    private static double _lastEventPayloadHash;

    public static void Initialize(gregFfiBridge bridge, MelonLogger.Instance logger)
    {
        _bridge = bridge;
        _logger = logger;
    }

    private static bool IsDuplicate(uint eventId, double payloadHash = 0.0)
    {
        long now = System.Diagnostics.Stopwatch.GetTimestamp();
        long elapsed = now - _lastEventTick;
        long threshold = System.Diagnostics.Stopwatch.Frequency / 20; // ~50ms window

        bool isDup = (eventId == _lastEventId)
                     && (elapsed < threshold)
                     && (Math.Abs(payloadHash - _lastEventPayloadHash) < 0.0001);

        _lastEventId = eventId;
        _lastEventTick = now;
        _lastEventPayloadHash = payloadHash;

        return isDup;
    }

    private static void DispatchWithData<T>(uint eventId, T data, double payloadHash = 0.0) where T : struct
    {
        if (IsDuplicate(eventId, payloadHash)) return;

        GregHookIntegration.EmitForStruct(eventId, data);

        if (_bridge == null) return;

        int size = Marshal.SizeOf<T>();
        IntPtr ptr = Marshal.AllocHGlobal(size);
        try
        {
            Marshal.StructureToPtr(data, ptr, false);
            string hookName = gregNativeEventHooks.Resolve(eventId);
            CrashLog.Log($"DispatchWithData: hook={hookName}, eventId={eventId}, dataType={typeof(T).Name}, size={size}");
            _bridge.DispatchEvent(eventId, ptr, (uint)size);
        }
        catch (Exception ex)
        {
            _logger?.Error($"Failed to dispatch event {eventId}: {ex.Message}");
        }
        finally
        {
            Marshal.FreeHGlobal(ptr);
        }
    }

    public static void FireSimple(uint eventId)
    {
        if (IsDuplicate(eventId)) return;

        GregHookIntegration.EmitForSimple(eventId);

        if (_bridge == null) return;

        try
        {
            string hookName = gregNativeEventHooks.Resolve(eventId);
            CrashLog.Log($"FireSimple: hook={hookName}, eventId={eventId}");
            _bridge.DispatchEvent(eventId, IntPtr.Zero, 0);
        }
        catch (Exception ex) { _logger?.Error($"Failed to dispatch event {eventId}: {ex.Message}"); }
    }

    public static void LogError(string message)
    {
        _logger?.Error("[Events] " + message);
    }

    public static void FireValueChanged(uint eventId, double oldValue, double newValue, double delta)
    {
        DispatchWithData(eventId, new ValueChangedData
        {
            OldValue = oldValue,
            NewValue = newValue,
            Delta = delta,
        }, oldValue + newValue * 31.0);
    }

    public static void FireServerPowered(bool poweredOn)
    {
        DispatchWithData(EventIds.ServerPowered, new ServerPoweredData
        {
            PoweredOn = poweredOn ? 1u : 0u,
        }, poweredOn ? 1.0 : 0.0);
    }

    public static void FireDayEnded(uint day)
    {
        DispatchWithData(EventIds.DayEnded, new DayEndedData { Day = day }, day);
    }

    public static void FireCustomerAccepted(int customerId)
    {
        DispatchWithData(EventIds.CustomerAccepted, new CustomerAcceptedData { CustomerId = customerId }, customerId);
    }

    public static void FireCustomerSatisfied(int customerBaseId)
    {
        DispatchWithData(EventIds.CustomerSatisfied, new CustomerSatisfiedData { CustomerBaseId = customerBaseId }, customerBaseId);
    }

    public static void FireCustomerUnsatisfied(int customerBaseId)
    {
        CrashLog.Log($"FireCustomerUnsatisfied: dispatching for customerBaseId={customerBaseId}");
        DispatchWithData(EventIds.CustomerUnsatisfied, new CustomerSatisfiedData { CustomerBaseId = customerBaseId }, customerBaseId + 0.5);
    }

    public static void FireCableConnected()
    {
        FireSimple(EventIds.CableConnected);
    }

    public static void FireCableDisconnected()
    {
        FireSimple(EventIds.CableDisconnected);
    }

    public static void FireCableCreated()
    {
        FireSimple(EventIds.CableCreated);
    }

    public static void FireCableRemoved()
    {
        FireSimple(EventIds.CableRemoved);
    }

    public static void FireCableCleared()
    {
        FireSimple(EventIds.CableCleared);
    }

    public static void FireCableSpeedChanged()
    {
        FireSimple(EventIds.CableSpeedChanged);
    }

    public static void FireCableSfpInserted()
    {
        FireSimple(EventIds.CableSfpInserted);
    }

    public static void FireCableSfpRemoved()
    {
        FireSimple(EventIds.CableSfpRemoved);
    }

    public static void FireServerCustomerChanged(int newCustomerId)
    {
        DispatchWithData(EventIds.ServerCustomerChanged, new ServerCustomerChangedData { NewCustomerId = newCustomerId }, newCustomerId);
    }

    public static void FireServerAppChanged(int newAppId)
    {
        DispatchWithData(EventIds.ServerAppChanged, new ServerAppChangedData { NewAppId = newAppId }, newAppId);
    }

    public static void FireRackUnmounted()
    {
        FireSimple(EventIds.RackUnmounted);
    }

    public static void FireSwitchBroken()
    {
        FireSimple(EventIds.SwitchBroken);
    }

    public static void FireSwitchRepaired()
    {
        FireSimple(EventIds.SwitchRepaired);
    }

    public static void FireMonthEnded(int month)
    {
        DispatchWithData(EventIds.MonthEnded, new MonthEndedData { Month = month }, month);
    }

    public static void FireShopItemAdded(int itemId, int price, int itemType)
    {
        DispatchWithData(EventIds.ShopItemAdded, new ShopItemAddedData { ItemId = itemId, Price = price, ItemType = itemType }, itemId * 1000.0 + price + itemType * 0.1);
    }

    public static void FireShopItemRemoved(int uid)
    {
        DispatchWithData(EventIds.ShopItemRemoved, new ShopItemRemovedData { Uid = uid }, uid);
    }

    public static void FireShopCartCleared()
    {
        FireSimple(EventIds.ShopCartCleared);
    }

    public static void FireGameAutoSaved()
    {
        FireSimple(EventIds.GameAutoSaved);
    }

    public static void FireWallPurchased()
    {
        FireSimple(EventIds.WallPurchased);
    }

    public static void FireNetWatchDispatched(int deviceType, int reason)
    {
        DispatchWithData(EventIds.NetWatchDispatched, new NetWatchDispatchedData
        {
            DeviceType = deviceType,
            Reason = reason
        }, deviceType * 10.0 + reason);
    }

    public static void FireCustomEmployeeHired(string employeeId)
    {
        DispatchWithData(EventIds.CustomEmployeeHired, CustomEmployeeEventData.Create(employeeId), employeeId.GetHashCode());
    }

    public static void FireCustomEmployeeFired(string employeeId)
    {
        DispatchWithData(EventIds.CustomEmployeeFired, CustomEmployeeEventData.Create(employeeId), employeeId.GetHashCode() + 0.5);
    }

    public static void FireHookBridgeInstalled(int installed, int failed)
    {
        CrashLog.Log($"FireHookBridgeInstalled: installed={installed}, failed={failed}");
        FireSimple(EventIds.HookBridgeInstalled);
    }

    public static void FireHookBridgeTriggered(string methodKey)
    {
        CrashLog.Log($"FireHookBridgeTriggered: method={methodKey}");
        FireSimple(EventIds.HookBridgeTriggered);
    }

    public static void FireRatesCalculated(double moneyPerSec, double xpPerSec, double expensesPerSec)
    {
        DispatchWithData(EventIds.RatesCalculated, new RatesCalculatedData
        {
            MoneyPerSec = moneyPerSec,
            XpPerSec = xpPerSec,
            ExpensesPerSec = expensesPerSec
        }, moneyPerSec + xpPerSec * 31.0 + expensesPerSec * 97.0);
    }

    public static void FireBalanceSheetScreenOpened()
    {
        FireSimple(EventIds.BalanceSheetScreenOpened);
    }

    public static void FireBalanceSheetRecordAccessed()
    {
        FireSimple(EventIds.BalanceSheetRecordAccessed);
    }

    public static void FireBalanceSheetTrackFinances()
    {
        FireSimple(EventIds.BalanceSheetTrackFinances);
    }

    public static void FireBalanceSheetFilled()
    {
        FireSimple(EventIds.BalanceSheetFilled);
    }

    public static void FireBalanceSheetTotalRowAdded(double revenue, double penalties, double total)
    {
        DispatchWithData(EventIds.BalanceSheetTotalRowAdded, new BalanceSheetTotalRowData
        {
            Revenue = revenue,
            Penalties = penalties,
            Total = total,
        }, revenue + penalties * 31.0 + total * 97.0);
    }

    public static void FireBalanceSheetSalaryRegistered(int monthlySalary)
    {
        DispatchWithData(EventIds.BalanceSheetSalaryRegistered, new BalanceSheetSalaryRegisteredData
        {
            MonthlySalary = monthlySalary,
        }, monthlySalary);
    }

    public static void FireBalanceSheetRecordRestored()
    {
        FireSimple(EventIds.BalanceSheetRecordRestored);
    }

    public static void FireBalanceSheetDataSaved()
    {
        FireSimple(EventIds.BalanceSheetDataSaved);
    }

    public static void FireBalanceSheetDataLoaded()
    {
        FireSimple(EventIds.BalanceSheetDataLoaded);
    }

    public static void FireBalanceSheetLatestSnapshotRequested()
    {
        FireSimple(EventIds.BalanceSheetLatestSnapshotRequested);
    }

    public static void FireShopCartTotalUpdated()
    {
        FireSimple(EventIds.ShopCartTotalUpdated);
    }

    public static void FireShopNewItemPurchased(int itemId, int price, int itemType)
    {
        DispatchWithData(EventIds.ShopNewItemPurchased, new ShopItemAddedData { ItemId = itemId, Price = price, ItemType = itemType }, itemId * 1000.0 + price + itemType * 0.1);
    }

    public static void FireShopAnotherItemPurchased(int itemId, int price, int itemType)
    {
        DispatchWithData(EventIds.ShopAnotherItemPurchased, new ShopItemAddedData { ItemId = itemId, Price = price, ItemType = itemType }, itemId * 1000.0 + price + itemType * 0.1 + 0.5);
    }

    public static void FireShopPhysicalItemSpawned(int price, int itemType)
    {
        DispatchWithData(EventIds.ShopPhysicalItemSpawned, new ShopItemAddedData { ItemId = -1, Price = price, ItemType = itemType }, price + itemType * 0.1);
    }

    public static void FireCustomerComponentInitialized(int customerBaseId)
    {
        DispatchWithData(EventIds.CustomerComponentInitialized, new CustomerBaseIdData { CustomerBaseId = customerBaseId }, customerBaseId);
    }

    public static void FireCustomerServerCountAndSpeedChanged(int count, float speed)
    {
        DispatchWithData(EventIds.CustomerServerCountAndSpeedChanged, new CustomerCountSpeedData { Count = count, Speed = speed }, count + speed * 31.0);
    }

    public static void FireCustomerAppPerformanceAdded(int appId, float speed)
    {
        DispatchWithData(EventIds.CustomerAppPerformanceAdded, new CustomerAppSpeedData { AppId = appId, Speed = speed }, appId + speed * 31.0);
    }

    public static void FireCustomerAppSpeedsReset(int customerBaseId)
    {
        DispatchWithData(EventIds.CustomerAppSpeedsReset, new CustomerBaseIdData { CustomerBaseId = customerBaseId }, customerBaseId);
    }

    public static void FireCustomerBaseSetup(int customerBaseId)
    {
        DispatchWithData(EventIds.CustomerBaseSetup, new CustomerBaseIdData { CustomerBaseId = customerBaseId }, customerBaseId);
    }

    public static void FireCustomerAppSetup(int appId, int difficulty)
    {
        DispatchWithData(EventIds.CustomerAppSetup, new CustomerAppSetupData { AppId = appId, Difficulty = difficulty }, appId + difficulty * 31.0);
    }

    public static void FireCustomerSpeedOnAppChanged(int appId, float speed)
    {
        DispatchWithData(EventIds.CustomerSpeedOnAppChanged, new CustomerAppSpeedData { AppId = appId, Speed = speed }, appId + speed * 31.0);
    }

    public static void FireCustomerDataLoaded(int customerBaseId)
    {
        DispatchWithData(EventIds.CustomerDataLoaded, new CustomerBaseIdData { CustomerBaseId = customerBaseId }, customerBaseId);
    }

    public static void FireCustomerDoorClicked()
    {
        FireSimple(EventIds.CustomerDoorClicked);
    }

    public static void FireCustomerDoorHovered()
    {
        FireSimple(EventIds.CustomerDoorHovered);
    }

    public static void FireCustomerDoorOpenedAndSetup()
    {
        FireSimple(EventIds.CustomerDoorOpenedAndSetup);
    }

    public static void FireCustomerDoorOpened()
    {
        FireSimple(EventIds.CustomerDoorOpened);
    }

    public static void FireCustomerDoorLoaded()
    {
        FireSimple(EventIds.CustomerDoorLoaded);
    }

    public static void FireCustomerDoorDestroyed()
    {
        FireSimple(EventIds.CustomerDoorDestroyed);
    }

    public static void FireCustomerCardSet()
    {
        FireSimple(EventIds.CustomerCardSet);
    }

    public static void FireCustomerChoiceCanceled()
    {
        FireSimple(EventIds.CustomerChoiceCanceled);
    }

    public static void FireCustomerCardsCanvasShown()
    {
        FireSimple(EventIds.CustomerCardsCanvasShown);
    }

    public static void FireCustomerFallbackCreated(int customerId)
    {
        DispatchWithData(EventIds.CustomerFallbackCreated, new CustomerAcceptedData { CustomerId = customerId }, customerId);
    }

    public static void FireCustomerTotalRequirementRequested()
    {
        FireSimple(EventIds.CustomerTotalRequirementRequested);
    }

    public static void FireCustomerSuitabilityChecked(bool isSuitable)
    {
        DispatchWithData(EventIds.CustomerSuitabilityChecked, new CustomerBoolResultData { Value = isSuitable ? 1 : 0 }, isSuitable ? 1.0 : 0.0);
    }

    public static void FireNetworkCustomerBaseRegistered(int customerBaseId)
    {
        DispatchWithData(EventIds.NetworkCustomerBaseRegistered, new CustomerBaseIdData { CustomerBaseId = customerBaseId }, customerBaseId);
    }

    public static void FireNetworkCustomerBaseRequested(int customerBaseId)
    {
        DispatchWithData(EventIds.NetworkCustomerBaseRequested, new CustomerBaseIdData { CustomerBaseId = customerBaseId }, customerBaseId);
    }

    public static void FireNetworkDeviceCustomerIdChanged(int customerId)
    {
        DispatchWithData(EventIds.NetworkDeviceCustomerIdChanged, new CustomerAcceptedData { CustomerId = customerId }, customerId);
    }

    public static void FireServerChangeCustomerClicked()
    {
        FireSimple(EventIds.ServerChangeCustomerClicked);
    }

    public static void FireServerNextCustomerIdRequested(int customerId)
    {
        DispatchWithData(EventIds.ServerNextCustomerIdRequested, new CustomerAcceptedData { CustomerId = customerId }, customerId);
    }

    public static void FireServerCustomerIdRequested(int customerId)
    {
        DispatchWithData(EventIds.ServerCustomerIdRequested, new CustomerAcceptedData { CustomerId = customerId }, customerId);
    }
}





