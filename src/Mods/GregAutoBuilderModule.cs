using System;
using System.Collections.Generic;
using UnityEngine;
using gregCore.UI;
using Il2Cpp;

namespace greg.Mods.AutoBuilder
{
    public class GregAutoBuilderModule : MonoBehaviour
    {
        private static GregAutoBuilderModule? _instance;
        public static void Initialize()
        {
            var go = new GameObject("greg_AutoBuilder");
            _instance = go.AddComponent<GregAutoBuilderModule>();
            UnityEngine.Object.DontDestroyOnLoad(go);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.F4)) // Dedicated AutoBuilder Key
            {
                ToggleUI();
            }
        }

        public void ToggleUI()
        {
            GregUIManager.TogglePanel("AutoBuilder");
            if (GregUIManager.Root?.Q<UnityEngine.UIElements.VisualElement>("Panel_AutoBuilder") == null)
            {
                BuildUI();
            }
        }

        private void BuildUI()
        {
            var builder = GregUIBuilder.CreateTablet("AutoRack Builder v3.5")
                .AddHeadline("Fleet Automation")
                .AddLabel("Automated deployment and maintenance of server infrastructure.")
                
                .AddHeadline("Wall Management")
                .AddPrimaryButton("OPEN ALL WALLS", () => {
                    OpenAllWalls();
                })
                
                .AddHeadline("Batch Deployment")
                .AddLabel("Automatically place racks on detected grid points.")
                .AddPrimaryButton("AUTO-FILL CURRENT ROOM", () => {
                    // Logic for auto-filling based on grid
                })
                
                .AddHeadline("Maintenance Relief")
                .AddToggle("Auto-Repair broken servers", true, (v) => { })
                .AddToggle("Optimize cooling distribution", false, (v) => { })
                
                .AddSecondaryButton("CLOSE", () => GregUIManager.SetPanelActive("AutoBuilder", false));

            builder.Build();
        }

        private void OpenAllWalls()
        {
            var walls = UnityEngine.Object.FindObjectsOfType<Il2Cpp.Wall>();
            foreach (var wall in walls)
            {
                if (wall != null && !wall.isWallOpened) wall.OpenWall();
            }
        }
    }
}
