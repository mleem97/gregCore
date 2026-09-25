/// <file-summary>
/// Layer:       UI
/// Purpose:     Central input system for mod UIs (UI Toolkit + UGUI).
///              The game provides no active EventSystem during gameplay,
///              so this system creates a persistent one on demand,
///              an EventSystem with InputSystemUIInputModule (programmatic
///              default actions: mouse + touch). If an active
///              EventSystem already exists (e.g. game menu), it is used.
/// Maintainer:  No assumptions about game UI. Only create, never destroy.
/// </file-summary>

using System.Diagnostics.CodeAnalysis;
using MelonLoader;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;

namespace gregCore.UI;

[ExcludeFromCodeCoverage(Justification = "Thin shell over live Unity input + scene objects; needs running game.")]
public static class GregUIInputSystem
{
    private static GameObject _eventSystemObj;
    private static bool _ensured;

    // True when an EventSystem is available for mod UIs (found or created).
    public static bool HasEventSystem { get; private set; }

    // Ensures Toolkit/UGUI clicks are delivered.
    // Cheap after the first call (null checks only). Return: ready?
    public static bool Ensure()
    {
        try
        {
            var current = EventSystem.current;
            if (current != null && current.isActiveAndEnabled)
            {
                HasEventSystem = true;
                MelonLogger.Msg("[gregCore][UI] EventSystem present (game).");
                return true;
            }
            if (_ensured && _eventSystemObj != null)
            {
                HasEventSystem = true;
                return true;
            }
            Create();
            _ensured = true;
            HasEventSystem = _eventSystemObj != null;
            return HasEventSystem;
        }
        catch (Exception ex)
        {
            MelonLogger.Warning($"[gregCore][UI] EventSystem-Setup failed: {ex.GetBaseException().Message}");
            return false;
        }
    }

    private static void Create()
    {
        var go = new GameObject("gregCoreEventSystem");
        UnityEngine.Object.DontDestroyOnLoad(go);
        var es = go.AddComponent<EventSystem>();
        es.sendNavigationEvents = true;
        es.pixelDragThreshold = 10;

        var ui = go.AddComponent<InputSystemUIInputModule>();
        ui.point = MakeAction("GregPoint", InputActionType.Value, "Vector2", "<Mouse>/position", "<Touchscreen>/touch*/position");
        ui.leftClick = MakeAction("GregLeftClick", InputActionType.Button, "Button", "<Mouse>/leftButton", "<Touchscreen>/touch*/press");
        ui.rightClick = MakeAction("GregRightClick", InputActionType.Button, "Button", "<Mouse>/rightButton");
        ui.middleClick = MakeAction("GregMiddleClick", InputActionType.Button, "Button", "<Mouse>/middleButton");
        ui.scrollWheel = MakeAction("GregScroll", InputActionType.Value, "Vector2", "<Mouse>/scroll");
        ui.move = MakeAction("GregMove", InputActionType.Value, "Vector2", "<Gamepad>/leftStick", "<Joystick>/stick");
        ui.submit = MakeAction("GregSubmit", InputActionType.Button, "Button", "<Keyboard>/enter", "<Keyboard>/numpadEnter", "<Gamepad>/buttonSouth");
        ui.cancel = MakeAction("GregCancel", InputActionType.Button, "Button", "<Keyboard>/escape", "<Gamepad>/buttonEast");

        _eventSystemObj = go;
        MelonLogger.Msg("[gregCore][UI] EventSystem created for mod UIs.");
    }

    private static InputActionReference MakeAction(string name, InputActionType type, string controlType, params string[] bindings)
    {
        var action = new InputAction(name, type, controlType);
        foreach (string b in bindings)
        {
            try { action.AddBinding(b); } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
        }
        try { action.Enable(); } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
        var reference = ScriptableObject.CreateInstance<InputActionReference>();
        try { reference.Set(action); } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
        return reference;
    }
}
