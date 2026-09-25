using System;
using Il2Cpp;
using UnityEngine;
using UnityEngine.UI;

namespace greg.CommonShop
{
    internal static partial class ShopUI
    {
        internal static void FixGridHeight(Transform gridContainer)
        {
            try
            {
                var grid = gridContainer.GetComponent<GridLayoutGroup>();
                var rt = gridContainer.GetComponent<RectTransform>();
                if (rt == null) return;
                EnsureLayoutElement(gridContainer);
                NormalizeGrid(grid);
                int activeCards = CountActiveCards(gridContainer);
                if (activeCards == 0)
                {
                    ApplyEmptyHeight(gridContainer, rt);
                    return;
                }
                ApplyGridHeight(gridContainer, grid, rt, activeCards);
            }
            catch (Exception ex)
            {
                _log.Warn($"FixGridHeight failed: {ex.GetBaseException().Message}");
            }
        }

        private static void EnsureLayoutElement(Transform gridContainer)
        {
            try
            {
                var le = gridContainer.GetComponent<LayoutElement>();
                if (le == null) le = gridContainer.gameObject.AddComponent<LayoutElement>();
            }
            catch { }
        }

        private static void NormalizeGrid(GridLayoutGroup grid)
        {
            try
            {
                if (grid == null) return;
                int leftPad = grid.padding.left;
                int rightPad = grid.padding.right;
                grid.padding = new RectOffset { left = leftPad, right = rightPad, top = 10, bottom = 20 };
                if (grid.spacing.y > 50f) grid.spacing = new Vector2(grid.spacing.x, 15f);
            }
            catch { }
        }

        private static int CountActiveCards(Transform gridContainer)
        {
            int activeCards = 0;
            try
            {
                for (int i = 0; i < gridContainer.childCount; i++)
                {
                    try { if (gridContainer.GetChild(i).gameObject.activeSelf) activeCards++; } catch { }
                }
            }
            catch { }
            return activeCards;
        }

        private static void ApplyEmptyHeight(Transform gridContainer, RectTransform rt)
        {
            try
            {
                var le = gridContainer.GetComponent<LayoutElement>();
                rt.sizeDelta = new Vector2(rt.sizeDelta.x, 0);
                if (le == null) return;
                le.minHeight = 0;
                le.preferredHeight = 0;
                le.flexibleHeight = 0;
            }
            catch { }
        }

        private static void ApplyGridHeight(Transform gridContainer, GridLayoutGroup grid, RectTransform rt, int activeCards)
        {
            try
            {
                int cols = ComputeColumnCount(grid, rt);
                int rows = Mathf.CeilToInt((float)activeCards / cols);
                float height = ComputeGridHeight(grid, rows);
                rt.sizeDelta = new Vector2(rt.sizeDelta.x, height);
                var le = gridContainer.GetComponent<LayoutElement>();
                if (le == null) return;
                le.minHeight = height;
                le.preferredHeight = height;
                le.flexibleHeight = 0;
            }
            catch { }
        }

        private static int ComputeColumnCount(GridLayoutGroup grid, RectTransform rt)
        {
            try
            {
                if (grid != null && grid.constraint == GridLayoutGroup.Constraint.FixedColumnCount)
                    return Math.Max(1, grid.constraintCount);
                if (grid != null)
                {
                    float usableWidth = rt.rect.width - grid.padding.left - grid.padding.right;
                    float cell = grid.cellSize.x + grid.spacing.x;
                    return cell > 0f ? Math.Max(1, Mathf.FloorToInt((usableWidth + grid.spacing.x) / cell)) : 4;
                }
            }
            catch { }
            return 4;
        }

        private static float ComputeGridHeight(GridLayoutGroup grid, int rows)
        {
            try
            {
                float cellH = grid != null ? grid.cellSize.y : 150f;
                float spacingY = grid != null ? grid.spacing.y : 15f;
                return 10f + 20f + (rows * cellH) + (Math.Max(0, rows - 1) * spacingY);
            }
            catch { return 0f; }
        }

        internal static void UpdateLayoutHeight(ComputerShop shop)
        {
            try
            {
                var sr = shop.shopItemParent.GetComponentInParent<ScrollRect>();
                if (sr == null || sr.content == null) return;
                LayoutRebuilder.ForceRebuildLayoutImmediate(sr.content);
                float totalHeight = SumChildHeights(shop);
                totalHeight += 50f;
                sr.content.sizeDelta = new Vector2(sr.content.sizeDelta.x, totalHeight);
                Canvas.ForceUpdateCanvases();
            }
            catch (Exception ex)
            {
                _log.Warn($"UpdateLayoutHeight failed: {ex.GetBaseException().Message}");
            }
        }

        private static float SumChildHeights(ComputerShop shop)
        {
            float totalHeight = 0f;
            try
            {
                var layout = shop.shopItemParent.GetComponent<VerticalLayoutGroup>();
                int activeChildren = 0;
                for (int i = 0; i < shop.shopItemParent.transform.childCount; i++)
                {
                    float h = GetChildHeight(shop, i);
                    if (h <= 0) continue;
                    totalHeight += h;
                    activeChildren++;
                }
                if (layout != null)
                {
                    totalHeight += layout.padding.top + layout.padding.bottom;
                    if (activeChildren > 1) totalHeight += (activeChildren - 1) * layout.spacing;
                }
            }
            catch { }
            return totalHeight;
        }

        private static float GetChildHeight(ComputerShop shop, int index)
        {
            try
            {
                var child = shop.shopItemParent.transform.GetChild(index);
                if (child == null || !child.gameObject.activeInHierarchy) return 0f;
                var le = child.GetComponent<LayoutElement>();
                var rt = child.GetComponent<RectTransform>();
                float childHeight = rt != null ? rt.rect.height : 0f;
                if (le != null && le.preferredHeight > 0) childHeight = le.preferredHeight;
                return childHeight;
            }
            catch { return 0f; }
        }
    }
}
