using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using greg.Sdk.Services;

namespace gregCoreSDK.Core.UI;

public static class gregModConfigManager
{
    private static GameObject _root;
    private static Canvas _canvas;
    private static RectTransform _contentRoot;
    private static bool _initialized;

    public static bool IsOpen => _root != null && _root.activeSelf;

    public static void Toggle(bool open)
    {
        if (!_initialized) Initialize();
        if (_root == null) return;

        _root.SetActive(open);
        GregUiFocusManager.RequestFocus(open);
        
        if (!open)
        {
            GregConfigService.Save();
        }
    }

    private static void Initialize()
    {
        try
        {
            _canvas = GregUiService.CreateCanvas("greg_ModConfigCanvas", 1000100);
            
            // Main Window
            _root = GregUiService.CreateModernPanel(_canvas.transform, "MainWindow", new Vector2(900, 700));
            
            // 1. Tab Bar
            var tabBar = GregUiService.CreateModernPanel(_root.transform, "TabBar", new Vector2(900, 60));
            var tabBarRT = tabBar.GetComponent<RectTransform>();
            tabBarRT.anchoredPosition = new Vector2(0, 320);
            tabBar.GetComponent<Image>().color = GregUiService.LuminescentArchitect.SurfaceContainer;
            GregUiService.AddHorizontalLayout(tabBar, 20);

            GregUiService.CreateModernButton(tabBar.transform, "TabMods", "MODS", () => ShowTab("MODS"));
            GregUiService.CreateModernButton(tabBar.transform, "TabMaintenance", "MAINTENANCE", () => ShowTab("MAINTENANCE"));
            GregUiService.CreateModernButton(tabBar.transform, "TabUI", "UI TAKEOVER", () => ShowTab("UI"));

            // 2. Scroll Area
            var scrollGo = new GameObject("ScrollArea");
            scrollGo.transform.SetParent(_root.transform, false);
            var scrollRT = scrollGo.AddComponent<RectTransform>();
            scrollRT.sizeDelta = new Vector2(850, 550);
            scrollRT.anchoredPosition = new Vector2(0, -20);

            scrollGo.AddComponent<RectMask2D>();
            var scrollRect = scrollGo.AddComponent<ScrollRect>();

            var contentGo = new GameObject("Content");
            contentGo.transform.SetParent(scrollGo.transform, false);
            _contentRoot = contentGo.AddComponent<RectTransform>();
            _contentRoot.anchorMin = new Vector2(0, 1);
            _contentRoot.anchorMax = new Vector2(1, 1);
            _contentRoot.pivot = new Vector2(0.5f, 1);
            _contentRoot.anchoredPosition = Vector2.zero;
            
            GregUiService.AddVerticalLayout(contentGo, 15, 30);
            scrollRect.content = _contentRoot;

            ShowTab("MODS");

            // Close Button
            var closeBtn = GregUiService.CreateModernButton(_root.transform, "CloseBtn", "CLOSE", () => Toggle(false));
            closeBtn.GetComponent<RectTransform>().anchoredPosition = new Vector2(350, -310);

            _root.SetActive(false);
            _initialized = true;
            MelonLoader.MelonLogger.Msg("[gregCore] Native Layout Engine Initialized.");
        }
        catch (Exception ex)
        {
            greg.Core.CrashLog.LogException("gregModConfigManager.Initialize", ex);
        }
    }

    private static void ShowTab(string tabId)
    {
        if (_contentRoot == null) return;
        
        foreach (Transform child in _contentRoot)
        {
            UnityEngine.Object.Destroy(child.gameObject);
        }

        if (tabId == "MODS")
        {
            AddSettingHeader("Core Framework");
            AddSettingToggle("gregCore", "Enable Verbose Logging", false);
            AddSettingHeader("HexViewer");
            AddSettingToggle("HexViewer", "Enabled", true);
        }
        else if (tabId == "UI")
        {
            AddSettingHeader("UI Architecture Takeover");
            GregUiService.CreateModernButton(_contentRoot, "HideHUD", "HIDE MAIN HUD", () => {
                GregUiService.TakeoverVanillaUi(GameObject.Find("MainHUD"));
            });
        }
        else if (tabId == "MAINTENANCE")
        {
            AddSettingHeader("Network Maintenance");
            var switches = GregNetworkMaintenanceService.GetAllInstalledSwitches();
            
            int brokenCount = 0;
            foreach (var s in switches) if (s.IsBroken || !s.HasPhysicalFlow) brokenCount++;

            GregUiService.CreateModernButton(_contentRoot, "ResetAll", $"RESET ALL BROKEN ({brokenCount})", () => {
                GregNetworkMaintenanceService.ResetAllBrokenSwitches();
                ShowTab("MAINTENANCE");
            });

            GregUiService.AddSeparator(_contentRoot);

            foreach (var sw in switches)
            {
                string statusColor = sw.IsBroken ? "red" : (sw.HasPhysicalFlow ? "#61F4D8" : "orange");
                string statusText = sw.IsBroken ? "BROKEN" : (sw.HasPhysicalFlow ? "FLOWING" : "NO FLOW");
                
                var row = GregUiService.CreateModernPanel(_contentRoot, $"Switch_{sw.Id}", new Vector2(800, 40));
                row.GetComponent<Image>().color = new Color(0, 0, 0, 0.2f);
                GregUiService.AddHorizontalLayout(row, 10);

                GregUiService.CreateLabel(row.transform, "Name", sw.Label, 14);
                GregUiService.CreateLabel(row.transform, "Status", $"<color={statusColor}>{statusText}</color>", 12);
                
                GregUiService.CreateModernButton(row.transform, "Reset", "FACTORY RESET", () => {
                    GregNetworkMaintenanceService.FactoryReset(sw.NativeInstance);
                    ShowTab("MAINTENANCE");
                });
            }
        }
    }

    private static void AddSettingHeader(string text)
    {
        var label = GregUiService.CreateLabel(_contentRoot, "Header", $"<color=#61F4D8>{text.ToUpper()}</color>", 16);
        label.fontStyle = FontStyles.Bold;
    }

    private static void AddSettingToggle(string modId, string key, bool def)
    {
        bool current = GregConfigService.Get(modId, key, def);
        GregUiService.CreateModernButton(_contentRoot, "Toggle", $"{key}: {(current ? "ON" : "OFF")}", () => {
            bool newVal = !GregConfigService.Get(modId, key, def);
            GregConfigService.Set(modId, key, newVal);
            ShowTab("MODS");
        });
    }

    public static void DrawConfigUI() { }
}
