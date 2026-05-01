using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using gregCore.UI;

namespace greg.Mods.AutoBuilder
{
    public class GregAutoBuilderModule : MonoBehaviour
    {
        public GregAutoBuilderModule(IntPtr ptr) : base(ptr) { }

        private static GregAutoBuilderModule? _instance;
        private bool _isVisible;
        private GregUIBuilder? _uiBuilder;

        public static void Initialize()
        {
            Il2CppInterop.Runtime.Injection.ClassInjector.RegisterTypeInIl2Cpp<GregAutoBuilderModule>();
            var go = new GameObject("greg_AutoBuilder");
            _instance = go.AddComponent<GregAutoBuilderModule>();
            UnityEngine.Object.DontDestroyOnLoad(go);
        }

        private void Update()
        {
            var kb = Keyboard.current;
            if (kb != null && kb.f4Key.wasPressedThisFrame)
            {
                _isVisible = !_isVisible;
                if (_isVisible) BuildUI();
            }
        }

        private void OnGUI()
        {
            if (!_isVisible) return;
            if (_uiBuilder == null) BuildUI();
            _uiBuilder?.Draw();
        }

        private void BuildUI()
        {
            _uiBuilder = GregUIBuilder.CreateTablet("AutoRack Builder v3.5")
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
                
                .AddSecondaryButton("CLOSE", () => _isVisible = false);

            _uiBuilder.Build();
        }

        private void OpenAllWalls()
        {
            try
            {
                // Note: Il2Cpp.Wall needs careful resolving if not available in current context
                var walls = UnityEngine.Object.FindObjectsOfType<Il2Cpp.Wall>();
                foreach (var wall in walls)
                {
                    if (wall != null && !wall.isWallOpened) wall.OpenWall();
                }
            }
            catch (Exception ex)
            {
                MelonLoader.MelonLogger.Warning($"[AutoBuilder] OpenAllWalls failed: {ex.Message}");
            }
        }
    }
}
