using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Il2Cpp;
using MelonLoader;
using UnityEngine;
using gregCore.API;

namespace DataCenterModLoader;

// function pointer table for rust mods, append-only
[StructLayout(LayoutKind.Sequential)]
public struct GameAPITable : IEquatable<GameAPITable>
{
    // v1
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

    // v2
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

    // v3
    public IntPtr SetNetWatchEnabled;
    public IntPtr IsNetWatchEnabled;
    public IntPtr GetNetWatchStats;

    // v4
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

    // v5
    public IntPtr RegisterCustomEmployee;
    public IntPtr IsCustomEmployeeHired;
    public IntPtr FireCustomEmployee;
    public IntPtr RegisterSalary;

    // v6
    public IntPtr ShowNotification;
    public IntPtr GetMoneyPerSecond;
    public IntPtr GetExpensesPerSecond;
    public IntPtr GetXpPerSecond;
    public IntPtr IsGamePaused;
    public IntPtr SetGamePaused;
    public IntPtr GetDifficulty;
    public IntPtr TriggerSave;

    // v7 - Legacy ABI slots. Native Data Center co-op owns lobby/session state;
    // these positions remain stable for older Rust plugins and are no-op where
    // the old custom implementation had no native backing.
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

    // v8 - Mod Configuration
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

    // v13 - World Object Sync
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
        return (ApiVersion == other.ApiVersion &&
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
                DispatchRepairServer == other.DispatchRepairServer &&
                DispatchRepairSwitch == other.DispatchRepairSwitch &&
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
                SteamPollEvent == other.SteamPollEvent &&
                GetPlayerPosition == other.GetPlayerPosition &&
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
                WorldSpawnObject == other.WorldSpawnObject &&
                WorldDestroyObject == other.WorldDestroyObject &&
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
                ObjSetStringField == other.ObjSetStringField);
    }

    public override bool Equals(object obj)
    {
        return obj is GameAPITable other && Equals(other);
    }

    public override int GetHashCode()
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

}

public partial class GameAPIManager : IDisposable
{
    public static uint API_VERSION { get; } = 19;

    private IntPtr _tablePtr;
    private GameAPITable _table;

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)] private delegate void LogDelegate(IntPtr message);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)] private delegate double GetDoubleDelegate();
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)] private delegate void SetDoubleDelegate(double value);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)] private delegate float GetFloatDelegate();
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)] private delegate void SetFloatDelegate(float value);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)] private delegate uint GetUIntDelegate();
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)] private delegate void SetUIntDelegate(uint value);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)] private delegate IntPtr GetStringDelegate();
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)] private delegate int GetIntDelegate();

    // v7 delegate types
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)] private delegate ulong GetULongDelegate();
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)] private delegate IntPtr GetStringFromU64Delegate(ulong steamId);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)] private delegate int CreateLobbyDelegate(uint lobbyType, uint maxPlayers);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)] private delegate int JoinLobbyDelegate(ulong lobbyId);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)] private delegate void VoidDelegate();
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)] private delegate ulong GetLobbyMemberDelegate(uint index);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)] private delegate int SetLobbyDataDelegate(IntPtr key, IntPtr value);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)] private delegate IntPtr GetLobbyDataDelegate(IntPtr key);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)] private delegate int SendP2PDelegate(ulong target, IntPtr data, uint len, uint reliable);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)] private delegate uint IsP2PAvailableDelegate(IntPtr outSize);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)] private delegate uint ReadP2PDelegate(IntPtr buf, uint bufLen, IntPtr outSender);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)] private delegate void AcceptP2PDelegate(ulong remote);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)] private delegate uint PollEventDelegate(IntPtr outType, IntPtr outData);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)] private delegate void GetPlayerPositionDelegate(IntPtr outX, IntPtr outY, IntPtr outZ, IntPtr outRy);

    // v8 delegate types
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)] private delegate uint ConfigRegisterBoolDelegate(IntPtr modId, IntPtr key, IntPtr displayName, uint defaultValue, IntPtr description);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)] private delegate uint ConfigRegisterIntDelegate(IntPtr modId, IntPtr key, IntPtr displayName, int defaultValue, int min, int max, IntPtr description);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)] private delegate uint ConfigRegisterFloatDelegate(IntPtr modId, IntPtr key, IntPtr displayName, float defaultValue, float min, float max, IntPtr description);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)] private delegate uint ConfigGetBoolDelegate(IntPtr modId, IntPtr key);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)] private delegate int ConfigGetIntDelegate(IntPtr modId, IntPtr key);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)] private delegate float ConfigGetFloatDelegate(IntPtr modId, IntPtr key);

    // v9 delegate types
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)] private delegate uint SpawnCharacterDelegate(uint prefabIdx, float x, float y, float z, float rotY, IntPtr name);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)] private delegate void DestroyEntityDelegate(uint entityId);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)] private delegate void SetEntityPositionDelegate(uint entityId, float x, float y, float z, float rotY);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)] private delegate uint IsEntityReadyDelegate(uint entityId);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)] private delegate void SetEntityAnimationDelegate(uint entityId, float speed, uint isWalking);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)] private delegate uint GetPrefabCountDelegate();
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)] private delegate void SetEntityNameDelegate(uint entityId, IntPtr name);

    // v10 delegate types
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)] private delegate void GetPlayerCarryStateDelegate(IntPtr outObjectInHand, IntPtr outNumObjects);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)] private delegate uint GetPlayerCrouchingDelegate();
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)] private delegate uint GetPlayerSittingDelegate();
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)] private delegate void SetEntityCrouchingDelegate(uint entityId, uint isCrouching);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)] private delegate void SetEntitySittingDelegate(uint entityId, uint isSitting);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)] private delegate void SetEntityCarryAnimDelegate(uint entityId, uint isCarrying);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)] private delegate void CreateEntityCarryVisualDelegate(uint entityId, uint objectInHandType);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)] private delegate void DestroyEntityCarryVisualDelegate(uint entityId);

    // v12 delegate types
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)] private delegate void GetDefaultSpawnPositionDelegate(IntPtr outX, IntPtr outY, IntPtr outZ);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)] private delegate void WarpLocalPlayerDelegate(float x, float y, float z);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate uint GetEntityPositionDelegate(uint entityId, IntPtr outX, IntPtr outY, IntPtr outZ);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate void AddEntityColliderDelegate(uint entityId);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate void SetEntityCarryTransformDelegate(uint entityId, float posX, float posY, float posZ, float rotX, float rotY, float rotZ);

    // v13 - World sync delegate types
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate uint WorldGetObjectCountDelegate();
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate uint WorldGetObjectHashesDelegate(IntPtr buf, uint maxCount);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate uint WorldGetObjectStateDelegate(IntPtr id, uint idLen, IntPtr buf, uint bufMax);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate int WorldSpawnObjectDelegate(byte objectType, int prefabId, float x, float y, float z, float rotX, float rotY, float rotZ, float rotW, IntPtr outId, uint outMax);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate int WorldDestroyObjectDelegate(IntPtr id, uint idLen);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate int WorldPlaceInRackDelegate(IntPtr id, uint idLen, int rackUid);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate int WorldRemoveFromRackDelegate(IntPtr id, uint idLen);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate int WorldSetPowerDelegate(IntPtr id, uint idLen, byte isOn);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate int WorldSetPropertyDelegate(IntPtr id, uint idLen, IntPtr key, uint keyLen, IntPtr val, uint valLen);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate int WorldConnectCableDelegate(int cableId, byte startType, float sx, float sy, float sz, IntPtr startDevice, uint startDeviceLen, byte endType, float ex, float ey, float ez, IntPtr endDevice, uint endDeviceLen);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate int WorldDisconnectCableDelegate(int cableId);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate int WorldPickupObjectDelegate(IntPtr id, uint idLen);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate int WorldDropObjectDelegate(IntPtr id, uint idLen, float x, float y, float z, float rotX, float rotY, float rotZ, float rotW);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate uint ObjFindByTypeDelegate(byte typeId, IntPtr outHandles, uint max);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate uint ObjGetStringFieldDelegate(ulong handle, ushort fieldId, IntPtr outBuf, uint max);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate int ObjSetStringFieldDelegate(ulong handle, ushort fieldId, IntPtr value, uint valueLen);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate int ObjIsActiveDelegate(ulong handle);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate int ObjSetActiveDelegate(ulong handle, int active);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate int ObjGetPositionDelegate(ulong handle, IntPtr outX, IntPtr outY, IntPtr outZ);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate int ObjSetPositionDelegate(ulong handle, float x, float y, float z);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate int ObjSetRotationDelegate(ulong handle, float x, float y, float z, float w);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate int ObjSetParentToWorldDelegate(ulong handle);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate int RbSetKinematicDelegate(ulong handle, int kinematic);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate int RbSetGravityDelegate(ulong handle, int useGravity);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate int RbWakeUpDelegate(ulong handle);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate ulong ObjFindByIdDelegate(byte typeId, ushort fieldId, IntPtr id, uint idLen);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate int GetHeldObjectDelegate(IntPtr outId, uint idMax, IntPtr outType);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate int ObjGetRotationDelegate(ulong handle, IntPtr outX, IntPtr outY, IntPtr outZ, IntPtr outW);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate int ObjSetParentDelegate(ulong child, ulong parent);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate int ObjSetLocalPositionDelegate(ulong handle, float x, float y, float z);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate int ObjSetLocalRotationDelegate(ulong handle, float x, float y, float z, float w);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate ulong RackFindPositionDelegate(int rackUid);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate int RackGameInstallDelegate(ulong objHandle, ulong rackPosHandle, byte objectType);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate int RackGameUninstallDelegate(ulong objHandle, byte objectType);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate int RegisterCustomEmployeeDelegate(IntPtr employeeId, IntPtr name, IntPtr description, float salary, float requiredReputation, uint confirmDialogs);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate uint IsCustomEmployeeHiredDelegate(IntPtr employeeId);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate int FireCustomEmployeeDelegate(IntPtr employeeId);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate int RegisterSalaryDelegate(int monthlySalary);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate int ShowNotificationDelegate(IntPtr message);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate void SetGamePausedDelegate(uint paused);

    // prevent GC while rust holds pointers
    private LogDelegate _logInfo, _logWarning, _logError;
    private GetDoubleDelegate _getPlayerMoney, _getPlayerXP, _getPlayerReputation;
    private SetDoubleDelegate _setPlayerMoney, _setPlayerXP, _setPlayerReputation;
    private GetFloatDelegate _getTimeScale, _getTimeOfDay, _getSecondsInFullDay;
    private SetFloatDelegate _setTimeScale, _setSecondsInFullDay;
    private GetUIntDelegate _getServerCount, _getRackCount, _getDay, _getSwitchCount, _getSatisfiedCustomerCount;
    private GetUIntDelegate _isNetWatchEnabled, _getNetWatchStats;
    private SetUIntDelegate _setNetWatchEnabled;
    private GetStringDelegate _getCurrentScene;
    // v4
    private GetUIntDelegate _getBrokenServerCount, _getBrokenSwitchCount, _getEolServerCount, _getEolSwitchCount;
    private GetUIntDelegate _getFreeTechnicianCount, _getTotalTechnicianCount;
    private GetIntDelegate _dispatchRepairServer, _dispatchRepairSwitch, _dispatchReplaceServer, _dispatchReplaceSwitch;
    // v5
    private RegisterCustomEmployeeDelegate _registerCustomEmployee;
    private IsCustomEmployeeHiredDelegate _isCustomEmployeeHired;
    private FireCustomEmployeeDelegate _fireCustomEmployee;
    private RegisterSalaryDelegate _registerSalary;
    // v6
    private ShowNotificationDelegate _showNotification;
    private GetFloatDelegate _getMoneyPerSecond, _getExpensesPerSecond, _getXpPerSecond;
    private GetUIntDelegate _isGamePaused2;
    private SetGamePausedDelegate _setGamePaused;
    private GetIntDelegate _getDifficulty, _triggerSave;
    // v7
    private GetULongDelegate _steamGetMyId;
    private GetStringFromU64Delegate _steamGetFriendName;
    private CreateLobbyDelegate _steamCreateLobby;
    private JoinLobbyDelegate _steamJoinLobby;
    private VoidDelegate _steamLeaveLobby;
    private GetULongDelegate _steamGetLobbyId;
    private GetULongDelegate _steamGetLobbyOwner;
    private GetUIntDelegate _steamGetLobbyMemberCount;
    private GetLobbyMemberDelegate _steamGetLobbyMemberByIndex;
    private SetLobbyDataDelegate _steamSetLobbyData;
    private GetLobbyDataDelegate _steamGetLobbyData;
    private SendP2PDelegate _steamSendP2P;
    private IsP2PAvailableDelegate _steamIsP2PAvailable;
    private ReadP2PDelegate _steamReadP2P;
    private AcceptP2PDelegate _steamAcceptP2P;
    private PollEventDelegate _steamPollEvent;
    private GetPlayerPositionDelegate _getPlayerPosition;
    // v8
    private ConfigRegisterBoolDelegate _configRegisterBool;
    private ConfigRegisterIntDelegate _configRegisterInt;
    private ConfigRegisterFloatDelegate _configRegisterFloat;
    private ConfigGetBoolDelegate _configGetBool;
    private ConfigGetIntDelegate _configGetInt;
    private ConfigGetFloatDelegate _configGetFloat;
    private SpawnCharacterDelegate _spawnCharacter;
    private DestroyEntityDelegate _destroyEntity;
    private SetEntityPositionDelegate _setEntityPosition;
    private IsEntityReadyDelegate _isEntityReady;
    private SetEntityAnimationDelegate _setEntityAnimation;
    private GetPrefabCountDelegate _getPrefabCount;
    private SetEntityNameDelegate _setEntityName;
    private GetPlayerCarryStateDelegate _getPlayerCarryState;
    private GetPlayerCrouchingDelegate _getPlayerCrouching;
    private GetPlayerSittingDelegate _getPlayerSitting;
    private SetEntityCrouchingDelegate _setEntityCrouching;
    private SetEntitySittingDelegate _setEntitySitting;
    private SetEntityCarryAnimDelegate _setEntityCarryAnim;
    private CreateEntityCarryVisualDelegate _createEntityCarryVisual;
    private DestroyEntityCarryVisualDelegate _destroyEntityCarryVisual;

    private GetDefaultSpawnPositionDelegate _getDefaultSpawnPosition;
    private WarpLocalPlayerDelegate _warpLocalPlayer;

    GetEntityPositionDelegate _getEntityPosition;
    AddEntityColliderDelegate _addEntityCollider;
    SetEntityCarryTransformDelegate _setEntityCarryTransform;
    // v13
    WorldGetObjectCountDelegate _worldGetObjectCount;
    WorldGetObjectHashesDelegate _worldGetObjectHashes;
    WorldGetObjectStateDelegate _worldGetObjectState;
    WorldSpawnObjectDelegate _worldSpawnObject;
    WorldDestroyObjectDelegate _worldDestroyObject;
    WorldPlaceInRackDelegate _worldPlaceInRack;
    WorldRemoveFromRackDelegate _worldRemoveFromRack;
    WorldSetPowerDelegate _worldSetPower;
    WorldSetPropertyDelegate _worldSetProperty;
    WorldConnectCableDelegate _worldConnectCable;
    WorldDisconnectCableDelegate _worldDisconnectCable;
    WorldPickupObjectDelegate _worldPickupObject;
    WorldDropObjectDelegate _worldDropObject;
    GetIntDelegate _worldEnsureRackUIDs;

    private ObjFindByTypeDelegate _objFindByType;
    private ObjGetStringFieldDelegate _objGetStringField;
    private ObjIsActiveDelegate _objIsActive;
    private ObjSetActiveDelegate _objSetActive;
    private ObjGetPositionDelegate _objGetPosition;
    private ObjSetPositionDelegate _objSetPosition;
    private ObjSetRotationDelegate _objSetRotation;
    private ObjSetParentToWorldDelegate _objSetParentToWorld;
    private RbSetKinematicDelegate _rbSetKinematic;
    private RbSetGravityDelegate _rbSetGravity;
    private RbWakeUpDelegate _rbWakeUp;
    ObjFindByIdDelegate _objFindById;

    private GetHeldObjectDelegate _getHeldObject;
    private ObjGetRotationDelegate _objGetRotation;

    private ObjSetParentDelegate _objSetParent;
    private ObjSetLocalPositionDelegate _objSetLocalPosition;
    private ObjSetLocalRotationDelegate _objSetLocalRotation;
    private RackFindPositionDelegate _rackFindPosition;
    private RackGameInstallDelegate _rackGameInstall;
    private RackGameUninstallDelegate _rackGameUninstall;
    private ObjSetStringFieldDelegate _objSetStringField2;

    private readonly MelonLogger.Instance _logger;
    private IntPtr _currentScenePtr = IntPtr.Zero;
    public GameAPIManager(MelonLogger.Instance logger)
    {
        _logger = logger;
        BindSystem();
        BindMaintenance();
        BindCrew();
        BindGame();
        BindSteam();
        BindConfig();
        BindEntities();
        BindWorld();
        BindRacks();
        BuildTable();
    }
    public void Dispose()
    {
        if (_tablePtr != IntPtr.Zero) { Marshal.FreeHGlobal(_tablePtr); _tablePtr = IntPtr.Zero; }
        if (_currentScenePtr != IntPtr.Zero) { Marshal.FreeHGlobal(_currentScenePtr); _currentScenePtr = IntPtr.Zero; }
        GC.SuppressFinalize(this);
    }
}
