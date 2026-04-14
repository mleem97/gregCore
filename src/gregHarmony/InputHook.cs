using System;
using HarmonyLib;
using Il2Cpp;
using MelonLoader;
using UnityEngine;
using UnityEngine.InputSystem;
using greg.Core.UI;

namespace greg.Harmony;

public class InputHook : MelonMod
{
    public override void OnUpdate()
    {
        if (Keyboard.current == null) return;

        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            HandleEscapeKey();
        }

        if (Keyboard.current.insertKey.wasPressedThisFrame)
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
}
