using System;
using System.Runtime.InteropServices;

namespace DataCenterModLoader;

public partial struct GameAPITable
{
    private static bool EqualsPart1(GameAPITable left, GameAPITable right)
    {
        return left.ApiVersion == right.ApiVersion &&
                left.LogInfo == right.LogInfo &&
                left.LogWarning == right.LogWarning &&
                left.LogError == right.LogError &&
                left.GetPlayerMoney == right.GetPlayerMoney &&
                left.SetPlayerMoney == right.SetPlayerMoney &&
                left.GetTimeScale == right.GetTimeScale &&
                left.SetTimeScale == right.SetTimeScale &&
                left.GetServerCount == right.GetServerCount &&
                left.GetRackCount == right.GetRackCount &&
                left.GetCurrentScene == right.GetCurrentScene &&
                left.GetPlayerXP == right.GetPlayerXP &&
                left.SetPlayerXP == right.SetPlayerXP &&
                left.GetPlayerReputation == right.GetPlayerReputation &&
                left.SetPlayerReputation == right.SetPlayerReputation &&
                left.GetTimeOfDay == right.GetTimeOfDay &&
                left.GetDay == right.GetDay &&
                left.GetSecondsInFullDay == right.GetSecondsInFullDay &&
                left.SetSecondsInFullDay == right.SetSecondsInFullDay &&
                left.GetSwitchCount == right.GetSwitchCount &&
                left.GetSatisfiedCustomerCount == right.GetSatisfiedCustomerCount &&
                left.SetNetWatchEnabled == right.SetNetWatchEnabled &&
                left.IsNetWatchEnabled == right.IsNetWatchEnabled &&
                left.GetNetWatchStats == right.GetNetWatchStats &&
                left.GetBrokenServerCount == right.GetBrokenServerCount &&
                left.GetBrokenSwitchCount == right.GetBrokenSwitchCount &&
                left.GetEolServerCount == right.GetEolServerCount &&
                left.GetEolSwitchCount == right.GetEolSwitchCount &&
                left.GetFreeTechnicianCount == right.GetFreeTechnicianCount &&
                left.GetTotalTechnicianCount == right.GetTotalTechnicianCount &&
                left.DispatchRepairServer == right.DispatchRepairServer;
    }

    private static bool EqualsPart2(GameAPITable left, GameAPITable right)
    {
        return left.DispatchRepairSwitch == right.DispatchRepairSwitch &&
                left.DispatchReplaceServer == right.DispatchReplaceServer &&
                left.DispatchReplaceSwitch == right.DispatchReplaceSwitch &&
                left.RegisterCustomEmployee == right.RegisterCustomEmployee &&
                left.IsCustomEmployeeHired == right.IsCustomEmployeeHired &&
                left.FireCustomEmployee == right.FireCustomEmployee &&
                left.RegisterSalary == right.RegisterSalary &&
                left.ShowNotification == right.ShowNotification &&
                left.GetMoneyPerSecond == right.GetMoneyPerSecond &&
                left.GetExpensesPerSecond == right.GetExpensesPerSecond &&
                left.GetXpPerSecond == right.GetXpPerSecond &&
                left.IsGamePaused == right.IsGamePaused &&
                left.SetGamePaused == right.SetGamePaused &&
                left.GetDifficulty == right.GetDifficulty &&
                left.TriggerSave == right.TriggerSave &&
                left.SteamGetMyId == right.SteamGetMyId &&
                left.SteamGetFriendName == right.SteamGetFriendName &&
                left.SteamCreateLobby == right.SteamCreateLobby &&
                left.SteamJoinLobby == right.SteamJoinLobby &&
                left.SteamLeaveLobby == right.SteamLeaveLobby &&
                left.SteamGetLobbyId == right.SteamGetLobbyId &&
                left.SteamGetLobbyOwner == right.SteamGetLobbyOwner &&
                left.SteamGetLobbyMemberCount == right.SteamGetLobbyMemberCount &&
                left.SteamGetLobbyMemberByIndex == right.SteamGetLobbyMemberByIndex &&
                left.SteamSetLobbyData == right.SteamSetLobbyData &&
                left.SteamGetLobbyData == right.SteamGetLobbyData &&
                left.SteamSendP2P == right.SteamSendP2P &&
                left.SteamIsP2PAvailable == right.SteamIsP2PAvailable &&
                left.SteamReadP2P == right.SteamReadP2P &&
                left.SteamAcceptP2P == right.SteamAcceptP2P &&
                left.SteamPollEvent == right.SteamPollEvent;
    }

    private static bool EqualsPart3(GameAPITable left, GameAPITable right)
    {
        return left.GetPlayerPosition == right.GetPlayerPosition &&
                left.ConfigRegisterBool == right.ConfigRegisterBool &&
                left.ConfigRegisterInt == right.ConfigRegisterInt &&
                left.ConfigRegisterFloat == right.ConfigRegisterFloat &&
                left.ConfigGetBool == right.ConfigGetBool &&
                left.ConfigGetInt == right.ConfigGetInt &&
                left.ConfigGetFloat == right.ConfigGetFloat &&
                left.SpawnCharacter == right.SpawnCharacter &&
                left.DestroyEntity == right.DestroyEntity &&
                left.SetEntityPosition == right.SetEntityPosition &&
                left.IsEntityReady == right.IsEntityReady &&
                left.SetEntityAnimation == right.SetEntityAnimation &&
                left.GetPrefabCount == right.GetPrefabCount &&
                left.SetEntityName == right.SetEntityName &&
                left.GetPlayerCarryState == right.GetPlayerCarryState &&
                left.GetPlayerCrouching == right.GetPlayerCrouching &&
                left.GetPlayerSitting == right.GetPlayerSitting &&
                left.SetEntityCrouching == right.SetEntityCrouching &&
                left.SetEntitySitting == right.SetEntitySitting &&
                left.SetEntityCarryAnim == right.SetEntityCarryAnim &&
                left.CreateEntityCarryVisual == right.CreateEntityCarryVisual &&
                left.DestroyEntityCarryVisual == right.DestroyEntityCarryVisual &&
                left.GetDefaultSpawnPosition == right.GetDefaultSpawnPosition &&
                left.WarpLocalPlayer == right.WarpLocalPlayer &&
                left.GetEntityPosition == right.GetEntityPosition &&
                left.AddEntityCollider == right.AddEntityCollider &&
                left.SetEntityCarryTransform == right.SetEntityCarryTransform &&
                left.WorldGetObjectCount == right.WorldGetObjectCount &&
                left.WorldGetObjectHashes == right.WorldGetObjectHashes &&
                left.WorldGetObjectState == right.WorldGetObjectState &&
                left.WorldSpawnObject == right.WorldSpawnObject;
    }

    private static bool EqualsPart4(GameAPITable left, GameAPITable right)
    {
        return left.WorldDestroyObject == right.WorldDestroyObject &&
                left.WorldPlaceInRack == right.WorldPlaceInRack &&
                left.WorldRemoveFromRack == right.WorldRemoveFromRack &&
                left.WorldSetPower == right.WorldSetPower &&
                left.WorldSetProperty == right.WorldSetProperty &&
                left.WorldConnectCable == right.WorldConnectCable &&
                left.WorldDisconnectCable == right.WorldDisconnectCable &&
                left.WorldPickupObject == right.WorldPickupObject &&
                left.WorldDropObject == right.WorldDropObject &&
                left.WorldEnsureRackUIDs == right.WorldEnsureRackUIDs &&
                left.ObjFindByType == right.ObjFindByType &&
                left.ObjGetStringField == right.ObjGetStringField &&
                left.ObjIsActive == right.ObjIsActive &&
                left.ObjSetActive == right.ObjSetActive &&
                left.ObjGetPosition == right.ObjGetPosition &&
                left.ObjSetPosition == right.ObjSetPosition &&
                left.ObjSetRotation == right.ObjSetRotation &&
                left.ObjSetParentToWorld == right.ObjSetParentToWorld &&
                left.RbSetKinematic == right.RbSetKinematic &&
                left.RbSetGravity == right.RbSetGravity &&
                left.RbWakeUp == right.RbWakeUp &&
                left.ObjFindById == right.ObjFindById &&
                left.GetHeldObject == right.GetHeldObject &&
                left.ObjGetRotation == right.ObjGetRotation &&
                left.ObjSetParent == right.ObjSetParent &&
                left.ObjSetLocalPosition == right.ObjSetLocalPosition &&
                left.ObjSetLocalRotation == right.ObjSetLocalRotation &&
                left.RackFindPosition == right.RackFindPosition &&
                left.RackGameInstall == right.RackGameInstall &&
                left.RackGameUninstall == right.RackGameUninstall &&
                left.ObjSetStringField == right.ObjSetStringField;
    }

    private static int HashPart1(GameAPITable t)
    {
        var h = new HashCode();
        h.Add(t.ApiVersion);
        h.Add(t.LogInfo);
        h.Add(t.LogWarning);
        h.Add(t.LogError);
        h.Add(t.GetPlayerMoney);
        h.Add(t.SetPlayerMoney);
        h.Add(t.GetTimeScale);
        h.Add(t.SetTimeScale);
        h.Add(t.GetServerCount);
        h.Add(t.GetRackCount);
        h.Add(t.GetCurrentScene);
        h.Add(t.GetPlayerXP);
        h.Add(t.SetPlayerXP);
        h.Add(t.GetPlayerReputation);
        h.Add(t.SetPlayerReputation);
        h.Add(t.GetTimeOfDay);
        h.Add(t.GetDay);
        h.Add(t.GetSecondsInFullDay);
        h.Add(t.SetSecondsInFullDay);
        h.Add(t.GetSwitchCount);
        h.Add(t.GetSatisfiedCustomerCount);
        h.Add(t.SetNetWatchEnabled);
        h.Add(t.IsNetWatchEnabled);
        h.Add(t.GetNetWatchStats);
        h.Add(t.GetBrokenServerCount);
        h.Add(t.GetBrokenSwitchCount);
        h.Add(t.GetEolServerCount);
        h.Add(t.GetEolSwitchCount);
        h.Add(t.GetFreeTechnicianCount);
        h.Add(t.GetTotalTechnicianCount);
        h.Add(t.DispatchRepairServer);
        return h.ToHashCode();
    }

    private static int HashPart2(GameAPITable t)
    {
        var h = new HashCode();
        h.Add(t.DispatchRepairSwitch);
        h.Add(t.DispatchReplaceServer);
        h.Add(t.DispatchReplaceSwitch);
        h.Add(t.RegisterCustomEmployee);
        h.Add(t.IsCustomEmployeeHired);
        h.Add(t.FireCustomEmployee);
        h.Add(t.RegisterSalary);
        h.Add(t.ShowNotification);
        h.Add(t.GetMoneyPerSecond);
        h.Add(t.GetExpensesPerSecond);
        h.Add(t.GetXpPerSecond);
        h.Add(t.IsGamePaused);
        h.Add(t.SetGamePaused);
        h.Add(t.GetDifficulty);
        h.Add(t.TriggerSave);
        h.Add(t.SteamGetMyId);
        h.Add(t.SteamGetFriendName);
        h.Add(t.SteamCreateLobby);
        h.Add(t.SteamJoinLobby);
        h.Add(t.SteamLeaveLobby);
        h.Add(t.SteamGetLobbyId);
        h.Add(t.SteamGetLobbyOwner);
        h.Add(t.SteamGetLobbyMemberCount);
        h.Add(t.SteamGetLobbyMemberByIndex);
        h.Add(t.SteamSetLobbyData);
        h.Add(t.SteamGetLobbyData);
        h.Add(t.SteamSendP2P);
        h.Add(t.SteamIsP2PAvailable);
        h.Add(t.SteamReadP2P);
        h.Add(t.SteamAcceptP2P);
        h.Add(t.SteamPollEvent);
        return h.ToHashCode();
    }

    private static int HashPart3(GameAPITable t)
    {
        var h = new HashCode();
        h.Add(t.GetPlayerPosition);
        h.Add(t.ConfigRegisterBool);
        h.Add(t.ConfigRegisterInt);
        h.Add(t.ConfigRegisterFloat);
        h.Add(t.ConfigGetBool);
        h.Add(t.ConfigGetInt);
        h.Add(t.ConfigGetFloat);
        h.Add(t.SpawnCharacter);
        h.Add(t.DestroyEntity);
        h.Add(t.SetEntityPosition);
        h.Add(t.IsEntityReady);
        h.Add(t.SetEntityAnimation);
        h.Add(t.GetPrefabCount);
        h.Add(t.SetEntityName);
        h.Add(t.GetPlayerCarryState);
        h.Add(t.GetPlayerCrouching);
        h.Add(t.GetPlayerSitting);
        h.Add(t.SetEntityCrouching);
        h.Add(t.SetEntitySitting);
        h.Add(t.SetEntityCarryAnim);
        h.Add(t.CreateEntityCarryVisual);
        h.Add(t.DestroyEntityCarryVisual);
        h.Add(t.GetDefaultSpawnPosition);
        h.Add(t.WarpLocalPlayer);
        h.Add(t.GetEntityPosition);
        h.Add(t.AddEntityCollider);
        h.Add(t.SetEntityCarryTransform);
        h.Add(t.WorldGetObjectCount);
        h.Add(t.WorldGetObjectHashes);
        h.Add(t.WorldGetObjectState);
        h.Add(t.WorldSpawnObject);
        return h.ToHashCode();
    }

    private static int HashPart4(GameAPITable t)
    {
        var h = new HashCode();
        h.Add(t.WorldDestroyObject);
        h.Add(t.WorldPlaceInRack);
        h.Add(t.WorldRemoveFromRack);
        h.Add(t.WorldSetPower);
        h.Add(t.WorldSetProperty);
        h.Add(t.WorldConnectCable);
        h.Add(t.WorldDisconnectCable);
        h.Add(t.WorldPickupObject);
        h.Add(t.WorldDropObject);
        h.Add(t.WorldEnsureRackUIDs);
        h.Add(t.ObjFindByType);
        h.Add(t.ObjGetStringField);
        h.Add(t.ObjIsActive);
        h.Add(t.ObjSetActive);
        h.Add(t.ObjGetPosition);
        h.Add(t.ObjSetPosition);
        h.Add(t.ObjSetRotation);
        h.Add(t.ObjSetParentToWorld);
        h.Add(t.RbSetKinematic);
        h.Add(t.RbSetGravity);
        h.Add(t.RbWakeUp);
        h.Add(t.ObjFindById);
        h.Add(t.GetHeldObject);
        h.Add(t.ObjGetRotation);
        h.Add(t.ObjSetParent);
        h.Add(t.ObjSetLocalPosition);
        h.Add(t.ObjSetLocalRotation);
        h.Add(t.RackFindPosition);
        h.Add(t.RackGameInstall);
        h.Add(t.RackGameUninstall);
        h.Add(t.ObjSetStringField);
        return h.ToHashCode();
    }
}
