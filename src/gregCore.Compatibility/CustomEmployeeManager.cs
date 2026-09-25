using System;
using System.Collections.Generic;
using System.IO;
using DataCenterModLoader;
using Il2Cpp;
using MelonLoader.Utils;
using Il2CppTMPro;
using MelonLoader;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

namespace gregCore.API;

public class CustomEmployeeEntry
{
    public string EmployeeId { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public float SalaryPerHour { get; set; }
    public float RequiredReputation { get; set; }
    public bool IsHired { get; set; }
    public bool RequiresConfirmation { get; set; }
}

public static partial class CustomEmployeeManager
{
    private static readonly List<CustomEmployeeEntry> _employees = new();
    private static readonly Dictionary<string, int> _employeeIndex = new();
    private static readonly List<UnityAction> _liveCallbacks = new();

    private static readonly string _statePath =
        Path.Combine(MelonEnvironment.UserDataDirectory, "custom_employees_hired.txt");

    private static string? _pendingEmployeeId;
    private static bool _pendingIsHire;
    private static bool _salariesNeedReregistration;

#pragma warning disable CS0414
    private static bool _scrollViewInjected = false;
    private static Transform? _injectedContent = null;
#pragma warning restore CS0414
    private static bool _hierarchyLogged = false;

    public static IReadOnlyList<CustomEmployeeEntry> Employees => _employees;
    public static bool HasPendingAction => _pendingEmployeeId != null;

    public static int Register(string id, string name, string description, float salary, float reputation)
    {
        return Register(id, name, description, salary, reputation, false);
    }

    public static int Register(string id, string name, string description, float salary, float reputation, bool requiresConfirmation)
    {
        if (!IsValidRegistration(id, name, description)) return 0;
        if (_employeeIndex.ContainsKey(id))
        {
            CrashLog.Log($"CustomEmployee: duplicate registration rejected for id={id}");
            return 0;
        }
        AddEntry(id, name, description, salary, reputation, requiresConfirmation);
        LoadState();
        return 1;
    }

    private static bool IsValidRegistration(string id, string name, string description)
    {
        if (string.IsNullOrEmpty(id)) return false;
        if (id.Length > 64 || (name != null && name.Length > 128) || (description != null && description.Length > 1024))
        {
            CrashLog.Log($"[Security] CustomEmployee: Input exceeded maximum allowed length for id={id}");
            return false;
        }
        if (id.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0 || id.Contains(".."))
        {
            CrashLog.Log($"[Security] CustomEmployee: Invalid characters in id={id}");
            return false;
        }
        return true;
    }

    private static void AddEntry(string id, string name, string description, float salary, float reputation, bool requiresConfirmation)
    {
        try
        {
            var entry = new CustomEmployeeEntry
            {
                EmployeeId = id,
                Name = name ?? "Unknown",
                Description = description ?? "",
                SalaryPerHour = salary,
                RequiredReputation = reputation,
                IsHired = false,
                RequiresConfirmation = requiresConfirmation,
            };
            _employeeIndex[id] = _employees.Count;
            _employees.Add(entry);
            CrashLog.Log($"CustomEmployee registered: id={id}, name={name}, salary={salary}/h, requiredRep={reputation}");
            MelonLogger.Msg($"[CustomEmployee] Registered: {name} (id={id}, salary={salary}/h, rep={reputation})");
        }
        catch { /* ignored: defensive best-effort (CONVENTIONS.md) */  }
    }

    public static bool IsHired(string id)
    {
        if (string.IsNullOrEmpty(id)) return false;
        if (_employeeIndex.TryGetValue(id, out int idx))
            return _employees[idx].IsHired;
        return false;
    }

    // Returns: 1 = hired, 0 = not found, -1 = insufficient reputation, -2 = already hired
    public static int Hire(string id)
    {
        if (!_employeeIndex.TryGetValue(id, out int idx)) return 0;
        var entry = _employees[idx];
        if (entry.IsHired) return -2;
        if (!HasRequiredReputation(entry)) return -1;
        CompleteHire(entry, id);
        return 1;
    }

    private static bool HasRequiredReputation(CustomEmployeeEntry entry)
    {
        float playerRep = 0f;
        try { playerRep = PlayerManager.instance?.playerClass?.reputation ?? 0f; } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */  }
        if (playerRep >= entry.RequiredReputation) return true;
        CrashLog.Log($"CustomEmployee hire rejected: {entry.EmployeeId} requires rep {entry.RequiredReputation}, player has {playerRep}");
        MelonLogger.Warning($"[CustomEmployee] Cannot hire {entry.Name}: need reputation {entry.RequiredReputation} (you have {playerRep:F0})");
        return false;
    }

    private static void CompleteHire(CustomEmployeeEntry entry, string id)
    {
        try
        {
            entry.IsHired = true;
            CrashLog.Log($"CustomEmployee hired: {id} ({entry.Name})");
            MelonLogger.Msg($"[CustomEmployee] Hired: {entry.Name}");
            try { BalanceSheet.instance?.RegisterSalary((int)entry.SalaryPerHour); } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */  }
            EventDispatcher.FireCustomEmployeeHired(id);
            SaveState();
        }
        catch { /* ignored: defensive best-effort (CONVENTIONS.md) */  }
    }

    // Returns: 1 = fired, 0 = not found or not currently hired
    public static int Fire(string id)
    {
        if (!_employeeIndex.TryGetValue(id, out int idx)) return 0;
        var entry = _employees[idx];
        if (!entry.IsHired) return 0;
        entry.IsHired = false;
        CrashLog.Log($"CustomEmployee.Fire: step 1 - set IsHired=false for '{id}' ({entry.Name})");
        LogFire(entry);
        UnregisterFireSalary(entry);
        DispatchFireEvent(id);
        CrashLog.Log("CustomEmployee.Fire: step 4 - saving state");
        SaveState();
        CrashLog.Log("CustomEmployee.Fire: step 5 - complete");
        return 1;
    }

    private static void LogFire(CustomEmployeeEntry entry)
    {
        try { MelonLogger.Msg($"[CustomEmployee] Fired: {entry.Name}"); }
        catch (Exception ex) { CrashLog.LogException("Fire: Logger.Msg", ex); }
    }

    private static void UnregisterFireSalary(CustomEmployeeEntry entry)
    {
        CrashLog.Log($"CustomEmployee.Fire: step 2 - about to unregister salary ({-(int)entry.SalaryPerHour})");
        try
        {
            var bs = BalanceSheet.instance;
            if (bs != null)
            {
                bs.RegisterSalary(-(int)entry.SalaryPerHour);
                CrashLog.Log("CustomEmployee.Fire: step 2 - salary unregistered OK");
            }
            else CrashLog.Log("CustomEmployee.Fire: step 2 - BalanceSheet.instance is null, skipping salary");
        }
        catch (Exception ex) { CrashLog.LogException("Fire: RegisterSalary", ex); }
    }

    private static void DispatchFireEvent(string id)
    {
        CrashLog.Log($"CustomEmployee.Fire: step 3 - dispatching CustomEmployeeFired event for '{id}'");
        try
        {
            EventDispatcher.FireCustomEmployeeHired(id);
            CrashLog.Log("CustomEmployee.Fire: step 3 - event dispatched OK");
        }
        catch (Exception ex) { CrashLog.LogException("Fire: FireCustomEmployeeFired", ex); }
    }

    public static void ResetInjectionState()
    {
        _injectedContent = null;
        _scrollViewInjected = false;
        _hierarchyLogged = false;
        CrashLog.Log("CustomEmployee: injection state reset");
    }

    // Called from Harmony prefix on ButtonConfirmHire. Returns true if handled (skip vanilla).
    public static bool HandleConfirmHire(HRSystem hr)
    {
        if (_pendingEmployeeId == null || !_pendingIsHire) return false;
        string id = _pendingEmployeeId;
        _pendingEmployeeId = null;
        CrashLog.Log($"HandleConfirmHire: confirming hire for '{id}'");
        Hire(id);
        hr.confirmHireOverlay?.SetActive(false);
        RefreshAllCards();
        return true;
    }

    // Called from Harmony prefix on ButtonConfirmFireEmployee. Returns true if handled.
    public static bool HandleConfirmFire(HRSystem hr)
    {
        if (_pendingEmployeeId == null || _pendingIsHire) return false;
        string id = _pendingEmployeeId;
        _pendingEmployeeId = null;
        CrashLog.Log($"HandleConfirmFire: confirming fire for '{id}'");
        Fire(id);
        hr.confirmFireOverlay?.SetActive(false);
        RefreshAllCards();
        return true;
    }

    public static void ClearPending() => _pendingEmployeeId = null;

    public static void SaveState()
    {
        try
        {
            var lines = new List<string>();
            foreach (var e in _employees)
                if (e.IsHired)
                    lines.Add(e.EmployeeId);
            File.WriteAllLines(_statePath, lines);
        }
        catch (Exception ex) { CrashLog.LogException("SaveState", ex); }
    }

    public static void LoadState()
    {
        try
        {
            if (!File.Exists(_statePath)) return;
            var hired = new HashSet<string>(File.ReadAllLines(_statePath));
            bool anyRestored = RestoreHiredFlags(hired);
            if (anyRestored) MarkSalariesForReregistration();
        }
        catch (Exception ex) { CrashLog.LogException("LoadState", ex); }
    }

    private static bool RestoreHiredFlags(HashSet<string> hired)
    {
        bool anyRestored = false;
        try
        {
            foreach (var e in _employees)
            {
                if (!hired.Contains(e.EmployeeId)) continue;
                e.IsHired = true;
                anyRestored = true;
                CrashLog.Log($"LoadState: restored IsHired for '{e.EmployeeId}' ({e.Name}), salary={e.SalaryPerHour}");
            }
        }
        catch { /* ignored: defensive best-effort (CONVENTIONS.md) */  }
        return anyRestored;
    }

    private static void MarkSalariesForReregistration()
    {
        _salariesNeedReregistration = true;
        CrashLog.Log("LoadState: salaries need re-registration (deferred until BalanceSheet is ready)");
    }

    // Deferred until BalanceSheet is available. Only acts once after LoadState sets the flag.
    public static void ReregisterSalariesIfNeeded()
    {
        if (!_salariesNeedReregistration) return;
        try
        {
            if (BalanceSheet.instance == null) return;
            int count = ReregisterAllSalaries();
            _salariesNeedReregistration = false;
            CrashLog.Log($"ReregisterSalariesIfNeeded: done, re-registered {count} salary entries");
        }
        catch (Exception ex) { CrashLog.LogException("ReregisterSalariesIfNeeded", ex); }
    }

    private static int ReregisterAllSalaries()
    {
        int count = 0;
        try
        {
            foreach (var e in _employees)
            {
                if (!e.IsHired) continue;
                BalanceSheet.instance.RegisterSalary((int)e.SalaryPerHour);
                count++;
                CrashLog.Log($"ReregisterSalariesIfNeeded: registered salary {e.SalaryPerHour} for '{e.EmployeeId}' ({e.Name})");
            }
        }
        catch { /* ignored: defensive best-effort (CONVENTIONS.md) */  }
        return count;
    }
}
