using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace gregCore.Infrastructure.UI;

internal static class GregUiManager
{
    private static EventSystem? _disabledEventSystem;
    private static int _reenableCounter;

    public static bool IsAnyUiOpen { get; private set; }

    public static void RegisterUiOpen()
    {
        IsAnyUiOpen = true;
        DisableGameInput();
    }

    public static void RegisterUiClosed()
    {
        IsAnyUiOpen = false;
        _reenableCounter = 2; // Defer by 2 frames to catch mouse-up
    }

    private static void DisableGameInput()
    {
        try
        {
            var es = EventSystem.current;
            if (es != null && es.enabled)
            {
                _disabledEventSystem = es;
                es.enabled = false;
            }
        }
        catch { }
    }

    public static void OnUpdate()
    {
        if (_reenableCounter > 0)
        {
            _reenableCounter--;
            if (_reenableCounter <= 0 && _disabledEventSystem != null)
            {
                try { _disabledEventSystem.enabled = true; } catch { }
                _disabledEventSystem = null;
            }
        }
    }
}
