using System;
using System.Collections;
using System.Collections.Generic;
using MelonLoader;
using UnityEngine;
using UnityEngine.UI;
using Il2CppTMPro;

namespace greg.Sdk.Services;

public enum ToastType { Success, Warning, Error, Info }

/// <summary>
/// In-game toast and banner notification service.
/// </summary>
public static class GregNotificationService
{
    private static Canvas _canvas;
    private static readonly Queue<(string msg, ToastType type, float dur)> _queue = new();
    private static bool _showing;

    private static readonly Dictionary<ToastType, Color> TypeColors = new()
    {
        { ToastType.Success, new Color(0.38f, 0.96f, 0.85f) },  // #61F4D8
        { ToastType.Warning, new Color(0.94f, 0.65f, 0f) },      // #F0A500
        { ToastType.Error,   new Color(0.93f, 0.26f, 0.27f) },    // #ED4245
        { ToastType.Info,    new Color(0.67f, 0.67f, 0.67f) }     // #AAAAAA
    };

    private static GameObject _bannerGO;

    public static void ShowToast(string msg, ToastType type, float duration = 3f)
    {
        _queue.Enqueue((msg, type, duration));
        if (!_showing) MelonCoroutines.Start(ProcessQueue());
    }

    public static void ShowBanner(string msg, Color color)
    {
        EnsureCanvas();
        if (_bannerGO != null) UnityEngine.Object.Destroy(_bannerGO);

        _bannerGO = new GameObject("GregBanner");
        _bannerGO.transform.SetParent(_canvas.transform, false);
        var rt = _bannerGO.AddComponent<RectTransform>();
        rt.anchorMin = new Vector2(0, 1);
        rt.anchorMax = new Vector2(1, 1);
        rt.pivot = new Vector2(0.5f, 1);
        rt.sizeDelta = new Vector2(0, 40);
        rt.anchoredPosition = Vector2.zero;

        var img = _bannerGO.AddComponent<Image>();
        img.color = new Color(color.r, color.g, color.b, 0.9f);

        var textGO = new GameObject("Text");
        textGO.transform.SetParent(_bannerGO.transform, false);
        var txt = textGO.AddComponent<TextMeshProUGUI>();
        txt.text = msg;
        txt.fontSize = 14;
        txt.alignment = TextAlignmentOptions.Center;
        txt.color = Color.white;
        var txtRt = textGO.GetComponent<RectTransform>();
        txtRt.anchorMin = Vector2.zero;
        txtRt.anchorMax = Vector2.one;
        txtRt.sizeDelta = Vector2.zero;
    }

    public static void HideBanner()
    {
        if (_bannerGO != null)
        {
            UnityEngine.Object.Destroy(_bannerGO);
            _bannerGO = null;
        }
    }

    private static IEnumerator ProcessQueue()
    {
        _showing = true;
        while (_queue.Count > 0)
        {
            var (msg, type, dur) = _queue.Dequeue();
            var toast = CreateToast(msg, type);
            yield return new WaitForSeconds(dur);
            if (toast != null) UnityEngine.Object.Destroy(toast);
        }
        _showing = false;
    }

    private static GameObject CreateToast(string msg, ToastType type)
    {
        EnsureCanvas();
        var color = TypeColors[type];

        var go = new GameObject("GregToast");
        go.transform.SetParent(_canvas.transform, false);
        var rt = go.AddComponent<RectTransform>();
        rt.anchorMin = new Vector2(0.5f, 0);
        rt.anchorMax = new Vector2(0.5f, 0);
        rt.pivot = new Vector2(0.5f, 0);
        rt.sizeDelta = new Vector2(500, 44);
        rt.anchoredPosition = new Vector2(0, 80);

        var img = go.AddComponent<Image>();
        img.color = new Color(0.02f, 0.08f, 0.08f, 0.95f);

        // Accent bar
        var accent = new GameObject("Accent");
        accent.transform.SetParent(go.transform, false);
        var accentRt = accent.AddComponent<RectTransform>();
        accentRt.anchorMin = new Vector2(0, 0);
        accentRt.anchorMax = new Vector2(0, 1);
        accentRt.pivot = new Vector2(0, 0.5f);
        accentRt.sizeDelta = new Vector2(4, 0);
        var accentImg = accent.AddComponent<Image>();
        accentImg.color = color;

        var textGO = new GameObject("Text");
        textGO.transform.SetParent(go.transform, false);
        var txt = textGO.AddComponent<TextMeshProUGUI>();
        txt.text = msg;
        txt.fontSize = 13;
        txt.alignment = TextAlignmentOptions.MidlineLeft;
        txt.color = color;
        var txtRt = textGO.GetComponent<RectTransform>();
        txtRt.anchorMin = Vector2.zero;
        txtRt.anchorMax = Vector2.one;
        txtRt.offsetMin = new Vector2(16, 0);
        txtRt.offsetMax = new Vector2(-8, 0);

        MelonLogger.Msg($"[Notification] Toast [{type}]: {msg}");
        return go;
    }

    private static void EnsureCanvas()
    {
        if (_canvas != null) return;
        var go = new GameObject("GregNotificationCanvas");
        UnityEngine.Object.DontDestroyOnLoad(go);
        _canvas = go.AddComponent<Canvas>();
        _canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        _canvas.sortingOrder = 1000500;
        go.AddComponent<CanvasScaler>();
        go.AddComponent<GraphicRaycaster>();
    }
}
