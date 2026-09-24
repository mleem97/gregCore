/// <file-summary>
/// Schicht:      Core (Networking)
/// Zweck:        Live-Brücke für SFP (Module/Box), RackTür, RackMount und
///               RackTemplates (Store + Applier): finden, lesen, sichere
///               Aktionen (Vanilla-Pfade, Coroutinen via MelonCoroutines).
///               Alles best-effort.
/// </file-summary>

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using MelonLoader;
using UnityEngine;

namespace gregCore.Core.Networking;

[ExcludeFromCodeCoverage(Justification = "Live Il2Cpp interop against game assemblies; needs running game.")]
public static class GregRackAndSfp
{
    // ── DTOs (RackTemplate) ──────────────────────────────────────────────────

    public sealed class TemplateDevice
    {
        public int Kind;
        public int PrefabID;
        public int PositionIndex;
        public int SizeInU;
        public string Label = "";
    }

    public sealed class TemplateSfp
    {
        public int DeviceIndex;
        public Vector3 PortLocalPos;
        public int SfpType;
    }

    public sealed class TemplateCable
    {
        public int DeviceIndexA;
        public Vector3 PortLocalPosA;
        public int DeviceIndexB;
        public Vector3 PortLocalPosB;
        public Color Color = Color.white;
        public List<Vector3> Waypoints = new List<Vector3>();
    }

    public sealed class RackTemplateDto
    {
        public string TemplateId = "";
        public int Price;
        public bool HasCustomColor;
        public Color RackColor = Color.white;
        public List<TemplateDevice> Devices = new List<TemplateDevice>();
        public List<TemplateSfp> Sfps = new List<TemplateSfp>();
        public List<TemplateCable> Cables = new List<TemplateCable>();
    }

    // ── Finden ───────────────────────────────────────────────────────────────

    public static List<global::Il2Cpp.SFPModule> FindAllSfpModules()
    {
        return FindAll<global::Il2Cpp.SFPModule>();
    }

    public static List<global::Il2Cpp.SFPBox> FindAllSfpBoxes()
    {
        return FindAll<global::Il2Cpp.SFPBox>();
    }

    public static List<global::Il2Cpp.RackDoor> FindAllRackDoors()
    {
        return FindAll<global::Il2Cpp.RackDoor>();
    }

    public static List<global::Il2Cpp.RackMount> FindAllRackMounts()
    {
        return FindAll<global::Il2Cpp.RackMount>();
    }

    private static List<T> FindAll<T>() where T : UnityEngine.Object
    {
        var result = new List<T>();
        Try(() =>
        {
            var all = Resources.FindObjectsOfTypeAll<T>();
            if (all == null) return;
            foreach (var o in all)
            {
                if (o == null) continue;
                try
                {
                    var go = (o as Component) != null
                        ? ((Component)(object)o).gameObject
                        : (o as GameObject);
                    if (go != null && go.scene.IsValid() && go.scene.isLoaded)
                        result.Add(o);
                }
                catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
            }
        });
        return result;
    }

    // ── SFP-Module ───────────────────────────────────────────────────────────

    public static float GetSfpSpeed(global::Il2Cpp.SFPModule module)
    {
        if (module == null) return 0f;
        try { return module.speed; } catch { return 0f; }
    }

    public static int GetSfpType(global::Il2Cpp.SFPModule module)
    {
        if (module == null) return -1;
        try { return module.sfpType; } catch { return -1; }
    }

    public static bool IsSfpInBox(global::Il2Cpp.SFPModule module)
    {
        if (module == null) return false;
        try { return module.isInTheBox; } catch { return false; }
    }

    public static bool InsertSfpDirectly(global::Il2Cpp.SFPModule module, global::Il2Cpp.CableLink link)
    {
        if (module == null || link == null) return false;
        try
        {
            var _ = module.gameObject; // liveness
            module.InsertDirectlyIntoPort(link);
            return true;
        }
        catch (Exception ex)
        {
            Warn($"InsertDirectlyIntoPort fehlgeschlagen: {Base(ex)}");
            return false;
        }
    }

    public static bool RemoveSfpFromPort(global::Il2Cpp.SFPModule module)
    {
        if (module == null) return false;
        try
        {
            var _ = module.gameObject; // liveness
            module.RemoveFromPort();
            return true;
        }
        catch (Exception ex)
        {
            Warn($"RemoveFromPort fehlgeschlagen: {Base(ex)}");
            return false;
        }
    }

    public static bool IsAnyCableConnected(global::Il2Cpp.SFPModule module)
    {
        if (module == null) return false;
        try { return module.IsAnyCableConnected(); } catch { return false; }
    }

    // ── SFP-Box ──────────────────────────────────────────────────────────────

    public static int GetBoxFreeSpace(global::Il2Cpp.SFPBox box)
    {
        if (box == null) return -1;
        try
        {
            var _ = box.gameObject; // liveness
            return box.GetFreeSpaceInTheBox();
        }
        catch { return -1; }
    }

    public static bool BoxCanAccept(global::Il2Cpp.SFPBox box, int sfpType)
    {
        if (box == null) return false;
        try
        {
            var _ = box.gameObject; // liveness
            return box.CanAcceptSFP(sfpType);
        }
        catch { return false; }
    }

    public static global::Il2Cpp.SFPModule TakeSfpFromBox(global::Il2Cpp.SFPBox box)
    {
        if (box == null) return null;
        try
        {
            var _ = box.gameObject; // liveness
            return box.TakeSFPFromBox();
        }
        catch (Exception ex)
        {
            Warn($"TakeSFPFromBox fehlgeschlagen: {Base(ex)}");
            return null;
        }
    }

    public static bool InsertSfpBackIntoBox(global::Il2Cpp.SFPBox box)
    {
        if (box == null) return false;
        try
        {
            var _ = box.gameObject; // liveness
            box.InsertSFPBackIntoBox();
            return true;
        }
        catch (Exception ex)
        {
            Warn($"InsertSFPBackIntoBox fehlgeschlagen: {Base(ex)}");
            return false;
        }
    }

    // ── RackTür / RackMount ──────────────────────────────────────────────────

    public static bool IsDoorOpened(global::Il2Cpp.RackDoor door)
    {
        if (door == null) return false;
        try { return door.isOpened; } catch { return false; }
    }

    public static bool ClickDoor(global::Il2Cpp.RackDoor door)
    {
        if (door == null) return false;
        try
        {
            var _ = door.gameObject; // liveness
            door.InteractOnClick();
            return true;
        }
        catch (Exception ex)
        {
            Warn($"RackDoor-Click fehlgeschlagen: {Base(ex)}");
            return false;
        }
    }

    public static bool IsRackInstantiated(global::Il2Cpp.RackMount mount)
    {
        if (mount == null) return false;
        try { return mount.isRackInstantiated; } catch { return false; }
    }

    public static bool BeginInstallRack(global::Il2Cpp.RackMount mount, bool cheat, int type)
    {
        if (mount == null) return false;
        try
        {
            var _ = mount.gameObject; // liveness
            var routine = mount.InstallRack(cheat, type, false);
            return StartIl2CppRoutine(routine);
        }
        catch (Exception ex)
        {
            Warn($"InstallRack fehlgeschlagen: {Base(ex)}");
            return false;
        }
    }

    // ── RackTemplateStore ────────────────────────────────────────────────────

    public static List<string> ListTemplateIds()
    {
        var result = new List<string>();
        Try(() =>
        {
            var dict = global::Il2Cpp.RackTemplateStore.templates;
            if (dict == null) return;
            foreach (var kv in dict)
            {
                try
                {
                    if (!string.IsNullOrEmpty(kv.Key)) result.Add(kv.Key);
                }
                catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
            }
        });
        result.Sort(StringComparer.OrdinalIgnoreCase);
        return result;
    }

    public static RackTemplateDto LoadTemplate(string templateId)
    {
        if (string.IsNullOrEmpty(templateId)) return new RackTemplateDto();
        global::Il2Cpp.RackTemplate template = null;
        try { template = global::Il2Cpp.RackTemplateStore.Load(templateId); } catch { template = null; }
        return ReadTemplate(template);
    }

    public static RackTemplateDto ReadTemplate(global::Il2Cpp.RackTemplate template)
    {
        var dto = new RackTemplateDto();
        if (template == null) return dto;
        Try(() => dto.TemplateId = template.templateId ?? "");
        Try(() => dto.Price = template.price);
        Try(() => dto.HasCustomColor = template.hasCustomColor);
        Try(() => dto.RackColor = template.rackColor);
        Try(() =>
        {
            var list = template.devices;
            if (list == null) return;
            foreach (var d in list)
            {
                if (d == null) continue;
                var td = new TemplateDevice();
                try { td.Kind = d.kind; } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
                try { td.PrefabID = d.prefabID; } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
                try { td.PositionIndex = d.positionIndex; } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
                try { td.SizeInU = d.sizeInU; } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
                try { td.Label = d.label ?? ""; } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
                dto.Devices.Add(td);
            }
        });
        Try(() =>
        {
            var list = template.sfps;
            if (list == null) return;
            foreach (var s in list)
            {
                if (s == null) continue;
                var ts = new TemplateSfp();
                try { ts.DeviceIndex = s.deviceIndex; } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
                try { ts.PortLocalPos = s.portLocalPos; } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
                try { ts.SfpType = s.sfpType; } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
                dto.Sfps.Add(ts);
            }
        });
        Try(() =>
        {
            var list = template.cables;
            if (list == null) return;
            foreach (var c in list)
            {
                if (c == null) continue;
                var tc = new TemplateCable();
                try { tc.DeviceIndexA = c.deviceIndexA; } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
                try { tc.PortLocalPosA = c.portLocalPosA; } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
                try { tc.DeviceIndexB = c.deviceIndexB; } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
                try { tc.PortLocalPosB = c.portLocalPosB; } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
                try { tc.Color = c.color; } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
                try
                {
                    var wps = c.waypointRackLocalPos;
                    if (wps != null)
                        foreach (var v in wps)
                        {
                            try { tc.Waypoints.Add(v); } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
                        }
                }
                catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
                dto.Cables.Add(tc);
            }
        });
        return dto;
    }

    public static bool DeleteTemplate(string templateId)
    {
        if (string.IsNullOrEmpty(templateId)) return false;
        try { return global::Il2Cpp.RackTemplateStore.Delete(templateId); }
        catch (Exception ex)
        {
            Warn($"DeleteTemplate fehlgeschlagen: {Base(ex)}");
            return false;
        }
    }

    public static bool BeginApplyTemplate(global::Il2Cpp.Rack rack, string templateId)
    {
        if (rack == null || string.IsNullOrEmpty(templateId)) return false;
        global::Il2Cpp.RackTemplate template = null;
        try { template = global::Il2Cpp.RackTemplateStore.Load(templateId); } catch { template = null; }
        if (template == null)
        {
            Warn($"Template nicht gefunden: '{templateId}'.");
            return false;
        }
        try
        {
            var _ = rack.gameObject; // liveness
            var routine = global::Il2Cpp.RackTemplateApplier.Apply(rack, template);
            return StartIl2CppRoutine(routine);
        }
        catch (Exception ex)
        {
            Warn($"ApplyTemplate fehlgeschlagen: {Base(ex)}");
            return false;
        }
    }

    // Il2Cpp-Coroutine über verwaltete Pumpe starten (MoveNext pro Frame).
    // Internal: auch von GregRacks nutzbar (gleiche Assembly).
    internal static bool StartIl2CppRoutine(Il2CppSystem.Collections.IEnumerator routine)
    {
        if (routine == null) return false;
        try
        {
            MelonCoroutines.Start(PumpRoutine(routine));
            return true;
        }
        catch (Exception ex)
        {
            Warn($"Coroutine-Start fehlgeschlagen: {Base(ex)}");
            return false;
        }
    }

    private static System.Collections.IEnumerator PumpRoutine(Il2CppSystem.Collections.IEnumerator routine)
    {
        while (true)
        {
            bool more = false;
            try { more = routine.MoveNext(); }
            catch { yield break; }
            if (!more) yield break;
            yield return null;
        }
    }

    private static string Base(Exception ex)
    {
        try { return ex != null ? ex.GetBaseException().Message : "?"; } catch { return "?"; }
    }

    private static void Warn(string message)
    {
        try { MelonLogger.Warning($"[gregCore][Net] RackAndSfp: {message}"); } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
    }

    private static void Try(Action action)
    {
        try { action?.Invoke(); }
        catch (Exception ex)
        {
            try { MelonLogger.Warning($"[gregCore][Net] RackAndSfp-Feld fehlgeschlagen: {ex.GetBaseException().Message}"); } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
        }
    }
}
