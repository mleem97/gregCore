using System;
using DataCenterModLoader;
using Il2Cpp;
using Il2CppTMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

namespace gregCore.API;

public static partial class CustomEmployeeManager
{
    private static void CreateCard(Transform grid, Transform template, CustomEmployeeEntry entry, string cardName)
    {
        var newCardObj = UnityEngine.Object.Instantiate(template.gameObject, grid);
        newCardObj.name = cardName;
        newCardObj.SetActive(true);
        var card = newCardObj.transform;
        AssignCardTexts(card, entry);
        SetupButtons(card, entry);
        SetPortrait(card, entry.EmployeeId);
        CrashLog.Log($"CustomEmployee: Card created for '{entry.Name}' (id={entry.EmployeeId}, hired={entry.IsHired})");
    }

    private static void UpdateCard(Transform card, CustomEmployeeEntry entry)
    {
        AssignCardTexts(card, entry);
        SetupButtons(card, entry);
    }

    /// <summary>
    /// Sets the three visible text fields on an employee card.
    /// Tries legacy paths, then name-hint scan, then positional fallback.
    /// This makes the card injection survive UI restructuring in game updates.
    /// </summary>
    private static void AssignCardTexts(Transform card, CustomEmployeeEntry entry)
    {
        string wantName = entry.Name;
        string wantSalary = $"Salary: {entry.SalaryPerHour:F0} / h";
        string wantRep = $"Required Reputation: {entry.RequiredReputation:F0}";
        bool nameSet = TrySetByPath(card, "VL/text_employeeName", wantName);
        bool salarySet = TrySetByPath(card, "VL/text_employeeSalary", wantSalary);
        bool repSet = TrySetByPath(card, "VL/text_requiredReputation", wantRep);
        if (nameSet && salarySet && repSet) return;
        try { ScanTextsByHint(card, wantName, wantSalary, wantRep, ref nameSet, ref salarySet, ref repSet); }
        catch (Exception ex) { CrashLog.LogException("AssignCardTexts fallback scan", ex); }
        CrashLog.Log($"AssignCardTexts: name={nameSet}, salary={salarySet}, rep={repSet} for '{entry.EmployeeId}'");
    }

    private static void ScanTextsByHint(Transform card, string wantName, string wantSalary, string wantRep, ref bool nameSet, ref bool salarySet, ref bool repSet)
    {
        var textTransforms = new System.Collections.Generic.List<Transform>();
        CollectTextTransforms(card, textTransforms);
        CrashLog.Log($"AssignCardTexts: found {textTransforms.Count} text component(s) in card '{card.name}'");
        foreach (var t in textTransforms)
            TryMatchHint(t, wantName, wantSalary, wantRep, ref nameSet, ref salarySet, ref repSet);
        if (!nameSet || !salarySet || !repSet)
            AssignByPosition(textTransforms, wantName, wantSalary, wantRep, ref nameSet, ref salarySet, ref repSet);
    }

    private static void TryMatchHint(Transform t, string wantName, string wantSalary, string wantRep, ref bool nameSet, ref bool salarySet, ref bool repSet)
    {
        try
        {
            string n = t.name.ToLowerInvariant();
            if (!nameSet && (n.Contains("name") || n.Contains("employee")))
            {
                nameSet = TrySetTextOnTransform(t, wantName);
                return;
            }
            if (!salarySet && (n.Contains("salary") || n.Contains("pay") || n.Contains("cost") || n.Contains("wage")))
            {
                salarySet = TrySetTextOnTransform(t, wantSalary);
                return;
            }
            if (!repSet && (n.Contains("rep") || n.Contains("reputation") || n.Contains("prestige")))
                repSet = TrySetTextOnTransform(t, wantRep);
        }
        catch { /* ignored: defensive best-effort (CONVENTIONS.md) */  }
    }

    private static void AssignByPosition(System.Collections.Generic.List<Transform> texts, string wantName, string wantSalary, string wantRep, ref bool nameSet, ref bool salarySet, ref bool repSet)
    {
        try
        {
            int pos = 0;
            foreach (var t in texts)
            {
                if (pos == 0 && !nameSet) { TrySetTextOnTransform(t, wantName); nameSet = true; }
                else if (pos == 1 && !salarySet) { TrySetTextOnTransform(t, wantSalary); salarySet = true; }
                else if (pos == 2 && !repSet) { TrySetTextOnTransform(t, wantRep); repSet = true; }
                pos++;
                if (nameSet && salarySet && repSet) break;
            }
        }
        catch { /* ignored: defensive best-effort (CONVENTIONS.md) */  }
    }

    private static bool TrySetByPath(Transform card, string path, string text)
    {
        var t = card.Find(path);
        if (t == null) return false;
        return TrySetTextOnTransform(t, text);
    }

    /// Collect all transforms that have a TextMeshProUGUI or legacy Text component,
    /// skipping sub-trees that are buttons (to avoid overwriting button labels).
    private static void CollectTextTransforms(Transform parent, System.Collections.Generic.List<Transform> result)
    {
        for (int i = 0; i < parent.childCount; i++)
        {
            var child = parent.GetChild(i);
            if (IsButtonSubtree(child)) continue;
            if (HasTextComponent(child)) result.Add(child);
            CollectTextTransforms(child, result);
        }
    }

    private static bool IsButtonSubtree(Transform child)
    {
        try { if (child.GetComponent<UnityEngine.UI.Button>() != null) return true; } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */  }
        try { if (child.GetComponent<ButtonExtended>() != null) return true; } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */  }
        return false;
    }

    private static bool HasTextComponent(Transform child)
    {
        try { if (child.GetComponent<TextMeshProUGUI>() != null) return true; } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */  }
        return false;
    }

    private static void SetTextAtPath(Transform root, string path, string text)
    {
        var target = root.Find(path);
        if (target == null)
        {
            CrashLog.Log($"CustomEmployee: Path '{path}' not found under '{root.name}'");
            return;
        }
        if (!TrySetTextOnTransform(target, text))
            CrashLog.Log($"CustomEmployee: No text component at '{path}'");
    }

    private static bool TrySetTextOnTransform(Transform t, string text)
    {
        if (t == null) return false;
        if (TrySetTmpText(t, text)) return true;
        return TrySetLegacyText(t, text);
    }

    private static bool TrySetTmpText(Transform t, string text)
    {
        try
        {
            var tmp = t.GetComponent<TextMeshProUGUI>();
            if (tmp == null) return false;
            tmp.text = text;
            return true;
        }
        catch (Exception ex)
        {
            CrashLog.LogException("TrySetText TMP", ex);
            return false;
        }
    }

    private static bool TrySetLegacyText(Transform t, string text)
    {
        try
        {
            var legacyText = t.GetComponent<Text>();
            if (legacyText == null) return false;
            legacyText.text = text;
            return true;
        }
        catch (Exception ex)
        {
            CrashLog.LogException("TrySetText legacy", ex);
            return false;
        }
    }

    /// <summary>
    /// Finds a button transform inside <paramref name="card"/> using a prioritised search:
    /// 1. Exact path (e.g. "VL/ButtonHire")
    /// 2. Any direct or deep child whose name contains <paramref name="nameHint"/>
    /// 3. Returns null if nothing matched.
    /// </summary>
    private static Transform? FindButton(Transform card, string exactPath, string nameHint)
    {
        var t = card.Find(exactPath);
        if (t != null) return t;
        return FindChildContaining(card, nameHint);
    }

    private static Transform? FindChildContaining(Transform parent, string nameHint)
    {
        for (int i = 0; i < parent.childCount; i++)
        {
            var child = parent.GetChild(i);
            if (child.name.IndexOf(nameHint, System.StringComparison.OrdinalIgnoreCase) >= 0)
                return child;
            var deep = FindChildContaining(child, nameHint);
            if (deep != null) return deep;
        }
        return null;
    }

    private static void SetupButtons(Transform card, CustomEmployeeEntry entry)
    {
        Transform buttonHireT = FindButton(card, "VL/ButtonHire", "Hire")!;
        Transform buttonFireT = FindButton(card, "VL/ButtonFire", "Fire")!;
        FallbackToButtonOrder(card, ref buttonHireT, ref buttonFireT);
        if (buttonHireT == null || buttonFireT == null)
        {
            CrashLog.Log($"CustomEmployee: SetupButtons — could not find hire/fire buttons for '{entry.EmployeeId}' (hire={buttonHireT != null}, fire={buttonFireT != null})");
            return;
        }
        CrashLog.Log($"CustomEmployee: SetupButtons — using hire='{buttonHireT.name}', fire='{buttonFireT.name}' for '{entry.EmployeeId}'");
        UpdateButtonVisibility(buttonHireT, buttonFireT, entry);
        SetButtonLabels(buttonHireT, buttonFireT);
        WireHireFireButtons(buttonHireT, buttonFireT, entry);
        CrashLog.Log($"CustomEmployee: Buttons configured for '{entry.EmployeeId}' (hired={entry.IsHired})");
    }

    private static void FallbackToButtonOrder(Transform card, ref Transform hireT, ref Transform fireT)
    {
        if (hireT != null && fireT != null) return;
        try
        {
            var allBtns = card.GetComponentsInChildren<ButtonExtended>(includeInactive: true);
            if (allBtns == null) return;
            if (hireT == null && allBtns.Length >= 1) hireT = allBtns[0].transform;
            if (fireT == null && allBtns.Length >= 2) fireT = allBtns[1].transform;
        }
        catch (Exception ex)
        {
            CrashLog.LogException("SetupButtons: ButtonExtended fallback", ex);
        }
    }

    private static void UpdateButtonVisibility(Transform hireT, Transform fireT, CustomEmployeeEntry entry)
    {
        try
        {
            hireT.gameObject.SetActive(!entry.IsHired);
            fireT.gameObject.SetActive(entry.IsHired);
        }
        catch { /* ignored: defensive best-effort (CONVENTIONS.md) */  }
    }

    private static void SetButtonLabels(Transform hireT, Transform fireT)
    {
        try
        {
            bool hireTextSet = TrySetTextOnTransform(hireT.Find("TextHire"), "Hire");
            if (!hireTextSet) TrySetTextOnTransform(hireT, "Hire");
            bool fireTextSet = TrySetTextOnTransform(fireT.Find("TextHire"), "Fire");
            if (!fireTextSet) TrySetTextOnTransform(fireT, "Fire");
        }
        catch { /* ignored: defensive best-effort (CONVENTIONS.md) */  }
    }

    private static void WireHireFireButtons(Transform hireT, Transform fireT, CustomEmployeeEntry entry)
    {
        string employeeId = entry.EmployeeId;
        WireButtonExtendedClick(hireT, () => OnHireClicked(entry, employeeId));
        WireButtonExtendedClick(fireT, () => OnFireClicked(entry, employeeId));
    }

    private static void OnHireClicked(CustomEmployeeEntry entry, string employeeId)
    {
        CrashLog.Log($"CustomEmployee: Hire clicked for '{employeeId}'");
        if (entry.RequiresConfirmation)
        {
            _pendingEmployeeId = employeeId;
            _pendingIsHire = true;
            ShowOverlay(hire: true);
        }
        else if (Hire(employeeId) == 1) RefreshAllCards();
    }

    private static void OnFireClicked(CustomEmployeeEntry entry, string employeeId)
    {
        CrashLog.Log($"CustomEmployee: Fire clicked for '{employeeId}'");
        if (entry.RequiresConfirmation)
        {
            _pendingEmployeeId = employeeId;
            _pendingIsHire = false;
            ShowOverlay(hire: false);
        }
        else if (Fire(employeeId) == 1) RefreshAllCards();
    }

    private static void ShowOverlay(bool hire)
    {
        var hrSystems = UnityEngine.Object.FindObjectsOfType<HRSystem>();
        if (hrSystems == null) return;
        for (int i = 0; i < hrSystems.Length; i++)
        {
            var hr = hrSystems[i];
            if (hr == null || !hr.gameObject.activeInHierarchy) continue;
            var overlay = hire ? hr.confirmHireOverlay : hr.confirmFireOverlay;
            overlay?.SetActive(true);
            return;
        }
    }

    // ButtonExtended is a Selectable subclass with its own onClick; falls back to standard Button.
    private static void WireButtonExtendedClick(Transform? buttonTransform, System.Action callback)
    {
        if (buttonTransform == null) return;
        if (TryWireExtended(buttonTransform, callback)) return;
        TryWireStandard(buttonTransform, callback);
    }

    private static bool TryWireExtended(Transform buttonTransform, System.Action callback)
    {
        try
        {
            var btnExt = buttonTransform.GetComponent<ButtonExtended>();
            if (btnExt == null) return false;
            var freshEvent = new ButtonExtended.ButtonClickedEvent();
            btnExt.m_OnClick = freshEvent;
            UnityAction action = callback;
            _liveCallbacks.Add(action);
            freshEvent.AddListener(action);
            CrashLog.Log($"CustomEmployee: Wired ButtonExtended.onClick on '{buttonTransform.name}'");
            return true;
        }
        catch (Exception ex)
        {
            CrashLog.LogException($"WireButtonExtendedClick on '{buttonTransform.name}'", ex);
            return false;
        }
    }

    private static void TryWireStandard(Transform buttonTransform, System.Action callback)
    {
        try
        {
            var button = buttonTransform.GetComponent<Button>();
            if (button == null)
            {
                CrashLog.Log($"CustomEmployee: No ButtonExtended or Button on '{buttonTransform.name}'");
                return;
            }
            button.onClick = new Button.ButtonClickedEvent();
            UnityAction action = callback;
            _liveCallbacks.Add(action);
            button.onClick.AddListener(action);
            CrashLog.Log($"CustomEmployee: Wired Button.onClick fallback on '{buttonTransform.name}'");
        }
        catch (Exception ex)
        {
            CrashLog.LogException($"WireButtonClick fallback on '{buttonTransform.name}'", ex);
        }
    }
}
