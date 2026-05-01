using System;
using System.Linq;
using Il2CppInterop.Runtime;
using MelonLoader;
using UnityEngine;
using gregCore.UI;

namespace greg.Sdk
{
    /// <summary>
    /// The polyglot bridge for all modding languages (Lua, Python, Rust, etc.)
    /// Provides access to UI, Economy, and World interaction.
    /// All IL2CPP interactions are wrapped with safe casting and null checks.
    /// </summary>
    public static class GregPublicAPI
    {
        // --- UI & UX ---
        public static void ShowNotification(string title, string msg)
        {
            try
            {
                gregCore.PublicApi.greg.UI.ShowNotification(msg);
            }
            catch (Exception ex)
            {
                MelonLogger.Error($"[gregCore][PublicAPI] ShowNotification failed: {ex.Message}");
            }
        }

        public static void CreateTablet(string title, Action<GregUIBuilder> buildFn)
        {
            try
            {
                var builder = GregUIBuilder.CreateTablet(title);
                buildFn(builder);
                builder.Build();
            }
            catch (Exception ex)
            {
                MelonLogger.Error($"[gregCore][PublicAPI] CreateTablet failed: {ex.Message}");
            }
        }

        public static void CreateWidget(string title, float x, float y, Action<GregUIBuilder> buildFn)
        {
            try
            {
                var builder = GregUIBuilder.CreateWidget(title, x, y);
                buildFn(builder);
                builder.Build();
            }
            catch (Exception ex)
            {
                MelonLogger.Error($"[gregCore][PublicAPI] CreateWidget failed: {ex.Message}");
            }
        }

        // --- Economy (Data Binding Access) ---
        public static float GetCoins()
        {
            try
            {
                var saveData = Il2Cpp.SaveData.instance;
                if (saveData == null || saveData.Pointer == IntPtr.Zero)
                {
                    MelonLogger.Warning("[gregCore][PublicAPI] SaveData.instance is null or collected.");
                    return 0f;
                }

                var playerData = saveData.playerData;
                if (playerData == null || playerData.Pointer == IntPtr.Zero)
                {
                    return 0f;
                }

                return playerData.coins;
            }
            catch (Exception ex)
            {
                MelonLogger.Error($"[gregCore][PublicAPI] GetCoins failed: {ex.Message}");
                return 0f;
            }
        }

        // --- World & Automation (gregTheBuilder relief) ---
        public static void OpenAllWalls()
        {
            try
            {
                var walls = UnityEngine.Object.FindObjectsOfType<Il2Cpp.Wall>();
                if (walls == null) return;

                foreach (var wall in walls)
                {
                    if (wall == null || wall.Pointer == IntPtr.Zero)
                        continue;

                    if (!wall.isWallOpened)
                    {
                        try
                        {
                            wall.OpenWall();
                        }
                        catch (Exception innerEx)
                        {
                            MelonLogger.Warning($"[gregCore][PublicAPI] OpenWall failed for one wall: {innerEx.Message}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MelonLogger.Error($"[gregCore][PublicAPI] OpenAllWalls failed: {ex.Message}");
            }
        }

        public static void AutoFillRacks()
        {
            try
            {
                MelonLogger.Msg("[gregCore][PublicAPI] Initiating batch rack placement...");
                // TODO: Integrate with RackBuilderCore via safe IL2CPP calls
            }
            catch (Exception ex)
            {
                MelonLogger.Error($"[gregCore][PublicAPI] AutoFillRacks failed: {ex.Message}");
            }
        }

        /// <summary>
        /// Safe wrapper to find objects of an IL2CPP type. Returns empty array on failure.
        /// </summary>
        public static T[] FindObjectsOfTypeSafe<T>() where T : UnityEngine.Object
        {
            try
            {
                var results = UnityEngine.Object.FindObjectsOfType<T>();
                return results?.Where(r => r != null && r.Pointer != IntPtr.Zero).ToArray() ?? Array.Empty<T>();
            }
            catch (Exception ex)
            {
                MelonLogger.Error($"[gregCore][PublicAPI] FindObjectsOfTypeSafe<{typeof(T).Name}> failed: {ex.Message}");
                return Array.Empty<T>();
            }
        }

        /// <summary>
        /// Safe wrapper to get SaveData.instance with null and collected-object checks.
        /// </summary>
        public static Il2Cpp.SaveData? GetSaveDataSafe()
        {
            try
            {
                var saveData = Il2Cpp.SaveData.instance;
                if (saveData == null || saveData.Pointer == IntPtr.Zero)
                    return null;
                return saveData;
            }
            catch (Exception ex)
            {
                MelonLogger.Error($"[gregCore][PublicAPI] GetSaveDataSafe failed: {ex.Message}");
                return null;
            }
        }
    }
}
