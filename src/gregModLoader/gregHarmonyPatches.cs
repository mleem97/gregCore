using System;
using System.Collections.Generic;
using HarmonyLib;
using greg.Sdk;
using Il2Cpp;
using UnityEngine;

namespace greg.Core;

// harmony patches -> rust events

[HarmonyPatch(typeof(Player), nameof(Player.UpdateCoin))]
internal static class Patch_Player_UpdateCoin
{
    private static float _oldMoney;

    internal static bool Prefix(Player __instance, float _coinChhangeAmount, bool withoutSound)
    {
        try { _oldMoney = __instance.money; }
        catch { _oldMoney = 0f; }

        try
        {
            float tentativeNew = _oldMoney + _coinChhangeAmount;
            var payload = new
            {
                coinChangeAmount = _coinChhangeAmount,
                withoutSound = withoutSound,
                newBalance = tentativeNew,
                accepted = true
            };
            if (!gregEventDispatcher.InvokeCancelable(gregNativeEventHooks.PlayerCoinChanged, payload))
                return false;
        }
        catch (Exception ex) { EventDispatcher.LogError($"UpdateCoin cancelable prefix: {ex.Message}"); }

        return true;
    }

    internal static void Postfix(Player __instance)
    {
        try
        {
            float newMoney = __instance.money;
            if (Math.Abs(newMoney - _oldMoney) > 0.001f)
                EventDispatcher.FireValueChanged(EventIds.MoneyChanged, _oldMoney, newMoney, newMoney - _oldMoney);
        }
        catch (Exception ex) { EventDispatcher.LogError($"UpdateCoin postfix: {ex.Message}"); }
    }
}

[HarmonyPatch(typeof(Player), nameof(Player.UpdateXP))]
internal static class Patch_Player_UpdateXP
{
    private static float _oldXP;

    internal static void Prefix(Player __instance)
    {
        try { _oldXP = __instance.xp; }
        catch { _oldXP = 0f; }
    }

    internal static void Postfix(Player __instance)
    {
        try
        {
            float newXP = __instance.xp;
            if (Math.Abs(newXP - _oldXP) > 0.001f)
                EventDispatcher.FireValueChanged(EventIds.XPChanged, _oldXP, newXP, newXP - _oldXP);
        }
        catch (Exception ex) { EventDispatcher.LogError($"UpdateXP postfix: {ex.Message}"); }
    }
}

[HarmonyPatch(typeof(Player), nameof(Player.UpdateReputation))]
internal static class Patch_Player_UpdateReputation
{
    private static float _oldRep;

    internal static void Prefix(Player __instance)
    {
        try { _oldRep = __instance.reputation; }
        catch { _oldRep = 0f; }
    }

    internal static void Postfix(Player __instance)
    {
        try
        {
            float newRep = __instance.reputation;
            if (Math.Abs(newRep - _oldRep) > 0.001f)
                EventDispatcher.FireValueChanged(EventIds.ReputationChanged, _oldRep, newRep, newRep - _oldRep);
        }
        catch (Exception ex) { EventDispatcher.LogError($"UpdateReputation postfix: {ex.Message}"); }
    }
}

[HarmonyPatch(typeof(Server), nameof(Server.PowerButton))]
internal static class Patch_Server_PowerButton
{
    internal static void Postfix(Server __instance)
    {
        try { EventDispatcher.FireServerPowered(__instance.isOn); }
        catch (Exception ex) { EventDispatcher.LogError($"PowerButton: {ex.Message}"); }
    }
}

[HarmonyPatch(typeof(Server), nameof(Server.ItIsBroken))]
internal static class Patch_Server_ItIsBroken
{
    internal static void Postfix()
    {
        try { EventDispatcher.FireSimple(EventIds.ServerBroken); }
        catch (Exception ex) { EventDispatcher.LogError($"ItIsBroken: {ex.Message}"); }
    }
}

[HarmonyPatch(typeof(Server), nameof(Server.RepairDevice))]
internal static class Patch_Server_RepairDevice
{
    internal static void Postfix()
    {
        try { EventDispatcher.FireSimple(EventIds.ServerRepaired); }
        catch (Exception ex) { EventDispatcher.LogError($"RepairDevice: {ex.Message}"); }
    }
}

[HarmonyPatch(typeof(Server), nameof(Server.ServerInsertedInRack))]
internal static class Patch_Server_ServerInsertedInRack
{
    internal static void Postfix()
    {
        try { EventDispatcher.FireSimple(EventIds.ServerInstalled); }
        catch (Exception ex) { EventDispatcher.LogError($"ServerInsertedInRack: {ex.Message}"); }
    }
}

// track day changes each frame
// note: NetWatchSystem also uses this to detect day changes for salary deduction
[HarmonyPatch(typeof(TimeController), "Update")]
internal static class Patch_TimeController_Update
{
    private static int _lastDay = -1;

    internal static void Postfix(TimeController __instance)
    {
        try
        {
            int currentDay = __instance.day;
            if (_lastDay >= 0 && currentDay != _lastDay)
                EventDispatcher.FireDayEnded((uint)currentDay);
            _lastDay = currentDay;
        }
        catch (Exception ex) { EventDispatcher.LogError($"TimeController.Update: {ex.Message}"); }
    }
}

[HarmonyPatch(typeof(MainGameManager), nameof(MainGameManager.ButtonCustomerChosen))]
internal static class Patch_MainGameManager_ButtonCustomerChosen
{
    internal static void Postfix(int __0)
    {
        try { EventDispatcher.FireCustomerAccepted(__0); }
        catch (Exception ex) { EventDispatcher.LogError($"ButtonCustomerChosen: {ex.Message}"); }
    }
}

[HarmonyPatch(typeof(ComputerShop), nameof(ComputerShop.ButtonCheckOut))]
internal static class Patch_ComputerShop_ButtonCheckOut
{
    internal static void Postfix()
    {
        try { EventDispatcher.FireSimple(EventIds.ShopCheckout); }
        catch (Exception ex) { EventDispatcher.LogError($"ButtonCheckOut: {ex.Message}"); }
    }
}

[HarmonyPatch(typeof(HRSystem), nameof(HRSystem.ButtonConfirmHire))]
internal static class Patch_HRSystem_ButtonConfirmHire
{
    private static bool _wasCustom;

    internal static bool Prefix(HRSystem __instance)
    {
        try
        {
            if (CustomEmployeeManager.HandleConfirmHire(__instance))
            {
                _wasCustom = true;
                return false;
            }
        }
        catch (Exception ex) { CrashLog.LogException("ButtonConfirmHire prefix", ex); }
        _wasCustom = false;
        return true;
    }

    internal static void Postfix()
    {
        if (_wasCustom) return;
        try { EventDispatcher.FireSimple(EventIds.EmployeeHired); }
        catch (Exception ex) { EventDispatcher.LogError($"ButtonConfirmHire: {ex.Message}"); }
    }
}

[HarmonyPatch(typeof(HRSystem), nameof(HRSystem.ButtonConfirmFireEmployee))]
internal static class Patch_HRSystem_ButtonConfirmFireEmployee
{
    private static bool _wasCustom;

    internal static bool Prefix(HRSystem __instance)
    {
        try
        {
            if (CustomEmployeeManager.HandleConfirmFire(__instance))
            {
                _wasCustom = true;
                return false;
            }
        }
        catch (Exception ex) { CrashLog.LogException("ButtonConfirmFireEmployee prefix", ex); }
        _wasCustom = false;
        return true;
    }

    internal static void Postfix()
    {
        if (_wasCustom) return;
        try { EventDispatcher.FireSimple(EventIds.EmployeeFired); }
        catch (Exception ex) { EventDispatcher.LogError($"ButtonConfirmFireEmployee: {ex.Message}"); }
    }
}

[HarmonyPatch(typeof(HRSystem), nameof(HRSystem.ButtonCancelBuying))]
internal static class Patch_HRSystem_ButtonCancelBuying
{
    internal static void Postfix()
    {
        try { CustomEmployeeManager.ClearPending(); }
        catch (Exception ex) { CrashLog.LogException("ButtonCancelBuying clear pending", ex); }
    }
}

[HarmonyPatch(typeof(SaveSystem), nameof(SaveSystem.SaveGame))]
internal static class Patch_SaveSystem_SaveGame
{
    internal static void Postfix()
    {
        try
        {
            CustomEmployeeManager.SaveState();
            ModSaveCompatibilityService.OnSaveCompleted();
            EventDispatcher.FireSimple(EventIds.GameSaved);
        }
        catch (Exception ex) { EventDispatcher.LogError($"SaveGame: {ex.Message}"); }
    }
}

[HarmonyPatch(typeof(SaveSystem), nameof(SaveSystem.Load))]
internal static class Patch_SaveSystem_Load
{
    internal static void Postfix()
    {
        try
        {
            CustomEmployeeManager.LoadState();
            ModSaveCompatibilityService.OnLoadCompleted();
            EventDispatcher.FireSimple(EventIds.GameLoaded);
            TechnicianHiring.RestoreOnLoad();
        }
        catch (Exception ex) { EventDispatcher.LogError($"Load: {ex.Message}"); }
    }
}

[HarmonyPatch(typeof(CustomerBase), nameof(CustomerBase.AreAllAppRequirementsMet))]
internal static class Patch_CustomerBase_AreAllAppRequirementsMet
{
    private static readonly HashSet<int> _satisfiedCustomers = new();

    internal static void Postfix(CustomerBase __instance, ref bool __result)
    {
        try
        {
            int id = __instance.customerBaseID;

            float cur = __instance.currentSpeed;
            float req = __instance.currentTotalAppSpeeRequirements * greg.Sdk.Services.GregCohousingService.DemandMultiplier;
            
            // Re-evaluate result if demand is boosted
            if (greg.Sdk.Services.GregCohousingService.DemandMultiplier > 1.001f)
            {
                if (cur < req) 
                {
                    __result = false;
                }
            }

            if (!__result && greg.Sdk.Services.GregCohousingService.IsCohousingEnabled)
            {
                float missing = req - cur;
                if (missing > 0f)
                {
                    if (greg.Sdk.Services.GregCohousingService.TryAllocateExternal(id, missing))
                    {
                        __result = true;
                    }
                }
            }

            if (__result)
            {
                if (_satisfiedCustomers.Add(id))
                    EventDispatcher.FireCustomerSatisfied(id);
            }
            else
            {
                if (_satisfiedCustomers.Remove(id))
                    EventDispatcher.FireCustomerUnsatisfied(id);
            }
        }
        catch (Exception ex) { EventDispatcher.LogError($"AreAllAppRequirementsMet: {ex.Message}"); }
    }
}

[HarmonyPatch(typeof(Server), nameof(Server.RegisterLink))]
internal static class Patch_Server_RegisterLink
{
    internal static void Postfix()
    {
        try { EventDispatcher.FireCableConnected(); }
        catch (Exception ex) { EventDispatcher.LogError($"RegisterLink: {ex.Message}"); }
    }
}

[HarmonyPatch(typeof(Server), nameof(Server.UnregisterLink))]
internal static class Patch_Server_UnregisterLink
{
    internal static void Postfix()
    {
        try { EventDispatcher.FireCableDisconnected(); }
        catch (Exception ex) { EventDispatcher.LogError($"UnregisterLink: {ex.Message}"); }
    }
}

[HarmonyPatch(typeof(CablePositions), nameof(CablePositions.CreateNewCable))]
internal static class Patch_CablePositions_CreateNewCable
{
    internal static void Postfix()
    {
        try { EventDispatcher.FireCableCreated(); }
        catch (Exception ex) { EventDispatcher.LogError($"CreateNewCable: {ex.Message}"); }
    }
}

[HarmonyPatch(typeof(CablePositions), nameof(CablePositions.RemovePosition))]
internal static class Patch_CablePositions_RemovePosition
{
    internal static void Postfix()
    {
        try { EventDispatcher.FireCableRemoved(); }
        catch (Exception ex) { EventDispatcher.LogError($"RemovePosition: {ex.Message}"); }
    }
}

[HarmonyPatch(typeof(CablePositions), nameof(CablePositions.ClearAllCables))]
internal static class Patch_CablePositions_ClearAllCables
{
    internal static void Postfix()
    {
        try { EventDispatcher.FireCableCleared(); }
        catch (Exception ex) { EventDispatcher.LogError($"ClearAllCables: {ex.Message}"); }
    }
}

[HarmonyPatch(typeof(CableLink), nameof(CableLink.SetConnectionSpeed))]
internal static class Patch_CableLink_SetConnectionSpeed
{
    internal static void Postfix()
    {
        try { EventDispatcher.FireCableSpeedChanged(); }
        catch (Exception ex) { EventDispatcher.LogError($"SetConnectionSpeed: {ex.Message}"); }
    }
}

[HarmonyPatch(typeof(CableLink), nameof(CableLink.InsertSFP))]
internal static class Patch_CableLink_InsertSFP
{
    internal static void Postfix()
    {
        try { EventDispatcher.FireCableSfpInserted(); }
        catch (Exception ex) { EventDispatcher.LogError($"InsertSFP: {ex.Message}"); }
    }
}

[HarmonyPatch(typeof(CableLink), nameof(CableLink.RemoveSFP))]
internal static class Patch_CableLink_RemoveSFP
{
    internal static void Postfix()
    {
        try { EventDispatcher.FireCableSfpRemoved(); }
        catch (Exception ex) { EventDispatcher.LogError($"RemoveSFP: {ex.Message}"); }
    }
}

[HarmonyPatch(typeof(Server), nameof(Server.UpdateCustomer))]
internal static class Patch_Server_UpdateCustomer
{
    internal static void Postfix(int newCustomerID)
    {
        try { EventDispatcher.FireServerCustomerChanged(newCustomerID); }
        catch (Exception ex) { EventDispatcher.LogError($"UpdateCustomer: {ex.Message}"); }
    }
}

[HarmonyPatch(typeof(Server), nameof(Server.UpdateAppID))]
internal static class Patch_Server_UpdateAppID
{
    internal static void Postfix(int _appID)
    {
        try { EventDispatcher.FireServerAppChanged(_appID); }
        catch (Exception ex) { EventDispatcher.LogError($"UpdateAppID: {ex.Message}"); }
    }
}

[HarmonyPatch(typeof(Rack), nameof(Rack.ButtonUnmountRack))]
internal static class Patch_Rack_ButtonUnmountRack
{
    internal static void Postfix()
    {
        try { EventDispatcher.FireRackUnmounted(); }
        catch (Exception ex) { EventDispatcher.LogError($"ButtonUnmountRack: {ex.Message}"); }
    }
}

[HarmonyPatch(typeof(NetworkMap), nameof(NetworkMap.AddBrokenSwitch))]
internal static class Patch_NetworkMap_AddBrokenSwitch
{
    internal static void Postfix()
    {
        try { EventDispatcher.FireSwitchBroken(); }
        catch (Exception ex) { EventDispatcher.LogError($"AddBrokenSwitch: {ex.Message}"); }
    }
}

[HarmonyPatch(typeof(NetworkMap), nameof(NetworkMap.RemoveBrokenSwitch))]
internal static class Patch_NetworkMap_RemoveBrokenSwitch
{
    internal static void Postfix()
    {
        try { EventDispatcher.FireSwitchRepaired(); }
        catch (Exception ex) { EventDispatcher.LogError($"RemoveBrokenSwitch: {ex.Message}"); }
    }
}

[HarmonyPatch(typeof(BalanceSheet), nameof(BalanceSheet.SaveSnapshot))]
internal static class Patch_BalanceSheet_SaveSnapshot
{
    internal static void Postfix(int __0)
    {
        try { EventDispatcher.FireMonthEnded(__0); }
        catch (Exception ex) { EventDispatcher.LogError($"SaveSnapshot: {ex.Message}"); }
    }
}

[HarmonyPatch(typeof(ComputerShop), nameof(ComputerShop.ButtonBuyShopItem))]
internal static class Patch_ComputerShop_ButtonBuyShopItem
{
    internal static void Postfix(int __0, int __1, int __2)
    {
        try { EventDispatcher.FireShopItemAdded(__0, __1, __2); }
        catch (Exception ex) { EventDispatcher.LogError($"ButtonBuyShopItem: {ex.Message}"); }
    }
}

[HarmonyPatch(typeof(ComputerShop), nameof(ComputerShop.ButtonClear))]
internal static class Patch_ComputerShop_ButtonClear
{
    internal static void Postfix()
    {
        try { EventDispatcher.FireShopCartCleared(); }
        catch (Exception ex) { EventDispatcher.LogError($"ButtonClear: {ex.Message}"); }
    }
}

[HarmonyPatch(typeof(MainGameManager), nameof(MainGameManager.ButtonBuyWall))]
internal static class Patch_MainGameManager_ButtonBuyWall
{
    internal static void Postfix()
    {
        try { EventDispatcher.FireWallPurchased(); }
        catch (Exception ex) { EventDispatcher.LogError($"ButtonBuyWall: {ex.Message}"); }
    }
}

[HarmonyPatch(typeof(SaveSystem), nameof(SaveSystem.AutoSave))]
internal static class Patch_SaveSystem_AutoSave
{
    internal static void Postfix()
    {
        try
        {
            CustomEmployeeManager.SaveState();
            EventDispatcher.FireGameAutoSaved();
        }
        catch (Exception ex) { EventDispatcher.LogError($"AutoSave: {ex.Message}"); }
    }
}

[HarmonyPatch(typeof(ComputerShop), nameof(ComputerShop.RemoveSpawnedItem))]
internal static class Patch_ComputerShop_RemoveSpawnedItem
{
    internal static void Postfix(int __0)
    {
        try { EventDispatcher.FireShopItemRemoved(__0); }
        catch (Exception ex) { EventDispatcher.LogError($"RemoveSpawnedItem: {ex.Message}"); }
    }
}

[HarmonyPatch(typeof(StaticUIElements), nameof(StaticUIElements.CalculateRates))]
internal static class Patch_StaticUIElements_CalculateRates
{
    internal static void Postfix(ref float moneyPerSec, ref float xpPerSec, ref float expensesPerSec)
    {
        try { EventDispatcher.FireRatesCalculated(moneyPerSec, xpPerSec, expensesPerSec); }
        catch (Exception ex) { EventDispatcher.LogError($"CalculateRates: {ex.Message}"); }
    }
}

[HarmonyPatch(typeof(ComputerShop), nameof(ComputerShop.ButtonBalanceSheetScreen))]
internal static class Patch_ComputerShop_ButtonBalanceSheetScreen
{
    internal static void Postfix()
    {
        try { EventDispatcher.FireBalanceSheetScreenOpened(); }
        catch (Exception ex) { EventDispatcher.LogError($"ButtonBalanceSheetScreen: {ex.Message}"); }
    }
}

[HarmonyPatch(typeof(BalanceSheet), nameof(BalanceSheet.GetOrCreateRecord))]
internal static class Patch_BalanceSheet_GetOrCreateRecord
{
    internal static void Postfix()
    {
        try { EventDispatcher.FireBalanceSheetRecordAccessed(); }
        catch (Exception ex) { EventDispatcher.LogError($"GetOrCreateRecord: {ex.Message}"); }
    }
}

[HarmonyPatch(typeof(BalanceSheet), nameof(BalanceSheet.TrackFinances))]
internal static class Patch_BalanceSheet_TrackFinances
{
    internal static void Postfix()
    {
        try { EventDispatcher.FireBalanceSheetTrackFinances(); }
        catch (Exception ex) { EventDispatcher.LogError($"TrackFinances: {ex.Message}"); }
    }
}

[HarmonyPatch(typeof(BalanceSheet), nameof(BalanceSheet.FillInBalanceSheet))]
internal static class Patch_BalanceSheet_FillInBalanceSheet
{
    internal static void Postfix()
    {
        try { EventDispatcher.FireBalanceSheetFilled(); }
        catch (Exception ex) { EventDispatcher.LogError($"FillInBalanceSheet: {ex.Message}"); }
    }
}

[HarmonyPatch(typeof(BalanceSheet), nameof(BalanceSheet.AddTotalRow))]
internal static class Patch_BalanceSheet_AddTotalRow
{
    internal static void Postfix(float __0, float __1, float __2)
    {
        try { EventDispatcher.FireBalanceSheetTotalRowAdded(__0, __1, __2); }
        catch (Exception ex) { EventDispatcher.LogError($"AddTotalRow: {ex.Message}"); }
    }
}

[HarmonyPatch(typeof(BalanceSheet), nameof(BalanceSheet.RegisterSalary))]
internal static class Patch_BalanceSheet_RegisterSalary
{
    internal static void Postfix(int __0)
    {
        try { EventDispatcher.FireBalanceSheetSalaryRegistered(__0); }
        catch (Exception ex) { EventDispatcher.LogError($"RegisterSalary: {ex.Message}"); }
    }
}

[HarmonyPatch(typeof(BalanceSheet), nameof(BalanceSheet.RestoreRecord))]
internal static class Patch_BalanceSheet_RestoreRecord
{
    internal static void Postfix()
    {
        try { EventDispatcher.FireBalanceSheetRecordRestored(); }
        catch (Exception ex) { EventDispatcher.LogError($"RestoreRecord: {ex.Message}"); }
    }
}

[HarmonyPatch(typeof(BalanceSheet), nameof(BalanceSheet.GetSaveData))]
internal static class Patch_BalanceSheet_GetSaveData
{
    internal static void Postfix()
    {
        try { EventDispatcher.FireBalanceSheetDataSaved(); }
        catch (Exception ex) { EventDispatcher.LogError($"GetSaveData: {ex.Message}"); }
    }
}

[HarmonyPatch(typeof(BalanceSheet), nameof(BalanceSheet.LoadFromSave))]
internal static class Patch_BalanceSheet_LoadFromSave
{
    internal static void Postfix()
    {
        try { EventDispatcher.FireBalanceSheetDataLoaded(); }
        catch (Exception ex) { EventDispatcher.LogError($"LoadFromSave: {ex.Message}"); }
    }
}

[HarmonyPatch(typeof(BalanceSheet), nameof(BalanceSheet.GetLatestSnapshot))]
internal static class Patch_BalanceSheet_GetLatestSnapshot
{
    internal static void Postfix()
    {
        try { EventDispatcher.FireBalanceSheetLatestSnapshotRequested(); }
        catch (Exception ex) { EventDispatcher.LogError($"GetLatestSnapshot: {ex.Message}"); }
    }
}

[HarmonyPatch(typeof(ComputerShop), nameof(ComputerShop.UpdateCartTotal))]
internal static class Patch_ComputerShop_UpdateCartTotal
{
    internal static void Postfix()
    {
        try { EventDispatcher.FireShopCartTotalUpdated(); }
        catch (Exception ex) { EventDispatcher.LogError($"UpdateCartTotal: {ex.Message}"); }
    }
}

[HarmonyPatch(typeof(ComputerShop), nameof(ComputerShop.BuyNewItem))]
internal static class Patch_ComputerShop_BuyNewItem
{
    internal static void Postfix(int __0, int __1, int __2)
    {
        try { EventDispatcher.FireShopNewItemPurchased(__0, __1, __2); }
        catch (Exception ex) { EventDispatcher.LogError($"BuyNewItem: {ex.Message}"); }
    }
}

[HarmonyPatch(typeof(ComputerShop), nameof(ComputerShop.BuyAnotherItem))]
internal static class Patch_ComputerShop_BuyAnotherItem
{
    internal static void Postfix(int __0, int __1, int __2)
    {
        try { EventDispatcher.FireShopAnotherItemPurchased(__0, __1, __2); }
        catch (Exception ex) { EventDispatcher.LogError($"BuyAnotherItem: {ex.Message}"); }
    }
}

[HarmonyPatch(typeof(ComputerShop), nameof(ComputerShop.SpawnPhysicalItem))]
internal static class Patch_ComputerShop_SpawnPhysicalItem
{
    internal static void Postfix(int __1, int __2)
    {
        try { EventDispatcher.FireShopPhysicalItemSpawned(__1, __2); }
        catch (Exception ex) { EventDispatcher.LogError($"SpawnPhysicalItem: {ex.Message}"); }
    }
}

[HarmonyPatch(typeof(CustomerBase), nameof(CustomerBase.Awake))]
internal static class Patch_CustomerBase_Awake
{
    internal static void Postfix(CustomerBase __instance)
    {
        try { EventDispatcher.FireCustomerComponentInitialized(__instance.customerBaseID); }
        catch (Exception ex) { EventDispatcher.LogError($"CustomerBase.Awake: {ex.Message}"); }
    }
}

[HarmonyPatch(typeof(CustomerBase), nameof(CustomerBase.Start))]
internal static class Patch_CustomerBase_Start
{
    internal static void Postfix(CustomerBase __instance)
    {
        try { EventDispatcher.FireCustomerComponentInitialized(__instance.customerBaseID); }
        catch (Exception ex) { EventDispatcher.LogError($"CustomerBase.Start: {ex.Message}"); }
    }
}

[HarmonyPatch(typeof(CustomerBase), nameof(CustomerBase.UpdateCustomerServerCountAndSpeed))]
internal static class Patch_CustomerBase_UpdateCustomerServerCountAndSpeed
{
    internal static void Postfix(int __0, float __1)
    {
        try { EventDispatcher.FireCustomerServerCountAndSpeedChanged(__0, __1); }
        catch (Exception ex) { EventDispatcher.LogError($"CustomerBase.UpdateCustomerServerCountAndSpeed: {ex.Message}"); }
    }
}

[HarmonyPatch(typeof(CustomerBase), nameof(CustomerBase.AddAppPerformance))]
internal static class Patch_CustomerBase_AddAppPerformance
{
    internal static void Postfix(int __0, float __1)
    {
        try { EventDispatcher.FireCustomerAppPerformanceAdded(__0, __1); }
        catch (Exception ex) { EventDispatcher.LogError($"CustomerBase.AddAppPerformance: {ex.Message}"); }
    }
}

[HarmonyPatch(typeof(CustomerBase), nameof(CustomerBase.ResetAllAppSpeeds))]
internal static class Patch_CustomerBase_ResetAllAppSpeeds
{
    internal static void Postfix(CustomerBase __instance)
    {
        try { EventDispatcher.FireCustomerAppSpeedsReset(__instance.customerBaseID); }
        catch (Exception ex) { EventDispatcher.LogError($"CustomerBase.ResetAllAppSpeeds: {ex.Message}"); }
    }
}

[HarmonyPatch(typeof(CustomerBase), nameof(CustomerBase.SetUpBase))]
internal static class Patch_CustomerBase_SetUpBase
{
    internal static void Postfix(CustomerBase __instance)
    {
        try { EventDispatcher.FireCustomerBaseSetup(__instance.customerBaseID); }
        catch (Exception ex) { EventDispatcher.LogError($"CustomerBase.SetUpBase: {ex.Message}"); }
    }
}

[HarmonyPatch(typeof(CustomerBase), nameof(CustomerBase.SetUpApp))]
internal static class Patch_CustomerBase_SetUpApp
{
    internal static void Postfix(int __0, int __1)
    {
        try { EventDispatcher.FireCustomerAppSetup(__0, __1); }
        catch (Exception ex) { EventDispatcher.LogError($"CustomerBase.SetUpApp: {ex.Message}"); }
    }
}

[HarmonyPatch(typeof(CustomerBase), nameof(CustomerBase.UpdateSpeedOnCustomerBaseApp))]
internal static class Patch_CustomerBase_UpdateSpeedOnCustomerBaseApp
{
    internal static void Postfix(int __0, float __1)
    {
        try { EventDispatcher.FireCustomerSpeedOnAppChanged(__0, __1); }
        catch (Exception ex) { EventDispatcher.LogError($"CustomerBase.UpdateSpeedOnCustomerBaseApp: {ex.Message}"); }
    }
}

[HarmonyPatch(typeof(CustomerBase), nameof(CustomerBase.LoadData))]
internal static class Patch_CustomerBase_LoadData
{
    internal static void Postfix(CustomerBase __instance)
    {
        try { EventDispatcher.FireCustomerDataLoaded(__instance.customerBaseID); }
        catch (Exception ex) { EventDispatcher.LogError($"CustomerBase.LoadData: {ex.Message}"); }
    }
}

[HarmonyPatch(typeof(CustomerBaseDoor), nameof(CustomerBaseDoor.InteractOnClick))]
internal static class Patch_CustomerBaseDoor_InteractOnClick
{
    internal static void Postfix()
    {
        try { EventDispatcher.FireCustomerDoorClicked(); }
        catch (Exception ex) { EventDispatcher.LogError($"CustomerBaseDoor.InteractOnClick: {ex.Message}"); }
    }
}

[HarmonyPatch(typeof(CustomerBaseDoor), nameof(CustomerBaseDoor.InteractOnHover))]
internal static class Patch_CustomerBaseDoor_InteractOnHover
{
    internal static void Postfix()
    {
        try { EventDispatcher.FireCustomerDoorHovered(); }
        catch (Exception ex) { EventDispatcher.LogError($"CustomerBaseDoor.InteractOnHover: {ex.Message}"); }
    }
}

[HarmonyPatch(typeof(CustomerBaseDoor), nameof(CustomerBaseDoor.OpenDoorAndSetupBase))]
internal static class Patch_CustomerBaseDoor_OpenDoorAndSetupBase
{
    internal static void Postfix()
    {
        try { EventDispatcher.FireCustomerDoorOpenedAndSetup(); }
        catch (Exception ex) { EventDispatcher.LogError($"CustomerBaseDoor.OpenDoorAndSetupBase: {ex.Message}"); }
    }
}

[HarmonyPatch(typeof(CustomerBaseDoor), nameof(CustomerBaseDoor.OpenDoor))]
internal static class Patch_CustomerBaseDoor_OpenDoor
{
    internal static void Postfix()
    {
        try { EventDispatcher.FireCustomerDoorOpened(); }
        catch (Exception ex) { EventDispatcher.LogError($"CustomerBaseDoor.OpenDoor: {ex.Message}"); }
    }
}

[HarmonyPatch(typeof(CustomerBaseDoor), nameof(CustomerBaseDoor.OnLoad))]
internal static class Patch_CustomerBaseDoor_OnLoad
{
    internal static void Postfix()
    {
        try { EventDispatcher.FireCustomerDoorLoaded(); }
        catch (Exception ex) { EventDispatcher.LogError($"CustomerBaseDoor.OnLoad: {ex.Message}"); }
    }
}

[HarmonyPatch(typeof(CustomerBaseDoor), nameof(CustomerBaseDoor.OnDestroy))]
internal static class Patch_CustomerBaseDoor_OnDestroy
{
    internal static void Postfix()
    {
        try { EventDispatcher.FireCustomerDoorDestroyed(); }
        catch (Exception ex) { EventDispatcher.LogError($"CustomerBaseDoor.OnDestroy: {ex.Message}"); }
    }
}

[HarmonyPatch(typeof(CustomerCard), nameof(CustomerCard.SetCustomer))]
internal static class Patch_CustomerCard_SetCustomer
{
    internal static void Postfix()
    {
        try { EventDispatcher.FireCustomerCardSet(); }
        catch (Exception ex) { EventDispatcher.LogError($"CustomerCard.SetCustomer: {ex.Message}"); }
    }
}

[HarmonyPatch(typeof(MainGameManager), nameof(MainGameManager.ButtonCancelCustomerChoice))]
internal static class Patch_MainGameManager_ButtonCancelCustomerChoice
{
    internal static void Postfix()
    {
        try { EventDispatcher.FireCustomerChoiceCanceled(); }
        catch (Exception ex) { EventDispatcher.LogError($"MainGameManager.ButtonCancelCustomerChoice: {ex.Message}"); }
    }
}

[HarmonyPatch(typeof(MainGameManager), nameof(MainGameManager.ShowCustomerCardsCanvas))]
internal static class Patch_MainGameManager_ShowCustomerCardsCanvas
{
    internal static void Postfix()
    {
        try { EventDispatcher.FireCustomerCardsCanvasShown(); }
        catch (Exception ex) { EventDispatcher.LogError($"MainGameManager.ShowCustomerCardsCanvas: {ex.Message}"); }
    }
}

[HarmonyPatch(typeof(MainGameManager), nameof(MainGameManager.CreateFallbackCustomer))]
internal static class Patch_MainGameManager_CreateFallbackCustomer
{
    internal static void Postfix(int __1)
    {
        try { EventDispatcher.FireCustomerFallbackCreated(__1); }
        catch (Exception ex) { EventDispatcher.LogError($"MainGameManager.CreateFallbackCustomer: {ex.Message}"); }
    }
}

[HarmonyPatch(typeof(MainGameManager), nameof(MainGameManager.GetCustomerTotalRequirement))]
internal static class Patch_MainGameManager_GetCustomerTotalRequirement
{
    internal static void Postfix()
    {
        try { EventDispatcher.FireCustomerTotalRequirementRequested(); }
        catch (Exception ex) { EventDispatcher.LogError($"MainGameManager.GetCustomerTotalRequirement: {ex.Message}"); }
    }
}

[HarmonyPatch(typeof(MainGameManager), nameof(MainGameManager.IsCustomerSuitableForBase))]
internal static class Patch_MainGameManager_IsCustomerSuitableForBase
{
    internal static void Postfix(ref bool __result)
    {
        try { EventDispatcher.FireCustomerSuitabilityChecked(__result); }
        catch (Exception ex) { EventDispatcher.LogError($"MainGameManager.IsCustomerSuitableForBase: {ex.Message}"); }
    }
}

[HarmonyPatch(typeof(NetworkMap), nameof(NetworkMap.RegisterCustomerBase))]
internal static class Patch_NetworkMap_RegisterCustomerBase
{
    internal static void Postfix(CustomerBase __0)
    {
        try { EventDispatcher.FireNetworkCustomerBaseRegistered(__0 != null ? __0.customerBaseID : -1); }
        catch (Exception ex) { EventDispatcher.LogError($"NetworkMap.RegisterCustomerBase: {ex.Message}"); }
    }
}

[HarmonyPatch(typeof(NetworkMap), nameof(NetworkMap.GetCustomerBase))]
internal static class Patch_NetworkMap_GetCustomerBase
{
    internal static void Postfix(int __0)
    {
        try { EventDispatcher.FireNetworkCustomerBaseRequested(__0); }
        catch (Exception ex) { EventDispatcher.LogError($"NetworkMap.GetCustomerBase: {ex.Message}"); }
    }
}

[HarmonyPatch(typeof(NetworkMap), nameof(NetworkMap.UpdateDeviceCustomerID))]
internal static class Patch_NetworkMap_UpdateDeviceCustomerID
{
    internal static void Postfix(int __1)
    {
        try { EventDispatcher.FireNetworkDeviceCustomerIdChanged(__1); }
        catch (Exception ex) { EventDispatcher.LogError($"NetworkMap.UpdateDeviceCustomerID: {ex.Message}"); }
    }
}

[HarmonyPatch(typeof(Server), nameof(Server.ButtonClickChangeCustomer))]
internal static class Patch_Server_ButtonClickChangeCustomer
{
    internal static void Postfix()
    {
        try { EventDispatcher.FireServerChangeCustomerClicked(); }
        catch (Exception ex) { EventDispatcher.LogError($"Server.ButtonClickChangeCustomer: {ex.Message}"); }
    }
}

[HarmonyPatch(typeof(Server), nameof(Server.GetNextCustomerID))]
internal static class Patch_Server_GetNextCustomerID
{
    internal static void Postfix(ref int __result)
    {
        try { EventDispatcher.FireServerNextCustomerIdRequested(__result); }
        catch (Exception ex) { EventDispatcher.LogError($"Server.GetNextCustomerID: {ex.Message}"); }
    }
}

[HarmonyPatch(typeof(Server), nameof(Server.GetCustomerID))]
internal static class Patch_Server_GetCustomerID
{
    internal static void Postfix(ref int __result)
    {
        try { EventDispatcher.FireServerCustomerIdRequested(__result); }
        catch (Exception ex) { EventDispatcher.LogError($"Server.GetCustomerID: {ex.Message}"); }
    }
}





