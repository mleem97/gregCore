using System;
using System.Runtime.InteropServices;

namespace DataCenterModLoader;

[StructLayout(LayoutKind.Sequential)]
public partial struct GameAPITable : IEquatable<GameAPITable>
{
    public uint ApiVersion;
    public IntPtr LogInfo;
    public IntPtr LogWarning;
    public IntPtr LogError;
    public IntPtr GetPlayerMoney;
    public IntPtr SetPlayerMoney;
    public IntPtr GetTimeScale;
    public IntPtr SetTimeScale;
    public IntPtr GetServerCount;
    public IntPtr GetRackCount;
    public IntPtr GetCurrentScene;
    public IntPtr GetPlayerXP;
    public IntPtr SetPlayerXP;
    public IntPtr GetPlayerReputation;
    public IntPtr SetPlayerReputation;
    public IntPtr GetTimeOfDay;
    public IntPtr GetDay;
    public IntPtr GetSecondsInFullDay;
    public IntPtr SetSecondsInFullDay;
    public IntPtr GetSwitchCount;
    public IntPtr GetSatisfiedCustomerCount;
    public IntPtr SetNetWatchEnabled;
    public IntPtr IsNetWatchEnabled;
    public IntPtr GetNetWatchStats;
    public IntPtr GetBrokenServerCount;
    public IntPtr GetBrokenSwitchCount;
    public IntPtr GetEolServerCount;
    public IntPtr GetEolSwitchCount;
    public IntPtr GetFreeTechnicianCount;
    public IntPtr GetTotalTechnicianCount;
    public IntPtr DispatchRepairServer;
    public IntPtr DispatchRepairSwitch;
    public IntPtr DispatchReplaceServer;
    public IntPtr DispatchReplaceSwitch;
    public IntPtr RegisterCustomEmployee;
    public IntPtr IsCustomEmployeeHired;
    public IntPtr FireCustomEmployee;
    public IntPtr RegisterSalary;
    public IntPtr ShowNotification;
    public IntPtr GetMoneyPerSecond;
    public IntPtr GetExpensesPerSecond;
    public IntPtr GetXpPerSecond;
    public IntPtr IsGamePaused;
    public IntPtr SetGamePaused;
    public IntPtr GetDifficulty;
    public IntPtr TriggerSave;
    public IntPtr SteamGetMyId;
    public IntPtr SteamGetFriendName;
    public IntPtr SteamCreateLobby;
    public IntPtr SteamJoinLobby;
    public IntPtr SteamLeaveLobby;
    public IntPtr SteamGetLobbyId;
    public IntPtr SteamGetLobbyOwner;
    public IntPtr SteamGetLobbyMemberCount;
    public IntPtr SteamGetLobbyMemberByIndex;
    public IntPtr SteamSetLobbyData;
    public IntPtr SteamGetLobbyData;
    public IntPtr SteamSendP2P;
    public IntPtr SteamIsP2PAvailable;
    public IntPtr SteamReadP2P;
    public IntPtr SteamAcceptP2P;
    public IntPtr SteamPollEvent;
    public IntPtr GetPlayerPosition;
    public IntPtr ConfigRegisterBool;
    public IntPtr ConfigRegisterInt;
    public IntPtr ConfigRegisterFloat;
    public IntPtr ConfigGetBool;
    public IntPtr ConfigGetInt;
    public IntPtr ConfigGetFloat;
    public IntPtr SpawnCharacter;
    public IntPtr DestroyEntity;
    public IntPtr SetEntityPosition;
    public IntPtr IsEntityReady;
    public IntPtr SetEntityAnimation;
    public IntPtr GetPrefabCount;
    public IntPtr SetEntityName;
    public IntPtr GetPlayerCarryState;
    public IntPtr GetPlayerCrouching;
    public IntPtr GetPlayerSitting;
    public IntPtr SetEntityCrouching;
    public IntPtr SetEntitySitting;
    public IntPtr SetEntityCarryAnim;
    public IntPtr CreateEntityCarryVisual;
    public IntPtr DestroyEntityCarryVisual;
    public IntPtr GetDefaultSpawnPosition;
    public IntPtr WarpLocalPlayer;
    public IntPtr GetEntityPosition;
    public IntPtr AddEntityCollider;
    public IntPtr SetEntityCarryTransform;
    public IntPtr WorldGetObjectCount;
    public IntPtr WorldGetObjectHashes;
    public IntPtr WorldGetObjectState;
    public IntPtr WorldSpawnObject;
    public IntPtr WorldDestroyObject;
    public IntPtr WorldPlaceInRack;
    public IntPtr WorldRemoveFromRack;
    public IntPtr WorldSetPower;
    public IntPtr WorldSetProperty;
    public IntPtr WorldConnectCable;
    public IntPtr WorldDisconnectCable;
    public IntPtr WorldPickupObject;
    public IntPtr WorldDropObject;
    public IntPtr WorldEnsureRackUIDs;
    public IntPtr ObjFindByType;
    public IntPtr ObjGetStringField;
    public IntPtr ObjIsActive;
    public IntPtr ObjSetActive;
    public IntPtr ObjGetPosition;
    public IntPtr ObjSetPosition;
    public IntPtr ObjSetRotation;
    public IntPtr ObjSetParentToWorld;
    public IntPtr RbSetKinematic;
    public IntPtr RbSetGravity;
    public IntPtr RbWakeUp;
    public IntPtr ObjFindById;
    public IntPtr GetHeldObject;
    public IntPtr ObjGetRotation;
    public IntPtr ObjSetParent;
    public IntPtr ObjSetLocalPosition;
    public IntPtr ObjSetLocalRotation;
    public IntPtr RackFindPosition;
    public IntPtr RackGameInstall;
    public IntPtr RackGameUninstall;
    public IntPtr ObjSetStringField;

    private bool EqualsPart1(GameAPITable other)
    {
        return ApiVersion == other.ApiVersion &&
                LogInfo == other.LogInfo &&
                LogWarning == other.LogWarning &&
                LogError == other.LogError &&
                GetPlayerMoney == other.GetPlayerMoney &&
                SetPlayerMoney == other.SetPlayerMoney &&
                GetTimeScale == other.GetTimeScale &&
                SetTimeScale == other.SetTimeScale &&
                GetServerCount == other.GetServerCount &&
                GetRackCount == other.GetRackCount &&
                GetCurrentScene == other.GetCurrentScene &&
                GetPlayerXP == other.GetPlayerXP &&
                SetPlayerXP == other.SetPlayerXP &&
                GetPlayerReputation == other.GetPlayerReputation &&
                SetPlayerReputation == other.SetPlayerReputation &&
                GetTimeOfDay == other.GetTimeOfDay &&
                GetDay == other.GetDay &&
                GetSecondsInFullDay == other.GetSecondsInFullDay &&
                SetSecondsInFullDay == other.SetSecondsInFullDay &&
                GetSwitchCount == other.GetSwitchCount &&
                GetSatisfiedCustomerCount == other.GetSatisfiedCustomerCount &&
                SetNetWatchEnabled == other.SetNetWatchEnabled &&
                IsNetWatchEnabled == other.IsNetWatchEnabled &&
                GetNetWatchStats == other.GetNetWatchStats &&
                GetBrokenServerCount == other.GetBrokenServerCount &&
                GetBrokenSwitchCount == other.GetBrokenSwitchCount &&
                GetEolServerCount == other.GetEolServerCount &&
                GetEolSwitchCount == other.GetEolSwitchCount &&
                GetFreeTechnicianCount == other.GetFreeTechnicianCount &&
                GetTotalTechnicianCount == other.GetTotalTechnicianCount &&
                DispatchRepairServer == other.DispatchRepairServer;
    }

    private bool EqualsPart2(GameAPITable other)
    {
        return DispatchRepairSwitch == other.DispatchRepairSwitch &&
                DispatchReplaceServer == other.DispatchReplaceServer &&
                DispatchReplaceSwitch == other.DispatchReplaceSwitch &&
                RegisterCustomEmployee == other.RegisterCustomEmployee &&
                IsCustomEmployeeHired == other.IsCustomEmployeeHired &&
                FireCustomEmployee == other.FireCustomEmployee &&
                RegisterSalary == other.RegisterSalary &&
                ShowNotification == other.ShowNotification &&
                GetMoneyPerSecond == other.GetMoneyPerSecond &&
                GetExpensesPerSecond == other.GetExpensesPerSecond &&
                GetXpPerSecond == other.GetXpPerSecond &&
                IsGamePaused == other.IsGamePaused &&
                SetGamePaused == other.SetGamePaused &&
                GetDifficulty == other.GetDifficulty &&
                TriggerSave == other.TriggerSave &&
                SteamGetMyId == other.SteamGetMyId &&
                SteamGetFriendName == other.SteamGetFriendName &&
                SteamCreateLobby == other.SteamCreateLobby &&
                SteamJoinLobby == other.SteamJoinLobby &&
                SteamLeaveLobby == other.SteamLeaveLobby &&
                SteamGetLobbyId == other.SteamGetLobbyId &&
                SteamGetLobbyOwner == other.SteamGetLobbyOwner &&
                SteamGetLobbyMemberCount == other.SteamGetLobbyMemberCount &&
                SteamGetLobbyMemberByIndex == other.SteamGetLobbyMemberByIndex &&
                SteamSetLobbyData == other.SteamSetLobbyData &&
                SteamGetLobbyData == other.SteamGetLobbyData &&
                SteamSendP2P == other.SteamSendP2P &&
                SteamIsP2PAvailable == other.SteamIsP2PAvailable &&
                SteamReadP2P == other.SteamReadP2P &&
                SteamAcceptP2P == other.SteamAcceptP2P &&
                SteamPollEvent == other.SteamPollEvent;
    }

    private bool EqualsPart3(GameAPITable other)
    {
        return GetPlayerPosition == other.GetPlayerPosition &&
                ConfigRegisterBool == other.ConfigRegisterBool &&
                ConfigRegisterInt == other.ConfigRegisterInt &&
                ConfigRegisterFloat == other.ConfigRegisterFloat &&
                ConfigGetBool == other.ConfigGetBool &&
                ConfigGetInt == other.ConfigGetInt &&
                ConfigGetFloat == other.ConfigGetFloat &&
                SpawnCharacter == other.SpawnCharacter &&
                DestroyEntity == other.DestroyEntity &&
                SetEntityPosition == other.SetEntityPosition &&
                IsEntityReady == other.IsEntityReady &&
                SetEntityAnimation == other.SetEntityAnimation &&
                GetPrefabCount == other.GetPrefabCount &&
                SetEntityName == other.SetEntityName &&
                GetPlayerCarryState == other.GetPlayerCarryState &&
                GetPlayerCrouching == other.GetPlayerCrouching &&
                GetPlayerSitting == other.GetPlayerSitting &&
                SetEntityCrouching == other.SetEntityCrouching &&
                SetEntitySitting == other.SetEntitySitting &&
                SetEntityCarryAnim == other.SetEntityCarryAnim &&
                CreateEntityCarryVisual == other.CreateEntityCarryVisual &&
                DestroyEntityCarryVisual == other.DestroyEntityCarryVisual &&
                GetDefaultSpawnPosition == other.GetDefaultSpawnPosition &&
                WarpLocalPlayer == other.WarpLocalPlayer &&
                GetEntityPosition == other.GetEntityPosition &&
                AddEntityCollider == other.AddEntityCollider &&
                SetEntityCarryTransform == other.SetEntityCarryTransform &&
                WorldGetObjectCount == other.WorldGetObjectCount &&
                WorldGetObjectHashes == other.WorldGetObjectHashes &&
                WorldGetObjectState == other.WorldGetObjectState &&
                WorldSpawnObject == other.WorldSpawnObject;
    }

    private bool EqualsPart4(GameAPITable other)
    {
        return WorldDestroyObject == other.WorldDestroyObject &&
                WorldPlaceInRack == other.WorldPlaceInRack &&
                WorldRemoveFromRack == other.WorldRemoveFromRack &&
                WorldSetPower == other.WorldSetPower &&
                WorldSetProperty == other.WorldSetProperty &&
                WorldConnectCable == other.WorldConnectCable &&
                WorldDisconnectCable == other.WorldDisconnectCable &&
                WorldPickupObject == other.WorldPickupObject &&
                WorldDropObject == other.WorldDropObject &&
                WorldEnsureRackUIDs == other.WorldEnsureRackUIDs &&
                ObjFindByType == other.ObjFindByType &&
                ObjGetStringField == other.ObjGetStringField &&
                ObjIsActive == other.ObjIsActive &&
                ObjSetActive == other.ObjSetActive &&
                ObjGetPosition == other.ObjGetPosition &&
                ObjSetPosition == other.ObjSetPosition &&
                ObjSetRotation == other.ObjSetRotation &&
                ObjSetParentToWorld == other.ObjSetParentToWorld &&
                RbSetKinematic == other.RbSetKinematic &&
                RbSetGravity == other.RbSetGravity &&
                RbWakeUp == other.RbWakeUp &&
                ObjFindById == other.ObjFindById &&
                GetHeldObject == other.GetHeldObject &&
                ObjGetRotation == other.ObjGetRotation &&
                ObjSetParent == other.ObjSetParent &&
                ObjSetLocalPosition == other.ObjSetLocalPosition &&
                ObjSetLocalRotation == other.ObjSetLocalRotation &&
                RackFindPosition == other.RackFindPosition &&
                RackGameInstall == other.RackGameInstall &&
                RackGameUninstall == other.RackGameUninstall &&
                ObjSetStringField == other.ObjSetStringField;
    }

    public bool Equals(GameAPITable other)
    {
        return EqualsPart1(other) && EqualsPart2(other)
            && EqualsPart3(other) && EqualsPart4(other);
    }

    public override bool Equals(object obj)
    {
        return obj is GameAPITable other && Equals(other);
    }
}
