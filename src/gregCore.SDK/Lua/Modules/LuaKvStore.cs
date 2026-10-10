/// <file-summary>
/// Layer:       Infrastructure
/// Purpose:      File-backed string key-value store for Lua modules
///               (config/save). One JSON file per store, immediate
///               writes on every change, robust against corrupt files.
/// Maintainer:   Only this file knows the file format (flat object).
/// </file-summary>

using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using MelonLoader;

namespace gregCore.Infrastructure.Scripting.Lua.Modules;

internal sealed class LuaKvStore
{
    private readonly string _filePath;
    private readonly string _modId;
    private readonly string _storeName;
    private readonly Dictionary<string, string> _data =
        new Dictionary<string, string>(StringComparer.Ordinal);
    private readonly object _gate = new object();

    internal LuaKvStore(string modId, string storeName, string dataDir, string fileName)
    {
        _modId = modId ?? "unknown";
        _storeName = storeName ?? "store";
        _filePath = Path.Combine(dataDir ?? ".", fileName ?? "store.json");
        Load();
    }

    internal string Get(string key, string defaultValue = null)
    {
        try
        {
            if (string.IsNullOrEmpty(key)) return defaultValue;
            lock (_gate)
            {
                string value;
                return _data.TryGetValue(key, out value) ? value : defaultValue;
            }
        }
        catch { return defaultValue; }
    }

    internal void Set(string key, string value)
    {
        try
        {
            if (string.IsNullOrEmpty(key)) return;
            lock (_gate) { _data[key] = value ?? ""; }
            Save();
        }
        catch (Exception ex)
        {
            LuaLog.Error($"[LuaMod:{_modId}] {_storeName}.set('{key}') failed: {ex.Message}");
        }
    }

    internal bool Delete(string key)
    {
        try
        {
            if (string.IsNullOrEmpty(key)) return false;
            bool removed;
            lock (_gate) { removed = _data.Remove(key); }
            if (removed) Save();
            return removed;
        }
        catch { return false; }
    }

    internal bool Has(string key)
    {
        try
        {
            if (string.IsNullOrEmpty(key)) return false;
            lock (_gate) { return _data.ContainsKey(key); }
        }
        catch { return false; }
    }

    internal List<string> Keys()
    {
        try
        {
            lock (_gate) { return new List<string>(_data.Keys); }
        }
        catch { return new List<string>(); }
    }

    internal bool SaveNow()
    {
        try { Save(); return true; }
        catch (Exception ex)
        {
            LuaLog.Error($"[LuaMod:{_modId}] {_storeName} save failed: {ex.Message}");
            return false;
        }
    }

    private void Load()
    {
        try
        {
            if (!File.Exists(_filePath)) return;
            string json = File.ReadAllText(_filePath);
            if (string.IsNullOrWhiteSpace(json)) return;
            var parsed = JsonSerializer.Deserialize<Dictionary<string, string>>(json);
            if (parsed == null) return;
            lock (_gate)
            {
                _data.Clear();
                foreach (var kv in parsed)
                {
                    if (!string.IsNullOrEmpty(kv.Key)) _data[kv.Key] = kv.Value ?? "";
                }
            }
        }
        catch (Exception ex)
        {
            LuaLog.Warning($"[LuaMod:{_modId}] {_storeName} load failed (starting empty): {ex.Message}");
            try
            {
                if (File.Exists(_filePath))
                    File.Copy(_filePath, _filePath + ".corrupt.bak", true);
            }
            catch { /* ignored: backup best-effort */ }
        }
    }

    private void Save()
    {
        try
        {
            string dir = Path.GetDirectoryName(_filePath);
            if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
                Directory.CreateDirectory(dir);
            Dictionary<string, string> snapshot;
            lock (_gate) { snapshot = new Dictionary<string, string>(_data, StringComparer.Ordinal); }
            string json = JsonSerializer.Serialize(snapshot, new JsonSerializerOptions { WriteIndented = true });
            string tmp = _filePath + ".tmp";
            File.WriteAllText(tmp, json);
            if (File.Exists(_filePath)) File.Delete(_filePath);
            File.Move(tmp, _filePath);
        }
        catch (Exception ex)
        {
            LuaLog.Error($"[LuaMod:{_modId}] {_storeName} write failed: {ex.Message}");
            throw;
        }
    }
}
