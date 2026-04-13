using System;
using UnityEngine;
using UnityEngine.UI;
using greg.Sdk.Services;
using Il2Cpp;

namespace gregCoreSDK.Core.UI;

/// <summary>
/// Replaces the Main HUD with a Luminescent Architect styled version.
/// </summary>
public static class gregMainHudReplacement
{
    private static GameObject _root;
    private static Canvas _canvas;
    private static bool _isEnabled = false;
    private static bool _vanillaHidden = false;

    // UI Elements
    private static TextMeshProUGUI _moneyText;
    private static TextMeshProUGUI _timeText;
    private static TextMeshProUGUI _statsText;

    public static void SetEnabled(bool enabled)
    {
        if (_isEnabled == enabled) return;
        _isEnabled = enabled;

        if (_isEnabled)
        {
            if (_root == null) BuildHud();
            _root.SetActive(true);
            HideVanillaHud();
        }
        else
        {
            if (_root != null) _root.SetActive(false);
            // We don't restore vanilla HUD automatically to prevent breaking logic, 
            // a scene reload will restore it.
        }
    }

    private static void HideVanillaHud()
    {
        if (_vanillaHidden) return;
        var vanillaHud = GameObject.Find("MainHUD");
        if (vanillaHud != null)
        {
            GregUiService.TakeoverVanillaUi(vanillaHud);
            _vanillaHidden = true;
            MelonLoader.MelonLogger.Msg("[gregCore] Vanilla MainHUD taken over by Luminescent Replacement.");
        }
    }

    public static void OnUpdate()
    {
        if (!_isEnabled || _root == null || !MainGameManager.instance) return;

        UpdateHudData();
    }

    private static void UpdateHudData()
    {
        try
        {
            // Money
            float money = GregBalanceService.GetMoney();
            var snap = GregBalanceService.GetLatestSnapshot();
            _moneyText.text = $"<color=#61F4D8>BALANCE</color>\n${money:N2}\n<size=12><color=#C0FCF6>INC: +${snap.revenue:N2}</color></size>";

            // Time
            float rawHours = GregTimeService.GetCurrentTimeInHours();
            int h = (int)rawHours;
            int m = (int)((rawHours - h) * 60);
            int day = GregTimeService.GetCurrentDay();
            _timeText.text = $"<color=#61F4D8>DAY {day}</color>\n{h:00}:{m:00}";

            // Network Stats
            int servers = NetworkMap.instance?.servers?.Count ?? 0;
            int switches = NetworkMap.instance?.switches?.Count ?? 0;
            _statsText.text = $"<color=#61F4D8>NETWORK</color>\nSRV: {servers}\nSW: {switches}";
        }
        catch { }
    }

    private static void BuildHud()
    {
        try
        {
            _canvas = GregUiService.CreateCanvas("greg_LuminescentHUD", 50); // Below ModConfig
            _root = new GameObject("HUD_Root");
            _root.transform.SetParent(_canvas.transform, false);
            var rt = _root.AddComponent<RectTransform>();
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;

            // 1. Top Right - Money
            var moneyPanel = GregUiService.CreateModernPanel(_root.transform, "MoneyPanel", new Vector2(250, 80));
            var mrt = moneyPanel.GetComponent<RectTransform>();
            mrt.anchorMin = new Vector2(1, 1);
            mrt.anchorMax = new Vector2(1, 1);
            mrt.pivot = new Vector2(1, 1);
            mrt.anchoredPosition = new Vector2(-20, -20);
            _moneyText = GregUiService.CreateLabel(moneyPanel.transform, "Text", "$0.00", 20);
            _moneyText.alignment = TextAlignmentOptions.Right;
            _moneyText.rectTransform.sizeDelta = new Vector2(230, 80);
            _moneyText.rectTransform.anchoredPosition = new Vector2(-10, 0);

            // 2. Top Center - Time
            var timePanel = GregUiService.CreateModernPanel(_root.transform, "TimePanel", new Vector2(200, 60));
            var trt = timePanel.GetComponent<RectTransform>();
            trt.anchorMin = new Vector2(0.5f, 1);
            trt.anchorMax = new Vector2(0.5f, 1);
            trt.pivot = new Vector2(0.5f, 1);
            trt.anchoredPosition = new Vector2(0, -20);
            _timeText = GregUiService.CreateLabel(timePanel.transform, "Text", "00:00", 18);
            _timeText.alignment = TextAlignmentOptions.Center;
            _timeText.rectTransform.sizeDelta = new Vector2(200, 60);

            // 3. Top Left - Stats
            var statsPanel = GregUiService.CreateModernPanel(_root.transform, "StatsPanel", new Vector2(200, 80));
            var srt = statsPanel.GetComponent<RectTransform>();
            srt.anchorMin = new Vector2(0, 1);
            srt.anchorMax = new Vector2(0, 1);
            srt.pivot = new Vector2(0, 1);
            srt.anchoredPosition = new Vector2(20, -20);
            _statsText = GregUiService.CreateLabel(statsPanel.transform, "Text", "SRV: 0", 16);
            _statsText.alignment = TextAlignmentOptions.Left;
            _statsText.rectTransform.sizeDelta = new Vector2(180, 80);
            _statsText.rectTransform.anchoredPosition = new Vector2(10, 0);

        }
        catch (Exception ex)
        {
            greg.Core.CrashLog.LogException("gregMainHudReplacement.BuildHud", ex);
        }
    }
}
