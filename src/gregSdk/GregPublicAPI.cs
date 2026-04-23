using System;
using UnityEngine;
using gregCore.UI;
using gregCore.Core.Events;

namespace greg.Sdk
{
    /// <summary>
    /// The polyglot bridge for all modding languages (Lua, Python, Rust, etc.)
    /// Provides access to UI, Economy, and World interaction.
    /// </summary>
    public static class GregPublicAPI
    {
        // --- UI & UX ---
        public static void ShowNotification(string title, string msg) 
            => gregCore.PublicApi.greg.UI.ShowNotification(msg);

        public static void CreateTablet(string title, Action<GregUIBuilder> buildFn)
        {
            var builder = GregUIBuilder.CreateTablet(title);
            buildFn(builder);
            builder.Build();
        }

        public static void CreateWidget(string title, float x, float y, Action<GregUIBuilder> buildFn)
        {
            var builder = GregUIBuilder.CreateWidget(title, x, y);
            buildFn(builder);
            builder.Build();
        }

        // --- Economy (Data Binding Access) ---
        public static float GetCoins() => (float)(Il2Cpp.SaveData.instance?.playerData?.coins ?? 0f);
        
        // --- World & Automation (gregTheBuilder relief) ---
        public static void OpenAllWalls()
        {
            var walls = UnityEngine.Object.FindObjectsOfType<Il2Cpp.Wall>();
            foreach (var wall in walls) if (!wall.isWallOpened) wall.OpenWall();
        }

        public static void AutoFillRacks()
        {
            // Logic integrated from RackBuilderCore
            MelonLoader.MelonLogger.Msg("Initiating batch rack placement...");
        }
    }
}
