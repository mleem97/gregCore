using System;
using System.Linq;
using Il2Cpp;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;
using greg.Logging;

namespace greg.CommonShop
{
    /// <summary>
    /// Shop layout helpers: real category/sub-category containers plus grid
    /// height repair. Best-effort everywhere — falls back to the vanilla
    /// parent so injection never breaks the shop.
    /// </summary>
    internal static class ShopUI
    {
        private static readonly GregModLogger _log = new GregModLogger("CommonShop");

        internal static void FixGridHeight(Transform gridContainer)
        {
            try
            {
                var grid = gridContainer.GetComponent<GridLayoutGroup>();
                var rt = gridContainer.GetComponent<RectTransform>();
                if (rt == null) return;
                var le = gridContainer.GetComponent<LayoutElement>();
                if (le == null) le = gridContainer.gameObject.AddComponent<LayoutElement>();

                if (grid != null)
                {
                    int leftPad = grid.padding.left;
                    int rightPad = grid.padding.right;
                    grid.padding = new RectOffset { left = leftPad, right = rightPad, top = 10, bottom = 20 };
                    if (grid.spacing.y > 50f) grid.spacing = new Vector2(grid.spacing.x, 15f);
                }

                int activeCards = 0;
                for (int i = 0; i < gridContainer.childCount; i++)
                {
                    try { if (gridContainer.GetChild(i).gameObject.activeSelf) activeCards++; } catch { }
                }

                if (activeCards == 0)
                {
                    rt.sizeDelta = new Vector2(rt.sizeDelta.x, 0);
                    le.minHeight = 0;
                    le.preferredHeight = 0;
                    le.flexibleHeight = 0;
                    return;
                }

                int cols;
                if (grid != null && grid.constraint == GridLayoutGroup.Constraint.FixedColumnCount)
                {
                    cols = Math.Max(1, grid.constraintCount);
                }
                else if (grid != null)
                {
                    float usableWidth = rt.rect.width - grid.padding.left - grid.padding.right;
                    float cell = grid.cellSize.x + grid.spacing.x;
                    cols = cell > 0f ? Math.Max(1, Mathf.FloorToInt((usableWidth + grid.spacing.x) / cell)) : 4;
                }
                else cols = 4;

                int rows = Mathf.CeilToInt((float)activeCards / cols);
                float height = 10f + 20f + (rows * (grid != null ? grid.cellSize.y : 150f))
                    + (Math.Max(0, rows - 1) * (grid != null ? grid.spacing.y : 15f));

                rt.sizeDelta = new Vector2(rt.sizeDelta.x, height);
                le.minHeight = height;
                le.preferredHeight = height;
                le.flexibleHeight = 0;
            }
            catch (Exception ex)
            {
                _log.Warn($"FixGridHeight failed: {ex.GetBaseException().Message}");
            }
        }

        internal static Transform EnsureCategoryContainer(ComputerShop shop, string mainCat, string subCat)
        {
            try
            {
                Transform parent = shop.shopItemParent.transform;
                if (parent == null) return null!;

                // Prevent Unity from stretching children (massive gaps).
                try
                {
                    var vlg = parent.GetComponent<VerticalLayoutGroup>();
                    if (vlg != null) vlg.childForceExpandHeight = false;
                }
                catch { }

                string gridName = string.IsNullOrEmpty(subCat) ? $"Grid_{mainCat}" : $"Grid_{mainCat}_{subCat}";
                Transform existing = parent.Find(gridName);
                if (existing != null)
                {
                    try { existing.gameObject.SetActive(true); } catch { }
                    return existing;
                }

                Transform hlTemplate = parent.Find("HL Mods");
                bool safeToInsertBefore = hlTemplate != null;
                if (hlTemplate == null)
                {
                    try { hlTemplate = parent.GetComponentsInChildren<GridLayoutGroup>(true).FirstOrDefault()?.transform; } catch { }
                }
                if (hlTemplate == null) return parent;

                int insertIdx = -1;
                if (safeToInsertBefore)
                {
                    try
                    {
                        int hlIdx = hlTemplate.GetSiblingIndex();
                        insertIdx = hlIdx > 0 ? hlIdx - 1 : 0;
                    }
                    catch { insertIdx = -1; }
                }

                Il2CppTMPro.TextMeshProUGUI textTemplate = null;
                float defaultFontSize = 32f;
                try
                {
                    foreach (var textItem in parent.GetComponentsInChildren<Il2CppTMPro.TextMeshProUGUI>(true))
                    {
                        if (textItem != null && textItem.transform.parent == parent)
                        {
                            textTemplate = textItem;
                            defaultFontSize = textItem.fontSize;
                            break;
                        }
                    }
                }
                catch { }

                string mainLabelName = $"Label_Main_{mainCat}";
                if (parent.Find(mainLabelName) == null && textTemplate != null)
                {
                    try
                    {
                        GameObject mainLabel = Object.Instantiate(textTemplate.gameObject, parent);
                        mainLabel.name = mainLabelName;
                        mainLabel.SetActive(true);
                        for (int i = mainLabel.transform.childCount - 1; i >= 0; i--)
                            Object.DestroyImmediate(mainLabel.transform.GetChild(i).gameObject);
                        var le = mainLabel.GetComponent<LayoutElement>();
                        if (le != null) Object.DestroyImmediate(le);
                        var tmp = mainLabel.GetComponent<Il2CppTMPro.TextMeshProUGUI>();
                        if (tmp != null)
                        {
                            tmp.text = mainCat;
                            tmp.fontSize = defaultFontSize;
                            tmp.margin = new Vector4(0, 0, 0, 0);
                        }
                        if (insertIdx != -1) mainLabel.transform.SetSiblingIndex(insertIdx++);
                        else mainLabel.transform.SetAsLastSibling();
                    }
                    catch { }
                }

                string subLabelName = $"Label_Sub_{mainCat}_{subCat}";
                if (!string.IsNullOrEmpty(subCat) && parent.Find(subLabelName) == null && textTemplate != null)
                {
                    try
                    {
                        GameObject subLabel = Object.Instantiate(textTemplate.gameObject, parent);
                        subLabel.name = subLabelName;
                        subLabel.SetActive(true);
                        for (int i = subLabel.transform.childCount - 1; i >= 0; i--)
                            Object.DestroyImmediate(subLabel.transform.GetChild(i).gameObject);
                        var le = subLabel.GetComponent<LayoutElement>();
                        if (le != null) Object.DestroyImmediate(le);
                        var tmp = subLabel.GetComponent<Il2CppTMPro.TextMeshProUGUI>();
                        if (tmp != null)
                        {
                            tmp.text = subCat;
                            tmp.fontSize = defaultFontSize * 0.75f;
                            tmp.margin = new Vector4(0, 0, 0, 0);
                        }
                        if (insertIdx != -1) subLabel.transform.SetSiblingIndex(insertIdx++);
                        else subLabel.transform.SetAsLastSibling();
                    }
                    catch { }
                }

                GameObject newGrid;
                try
                {
                    newGrid = Object.Instantiate(hlTemplate.gameObject, parent);
                }
                catch { return parent; }
                newGrid.name = gridName;
                newGrid.SetActive(true);

                try
                {
                    var gridLe = newGrid.GetComponent<LayoutElement>();
                    if (gridLe == null) gridLe = newGrid.AddComponent<LayoutElement>();
                    gridLe.flexibleHeight = 0;
                }
                catch { }

                try
                {
                    if (insertIdx != -1) newGrid.transform.SetSiblingIndex(insertIdx);
                    else newGrid.transform.SetAsLastSibling();
                }
                catch { }

                for (int i = newGrid.transform.childCount - 1; i >= 0; i--)
                {
                    try { Object.DestroyImmediate(newGrid.transform.GetChild(i).gameObject); } catch { }
                }

                return newGrid.transform;
            }
            catch (Exception ex)
            {
                _log.Warn($"EnsureCategoryContainer failed: {ex.GetBaseException().Message}");
                try { return shop.shopItemParent.transform; } catch { return null!; }
            }
        }

        internal static void UpdateLayoutHeight(ComputerShop shop)
        {
            try
            {
                var sr = shop.shopItemParent.GetComponentInParent<ScrollRect>();
                if (sr == null || sr.content == null) return;

                LayoutRebuilder.ForceRebuildLayoutImmediate(sr.content);

                float totalHeight = 0f;
                var layout = shop.shopItemParent.GetComponent<VerticalLayoutGroup>();
                int activeChildren = 0;

                for (int i = 0; i < shop.shopItemParent.transform.childCount; i++)
                {
                    var child = shop.shopItemParent.transform.GetChild(i);
                    if (child == null || !child.gameObject.activeInHierarchy) continue;
                    var le = child.GetComponent<LayoutElement>();
                    var rt = child.GetComponent<RectTransform>();
                    float childHeight = rt != null ? rt.rect.height : 0f;
                    if (le != null && le.preferredHeight > 0) childHeight = le.preferredHeight;
                    if (childHeight > 0)
                    {
                        totalHeight += childHeight;
                        activeChildren++;
                    }
                }

                if (layout != null)
                {
                    totalHeight += layout.padding.top + layout.padding.bottom;
                    if (activeChildren > 1) totalHeight += (activeChildren - 1) * layout.spacing;
                }

                totalHeight += 50f;
                sr.content.sizeDelta = new Vector2(sr.content.sizeDelta.x, totalHeight);
                Canvas.ForceUpdateCanvases();
            }
            catch (Exception ex)
            {
                _log.Warn($"UpdateLayoutHeight failed: {ex.GetBaseException().Message}");
            }
        }
    }
}
