using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using greg.Mods.ResetSwitch.Config;
using greg.Mods.ResetSwitch.Core;
using Il2CppTMPro;
using UnityEngine;
using UnityEngine.UI;

namespace greg.Mods.ResetSwitch.UI
{
    public enum SwitchFilterMode
    {
        All,
        NoFlowOnly,
        DegradedOnly
    }

    public enum UIViewMode
    {
        Table,
        Metrics,
        RackPlan
    }

    public class FixedTableUI
    {
        private readonly GameObject _root;
        private readonly RectTransform _contentRoot;
        private readonly TextMeshProUGUI _summaryLabel;
        private readonly GameObject _mountedBanner;
        private readonly TextMeshProUGUI _mountedBannerText;
        private readonly TextMeshProUGUI _toastLabel;
        private readonly TextMeshProUGUI _saveHealthLabel;
        private TextMeshProUGUI _filterLabel;
        private TextMeshProUGUI _viewModeLabel;
        private bool _isFocused;
        private bool _scanInProgress;
        private List<SwitchInfo> _items = new();
        private List<SwitchInfo> _renderedItems = new();
        private HashSet<string> _mountedCandidates = new();
        private SaveAnalysisReport _saveReport;
        private SwitchFilterMode _filterMode = SwitchFilterMode.NoFlowOnly;
        private UIViewMode _viewMode = UIViewMode.Table;

        public FixedTableUI(Transform parent)
        {
            _root = new GameObject("greg.Mods.ResetSwitch_FixedTableUI");
            _root.transform.SetParent(parent, false);

            var rootRect = _root.AddComponent<RectTransform>();
            rootRect.anchorMin = new Vector2(0.01f, 0.03f);
            rootRect.anchorMax = new Vector2(0.99f, 0.97f);
            rootRect.pivot = new Vector2(0.5f, 0.5f);
            rootRect.offsetMin = Vector2.zero;
            rootRect.offsetMax = Vector2.zero;

            var bg = _root.AddComponent<Image>();
            bg.color = new Color(0.03f, 0.08f, 0.08f, 0.92f);

            CreateHeader(_root.transform);
            CreateToolbar(_root.transform);

            var scrollGo = new GameObject("ScrollArea");
            scrollGo.transform.SetParent(_root.transform, false);
            var scrollRectTransform = scrollGo.AddComponent<RectTransform>();
            scrollRectTransform.anchorMin = new Vector2(0.02f, 0.13f);
            scrollRectTransform.anchorMax = new Vector2(0.98f, 0.88f);
            scrollRectTransform.offsetMin = Vector2.zero;
            scrollRectTransform.offsetMax = Vector2.zero;

            var scrollImage = scrollGo.AddComponent<Image>();
            scrollImage.color = new Color(0f, 0f, 0f, 0.25f);

            var scrollRect = scrollGo.AddComponent<ScrollRect>();
            scrollGo.AddComponent<RectMask2D>();

            var contentGo = new GameObject("Content");
            contentGo.transform.SetParent(scrollGo.transform, false);
            _contentRoot = contentGo.AddComponent<RectTransform>();
            _contentRoot.anchorMin = new Vector2(0, 1);
            _contentRoot.anchorMax = new Vector2(1, 1);
            _contentRoot.pivot = new Vector2(0.5f, 1);
            _contentRoot.anchoredPosition = Vector2.zero;

            var vertical = contentGo.AddComponent<VerticalLayoutGroup>();
            vertical.childControlWidth = true;
            vertical.childControlHeight = true;
            vertical.childForceExpandWidth = true;
            vertical.childForceExpandHeight = false;
            vertical.spacing = 4;
            vertical.padding = new RectOffset { left = 6, right = 6, top = 6, bottom = 6 };

            contentGo.AddComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            scrollRect.content = _contentRoot;
            scrollRect.viewport = scrollRectTransform;
            scrollRect.horizontal = false;

            _mountedBanner = new GameObject("MountedBanner");
            _mountedBanner.transform.SetParent(_root.transform, false);
            var bannerRt = _mountedBanner.AddComponent<RectTransform>();
            bannerRt.anchorMin = new Vector2(0.02f, 0.88f);
            bannerRt.anchorMax = new Vector2(0.98f, 0.92f);
            bannerRt.offsetMin = Vector2.zero;
            bannerRt.offsetMax = Vector2.zero;
            var bannerImage = _mountedBanner.AddComponent<Image>();
            bannerImage.color = new Color(0.15f, 0.10f, 0.02f, 0.95f);

            var bannerLayout = _mountedBanner.AddComponent<HorizontalLayoutGroup>();
            bannerLayout.spacing = 10;
            bannerLayout.padding = new RectOffset { left = 8, right = 8, top = 4, bottom = 4 };
            bannerLayout.childControlWidth = false;
            bannerLayout.childControlHeight = true;
            bannerLayout.childForceExpandWidth = false;
            bannerLayout.childForceExpandHeight = true;

            _mountedBannerText = CreateText(_mountedBanner.transform, "");
            _mountedBannerText.alignment = TextAlignmentOptions.Left;
            _mountedBannerText.rectTransform.sizeDelta = new Vector2(760, 28);
            _mountedBannerText.color = new Color(0.94f, 0.65f, 0.0f, 1f);

            CreateButton(_mountedBanner.transform, "REPAIR ALL UNMOUNTED", 280, RepairAllUnmounted);
            _mountedBanner.SetActive(false);

            var footer = new GameObject("Footer");
            footer.transform.SetParent(_root.transform, false);
            var footerRt = footer.AddComponent<RectTransform>();
            footerRt.anchorMin = new Vector2(0.02f, 0.02f);
            footerRt.anchorMax = new Vector2(0.98f, 0.12f);
            footerRt.offsetMin = Vector2.zero;
            footerRt.offsetMax = Vector2.zero;

            var footerLayout = footer.AddComponent<VerticalLayoutGroup>();
            footerLayout.spacing = 6;
            footerLayout.padding = new RectOffset { left = 0, right = 0, top = 0, bottom = 0 };
            footerLayout.childControlHeight = false;
            footerLayout.childControlWidth = true;
            footerLayout.childForceExpandHeight = false;
            footerLayout.childForceExpandWidth = true;

            var infoRow = new GameObject("InfoRow");
            infoRow.transform.SetParent(footer.transform, false);
            var infoRowRt = infoRow.AddComponent<RectTransform>();
            infoRowRt.sizeDelta = new Vector2(0, 34);
            var infoRowElement = infoRow.AddComponent<LayoutElement>();
            infoRowElement.minHeight = 34;
            infoRowElement.preferredHeight = 34;
            infoRowElement.flexibleHeight = 0;

            var infoRowLayout = infoRow.AddComponent<HorizontalLayoutGroup>();
            infoRowLayout.spacing = 8;
            infoRowLayout.childControlHeight = true;
            infoRowLayout.childControlWidth = false;
            infoRowLayout.childForceExpandHeight = true;
            infoRowLayout.childForceExpandWidth = false;

            _summaryLabel = CreateText(infoRow.transform, "Selected: 0");
            _summaryLabel.alignment = TextAlignmentOptions.Left;
            _summaryLabel.rectTransform.sizeDelta = new Vector2(360, 34);

            _toastLabel = CreateText(infoRow.transform, "");
            _toastLabel.alignment = TextAlignmentOptions.Left;
            _toastLabel.rectTransform.sizeDelta = new Vector2(340, 34);
            _toastLabel.color = new Color(0.38f, 0.96f, 0.85f, 1f);

            _saveHealthLabel = CreateText(infoRow.transform, "");
            _saveHealthLabel.alignment = TextAlignmentOptions.Left;
            _saveHealthLabel.rectTransform.sizeDelta = new Vector2(420, 34);
            _saveHealthLabel.color = new Color(0.94f, 0.65f, 0.0f, 1f);

            var actionRow = new GameObject("ActionRow");
            actionRow.transform.SetParent(footer.transform, false);
            var actionRowRt = actionRow.AddComponent<RectTransform>();
            actionRowRt.sizeDelta = new Vector2(0, 34);
            var actionRowElement = actionRow.AddComponent<LayoutElement>();
            actionRowElement.minHeight = 34;
            actionRowElement.preferredHeight = 34;
            actionRowElement.flexibleHeight = 0;

            var actionRowLayout = actionRow.AddComponent<HorizontalLayoutGroup>();
            actionRowLayout.spacing = 8;
            actionRowLayout.childControlHeight = true;
            actionRowLayout.childControlWidth = false;
            actionRowLayout.childForceExpandHeight = true;
            actionRowLayout.childForceExpandWidth = false;

            CreateButton(actionRow.transform, "SELECT NO-FLOW", 220, SelectAllNoFlow);
            CreateButton(actionRow.transform, "REPAIR SELECTED", 220, RepairSelectedUnmounted);
            CreateButton(actionRow.transform, "RESET SELECTED", 220, ResetSelectedWithConfirmation);
            CreateButton(actionRow.transform, "CANCEL", 160, () => Toggle(false));

            _root.SetActive(false);
        }

        public void Toggle(bool show)
        {
            _root.SetActive(show);
            if (show)
            {
                MelonLoader.MelonLogger.Msg("[FixedTableUI] UI opened via F8.");
                if (ModConfig.AutoScanOnOpen.Value)
                {
                    RefreshList();
                }
                SetFocus(true);
            }
            else
            {
                SetFocus(false);
            }
        }

        public void Update()
        {
            if (!IsOpen())
            {
                return;
            }

            var keyboard = UnityEngine.InputSystem.Keyboard.current;
            if (keyboard == null)
            {
                return;
            }

            if (keyboard.leftAltKey.wasPressedThisFrame)
            {
                SetFocus(!_isFocused);
            }
        }

        public bool IsOpen()
        {
            return _root != null && _root.activeSelf;
        }

        public void PopulateTable(List<SwitchInfo> switches)
        {
            _items = switches ?? new List<SwitchInfo>();

            for (var i = _contentRoot.childCount - 1; i >= 0; i--)
            {
                UnityEngine.Object.Destroy(_contentRoot.GetChild(i).gameObject);
            }

            CreateHeaderRow(_contentRoot);

            _renderedItems = FilterItems();
            if (_renderedItems.Count == 0)
            {
                CreateEmptyStateRow(_contentRoot, _items.Count == 0 ? "No switches found. Press SCAN." : "No rows for current filter.");
            }

            for (var index = 0; index < _renderedItems.Count; index++)
            {
                CreateDataRow(_contentRoot, _renderedItems[index], index);
            }

            UpdateSummary();
            MelonLoader.MelonLogger.Msg($"[FixedTableUI] Rendered {_renderedItems.Count} switch rows.");
        }

        private void RefreshScan()
        {
            RefreshList();
        }

        private void RefreshList()
        {
            if (_scanInProgress)
            {
                MelonLoader.MelonLogger.Warning("[Coroutine] RefreshList skipped: scan already in progress.");
                return;
            }

            MelonLoader.MelonLogger.Msg("[Coroutine] RefreshListAsync started.");
            MelonLoader.MelonCoroutines.Start(RefreshListAsync());
        }

        private IEnumerator RefreshListAsync()
        {
            _scanInProgress = true;

            for (var i = _contentRoot.childCount - 1; i >= 0; i--)
            {
                UnityEngine.Object.Destroy(_contentRoot.GetChild(i).gameObject);
            }

            var loading = CreateText(_contentRoot, "⏳ Scanning network devices...", 16);
            loading.color = new Color(0.94f, 0.65f, 0.0f, 1f);

            yield return null;

            if (_saveReport == null)
            {
                _saveReport = SaveAnalysisService.GetCachedOrAnalyze();
            }

            var scanned = SwitchScanner.ScanAll();

            if (loading != null)
            {
                UnityEngine.Object.Destroy(loading.gameObject);
            }

            _items = scanned;
            _mountedCandidates = BuildMountedCandidates(scanned);
            UpdateMountedBanner();

            if (_viewMode == UIViewMode.Metrics)
            {
                ShowMetricsView();
            }
            else
            {
                CreateHeaderRow(_contentRoot);
                _renderedItems = FilterItems();
                if (_renderedItems.Count == 0)
                {
                    CreateEmptyStateRow(_contentRoot, _items.Count == 0 ? "No switches found. Press SCAN." : "No rows for current filter.");
                }

                var rendered = 0;
                for (var index = 0; index < _renderedItems.Count; index++)
                {
                    CreateDataRow(_contentRoot, _renderedItems[index], index);
                    rendered++;
                    if (rendered % 25 == 0)
                    {
                        MelonLoader.MelonLogger.Msg($"[Coroutine] Built {rendered}/{_renderedItems.Count} rows, yielding frame.");
                        yield return null;
                    }
                }
            }

            UpdateSummary();
            UpdateSaveHealthHint();
            var visibleCount = _viewMode == UIViewMode.Table ? _renderedItems.Count : _items.Count;
            MelonLoader.MelonLogger.Msg($"[UI] Scan-Ende: total={_items.Count}, visible={visibleCount}, filter={_filterMode}, view={_viewMode}");
            _scanInProgress = false;
        }

        private void UpdateMountedBanner()
        {
            var mountedCount = _mountedCandidates.Count;

            if (mountedCount <= 0)
            {
                _mountedBanner.SetActive(false);
                return;
            }

            _mountedBannerText.text = $"⚠ {mountedCount} switches likely NOT MOUNTED (runtime + save heuristics).";
            _mountedBanner.SetActive(true);
        }

        private HashSet<string> BuildMountedCandidates(List<SwitchInfo> scanned)
        {
            var result = new HashSet<string>();
            if (scanned != null)
            {
                foreach (var item in scanned)
                {
                    if (item?.Instance == null || string.IsNullOrWhiteSpace(item.SwitchId))
                    {
                        continue;
                    }

                    if (!item.Instance.isOn && !item.Instance.isBroken)
                    {
                        result.Add(item.SwitchId);
                    }
                }
            }

            if (_saveReport?.Entries != null)
            {
                foreach (var entry in _saveReport.Entries)
                {
                    if (entry == null || string.IsNullOrWhiteSpace(entry.SwitchId))
                    {
                        continue;
                    }

                    if (entry.CorruptionReason == "NEVER_INITIALIZED" ||
                        entry.CorruptionReason == "RUNTIME_MISMATCH" ||
                        entry.CorruptionReason == "STATE_DIFFERENCE")
                    {
                        result.Add(entry.SwitchId);
                    }
                }
            }

            return result;
        }

        private void RepairAllUnmounted()
        {
            if (_saveReport != null && _saveReport.SaveCorrupt > 0)
            {
                ShowSaveCorruptRepairDialog();
                return;
            }

            ExecuteRepairAllUnmounted();
        }

        private void ExecuteRepairAllUnmounted()
        {
            var reports = MountedStateRepairer.RepairAll();
            var fixedCount = reports.Count(r => r.Result == MountedStateRepairer.RepairResult.Fixed);
            ShowToast($"✓ Repaired {fixedCount} switches. Refreshing...", new Color(0.38f, 0.96f, 0.85f, 1f));
            RefreshScan();
        }

        private void ShowSaveCorruptRepairDialog()
        {
            var dialog = new GameObject("SaveCorruptWarningDialog");
            dialog.transform.SetParent(_root.transform, false);

            var rt = dialog.AddComponent<RectTransform>();
            rt.anchorMin = new Vector2(0.22f, 0.25f);
            rt.anchorMax = new Vector2(0.78f, 0.75f);
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;

            var image = dialog.AddComponent<Image>();
            image.color = new Color(0.10f, 0.05f, 0.05f, 0.98f);

            var outline = dialog.AddComponent<Outline>();
            outline.effectColor = new Color(0.93f, 0.26f, 0.27f, 1f);
            outline.effectDistance = new Vector2(2f, -2f);

            var layout = dialog.AddComponent<VerticalLayoutGroup>();
            layout.spacing = 10;
            layout.padding = new RectOffset { left = 12, right = 12, top = 12, bottom = 12 };
            layout.childControlWidth = true;
            layout.childControlHeight = false;
            layout.childForceExpandHeight = false;

            CreateText(dialog.transform, "⚠ WICHTIG: Diese Switches sind in deiner Save-Datei als OFF gespeichert.", 18).color = new Color(0.93f, 0.26f, 0.27f, 1f);
            CreateText(dialog.transform, "Der Fix ist nur temporär bis du das Spiel speicherst.", 16);
            CreateText(dialog.transform, "→ Reparieren → Spiel speichern → Fertig", 16);

            CreateButton(dialog.transform, "UNDERSTOOD — REPAIR ALL NOW", 320, () =>
            {
                UnityEngine.Object.Destroy(dialog);
                ExecuteRepairAllUnmounted();
            });
        }

        private void RepairSelectedUnmounted()
        {
            var selected = _items.Where(item => item.Selected && IsNotMounted(item) && item.Instance != null).ToList();
            if (selected.Count == 0)
            {
                return;
            }

            var fixedCount = 0;
            foreach (var item in selected)
            {
                var result = MountedStateRepairer.RepairOne(item.Instance);
                if (result == MountedStateRepairer.RepairResult.Fixed)
                {
                    fixedCount++;
                }
            }

            ShowToast($"✓ Repaired {fixedCount} switches. Refreshing...", new Color(0.38f, 0.96f, 0.85f, 1f));
            RefreshScan();
        }

        private List<SwitchInfo> FilterItems()
        {
            if (ModConfig.ShowOkSwitches.Value && _filterMode == SwitchFilterMode.NoFlowOnly)
            {
                return _items;
            }

            return _filterMode switch
            {
                SwitchFilterMode.NoFlowOnly => _items.Where(x => x.Status != FlowStatus.OK).ToList(),
                SwitchFilterMode.DegradedOnly => _items.Where(x => x.Status == FlowStatus.Degraded).ToList(),
                _ => _items
            };
        }

        private void SelectAllNoFlow()
        {
            foreach (var item in _items)
            {
                item.Selected = item.Status != FlowStatus.OK;
            }

            RenderCurrentView();
        }

        private void ResetSelectedWithConfirmation()
        {
            var selected = _items.Where(item => item.Selected && item.Instance != null).ToList();
            if (selected.Count == 0)
            {
                return;
            }

            if (selected.Count >= ModConfig.ConfirmThreshold.Value)
            {
                ShowConfirmDialog(selected);
                return;
            }

            ExecuteReset(selected);
        }

        private void ExecuteReset(List<SwitchInfo> selected)
        {
            var count = SwitchResetter.ResetMany(selected);
            MelonLoader.MelonLogger.Msg($"[FixedTableUI] Reset selected done. Count={count}");
            ShowToast($"✓ Reset {count} switches. Refreshing...", new Color(0.38f, 0.96f, 0.85f, 1f));
            RefreshScan();
        }

        private void ShowConfirmDialog(List<SwitchInfo> selected)
        {
            var dialog = new GameObject("ConfirmDialog");
            dialog.transform.SetParent(_root.transform, false);

            var rt = dialog.AddComponent<RectTransform>();
            rt.anchorMin = new Vector2(0.2f, 0.2f);
            rt.anchorMax = new Vector2(0.8f, 0.8f);
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;

            var image = dialog.AddComponent<Image>();
            image.color = new Color(0.12f, 0.04f, 0.04f, 0.98f);

            var layout = dialog.AddComponent<VerticalLayoutGroup>();
            layout.spacing = 8;
            layout.padding = new RectOffset { left = 12, right = 12, top = 12, bottom = 12 };
            layout.childControlWidth = true;
            layout.childControlHeight = false;
            layout.childForceExpandHeight = false;

            CreateText(dialog.transform, $"⚠ Confirm Factory Reset ({selected.Count})");
            foreach (var item in selected.Take(8))
            {
                CreateText(dialog.transform, $"• {item.Name} ({item.RackId}) — {GetStatusText(item)}", 14);
            }
            CreateText(dialog.transform, "This will disconnect cables, clear LACP/map and reinitialize.", 14);
            CreateText(dialog.transform, "Backup will be created in UserData/greg.Mods.ResetSwitch.", 14);

            var row = new GameObject("Buttons");
            row.transform.SetParent(dialog.transform, false);
            var rowLayout = row.AddComponent<HorizontalLayoutGroup>();
            rowLayout.spacing = 8;
            rowLayout.childControlHeight = true;
            rowLayout.childControlWidth = false;
            rowLayout.childForceExpandHeight = true;
            rowLayout.childForceExpandWidth = false;

            CreateButton(row.transform, "PROCEED", 180, () =>
            {
                UnityEngine.Object.Destroy(dialog);
                ExecuteReset(selected);
            });
            CreateButton(row.transform, "CANCEL", 180, () => UnityEngine.Object.Destroy(dialog));
        }

        private void SetFocus(bool focus)
        {
            _isFocused = focus;
            var controller = UnityEngine.Object.FindObjectOfType<UnityStandardAssets.Characters.FirstPerson.FirstPersonController>();

            if (focus)
            {
                if (controller != null)
                {
                    controller.enabled = false;
                }

                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
                MelonLoader.MelonLogger.Msg("[FixedTableUI] UI focus ON.");
            }
            else
            {
                if (controller != null)
                {
                    controller.enabled = true;
                }

                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
                MelonLoader.MelonLogger.Msg("[FixedTableUI] UI focus OFF.");
            }
        }

        private static void CreateHeader(Transform parent)
        {
            var headerGo = new GameObject("Header");
            headerGo.transform.SetParent(parent, false);

            var rt = headerGo.AddComponent<RectTransform>();
            rt.anchorMin = new Vector2(0f, 0.95f);
            rt.anchorMax = new Vector2(1f, 1f);
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;

            var label = headerGo.AddComponent<TextMeshProUGUI>();
            label.text = "GREG RESET SWITCH TOOLKIT  (F8, ALT=Focus Toggle)";
            label.alignment = TextAlignmentOptions.Center;
            label.fontSize = 24;
            label.color = new Color(0.38f, 0.96f, 0.85f, 1f);
        }

        private void CreateToolbar(Transform parent)
        {
            var toolbar = new GameObject("Toolbar");
            toolbar.transform.SetParent(parent, false);

            var rt = toolbar.AddComponent<RectTransform>();
            rt.anchorMin = new Vector2(0.02f, 0.92f);
            rt.anchorMax = new Vector2(0.98f, 0.95f);
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;

            var layout = toolbar.AddComponent<HorizontalLayoutGroup>();
            layout.spacing = 8;
            layout.childControlHeight = true;
            layout.childControlWidth = false;
            layout.childForceExpandHeight = true;
            layout.childForceExpandWidth = false;

            CreateButton(toolbar.transform, "SCAN", 150, RefreshScan);
            CreateButton(toolbar.transform, "SAVE HEALTH", 150, ReAnalyzeSaveHealth);
            var viewBtn = CreateButton(toolbar.transform, "VIEW: TABLE", 170, ToggleViewMode);
            _viewModeLabel = viewBtn.GetComponentInChildren<TextMeshProUGUI>();

            var filterBtn = CreateButton(toolbar.transform, "FILTER: NO FLOW", 180, CycleFilter);
            _filterLabel = filterBtn.GetComponentInChildren<TextMeshProUGUI>();

            CreateButton(toolbar.transform, "NO-FLOW ONLY", 160, () =>
            {
                _filterMode = SwitchFilterMode.NoFlowOnly;
                UpdateFilterLabel();
                RenderCurrentView();
            });

            CreateButton(toolbar.transform, "RESET SELECTED", 180, ResetSelectedWithConfirmation);
            CreateButton(toolbar.transform, "CLOSE", 140, () => Toggle(false));
        }

        private static void CreateEmptyStateRow(Transform parent, string message)
        {
            var rowGo = new GameObject("TableEmptyState");
            rowGo.transform.SetParent(parent, false);

            var rowRect = rowGo.AddComponent<RectTransform>();
            rowRect.sizeDelta = new Vector2(0, 34);
            var rowElement = rowGo.AddComponent<LayoutElement>();
            rowElement.minHeight = 34;
            rowElement.preferredHeight = 34;
            rowElement.flexibleHeight = 0;

            var layout = rowGo.AddComponent<HorizontalLayoutGroup>();
            layout.spacing = 6;
            layout.childControlHeight = false;
            layout.childControlWidth = false;
            layout.childForceExpandHeight = false;
            layout.childForceExpandWidth = false;

            var image = rowGo.AddComponent<Image>();
            image.color = new Color(0f, 0f, 0f, 0.20f);

            var hintColor = new Color(0.84f, 0.86f, 0.88f, 1f);
            CreateCell(rowGo.transform, "-", 70, false, hintColor);
            CreateCell(rowGo.transform, message, 350, false, hintColor);
            CreateCell(rowGo.transform, "-", 280, false, hintColor);
            CreateCell(rowGo.transform, "-", 260, false, hintColor);
            CreateCell(rowGo.transform, "-", 260, false, hintColor);
        }

        private void ReAnalyzeSaveHealth()
        {
            _saveReport = SaveAnalysisService.Analyze();
            UpdateSaveHealthHint();
            ShowToast("✓ Save analysis refreshed.", new Color(0.38f, 0.96f, 0.85f, 1f));
        }

        private void CycleFilter()
        {
            _filterMode = _filterMode switch
            {
                SwitchFilterMode.All => SwitchFilterMode.NoFlowOnly,
                SwitchFilterMode.NoFlowOnly => SwitchFilterMode.DegradedOnly,
                _ => SwitchFilterMode.All
            };

            UpdateFilterLabel();
            RenderCurrentView();
        }

        private void ToggleViewMode()
        {
            _viewMode = _viewMode switch
            {
                UIViewMode.Table => UIViewMode.Metrics,
                UIViewMode.Metrics => UIViewMode.RackPlan,
                _ => UIViewMode.Table
            };
            UpdateViewModeLabel();
            RenderCurrentView();
        }

        private void UpdateViewModeLabel()
        {
            if (_viewModeLabel == null)
            {
                return;
            }

            _viewModeLabel.text = _viewMode switch
            {
                UIViewMode.Table => "VIEW: TABLE",
                UIViewMode.Metrics => "VIEW: METRICS",
                _ => "VIEW: RACK PLAN"
            };
        }

        private void RenderCurrentView()
        {
            if (_viewMode == UIViewMode.Metrics)
            {
                ShowMetricsView();
                UpdateSummary();
                return;
            }

            if (_viewMode == UIViewMode.RackPlan)
            {
                ShowRackPlanView();
                UpdateSummary();
                return;
            }

            PopulateTable(_items);
        }

        private void ShowRackPlanView()
        {
            for (var i = _contentRoot.childCount - 1; i >= 0; i--)
            {
                UnityEngine.Object.Destroy(_contentRoot.GetChild(i).gameObject);
            }

            CreateRackPlanHeaderRow(_contentRoot);

            var grouped = _items
                .GroupBy(x => string.IsNullOrWhiteSpace(x.RackId) ? "UNKNOWN" : x.RackId)
                .OrderBy(g => g.Key)
                .ToList();

            if (grouped.Count == 0)
            {
                CreateRackPlanRow(_contentRoot, "NO RACKS", 0, 0, 0, 0, 0);
                return;
            }

            foreach (var rack in grouped)
            {
                var rackItems = rack.ToList();
                var total = rackItems.Count;
                var ok = rackItems.Count(x => x.Status == FlowStatus.OK);
                var degraded = rackItems.Count(x => x.Status == FlowStatus.Degraded);
                var dead = rackItems.Count(x => x.Status == FlowStatus.Dead);
                var notMounted = rackItems.Count(IsNotMounted);

                CreateRackPlanRow(_contentRoot, rack.Key, total, ok, degraded, dead, notMounted);
            }
        }

        private static void CreateRackPlanHeaderRow(Transform parent)
        {
            var rowGo = new GameObject("RackPlanHeader");
            rowGo.transform.SetParent(parent, false);

            var rowRect = rowGo.AddComponent<RectTransform>();
            rowRect.sizeDelta = new Vector2(0, 38);
            var rowElement = rowGo.AddComponent<LayoutElement>();
            rowElement.minHeight = 38;
            rowElement.preferredHeight = 38;
            rowElement.flexibleHeight = 0;

            var layout = rowGo.AddComponent<HorizontalLayoutGroup>();
            layout.spacing = 6;
            layout.childControlHeight = false;
            layout.childControlWidth = false;
            layout.childForceExpandHeight = false;
            layout.childForceExpandWidth = false;

            var image = rowGo.AddComponent<Image>();
            image.color = new Color(0.08f, 0.18f, 0.18f, 0.95f);

            CreateCell(rowGo.transform, "RACK", 340, true, Color.white);
            CreateCell(rowGo.transform, "TOTAL", 110, true, Color.white);
            CreateCell(rowGo.transform, "OK", 110, true, Color.white);
            CreateCell(rowGo.transform, "DEG", 110, true, Color.white);
            CreateCell(rowGo.transform, "DEAD", 110, true, Color.white);
            CreateCell(rowGo.transform, "NOT MOUNTED", 170, true, Color.white);
        }

        private static void CreateRackPlanRow(Transform parent, string rack, int total, int ok, int degraded, int dead, int notMounted)
        {
            var rowGo = new GameObject($"Rack_{rack}");
            rowGo.transform.SetParent(parent, false);

            var rowRect = rowGo.AddComponent<RectTransform>();
            rowRect.sizeDelta = new Vector2(0, 34);
            var rowElement = rowGo.AddComponent<LayoutElement>();
            rowElement.minHeight = 34;
            rowElement.preferredHeight = 34;
            rowElement.flexibleHeight = 0;

            var layout = rowGo.AddComponent<HorizontalLayoutGroup>();
            layout.spacing = 6;
            layout.childControlHeight = false;
            layout.childControlWidth = false;
            layout.childForceExpandHeight = false;
            layout.childForceExpandWidth = false;

            var image = rowGo.AddComponent<Image>();
            image.color = new Color(0f, 0f, 0f, 0.25f);

            CreateCell(rowGo.transform, rack, 340, false, Color.white);
            CreateCell(rowGo.transform, total.ToString(), 110, false, Color.white);
            CreateCell(rowGo.transform, ok.ToString(), 110, false, new Color(0.23f, 0.65f, 0.36f, 1f));
            CreateCell(rowGo.transform, degraded.ToString(), 110, false, new Color(0.94f, 0.65f, 0.0f, 1f));
            CreateCell(rowGo.transform, dead.ToString(), 110, false, new Color(0.93f, 0.26f, 0.27f, 1f));
            CreateCell(rowGo.transform, notMounted.ToString(), 170, false, new Color(0.94f, 0.65f, 0.0f, 1f));
        }

        private void ShowMetricsView()
        {
            for (var i = _contentRoot.childCount - 1; i >= 0; i--)
            {
                UnityEngine.Object.Destroy(_contentRoot.GetChild(i).gameObject);
            }

            var total = _items.Count;
            var ok = _items.Count(x => x.Status == FlowStatus.OK);
            var degraded = _items.Count(x => x.Status == FlowStatus.Degraded);
            var dead = _items.Count(x => x.Status == FlowStatus.Dead);
            var noFlow = _items.Count(x => x.Status != FlowStatus.OK);
            var broken = _items.Count(x => x.Instance != null && x.Instance.isBroken);
            var notMounted = _items.Count(IsNotMounted);
            var deadIsolated = _items.Count(x => x.DeepStatus == DeepFlowStatus.Isolated);
            var deadOff = _items.Count(x => x.DeepStatus == DeepFlowStatus.PoweredOff);
            var deadBroken = _items.Count(x => x.DeepStatus == DeepFlowStatus.Broken);
            var idleNoTraffic = _items.Count(x => x.DeepStatus == DeepFlowStatus.Idle);

            CreateMetricsHeaderRow(_contentRoot);
            CreateMetricsRow(_contentRoot, "Total Switches", total, Color.white);
            CreateMetricsRow(_contentRoot, "No Flow (Dead + Degraded)", noFlow, new Color(0.94f, 0.65f, 0.0f, 1f));
            CreateMetricsRow(_contentRoot, "No Flow Cause: Isolated", deadIsolated, new Color(0.93f, 0.26f, 0.27f, 1f));
            CreateMetricsRow(_contentRoot, "No Flow Cause: Powered Off", deadOff, new Color(0.93f, 0.26f, 0.27f, 1f));
            CreateMetricsRow(_contentRoot, "No Flow Cause: Broken", deadBroken, new Color(0.93f, 0.26f, 0.27f, 1f));
            CreateMetricsRow(_contentRoot, "No Flow Cause: Idle (No Traffic)", idleNoTraffic, new Color(0.94f, 0.65f, 0.0f, 1f));
            CreateMetricsRow(_contentRoot, "Dead", dead, new Color(0.93f, 0.26f, 0.27f, 1f));
            CreateMetricsRow(_contentRoot, "Degraded", degraded, new Color(0.94f, 0.65f, 0.0f, 1f));
            CreateMetricsRow(_contentRoot, "Broken Hardware", broken, new Color(0.93f, 0.26f, 0.27f, 1f));
            CreateMetricsRow(_contentRoot, "Not Mounted", notMounted, new Color(0.94f, 0.65f, 0.0f, 1f));
            CreateMetricsRow(_contentRoot, "Healthy (OK)", ok, new Color(0.23f, 0.65f, 0.36f, 1f));

            if (total == 0)
            {
                CreateMetricsRow(_contentRoot, "Info", 0, new Color(0.84f, 0.86f, 0.88f, 1f), "No switches scanned yet. Press SCAN.");
            }
        }

        private static void CreateMetricsHeaderRow(Transform parent)
        {
            var rowGo = new GameObject("MetricsHeader");
            rowGo.transform.SetParent(parent, false);

            var rowRect = rowGo.AddComponent<RectTransform>();
            rowRect.sizeDelta = new Vector2(0, 38);
            var rowElement = rowGo.AddComponent<LayoutElement>();
            rowElement.minHeight = 38;
            rowElement.preferredHeight = 38;
            rowElement.flexibleHeight = 0;

            var layout = rowGo.AddComponent<HorizontalLayoutGroup>();
            layout.spacing = 6;
            layout.childControlHeight = false;
            layout.childControlWidth = false;
            layout.childForceExpandHeight = false;
            layout.childForceExpandWidth = false;

            var image = rowGo.AddComponent<Image>();
            image.color = new Color(0.08f, 0.18f, 0.18f, 0.95f);

            CreateCell(rowGo.transform, "METRIC", 500, true, Color.white);
            CreateCell(rowGo.transform, "VALUE", 220, true, Color.white);
            CreateCell(rowGo.transform, "DETAIL", 500, true, Color.white);
        }

        private static void CreateMetricsRow(Transform parent, string metric, int value, Color valueColor, string detail = "")
        {
            var rowGo = new GameObject($"Metric_{metric}");
            rowGo.transform.SetParent(parent, false);

            var rowRect = rowGo.AddComponent<RectTransform>();
            rowRect.sizeDelta = new Vector2(0, 34);
            var rowElement = rowGo.AddComponent<LayoutElement>();
            rowElement.minHeight = 34;
            rowElement.preferredHeight = 34;
            rowElement.flexibleHeight = 0;

            var layout = rowGo.AddComponent<HorizontalLayoutGroup>();
            layout.spacing = 6;
            layout.childControlHeight = false;
            layout.childControlWidth = false;
            layout.childForceExpandHeight = false;
            layout.childForceExpandWidth = false;

            var image = rowGo.AddComponent<Image>();
            image.color = new Color(0f, 0f, 0f, 0.25f);

            CreateCell(rowGo.transform, metric, 500, false, Color.white);
            CreateCell(rowGo.transform, value.ToString(), 220, false, valueColor);
            CreateCell(rowGo.transform, detail, 500, false, new Color(0.84f, 0.86f, 0.88f, 1f));
        }

        private void UpdateFilterLabel()
        {
            if (_filterLabel == null)
            {
                return;
            }

            var label = _filterMode switch
            {
                SwitchFilterMode.All => "ALL",
                SwitchFilterMode.NoFlowOnly => "NO FLOW",
                _ => "DEGRADED"
            };

            _filterLabel.text = $"FILTER: {label}";
        }

        private static GameObject CreateButton(Transform parent, string text, float width, Action onClick)
        {
            var btnGo = new GameObject($"Btn_{text}");
            btnGo.transform.SetParent(parent, false);

            var rt = btnGo.AddComponent<RectTransform>();
            rt.sizeDelta = new Vector2(width, 34);
            var layoutElement = btnGo.AddComponent<LayoutElement>();
            layoutElement.minWidth = width;
            layoutElement.preferredWidth = width;
            layoutElement.minHeight = 34;
            layoutElement.preferredHeight = 34;
            layoutElement.flexibleWidth = 0;
            layoutElement.flexibleHeight = 0;

            var image = btnGo.AddComponent<Image>();
            image.color = new Color(0.08f, 0.22f, 0.22f, 0.95f);

            var button = btnGo.AddComponent<Button>();
            button.onClick.AddListener((System.Action)(() => onClick()));

            var textGo = new GameObject("Text");
            textGo.transform.SetParent(btnGo.transform, false);
            var textRt = textGo.AddComponent<RectTransform>();
            textRt.anchorMin = Vector2.zero;
            textRt.anchorMax = Vector2.one;
            textRt.offsetMin = Vector2.zero;
            textRt.offsetMax = Vector2.zero;

            var label = textGo.AddComponent<TextMeshProUGUI>();
            label.text = text;
            label.alignment = TextAlignmentOptions.Center;
            label.fontSize = 14;
            label.color = Color.white;
            label.enableWordWrapping = false;
            label.overflowMode = TextOverflowModes.Ellipsis;

            return btnGo;
        }

        private static void CreateHeaderRow(Transform parent)
        {
            var rowGo = new GameObject("TableHeader");
            rowGo.transform.SetParent(parent, false);

            var rowRect = rowGo.AddComponent<RectTransform>();
            rowRect.sizeDelta = new Vector2(0, 36);
            var rowElement = rowGo.AddComponent<LayoutElement>();
            rowElement.minHeight = 36;
            rowElement.preferredHeight = 36;
            rowElement.flexibleHeight = 0;

            var layout = rowGo.AddComponent<HorizontalLayoutGroup>();
            layout.spacing = 6;
            layout.childControlHeight = false;
            layout.childControlWidth = false;
            layout.childForceExpandHeight = false;
            layout.childForceExpandWidth = false;

            var image = rowGo.AddComponent<Image>();
            image.color = new Color(0.08f, 0.18f, 0.18f, 0.95f);

            CreateCell(rowGo.transform, "SEL", 70, true, Color.white);
            CreateCell(rowGo.transform, "NAME", 350, true, Color.white);
            CreateCell(rowGo.transform, "RACK", 280, true, Color.white);
            CreateCell(rowGo.transform, "STATUS", 260, true, Color.white);
            CreateCell(rowGo.transform, "ACTION", 260, true, Color.white);
        }

        private void CreateDataRow(Transform parent, SwitchInfo sw, int renderedIndex)
        {
            var rowGo = new GameObject($"Row_{sw.Instance?.switchId ?? "unknown"}");
            rowGo.transform.SetParent(parent, false);

            var rowRect = rowGo.AddComponent<RectTransform>();
            rowRect.sizeDelta = new Vector2(0, 34);
            var rowElement = rowGo.AddComponent<LayoutElement>();
            rowElement.minHeight = 34;
            rowElement.preferredHeight = 34;
            rowElement.flexibleHeight = 0;

            var layout = rowGo.AddComponent<HorizontalLayoutGroup>();
            layout.spacing = 6;
            layout.childControlHeight = false;
            layout.childControlWidth = false;
            layout.childForceExpandHeight = false;
            layout.childForceExpandWidth = false;

            var image = rowGo.AddComponent<Image>();
            image.color = new Color(0f, 0f, 0f, 0.25f);

            var toggleHost = new GameObject("ToggleHost");
            toggleHost.transform.SetParent(rowGo.transform, false);
            var toggleRt = toggleHost.AddComponent<RectTransform>();
            toggleRt.sizeDelta = new Vector2(70, 30);
            var toggleLayout = toggleHost.AddComponent<LayoutElement>();
            toggleLayout.minWidth = 70;
            toggleLayout.preferredWidth = 70;
            toggleLayout.minHeight = 30;
            toggleLayout.preferredHeight = 30;
            toggleLayout.flexibleWidth = 0;
            toggleLayout.flexibleHeight = 0;

            var toggle = toggleHost.AddComponent<Toggle>();
            toggle.isOn = sw.Selected;
            var capturedIdx = renderedIndex;
            toggle.onValueChanged.AddListener((System.Action<bool>)(v =>
            {
                if (capturedIdx >= 0 && capturedIdx < _renderedItems.Count)
                {
                    _renderedItems[capturedIdx].Selected = v;
                }

                MelonLoader.MelonLogger.Msg($"[UI] Toggle[{capturedIdx}] fired: selected={v}");
                UpdateSummary();
            }));

            var displayName = sw.Name;
            if (string.IsNullOrWhiteSpace(displayName))
            {
                displayName = sw.Instance?.switchId;
            }

            if (string.IsNullOrWhiteSpace(displayName))
            {
                displayName = sw.Instance?.name;
            }

            if (string.IsNullOrWhiteSpace(displayName))
            {
                displayName = "UNKNOWN_SWITCH";
            }

            CreateCell(rowGo.transform, displayName, 350, false, Color.white);
            CreateCell(rowGo.transform, sw.RackId ?? "UNKNOWN", 280, false, Color.white);

            var notMounted = IsNotMounted(sw);
            var statusText = notMounted ? "⚠ NOT MOUNTED" : GetStatusText(sw);
            var statusColor = notMounted ? new Color(0.94f, 0.65f, 0.0f, 1f) : GetStatusColor(sw.Status);
            CreateCell(rowGo.transform, statusText, 260, false, statusColor);

            var actionHost = new GameObject("ActionHost");
            actionHost.transform.SetParent(rowGo.transform, false);
            var actionRt = actionHost.AddComponent<RectTransform>();
            actionRt.sizeDelta = new Vector2(260, 30);
            var actionLayout = actionHost.AddComponent<LayoutElement>();
            actionLayout.minWidth = 260;
            actionLayout.preferredWidth = 260;
            actionLayout.minHeight = 30;
            actionLayout.preferredHeight = 30;
            actionLayout.flexibleWidth = 0;
            actionLayout.flexibleHeight = 0;

            var actionRow = actionHost.AddComponent<HorizontalLayoutGroup>();
            actionRow.spacing = 6;
            actionRow.childControlWidth = false;
            actionRow.childControlHeight = true;
            actionRow.childForceExpandWidth = false;
            actionRow.childForceExpandHeight = true;

            if (notMounted)
            {
                CreateButton(actionHost.transform, "REPAIR", 120, () =>
                {
                    if (sw.Instance != null)
                    {
                        var result = MountedStateRepairer.RepairOne(sw.Instance);
                        ShowToast($"Mounted repair {sw.SwitchId}: {result}", new Color(0.38f, 0.96f, 0.85f, 1f));
                        RefreshScan();
                    }
                });
            }

            CreateButton(actionHost.transform, "RESET", 130, () =>
            {
                if (sw.Instance != null)
                {
                    SwitchResetter.ResetMany(new List<SwitchInfo> { sw });
                    RefreshScan();
                }
            });
        }

        private bool IsNotMounted(SwitchInfo sw)
        {
            if (sw?.Instance == null)
            {
                return false;
            }

            if (!string.IsNullOrWhiteSpace(sw.SwitchId) && _mountedCandidates.Contains(sw.SwitchId))
            {
                return true;
            }

            return !sw.Instance.isOn && !sw.Instance.isBroken;
        }

        private void UpdateSaveHealthHint()
        {
            if (_saveReport == null)
            {
                _saveHealthLabel.text = string.Empty;
                return;
            }

            if (_saveReport.SaveCorrupt > 0)
            {
                _saveHealthLabel.color = new Color(0.93f, 0.26f, 0.27f, 1f);
                _saveHealthLabel.text = $"⚠ {_saveReport.SaveCorrupt} switches are corrupt in save. Repair then save immediately.";
            }
            else if (_saveReport.RuntimeMismatch > 0)
            {
                _saveHealthLabel.color = new Color(0.38f, 0.96f, 0.85f, 1f);
                _saveHealthLabel.text = $"✓ Save healthy. {_saveReport.RuntimeMismatch} runtime mismatches are repairable.";
            }
            else
            {
                _saveHealthLabel.color = new Color(0.38f, 0.96f, 0.85f, 1f);
                _saveHealthLabel.text = "✓ Save health check complete.";
            }
        }

        private void ShowToast(string message, Color color)
        {
            _toastLabel.text = message;
            _toastLabel.color = color;
            MelonLoader.MelonCoroutines.Start(ClearToastAfterDelay());
        }

        private IEnumerator ClearToastAfterDelay()
        {
            yield return new WaitForSeconds(3f);
            _toastLabel.text = string.Empty;
        }

        private static TextMeshProUGUI CreateText(Transform parent, string text, float fontSize = 16)
        {
            var textGo = new GameObject("Text");
            textGo.transform.SetParent(parent, false);

            var textRect = textGo.AddComponent<RectTransform>();
            textRect.sizeDelta = new Vector2(0, 28);

            var label = textGo.AddComponent<TextMeshProUGUI>();
            label.text = text;
            label.alignment = TextAlignmentOptions.Left;
            label.fontSize = fontSize;
            label.color = Color.white;
            return label;
        }

        private void UpdateSummary()
        {
            var selected = _items.Count(x => x.Selected);
            var visible = _viewMode == UIViewMode.Table ? _renderedItems.Count : _items.Count;
            _summaryLabel.text = $"Selected: {selected} | Visible: {visible}/{_items.Count} | Filter: {_filterMode}";
        }

        private static string GetStatusText(SwitchInfo item)
        {
            if (item == null)
            {
                return "❌ DEAD";
            }

            return item.DeepStatus switch
            {
                DeepFlowStatus.Active => "✅ OK",
                DeepFlowStatus.Idle => "⚠ DEG (NO TRAFFIC)",
                DeepFlowStatus.PoweredOff => "❌ DEAD (OFF)",
                DeepFlowStatus.Broken => "❌ DEAD (BROKEN)",
                DeepFlowStatus.Isolated => "❌ DEAD (ISOLATED)",
                _ => item.Status == FlowStatus.Degraded ? "⚠ DEG" : "❌ DEAD"
            };
        }

        private static Color GetStatusColor(FlowStatus status)
        {
            return status switch
            {
                FlowStatus.OK => new Color(0.23f, 0.65f, 0.36f, 1f),
                FlowStatus.Degraded => new Color(0.94f, 0.65f, 0.0f, 1f),
                _ => new Color(0.93f, 0.26f, 0.27f, 1f)
            };
        }

        private static void CreateCell(Transform parent, string text, float width, bool isHeader, Color color)
        {
            var cellGo = new GameObject("Cell");
            cellGo.transform.SetParent(parent, false);
            var rect = cellGo.AddComponent<RectTransform>();
            rect.sizeDelta = new Vector2(width, 30);
            var layoutElement = cellGo.AddComponent<LayoutElement>();
            layoutElement.minWidth = width;
            layoutElement.preferredWidth = width;
            layoutElement.minHeight = 30;
            layoutElement.preferredHeight = 30;
            layoutElement.flexibleWidth = 0;
            layoutElement.flexibleHeight = 0;

            var textGo = new GameObject("Text");
            textGo.transform.SetParent(cellGo.transform, false);
            var textRect = textGo.AddComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = new Vector2(4, 2);
            textRect.offsetMax = new Vector2(-4, -2);

            var label = textGo.AddComponent<TextMeshProUGUI>();
            label.text = text;
            label.alignment = TextAlignmentOptions.Midline;
            label.fontSize = isHeader ? 15 : 13;
            label.color = color;
            label.enableWordWrapping = false;
            label.overflowMode = TextOverflowModes.Ellipsis;
        }
    }
}
