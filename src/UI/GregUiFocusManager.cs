using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;
using MelonLoader;

namespace greg.Core.UI;

/// <summary>
/// Manages UI focus, cursor visibility, and player controller state.
/// </summary>
public static class GregUiFocusManager
{
    private static bool _isFocusing = false;
    private static FirstPersonController _cachedController;

    public static void RequestFocus(bool focus)
    {
        if (focus == _isFocusing) return;
        _isFocusing = focus;

        if (_cachedController == null)
        {
            _cachedController = Object.FindObjectOfType<FirstPersonController>();
        }

        if (focus)
        {
            MelonLogger.Msg("[GregUiFocusManager] Requesting UI Focus. Locking Camera.");
            if (_cachedController != null)
            {
                _cachedController.enabled = false;
            }
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            MelonLogger.Msg("[GregUiFocusManager] Releasing UI Focus. Unlocking Camera.");
            if (_cachedController != null)
            {
                _cachedController.enabled = true;
            }
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }
}
