using gregCore.Infrastructure.Settings.Services;
using gregCore.Core.Abstractions;
using gregCore.GameLayer.Bootstrap;
using HarmonyLib;
using UnityEngine;

namespace gregCore.GameLayer.Patches.UI;

[HarmonyPatch]
internal static class SettingsUiBridgePatch
{
    private static bool _tabInjected = false;
    private static GregSettingsUiBridge _uiBridge = null!;

    private static GregSettingsUiBridge? GetBridge()
    {
        if (_uiBridge == null)
        {
            _uiBridge = GregServiceContainer.Get<GregSettingsUiBridge>()!;
        }
        return _uiBridge;
    }

    // We hook into PauseMenu_TabGroup.Start or PauseMenu.Awake. Let's patch PauseMenu.Awake for setup.
    [HarmonyPatch(typeof(global::Il2Cpp.PauseMenu), nameof(global::Il2Cpp.PauseMenu.Awake))]
    [HarmonyPostfix]
    internal static void OnPauseMenuAwake(global::Il2Cpp.PauseMenu __instance)
    {
        try
        {
            if (_tabInjected) return; // Prevent multiple injections on scene reloads if it persists

            // Find the tab group (we assume mainPauseMenuTabGroup or systemPauseMenuTabGroup is setup)
            var tabGroup = __instance.systemPauseMenuTabGroup; // usually contains Gameplay, Graphics, Volume, Controls

            if (tabGroup != null)
            {
                // We need to clone an existing tab button
                if (tabGroup.tabButtons != null && tabGroup.tabButtons.Count > 0)
                {
                    var sourceTab = tabGroup.tabButtons[0];
                    var newTabObj = UnityEngine.Object.Instantiate(sourceTab.gameObject, sourceTab.transform.parent);
                    newTabObj.name = "ModSettingsTab";

                    // The tab button has a Text / TextMeshPro component
                    // var tmp = newTabObj.GetComponentInChildren<global::Il2CppTMPro.TextMeshProUGUI>();
                    // if (tmp != null)
                    // {
                    //     tmp.text = "Mods";
                    // }

                    var newTabButton = newTabObj.GetComponent<global::Il2Cpp.PauseMenu_TabButton>();

                    // We need a corresponding panel object to swap to
                    var sourcePanel = tabGroup.objectsToSwap[0];
                    var newPanelObj = new GameObject("ModSettingsPanel");
                    newPanelObj.transform.SetParent(sourcePanel.transform.parent, false);

                    // Setup UI (we will build this via code or a separate prefab)
                    GetBridge()?.BuildModSettingsPanel(newPanelObj);

                    // Add to lists
                    tabGroup.tabButtons.Add(newTabButton);
                    tabGroup.objectsToSwap.Add(newPanelObj);

                    greg.Logging.GregLogger.Msg("Injected 'Mods' Tab into Settings Menu", "UIBridge");
                    _tabInjected = true;
                }
            }
        }
        catch (Exception ex)
        {
            greg.Logging.GregLogger.Error("Failed to inject Mod Settings UI", ex, "UIBridge");
        }
    }

    [HarmonyPatch(typeof(global::Il2Cpp.PauseMenu), nameof(global::Il2Cpp.PauseMenu.OnEnable))]
    [HarmonyPostfix]
    internal static void OnPauseMenuOpened()
    {
        try
        {
            // Publish Event
            API.GregAPI.FireEvent("0"); // We will define a real event ID later, or string based
            // Reload and check conflicts
            gregCore.PublicApi.greg._context?.EventBus.Publish("greg.SYSTEM.SettingsOpened", new Core.Models.EventPayload());

            // Trigger Refresh
            GetBridge()?.RefreshUi();
        }
        catch (Exception ex)
        {
            greg.Logging.GregLogger.Error("Error on open", ex, "UIBridge");
        }
    }

    [HarmonyPatch(typeof(global::Il2Cpp.PauseMenu), nameof(global::Il2Cpp.PauseMenu.OnDisable))]
    [HarmonyPostfix]
    internal static void OnPauseMenuClosed()
    {
        try
        {
            // Save state
            gregCore.PublicApi.greg._context?.EventBus.Publish("greg.SYSTEM.SettingsClosed", new Core.Models.EventPayload());
        }
        catch (Exception ex)
        {
            greg.Logging.GregLogger.Error("Error on close", ex, "UIBridge");
        }
    }
}

