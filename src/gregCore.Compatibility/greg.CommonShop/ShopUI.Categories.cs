using System;
using System.Linq;
using Il2Cpp;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace greg.CommonShop
{
    internal static partial class ShopUI
    {
        internal static Transform EnsureCategoryContainer(ComputerShop shop, string mainCat, string subCat)
        {
            try
            {
                Transform parent = shop.shopItemParent.transform;
                if (parent == null) return null!;
                DisableChildExpansion(parent);
                string gridName = BuildGridName(mainCat, subCat);
                var existing = FindExistingGrid(parent, gridName);
                if (existing != null) return existing;
                var template = ResolveGridTemplate(parent);
                if (template == null) return parent;
                int insertIdx = ResolveInsertIndex(parent, template);
                var textTemplate = FindHeaderTextTemplate(parent, out float fontSize);
                EnsureMainLabel(parent, mainCat, textTemplate, fontSize, ref insertIdx);
                EnsureSubLabel(parent, mainCat, subCat, textTemplate, fontSize, ref insertIdx);
                return CreateGrid(parent, template, gridName, insertIdx);
            }
            catch (Exception ex)
            {
                _log.Warn($"EnsureCategoryContainer failed: {ex.GetBaseException().Message}");
                try { return shop.shopItemParent.transform; } catch { return null!; }
            }
        }

        private static void DisableChildExpansion(Transform parent)
        {
            try
            {
                var vlg = parent.GetComponent<VerticalLayoutGroup>();
                if (vlg != null) vlg.childForceExpandHeight = false;
            }
            catch { }
        }

        private static string BuildGridName(string mainCat, string subCat)
        {
            try { return string.IsNullOrEmpty(subCat) ? $"Grid_{mainCat}" : $"Grid_{mainCat}_{subCat}"; }
            catch { return $"Grid_{mainCat}"; }
        }

        private static Transform FindExistingGrid(Transform parent, string gridName)
        {
            try
            {
                Transform existing = parent.Find(gridName);
                if (existing == null) return null!;
                try { existing.gameObject.SetActive(true); } catch { }
                return existing;
            }
            catch { return null!; }
        }

        private static Transform ResolveGridTemplate(Transform parent)
        {
            try
            {
                Transform hlTemplate = parent.Find("HL Mods");
                if (hlTemplate != null) return hlTemplate;
                try { return parent.GetComponentsInChildren<GridLayoutGroup>(true).FirstOrDefault()?.transform!; } catch { }
            }
            catch { }
            return null!;
        }

        private static int ResolveInsertIndex(Transform parent, Transform hlTemplate)
        {
            try
            {
                bool isLegacySlot = hlTemplate.name == "HL Mods";
                if (!isLegacySlot) return -1;
                try
                {
                    int hlIdx = hlTemplate.GetSiblingIndex();
                    return hlIdx > 0 ? hlIdx - 1 : 0;
                }
                catch { return -1; }
            }
            catch { return -1; }
        }

        private static Il2CppTMPro.TextMeshProUGUI FindHeaderTextTemplate(Transform parent, out float fontSize)
        {
            fontSize = 32f;
            try
            {
                foreach (var textItem in parent.GetComponentsInChildren<Il2CppTMPro.TextMeshProUGUI>(true))
                {
                    if (textItem != null && textItem.transform.parent == parent)
                    {
                        fontSize = textItem.fontSize;
                        return textItem;
                    }
                }
            }
            catch { }
            return null!;
        }

        private static void EnsureMainLabel(Transform parent, string mainCat, Il2CppTMPro.TextMeshProUGUI textTemplate, float fontSize, ref int insertIdx)
        {
            try
            {
                string mainLabelName = $"Label_Main_{mainCat}";
                if (parent.Find(mainLabelName) != null || textTemplate == null) return;
                var mainLabel = CloneLabel(parent, textTemplate.gameObject, mainLabelName);
                if (mainLabel == null) return;
                var tmp = mainLabel.GetComponent<Il2CppTMPro.TextMeshProUGUI>();
                if (tmp != null)
                {
                    tmp.text = mainCat;
                    tmp.fontSize = fontSize;
                    tmp.margin = new Vector4(0, 0, 0, 0);
                }
                PlaceLabel(parent, mainLabel, ref insertIdx);
            }
            catch { }
        }

        private static void EnsureSubLabel(Transform parent, string mainCat, string subCat, Il2CppTMPro.TextMeshProUGUI textTemplate, float fontSize, ref int insertIdx)
        {
            try
            {
                if (string.IsNullOrEmpty(subCat) || textTemplate == null) return;
                string subLabelName = $"Label_Sub_{mainCat}_{subCat}";
                if (parent.Find(subLabelName) != null) return;
                var subLabel = CloneLabel(parent, textTemplate.gameObject, subLabelName);
                if (subLabel == null) return;
                var tmp = subLabel.GetComponent<Il2CppTMPro.TextMeshProUGUI>();
                if (tmp != null)
                {
                    tmp.text = subCat;
                    tmp.fontSize = fontSize * 0.75f;
                    tmp.margin = new Vector4(0, 0, 0, 0);
                }
                PlaceLabel(parent, subLabel, ref insertIdx);
            }
            catch { }
        }

        private static GameObject CloneLabel(Transform parent, GameObject template, string labelName)
        {
            try
            {
                GameObject label = Object.Instantiate(template, parent);
                label.name = labelName;
                label.SetActive(true);
                for (int i = label.transform.childCount - 1; i >= 0; i--)
                    Object.DestroyImmediate(label.transform.GetChild(i).gameObject);
                var le = label.GetComponent<LayoutElement>();
                if (le != null) Object.DestroyImmediate(le);
                return label;
            }
            catch { return null!; }
        }

        private static void PlaceLabel(Transform parent, GameObject label, ref int insertIdx)
        {
            try
            {
                if (insertIdx != -1) label.transform.SetSiblingIndex(insertIdx++);
                else label.transform.SetAsLastSibling();
            }
            catch { }
        }

        private static Transform CreateGrid(Transform parent, Transform template, string gridName, int insertIdx)
        {
            GameObject newGrid;
            try { newGrid = Object.Instantiate(template.gameObject, parent); }
            catch { return parent; }
            try
            {
                newGrid.name = gridName;
                newGrid.SetActive(true);
                EnsureGridLayout(newGrid);
                PlaceGrid(newGrid, insertIdx);
                ClearGridChildren(newGrid);
                return newGrid.transform;
            }
            catch { return parent; }
        }

        private static void EnsureGridLayout(GameObject newGrid)
        {
            try
            {
                var gridLe = newGrid.GetComponent<LayoutElement>();
                if (gridLe == null) gridLe = newGrid.AddComponent<LayoutElement>();
                gridLe.flexibleHeight = 0;
            }
            catch { }
        }

        private static void PlaceGrid(GameObject newGrid, int insertIdx)
        {
            try
            {
                if (insertIdx != -1) newGrid.transform.SetSiblingIndex(insertIdx);
                else newGrid.transform.SetAsLastSibling();
            }
            catch { }
        }

        private static void ClearGridChildren(GameObject newGrid)
        {
            try
            {
                for (int i = newGrid.transform.childCount - 1; i >= 0; i--)
                {
                    try { Object.DestroyImmediate(newGrid.transform.GetChild(i).gameObject); } catch { }
                }
            }
            catch { }
        }
    }
}
