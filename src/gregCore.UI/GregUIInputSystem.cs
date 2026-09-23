/// <file-summary>
/// Schicht:      UI
/// Zweck:        Zentrales Eingabe-System fuer Mod-UIs (UI Toolkit + UGUI).
///               Das Spiel stellt im Gameplay kein aktives EventSystem bereit,
///               daher erzeugt dieses System bei Bedarf ein persistentes
///               EventSystem mit InputSystemUIInputModule (programmatische
///               Standard-Aktionen: Maus + Touch). Existiert bereits ein
///               aktives EventSystem (z.B. Spiel-Menue), wird es benutzt.
/// Maintainer:   Keine Annahmen ueber Spiel-UI. Erzeugen nur, nie zerstoeren.
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

    // True, wenn ein EventSystem fuer Mod-UIs bereitsteht (gefunden oder erzeugt).
    public static bool HasEventSystem { get; private set; }

    // Stellt sicher, dass Toolkit/UGUI-Klicks zugestellt werden.
    // Billig nach dem ersten Aufruf (nur Null-Checks). Rueckgabe: bereit?
    public static bool Ensure()
    {
        try
        {
            var current = EventSystem.current;
            if (current != null && current.isActiveAndEnabled)
            {
                HasEventSystem = true;
                MelonLogger.Msg("[gregCore][UI] EventSystem vorhanden (Spiel).");
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
            MelonLogger.Warning($"[gregCore][UI] EventSystem-Setup fehlgeschlagen: {ex.GetBaseException().Message}");
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
        MelonLogger.Msg("[gregCore][UI] EventSystem fuer Mod-UIs erzeugt.");
    }

    private static InputActionReference MakeAction(string name, InputActionType type, string controlType, params string[] bindings)
    {
        var action = new InputAction(name, type, controlType);
        foreach (string b in bindings)
        {
            try { action.AddBinding(b); } catch { }
        }
        try { action.Enable(); } catch { }
        var reference = ScriptableObject.CreateInstance<InputActionReference>();
        try { reference.Set(action); } catch { }
        return reference;
    }
}
