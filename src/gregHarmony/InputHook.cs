using System;
using HarmonyLib;
using Il2Cpp;
using MelonLoader;
using UnityEngine;
using greg.Core.UI;

namespace greg.Harmony;

public class InputHook : MelonMod
{
    private static InputHook _instance;
    private bool _wasPaused = false;

    public override void OnUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            HandleEscapeKey();
        }

        if (Input.GetKeyDown(KeyCode.Insert))
        {
            UIRouter.SetMode(UIMode.ModConfig);
            gregModConfigManager.Toggle(true);
        }
    }

    private void HandleEscapeKey()
    {
        var currentMode = UIRouter.CurrentMode;

        switch (currentMode)
        {
            case UIMode.Playing:
                UIRouter.SetMode(UIMode.Paused);
                break;

            case UIMode.Paused:
                UIRouter.SetMode(UIMode.Playing);
                break;

            case UIMode.ComputerShop:
            case UIMode.AssetManagement:
            case UIMode.BalanceSheet:
            case UIMode.Hire:
            case UIMode.Tutorial:
                UIRouter.SetMode(UIMode.Playing);
                break;

            case UIMode.Settings:
            case UIMode.ModConfig:
                UIRouter.GoBack();
                break;

            case UIMode.MainMenu:
                break;
        }
    }

    public override void OnGUI()
    {
        if (Event.current != null && Event.current.type == EventType.KeyDown)
        {
            if (Event.current.keyCode == KeyCode.Escape)
            {
                Event.current.Use();
            }
        }
    }
}
