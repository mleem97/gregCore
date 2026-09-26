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
        AssetDatabase.ImportAsset(assetPath, ImportAssetOptions.ForceUpdate);
        Debug.Log($"[FontBundle] Imported raw font: {assetPath}");
        Font font = LoadFont(assetPath);
        if (font == null) return false;
        string fontAssetPath = $"Assets/Fonts/{fileName}_FontAsset.asset";
        if (!EnsureFontAsset(font, fileName, fontAssetPath)) return false;
        return AssignBundle(fontAssetPath);
    }

    private static Font LoadFont(string assetPath)
    {
        try
        {
            Font font = AssetDatabase.LoadAssetAtPath<Font>(assetPath);
            if (font == null)
            {
                Debug.LogError($"[FontBundle] Could not load Font asset at: {assetPath}");
                return null;
            }
            Debug.Log($"[FontBundle] Loaded Font: {font.name}");
            return font;
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"[FontBundle] Load failed: {ex.Message}");
            return null;
        }
    }

    private static bool EnsureFontAsset(Font font, string fileName, string fontAssetPath)
    {
        try
        {
            FontAsset fontAsset = AssetDatabase.LoadAssetAtPath<FontAsset>(fontAssetPath);
            if (fontAsset != null)
            {
                Debug.Log($"[FontBundle] Updating existing FontAsset: {fontAssetPath}");
                EditorUtility.SetDirty(fontAsset);
                return true;
            }
            return CreateFontAsset(font, fileName, fontAssetPath);
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"[FontBundle] Ensure failed: {ex.Message}");
            return false;
        }
    }

    private static bool CreateFontAsset(Font font, string fileName, string fontAssetPath)
    {
        try
        {
            Debug.Log($"[FontBundle] Creating new FontAsset for: {fileName}");
            FontAsset fontAsset = TryCreateFull(font) ?? TryCreateSimple(font);
            if (fontAsset == null)
            {
                Debug.LogError($"[FontBundle] FontAsset.CreateFontAsset returned null for: {fileName}");
                return false;
            }
            fontAsset.name = fileName;
            AssetDatabase.CreateAsset(fontAsset, fontAssetPath);
            Debug.Log($"[FontBundle] Created FontAsset: {fontAssetPath}");
            return true;
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"[FontBundle] Create failed: {ex.Message}");
            return false;
        }
    }

    private static FontAsset TryCreateFull(Font font)
    {
        try
        {
            return FontAsset.CreateFontAsset(font, 90, 9, GlyphRenderMode.SDFAA, 1024, 1024, AtlasPopulationMode.Dynamic);
        }
        catch (System.Exception ex1)
        {
            Debug.LogWarning($"[FontBundle] First CreateFontAsset attempt failed: {ex1.Message}");
            return null;
        }
    }

    private static FontAsset TryCreateSimple(Font font)
    {
        try
        {
            return FontAsset.CreateFontAsset(font);
        }
        catch (System.Exception ex2)
        {
            Debug.LogError($"[FontBundle] CreateFontAsset completely failed: {ex2.Message}");
            return null;
        }
    }

    private static bool AssignBundle(string fontAssetPath)
    {
        try
        {
            AssetImporter importer = AssetImporter.GetAtPath(fontAssetPath);
            if (importer == null)
            {
                Debug.LogError($"[FontBundle] Could not get AssetImporter for: {fontAssetPath}");
                return false;
            }
            importer.assetBundleName = BundleName;
            Debug.Log($"[FontBundle] Assigned bundle '{BundleName}' to: {fontAssetPath}");
            return true;
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"[FontBundle] Assign failed: {ex.Message}");
            return false;
        }
    }

    private static void BuildAssetBundle()
    {
        string[] assetPaths = GetFontAssetPaths();
        LogAssetList(assetPaths);
        if (assetPaths.Length == 0)
        {
            Debug.LogError("[FontBundle] No FontAsset paths found! Cannot build empty bundle.");
            return;
        }
        if (!TryBuild(assetPaths)) return;
        VerifyOutput();
    }

    private static void LogAssetList(string[] assetPaths)
    {
        try
        {
            Debug.Log($"[FontBundle] Assets to bundle: {assetPaths.Length}");
            foreach (var p in assetPaths)
                Debug.Log($"[FontBundle]   - {p}");
        }
        catch { /* ignored: best-effort editor bundling; failures surface via Debug.Log below */ }
    }

    private static bool TryBuild(string[] assetPaths)
    {
        try
        {
            var buildMap = new AssetBundleBuild[]
            {
                new AssetBundleBuild { assetBundleName = BundleName, assetNames = assetPaths }
            };
            Debug.Log($"[FontBundle] Building bundle '{BundleName}' to: {OutputDir}");
            Debug.Log($"[FontBundle] Build target: {EditorUserBuildSettings.activeBuildTarget}");
            var manifest = BuildPipeline.BuildAssetBundles(OutputDir, buildMap, BuildAssetBundleOptions.ForceRebuildAssetBundle, EditorUserBuildSettings.activeBuildTarget);
            if (manifest == null)
            {
                Debug.LogError("[FontBundle] BuildPipeline.BuildAssetBundles returned null!");
                return false;
            }
            Debug.Log("[FontBundle] BuildAssetBundles completed.");
            return true;
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"[FontBundle] Exception during BuildAssetBundles: {ex}");
            return false;
        }
    }

    private static void VerifyOutput()
    {
        try
        {
            string[] possiblePaths = new[]
            {
                Path.Combine(OutputDir, BundleName),
                Path.Combine(OutputDir, BundleName + ".unity3d"),
                Path.Combine(OutputDir, BundleName + ".ab"),
                Path.Combine(OutputDir, "Fonts", BundleName),
            };
            foreach (var path in possiblePaths)
            {
                if (TryReportFound(path)) return;
            }
            ReportMissing();
        }
        catch { /* ignored: best-effort editor scan; failures surface via return value */ }
    }

    private static bool TryReportFound(string path)
    {
        try
        {
            if (!File.Exists(path)) return false;
            var fi = new FileInfo(path);
            Debug.Log($"[FontBundle] BUNDLE FOUND: {path}");
            Debug.Log($"[FontBundle]    Size: {fi.Length / 1024} KB");
            Debug.Log($"[FontBundle]    Copy to: <Data Center>/StreamingAssets/Fonts/{Path.GetFileName(path)}");
            return true;
        }
        catch { return false; }
    }

    private static void ReportMissing()
    {
        try
        {
            Debug.LogError("[FontBundle] Bundle file not found at any expected path!");
            Debug.Log($"[FontBundle] Checking directory contents of: {OutputDir}");
            if (!Directory.Exists(OutputDir))
            {
                Debug.LogError($"[FontBundle] Output directory doesn't exist: {OutputDir}");
                return;
            }
            foreach (var f in Directory.GetFiles(OutputDir))
            {
                Debug.Log($"[FontBundle]   Found: {Path.GetFileName(f)} ({new FileInfo(f).Length / 1024} KB)");
            }
        }
        catch { /* ignored: best-effort editor scan; failures surface via return value */ }
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
