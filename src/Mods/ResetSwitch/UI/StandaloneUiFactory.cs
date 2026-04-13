using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

namespace greg.Mods.ResetSwitch.UI;

public static class StandaloneUiFactory
{
    public static Canvas CreateCanvas(string name, int sortingOrder)
    {
        var existing = GameObject.Find(name);
        if (existing != null)
        {
            var existingCanvas = existing.GetComponent<Canvas>();
            if (existingCanvas != null)
            {
                return existingCanvas;
            }
        }

        var canvasGo = new GameObject(name);
        var canvas = canvasGo.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = sortingOrder;

        canvasGo.AddComponent<CanvasScaler>();
        canvasGo.AddComponent<GraphicRaycaster>();

        EnsureEventSystem();
        return canvas;
    }

    private static void EnsureEventSystem()
    {
        var existing = Object.FindObjectOfType<EventSystem>();
        if (existing != null)
        {
            return;
        }

        var eventSystemGo = new GameObject("EventSystem");
        eventSystemGo.AddComponent<EventSystem>();
        eventSystemGo.AddComponent<InputSystemUIInputModule>();
    }
}
