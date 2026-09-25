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
    private void LogInfoImpl(IntPtr msg) { _logger.Msg("[RustMod] " + (Marshal.PtrToStringAnsi(msg) ?? "")); }
    private void LogWarningImpl(IntPtr msg) { _logger.Warning("[RustMod] " + (Marshal.PtrToStringAnsi(msg) ?? "")); }
    private void LogErrorImpl(IntPtr msg) { _logger.Error("[RustMod] " + (Marshal.PtrToStringAnsi(msg) ?? "")); }

    private double GetPlayerMoneyImpl()
    {
        try { return GameHooks.GetPlayerMoney(); }
        catch (Exception ex) { _logger.Error("GetPlayerMoney: " + ex.Message); return 0.0; }
    }

    private void SetPlayerMoneyImpl(double value)
    {
        try { GameHooks.SetPlayerMoney((float)value); }
        catch (Exception ex) { _logger.Error("SetPlayerMoney: " + ex.Message); }
    }

    private float GetTimeScaleImpl()
    {
        try { return Time.timeScale; } catch { return 1.0f; }
    }

    private void SetTimeScaleImpl(float value)
    {
        try { Time.timeScale = value; }
        catch (Exception ex) { _logger.Error("SetTimeScale: " + ex.Message); }
    }

    private uint GetServerCountImpl() { try { return GameHooks.GetServerCount(); } catch { return 0; } }
    private uint GetRackCountImpl() { try { return GameHooks.GetRackCount(); } catch { return 0; } }

    private IntPtr GetCurrentSceneImpl()
    {
        try
        {
            var name = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name ?? "";
            if (_currentScenePtr != IntPtr.Zero) Marshal.FreeHGlobal(_currentScenePtr);
            _currentScenePtr = Marshal.StringToHGlobalAnsi(name);
            return _currentScenePtr;
        }
        catch { return IntPtr.Zero; }
    }

    private double GetPlayerXPImpl()
    {
        try { return GameHooks.GetPlayerXP(); }
        catch (Exception ex) { _logger.Error("GetPlayerXP: " + ex.Message); return 0.0; }
    }

    private void SetPlayerXPImpl(double value)
    {
        try { GameHooks.SetPlayerXP((float)value); }
        catch (Exception ex) { _logger.Error("SetPlayerXP: " + ex.Message); }
    }

    private double GetPlayerReputationImpl()
    {
        try { return GameHooks.GetPlayerReputation(); }
        catch (Exception ex) { _logger.Error("GetPlayerReputation: " + ex.Message); return 0.0; }
    }

    private void SetPlayerReputationImpl(double value)
    {
        try { GameHooks.SetPlayerReputation((float)value); }
        catch (Exception ex) { _logger.Error("SetPlayerReputation: " + ex.Message); }
    }

    private float GetTimeOfDayImpl() { try { return GameHooks.GetTimeOfDay(); } catch { return 0f; } }
    private uint GetDayImpl() { try { return (uint)Math.Max(0, GameHooks.GetDay()); } catch { return 0; } }
    private float GetSecondsInFullDayImpl() { try { return GameHooks.GetSecondsInFullDay(); } catch { return 0f; } }

    private void SetSecondsInFullDayImpl(float value)
    {
        try { GameHooks.SetSecondsInFullDay(value); }
        catch (Exception ex) { _logger.Error("SetSecondsInFullDay: " + ex.Message); }
    }

    private uint GetSwitchCountImpl() { try { return GameHooks.GetSwitchCount(); } catch { return 0; } }
    private uint GetSatisfiedCustomerCountImpl() { try { return (uint)Math.Max(0, GameHooks.GetSatisfiedCustomerCount()); } catch { return 0; } }

    private static bool _netWatchEnabled;

    private void SetNetWatchEnabledImpl(uint value)
    {
        _netWatchEnabled = value != 0;
    }

    private uint IsNetWatchEnabledImpl() { return _netWatchEnabled ? 1u : 0u; }
    private uint GetNetWatchStatsImpl() { return 0; }


    private uint GetBrokenServerCountImpl() { try { return GameHooks.GetBrokenServerCount(); } catch { return 0; } }
    private uint GetBrokenSwitchCountImpl() { try { return GameHooks.GetBrokenSwitchCount(); } catch { return 0; } }
    private uint GetEolServerCountImpl() { try { return GameHooks.GetEolServerCount(); } catch { return 0; } }
    private uint GetEolSwitchCountImpl() { try { return GameHooks.GetEolSwitchCount(); } catch { return 0; } }
    private uint GetFreeTechnicianCountImpl() { try { return GameHooks.GetFreeTechnicianCount(); } catch { return 0; } }
    private uint GetTotalTechnicianCountImpl() { try { return GameHooks.GetTotalTechnicianCount(); } catch { return 0; } }
    private int DispatchRepairServerImpl() { try { return GameHooks.DispatchRepairServer(); } catch { return 0; } }
    private int DispatchRepairSwitchImpl() { try { return GameHooks.DispatchRepairSwitch(); } catch { return 0; } }
    private int DispatchReplaceServerImpl() { try { return GameHooks.DispatchReplaceServer(); } catch { return 0; } }
    private int DispatchReplaceSwitchImpl() { try { return GameHooks.DispatchReplaceSwitch(); } catch { return 0; } }


    private int RegisterCustomEmployeeImpl(IntPtr employeeId, IntPtr name, IntPtr description, float salary, float requiredReputation, uint confirmDialogs)
    {
        try
        {
            string id = Marshal.PtrToStringAnsi(employeeId) ?? "";
            string n = Marshal.PtrToStringAnsi(name) ?? "";
            string desc = Marshal.PtrToStringAnsi(description) ?? "";
            CrashLog.Log($"RegisterCustomEmployee: id={id}, name={n}, salary={salary}, rep={requiredReputation}, confirmDialogs={confirmDialogs}");
            return CustomEmployeeManager.Register(id, n, desc, salary, requiredReputation, confirmDialogs != 0);
        }
        catch (Exception ex)
        {
            _logger.Error("RegisterCustomEmployee: " + ex.Message);
            CrashLog.LogException("RegisterCustomEmployee", ex);
            return 0;
        }
    }

    private uint IsCustomEmployeeHiredImpl(IntPtr employeeId)
    {
        try
        {
            string id = Marshal.PtrToStringAnsi(employeeId) ?? "";
            return CustomEmployeeManager.IsHired(id) ? 1u : 0u;
        }
        catch { return 0; }
    }

    private int FireCustomEmployeeImpl(IntPtr employeeId)
    {
        try
        {
            string id = Marshal.PtrToStringAnsi(employeeId) ?? "";
            return CustomEmployeeManager.Fire(id);
        }
        catch { return 0; }
    }

    private int RegisterSalaryImpl(int monthlySalary)
    {
        try
        {
            var bs = BalanceSheet.instance;
            if (bs == null) return 0;
            bs.RegisterSalary(monthlySalary);
            return 1;
        }
        catch (Exception ex)
        {
            CrashLog.LogException("RegisterSalary", ex);
            return 0;
        }
    }


    private int ShowNotificationImpl(IntPtr message)
    {
        try
        {
            string msg = Marshal.PtrToStringAnsi(message) ?? "";
            var ui = StaticUIElements.instance;
            if (ui == null) return 0;
            ui.AddMeesageInField(msg);
            return 1;
        }
        catch (Exception ex) { CrashLog.LogException("ShowNotification", ex); return 0; }
    }

    private float GetMoneyPerSecondImpl()
    {
        try
        {
            var ui = StaticUIElements.instance;
            if (ui == null) return 0f;
            ui.CalculateRates(out float money, out float _, out float _);
            return money;
        }
        catch { return 0f; }
    }

    private float GetExpensesPerSecondImpl()
    {
        try
        {
            var ui = StaticUIElements.instance;
            if (ui == null) return 0f;
            ui.CalculateRates(out float _, out float _, out float expenses);
            return expenses;
        }
        catch { return 0f; }
    }

    private float GetXpPerSecondImpl()
    {
        try
        {
            var ui = StaticUIElements.instance;
            if (ui == null) return 0f;
            ui.CalculateRates(out float _, out float xp, out float _);
            return xp;
        }
        catch { return 0f; }
    }

    private uint IsGamePausedImpl()
    {
        try { return MainGameManager.instance?.isGamePaused == true ? 1u : 0u; }
        catch { return 0; }
    }

    private void SetGamePausedImpl(uint paused)
    {
        try
        {
            var mgr = MainGameManager.instance;
            if (mgr != null) mgr.isGamePaused = paused != 0;
        }
        catch (Exception ex) { CrashLog.LogException("SetGamePaused", ex); }
    }

    private int GetDifficultyImpl()
    {
        try { return MainGameManager.instance?.difficulty ?? -1; }
        catch { return -1; }
    }

    private int TriggerSaveImpl()
    {
        try { SaveSystem.SaveGame(); return 1; }
        catch (Exception ex) { CrashLog.LogException("TriggerSave", ex); return 0; }
    }


    // Legacy v7 ABI slots are intentionally inert. Data Center owns Steam,
    // lobby and co-op lifecycle now; gregCore must not open a second session.
    private ulong SteamGetMyIdImpl() => 0;
    private IntPtr SteamGetFriendNameImpl(ulong steamId) => IntPtr.Zero;
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "S1172:Unused method parameters should be removed", Justification = "Native FFI delegate signature is fixed by the C ABI (see GameAPI.Bind.cs). Parameter intentionally unused in this inert stub.")]
    private int SteamCreateLobbyImpl(uint lobbyType, uint maxPlayers) { return 0; }
    private int SteamJoinLobbyImpl(ulong lobbyId) { return 0; }
    private void SteamLeaveLobbyImpl() { /* Intentionally inert: native Data Center owns lobby callbacks. */ }
    private ulong SteamGetLobbyIdImpl() { return 0; }
    private ulong SteamGetLobbyOwnerImpl() { return 0; }
    private uint SteamGetLobbyMemberCountImpl() { return 0; }
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "S1172:Unused method parameters should be removed", Justification = "Native FFI delegate signature is fixed by the C ABI (see GameAPI.Bind.cs). Parameter intentionally unused in this inert stub.")]
    private ulong SteamGetLobbyMemberByIndexImpl(uint index) { return 0; }
    private int SteamSetLobbyDataImpl(IntPtr key, IntPtr value) { return 0; }
    private IntPtr SteamGetLobbyDataImpl(IntPtr key) { return IntPtr.Zero; }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "S1172:Unused method parameters should be removed", Justification = "Native FFI delegate signature is fixed by the C ABI (see GameAPI.Bind.cs). Parameter intentionally unused in this inert stub.")]
    private int SteamSendP2PImpl(ulong target, IntPtr data, uint len, uint reliable) => 0;
    private uint SteamIsP2PAvailableImpl(IntPtr outSize) => 0;
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "S1172:Unused method parameters should be removed", Justification = "Native FFI delegate signature is fixed by the C ABI (see GameAPI.Bind.cs). Parameter intentionally unused in this inert stub.")]
    private uint SteamReadP2PImpl(IntPtr buf, uint bufLen, IntPtr outSender) => 0;
    private void SteamAcceptP2PImpl(ulong remote) { /* intentionally inert: native Data Center owns lobby callbacks */ }
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "S1172:Unused method parameters should be removed", Justification = "Native FFI delegate signature is fixed by the C ABI (see GameAPI.Bind.cs). Parameter intentionally unused in this inert stub.")]
    private uint SteamPollEventImpl(IntPtr outType, IntPtr outData)
    {
        // Intentionally inert: native Data Center owns lobby callbacks.
        return 0;
    }

    private void GetPlayerPositionImpl(IntPtr outX, IntPtr outY, IntPtr outZ, IntPtr outRy)
    {
        try
        {
            var pm = PlayerManager.instance;
            if (pm == null || pm.playerGO == null) return;

            var pos = pm.playerGO.transform.position;
            var rot = pm.playerGO.transform.eulerAngles;

            if (outX != IntPtr.Zero) Marshal.Copy(new float[] { pos.x }, 0, outX, 1);
            if (outY != IntPtr.Zero) Marshal.Copy(new float[] { pos.y }, 0, outY, 1);
            if (outZ != IntPtr.Zero) Marshal.Copy(new float[] { pos.z }, 0, outZ, 1);
            if (outRy != IntPtr.Zero) Marshal.Copy(new float[] { rot.y }, 0, outRy, 1);
        }
        catch (Exception ex) { CrashLog.LogException("GetPlayerPosition", ex); }
    }

    private static uint ConfigRegisterBoolImpl(IntPtr modId, IntPtr key, IntPtr displayName, uint defaultValue, IntPtr description)
    {
        try
        {
            string mId = Marshal.PtrToStringAnsi(modId) ?? "";
            string k = Marshal.PtrToStringAnsi(key) ?? "";
            string dn = Marshal.PtrToStringAnsi(displayName) ?? k;
            string desc = Marshal.PtrToStringAnsi(description) ?? "";
            return ModConfigSystem.RegisterBool(mId, k, dn, defaultValue != 0, desc);
        }
        catch (Exception ex) { CrashLog.LogException("ConfigRegisterBoolImpl", ex); return 0; }
    }

    private static uint ConfigRegisterIntImpl(IntPtr modId, IntPtr key, IntPtr displayName, int defaultValue, int min, int max, IntPtr description)
    {
        try
        {
            string mId = Marshal.PtrToStringAnsi(modId) ?? "";
            string k = Marshal.PtrToStringAnsi(key) ?? "";
            string dn = Marshal.PtrToStringAnsi(displayName) ?? k;
            string desc = Marshal.PtrToStringAnsi(description) ?? "";
            return ModConfigSystem.RegisterInt(mId, k, dn, defaultValue, min, max, desc);
        }
        catch (Exception ex) { CrashLog.LogException("ConfigRegisterIntImpl", ex); return 0; }
    }

    private static uint ConfigRegisterFloatImpl(IntPtr modId, IntPtr key, IntPtr displayName, float defaultValue, float min, float max, IntPtr description)
    {
        try
        {
            string mId = Marshal.PtrToStringAnsi(modId) ?? "";
            string k = Marshal.PtrToStringAnsi(key) ?? "";
            string dn = Marshal.PtrToStringAnsi(displayName) ?? k;
            string desc = Marshal.PtrToStringAnsi(description) ?? "";
            return ModConfigSystem.RegisterFloat(mId, k, dn, defaultValue, min, max, desc);
        }
        catch (Exception ex) { CrashLog.LogException("ConfigRegisterFloatImpl", ex); return 0; }
    }

    private static uint ConfigGetBoolImpl(IntPtr modId, IntPtr key)
    {
        try
        {
            string mId = Marshal.PtrToStringAnsi(modId) ?? "";
            string k = Marshal.PtrToStringAnsi(key) ?? "";
            return ModConfigSystem.GetBool(mId, k);
        }
        catch (Exception ex) { CrashLog.LogException("ConfigGetBoolImpl", ex); return 0xFFFFFFFF; }
    }

    private static int ConfigGetIntImpl(IntPtr modId, IntPtr key)
    {
        try
        {
            string mId = Marshal.PtrToStringAnsi(modId) ?? "";
            string k = Marshal.PtrToStringAnsi(key) ?? "";
            return ModConfigSystem.GetInt(mId, k);
        }
        catch (Exception ex) { CrashLog.LogException("ConfigGetIntImpl", ex); return 0; }
    }

    private static float ConfigGetFloatImpl(IntPtr modId, IntPtr key)
    {
        try
        {
            string mId = Marshal.PtrToStringAnsi(modId) ?? "";
            string k = Marshal.PtrToStringAnsi(key) ?? "";
            return ModConfigSystem.GetFloat(mId, k);
        }
        catch (Exception ex) { CrashLog.LogException("ConfigGetFloatImpl", ex); return 0f; }
    }

    private static uint SpawnCharacterImpl(uint prefabIdx, float x, float y, float z, float rotY, IntPtr name)
    {
        try
        {
            string n = Marshal.PtrToStringAnsi(name) ?? "Entity";
            return EntityManager.SpawnCharacter(prefabIdx, x, y, z, rotY, n);
        }
        catch (Exception ex) { CrashLog.LogException("SpawnCharacterImpl", ex); return 0; }
    }

    private static void DestroyEntityImpl(uint entityId)
    {
        try { EntityManager.DestroyEntity(entityId); }
        catch (Exception ex) { CrashLog.LogException("DestroyEntityImpl", ex); }
    }

    private static void SetEntityPositionImpl(uint entityId, float x, float y, float z, float rotY)
    {
        try { EntityManager.SetPosition(entityId, x, y, z, rotY); }
        catch (Exception ex) { CrashLog.LogException("SetEntityPositionImpl", ex); }
    }

    private static uint IsEntityReadyImpl(uint entityId)
    {
        try { return EntityManager.IsEntityReady(entityId) ? 1u : 0u; }
        catch (Exception ex) { CrashLog.LogException("IsEntityReadyImpl", ex); return 0; }
    }

    private static void SetEntityAnimationImpl(uint entityId, float speed, uint isWalking)
    {
        try { EntityManager.SetAnimation(entityId, speed, isWalking != 0); }
        catch (Exception ex) { CrashLog.LogException("SetEntityAnimationImpl", ex); }
    }

    private static uint GetPrefabCountImpl()
    {
        try { return EntityManager.GetPrefabCount(); }
        catch (Exception ex) { CrashLog.LogException("GetPrefabCountImpl", ex); return 0; }
    }

    private static void SetEntityNameImpl(uint entityId, IntPtr name)
    {
        try
        {
            string n = Marshal.PtrToStringAnsi(name) ?? "";
            EntityManager.SetEntityName(entityId, n);
        }
        catch (Exception ex) { CrashLog.LogException("SetEntityNameImpl", ex); }
    }

    private void GetPlayerCarryStateImpl(IntPtr outObjectInHand, IntPtr outNumObjects)
    {
        try
        {
            var pm = PlayerManager.instance;
            if (pm == null) return;
            uint objInHand = (uint)(int)pm.objectInHand;
            uint numObj = (uint)pm.numberOfObjectsInHand;
            if (outObjectInHand != IntPtr.Zero) Marshal.Copy(new int[] { (int)objInHand }, 0, outObjectInHand, 1);
            if (outNumObjects != IntPtr.Zero) Marshal.Copy(new int[] { (int)numObj }, 0, outNumObjects, 1);
        }
        catch (Exception ex) { CrashLog.LogException("GetPlayerCarryStateImpl", ex); }
    }

    private static uint GetPlayerCrouchingImpl()
    {
        try
        {
            var pm = PlayerManager.instance;
            if (pm == null || pm.fpc == null) return 0;
            return pm.fpc.m_isCrouching ? 1u : 0u;
        }
        catch (Exception ex) { CrashLog.LogException("GetPlayerCrouchingImpl", ex); return 0; }
    }

    private static uint GetPlayerSittingImpl()
    {
        try
        {
            var pm = PlayerManager.instance;
            if (pm == null || pm.fpc == null) return 0;
            return pm.fpc.m_IsSitting ? 1u : 0u;
        }
        catch (Exception ex) { CrashLog.LogException("GetPlayerSittingImpl", ex); return 0; }
    }

    private static void SetEntityCrouchingImpl(uint entityId, uint isCrouching)
    {
        try { EntityManager.SetCrouching(entityId, isCrouching != 0); }
        catch (Exception ex) { CrashLog.LogException("SetEntityCrouchingImpl", ex); }
    }

    private static void SetEntitySittingImpl(uint entityId, uint isSitting)
    {
        try { EntityManager.SetSitting(entityId, isSitting != 0); }
        catch (Exception ex) { CrashLog.LogException("SetEntitySittingImpl", ex); }
    }

    private static void SetEntityCarryAnimImpl(uint entityId, uint isCarrying)
    {
        try { EntityManager.SetCarryAnim(entityId, isCarrying != 0); }
        catch (Exception ex) { CrashLog.LogException("SetEntityCarryAnimImpl", ex); }
    }

    private static void CreateEntityCarryVisualImpl(uint entityId, uint objectInHandType)
    {
        try { EntityManager.CreateCarryVisual(entityId, objectInHandType); }
        catch (Exception ex) { CrashLog.LogException("CreateEntityCarryVisualImpl", ex); }
    }

    private static void DestroyEntityCarryVisualImpl(uint entityId)
    {
        try { EntityManager.DestroyCarryVisual(entityId); }
        catch (Exception ex) { CrashLog.LogException("DestroyEntityCarryVisualImpl", ex); }
    }

    private void GetDefaultSpawnPositionImpl(IntPtr outX, IntPtr outY, IntPtr outZ)
    {
        try
        {
            if (outX != IntPtr.Zero) Marshal.Copy(new float[] { 5f }, 0, outX, 1);
            if (outY != IntPtr.Zero) Marshal.Copy(new float[] { 1f }, 0, outY, 1);
            if (outZ != IntPtr.Zero) Marshal.Copy(new float[] { -24f }, 0, outZ, 1);
            CrashLog.Log("[GameAPI] Default spawn: (5, 1, -24)");
        }
        catch (Exception ex) { CrashLog.LogException("GetDefaultSpawnPosition", ex); }
    }

    private void WarpLocalPlayerImpl(float x, float y, float z)
    {
        try
        {
            var pm = PlayerManager.instance;
            if (pm == null || pm.playerClass == null || pm.playerGO == null) return;

            var pos = new Vector3(x, y, z);
            var rot = pm.playerGO.transform.rotation;
            pm.playerClass.WarpPlayer(pos, rot);
            CrashLog.Log($"[GameAPI] Warped local player to ({x:F1},{y:F1},{z:F1})");
        }
        catch (Exception ex) { CrashLog.LogException("WarpLocalPlayer", ex); }
    }

    private static uint GetEntityPositionImpl(uint entityId, IntPtr outX, IntPtr outY, IntPtr outZ)
    {
        try
        {
            var pos = EntityManager.GetEntityPosition(entityId);
            if (pos == null) return 0;
            var p = pos.Value;
            if (outX != IntPtr.Zero) Marshal.Copy(new float[] { p.x }, 0, outX, 1);
            if (outY != IntPtr.Zero) Marshal.Copy(new float[] { p.y }, 0, outY, 1);
            if (outZ != IntPtr.Zero) Marshal.Copy(new float[] { p.z }, 0, outZ, 1);
            return 1;
        }
        catch (Exception ex) { CrashLog.LogException("GetEntityPositionImpl", ex); return 0; }
    }

    private static void AddEntityColliderImpl(uint entityId)
    {
        try { EntityManager.AddEntityCollider(entityId); }
        catch (Exception ex) { CrashLog.LogException("AddEntityColliderImpl", ex); }
    }

    private static void SetEntityCarryTransformImpl(uint entityId, float posX, float posY, float posZ, float rotX, float rotY, float rotZ)
    {
        try { EntityManager.SetEntityCarryTransform(entityId, posX, posY, posZ, rotX, rotY, rotZ); }
        catch (Exception ex) { CrashLog.LogException("SetEntityCarryTransformImpl", ex); }
    }
}
