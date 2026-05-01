using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.TextCore.Text;

public static class BuildGregCoreFontBundle
{
    private const string BundleName = "gregcore_fonts";
    private const string OutputDir = "Assets/StreamingAssets/Fonts";
    private const string SourceFontDir = "Assets/Fonts";

    [MenuItem("Assets/Build gregCore Font Bundle")]
    public static void BuildBundle()
    {
        Debug.Log("[FontBundle] ===== START =====");

        if (!Directory.Exists(SourceFontDir))
        {
            Debug.LogError($"[FontBundle] ❌ Source directory not found: {SourceFontDir}");
            return;
        }

        Directory.CreateDirectory(OutputDir);
        Debug.Log($"[FontBundle] Output directory: {OutputDir}");

        var allFiles = Directory.GetFiles(SourceFontDir)
            .Where(f => f.EndsWith(".ttf") || f.EndsWith(".otf"))
            .ToArray();

        Debug.Log($"[FontBundle] Found {allFiles.Length} font files");

        if (allFiles.Length == 0)
        {
            Debug.LogError("[FontBundle] ❌ No .ttf/.otf files found!");
            return;
        }

        AssetDatabase.StartAssetEditing();
        try
        {
            int imported = 0;
            foreach (var file in allFiles)
            {
                Debug.Log($"[FontBundle] Processing: {Path.GetFileName(file)}");
                if (ImportFont(file))
                    imported++;
            }

            Debug.Log($"[FontBundle] Successfully imported: {imported} fonts");

            if (imported == 0)
            {
                Debug.LogError("[FontBundle] ❌ No fonts could be imported. Aborting.");
                return;
            }

            BuildAssetBundle();
        }
        finally
        {
            AssetDatabase.StopAssetEditing();
        }

        Debug.Log("[FontBundle] ===== DONE =====");
    }

    private static bool ImportFont(string filePath)
    {
        string fileName = Path.GetFileNameWithoutExtension(filePath);
        string assetPath = $"Assets/Fonts/{Path.GetFileName(filePath)}";

        // Step 1: Import the raw font
        AssetDatabase.ImportAsset(assetPath, ImportAssetOptions.ForceUpdate);
        Debug.Log($"[FontBundle] Imported raw font: {assetPath}");

        // Step 2: Load it
        Font font = AssetDatabase.LoadAssetAtPath<Font>(assetPath);
        if (font == null)
        {
            Debug.LogError($"[FontBundle] ❌ Could not load Font asset at: {assetPath}");
            return false;
        }
        Debug.Log($"[FontBundle] Loaded Font: {font.name}");

        // Step 3: Create FontAsset for UI Toolkit
        string fontAssetPath = $"Assets/Fonts/{fileName}_FontAsset.asset";
        FontAsset fontAsset = AssetDatabase.LoadAssetAtPath<FontAsset>(fontAssetPath);

        if (fontAsset == null)
        {
            Debug.Log($"[FontBundle] Creating new FontAsset for: {fileName}");

            // Try different CreateFontAsset overloads for compatibility
            try
            {
                // Unity 6000+ style
                fontAsset = FontAsset.CreateFontAsset(
                    font,
                    90,     // samplingPointSize
                    9,      // atlasPadding
                    GlyphRenderMode.SDFAA,
                    1024,   // atlasWidth
                    1024,   // atlasHeight
                    AtlasPopulationMode.Dynamic
                );
            }
            catch (System.Exception ex1)
            {
                Debug.LogWarning($"[FontBundle] First CreateFontAsset attempt failed: {ex1.Message}");
                try
                {
                    // Fallback: simpler overload
                    fontAsset = FontAsset.CreateFontAsset(font);
                }
                catch (System.Exception ex2)
                {
                    Debug.LogError($"[FontBundle] ❌ CreateFontAsset completely failed: {ex2.Message}");
                    return false;
                }
            }

            if (fontAsset == null)
            {
                Debug.LogError($"[FontBundle] ❌ FontAsset.CreateFontAsset returned null for: {fileName}");
                return false;
            }

            fontAsset.name = fileName;
            AssetDatabase.CreateAsset(fontAsset, fontAssetPath);
            Debug.Log($"[FontBundle] ✅ Created FontAsset: {fontAssetPath}");
        }
        else
        {
            Debug.Log($"[FontBundle] Updating existing FontAsset: {fontAssetPath}");
            EditorUtility.SetDirty(fontAsset);
        }

        // Step 4: Assign to asset bundle
        AssetImporter importer = AssetImporter.GetAtPath(fontAssetPath);
        if (importer == null)
        {
            Debug.LogError($"[FontBundle] ❌ Could not get AssetImporter for: {fontAssetPath}");
            return false;
        }

        importer.assetBundleName = BundleName;
        Debug.Log($"[FontBundle] Assigned bundle '{BundleName}' to: {fontAssetPath}");

        return true;
    }

    private static void BuildAssetBundle()
    {
        string[] assetPaths = GetFontAssetPaths();
        Debug.Log($"[FontBundle] Assets to bundle: {assetPaths.Length}");
        foreach (var p in assetPaths)
            Debug.Log($"[FontBundle]   - {p}");

        if (assetPaths.Length == 0)
        {
            Debug.LogError("[FontBundle] ❌ No FontAsset paths found! Cannot build empty bundle.");
            return;
        }

        var buildMap = new AssetBundleBuild[]
        {
            new AssetBundleBuild
            {
                assetBundleName = BundleName,
                assetNames = assetPaths
            }
        };

        Debug.Log($"[FontBundle] Building bundle '{BundleName}' to: {OutputDir}");
        Debug.Log($"[FontBundle] Build target: {EditorUserBuildSettings.activeBuildTarget}");

        try
        {
            var manifest = BuildPipeline.BuildAssetBundles(
                OutputDir,
                buildMap,
                BuildAssetBundleOptions.ForceRebuildAssetBundle,
                EditorUserBuildSettings.activeBuildTarget
            );

            if (manifest == null)
            {
                Debug.LogError("[FontBundle] ❌ BuildPipeline.BuildAssetBundles returned null!");
                return;
            }

            Debug.Log("[FontBundle] BuildAssetBundles completed.");
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"[FontBundle] ❌ Exception during BuildAssetBundles: {ex}");
            return;
        }

        // Check multiple possible output paths
        string[] possiblePaths = new[]
        {
            Path.Combine(OutputDir, BundleName),
            Path.Combine(OutputDir, BundleName + ".unity3d"),
            Path.Combine(OutputDir, BundleName + ".ab"),
            Path.Combine(OutputDir, "Fonts", BundleName),
        };

        bool found = false;
        foreach (var path in possiblePaths)
        {
            if (File.Exists(path))
            {
                var fi = new FileInfo(path);
                Debug.Log($"[FontBundle] ✅ BUNDLE FOUND: {path}");
                Debug.Log($"[FontBundle]    Size: {fi.Length / 1024} KB");
                Debug.Log($"[FontBundle]    Copy to: <Data Center>/StreamingAssets/Fonts/{Path.GetFileName(path)}");
                found = true;
                break;
            }
        }

        if (!found)
        {
            Debug.LogError("[FontBundle] ❌ Bundle file not found at any expected path!");
            Debug.Log($"[FontBundle] Checking directory contents of: {OutputDir}");
            if (Directory.Exists(OutputDir))
            {
                foreach (var f in Directory.GetFiles(OutputDir))
                {
                    Debug.Log($"[FontBundle]   Found: {Path.GetFileName(f)} ({new FileInfo(f).Length / 1024} KB)");
                }
            }
            else
            {
                Debug.LogError($"[FontBundle] Output directory doesn't exist: {OutputDir}");
            }
        }
    }

    private static string[] GetFontAssetPaths()
    {
        var guids = AssetDatabase.FindAssets("t:FontAsset", new[] { SourceFontDir });
        var paths = new string[guids.Length];
        for (int i = 0; i < guids.Length; i++)
        {
            paths[i] = AssetDatabase.GUIDToAssetPath(guids[i]);
        }
        return paths;
    }
}
