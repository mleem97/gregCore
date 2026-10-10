using System;
using System.Collections.Generic;
using DataCenterModLoader;
using Il2Cpp;
using UnityEngine;
using UnityEngine.UI;

namespace gregCore.API;

public static partial class CustomEmployeeManager
{
    private static Transform EnsureScrollView(Transform hrTransform, Transform grid)
    {
        var reused = ReuseExistingScrollContent(hrTransform);
        if (reused != null) return reused;
        CrashLog.Log("CustomEmployee: Creating ScrollView wrapper for Grid");
        var scrollGO = BuildScrollRoot(grid);
        var viewportRect = BuildViewport(scrollGO);
        var contentGO = BuildContent(viewportGO: viewportRect.gameObject, grid);
        MoveChildrenToContent(grid, contentGO.transform);
        ConfigureScrollRect(scrollGO, contentGO, viewportRect);
        grid.gameObject.SetActive(false);
        _injectedContent = contentGO.transform;
        _scrollViewInjected = true;
        CrashLog.Log("CustomEmployee: ScrollView injection complete");
        return contentGO.transform;
    }

    private static Transform ReuseExistingScrollContent(Transform hrTransform)
    {
        try
        {
            var existingScroll = hrTransform.Find("ModScrollView");
            if (existingScroll == null) return null!;
            var existingContent = existingScroll.Find("Viewport/Content");
            if (existingContent == null) return null!;
            CrashLog.Log("CustomEmployee: ScrollView already exists, reusing");
            return existingContent;
        }
        catch { return null!; }
    }

    private static GameObject BuildScrollRoot(Transform grid)
    {
        var gridRect = grid.GetComponent<RectTransform>();
        var gridParent = grid.parent;
        int siblingIndex = grid.GetSiblingIndex();
        var scrollGO = new GameObject("ModScrollView");
        scrollGO.AddComponent<RectTransform>();
        scrollGO.transform.SetParent(gridParent, false);
        scrollGO.transform.SetSiblingIndex(siblingIndex);
        var rt = scrollGO.GetComponent<RectTransform>();
        rt.anchorMin = gridRect.anchorMin;
        rt.anchorMax = gridRect.anchorMax;
        rt.offsetMin = gridRect.offsetMin;
        rt.offsetMax = gridRect.offsetMax;
        rt.pivot = gridRect.pivot;
        rt.sizeDelta = gridRect.sizeDelta;
        rt.anchoredPosition = gridRect.anchoredPosition;
        return scrollGO;
    }

    private static RectTransform BuildViewport(GameObject scrollGO)
    {
        var viewportGO = new GameObject("Viewport");
        viewportGO.AddComponent<RectTransform>();
        viewportGO.AddComponent<RectMask2D>();
        viewportGO.AddComponent<Image>();
        viewportGO.transform.SetParent(scrollGO.transform, false);
        var viewportRect = viewportGO.GetComponent<RectTransform>();
        viewportRect.anchorMin = Vector2.zero;
        viewportRect.anchorMax = Vector2.one;
        viewportRect.offsetMin = Vector2.zero;
        viewportRect.offsetMax = Vector2.zero;
        viewportRect.pivot = new Vector2(0.5f, 1f);
        var viewportImage = viewportGO.GetComponent<Image>();
        viewportImage.color = new Color(0, 0, 0, 0);
        viewportImage.raycastTarget = true;
        return viewportRect;
    }

    private static GameObject BuildContent(GameObject viewportGO, Transform grid)
    {
        var contentGO = new GameObject("Content");
        contentGO.AddComponent<RectTransform>();
        contentGO.AddComponent<ContentSizeFitter>();
        contentGO.transform.SetParent(viewportGO.transform, false);
        var contentRect = contentGO.GetComponent<RectTransform>();
        contentRect.anchorMin = new Vector2(0, 1);
        contentRect.anchorMax = new Vector2(1, 1);
        contentRect.pivot = new Vector2(0.5f, 1f);
        contentRect.offsetMin = new Vector2(0, 0);
        contentRect.offsetMax = new Vector2(0, 0);
        contentRect.sizeDelta = new Vector2(0, 0);
        var fitter = contentGO.GetComponent<ContentSizeFitter>();
        fitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
        fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        CopyGridLayout(grid, contentGO);
        return contentGO;
    }

    private static void CopyGridLayout(Transform grid, GameObject contentGO)
    {
        try
        {
            var srcLayout = grid.GetComponent<GridLayoutGroup>();
            if (srcLayout != null)
            {
                CopyGridLayoutGroup(srcLayout, contentGO);
                return;
            }
            CopyFallbackLayouts(grid, contentGO);
        }
        catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
    }

    private static void CopyGridLayoutGroup(GridLayoutGroup src, GameObject contentGO)
    {
        try
        {
            var dstLayout = contentGO.AddComponent<GridLayoutGroup>();
            dstLayout.cellSize = src.cellSize;
            dstLayout.spacing = src.spacing;
            dstLayout.startCorner = src.startCorner;
            dstLayout.startAxis = src.startAxis;
            dstLayout.childAlignment = src.childAlignment;
            dstLayout.constraint = src.constraint;
            dstLayout.constraintCount = src.constraintCount;
            dstLayout.padding = src.padding;
            CrashLog.Log($"CustomEmployee: Copied GridLayoutGroup (cellSize={dstLayout.cellSize}, spacing={dstLayout.spacing}, constraint={dstLayout.constraint}, count={dstLayout.constraintCount})");
        }
        catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
    }

    private static void CopyFallbackLayouts(Transform grid, GameObject contentGO)
    {
        try
        {
            CrashLog.Log("CustomEmployee: Grid has no GridLayoutGroup, checking for other layouts...");
            var hLayout = grid.GetComponent<HorizontalLayoutGroup>();
            if (hLayout != null)
            {
                var dst = contentGO.AddComponent<HorizontalLayoutGroup>();
                dst.spacing = hLayout.spacing;
                dst.childAlignment = hLayout.childAlignment;
                dst.padding = hLayout.padding;
            }
            var vLayout = grid.GetComponent<VerticalLayoutGroup>();
            if (vLayout != null)
            {
                var dst = contentGO.AddComponent<VerticalLayoutGroup>();
                dst.spacing = vLayout.spacing;
                dst.childAlignment = vLayout.childAlignment;
                dst.padding = vLayout.padding;
            }
        }
        catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
    }

    private static void MoveChildrenToContent(Transform grid, Transform content)
    {
        try
        {
            var childrenToMove = new List<Transform>();
            for (int i = 0; i < grid.childCount; i++)
                childrenToMove.Add(grid.GetChild(i));
            foreach (var child in childrenToMove)
                child.SetParent(content, false);
            CrashLog.Log($"CustomEmployee: Moved {childrenToMove.Count} children from Grid to Content");
        }
        catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
    }

    private static void ConfigureScrollRect(GameObject scrollGO, GameObject contentGO, RectTransform viewportRect)
    {
        try
        {
            var contentRect = contentGO.GetComponent<RectTransform>();
            var scrollComp = scrollGO.AddComponent<ScrollRect>();
            scrollComp.content = contentRect;
            scrollComp.viewport = viewportRect;
            scrollComp.horizontal = false;
            scrollComp.vertical = true;
            scrollComp.movementType = ScrollRect.MovementType.Clamped;
            scrollComp.scrollSensitivity = 30f;
            scrollComp.inertia = true;
            scrollComp.decelerationRate = 0.1f;
        }
        catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
    }

    public static void InjectIntoHRSystem(HRSystem hrSystem)
    {
        if (_employees.Count == 0) return;
        try
        {
            var hrTransform = hrSystem.gameObject.transform;
            LogHierarchy(hrTransform, 0);
            Transform contentGrid = ResolveContentGrid(hrTransform, hrSystem);
            if (contentGrid == null) return;
            CrashLog.Log($"CustomEmployee: Using content grid '{contentGrid.name}' with {contentGrid.childCount} children");
            Transform templateCard = FindTemplateCard(contentGrid);
            if (templateCard == null) return;
            CrashLog.Log($"CustomEmployee: Using template '{templateCard.name}'");
            InjectAllCards(contentGrid, templateCard);
        }
        catch (Exception ex)
        {
            CrashLog.LogException("InjectIntoHRSystem", ex);
        }
    }

    private static Transform ResolveContentGrid(Transform hrTransform, HRSystem hrSystem)
    {
        try
        {
            if (_injectedContent != null)
            {
                CrashLog.Log($"CustomEmployee: Reusing cached content '{_injectedContent.name}' ({_injectedContent.childCount} children)");
                return _injectedContent;
            }
            Transform grid = FindContainerGrid(hrTransform, hrSystem);
            if (grid == null)
            {
                CrashLog.Log("CustomEmployee: Could not find employee card container in HRSystem");
                return null!;
            }
            CrashLog.Log($"CustomEmployee: Found container '{grid.name}' with {grid.childCount} children");
            return EnsureScrollView(hrTransform, grid);
        }
        catch { return null!; }
    }

    private static Transform FindContainerGrid(Transform hrTransform, HRSystem hrSystem)
    {
        try
        {
            Transform grid = hrTransform.Find("Grid");
            if (grid != null)
            {
                CrashLog.Log("CustomEmployee: Found container via legacy 'Grid' child");
                return grid;
            }
            grid = FindGridByButtons(hrSystem);
            if (grid != null) return grid;
            return FindGridByScan(hrTransform);
        }
        catch { return null!; }
    }

    private static Transform FindGridByButtons(HRSystem hrSystem)
    {
        try
        {
            var hireButtons = hrSystem.buttonsHireEmployees;
            var fireButtons = hrSystem.buttonsFireEmployees;
            Transform btn0 = hireButtons?.Length > 0 ? hireButtons[0]?.transform : null!;
            Transform btn1 = hireButtons?.Length > 1 ? hireButtons[1]?.transform
                           : fireButtons?.Length > 0 ? fireButtons[0]?.transform : null!;
            if (btn0 != null && btn1 != null)
            {
                var grid = FindLowestCommonAncestor(btn0, btn1);
                if (grid != null) CrashLog.Log($"CustomEmployee: Found container via button LCA: '{grid.name}'");
                return grid!;
            }
            if (btn0 != null)
            {
                var grid = btn0.parent?.parent?.parent ?? btn0.parent?.parent;
                if (grid != null) CrashLog.Log($"CustomEmployee: Found container via button walk-up: '{grid.name}'");
                return grid!;
            }
        }
        catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
        return null!;
    }

    private static Transform FindGridByScan(Transform hrTransform)
    {
        try
        {
            Transform grid = null!;
            int maxChildren = 0;
            for (int i = 0; i < hrTransform.childCount; i++)
            {
                var c = hrTransform.GetChild(i);
                if (c.childCount > maxChildren) { maxChildren = c.childCount; grid = c; }
            }
            if (grid != null && maxChildren > 0)
                CrashLog.Log($"CustomEmployee: Found container by scan: '{grid.name}' ({maxChildren} children)");
            return grid!;
        }
        catch { return null!; }
    }

    private static Transform FindTemplateCard(Transform contentGrid)
    {
        try
        {
            Transform templateCard = null!;
            for (int i = contentGrid.childCount - 1; i >= 0; i--)
            {
                var child = contentGrid.GetChild(i);
                if (!child.gameObject.activeSelf || child.name.StartsWith("CustomEmployee_")) continue;
                if (child.name.StartsWith("EmployeeCard")) return child;
                if (templateCard == null) templateCard = child;
            }
            if (templateCard == null)
                CrashLog.Log("CustomEmployee: No EmployeeCard template found in content grid");
            return templateCard!;
        }
        catch { return null!; }
    }

    private static void InjectAllCards(Transform contentGrid, Transform templateCard)
    {
        try
        {
            foreach (var entry in _employees)
            {
                string cardName = "CustomEmployee_" + entry.EmployeeId;
                var existing = contentGrid.Find(cardName);
                if (existing != null) { UpdateCard(existing, entry); continue; }
                try { CreateCard(contentGrid, templateCard, entry, cardName); }
                catch (Exception ex) { CrashLog.LogException($"CreateCard({entry.EmployeeId})", ex); }
            }
        }
        catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
    }

    private static Transform? FindLowestCommonAncestor(Transform a, Transform b)
    {
        if (a == null || b == null) return null;
        var ancestors = new HashSet<Transform>();
        for (var t = a.parent; t != null; t = t.parent)
            ancestors.Add(t);
        for (var t = b.parent; t != null; t = t.parent)
            if (ancestors.Contains(t)) return t;
        return null;
    }
}
