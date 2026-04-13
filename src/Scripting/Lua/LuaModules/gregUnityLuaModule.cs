using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using Il2Cpp;
using MoonSharp.Interpreter;
using UnityEngine;
using greg.Core;

namespace greg.Core.Scripting.Lua;

/// <summary>
/// Generic Unity/Il2Cpp API surface for Lua via integer handles.
/// All heavy Unity work stays in C#; Lua drives policy via handle IDs.
/// </summary>
public sealed class gregUnityLuaModule : iGregLuaModule
{
    public void Register(Script vm, Table greg)
    {
        var unity = new Table(vm);
        greg["unity"] = unity;
        RegisterFinders(vm, unity);
        RegisterComponents(vm, unity);
        RegisterProperties(vm, unity);
        RegisterTransform(vm, unity);
        RegisterInstantiate(vm, unity);
        RegisterMaterial(vm, unity);
        RegisterTmpro(vm, unity);
        RegisterTextMesh(vm, unity);
        RegisterPhysics(vm, unity);
        RegisterGameObject(vm, unity);

        var color = new Table(vm);
        greg["color"] = color;
        RegisterColor(vm, color);

        var config = new Table(vm);
        greg["config"] = config;
        RegisterConfig(vm, config);

        var gui = new Table(vm);
        greg["gui"] = gui;
        RegisterGui(vm, gui);

        var hud = new Table(vm);
        greg["hud"] = hud;
        RegisterHud(vm, hud);

        var target = new Table(vm);
        greg["target"] = target;
        RegisterTarget(vm, target);
    }

    private static void RegisterHud(Script vm, Table hud)
    {
        hud["begin_panel"] = (Action<string, Table>)((id, rect) =>
        {
            float x = rect["x"] is double dx ? (float)dx : 0f;
            float y = rect["y"] is double dy ? (float)dy : 0f;
            float w = rect["w"] is double dw ? (float)dw : 200f;
            float h = rect["h"] is double dh ? (float)dh : 100f;
            gregGameHooks.GuiBeginPanel(id, x, y, w, h);
        });

        hud["label"] = (Action<string>)(text => gregGameHooks.GuiLabel(text));
        hud["end_panel"] = (Action)(() => gregGameHooks.GuiEndPanel());
    }

    private static void RegisterTarget(Script vm, Table target)
    {
        target["raycast_forward"] = (Func<double, Table>)(distance =>
        {
            var hit = gregGameHooks.RaycastForward((float)distance);
            if (hit == null) return null;

            var t = new Table(vm);
            t["name"] = hit.Value.Name;
            t["distance"] = (double)hit.Value.Distance;
            var pt = new Table(vm);
            pt["x"] = (double)hit.Value.Point.x;
            pt["y"] = (double)hit.Value.Point.y;
            pt["z"] = (double)hit.Value.Point.z;
            t["point"] = pt;
            
            if (hit.Value.Entity != null)
            {
                t["entity_handle"] = gregLuaObjectHandleRegistry.Register(hit.Value.Entity);
            }

            return t;
        });
    }

    private static void RegisterFinders(Script vm, Table unity)
    {
        // greg.unity.find(typeName) -> table of handles
        unity["find"] = (Func<string, Table>)(typeName =>
        {
            var result = new Table(vm);
            try
            {
                var type = ResolveIl2CppType(typeName);
                if (type == null) return result;
                var objects = UnityEngine.Object.FindObjectsOfType(Il2CppInterop.Runtime.Il2CppType.From(type));
                if (objects == null) return result;
                for (int i = 0; i < objects.Count; i++)
                {
                    var obj = objects[i];
                    if (obj != null)
                        result.Append(DynValue.NewNumber(gregLuaObjectHandleRegistry.Register(obj)));
                }
            }
            catch (Exception ex) { CrashLog.LogException("lua:greg.unity.find", ex); }
            return result;
        });

        // greg.unity.find_child(handle, childName) -> handle or 0
        unity["find_child"] = (Func<int, string, int>)((handle, childName) =>
        {
            try
            {
                var obj = gregLuaObjectHandleRegistry.Resolve(handle);
                Transform t = null;
                if (obj is GameObject go) t = go.transform;
                else if (obj is Component c) t = c.transform;
                if (t == null) return 0;
                var child = t.Find(childName);
                return child != null ? gregLuaObjectHandleRegistry.Register(child.gameObject) : 0;
            }
            catch { return 0; }
        });

        // greg.unity.handle_alive(handle) -> bool
        unity["handle_alive"] = (Func<int, bool>)(handle =>
        {
            var obj = gregLuaObjectHandleRegistry.Resolve(handle);
            if (obj == null) return false;
            if (obj is UnityEngine.Object uo) return uo != null;
            return true;
        });

        // greg.unity.release(handle)
        unity["release"] = (Action<int>)(handle => gregLuaObjectHandleRegistry.Release(handle));
    }

    private static void RegisterComponents(Script vm, Table unity)
    {
        // greg.unity.get_component(handle, typeName) -> handle or 0
        unity["get_component"] = (Func<int, string, int>)((handle, typeName) =>
        {
            try
            {
                var obj = gregLuaObjectHandleRegistry.Resolve(handle);
                GameObject go = null;
                if (obj is GameObject g) go = g;
                else if (obj is Component c) go = c.gameObject;
                if (go == null) return 0;

                var type = ResolveIl2CppType(typeName);
                if (type == null) return 0;
                var comp = go.GetComponent(Il2CppInterop.Runtime.Il2CppType.From(type));
                return comp != null ? gregLuaObjectHandleRegistry.Register(comp) : 0;
            }
            catch { return 0; }
        });

        // greg.unity.add_component(handle, typeName) -> handle or 0
        unity["add_component"] = (Func<int, string, int>)((handle, typeName) =>
        {
            try
            {
                var obj = gregLuaObjectHandleRegistry.Resolve(handle);
                GameObject go = null;
                if (obj is GameObject g) go = g;
                else if (obj is Component c) go = c.gameObject;
                if (go == null) return 0;

                var type = ResolveIl2CppType(typeName);
                if (type == null) return 0;
                var comp = go.AddComponent(Il2CppInterop.Runtime.Il2CppType.From(type));
                return comp != null ? gregLuaObjectHandleRegistry.Register(comp) : 0;
            }
            catch { return 0; }
        });
    }

    private static void RegisterProperties(Script vm, Table unity)
    {
        // greg.unity.get_string(handle, memberName) -> string or nil
        unity["get_string"] = (Func<int, string, string>)((handle, name) =>
        {
            try
            {
                var obj = gregLuaObjectHandleRegistry.Resolve(handle);
                if (obj == null) return null;
                var val = GetMemberValue(obj, name);
                return val?.ToString();
            }
            catch { return null; }
        });

        // greg.unity.get_number(handle, memberName) -> number or 0
        unity["get_number"] = (Func<int, string, double>)((handle, name) =>
        {
            try
            {
                var obj = gregLuaObjectHandleRegistry.Resolve(handle);
                if (obj == null) return 0;
                var val = GetMemberValue(obj, name);
                if (val is float f) return f;
                if (val is double d) return d;
                if (val is int i) return i;
                if (val is uint u) return u;
                return 0;
            }
            catch { return 0; }
        });

        // greg.unity.get_bool(handle, memberName) -> bool
        unity["get_bool"] = (Func<int, string, bool>)((handle, name) =>
        {
            try
            {
                var obj = gregLuaObjectHandleRegistry.Resolve(handle);
                if (obj == null) return false;
                var val = GetMemberValue(obj, name);
                return val is true;
            }
            catch { return false; }
        });

        // greg.unity.set_string(handle, memberName, value)
        unity["set_string"] = (Action<int, string, string>)((handle, name, value) =>
        {
            try
            {
                var obj = gregLuaObjectHandleRegistry.Resolve(handle);
                if (obj != null) SetMemberValue(obj, name, value);
            }
            catch { }
        });

        // greg.unity.set_number(handle, memberName, value)
        unity["set_number"] = (Action<int, string, double>)((handle, name, value) =>
        {
            try
            {
                var obj = gregLuaObjectHandleRegistry.Resolve(handle);
                if (obj == null) return;
                var member = FindMember(obj.GetType(), name);
                if (member is FieldInfo fi)
                {
                    if (fi.FieldType == typeof(float)) fi.SetValue(obj, (float)value);
                    else if (fi.FieldType == typeof(int)) fi.SetValue(obj, (int)value);
                    else fi.SetValue(obj, value);
                }
                else if (member is PropertyInfo pi && pi.CanWrite)
                {
                    if (pi.PropertyType == typeof(float)) pi.SetValue(obj, (float)value);
                    else if (pi.PropertyType == typeof(int)) pi.SetValue(obj, (int)value);
                    else pi.SetValue(obj, value);
                }
            }
            catch { }
        });

        // greg.unity.set_bool(handle, memberName, value)
        unity["set_bool"] = (Action<int, string, bool>)((handle, name, value) =>
        {
            try
            {
                var obj = gregLuaObjectHandleRegistry.Resolve(handle);
                if (obj != null) SetMemberValue(obj, name, value);
            }
            catch { }
        });

        // greg.unity.get_handle(handle, memberName) -> handle to the referenced object
        unity["get_handle"] = (Func<int, string, int>)((handle, name) =>
        {
            try
            {
                var obj = gregLuaObjectHandleRegistry.Resolve(handle);
                if (obj == null) return 0;
                var val = GetMemberValue(obj, name);
                return val != null ? gregLuaObjectHandleRegistry.Register(val) : 0;
            }
            catch { return 0; }
        });
    }

    private static void RegisterTransform(Script vm, Table unity)
    {
        // greg.unity.position(handle) -> x, y, z (as table {x, y, z})
        unity["position"] = (Func<int, Table>)(handle =>
        {
            var t = new Table(vm);
            try
            {
                var obj = gregLuaObjectHandleRegistry.Resolve(handle);
                Transform tr = null;
                if (obj is GameObject go) tr = go.transform;
                else if (obj is Component c) tr = c.transform;
                else if (obj is Transform tt) tr = tt;
                if (tr != null)
                {
                    t["x"] = (double)tr.position.x;
                    t["y"] = (double)tr.position.y;
                    t["z"] = (double)tr.position.z;
                }
            }
            catch { }
            return t;
        });

        // greg.unity.set_position(handle, x, y, z)
        unity["set_position"] = (Action<int, double, double, double>)((handle, x, y, z) =>
        {
            try
            {
                var obj = gregLuaObjectHandleRegistry.Resolve(handle);
                Transform tr = null;
                if (obj is GameObject go) tr = go.transform;
                else if (obj is Component c) tr = c.transform;
                else if (obj is Transform tt) tr = tt;
                if (tr != null) tr.position = new Vector3((float)x, (float)y, (float)z);
            }
            catch { }
        });

        // greg.unity.set_local_scale(handle, x, y, z)
        unity["set_local_scale"] = (Action<int, double, double, double>)((handle, x, y, z) =>
        {
            try
            {
                var obj = gregLuaObjectHandleRegistry.Resolve(handle);
                Transform tr = null;
                if (obj is GameObject go) tr = go.transform;
                else if (obj is Component c) tr = c.transform;
                else if (obj is Transform tt) tr = tt;
                if (tr != null) tr.localScale = new Vector3((float)x, (float)y, (float)z);
            }
            catch { }
        });

        // greg.unity.set_rotation(handle, x, y, z, w)
        unity["set_rotation"] = (Action<int, double, double, double, double>)((handle, x, y, z, w) =>
        {
            try
            {
                var obj = gregLuaObjectHandleRegistry.Resolve(handle);
                Transform tr = null;
                if (obj is GameObject go) tr = go.transform;
                else if (obj is Component c) tr = c.transform;
                else if (obj is Transform tt) tr = tt;
                if (tr != null) tr.rotation = new Quaternion((float)x, (float)y, (float)z, (float)w);
            }
            catch { }
        });

        // greg.unity.set_parent(handle, parentHandle)
        unity["set_parent"] = (Action<int, int>)((handle, parentHandle) =>
        {
            try
            {
                var obj = gregLuaObjectHandleRegistry.Resolve(handle);
                var parentObj = gregLuaObjectHandleRegistry.Resolve(parentHandle);
                Transform child = null, parent = null;
                if (obj is GameObject go) child = go.transform;
                else if (obj is Component c) child = c.transform;
                if (parentObj is GameObject pgo) parent = pgo.transform;
                else if (parentObj is Component pc) parent = pc.transform;
                else if (parentObj is Transform pt) parent = pt;
                if (child != null) child.SetParent(parent, true);
            }
            catch { }
        });
    }

    private static void RegisterInstantiate(Script vm, Table unity)
    {
        // greg.unity.instantiate(handle, parentHandle?) -> new handle
        unity["instantiate"] = (Func<int, int, int>)((handle, parentHandle) =>
        {
            try
            {
                var obj = gregLuaObjectHandleRegistry.Resolve(handle);
                if (obj is not UnityEngine.Object uo) return 0;
                var parent = gregLuaObjectHandleRegistry.Resolve(parentHandle);
                Transform parentT = null;
                if (parent is GameObject pgo) parentT = pgo.transform;
                else if (parent is Component pc) parentT = pc.transform;
                else if (parent is Transform pt) parentT = pt;
                var clone = parentT != null
                    ? UnityEngine.Object.Instantiate(uo, parentT)
                    : UnityEngine.Object.Instantiate(uo);
                return clone != null ? gregLuaObjectHandleRegistry.Register(clone) : 0;
            }
            catch { return 0; }
        });

        // greg.unity.destroy(handle)
        unity["destroy"] = (Action<int>)(handle =>
        {
            try
            {
                var obj = gregLuaObjectHandleRegistry.Resolve(handle);
                if (obj is UnityEngine.Object uo) UnityEngine.Object.Destroy(uo);
                gregLuaObjectHandleRegistry.Release(handle);
            }
            catch { }
        });
    }

    private static void RegisterMaterial(Script vm, Table unity)
    {
        // greg.unity.material_hex(handle, propertyName) -> "#RRGGBB" or nil
        unity["material_hex"] = (Func<int, string, string>)((handle, prop) =>
        {
            try
            {
                var obj = gregLuaObjectHandleRegistry.Resolve(handle);
                Material mat = null;
                if (obj is Material m) mat = m;
                else if (obj is Renderer r) mat = r.sharedMaterial;
                else if (obj is Component c)
                {
                    var rend = c.GetComponent<Renderer>();
                    if (rend != null) mat = rend.sharedMaterial;
                }
                if (mat == null || !mat.HasProperty(prop)) return null;
                var col = mat.GetColor(prop);
                return ColorToHex(col);
            }
            catch { return null; }
        });
    }

    private static void RegisterTmpro(Script vm, Table unity)
    {
        // greg.unity.tmpro_set(handle, text, fontSize, fontMin, fontMax, autoSize, wordWrap, alignment, colorHex)
        unity["tmpro_set"] = (Action<int, string, double, double, double, bool, bool, int, string>)(
            (handle, text, fontSize, fontMin, fontMax, autoSize, wordWrap, alignment, colorHex) =>
        {
            try
            {
                var obj = gregLuaObjectHandleRegistry.Resolve(handle);
                TextMeshProUGUI tmp = null;
                if (obj is TextMeshProUGUI t) tmp = t;
                else if (obj is GameObject go) tmp = go.GetComponent<TextMeshProUGUI>();
                else if (obj is Component c) tmp = c.GetComponent<TextMeshProUGUI>();
                if (tmp == null) return;

                if (text != null) tmp.text = text;
                if (fontSize > 0) tmp.fontSize = (float)fontSize;
                tmp.fontSizeMin = (float)fontMin;
                tmp.fontSizeMax = (float)fontMax;
                tmp.enableAutoSizing = autoSize;
                tmp.enableWordWrapping = wordWrap;
                tmp.alignment = (TextAlignmentOptions)alignment;
                if (!string.IsNullOrEmpty(colorHex) && ColorUtility.TryParseHtmlString(colorHex, out var col))
                    tmp.color = col;
            }
            catch { }
        });

        // greg.unity.tmpro_get_text(handle) -> string
        unity["tmpro_get_text"] = (Func<int, string>)(handle =>
        {
            try
            {
                var obj = gregLuaObjectHandleRegistry.Resolve(handle);
                TextMeshProUGUI tmp = null;
                if (obj is TextMeshProUGUI t) tmp = t;
                else if (obj is GameObject go) tmp = go.GetComponent<TextMeshProUGUI>();
                else if (obj is Component c) tmp = c.GetComponent<TextMeshProUGUI>();
                return tmp?.text;
            }
            catch { return null; }
        });

        // greg.unity.tmpro_anchored_pos(handle) -> {x, y}
        unity["tmpro_anchored_pos"] = (Func<int, Table>)(handle =>
        {
            var t = new Table(vm);
            try
            {
                var obj = gregLuaObjectHandleRegistry.Resolve(handle);
                TextMeshProUGUI tmp = null;
                if (obj is TextMeshProUGUI tt) tmp = tt;
                else if (obj is GameObject go) tmp = go.GetComponent<TextMeshProUGUI>();
                else if (obj is Component c) tmp = c.GetComponent<TextMeshProUGUI>();
                if (tmp?.rectTransform != null)
                {
                    t["x"] = (double)tmp.rectTransform.anchoredPosition.x;
                    t["y"] = (double)tmp.rectTransform.anchoredPosition.y;
                }
            }
            catch { }
            return t;
        });

        // greg.unity.tmpro_set_anchored_pos(handle, x, y)
        unity["tmpro_set_anchored_pos"] = (Action<int, double, double>)((handle, x, y) =>
        {
            try
            {
                var obj = gregLuaObjectHandleRegistry.Resolve(handle);
                TextMeshProUGUI tmp = null;
                if (obj is TextMeshProUGUI t) tmp = t;
                else if (obj is GameObject go) tmp = go.GetComponent<TextMeshProUGUI>();
                else if (obj is Component c) tmp = c.GetComponent<TextMeshProUGUI>();
                if (tmp?.rectTransform != null)
                    tmp.rectTransform.anchoredPosition = new Vector2((float)x, (float)y);
            }
            catch { }
        });
    }

    private static void RegisterTextMesh(Script vm, Table unity)
    {
        // greg.unity.textmesh_set(handle, text, fontSize, charSize, colorHex, anchor, alignment)
        unity["textmesh_set"] = (Action<int, string, int, double, string, int, int>)(
            (handle, text, fontSize, charSize, colorHex, anchor, alignment) =>
        {
            try
            {
                var obj = gregLuaObjectHandleRegistry.Resolve(handle);
                TextMesh tm = null;
                if (obj is TextMesh t) tm = t;
                else if (obj is GameObject go) tm = go.GetComponent<TextMesh>();
                else if (obj is Component c) tm = c.GetComponent<TextMesh>();
                if (tm == null) return;

                if (text != null) tm.text = text;
                if (fontSize > 0) tm.fontSize = fontSize;
                if (charSize > 0) tm.characterSize = (float)charSize;
                tm.anchor = (TextAnchor)anchor;
                tm.alignment = (TextAlignment)alignment;
                if (!string.IsNullOrEmpty(colorHex) && ColorUtility.TryParseHtmlString(colorHex, out var col))
                    tm.color = col;
            }
            catch { }
        });
    }

    private static void RegisterPhysics(Script vm, Table unity)
    {
        // greg.unity.raycast(ox, oy, oz, dx, dy, dz, maxDist) -> { hit, handle, point = {x,y,z} }
        unity["raycast"] = (Func<double, double, double, double, double, double, double, Table>)(
            (ox, oy, oz, dx, dy, dz, maxDist) =>
        {
            var t = new Table(vm);
            t["hit"] = false;
            try
            {
                var origin = new Vector3((float)ox, (float)oy, (float)oz);
                var direction = new Vector3((float)dx, (float)dy, (float)dz);
                if (Physics.Raycast(new Ray(origin, direction), out var hitInfo, (float)maxDist))
                {
                    t["hit"] = true;
                    t["handle"] = gregLuaObjectHandleRegistry.Register(hitInfo.collider.gameObject);
                    var pt = new Table(vm);
                    pt["x"] = (double)hitInfo.point.x;
                    pt["y"] = (double)hitInfo.point.y;
                    pt["z"] = (double)hitInfo.point.z;
                    t["point"] = pt;
                }
            }
            catch { }
            return t;
        });

        // greg.unity.camera_ray() -> { ox, oy, oz, dx, dy, dz } from gregMain camera
        unity["camera_ray"] = (Func<Table>)(() =>
        {
            var t = new Table(vm);
            try
            {
                var cam = Camera.main;
                if (cam != null)
                {
                    var pos = cam.transform.position;
                    var fwd = cam.transform.forward;
                    t["ox"] = (double)pos.x; t["oy"] = (double)pos.y; t["oz"] = (double)pos.z;
                    t["dx"] = (double)fwd.x; t["dy"] = (double)fwd.y; t["dz"] = (double)fwd.z;
                }
            }
            catch { }
            return t;
        });
    }

    private static void RegisterGameObject(Script vm, Table unity)
    {
        // greg.unity.create_gameobject(name, parentHandle) -> handle
        unity["create_gameobject"] = (Func<string, int, int>)((name, parentHandle) =>
        {
            try
            {
                var go = new GameObject(name);
                var parent = gregLuaObjectHandleRegistry.Resolve(parentHandle);
                Transform pt = null;
                if (parent is GameObject pgo) pt = pgo.transform;
                else if (parent is Component pc) pt = pc.transform;
                if (pt != null) go.transform.SetParent(pt, true);
                return gregLuaObjectHandleRegistry.Register(go);
            }
            catch { return 0; }
        });

        // greg.unity.set_name(handle, name)
        unity["set_name"] = (Action<int, string>)((handle, name) =>
        {
            try
            {
                var obj = gregLuaObjectHandleRegistry.Resolve(handle);
                if (obj is GameObject go) go.name = name;
                else if (obj is Component c) c.gameObject.name = name;
            }
            catch { }
        });

        // greg.unity.get_name(handle) -> string
        unity["get_name"] = (Func<int, string>)(handle =>
        {
            try
            {
                var obj = gregLuaObjectHandleRegistry.Resolve(handle);
                if (obj is GameObject go) return go.name;
                if (obj is Component c) return c.gameObject?.name;
                return null;
            }
            catch { return null; }
        });

        // greg.unity.get_parent_component(handle, typeName) -> handle
        unity["get_parent_component"] = (Func<int, string, int>)((handle, typeName) =>
        {
            try
            {
                var obj = gregLuaObjectHandleRegistry.Resolve(handle);
                Component src = null;
                if (obj is GameObject go) src = go.transform;
                else if (obj is Component c) src = c;
                if (src == null) return 0;

                var type = ResolveIl2CppType(typeName);
                if (type == null) return 0;
                var comp = src.GetComponentInParent(Il2CppInterop.Runtime.Il2CppType.From(type));
                return comp != null ? gregLuaObjectHandleRegistry.Register(comp) : 0;
            }
            catch { return 0; }
        });

        // greg.unity.get_children_components(handle, typeName) -> table of handles
        unity["get_children_components"] = (Func<int, string, Table>)((handle, typeName) =>
        {
            var result = new Table(vm);
            try
            {
                var obj = gregLuaObjectHandleRegistry.Resolve(handle);
                Component src = null;
                if (obj is GameObject go) src = go.transform;
                else if (obj is Component c) src = c;
                if (src == null) return result;

                var type = ResolveIl2CppType(typeName);
                if (type == null) return result;
                var comps = src.GetComponentsInChildren(Il2CppInterop.Runtime.Il2CppType.From(type), true);
                if (comps == null) return result;
                for (int i = 0; i < comps.Count; i++)
                {
                    if (comps[i] != null)
                        result.Append(DynValue.NewNumber(gregLuaObjectHandleRegistry.Register(comps[i])));
                }
            }
            catch { }
            return result;
        });
    }

    private static void RegisterColor(Script vm, Table color)
    {
        // greg.color.to_hex(r, g, b) -> "#RRGGBB"
        color["to_hex"] = (Func<double, double, double, string>)((r, g, b) =>
            ColorToHex(new Color((float)r, (float)g, (float)b)));

        // greg.color.normalize_hex(raw) -> "#RRGGBB" or nil
        color["normalize_hex"] = (Func<string, string>)(raw =>
        {
            if (string.IsNullOrWhiteSpace(raw)) return null;
            var s = raw.Trim();
            if (!s.StartsWith("#")) s = "#" + s;
            if (ColorUtility.TryParseHtmlString(s, out var c))
                return ColorToHex(c);
            return null;
        });

        // greg.color.parse(hex) -> {r, g, b}
        color["parse"] = (Func<string, Table>)(hex =>
        {
            var t = new Table(vm);
            if (!string.IsNullOrEmpty(hex))
            {
                var s = hex.Trim();
                if (!s.StartsWith("#")) s = "#" + s;
                if (ColorUtility.TryParseHtmlString(s, out var c))
                {
                    t["r"] = (double)c.r; t["g"] = (double)c.g; t["b"] = (double)c.b;
                }
            }
            return t;
        });
    }

    private static void RegisterConfig(Script vm, Table config)
    {
        // greg.config.load(path) -> table of {key = value}
        config["load"] = (Func<string, Table>)(path =>
        {
            var t = new Table(vm);
            try
            {
                if (!File.Exists(path)) return t;
                foreach (var line in File.ReadAllLines(path))
                {
                    if (string.IsNullOrWhiteSpace(line)) continue;
                    var trimmed = line.Trim();
                    if (trimmed.StartsWith("#")) continue;
                    var eq = trimmed.IndexOf('=');
                    if (eq <= 0 || eq >= trimmed.Length - 1) continue;
                    var key = trimmed.Substring(0, eq).Trim();
                    var val = trimmed.Substring(eq + 1).Trim();
                    if (float.TryParse(val, NumberStyles.Float, CultureInfo.InvariantCulture, out var num))
                        t[key] = (double)num;
                    else
                        t[key] = val;
                }
            }
            catch { }
            return t;
        });

        // greg.config.save(path, table)
        config["save"] = (Action<string, Table>)((path, data) =>
        {
            try
            {
                var dir = Path.GetDirectoryName(path);
                if (!string.IsNullOrWhiteSpace(dir)) Directory.CreateDirectory(dir);
                var lines = new List<string>();
                foreach (var pair in data.Pairs)
                {
                    var key = pair.Key.CastToString();
                    var val = pair.Value.Type == DataType.Number
                        ? pair.Value.Number.ToString(CultureInfo.InvariantCulture)
                        : pair.Value.CastToString();
                    lines.Add($"{key}={val}");
                }
                File.WriteAllLines(path, lines);
            }
            catch { }
        });

        // greg.config.userdata_path() -> string
        config["userdata_path"] = (Func<string>)(() =>
            MelonLoader.Utils.MelonEnvironment.UserDataDirectory);
    }

    private static void RegisterGui(Script vm, Table gui)
    {
        // greg.gui functions are only valid inside on_gui callbacks.
        // The LuaLanguageBridge calls on_gui Lua functions inside Unity's OnGUI.

        gui["box"] = (Action<double, double, double, double, string>)((x, y, w, h, title) =>
            GUI.Box(new Rect((float)x, (float)y, (float)w, (float)h), title ?? ""));

        gui["label"] = (Action<double, double, double, double, string>)((x, y, w, h, text) =>
            GUI.Label(new Rect((float)x, (float)y, (float)w, (float)h), text ?? ""));

        gui["button"] = (Func<double, double, double, double, string, bool>)((x, y, w, h, text) =>
            GUI.Button(new Rect((float)x, (float)y, (float)w, (float)h), text ?? ""));

        gui["toggle"] = (Func<double, double, double, double, bool, string, bool>)((x, y, w, h, value, text) =>
            GUI.Toggle(new Rect((float)x, (float)y, (float)w, (float)h), value, text ?? ""));

        gui["screen_width"] = (Func<double>)(() => Screen.width);
        gui["screen_height"] = (Func<double>)(() => Screen.height);
    }

    // --- helpers ---

    private static string ColorToHex(Color c)
    {
        int r = Mathf.Clamp(Mathf.RoundToInt(c.r * 255f), 0, 255);
        int g = Mathf.Clamp(Mathf.RoundToInt(c.g * 255f), 0, 255);
        int b = Mathf.Clamp(Mathf.RoundToInt(c.b * 255f), 0, 255);
        return $"#{r:X2}{g:X2}{b:X2}";
    }

    private static Type ResolveIl2CppType(string typeName)
    {
        if (string.IsNullOrWhiteSpace(typeName)) return null;

        // Try Il2Cpp namespace first
        var t = Type.GetType($"Il2Cpp.{typeName}, Assembly-CSharp");
        if (t != null) return t;

        // Try Il2CppTMPro
        t = Type.GetType($"Il2CppTMPro.{typeName}, Il2CppTMPro");
        if (t != null) return t;

        // Try UnityEngine
        t = Type.GetType($"UnityEngine.{typeName}, UnityEngine.CoreModule");
        if (t != null) return t;

        // Try fully qualified
        foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
        {
            t = asm.GetType(typeName);
            if (t != null) return t;
        }
        return null;
    }

    private static object GetMemberValue(object obj, string name)
    {
        if (obj == null || string.IsNullOrEmpty(name)) return null;
        var member = FindMember(obj.GetType(), name);
        if (member is FieldInfo fi) return fi.GetValue(obj);
        if (member is PropertyInfo pi && pi.GetIndexParameters().Length == 0) return pi.GetValue(obj);
        return null;
    }

    private static void SetMemberValue(object obj, string name, object value)
    {
        if (obj == null || string.IsNullOrEmpty(name)) return;
        var member = FindMember(obj.GetType(), name);
        if (member is FieldInfo fi) fi.SetValue(obj, value);
        else if (member is PropertyInfo pi && pi.CanWrite) pi.SetValue(obj, value);
    }

    private static MemberInfo FindMember(Type type, string name)
    {
        var fi = type.GetField(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        if (fi != null) return fi;
        var pi = type.GetProperty(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        return pi;
    }
}






