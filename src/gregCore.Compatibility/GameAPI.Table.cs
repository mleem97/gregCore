using System;
using System.Runtime.InteropServices;

namespace DataCenterModLoader;

// ABI-CRITICAL: sequential layout mirrors the native table (see GameAPI.Bind.cs).
// Equality members stay in this part (single declaration scope); chunk helpers
// live in GameAPI.TableHash.cs as static methods.
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

    public bool Equals(GameAPITable other)
    {
        return EqualsPart1(this, other) && EqualsPart2(this, other) && EqualsPart3(this, other) && EqualsPart4(this, other);
    }

    public override bool Equals(object obj)
    {
        return obj is GameAPITable other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(HashPart1(this), HashPart2(this), HashPart3(this), HashPart4(this));
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
