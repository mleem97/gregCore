using System;
using System.Collections.Generic;
using System.Linq;
using MoonSharp.Interpreter;
using UnityEngine;
using greg.Sdk.Services;

namespace greg.Core.Scripting.Lua.LuaModules;

public sealed class gregSdkLuaModule : iGregLuaModule
{
    public void Register(Script vm, Table greg)
    {
        var sdk = new Table(vm);
        greg["sdk"] = sdk;

        // --- Targeting Service ---
        var targeting = new Table(vm);
        sdk["targeting"] = targeting;

        targeting["get_target_info"] = (Func<float, Table>)(maxDist => {
            var info = GregTargetingService.GetTargetInfo(maxDist);
            var t = new Table(vm);
            t["type"] = info.TargetType.ToString();
            t["name"] = info.Name;
            t["distance"] = info.Distance;
            t["hit_point"] = new Table(vm) { ["x"] = info.HitPoint.x, ["y"] = info.HitPoint.y, ["z"] = info.HitPoint.z };
            return t;
        });

        // --- Metadata Service ---
        var metadata = new Table(vm);
        sdk["metadata"] = metadata;

        metadata["get_metadata"] = (Func<float, Table>)(maxDist => {
            var info = GregTargetingService.GetTargetInfo(maxDist);
            var entries = GregComponentMetadataService.GetMetadata(info);
            
            var result = new Table(vm);
            result["title"] = info.TargetType == GregTargetType.None ? "SCANNING..." : info.TargetType.ToString().ToUpper();
            result["sub_header"] = info.TargetType == GregTargetType.None ? "" : "TELEMETRY";
            
            var list = new Table(vm);
            for (int i = 0; i < entries.Count; i++)
            {
                var entry = entries[i];
                var et = new Table(vm);
                et["label"] = entry.Label;
                et["value"] = entry.Value;
                et["color"] = $"#{ColorUtility.ToHtmlStringRGB(entry.ValueColor)}";
                list[i + 1] = et;
            }
            result["entries"] = list;
            return result;
        });

        // --- HUD Service ---
        var hud = new Table(vm);
        sdk["hud"] = hud;

        hud["update_jade_box"] = (Action<string, string, Table>)((title, subHeader, entriesTable) => {
            var entries = new List<GregMetadataEntry>();
            foreach (var pair in entriesTable.Pairs)
            {
                var et = pair.Value.Table;
                if (et == null) continue;
                
                string label = et.Get("label").String ?? "";
                string value = et.Get("value").String ?? "";
                string hexColor = et.Get("color").String ?? "#FFFFFF";
                
                ColorUtility.TryParseHtmlString(hexColor, out var color);
                entries.Add(new GregMetadataEntry(label, value, color));
            }
            GregHudService.UpdateJadeBox(title, subHeader, entries);
        });

        hud["hide_jade_box"] = (Action)(() => GregHudService.HideJadeBox());

        // --- UI Hijack Service (v12) ---
        var ui = new Table(vm);
        sdk["ui"] = ui;

        ui["hijack_canvas"] = (Action<string, bool>)((name, active) => {
            var canvases = GameObject.FindObjectsOfType<Canvas>(true);
            foreach (var c in canvases)
            {
                if (c.name == name)
                {
                    c.gameObject.SetActive(active);
                    break;
                }
            }
        });

        ui["create_modern_canvas"] = (Func<string, int, DynValue>)((name, sorting) => {
            var canvas = GregUiService.CreateCanvas(name, sorting);
            return UserData.Create(canvas);
        });
    }
}
