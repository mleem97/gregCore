using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Il2Cpp;
using MelonLoader;
using UnityEngine;
using gregCore.API;

namespace DataCenterModLoader;

public partial class GameAPIManager
{
    private void BindSystem()
    {
        _logInfo = LogInfoImpl;
        _logWarning = LogWarningImpl;
        _logError = LogErrorImpl;
        _getPlayerMoney = GetPlayerMoneyImpl;
        _setPlayerMoney = SetPlayerMoneyImpl;
        _getTimeScale = GetTimeScaleImpl;
        _setTimeScale = SetTimeScaleImpl;
        _getServerCount = GetServerCountImpl;
        _getRackCount = GetRackCountImpl;
        _getCurrentScene = GetCurrentSceneImpl;
        _getPlayerXP = GetPlayerXPImpl;
        _setPlayerXP = SetPlayerXPImpl;
        _getPlayerReputation = GetPlayerReputationImpl;
        _setPlayerReputation = SetPlayerReputationImpl;
        _getTimeOfDay = GetTimeOfDayImpl;
        _getDay = GetDayImpl;
        _getSecondsInFullDay = GetSecondsInFullDayImpl;
        _setSecondsInFullDay = SetSecondsInFullDayImpl;
        _getSwitchCount = GetSwitchCountImpl;
        _getSatisfiedCustomerCount = GetSatisfiedCustomerCountImpl;
        _setNetWatchEnabled = SetNetWatchEnabledImpl;
        _isNetWatchEnabled = IsNetWatchEnabledImpl;
        _getNetWatchStats = GetNetWatchStatsImpl;
    }

    private void BindMaintenance()
    {
        _getBrokenServerCount = GetBrokenServerCountImpl;
        _getBrokenSwitchCount = GetBrokenSwitchCountImpl;
        _getEolServerCount = GetEolServerCountImpl;
        _getEolSwitchCount = GetEolSwitchCountImpl;
        _getFreeTechnicianCount = GetFreeTechnicianCountImpl;
        _getTotalTechnicianCount = GetTotalTechnicianCountImpl;
        _dispatchRepairServer = DispatchRepairServerImpl;
        _dispatchRepairSwitch = DispatchRepairSwitchImpl;
        _dispatchReplaceServer = DispatchReplaceServerImpl;
        _dispatchReplaceSwitch = DispatchReplaceSwitchImpl;
    }

    private void BindCrew()
    {
        _registerCustomEmployee = RegisterCustomEmployeeImpl;
        _isCustomEmployeeHired = IsCustomEmployeeHiredImpl;
        _fireCustomEmployee = FireCustomEmployeeImpl;
        _registerSalary = RegisterSalaryImpl;
    }

    private void BindGame()
    {
        _showNotification = ShowNotificationImpl;
        _getMoneyPerSecond = GetMoneyPerSecondImpl;
        _getExpensesPerSecond = GetExpensesPerSecondImpl;
        _getXpPerSecond = GetXpPerSecondImpl;
        _isGamePaused2 = IsGamePausedImpl;
        _setGamePaused = SetGamePausedImpl;
        _getDifficulty = GetDifficultyImpl;
        _triggerSave = TriggerSaveImpl;
    }

    private void BindSteam()
    {
        _steamGetMyId = SteamGetMyIdImpl;
        _steamGetFriendName = SteamGetFriendNameImpl;
        _steamCreateLobby = SteamCreateLobbyImpl;
        _steamJoinLobby = SteamJoinLobbyImpl;
        _steamLeaveLobby = SteamLeaveLobbyImpl;
        _steamGetLobbyId = SteamGetLobbyIdImpl;
        _steamGetLobbyOwner = SteamGetLobbyOwnerImpl;
        _steamGetLobbyMemberCount = SteamGetLobbyMemberCountImpl;
        _steamGetLobbyMemberByIndex = SteamGetLobbyMemberByIndexImpl;
        _steamSetLobbyData = SteamSetLobbyDataImpl;
        _steamGetLobbyData = SteamGetLobbyDataImpl;
        _steamSendP2P = SteamSendP2PImpl;
        _steamIsP2PAvailable = SteamIsP2PAvailableImpl;
        _steamReadP2P = SteamReadP2PImpl;
        _steamAcceptP2P = SteamAcceptP2PImpl;
        _steamPollEvent = SteamPollEventImpl;
        _getPlayerPosition = GetPlayerPositionImpl;
    }

    private void BindConfig()
    {
        _configRegisterBool = ConfigRegisterBoolImpl;
        _configRegisterInt = ConfigRegisterIntImpl;
        _configRegisterFloat = ConfigRegisterFloatImpl;
        _configGetBool = ConfigGetBoolImpl;
        _configGetInt = ConfigGetIntImpl;
        _configGetFloat = ConfigGetFloatImpl;
    }

    private void BindEntities()
    {
        _spawnCharacter = SpawnCharacterImpl;
        _destroyEntity = DestroyEntityImpl;
        _setEntityPosition = SetEntityPositionImpl;
        _isEntityReady = IsEntityReadyImpl;
        _setEntityAnimation = SetEntityAnimationImpl;
        _getPrefabCount = GetPrefabCountImpl;
        _setEntityName = SetEntityNameImpl;
        _getPlayerCarryState = GetPlayerCarryStateImpl;
        _getPlayerCrouching = GetPlayerCrouchingImpl;
        _getPlayerSitting = GetPlayerSittingImpl;
        _setEntityCrouching = SetEntityCrouchingImpl;
        _setEntitySitting = SetEntitySittingImpl;
        _setEntityCarryAnim = SetEntityCarryAnimImpl;
        _createEntityCarryVisual = CreateEntityCarryVisualImpl;
        _destroyEntityCarryVisual = DestroyEntityCarryVisualImpl;
        _getDefaultSpawnPosition = GetDefaultSpawnPositionImpl;
        _warpLocalPlayer = WarpLocalPlayerImpl;
        _getEntityPosition = GetEntityPositionImpl;
        _addEntityCollider = AddEntityColliderImpl;
        _setEntityCarryTransform = SetEntityCarryTransformImpl;
    }

    private void BindWorld()
    {
        // v13
        _worldGetObjectCount = WorldGetObjectCountImpl;
        _worldGetObjectHashes = WorldGetObjectHashesImpl;
        _worldGetObjectState = WorldGetObjectStateImpl;
        _worldSpawnObject = WorldSpawnObjectImpl;
        _worldDestroyObject = WorldDestroyObjectImpl;
        _worldPlaceInRack = WorldPlaceInRackImpl;
        _worldRemoveFromRack = WorldRemoveFromRackImpl;
        _worldSetPower = WorldSetPowerImpl;
        _worldSetProperty = WorldSetPropertyImpl;
        _worldConnectCable = WorldConnectCableImpl;
        _worldDisconnectCable = WorldDisconnectCableImpl;
        _worldPickupObject = WorldPickupObjectImpl;
        _worldDropObject = WorldDropObjectImpl;
        _worldEnsureRackUIDs = WorldEnsureRackUIDsImpl;
        _objFindByType = ObjFindByTypeImpl;
        _objGetStringField = ObjGetStringFieldImpl;
        _objIsActive = ObjIsActiveImpl;
        _objSetActive = ObjSetActiveImpl;
        _objGetPosition = ObjGetPositionImpl;
        _objSetPosition = ObjSetPositionImpl;
        _objSetRotation = ObjSetRotationImpl;
        _objSetParentToWorld = ObjSetParentToWorldImpl;
        _rbSetKinematic = RbSetKinematicImpl;
        _rbSetGravity = RbSetGravityImpl;
        _rbWakeUp = RbWakeUpImpl;
        _objFindById = ObjFindByIdImpl;
        _getHeldObject = GetHeldObjectImpl;
        _objGetRotation = ObjGetRotationImpl;
    }

    private void BindRacks()
    {
        _objSetParent = ObjSetParentImpl;
        _objSetLocalPosition = ObjSetLocalPositionImpl;
        _objSetLocalRotation = ObjSetLocalRotationImpl;
        _rackFindPosition = RackFindPositionImpl;
        _rackGameInstall = RackGameInstallImpl;
        _rackGameUninstall = RackGameUninstallImpl;
        _objSetStringField2 = ObjSetStringFieldImpl;
    }

    private void BuildTable()
    {
        _table = new GameAPITable
        {
            ApiVersion = API_VERSION,
        };
        FillTableSystem();
        FillTableDevices();
        FillTableSteam();
        FillTableConfig();
        FillTableEntities();
        FillTableWorld();
        FillTableRacks();
        _tablePtr = Marshal.AllocHGlobal(Marshal.SizeOf<GameAPITable>());
        Marshal.StructureToPtr(_table, _tablePtr, false);
    }

    private void FillTableSystem()
    {
        _table.LogInfo = Marshal.GetFunctionPointerForDelegate(_logInfo);
        _table.LogWarning = Marshal.GetFunctionPointerForDelegate(_logWarning);
        _table.LogError = Marshal.GetFunctionPointerForDelegate(_logError);
        _table.GetPlayerMoney = Marshal.GetFunctionPointerForDelegate(_getPlayerMoney);
        _table.SetPlayerMoney = Marshal.GetFunctionPointerForDelegate(_setPlayerMoney);
        _table.GetTimeScale = Marshal.GetFunctionPointerForDelegate(_getTimeScale);
        _table.SetTimeScale = Marshal.GetFunctionPointerForDelegate(_setTimeScale);
        _table.GetServerCount = Marshal.GetFunctionPointerForDelegate(_getServerCount);
        _table.GetRackCount = Marshal.GetFunctionPointerForDelegate(_getRackCount);
        _table.GetCurrentScene = Marshal.GetFunctionPointerForDelegate(_getCurrentScene);
        _table.GetPlayerXP = Marshal.GetFunctionPointerForDelegate(_getPlayerXP);
        _table.SetPlayerXP = Marshal.GetFunctionPointerForDelegate(_setPlayerXP);
        _table.GetPlayerReputation = Marshal.GetFunctionPointerForDelegate(_getPlayerReputation);
        _table.SetPlayerReputation = Marshal.GetFunctionPointerForDelegate(_setPlayerReputation);
        _table.GetTimeOfDay = Marshal.GetFunctionPointerForDelegate(_getTimeOfDay);
        _table.GetDay = Marshal.GetFunctionPointerForDelegate(_getDay);
        _table.GetSecondsInFullDay = Marshal.GetFunctionPointerForDelegate(_getSecondsInFullDay);
        _table.SetSecondsInFullDay = Marshal.GetFunctionPointerForDelegate(_setSecondsInFullDay);
        _table.GetSwitchCount = Marshal.GetFunctionPointerForDelegate(_getSwitchCount);
        _table.GetSatisfiedCustomerCount = Marshal.GetFunctionPointerForDelegate(_getSatisfiedCustomerCount);
        _table.SetNetWatchEnabled = Marshal.GetFunctionPointerForDelegate(_setNetWatchEnabled);
        _table.IsNetWatchEnabled = Marshal.GetFunctionPointerForDelegate(_isNetWatchEnabled);
        _table.GetNetWatchStats = Marshal.GetFunctionPointerForDelegate(_getNetWatchStats);
    }

    private void FillTableDevices()
    {
        _table.GetBrokenServerCount = Marshal.GetFunctionPointerForDelegate(_getBrokenServerCount);
        _table.GetBrokenSwitchCount = Marshal.GetFunctionPointerForDelegate(_getBrokenSwitchCount);
        _table.GetEolServerCount = Marshal.GetFunctionPointerForDelegate(_getEolServerCount);
        _table.GetEolSwitchCount = Marshal.GetFunctionPointerForDelegate(_getEolSwitchCount);
        _table.GetFreeTechnicianCount = Marshal.GetFunctionPointerForDelegate(_getFreeTechnicianCount);
        _table.GetTotalTechnicianCount = Marshal.GetFunctionPointerForDelegate(_getTotalTechnicianCount);
        _table.DispatchRepairServer = Marshal.GetFunctionPointerForDelegate(_dispatchRepairServer);
        _table.DispatchRepairSwitch = Marshal.GetFunctionPointerForDelegate(_dispatchRepairSwitch);
        _table.DispatchReplaceServer = Marshal.GetFunctionPointerForDelegate(_dispatchReplaceServer);
        _table.DispatchReplaceSwitch = Marshal.GetFunctionPointerForDelegate(_dispatchReplaceSwitch);
        _table.RegisterCustomEmployee = Marshal.GetFunctionPointerForDelegate(_registerCustomEmployee);
        _table.IsCustomEmployeeHired = Marshal.GetFunctionPointerForDelegate(_isCustomEmployeeHired);
        _table.FireCustomEmployee = Marshal.GetFunctionPointerForDelegate(_fireCustomEmployee);
        _table.RegisterSalary = Marshal.GetFunctionPointerForDelegate(_registerSalary);
        _table.ShowNotification = Marshal.GetFunctionPointerForDelegate(_showNotification);
        _table.GetMoneyPerSecond = Marshal.GetFunctionPointerForDelegate(_getMoneyPerSecond);
        _table.GetExpensesPerSecond = Marshal.GetFunctionPointerForDelegate(_getExpensesPerSecond);
        _table.GetXpPerSecond = Marshal.GetFunctionPointerForDelegate(_getXpPerSecond);
        _table.IsGamePaused = Marshal.GetFunctionPointerForDelegate(_isGamePaused2);
        _table.SetGamePaused = Marshal.GetFunctionPointerForDelegate(_setGamePaused);
        _table.GetDifficulty = Marshal.GetFunctionPointerForDelegate(_getDifficulty);
        _table.TriggerSave = Marshal.GetFunctionPointerForDelegate(_triggerSave);
    }

    private void FillTableSteam()
    {
        _table.SteamGetMyId = Marshal.GetFunctionPointerForDelegate(_steamGetMyId);
        _table.SteamGetFriendName = Marshal.GetFunctionPointerForDelegate(_steamGetFriendName);
        _table.SteamCreateLobby = Marshal.GetFunctionPointerForDelegate(_steamCreateLobby);
        _table.SteamJoinLobby = Marshal.GetFunctionPointerForDelegate(_steamJoinLobby);
        _table.SteamLeaveLobby = Marshal.GetFunctionPointerForDelegate(_steamLeaveLobby);
        _table.SteamGetLobbyId = Marshal.GetFunctionPointerForDelegate(_steamGetLobbyId);
        _table.SteamGetLobbyOwner = Marshal.GetFunctionPointerForDelegate(_steamGetLobbyOwner);
        _table.SteamGetLobbyMemberCount = Marshal.GetFunctionPointerForDelegate(_steamGetLobbyMemberCount);
        _table.SteamGetLobbyMemberByIndex = Marshal.GetFunctionPointerForDelegate(_steamGetLobbyMemberByIndex);
        _table.SteamSetLobbyData = Marshal.GetFunctionPointerForDelegate(_steamSetLobbyData);
        _table.SteamGetLobbyData = Marshal.GetFunctionPointerForDelegate(_steamGetLobbyData);
        _table.SteamSendP2P = Marshal.GetFunctionPointerForDelegate(_steamSendP2P);
        _table.SteamIsP2PAvailable = Marshal.GetFunctionPointerForDelegate(_steamIsP2PAvailable);
        _table.SteamReadP2P = Marshal.GetFunctionPointerForDelegate(_steamReadP2P);
        _table.SteamAcceptP2P = Marshal.GetFunctionPointerForDelegate(_steamAcceptP2P);
        _table.SteamPollEvent = Marshal.GetFunctionPointerForDelegate(_steamPollEvent);
        _table.GetPlayerPosition = Marshal.GetFunctionPointerForDelegate(_getPlayerPosition);
    }

    private void FillTableConfig()
    {
        _table.ConfigRegisterBool = Marshal.GetFunctionPointerForDelegate(_configRegisterBool);
        _table.ConfigRegisterInt = Marshal.GetFunctionPointerForDelegate(_configRegisterInt);
        _table.ConfigRegisterFloat = Marshal.GetFunctionPointerForDelegate(_configRegisterFloat);
        _table.ConfigGetBool = Marshal.GetFunctionPointerForDelegate(_configGetBool);
        _table.ConfigGetInt = Marshal.GetFunctionPointerForDelegate(_configGetInt);
        _table.ConfigGetFloat = Marshal.GetFunctionPointerForDelegate(_configGetFloat);
    }

    private void FillTableEntities()
    {
        _table.SpawnCharacter = Marshal.GetFunctionPointerForDelegate(_spawnCharacter);
        _table.DestroyEntity = Marshal.GetFunctionPointerForDelegate(_destroyEntity);
        _table.SetEntityPosition = Marshal.GetFunctionPointerForDelegate(_setEntityPosition);
        _table.IsEntityReady = Marshal.GetFunctionPointerForDelegate(_isEntityReady);
        _table.SetEntityAnimation = Marshal.GetFunctionPointerForDelegate(_setEntityAnimation);
        _table.GetPrefabCount = Marshal.GetFunctionPointerForDelegate(_getPrefabCount);
        _table.SetEntityName = Marshal.GetFunctionPointerForDelegate(_setEntityName);
        _table.GetPlayerCarryState = Marshal.GetFunctionPointerForDelegate(_getPlayerCarryState);
        _table.GetPlayerCrouching = Marshal.GetFunctionPointerForDelegate(_getPlayerCrouching);
        _table.GetPlayerSitting = Marshal.GetFunctionPointerForDelegate(_getPlayerSitting);
        _table.SetEntityCrouching = Marshal.GetFunctionPointerForDelegate(_setEntityCrouching);
        _table.SetEntitySitting = Marshal.GetFunctionPointerForDelegate(_setEntitySitting);
        _table.SetEntityCarryAnim = Marshal.GetFunctionPointerForDelegate(_setEntityCarryAnim);
        _table.CreateEntityCarryVisual = Marshal.GetFunctionPointerForDelegate(_createEntityCarryVisual);
        _table.DestroyEntityCarryVisual = Marshal.GetFunctionPointerForDelegate(_destroyEntityCarryVisual);
        _table.GetDefaultSpawnPosition = Marshal.GetFunctionPointerForDelegate(_getDefaultSpawnPosition);
        _table.WarpLocalPlayer = Marshal.GetFunctionPointerForDelegate(_warpLocalPlayer);
        _table.GetEntityPosition = Marshal.GetFunctionPointerForDelegate(_getEntityPosition);
        _table.AddEntityCollider = Marshal.GetFunctionPointerForDelegate(_addEntityCollider);
        _table.SetEntityCarryTransform = Marshal.GetFunctionPointerForDelegate(_setEntityCarryTransform);
    }

    private void FillTableWorld()
    {
        _table.WorldGetObjectCount = Marshal.GetFunctionPointerForDelegate(_worldGetObjectCount);
        _table.WorldGetObjectHashes = Marshal.GetFunctionPointerForDelegate(_worldGetObjectHashes);
        _table.WorldGetObjectState = Marshal.GetFunctionPointerForDelegate(_worldGetObjectState);
        _table.WorldSpawnObject = Marshal.GetFunctionPointerForDelegate(_worldSpawnObject);
        _table.WorldDestroyObject = Marshal.GetFunctionPointerForDelegate(_worldDestroyObject);
        _table.WorldPlaceInRack = Marshal.GetFunctionPointerForDelegate(_worldPlaceInRack);
        _table.WorldRemoveFromRack = Marshal.GetFunctionPointerForDelegate(_worldRemoveFromRack);
        _table.WorldSetPower = Marshal.GetFunctionPointerForDelegate(_worldSetPower);
        _table.WorldSetProperty = Marshal.GetFunctionPointerForDelegate(_worldSetProperty);
        _table.WorldConnectCable = Marshal.GetFunctionPointerForDelegate(_worldConnectCable);
        _table.WorldDisconnectCable = Marshal.GetFunctionPointerForDelegate(_worldDisconnectCable);
        _table.WorldPickupObject = Marshal.GetFunctionPointerForDelegate(_worldPickupObject);
        _table.WorldDropObject = Marshal.GetFunctionPointerForDelegate(_worldDropObject);
        _table.WorldEnsureRackUIDs = Marshal.GetFunctionPointerForDelegate(_worldEnsureRackUIDs);
        _table.ObjFindByType = Marshal.GetFunctionPointerForDelegate(_objFindByType);
        _table.ObjGetStringField = Marshal.GetFunctionPointerForDelegate(_objGetStringField);
        _table.ObjIsActive = Marshal.GetFunctionPointerForDelegate(_objIsActive);
        _table.ObjSetActive = Marshal.GetFunctionPointerForDelegate(_objSetActive);
        _table.ObjGetPosition = Marshal.GetFunctionPointerForDelegate(_objGetPosition);
        _table.ObjSetPosition = Marshal.GetFunctionPointerForDelegate(_objSetPosition);
        _table.ObjSetRotation = Marshal.GetFunctionPointerForDelegate(_objSetRotation);
        _table.ObjSetParentToWorld = Marshal.GetFunctionPointerForDelegate(_objSetParentToWorld);
        _table.RbSetKinematic = Marshal.GetFunctionPointerForDelegate(_rbSetKinematic);
        _table.RbSetGravity = Marshal.GetFunctionPointerForDelegate(_rbSetGravity);
        _table.RbWakeUp = Marshal.GetFunctionPointerForDelegate(_rbWakeUp);
        _table.ObjFindById = Marshal.GetFunctionPointerForDelegate(_objFindById);
        _table.GetHeldObject = Marshal.GetFunctionPointerForDelegate(_getHeldObject);
        _table.ObjGetRotation = Marshal.GetFunctionPointerForDelegate(_objGetRotation);
    }

    private void FillTableRacks()
    {
        _table.ObjSetParent = Marshal.GetFunctionPointerForDelegate(_objSetParent);
        _table.ObjSetLocalPosition = Marshal.GetFunctionPointerForDelegate(_objSetLocalPosition);
        _table.ObjSetLocalRotation = Marshal.GetFunctionPointerForDelegate(_objSetLocalRotation);
        _table.RackFindPosition = Marshal.GetFunctionPointerForDelegate(_rackFindPosition);
        _table.RackGameInstall = Marshal.GetFunctionPointerForDelegate(_rackGameInstall);
        _table.RackGameUninstall = Marshal.GetFunctionPointerForDelegate(_rackGameUninstall);
        _table.ObjSetStringField = Marshal.GetFunctionPointerForDelegate(_objSetStringField2);
    }

    public IntPtr GetTablePointer() => _tablePtr;
}
