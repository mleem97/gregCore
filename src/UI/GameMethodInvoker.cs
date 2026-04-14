using System;
using System.Reflection;
using UnityEngine;
using MelonLoader;

namespace greg.Core.UI;

public static class GameMethodInvoker
{
    private static readonly BindingFlags DefaultFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static;

    public static bool Invoke(string typeName, string methodName, params object[] args)
    {
        try
        {
            var assembly = Assembly.Load("Assembly-CSharp");
            var type = assembly.GetType(typeName);
            if (type == null)
            {
                MelonLogger.Warning($"[GameMethodInvoker] Type not found: {typeName}");
                return false;
            }

            return Invoke(type, methodName, null, args);
        }
        catch (Exception ex)
        {
            MelonLogger.Error($"[GameMethodInvoker] Failed: {ex.Message}");
            return false;
        }
    }

    public static bool Invoke<T>(string methodName, T instance, params object[] args)
    {
        return Invoke(typeof(T), methodName, instance, args);
    }

    public static bool Invoke(Type type, string methodName, object instance, params object[] args)
    {
        try
        {
            var method = FindMethod(type, methodName, args?.Length ?? 0);
            if (method == null)
            {
                MelonLogger.Warning($"[GameMethodInvoker] Method not found: {type.Name}.{methodName}");
                return false;
            }

            var result = method.Invoke(instance, args);
            
            if (result is bool boolResult && !boolResult)
            {
                MelonLogger.Warning($"[GameMethodInvoker] Method returned false: {type.Name}.{methodName}");
                return false;
            }

            MelonLogger.Msg($"[GameMethodInvoker] Invoked: {type.Name}.{methodName}");
            return true;
        }
        catch (Exception ex)
        {
            MelonLogger.Error($"[GameMethodInvoker] {type.Name}.{methodName}: {ex.Message}");
            return false;
        }
    }

    public static T GetStaticField<T>(string typeName, string fieldName)
    {
        try
        {
            var assembly = Assembly.Load("Assembly-CSharp");
            var type = assembly.GetType(typeName);
            if (type == null) return default;

            var field = type.GetField(fieldName, DefaultFlags);
            if (field == null) return default;

            var value = field.GetValue(null);
            return value != null ? (T)value : default;
        }
        catch
        {
            return default;
        }
    }

    public static T GetInstanceField<T>(object instance, string fieldName)
    {
        try
        {
            if (instance == null) return default;

            var type = instance.GetType();
            var field = type.GetField(fieldName, DefaultFlags);
            if (field == null) return default;

            var value = field.GetValue(instance);
            return value != null ? (T)value : default;
        }
        catch
        {
            return default;
        }
    }

    public static void SetInstanceField(object instance, string fieldName, object value)
    {
        try
        {
            if (instance == null) return;

            var type = instance.GetType();
            var field = type.GetField(fieldName, DefaultFlags);
            field?.SetValue(instance, value);
        }
        catch (Exception ex)
        {
            MelonLogger.Warning($"[GameMethodInvoker] SetField failed: {ex.Message}");
        }
    }

    private static MethodInfo FindMethod(Type type, string methodName, int argCount)
    {
        var methods = type.GetMethods(DefaultFlags);
        
        foreach (var method in methods)
        {
            if (method.Name != methodName) continue;
            
            var parameters = method.GetParameters();
            if (parameters.Length != argCount) continue;
            
            return method;
        }

        foreach (var method in methods)
        {
            if (method.Name != methodName) continue;
            if (method.GetParameters().Length == 0 || argCount == 0) return method;
        }

        return null;
    }
}

public static class GameUIElements
{
    public static class Paths
    {
        public const string MainGameManager = "MainGameManager";
        public const string PauseMenuCanvas = "PauseMenuCanvas";
        public const string PauseMenu = "PauseMenu";
        public const string MainMenu = "MainMenu";
        public const string ComputerShop = "ComputerShop";
        public const string GameManager = "GameManager";
    }

    public static class Methods
    {
        public const string LoadGame = "LoadGame";
        public const string SaveGame = "SaveGame";
        public const string Resume = "Resume";
        public const string Pause = "Pause";
        public const string OpenShop = "OpenShop";
        public const string CloseShop = "CloseShop";
        public const string ReturnToMenu = "ReturnToMenu";
    }

    public static class Types
    {
        public const string MainGameManager = "MainGameManager";
        public const string GameManager = "GameManager";
        public const string PauseMenu = "PauseMenu";
        public const string MainMenu = "MainMenu";
        public const string ComputerShop = "ComputerShop";
    }
}

public static class GameUIButtons
{
    public static bool ClickButton(string buttonPath)
    {
        var button = GameObject.Find(buttonPath);
        if (button == null)
        {
            MelonLogger.Warning($"[GameUIButtons] Button not found: {buttonPath}");
            return false;
        }

        var btn = button.GetComponent<UnityEngine.UI.Button>();
        if (btn == null)
        {
            MelonLogger.Warning($"[GameUIButtons] No Button component: {buttonPath}");
            return false;
        }

        btn.onClick?.Invoke();
        MelonLogger.Msg($"[GameUIButtons] Clicked: {buttonPath}");
        return true;
    }

    public static bool ClickButtonInCanvas(string canvasName, string buttonName)
    {
        return ClickButton($"{canvasName}/{buttonName}");
    }

    public static bool ClickLoadButton()
    {
        return ClickButton("PauseMenuCanvas/Pause menu -  Settings Scripts/PanelArea/PanelBackground/SystemPanel/Game/LoadButton");
    }

    public static bool ClickSaveButton()
    {
        return ClickButton("PauseMenuCanvas/Pause menu -  Settings Scripts/PanelArea/PanelBackground/SystemPanel/Game/SaveButton");
    }

    public static bool ClickResumeButton()
    {
        return ClickButton("PauseMenuCanvas/Pause menu -  Settings Scripts/PanelArea/PanelBackground/SystemPanel/Game/ResumeButton");
    }

    public static bool ClickMainMenuButton()
    {
        return ClickButton("PauseMenuCanvas/Pause menu -  Settings Scripts/PanelArea/PanelBackground/SystemPanel/Game/MainMenuButton");
    }
}
