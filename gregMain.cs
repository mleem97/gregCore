using Il2CppInterop.Runtime.InteropTypes.Arrays;
using MelonLoader;
using MelonLoader.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using greg.Diagnostic;
using greg.Core;


// Namespace gregAssetExporter muss zu deiner AssemblyInfo passen
[assembly: MelonInfo(typeof(gregAssetExporter.gregMain), "gregCore Framework", greg.Core.gregReleaseVersion.Current, "MLeeM97 (teamGreg)")]
[assembly: MelonGame("Waseku", "Data Center")]

namespace gregAssetExporter
{
    public class gregMain : MelonMod
    {
        private string exportPath = string.Empty;
        private bool exportBetaNotUsed = true;
        private bool showDebugOverlay = true;

        private readonly greg.Exporter.Il2CppEventCatalogService eventCatalogService = new greg.Exporter.Il2CppEventCatalogService();
        private readonly greg.Exporter.Il2CppGameplayIndexService gameplayIndexService = new greg.Exporter.Il2CppGameplayIndexService();
        private readonly greg.Exporter.RuntimeHookService runtimeHookService = new greg.Exporter.RuntimeHookService();
        private readonly greg.Exporter.GameSignalSnapshotService gameSignalSnapshotService = new greg.Exporter.GameSignalSnapshotService();

#if DEBUG
        // private greg.Exporter.TestMods.FrameworkDependencyTestMod frameworkDependencyTestMod; // TODO: removed, class does not exist
        private Texture2D debugOverlayBackgroundTexture;
        private int debugHooksAvailable;
        private int debugHookEventsAvailable;
        private int debugNotYetImplemented;
        private bool debugOverlayStatsInitialized;
#endif

        public override void OnInitializeMelon()
        {
            // --- gregCore Diagnostic & Session Logging ---
            greg.Core.Diagnostic.GregSessionLogger.Initialize();
            greg.Core.Diagnostic.GregSessionLogger.Log("Initializing gregCore Framework...");

            // --- gregCore Framework Internal Initialization ---
            greg.Sdk.Services.GregSaveService.Init();
            greg.Sdk.Services.GregUiService.SetGlobalScale(0.85f); // Use user-preferred 0.85x by default
            greg.Sdk.Services.GregHudService.Initialize();
            greg.Sdk.Services.MCP.GregMCPServer.Start();
            
            // Apply Deep-Layer Hijacker Patches
            var harmony = new HarmonyLib.Harmony("greg.core.hijacker");
            harmony.PatchAll(typeof(greg.Sdk.Internal.GregUiHijacker).Assembly);

            // --- Legacy Exporter Initialization ---
            exportPath = Path.Combine(MelonEnvironment.ModsDirectory, "ExportedAssets");
            if (!Directory.Exists(exportPath)) Directory.CreateDirectory(exportPath);

            MelonLogger.Msg($"gregCore Framework v{greg.Core.gregReleaseVersion.Current} loaded (SDK-only build).");
            MelonLogger.Msg("Want to help building the future of Modding in DataCenter? Join our Discord: discord.gg/greg");
            MelonLogger.Msg($"gregCore provides {greg.Sdk.Services.GregModRegistry.GetLoadedMods().Count} registered mods.");
            greg.Exporter.ModFramework.Events.Publish(new greg.Exporter.ModInitializedEvent(DateTime.UtcNow, greg.Core.gregReleaseVersion.Current));
        }

        public override void OnSceneWasLoaded(int buildIndex, string sceneName)
        {
            MelonLogger.Msg($"[gregCore] Scene Loaded: {sceneName}. Triggering data export...");
            greg.Core.Exporter.DataExporter.RunFullExport();
        }

        public override void OnApplicationQuit()
        {
            greg.Sdk.Services.MCP.GregMCPServer.Stop();
        }

        public override void OnUpdate()
        {
            greg.Exporter.ModFramework.Events.Publish(new greg.Exporter.ModTickEvent(Time.deltaTime, Time.frameCount));

#if DEBUG
            if (Keyboard.current != null && Keyboard.current.f5Key.wasPressedThisFrame)
            {
                showDebugOverlay = !showDebugOverlay;
            }

            if (!showDebugOverlay) return;

            if (Keyboard.current != null && Keyboard.current.ctrlKey.isPressed && Keyboard.current.f8Key.wasPressedThisFrame)
            {
                ExportAllResources();
            }

            if (Keyboard.current != null && Keyboard.current.f6Key.wasPressedThisFrame)
            {
                RefreshDebugOverlayStats(forceHookScan: true);
            }

            if (Keyboard.current != null && Keyboard.current.f11Key.wasPressedThisFrame)
            {
                ExportIl2CppEventCatalog();
            }

            if (Keyboard.current != null && Keyboard.current.f12Key.wasPressedThisFrame)
            {
                InstallRuntimeHooks();
            }
#endif
        }

#if DEBUG
        public override void OnGUI()
        {
            if (!showDebugOverlay) return;

            EnsureDebugOverlayAssets();

            if (!debugOverlayStatsInitialized)
                RefreshDebugOverlayStats(forceHookScan: true);

            string overlayText =
                $"<b>gregCore v{greg.Core.gregReleaseVersion.Current}</b>\n" +
                $"Hooks: {debugHooksAvailable:D5} | Events: {debugHookEventsAvailable:D5} | Missing: {debugNotYetImplemented:D5}\n" +
                $"[F5] Hide | [F6] Health | [F8] Export | [F11] Catalog | [F12] Hooks";

            const float marginX = 10f;
            const float topY = 10f;
            const float height = 70f;
            float width = 450f;
            var boxRect = new Rect(marginX, topY, width, height);

            GUI.DrawTexture(boxRect, debugOverlayBackgroundTexture, ScaleMode.StretchToFill);

            // Use direct GUIStyle initialization
            GUIStyle style = new GUIStyle();
            if (GUI.skin != null && GUI.skin.label != null)
            {
                style.font = GUI.skin.label.font;
            }
            style.richText = true;
            style.fontSize = 14;
            // IL2CPP RectOffset might not have 4-arg ctor in some builds, setting manually
            style.padding = new RectOffset();
            style.padding.left = 10;
            style.padding.right = 10;
            style.padding.top = 5;
            style.padding.bottom = 5;

            GUI.Label(boxRect, overlayText, style);
        }

        private void EnsureDebugOverlayAssets()
        {
            if (debugOverlayBackgroundTexture == null)
            {
                debugOverlayBackgroundTexture = new Texture2D(1, 1, TextureFormat.RGBA32, false);
                debugOverlayBackgroundTexture.SetPixel(0, 0, new Color(0.02f, 0.04f, 0.08f, 0.85f));
                debugOverlayBackgroundTexture.Apply();
            }
        }

        private void RefreshDebugOverlayStats(bool forceHookScan)
        {
            try
            {
                if (forceHookScan || !debugOverlayStatsInitialized)
                {
                    var hookScanResult = runtimeHookService.ScanCandidates(100000);
                    debugHooksAvailable = hookScanResult.Candidates.Count;
                }

                int eventCount = 0;
                var eventFields = typeof(EventIds).GetFields(BindingFlags.Public | BindingFlags.Static);
                for (int index = 0; index < eventFields.Length; index++)
                {
                    FieldInfo field = eventFields[index];
                    if (field.IsLiteral && field.FieldType == typeof(uint))
                        eventCount++;
                }

                debugHookEventsAvailable = eventCount;
                debugNotYetImplemented = Math.Max(0, debugHooksAvailable - debugHookEventsAvailable);
                debugOverlayStatsInitialized = true;
            }
            catch (Exception ex)
            {
                MelonLogger.Warning($"Debug overlay stats update failed: {ex.Message}");
            }
        }
#endif


        private void ExportAllGameSignalsOnStartup()
        {
            try
            {
                string diagnosticsPath = Path.Combine(exportPath, "Diagnostics");
                string snapshotPath = gameSignalSnapshotService.ExportAll(diagnosticsPath, eventCatalogService, gameplayIndexService, runtimeHookService);
                MelonLogger.Msg($"Startup-Snapshot erstellt: {snapshotPath}");
            }
            catch (Exception ex)
            {
                MelonLogger.Error($"Startup-Snapshot fehlgeschlagen: {ex.Message}");
                greg.Exporter.ModFramework.Events.Publish(new greg.Exporter.ModErrorEvent(DateTime.UtcNow, "StartupSnapshot", ex.Message));
            }
        }

        private void ExportIl2CppEventCatalog()
        {
            try
            {
                string diagnosticsPath = Path.Combine(exportPath, "Diagnostics");
                string filePath = eventCatalogService.ExportCatalog(diagnosticsPath);
                int linesCount = File.ReadAllLines(filePath).Length;
                string gameplayIndex = gameplayIndexService.ExportGameplayIndex(diagnosticsPath);

                MelonLogger.Msg($"IL2CPP Event-Katalog exportiert: {filePath}");
                MelonLogger.Msg($"IL2CPP Gameplay-Index exportiert: {gameplayIndex}");
                greg.Exporter.ModFramework.Events.Publish(new greg.Exporter.Il2CppCatalogExportedEvent(DateTime.UtcNow, filePath, linesCount));
                greg.Exporter.ModFramework.Events.Publish(new greg.Exporter.Il2CppGameplayIndexExportedEvent(DateTime.UtcNow, gameplayIndex));
            }
            catch (Exception ex)
            {
                MelonLogger.Error($"Fehler beim Export des IL2CPP Event-Katalogs: {ex.Message}");
                greg.Exporter.ModFramework.Events.Publish(new greg.Exporter.ModErrorEvent(DateTime.UtcNow, "Il2CppCatalog", ex.Message));
            }
        }

        private void InstallRuntimeHooks()
        {
            InstallRuntimeHooks(250);
        }

        private void InstallRuntimeHooks(int maxHooks)
        {
            try
            {
                var result = runtimeHookService.ScanAndInstall(maxHooks);
                MelonLogger.Msg($"Hook-Scan abgeschlossen. Kandidaten={result.Scanned}, installiert={result.Installed}, fehlgeschlagen={result.Failed}");

                if (result.Errors.Count > 0)
                {
                    string diagnosticsPath = Path.Combine(exportPath, "Diagnostics");
                    Directory.CreateDirectory(diagnosticsPath);
                    string errorFile = Path.Combine(diagnosticsPath, "hook-install-errors.txt");
                    File.WriteAllLines(errorFile, result.Errors);
                    MelonLogger.Warning($"Hook-Fehlerliste geschrieben: {errorFile}");
                }
            }
            catch (Exception ex)
            {
                MelonLogger.Error($"Fehler beim Installieren der Runtime-Hooks: {ex.Message}");
                greg.Exporter.ModFramework.Events.Publish(new greg.Exporter.ModErrorEvent(DateTime.UtcNow, "RuntimeHooks", ex.Message));
            }
        }

        private void InstallRuntimeHooksFromCatalog(string catalogPath, int maxHooks)
        {
            try
            {
                var result = runtimeHookService.InstallFromCatalog(catalogPath, maxHooks);
                MelonLogger.Msg($"Hook-Catalog verarbeitet. Datei={catalogPath} Kandidaten={result.Scanned}, installiert={result.Installed}, fehlgeschlagen={result.Failed}");

                if (result.Errors.Count > 0)
                {
                    string diagnosticsPath = Path.Combine(exportPath, "Diagnostics");
                    Directory.CreateDirectory(diagnosticsPath);
                    string errorFile = Path.Combine(diagnosticsPath, "hook-install-errors.txt");
                    File.WriteAllLines(errorFile, result.Errors);
                    MelonLogger.Warning($"Hook-Fehlerliste geschrieben: {errorFile}");
                }
            }
            catch (Exception ex)
            {
                MelonLogger.Error($"Fehler beim Installieren der Catalog-Hooks: {ex.Message}");
                greg.Exporter.ModFramework.Events.Publish(new greg.Exporter.ModErrorEvent(DateTime.UtcNow, "CatalogHooks", ex.Message));
            }
        }

        private void RunAutoHookCommandIfRequested()
        {
            try
            {
                string[] args = Environment.GetCommandLineArgs();
                bool autoScan = HasArg(args, "--greg-hooks-auto");
                bool installAll = HasArg(args, "--greg-hooks-all");
                string catalogPath = GetArgValue(args, "--greg-hooks-catalog=");

                if (!autoScan && string.IsNullOrWhiteSpace(catalogPath))
                    return;

                int defaultMax = installAll ? int.MaxValue : 250;
                int maxHooks = GetIntArgValue(args, "--greg-hooks-max=", defaultMax);

                if (!string.IsNullOrWhiteSpace(catalogPath))
                {
                    MelonLogger.Msg($"AutoHook-Command erkannt (catalog). maxHooks={maxHooks}");
                    InstallRuntimeHooksFromCatalog(catalogPath, maxHooks);
                    return;
                }

                MelonLogger.Msg($"AutoHook-Command erkannt (scan). maxHooks={maxHooks}");
                InstallRuntimeHooks(maxHooks);
            }
            catch (Exception ex)
            {
                MelonLogger.Warning($"AutoHook-Command konnte nicht ausgeführt werden: {ex.Message}");
            }
        }

        private static bool HasArg(IEnumerable<string> args, string name)
        {
            foreach (string arg in args)
            {
                if (string.Equals(arg, name, StringComparison.OrdinalIgnoreCase))
                    return true;
            }

            return false;
        }

        private static string GetArgValue(IEnumerable<string> args, string prefix)
        {
            foreach (string arg in args)
            {
                if (!arg.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
                    continue;

                return arg.Substring(prefix.Length).Trim('"');
            }

            return string.Empty;
        }

        private static int GetIntArgValue(IEnumerable<string> args, string prefix, int fallback)
        {
            string raw = GetArgValue(args, prefix);
            if (string.IsNullOrWhiteSpace(raw))
                return fallback;

            return int.TryParse(raw, out int parsed) && parsed > 0 ? parsed : fallback;
        }

        private static void OnHookTriggered(greg.Exporter.HookTriggeredEvent evt)
        {
            if (evt.TriggerCount <= 3 || evt.TriggerCount % 100 == 0)
            {
                MelonLogger.Msg($"Hook Trigger: {evt.MethodName} (count={evt.TriggerCount})");
            }
        }

        private void ExportAllResources()
        {
            greg.Exporter.ModFramework.Events.Publish(new greg.Exporter.ExportStartedEvent(DateTime.UtcNow, exportPath));

            string currentGamePath = Path.Combine(exportPath, "CurrentGame");
            string modelsPath = Path.Combine(currentGamePath, "Models");
            string texturesPath = Path.Combine(currentGamePath, "Textures");
            string spritesPath = Path.Combine(currentGamePath, "Sprites");
            string materialsPath = Path.Combine(currentGamePath, "Materials");
            string scriptsPath = Path.Combine(currentGamePath, "Scripts");
            string settingsPath = Path.Combine(currentGamePath, "Settings");
            string notUsedPath = Path.Combine(currentGamePath, "NotUsed");
            string notUsedModelsPath = Path.Combine(notUsedPath, "Models");
            string notUsedTexturesPath = Path.Combine(notUsedPath, "Textures");

            Directory.CreateDirectory(currentGamePath);
            Directory.CreateDirectory(modelsPath);
            Directory.CreateDirectory(texturesPath);
            Directory.CreateDirectory(spritesPath);
            Directory.CreateDirectory(materialsPath);
            Directory.CreateDirectory(scriptsPath);
            Directory.CreateDirectory(settingsPath);
            if (exportBetaNotUsed)
            {
                Directory.CreateDirectory(notUsedPath);
                Directory.CreateDirectory(notUsedModelsPath);
                Directory.CreateDirectory(notUsedTexturesPath);
            }

            File.WriteAllText(
                Path.Combine(currentGamePath, "README_NOT_USED.txt"),
                "Dieser Ordner enthält verwendete Assets aus dem aktuellen Spielstand (aktiv + inaktiv).\n" +
                "Struktur: Models, Textures, Sprites, Materials, Scripts, Settings.\n" +
                "Optional werden nicht verwendete, aber geladene Assets nach 'NotUsed/Models' und 'NotUsed/Textures' exportiert."
            );

            MelonLogger.Msg("Starte Export: verwendete Assets (aktiv + inaktiv) aus allen geladenen Szenen...");

            HashSet<int> usedMeshIds = new HashSet<int>();
            HashSet<int> usedTextureIds = new HashSet<int>();
            HashSet<int> usedSpriteTextureIds = new HashSet<int>();
            HashSet<string> exportedCurrentGame = new HashSet<string>();
            HashSet<string> exportedScriptTypes = new HashSet<string>();
            HashSet<string> exportedMaterials = new HashSet<string>();
            List<string> settingLines = new List<string>();
            List<string> materialInfoLines = new List<string>();

            foreach (GameObject obj in EnumerateAllSceneObjects(includeInactive: true))
            {
                try
                {
                    settingLines.Add($"{GetGameObjectPath(obj)} | activeSelf={obj.activeSelf} | activeInHierarchy={obj.activeInHierarchy} | layer={obj.layer} | tag={obj.tag} | scene={obj.scene.name}");

                    Component[] components = GetComponentsSafe(obj);
                    for (int componentIndex = 0; componentIndex < components.Length; componentIndex++)
                    {
                        Component component = components[componentIndex];
                        if (component == null) continue;
                        string typeName = component.GetType().FullName;
                        if (!string.IsNullOrWhiteSpace(typeName))
                            exportedScriptTypes.Add(typeName);
                    }

                    MeshFilter meshFilter = obj.GetComponent<MeshFilter>();
                    if (meshFilter != null && meshFilter.sharedMesh != null)
                    {
                        usedMeshIds.Add(meshFilter.sharedMesh.GetInstanceID());
                        if (TryRegister(exportedCurrentGame, $"mesh:{meshFilter.sharedMesh.name}"))
                            SaveMesh(meshFilter.sharedMesh, modelsPath);
                    }

                    SkinnedMeshRenderer skinnedMeshRenderer = obj.GetComponent<SkinnedMeshRenderer>();
                    if (skinnedMeshRenderer != null && skinnedMeshRenderer.sharedMesh != null)
                    {
                        usedMeshIds.Add(skinnedMeshRenderer.sharedMesh.GetInstanceID());
                        if (TryRegister(exportedCurrentGame, $"mesh:{skinnedMeshRenderer.sharedMesh.name}"))
                            SaveMesh(skinnedMeshRenderer.sharedMesh, modelsPath);
                    }

                    Renderer renderer = obj.GetComponent<Renderer>();
                    if (renderer != null)
                    {
                        Material[] materials = renderer.sharedMaterials;
                        for (int materialIndex = 0; materialIndex < materials.Length; materialIndex++)
                        {
                            Material material = materials[materialIndex];
                            if (material == null) continue;

                            if (TryRegister(exportedMaterials, $"mat:{material.name}"))
                            {
                                materialInfoLines.Add($"material={material.name} | shader={material.shader?.name ?? "null"} | object={GetGameObjectPath(obj)}");
                            }

                            string[] texturePropertyNames = material.GetTexturePropertyNames();
                            for (int texturePropertyIndex = 0; texturePropertyIndex < texturePropertyNames.Length; texturePropertyIndex++)
                            {
                                string propertyName = texturePropertyNames[texturePropertyIndex];
                                Texture texture = material.GetTexture(propertyName);
                                if (texture is Texture2D tex2D)
                                {
                                    usedTextureIds.Add(tex2D.GetInstanceID());
                                    materialInfoLines.Add($"material={material.name} | texProp={propertyName} | texture={tex2D.name}");
                                    if (TryRegister(exportedCurrentGame, $"tex:{tex2D.name}"))
                                        SaveTexture(tex2D, texturesPath);
                                }
                            }
                        }
                    }

                    Component uiImage = obj.GetComponent("Image");
                    if (uiImage != null)
                    {
                        PropertyInfo spriteProperty = uiImage.GetType().GetProperty("sprite");
                        Sprite sprite = spriteProperty?.GetValue(uiImage) as Sprite;
                        if (sprite != null && sprite.texture != null)
                        {
                            usedTextureIds.Add(sprite.texture.GetInstanceID());
                            usedSpriteTextureIds.Add(sprite.texture.GetInstanceID());
                            if (TryRegister(exportedCurrentGame, $"tex:{sprite.texture.name}"))
                                SaveTexture(sprite.texture, spritesPath);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MelonLogger.Warning($"Export-Fehler bei Objekt '{obj.name}': {ex.Message}");
                    greg.Exporter.ModFramework.Events.Publish(new greg.Exporter.ModErrorEvent(DateTime.UtcNow, "ExportObject", ex.Message));
                }
            }

            File.WriteAllLines(Path.Combine(scriptsPath, "components.txt"), exportedScriptTypes);
            File.WriteAllLines(Path.Combine(settingsPath, "objects.txt"), settingLines);
            File.WriteAllLines(Path.Combine(materialsPath, "materials.txt"), materialInfoLines);

            int notUsedMeshCount = 0;
            int notUsedTextureCount = 0;

            if (exportBetaNotUsed)
            {
                MelonLogger.Msg("Starte Beta-Export: nicht verwendete, aber geladene Assets...");

                HashSet<string> exportedBeta = new HashSet<string>();

                Mesh[] loadedMeshes = Resources.FindObjectsOfTypeAll<Mesh>();
                for (int meshIndex = 0; meshIndex < loadedMeshes.Length; meshIndex++)
                {
                    Mesh mesh = loadedMeshes[meshIndex];
                    if (mesh == null) continue;
                    if (usedMeshIds.Contains(mesh.GetInstanceID())) continue;
                    if (!IsCandidateNotUsedMesh(mesh)) continue;
                    if (!TryRegister(exportedBeta, $"mesh:{mesh.name}")) continue;
                    SaveMesh(mesh, notUsedModelsPath);
                    notUsedMeshCount++;
                }

                Texture2D[] loadedTextures = Resources.FindObjectsOfTypeAll<Texture2D>();
                for (int textureIndex = 0; textureIndex < loadedTextures.Length; textureIndex++)
                {
                    Texture2D tex = loadedTextures[textureIndex];
                    if (tex == null) continue;
                    if (usedTextureIds.Contains(tex.GetInstanceID())) continue;
                    if (!IsCandidateNotUsedTexture(tex)) continue;
                    if (!TryRegister(exportedBeta, $"tex:{tex.name}")) continue;
                    SaveTexture(tex, notUsedTexturesPath);
                    notUsedTextureCount++;
                }

                MelonLogger.Msg($"Export abgeschlossen! Verbaute Assets: {currentGamePath} | Nicht verwendet: {notUsedPath}");
            }
            else
            {
                MelonLogger.Msg($"Export abgeschlossen! Verbaute Assets: {currentGamePath} | NotUsed-Export deaktiviert (F10 zum Umschalten).");
            }

            var summaryLines = new List<string>
            {
                $"timestamp={DateTime.Now:yyyy-MM-dd HH:mm:ss}",
                $"scenesLoaded={SceneManager.sceneCount}",
                $"objectsScanned={settingLines.Count}",
                $"uniqueComponents={exportedScriptTypes.Count}",
                $"usedMeshes={usedMeshIds.Count}",
                $"usedTextures={usedTextureIds.Count}",
                $"usedSpriteTextures={usedSpriteTextureIds.Count}",
                $"usedMaterials={exportedMaterials.Count}",
                $"notUsedEnabled={exportBetaNotUsed}",
                $"notUsedMeshes={notUsedMeshCount}",
                $"notUsedTextures={notUsedTextureCount}"
            };
            File.WriteAllLines(Path.Combine(settingsPath, "summary.txt"), summaryLines);
            greg.Exporter.ModFramework.Events.Publish(new greg.Exporter.ExportCompletedEvent(DateTime.UtcNow, currentGamePath, settingLines.Count));
        }

        private IEnumerable<GameObject> EnumerateAllSceneObjects(bool includeInactive)
        {
            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                Scene scene = SceneManager.GetSceneAt(i);
                if (!scene.IsValid() || !scene.isLoaded) continue;

                GameObject[] roots = scene.GetRootGameObjects();
                for (int rootIndex = 0; rootIndex < roots.Length; rootIndex++)
                {
                    GameObject root = roots[rootIndex];
                    if (root == null) continue;

                    Queue<Transform> queue = new Queue<Transform>();
                    queue.Enqueue(root.transform);

                    while (queue.Count > 0)
                    {
                        Transform current = queue.Dequeue();
                        if (current == null || current.gameObject == null)
                            continue;

                        GameObject currentObject = current.gameObject;
                        if (includeInactive || currentObject.activeInHierarchy)
                            yield return currentObject;

                        int childCount;
                        try
                        {
                            childCount = current.childCount;
                        }
                        catch
                        {
                            childCount = 0;
                        }

                        for (int childIndex = 0; childIndex < childCount; childIndex++)
                        {
                            Transform child;
                            try
                            {
                                child = current.GetChild(childIndex);
                            }
                            catch
                            {
                                continue;
                            }

                            if (child != null)
                                queue.Enqueue(child);
                        }
                    }
                }
            }
        }

        private void LogUiPathUnderCursor()
        {
            if (Mouse.current == null)
            {
                MelonLogger.Warning("Keine Maus verfügbar.");
                greg.Exporter.ModFramework.Events.Publish(new greg.Exporter.ModErrorEvent(DateTime.UtcNow, "UIPath", "Keine Maus verfügbar"));
                return;
            }

            Type eventSystemType = Type.GetType("UnityEngine.EventSystems.EventSystem, UnityEngine.UI");
            Type pointerEventDataType = Type.GetType("UnityEngine.EventSystems.PointerEventData, UnityEngine.UI");
            Type raycastResultType = Type.GetType("UnityEngine.EventSystems.RaycastResult, UnityEngine.UI");

            if (eventSystemType == null || pointerEventDataType == null || raycastResultType == null)
            {
                MelonLogger.Warning("UI EventSystem-Typen konnten nicht aufgelöst werden.");
                greg.Exporter.ModFramework.Events.Publish(new greg.Exporter.ModErrorEvent(DateTime.UtcNow, "UIPath", "UI EventSystem-Typen konnten nicht aufgelöst werden"));
                return;
            }

            object currentEventSystem = eventSystemType.GetProperty("current", BindingFlags.Public | BindingFlags.Static)?.GetValue(null);
            if (currentEventSystem == null)
            {
                MelonLogger.Warning("Kein aktives EventSystem gefunden.");
                greg.Exporter.ModFramework.Events.Publish(new greg.Exporter.ModErrorEvent(DateTime.UtcNow, "UIPath", "Kein aktives EventSystem gefunden"));
                return;
            }

            object pointerEventData = Activator.CreateInstance(pointerEventDataType, currentEventSystem);
            pointerEventDataType.GetProperty("position")?.SetValue(pointerEventData, Mouse.current.position.ReadValue());

            Type il2CppListGeneric = Type.GetType("Il2CppSystem.Collections.Generic.List`1, Il2Cppmscorlib");
            if (il2CppListGeneric == null)
            {
                MelonLogger.Warning("Il2Cpp-Liste für UI-Raycasts konnte nicht aufgelöst werden.");
                greg.Exporter.ModFramework.Events.Publish(new greg.Exporter.ModErrorEvent(DateTime.UtcNow, "UIPath", "Il2Cpp-Liste für UI-Raycasts konnte nicht aufgelöst werden"));
                return;
            }

            Type listType = il2CppListGeneric.MakeGenericType(raycastResultType);
            object results = Activator.CreateInstance(listType);
            eventSystemType.GetMethod("RaycastAll")?.Invoke(currentEventSystem, new[] { pointerEventData, results });

            int resultCount = (int)(listType.GetProperty("Count")?.GetValue(results) ?? 0);

            if (resultCount == 0)
            {
                MelonLogger.Msg("Kein UI-Element unter dem Cursor gefunden.");
                return;
            }

            MethodInfo getItemMethod = listType.GetMethod("get_Item");
            if (getItemMethod == null)
            {
                MelonLogger.Warning("Il2Cpp-Raycast-Liste konnte nicht gelesen werden.");
                greg.Exporter.ModFramework.Events.Publish(new greg.Exporter.ModErrorEvent(DateTime.UtcNow, "UIPath", "Il2Cpp-Raycast-Liste konnte nicht gelesen werden"));
                return;
            }

            for (int i = 0; i < resultCount; i++)
            {
                object result = getItemMethod.Invoke(results, new object[] { i });
                GameObject gameObject = raycastResultType.GetProperty("gameObject")?.GetValue(result) as GameObject;
                if (gameObject == null) continue;

                string path = gameObject.name;
                Transform parent = gameObject.transform.parent;

                while (parent != null)
                {
                    path = parent.name + "/" + path;
                    parent = parent.parent;
                }

                MelonLogger.Msg("UI-Pfad gefunden: " + path);
            }
        }

        private static bool TryRegister(HashSet<string> exportedNames, string rawName)
        {
            if (string.IsNullOrWhiteSpace(rawName)) return false;
            if (rawName.ToLowerInvariant().Contains("unity")) return false;
            return exportedNames.Add(rawName);
        }

        private static string GetGameObjectPath(GameObject gameObject)
        {
            string path = gameObject.name;
            Transform parent = gameObject.transform.parent;
            while (parent != null)
            {
                path = parent.name + "/" + path;
                parent = parent.parent;
            }
            return path;
        }

        private static bool IsCandidateNotUsedMesh(Mesh mesh)
        {
            if (mesh == null) return false;
            if (mesh.vertexCount <= 0) return false;
            if (string.IsNullOrWhiteSpace(mesh.name)) return false;
            if (mesh.hideFlags == HideFlags.HideAndDontSave) return false;
            return true;
        }

        private static bool IsCandidateNotUsedTexture(Texture2D tex)
        {
            if (tex == null) return false;
            if (string.IsNullOrWhiteSpace(tex.name)) return false;
            if (tex.width <= 4 && tex.height <= 4) return false;
            if (tex.hideFlags == HideFlags.HideAndDontSave) return false;
            return true;
        }

        private void SaveTexture(Texture2D tex, string targetDirectory)
        {
            if (tex == null || string.IsNullOrEmpty(tex.name) || tex.name.Contains("unity")) return;
            if (!Directory.Exists(targetDirectory)) Directory.CreateDirectory(targetDirectory);

            // RenderTexture Trick um Read/Write-Sperre zu umgehen
            RenderTexture tmp = RenderTexture.GetTemporary(tex.width, tex.height, 0, RenderTextureFormat.Default, RenderTextureReadWrite.Linear);
            Graphics.Blit(tex, tmp);
            RenderTexture previous = RenderTexture.active;
            RenderTexture.active = tmp;

            Texture2D readableTex = new Texture2D(tex.width, tex.height);
            readableTex.ReadPixels(new Rect(0, 0, tmp.width, tmp.height), 0, 0);
            readableTex.Apply();

            RenderTexture.active = previous;
            RenderTexture.ReleaseTemporary(tmp);

            byte[] bytes = ImageConversion.EncodeToPNG(readableTex);
            string safeName = string.Join("_", tex.name.Split(Path.GetInvalidFileNameChars()));
            string filePath = EnsureUniquePath(targetDirectory, safeName, ".png");
            File.WriteAllBytes(filePath, bytes);

            // Objekt zerstören um Speicher zu sparen während des Exports
            UnityEngine.Object.Destroy(readableTex);
        }

        private void SaveMesh(Mesh mesh, string targetDirectory)
        {
            if (mesh == null || string.IsNullOrEmpty(mesh.name) || mesh.name.Contains("unity")) return;
            if (!Directory.Exists(targetDirectory)) Directory.CreateDirectory(targetDirectory);

            string safeName = string.Join("_", mesh.name.Split(Path.GetInvalidFileNameChars()));
            string filePath = EnsureUniquePath(targetDirectory, safeName, ".obj");

            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.Append("g ").Append(safeName).Append("\n");

            // Nutze die expliziten Unity-Typen um Konflikte mit System.Numerics zu vermeiden
            UnityEngine.Vector3[] vertices = mesh.vertices;
            for (int vertexIndex = 0; vertexIndex < vertices.Length; vertexIndex++)
            {
                UnityEngine.Vector3 vertex = vertices[vertexIndex];
                sb.Append(string.Format("v {0} {1} {2}\n", vertex.x, vertex.y, vertex.z).Replace(",", "."));
            }

            sb.Append("\n");

            UnityEngine.Vector3[] normals = mesh.normals;
            for (int normalIndex = 0; normalIndex < normals.Length; normalIndex++)
            {
                UnityEngine.Vector3 normal = normals[normalIndex];
                sb.Append(string.Format("vn {0} {1} {2}\n", normal.x, normal.y, normal.z).Replace(",", "."));
            }

            sb.Append("\n");

            UnityEngine.Vector2[] uvs = mesh.uv;
            for (int uvIndex = 0; uvIndex < uvs.Length; uvIndex++)
            {
                UnityEngine.Vector2 uv = uvs[uvIndex];
                sb.Append(string.Format("vt {0} {1}\n", uv.x, uv.y).Replace(",", "."));
            }

            for (int i = 0; i < mesh.subMeshCount; i++)
            {
                int[] triangles = mesh.GetTriangles(i);
                for (int j = 0; j < triangles.Length; j += 3)
                {
                    // OBJ Format Indizes starten bei 1
                    sb.Append(string.Format("f {0}/{0}/{0} {1}/{1}/{1} {2}/{2}/{2}\n",
                        triangles[j] + 1, triangles[j + 1] + 1, triangles[j + 2] + 1));
                }
            }

            File.WriteAllText(filePath, sb.ToString());
        }

        private static string EnsureUniquePath(string directory, string baseName, string extension)
        {
            string filePath = Path.Combine(directory, baseName + extension);
            int i = 1;
            while (File.Exists(filePath))
            {
                filePath = Path.Combine(directory, $"{baseName}_{i}{extension}");
                i++;
            }

            return filePath;
        }

        private static Component[] GetComponentsSafe(GameObject gameObject)
        {
            if (gameObject == null)
                return Array.Empty<Component>();

            try
            {
                return gameObject.GetComponents<Component>() ?? Array.Empty<Component>();
            }
            catch (Exception ex) when (IsSpanInteropMethodMissing(ex))
            {
                MelonLogger.Warning($"Unity6/Il2Cpp Span fallback aktiv für Objekt '{gameObject.name}'.");
            }
            catch
            {
            }

            try
            {
                MethodInfo getComponentsByType = typeof(GameObject).GetMethod("GetComponents", new[] { typeof(Type) });
                if (getComponentsByType == null)
                    return Array.Empty<Component>();

                object raw = getComponentsByType.Invoke(gameObject, new object[] { typeof(Component) });
                if (raw is not Array rawArray)
                    return Array.Empty<Component>();

                Component[] managedComponents = new Component[rawArray.Length];
                for (int index = 0; index < rawArray.Length; index++)
                    managedComponents[index] = rawArray.GetValue(index) as Component;

                return managedComponents;
            }
            catch
            {
                return Array.Empty<Component>();
            }
        }

        private static bool IsSpanInteropMethodMissing(Exception exception)
        {
            Exception current = exception;
            while (current != null)
            {
                string message = current.Message ?? string.Empty;
                if (message.Contains("GetPinnableReference", StringComparison.OrdinalIgnoreCase)
                    && message.Contains("ReadOnlySpan", StringComparison.OrdinalIgnoreCase)
                    && message.Contains("Method not found", StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }

                current = current.InnerException;
            }

            return false;
        }
    }
}
