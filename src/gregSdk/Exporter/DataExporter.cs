using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MelonLoader;
using UnityEngine;
using UnityEngine.SceneManagement;
using greg.Core;
using greg.Exporter;

namespace greg.Core.Exporter
{
    public static class DataExporter
    {
        private static string ExportRoot => Path.Combine(MelonLoader.Utils.MelonEnvironment.UserDataDirectory, "gregExports");

        public static void RunFullExport()
        {
            try
            {
                if (!Directory.Exists(ExportRoot)) Directory.CreateDirectory(ExportRoot);

                MelonLogger.Msg("[gregCore] Starting Full Data Export...");

                ExportHooksAndEvents();
                ExportUiPaths();
                ExportModifiableObjects();

                MelonLogger.Msg($"[gregCore] Full Data Export completed to: {ExportRoot}");
            }
            catch (Exception ex)
            {
                MelonLogger.Error($"[gregCore] Full Data Export failed: {ex.Message}");
            }
        }

        private static void ExportHooksAndEvents()
        {
            var lines = new List<string> { "# gregCore Hooks and Events Catalog", $"Exported: {DateTime.Now}", "" };
            
            lines.Add("## Events");
            try {
                var eventFields = typeof(greg.Core.EventIds).GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
                foreach (var field in eventFields)
                {
                    if (field.IsLiteral && field.FieldType == typeof(uint))
                    {
                        lines.Add($"- {field.Name} (ID: 0x{((uint)field.GetValue(null)):X8})");
                    }
                }
            } catch (Exception e) { lines.Add($"Error exporting events: {e.Message}"); }

            lines.Add("");
            lines.Add("## Hooks (Available Candidates)");
            try {
                // Using the Il2CppEventCatalogService for logic
                var eventCatalog = new Il2CppEventCatalogService();
                string catalogFile = eventCatalog.ExportCatalog(ExportRoot);
                lines.Add($"- Detailed IL2CPP Catalog exported to: {Path.GetFileName(catalogFile)}");
            } catch (Exception e) { lines.Add($"Error exporting hooks: {e.Message}"); }

            File.WriteAllLines(Path.Combine(ExportRoot, "hooks_events.txt"), lines);
        }

        private static void ExportUiPaths()
        {
            var lines = new List<string> { "# gregCore UI Paths and Prefabs", $"Exported: {DateTime.Now}", "" };
            
            try {
                var canvases = GameObject.FindObjectsOfType<Canvas>(true);
                foreach (var canvas in canvases)
                {
                    lines.Add($"## Canvas: {canvas.name}");
                    TraverseHierarchy(canvas.transform, "", lines);
                    lines.Add("");
                }
            } catch (Exception e) { lines.Add($"Error exporting UI paths: {e.Message}"); }

            File.WriteAllLines(Path.Combine(ExportRoot, "ui_paths.txt"), lines);
        }

        private static void ExportModifiableObjects()
        {
            var lines = new List<string> { "# gregCore Modifiable Objects (Devices, Items, Items)", $"Exported: {DateTime.Now}", "" };
            
            try {
                var keywords = new[] { "Device", "Item", "Switch", "Server", "Rack", "Network", "Cable" };
                var allObjects = GameObject.FindObjectsOfType<GameObject>(true);
                
                var sortedObjects = allObjects
                    .Where(go => keywords.Any(k => go.name.Contains(k, StringComparison.OrdinalIgnoreCase)))
                    .GroupBy(go => go.name)
                    .Select(g => g.First())
                    .OrderBy(go => go.name);

                foreach (var go in sortedObjects)
                {
                    lines.Add($"- Name: {go.name} | Tags: {go.tag} | Layer: {LayerMask.LayerToName(go.layer)}");
                    var components = go.GetComponents<Component>();
                    foreach (var comp in components)
                    {
                        if (comp != null)
                            lines.Add($"  * Component: {comp.GetIl2CppType().FullName}");
                    }
                }
            } catch (Exception e) { lines.Add($"Error exporting objects: {e.Message}"); }

            File.WriteAllLines(Path.Combine(ExportRoot, "modifiable_objects.txt"), lines);
        }

        private static void TraverseHierarchy(Transform t, string indent, List<string> lines)
        {
            lines.Add($"{indent}- {t.name}");

            for (int i = 0; i < t.childCount; i++)
            {
                TraverseHierarchy(t.GetChild(i), indent + "  ", lines);
            }
        }
    }
}
