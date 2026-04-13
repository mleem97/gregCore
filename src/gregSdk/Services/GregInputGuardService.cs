using System;
using Il2CppTMPro;
using MelonLoader;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace greg.Sdk.Services;

/// <summary>
/// Framework-level input guard service. Prevents hotkey handlers from firing
/// when the user is typing into a UI input field (InputField or TMP_InputField).
/// 
/// Integrated from gregAddons/gregMods/gregMod.ConsoleInputGuard.
/// All mods should check GregInputGuardService.IsTypingInInputField() before
/// processing hotkey input.
/// </summary>
public static class GregInputGuardService
{
    /// <summary>
    /// Returns true if the user is currently focused on a text input field.
    /// Mods should skip hotkey processing when this returns true.
    /// </summary>
    public static bool IsTypingInInputField()
    {
        try
        {
            var eventSystem = EventSystem.current;
            if (eventSystem == null) return false;

            var selected = eventSystem.currentSelectedGameObject;
            if (selected == null) return false;

            if (selected.GetComponent<InputField>() != null)
                return true;

            if (selected.GetComponent<TMP_InputField>() != null)
                return true;
        }
        catch (Exception ex)
        {
            MelonLogger.Warning($"[GregInputGuardService] Check failed: {ex.Message}");
        }

        return false;
    }
}
