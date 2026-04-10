using System.Collections.Generic;

namespace DataCenterModLoader;

internal static class HookNames
{
    public const string EconomyBalanceOnChanged = "greg.Economy.Balance.OnChanged";
    public const string GameXpOnGained = "greg.Game.XP.OnGained";
    public const string CustomerReputationOnChanged = "greg.Customer.Reputation.OnChanged";

    public const string ObjectsDeviceOnPoweredOn = "greg.Objects.Device.OnPoweredOn";
    public const string ObjectsDeviceOnPoweredOff = "greg.Objects.Device.OnPoweredOff";
    public const string ObjectsDeviceOnDegraded = "greg.Objects.Device.OnDegraded";
    public const string ObjectsDeviceOnEOL = "greg.Objects.Device.OnEOL";
    public const string ObjectsDeviceOnRepaired = "greg.Objects.Device.OnRepaired";
    public const string ObjectsRackOnDevicePlaced = "greg.Objects.Rack.OnDevicePlaced";
    public const string NetworkCableOnConnected = "greg.Network.Cable.OnConnected";
    public const string NetworkCableOnDisconnected = "greg.Network.Cable.OnDisconnected";
    public const string ObjectsServerOnClientAssigned = "greg.Objects.Server.OnClientAssigned";
    public const string ObjectsServerOnClientUnassigned = "greg.Objects.Server.OnClientUnassigned";
    public const string ObjectsRackOnRemoved = "greg.Objects.Rack.OnRemoved";
    public const string NetworkCableOnLinkUp = "greg.Network.Cable.OnLinkUp";
    public const string NetworkCableOnLinkDown = "greg.Network.Cable.OnLinkDown";
    public const string NetworkCableOnConnectedSuppress = "greg.Network.Cable.OnConnected.Suppress";
    public const string NetworkCableOnDisconnectedSuppress = "greg.Network.Cable.OnDisconnected.Suppress";

    public const string GameTimeOnDayChanged = "greg.Game.Time.OnDayChanged";
    public const string GameTimeOnMonthChanged = "greg.Game.Time.OnMonthChanged";

    public const string CustomerContractOnSigned = "greg.Customer.Contract.OnSigned";
    public const string CustomerSlaOnRestored = "greg.Customer.SLA.OnRestored";
    public const string CustomerSlaOnBreached = "greg.Customer.SLA.OnBreached";

    public const string StoreCartOnCheckedOut = "greg.Store.Cart.OnCheckedOut";
    public const string StoreCartOnItemAdded = "greg.Store.Cart.OnItemAdded";
    public const string StoreCartOnCheckedOutCleared = "greg.Store.Cart.OnCheckedOut";
    public const string StoreCartOnItemRemoved = "greg.Store.Cart.OnItemRemoved";

    public const string EmployeesStaffOnHired = "greg.Employees.Staff.OnHired";
    public const string EmployeesStaffOnTerminated = "greg.Employees.Staff.OnTerminated";

    public const string GameSaveOnCompleted = "greg.Game.Save.OnCompleted";
    public const string GameLoadOnCompleted = "greg.Game.Load.OnCompleted";
    public const string GameSaveOnRequested = "greg.Game.Save.OnRequested";

    public const string WorldRoomOnExpanded = "greg.World.Room.OnExpanded";
    public const string NetworkTrafficOnThresholdExceeded = "greg.Network.Traffic.OnThresholdExceeded";

    public const string EmployeesStaffOnHiredCustom = "greg.Employees.Staff.OnHired";
    public const string EmployeesStaffOnTerminatedCustom = "greg.Employees.Staff.OnTerminated";

    public const string FrameworkHooksOnBridgeInstalled = "greg.Framework.Hooks.OnBridgeInstalled";
    public const string FrameworkHooksOnBridgeTriggered = "greg.Framework.Hooks.OnBridgeTriggered";

    private static readonly IReadOnlyDictionary<uint, string> EventIdToHookName =
        new Dictionary<uint, string>
        {
            [EventIds.MoneyChanged] = EconomyBalanceOnChanged,
            [EventIds.XPChanged] = GameXpOnGained,
            [EventIds.ReputationChanged] = CustomerReputationOnChanged,

            [EventIds.ServerPowered] = ObjectsDeviceOnPoweredOn,
            [EventIds.ServerBroken] = ObjectsDeviceOnDegraded,
            [EventIds.ServerRepaired] = ObjectsDeviceOnRepaired,
            [EventIds.ServerInstalled] = ObjectsRackOnDevicePlaced,
            [EventIds.CableConnected] = NetworkCableOnConnected,
            [EventIds.CableDisconnected] = NetworkCableOnDisconnected,
            [EventIds.ServerCustomerChanged] = ObjectsServerOnClientAssigned,
            [EventIds.ServerAppChanged] = ObjectsServerOnClientUnassigned,
            [EventIds.RackUnmounted] = ObjectsRackOnRemoved,
            [EventIds.SwitchBroken] = NetworkCableOnLinkDown,
            [EventIds.SwitchRepaired] = NetworkCableOnLinkUp,
            [EventIds.CableCreated] = NetworkCableOnConnected,
            [EventIds.CableRemoved] = NetworkCableOnDisconnected,
            [EventIds.CableCleared] = StoreCartOnCheckedOutCleared,
            [EventIds.CableSpeedChanged] = NetworkTrafficOnThresholdExceeded,
            [EventIds.CableSfpInserted] = NetworkCableOnConnected,
            [EventIds.CableSfpRemoved] = NetworkCableOnDisconnected,

            [EventIds.DayEnded] = GameTimeOnDayChanged,
            [EventIds.MonthEnded] = GameTimeOnMonthChanged,

            [EventIds.CustomerAccepted] = CustomerContractOnSigned,
            [EventIds.CustomerSatisfied] = CustomerSlaOnRestored,
            [EventIds.CustomerUnsatisfied] = CustomerSlaOnBreached,

            [EventIds.ShopCheckout] = StoreCartOnCheckedOut,
            [EventIds.ShopItemAdded] = StoreCartOnItemAdded,
            [EventIds.ShopCartCleared] = StoreCartOnCheckedOutCleared,
            [EventIds.ShopItemRemoved] = StoreCartOnItemRemoved,

            [EventIds.EmployeeHired] = EmployeesStaffOnHired,
            [EventIds.EmployeeFired] = EmployeesStaffOnTerminated,

            [EventIds.GameSaved] = GameSaveOnCompleted,
            [EventIds.GameLoaded] = GameLoadOnCompleted,
            [EventIds.GameAutoSaved] = GameSaveOnRequested,

            [EventIds.WallPurchased] = WorldRoomOnExpanded,
            [EventIds.NetWatchDispatched] = NetworkTrafficOnThresholdExceeded,

            [EventIds.CustomEmployeeHired] = EmployeesStaffOnHiredCustom,
            [EventIds.CustomEmployeeFired] = EmployeesStaffOnTerminatedCustom,

            [FrikaMF.EventIds.HookBridgeInstalled] = FrameworkHooksOnBridgeInstalled,
            [FrikaMF.EventIds.HookBridgeTriggered] = FrameworkHooksOnBridgeTriggered,
        };

    public static string Resolve(uint eventId)
    {
        if (EventIdToHookName.TryGetValue(eventId, out string name))
            return name;

        return "greg.Framework.Unknown.OnEvent";
    }
}