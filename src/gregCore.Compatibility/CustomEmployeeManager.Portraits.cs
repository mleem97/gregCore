using System;
using System.Collections.Generic;
using System.IO;
using DataCenterModLoader;
using Il2Cpp;
using MelonLoader.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace gregCore.API;

public static partial class CustomEmployeeManager
{
    private static void SetPortrait(Transform card, string employeeId)
    {
        try
        {
            var portraitTransform = card.Find("Image");
            if (portraitTransform == null) return;
            if (TryLoadPortrait(portraitTransform, employeeId)) return;
            ApplyPortraitFallback(portraitTransform);
        }
        catch (Exception ex)
        {
            CrashLog.LogException("SetPortrait", ex);
        }
    }

    private static bool TryLoadPortrait(Transform portraitTransform, string employeeId)
    {
        try
        {
            string imagePath = null!;
            if (!TryResolvePortraitPath(employeeId, out imagePath))
            {
                CrashLog.Log($"[Security] CustomEmployee: rejected portrait id='{employeeId}'");
                return false;
            }
            if (imagePath == null) return false;
            return TryDecodePortrait(portraitTransform, employeeId, imagePath);
        }
        catch (Exception ex)
        {
            CrashLog.LogException($"SetPortrait: load image for '{employeeId}'", ex);
            return false;
        }
    }

    private static bool TryDecodePortrait(Transform portraitTransform, string employeeId, string imagePath)
    {
        try
        {
            byte[] imageData = File.ReadAllBytes(imagePath);
            var tex = new Texture2D(2, 2, TextureFormat.RGBA32, false);
            if (!ImageConversion.LoadImage(tex, imageData))
            {
                CrashLog.Log($"CustomEmployee: Failed to decode image at '{imagePath}' for '{employeeId}'");
                return false;
            }
            ApplyPortraitTexture(portraitTransform, tex);
            CrashLog.Log($"CustomEmployee: Loaded portrait from '{imagePath}' for '{employeeId}'");
            return true;
        }
        catch (Exception ex)
        {
            CrashLog.LogException($"SetPortrait: load image for '{employeeId}'", ex);
            return false;
        }
    }

    private static void ApplyPortraitTexture(Transform portraitTransform, Texture2D tex)
    {
        try
        {
            var sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f), 100f);
            var image = portraitTransform.GetComponent<Image>();
            if (image != null)
            {
                image.sprite = sprite;
                image.color = Color.white;
                image.preserveAspect = true;
            }
            var rawImage = portraitTransform.GetComponent<RawImage>();
            if (rawImage != null)
            {
                rawImage.texture = tex;
                rawImage.color = Color.white;
            }
        }
        catch { /* ignored: defensive best-effort (CONVENTIONS.md) */  }
    }

    private static void ApplyPortraitFallback(Transform portraitTransform)
    {
        try
        {
            var fallbackImage = portraitTransform.GetComponent<Image>();
            if (fallbackImage != null)
            {
                fallbackImage.sprite = null;
                fallbackImage.color = new Color(0.0f, 0.6f, 0.7f, 1f);
            }
            var fallbackRaw = portraitTransform.GetComponent<RawImage>();
            if (fallbackRaw != null)
            {
                fallbackRaw.texture = null;
                fallbackRaw.color = new Color(0.0f, 0.6f, 0.7f, 1f);
            }
        }
        catch { }
    }

    private static bool TryResolvePortraitPath(string employeeId, out string? imagePath)
    {
        imagePath = null;
        if (string.IsNullOrWhiteSpace(employeeId) || Path.IsPathRooted(employeeId)) return false;
        if (employeeId.IndexOfAny(new[] { '/', '\\', '\0' }) >= 0 || employeeId.Contains("..", StringComparison.Ordinal))
            return false;
        try
        {
            var assetsRoot = Path.GetFullPath(Path.Combine(MelonEnvironment.UserDataDirectory, "ModAssets"));
            var rootPrefix = assetsRoot.EndsWith(Path.DirectorySeparatorChar) ? assetsRoot : assetsRoot + Path.DirectorySeparatorChar;
            return TryFindPortraitFile(assetsRoot, rootPrefix, employeeId, out imagePath);
        }
        catch (Exception ex)
        {
            CrashLog.LogException($"ResolvePortraitPath for '{employeeId}'", ex);
        }
        return true;
    }

    private static bool TryFindPortraitFile(string assetsRoot, string rootPrefix, string employeeId, out string? imagePath)
    {
        imagePath = null;
        try
        {
            foreach (var extension in new[] { ".jpg", ".png" })
            {
                var candidate = Path.GetFullPath(Path.Combine(assetsRoot, employeeId + extension));
                if (!candidate.StartsWith(rootPrefix, StringComparison.OrdinalIgnoreCase)) return false;
                if (File.Exists(candidate))
                {
                    imagePath = candidate;
                    return true;
                }
            }
        }
        catch { }
        return true;
    }

    private static void RefreshAllCards()
    {
        try
        {
            var hrSystems = UnityEngine.Object.FindObjectsOfType<HRSystem>();
            if (hrSystems == null) return;
            for (int h = 0; h < hrSystems.Length; h++)
            {
                var hr = hrSystems[h];
                if (hr == null) continue;
                Transform contentGrid = ResolveRefreshGrid(hr);
                if (contentGrid == null) continue;
                RefreshGridCards(contentGrid);
            }
        }
        catch (Exception ex)
        {
            CrashLog.LogException("RefreshAllCards", ex);
        }
    }

    private static Transform ResolveRefreshGrid(HRSystem hr)
    {
        try
        {
            var scrollView = hr.transform.Find("ModScrollView");
            Transform contentGrid = null!;
            if (scrollView != null) contentGrid = scrollView.Find("Viewport/Content")!;
            if (contentGrid == null) contentGrid = hr.transform.Find("Grid")!;
            return contentGrid;
        }
        catch { return null!; }
    }

    private static void RefreshGridCards(Transform contentGrid)
    {
        try
        {
            foreach (var entry in _employees)
            {
                var cardTransform = contentGrid.Find("CustomEmployee_" + entry.EmployeeId);
                if (cardTransform != null) UpdateCard(cardTransform, entry);
            }
        }
        catch { }
    }

    private static void LogHierarchy(Transform t, int depth)
    {
        if (_hierarchyLogged) return;
        if (depth == 0) BeginHierarchyDump();
        try
        {
            LogHierarchyNode(t, depth);
            LogHierarchyChildren(t, depth);
        }
        catch { }
        if (depth == 0) CrashLog.Log("=== end hierarchy dump ===");
    }

    private static void BeginHierarchyDump()
    {
        CrashLog.Log("=== HRSystem hierarchy dump ===");
        _hierarchyLogged = true;
    }

    private static void LogHierarchyNode(Transform t, int depth)
    {
        try
        {
            string indent = new string(' ', depth * 2);
            string activeFlag = t.gameObject.activeSelf ? "" : " [INACTIVE]";
            string compsStr = CollectComponentNames(t);
            CrashLog.Log($"{indent}{t.name}{activeFlag}{compsStr}");
        }
        catch { }
    }

    private static string CollectComponentNames(Transform t)
    {
        try
        {
            var components = t.gameObject.GetComponents<Component>();
            var compNames = new List<string>();
            if (components == null) return "";
            for (int i = 0; i < components.Count; i++)
            {
                try
                {
                    var comp = components[i];
                    if (comp != null) compNames.Add(comp.GetIl2CppType().Name);
                }
                catch { }
            }
            return compNames.Count > 0 ? " [" + string.Join(", ", compNames) + "]" : "";
        }
        catch { return ""; }
    }

    private static void LogHierarchyChildren(Transform t, int depth)
    {
        try
        {
            for (int i = 0; i < t.childCount; i++)
            {
                try { LogHierarchy(t.GetChild(i), depth + 1); } catch { }
            }
        }
        catch { }
    }
}
