using System;
using System.Runtime.InteropServices;

namespace DataCenterModLoader;

public partial struct GameAPITable
{
    private int GetHashCodePart1()
    {
        var h = new HashCode();
        h.Add(ApiVersion);
        h.Add(LogInfo);
        h.Add(LogWarning);
        h.Add(LogError);
        h.Add(GetPlayerMoney);
        h.Add(SetPlayerMoney);
        h.Add(GetTimeScale);
        h.Add(SetTimeScale);
        h.Add(GetServerCount);
        h.Add(GetRackCount);
        h.Add(GetCurrentScene);
        h.Add(GetPlayerXP);
        h.Add(SetPlayerXP);
        h.Add(GetPlayerReputation);
        h.Add(SetPlayerReputation);
        h.Add(GetTimeOfDay);
        h.Add(GetDay);
        h.Add(GetSecondsInFullDay);
        h.Add(SetSecondsInFullDay);
        h.Add(GetSwitchCount);
        h.Add(GetSatisfiedCustomerCount);
        h.Add(SetNetWatchEnabled);
        h.Add(IsNetWatchEnabled);
        h.Add(GetNetWatchStats);
        h.Add(GetBrokenServerCount);
        h.Add(GetBrokenSwitchCount);
        h.Add(GetEolServerCount);
        h.Add(GetEolSwitchCount);
        h.Add(GetFreeTechnicianCount);
        h.Add(GetTotalTechnicianCount);
        h.Add(DispatchRepairServer);
        return h.ToHashCode();
    }

    private int GetHashCodePart2()
    {
        var h = new HashCode();
        h.Add(DispatchRepairSwitch);
        h.Add(DispatchReplaceServer);
        h.Add(DispatchReplaceSwitch);
        h.Add(RegisterCustomEmployee);
        h.Add(IsCustomEmployeeHired);
        h.Add(FireCustomEmployee);
        h.Add(RegisterSalary);
        h.Add(ShowNotification);
        h.Add(GetMoneyPerSecond);
        h.Add(GetExpensesPerSecond);
        h.Add(GetXpPerSecond);
        h.Add(IsGamePaused);
        h.Add(SetGamePaused);
        h.Add(GetDifficulty);
        h.Add(TriggerSave);
        h.Add(SteamGetMyId);
        h.Add(SteamGetFriendName);
        h.Add(SteamCreateLobby);
        h.Add(SteamJoinLobby);
        h.Add(SteamLeaveLobby);
        h.Add(SteamGetLobbyId);
        h.Add(SteamGetLobbyOwner);
        h.Add(SteamGetLobbyMemberCount);
        h.Add(SteamGetLobbyMemberByIndex);
        h.Add(SteamSetLobbyData);
        h.Add(SteamGetLobbyData);
        h.Add(SteamSendP2P);
        h.Add(SteamIsP2PAvailable);
        h.Add(SteamReadP2P);
        h.Add(SteamAcceptP2P);
        h.Add(SteamPollEvent);
        return h.ToHashCode();
    }

    private int GetHashCodePart3()
    {
        var h = new HashCode();
        h.Add(GetPlayerPosition);
        h.Add(ConfigRegisterBool);
        h.Add(ConfigRegisterInt);
        h.Add(ConfigRegisterFloat);
        h.Add(ConfigGetBool);
        h.Add(ConfigGetInt);
        h.Add(ConfigGetFloat);
        h.Add(SpawnCharacter);
        h.Add(DestroyEntity);
        h.Add(SetEntityPosition);
        h.Add(IsEntityReady);
        h.Add(SetEntityAnimation);
        h.Add(GetPrefabCount);
        h.Add(SetEntityName);
        h.Add(GetPlayerCarryState);
        h.Add(GetPlayerCrouching);
        h.Add(GetPlayerSitting);
        h.Add(SetEntityCrouching);
        h.Add(SetEntitySitting);
        h.Add(SetEntityCarryAnim);
        h.Add(CreateEntityCarryVisual);
        h.Add(DestroyEntityCarryVisual);
        h.Add(GetDefaultSpawnPosition);
        h.Add(WarpLocalPlayer);
        h.Add(GetEntityPosition);
        h.Add(AddEntityCollider);
        h.Add(SetEntityCarryTransform);
        h.Add(WorldGetObjectCount);
        h.Add(WorldGetObjectHashes);
        h.Add(WorldGetObjectState);
        h.Add(WorldSpawnObject);
        return h.ToHashCode();
    }

    private int GetHashCodePart4()
    {
        var h = new HashCode();
        h.Add(WorldDestroyObject);
        h.Add(WorldPlaceInRack);
        h.Add(WorldRemoveFromRack);
        h.Add(WorldSetPower);
        h.Add(WorldSetProperty);
        h.Add(WorldConnectCable);
        h.Add(WorldDisconnectCable);
        h.Add(WorldPickupObject);
        h.Add(WorldDropObject);
        h.Add(WorldEnsureRackUIDs);
        h.Add(ObjFindByType);
        h.Add(ObjGetStringField);
        h.Add(ObjIsActive);
        h.Add(ObjSetActive);
        h.Add(ObjGetPosition);
        h.Add(ObjSetPosition);
        h.Add(ObjSetRotation);
        h.Add(ObjSetParentToWorld);
        h.Add(RbSetKinematic);
        h.Add(RbSetGravity);
        h.Add(RbWakeUp);
        h.Add(ObjFindById);
        h.Add(GetHeldObject);
        h.Add(ObjGetRotation);
        h.Add(ObjSetParent);
        h.Add(ObjSetLocalPosition);
        h.Add(ObjSetLocalRotation);
        h.Add(RackFindPosition);
        h.Add(RackGameInstall);
        h.Add(RackGameUninstall);
        h.Add(ObjSetStringField);
        return h.ToHashCode();
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(GetHashCodePart1(), GetHashCodePart2(),
            GetHashCodePart3(), GetHashCodePart4());
    }

    public static bool operator ==(GameAPITable left, GameAPITable right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(GameAPITable left, GameAPITable right)
    {
        return !left.Equals(right);
    }
}
