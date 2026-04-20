using System.Collections.Generic;
using greg.Core;

namespace greg.Sdk;

/// <summary>
/// Canonical greg.* hook names for the native / FFI event pipeline (<see cref="EventDispatcher"/>).
/// Strings match <c>greg_hooks.json</c> <c>name</c> where that file lists the patched method; otherwise
/// <see cref="gregHookName.Create"/> is used for framework-only events (save/load, bridge, NetWatch, etc.).
/// </summary>
public static class gregNativeEventHooks
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
    public const string SystemCalculateRates = "greg.SYSTEM.CalculateRates";
    public const string SystemButtonBalanceSheetScreen = "greg.SYSTEM.ButtonBalanceSheetScreen";
    public const string UiGetOrCreateRecord = "greg.UI.GetOrCreateRecord";
    public const string SystemTrackFinances = "greg.SYSTEM.TrackFinances";
    public const string SystemFillInBalanceSheet = "greg.SYSTEM.FillInBalanceSheet";
    public const string SystemTotalRowAdded = "greg.SYSTEM.TotalRowAdded";
    public const string SystemRegisterSalary = "greg.SYSTEM.RegisterSalary";
    public const string SystemRestoreRecord = "greg.SYSTEM.RestoreRecord";
    public const string UiGetSaveData = "greg.UI.GetSaveData";
    public const string UiFromSaveLoaded = "greg.UI.FromSaveLoaded";
    public const string UiGetLatestSnapshot = "greg.UI.GetLatestSnapshot";
    public const string SystemCartTotalChanged = "greg.SYSTEM.CartTotalChanged";
    public const string SystemNewItemPurchased = "greg.SYSTEM.NewItemPurchased";
    public const string SystemAnotherItemPurchased = "greg.SYSTEM.AnotherItemPurchased";
    public const string SystemPhysicalItemSpawned = "greg.SYSTEM.PhysicalItemSpawned";

    // —— Customer extensions ——
    public const string CustomerComponentInitialized = "greg.CUSTOMER.ComponentInitialized";
    public const string CustomerServerCountAndSpeedChanged = "greg.CUSTOMER.CustomerServerCountAndSpeedChanged";
    public const string CustomerAppPerformanceAdded = "greg.CUSTOMER.AppPerformanceAdded";
    public const string CustomerResetAllAppSpeeds = "greg.CUSTOMER.ResetAllAppSpeeds";
    public const string CustomerUpBaseSet = "greg.CUSTOMER.UpBaseSet";
    public const string CustomerUpAppSet = "greg.CUSTOMER.UpAppSet";
    public const string CustomerSpeedOnCustomerBaseAppChanged = "greg.CUSTOMER.SpeedOnCustomerBaseAppChanged";
    public const string CustomerDataLoaded = "greg.CUSTOMER.DataLoaded";
    public const string CustomerInteractOnClick = "greg.CUSTOMER.InteractOnClick";
    public const string CustomerInteractOnHover = "greg.CUSTOMER.InteractOnHover";
    public const string CustomerOpenDoorAndSetupBase = "greg.CUSTOMER.OpenDoorAndSetupBase";
    public const string CustomerOpenDoor = "greg.CUSTOMER.OpenDoor";
    public const string CustomerOnLoad = "greg.CUSTOMER.OnLoad";
    public const string CustomerOnDestroy = "greg.CUSTOMER.OnDestroy";
    public const string CustomerSet = "greg.CUSTOMER.CustomerSet";
    public const string SystemButtonCancelCustomerChoice = "greg.SYSTEM.ButtonCancelCustomerChoice";
    public const string SystemShowCustomerCardsCanvas = "greg.SYSTEM.ShowCustomerCardsCanvas";
    public const string SystemCreateFallbackCustomer = "greg.SYSTEM.CreateFallbackCustomer";
    public const string SystemGetCustomerTotalRequirement = "greg.SYSTEM.GetCustomerTotalRequirement";
    public const string SystemIsCustomerSuitableForBase = "greg.SYSTEM.IsCustomerSuitableForBase";
    public const string NetworkRegisterCustomerBase = "greg.NETWORK.RegisterCustomerBase";
    public const string NetworkGetCustomerBase = "greg.NETWORK.GetCustomerBase";
    public const string NetworkDeviceCustomerIDChanged = "greg.NETWORK.DeviceCustomerIDChanged";
    public const string ServerButtonClickChangeCustomer = "greg.SERVER.ButtonClickChangeCustomer";
    public const string ServerGetNextCustomerID = "greg.SERVER.GetNextCustomerID";
    public const string ServerGetCustomerID = "greg.SERVER.GetCustomerID";

    // —— MainGameManager / ComputerShop / HR (greg_hooks lists these under greg.SYSTEM.*) ——
    public const string SystemButtonCustomerChosen = "greg.SYSTEM.ButtonCustomerChosen";
    public const string SystemButtonCheckOut = "greg.SYSTEM.ButtonCheckOut";
    public const string SystemButtonBuyShopItem = "greg.SYSTEM.ButtonBuyShopItem";
    public const string SystemButtonClear = "greg.SYSTEM.ButtonClear";
    public const string SystemSpawnedItemRemoved = "greg.SYSTEM.oveSpawnedItemRemoved";
    public const string SystemButtonConfirmHire = "greg.SYSTEM.ButtonConfirmHire";
    public const string SystemButtonConfirmFireEmployee = "greg.SYSTEM.ButtonConfirmFireEmployee";
    public const string SystemButtonBuyWall = "greg.SYSTEM.ButtonBuyWall";

    // —— Added per Community Feature Requests ——
    public const string EmployeeIdleStateEntered = "greg.EMPLOYEE.IdleStateEntered";
    public const string EmployeeIdleStateExited = "greg.EMPLOYEE.IdleStateExited";
    public const string SystemIncidentTriggered = "greg.SYSTEM.IncidentTriggered";
    public const string SystemIncidentResolved = "greg.SYSTEM.IncidentResolved";
    public const string NetworkTopologyValidated = "greg.NETWORK.TopologyValidated";

    // —— Content Lifecycle Hooks ——
    public const string ContentLoaded = "greg.CONTENT.Loaded";
    public const string ContentRegistered = "greg.CONTENT.Registered";
    public const string ContentOverrideApplied = "greg.CONTENT.OverrideApplied";

    // —— Normalized Unity Signal Hooks ——
    public const string SystemRuntimeObjectEnabled = "greg.SYSTEM.RuntimeObjectEnabled";
    public const string SystemRuntimeObjectDisabled = "greg.SYSTEM.RuntimeObjectDisabled";
    public const string SystemRuntimeObjectDestroyed = "greg.SYSTEM.RuntimeObjectDestroyed";
    public const string SystemRuntimeObjectLoaded = "greg.SYSTEM.RuntimeObjectLoaded";
    public const string SystemApplicationQuit = "greg.SYSTEM.ApplicationQuit";
    public const string SystemSaveStarted = "greg.SYSTEM.SaveStarted";
    public const string SystemLoadStarted = "greg.SYSTEM.LoadStarted";
    public const string SystemLoadCompleted = "greg.SYSTEM.LoadCompleted";
    public const string SystemDayEnded = "greg.SYSTEM.DayEnded";

    public const string InputActionTriggered = "greg.INPUT.ActionTriggered";
    public const string InputBindingStarted = "greg.INPUT.BindingStarted";
    public const string InputBindingApplied = "greg.INPUT.BindingApplied";
    public const string UiPointerEnter = "greg.UI.PointerEnter";
    public const string UiPointerExit = "greg.UI.PointerExit";
    public const string UiPointerClick = "greg.UI.PointerClick";
    public const string UiSubmit = "greg.UI.Submit";
    public const string UiCancel = "greg.UI.Cancel";
    public const string UiPauseOpened = "greg.UI.PauseOpened";
    public const string UiPauseClosed = "greg.UI.PauseClosed";
    public const string UiTabSelected = "greg.UI.TabSelected";

    public const string CustomerRequirementEvaluated = "greg.CUSTOMER.RequirementEvaluated";
    public const string CustomerMoneyUpdated = "greg.CUSTOMER.MoneyUpdated";
    public const string SystemShopCartItemAdded = "greg.SYSTEM.ShopCartItemAdded";
    public const string SystemShopCartItemRemoved = "greg.SYSTEM.ShopCartItemRemoved";

    public const string NetworkCableLifecycleChanged = "greg.NETWORK.CableLifecycleChanged";
    public const string NetworkSwitchConfigOpened = "greg.NETWORK.SwitchConfigOpened";
    public const string NetworkSwitchConfigClosed = "greg.NETWORK.SwitchConfigClosed";
    public const string NetworkDispatchQueued = "greg.NETWORK.DispatchQueued";
    public const string NetworkDispatchProcessed = "greg.NETWORK.DispatchProcessed";
    public const string ServerLoadingStarted = "greg.SERVER.LoadingStarted";
    public const string ServerLoadingCompleted = "greg.SERVER.LoadingCompleted";
    public const string RackDoorStateChanged = "greg.RACK.DoorStateChanged";
    public const string WorldInteractionHovered = "greg.WORLD.InteractionHovered";
    public const string WorldTriggerEntered = "greg.WORLD.TriggerEntered";
    public const string WorldCollisionEntered = "greg.WORLD.CollisionEntered";

    public const string EmployeeDispatchQueued = "greg.EMPLOYEE.DispatchQueued";
    public const string EmployeeDispatchProcessed = "greg.EMPLOYEE.DispatchProcessed";
    public const string EmployeeAnimationStateChanged = "greg.EMPLOYEE.AnimationStateChanged";
    public const string GameplayIncidentTriggered = "greg.GAMEPLAY.IncidentTriggered";

    // —— Performance & Telemetry ——
    public const string SYSTEM_GameStateChanged = "greg.SYSTEM.GameStateChanged";
    public const string SYSTEM_FrameLimitChanged = "greg.SYSTEM.FrameLimitChanged";
    public const string SYSTEM_RenderOptimizerApplied = "greg.SYSTEM.RenderOptimizerApplied";
    public const string SYSTEM_TelemetryExported = "greg.SYSTEM.TelemetryExported";
    public const string SYSTEM_PlayerAfkDetected = "greg.SYSTEM.PlayerAfkDetected";
    public const string SYSTEM_PlayerAfkEnded = "greg.SYSTEM.PlayerAfkEnded";
    public const string SYSTEM_FrameSpikeDetected = "greg.SYSTEM.FrameSpikeDetected";

    // —— PerfCore Hooks ——
    public const string SYSTEM_FrameCapApplied = "greg.SYSTEM.FrameCapApplied";
    public const string SYSTEM_ThreadingConfigured = "greg.SYSTEM.ThreadingConfigured";
    public const string SYSTEM_PostFxOptimized = "greg.SYSTEM.PostFxOptimized";
    public const string SYSTEM_GcTriggered = "greg.SYSTEM.GcTriggered";

    // —— Framework-only (no matching entry in greg_hooks for this pipeline) ——
    public static readonly string SystemGameDayAdvanced = gregHookName.Create(GregDomain.System, "GameDayAdvanced");
    public static readonly string CustomerAppRequirementsSatisfied = gregHookName.Create(GregDomain.Customer, "AppRequirementsSatisfied");
    public static readonly string CustomerAppRequirementsFailed = gregHookName.Create(GregDomain.Customer, "AppRequirementsFailed");
    public static readonly string SystemGameSaved = gregHookName.Create(GregDomain.System, "GameSaved");
    public static readonly string SystemGameLoaded = gregHookName.Create(GregDomain.System, "GameLoaded");
    public static readonly string SystemAutoSaveRequested = gregHookName.Create(GregDomain.System, "AutoSaveRequested");
    public static readonly string NetworkNetWatchDispatched = gregHookName.Create(GregDomain.Network, "NetWatchDispatched");
    public static readonly string EmployeeCustomHired = gregHookName.Create(GregDomain.Employee, "CustomHired");
    public static readonly string EmployeeCustomFired = gregHookName.Create(GregDomain.Employee, "CustomFired");
    public static readonly string SystemHookBridgeInstalled = gregHookName.Create(GregDomain.System, "HookBridgeInstalled");
    public static readonly string SystemHookBridgeTriggered = gregHookName.Create(GregDomain.System, "HookBridgeTriggered");

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
            [EventIds.RatesCalculated] = SystemCalculateRates,
            [EventIds.BalanceSheetScreenOpened] = SystemButtonBalanceSheetScreen,
            [EventIds.BalanceSheetRecordAccessed] = UiGetOrCreateRecord,
            [EventIds.BalanceSheetTrackFinances] = SystemTrackFinances,
            [EventIds.BalanceSheetFilled] = SystemFillInBalanceSheet,
            [EventIds.BalanceSheetTotalRowAdded] = SystemTotalRowAdded,
            [EventIds.BalanceSheetSalaryRegistered] = SystemRegisterSalary,
            [EventIds.BalanceSheetRecordRestored] = SystemRestoreRecord,
            [EventIds.BalanceSheetDataSaved] = UiGetSaveData,
            [EventIds.BalanceSheetDataLoaded] = UiFromSaveLoaded,
            [EventIds.BalanceSheetLatestSnapshotRequested] = UiGetLatestSnapshot,
            [EventIds.ShopCartTotalUpdated] = SystemCartTotalChanged,
            [EventIds.ShopNewItemPurchased] = SystemNewItemPurchased,
            [EventIds.ShopAnotherItemPurchased] = SystemAnotherItemPurchased,
            [EventIds.ShopPhysicalItemSpawned] = SystemPhysicalItemSpawned,

            [EventIds.CustomerComponentInitialized] = CustomerComponentInitialized,
            [EventIds.CustomerServerCountAndSpeedChanged] = CustomerServerCountAndSpeedChanged,
            [EventIds.CustomerAppPerformanceAdded] = CustomerAppPerformanceAdded,
            [EventIds.CustomerAppSpeedsReset] = CustomerResetAllAppSpeeds,
            [EventIds.CustomerBaseSetup] = CustomerUpBaseSet,
            [EventIds.CustomerAppSetup] = CustomerUpAppSet,
            [EventIds.CustomerSpeedOnAppChanged] = CustomerSpeedOnCustomerBaseAppChanged,
            [EventIds.CustomerDataLoaded] = CustomerDataLoaded,
            [EventIds.CustomerDoorClicked] = CustomerInteractOnClick,
            [EventIds.CustomerDoorHovered] = CustomerInteractOnHover,
            [EventIds.CustomerDoorOpenedAndSetup] = CustomerOpenDoorAndSetupBase,
            [EventIds.CustomerDoorOpened] = CustomerOpenDoor,
            [EventIds.CustomerDoorLoaded] = CustomerOnLoad,
            [EventIds.CustomerDoorDestroyed] = CustomerOnDestroy,
            [EventIds.CustomerCardSet] = CustomerSet,
            [EventIds.CustomerChoiceCanceled] = SystemButtonCancelCustomerChoice,
            [EventIds.CustomerCardsCanvasShown] = SystemShowCustomerCardsCanvas,
            [EventIds.CustomerFallbackCreated] = SystemCreateFallbackCustomer,
            [EventIds.CustomerTotalRequirementRequested] = SystemGetCustomerTotalRequirement,
            [EventIds.CustomerSuitabilityChecked] = SystemIsCustomerSuitableForBase,
            [EventIds.NetworkCustomerBaseRegistered] = NetworkRegisterCustomerBase,
            [EventIds.NetworkCustomerBaseRequested] = NetworkGetCustomerBase,
            [EventIds.NetworkDeviceCustomerIdChanged] = NetworkDeviceCustomerIDChanged,
            [EventIds.ServerChangeCustomerClicked] = ServerButtonClickChangeCustomer,
            [EventIds.ServerNextCustomerIdRequested] = ServerGetNextCustomerID,
            [EventIds.ServerCustomerIdRequested] = ServerGetCustomerID,

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




