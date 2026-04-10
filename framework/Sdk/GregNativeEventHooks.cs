using System.Collections.Generic;
using DataCenterModLoader;

namespace gregFramework.Core;

/// <summary>
/// Canonical greg.* hook names for the native / FFI event pipeline (<see cref="EventDispatcher"/>).
/// Strings match <c>greg_hooks.json</c> <c>name</c> where that file lists the patched method; otherwise
/// <see cref="GregHookName.Create"/> is used for framework-only events (save/load, bridge, NetWatch, etc.).
/// </summary>
public static class GregNativeEventHooks
{
    public const string UnknownNativeEvent = "greg.SYSTEM.UnmappedNativeEvent";

    // —— Player (greg_hooks.json: Player.UpdateCoin / UpdateXP / UpdateReputation) ——
    public const string PlayerCoinChanged = "greg.PLAYER.CoinChanged";
    public const string PlayerXpChanged = "greg.PLAYER.XPChanged";
    public const string PlayerReputationChanged = "greg.PLAYER.ReputationChanged";

    // —— Server / rack / cables (same file, SERVER.* / RACK.* / NETWORK.*) ——
    public const string ServerPowerButton = "greg.SERVER.PowerButton";
    public const string ServerItIsBroken = "greg.SERVER.ItIsBroken";
    public const string ServerDeviceRepaired = "greg.SERVER.DeviceRepaired";
    public const string ServerInsertedInRack = "greg.SERVER.ServerInsertedInRack";
    public const string ServerRegisterLink = "greg.SERVER.RegisterLink";
    public const string ServerUnregisterLink = "greg.SERVER.UnregisterLink";
    public const string ServerCustomerChanged = "greg.SERVER.CustomerChanged";
    public const string ServerAppIdChanged = "greg.SERVER.AppIDChanged";
    public const string RackButtonUnmountRack = "greg.RACK.ButtonUnmountRack";
    public const string NetworkBrokenSwitchAdded = "greg.NETWORK.BrokenSwitchAdded";
    public const string NetworkBrokenSwitchRemoved = "greg.NETWORK.oveBrokenSwitchRemoved";
    public const string NetworkCreateNewCable = "greg.NETWORK.CreateNewCable";
    public const string NetworkPositionRemoved = "greg.NETWORK.ovePositionRemoved";
    public const string NetworkClearAllCables = "greg.NETWORK.ClearAllCables";
    public const string NetworkConnectionSpeedSet = "greg.NETWORK.ConnectionSpeedSet";
    public const string NetworkInsertSfp = "greg.NETWORK.InsertSFP";
    public const string NetworkSfpRemoved = "greg.NETWORK.oveSFPRemoved";

    // —— Time / balance sheet ——
    public const string SystemSnapshotSaved = "greg.SYSTEM.SnapshotSaved";

    // —— MainGameManager / ComputerShop / HR (greg_hooks lists these under greg.SYSTEM.*) ——
    public const string SystemButtonCustomerChosen = "greg.SYSTEM.ButtonCustomerChosen";
    public const string SystemButtonCheckOut = "greg.SYSTEM.ButtonCheckOut";
    public const string SystemButtonBuyShopItem = "greg.SYSTEM.ButtonBuyShopItem";
    public const string SystemButtonClear = "greg.SYSTEM.ButtonClear";
    public const string SystemSpawnedItemRemoved = "greg.SYSTEM.oveSpawnedItemRemoved";
    public const string SystemButtonConfirmHire = "greg.SYSTEM.ButtonConfirmHire";
    public const string SystemButtonConfirmFireEmployee = "greg.SYSTEM.ButtonConfirmFireEmployee";
    public const string SystemButtonBuyWall = "greg.SYSTEM.ButtonBuyWall";

    // —— Framework-only (no matching entry in greg_hooks for this pipeline) ——
    public static readonly string SystemGameDayAdvanced = GregHookName.Create(GregDomain.System, "GameDayAdvanced");
    public static readonly string CustomerAppRequirementsSatisfied = GregHookName.Create(GregDomain.Customer, "AppRequirementsSatisfied");
    public static readonly string CustomerAppRequirementsFailed = GregHookName.Create(GregDomain.Customer, "AppRequirementsFailed");
    public static readonly string SystemGameSaved = GregHookName.Create(GregDomain.System, "GameSaved");
    public static readonly string SystemGameLoaded = GregHookName.Create(GregDomain.System, "GameLoaded");
    public static readonly string SystemAutoSaveRequested = GregHookName.Create(GregDomain.System, "AutoSaveRequested");
    public static readonly string NetworkNetWatchDispatched = GregHookName.Create(GregDomain.Network, "NetWatchDispatched");
    public static readonly string EmployeeCustomHired = GregHookName.Create(GregDomain.Employee, "CustomHired");
    public static readonly string EmployeeCustomFired = GregHookName.Create(GregDomain.Employee, "CustomFired");
    public static readonly string SystemHookBridgeInstalled = GregHookName.Create(GregDomain.System, "HookBridgeInstalled");
    public static readonly string SystemHookBridgeTriggered = GregHookName.Create(GregDomain.System, "HookBridgeTriggered");

    private static readonly IReadOnlyDictionary<uint, string> ByEventId = BuildByEventId();

    private static Dictionary<uint, string> BuildByEventId()
    {
        return new Dictionary<uint, string>
        {
            [EventIds.MoneyChanged] = PlayerCoinChanged,
            [EventIds.XPChanged] = PlayerXpChanged,
            [EventIds.ReputationChanged] = PlayerReputationChanged,

            [EventIds.ServerPowered] = ServerPowerButton,
            [EventIds.ServerBroken] = ServerItIsBroken,
            [EventIds.ServerRepaired] = ServerDeviceRepaired,
            [EventIds.ServerInstalled] = ServerInsertedInRack,

            [EventIds.CableConnected] = ServerRegisterLink,
            [EventIds.CableDisconnected] = ServerUnregisterLink,
            [EventIds.ServerCustomerChanged] = ServerCustomerChanged,
            [EventIds.ServerAppChanged] = ServerAppIdChanged,
            [EventIds.RackUnmounted] = RackButtonUnmountRack,
            [EventIds.SwitchBroken] = NetworkBrokenSwitchAdded,
            [EventIds.SwitchRepaired] = NetworkBrokenSwitchRemoved,
            [EventIds.CableCreated] = NetworkCreateNewCable,
            [EventIds.CableRemoved] = NetworkPositionRemoved,
            [EventIds.CableCleared] = NetworkClearAllCables,
            [EventIds.CableSpeedChanged] = NetworkConnectionSpeedSet,
            [EventIds.CableSfpInserted] = NetworkInsertSfp,
            [EventIds.CableSfpRemoved] = NetworkSfpRemoved,

            [EventIds.DayEnded] = SystemGameDayAdvanced,
            [EventIds.MonthEnded] = SystemSnapshotSaved,

            [EventIds.CustomerAccepted] = SystemButtonCustomerChosen,
            [EventIds.CustomerSatisfied] = CustomerAppRequirementsSatisfied,
            [EventIds.CustomerUnsatisfied] = CustomerAppRequirementsFailed,

            [EventIds.ShopCheckout] = SystemButtonCheckOut,
            [EventIds.ShopItemAdded] = SystemButtonBuyShopItem,
            [EventIds.ShopCartCleared] = SystemButtonClear,
            [EventIds.ShopItemRemoved] = SystemSpawnedItemRemoved,

            [EventIds.EmployeeHired] = SystemButtonConfirmHire,
            [EventIds.EmployeeFired] = SystemButtonConfirmFireEmployee,

            [EventIds.GameSaved] = SystemGameSaved,
            [EventIds.GameLoaded] = SystemGameLoaded,
            [EventIds.GameAutoSaved] = SystemAutoSaveRequested,

            [EventIds.WallPurchased] = SystemButtonBuyWall,
            [EventIds.NetWatchDispatched] = NetworkNetWatchDispatched,

            [EventIds.CustomEmployeeHired] = EmployeeCustomHired,
            [EventIds.CustomEmployeeFired] = EmployeeCustomFired,

            [EventIds.HookBridgeInstalled] = SystemHookBridgeInstalled,
            [EventIds.HookBridgeTriggered] = SystemHookBridgeTriggered,
        };
    }

    /// <summary>Resolved greg hook for crash logs and tooling; never null.</summary>
    public static string Resolve(uint eventId)
    {
        return ByEventId.TryGetValue(eventId, out var name) ? name : UnknownNativeEvent;
    }

    internal static bool TryGetHookForEvent(uint eventId, out string hookName)
    {
        return ByEventId.TryGetValue(eventId, out hookName);
    }
}
