/// <file-summary>
/// Schicht:      Infrastructure
/// Zweck:        Sandboxed IO Funktionen für Lua.
/// Maintainer:   Darf nur auf {modDir}/data/ zugreifen.
///               greg.io.read_file(), write_file(), file_exists(), list_files(), delete_file()
/// </file-summary>

using System;
using System.IO;
using System.Linq;
using MoonSharp.Interpreter;
using MelonLoader;

namespace gregCore.Infrastructure.Scripting.Lua.Modules;

public static class GregIoLuaModule
{
    /// <summary>
    /// Registriert sandboxed I/O-Funktionen im greg.io Table.
    /// </summary>
    public static void Register(Table greg, Script script, string modId, string modDir)
    {
        MigrateLegacyDataDir(modDir, modId);
        string dataDir = Path.Combine(modDir, "data");
        Directory.CreateDirectory(dataDir);

        var ioTable = new Table(script);

        // greg.io.read_file(path) → string
        ioTable["read_file"] = (Func<string, string>)(path =>
        {
            try
            {
                string fullPath = ResolveSafe(dataDir, path);
                return File.ReadAllText(fullPath);
            }
            catch (Exception ex)
            {
                LuaLog.Error($"[LuaMod:{modId}] io.read_file('{path}') failed: {ex.Message}");
                return "";
            }
        });

        // greg.io.write_file(path, content)
        ioTable["write_file"] = (Action<string, string>)((path, content) =>
        {
            try
            {
                string fullPath = ResolveSafe(dataDir, path);
                string dir = Path.GetDirectoryName(fullPath)!;
                if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
                File.WriteAllText(fullPath, content);
            }
            catch (Exception ex)
            {
                LuaLog.Error($"[LuaMod:{modId}] io.write_file('{path}') failed: {ex.Message}");
            }
        });

        // greg.io.append_file(path, content)
        ioTable["append_file"] = (Action<string, string>)((path, content) =>
        {
            try
            {
                string fullPath = ResolveSafe(dataDir, path);
                File.AppendAllText(fullPath, content);
            }
            catch (Exception ex)
            {
                LuaLog.Error($"[LuaMod:{modId}] io.append_file('{path}') failed: {ex.Message}");
            }
        });

        // greg.io.file_exists(path) → bool
        ioTable["file_exists"] = (Func<string, bool>)(path =>
        {
            try
            {
                string fullPath = ResolveSafe(dataDir, path);
                return File.Exists(fullPath);
            }
            catch
            {
                return false;
            }
        });

        // greg.io.delete_file(path)
        ioTable["delete_file"] = (Action<string>)(path =>
        {
            try
            {
                string fullPath = ResolveSafe(dataDir, path);
                if (File.Exists(fullPath)) File.Delete(fullPath);
            }
            catch (Exception ex)
            {
                LuaLog.Error($"[LuaMod:{modId}] io.delete_file('{path}') failed: {ex.Message}");
            }
        });

        // greg.io.list_files(pattern?) → table of strings
        ioTable["list_files"] = (Func<string?, Table>)(pattern =>
        {
            try
            {
                // Sandbox: Pattern darf nicht aus dataDir ausbrechen.
                string safe = SanitizeSearchPattern(pattern);
                var files = Directory.GetFiles(dataDir, safe, SearchOption.AllDirectories)
                    .Where(f => IsInsideSandbox(dataDir, f))
                    .Select(f => Path.GetRelativePath(dataDir, f).Replace('\\', '/'))
                    .ToArray();

                var table = new Table(script);
                for (int i = 0; i < files.Length; i++)
                {
                    table[i + 1] = files[i]; // Lua arrays are 1-indexed
                }
                return table;
            }
            catch (Exception ex)
            {
                LuaLog.Error($"[LuaMod:{modId}] io.list_files failed: {ex.Message}");
                return new Table(script);
            }
        });

        // greg.io.data_dir → string (read-only)
        ioTable["data_dir"] = dataDir.Replace('\\', '/');

        // Aliases matching the public docs (same implementation, no drift).
        ioTable["read_text"] = ioTable.Get("read_file");
        ioTable["write_text"] = ioTable.Get("write_file");

        // greg.io.read_json(path) → table or nil
        ioTable["read_json"] = (Func<string, DynValue>)((path) =>
        {
            try
            {
                string fullPath = ResolveSafe(dataDir, path);
                if (!File.Exists(fullPath)) return DynValue.Nil;
                string text = File.ReadAllText(fullPath);
                if (string.IsNullOrWhiteSpace(text)) return DynValue.Nil;
                using (var doc = System.Text.Json.JsonDocument.Parse(text))
                {
                    return LuaJsonModule.FromJsonElement(script, doc.RootElement);
                }
            }
            catch (Exception ex)
            {
                LuaLog.Error($"[LuaMod:{modId}] io.read_json('{path}') failed: {ex.Message}");
                return DynValue.Nil;
            }
        });

        // greg.io.write_json(path, table) → bool
        ioTable["write_json"] = (Func<string, DynValue, bool>)((path, value) =>
        {
            try
            {
                string fullPath = ResolveSafe(dataDir, path);
                string dir = Path.GetDirectoryName(fullPath)!;
                if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
                object plain = LuaJsonModule.ToPlainObject(value);
                File.WriteAllText(fullPath, System.Text.Json.JsonSerializer.Serialize(plain,
                    new System.Text.Json.JsonSerializerOptions { WriteIndented = true }));
                return true;
            }
            catch (Exception ex)
            {
                LuaLog.Error($"[LuaMod:{modId}] io.write_json('{path}') failed: {ex.Message}");
                return false;
            }
        });

        greg["io"] = ioTable;
    }

    /// <summary>
    /// Einmalige Migration: fruehere Builds legten Dateien unter
    /// {modDir}/data/data ab (doppelter Pfad). Bestehende Dateien werden
    /// eine Ebene hoch kopiert (nie ueberschrieben, Originale bleiben).
    /// </summary>
    private static void MigrateLegacyDataDir(string modDir, string modId)
    {
        try
        {
            if (string.IsNullOrEmpty(modDir)) return;
            string legacy = Path.Combine(modDir, "data", "data");
            string current = Path.Combine(modDir, "data");
            if (!Directory.Exists(legacy)) return;
            foreach (string file in Directory.GetFiles(legacy, "*", SearchOption.AllDirectories))
            {
                string rel = Path.GetRelativePath(legacy, file).Replace('\\', '/');
                string dest = Path.Combine(current, rel);
                if (File.Exists(dest)) continue;
                string destDir = Path.GetDirectoryName(dest)!;
                if (!Directory.Exists(destDir)) Directory.CreateDirectory(destDir);
                File.Copy(file, dest);
            }
            MelonLogger.Msg($"[LuaMod:{modId}] migrated legacy data/data files up one level (originals kept).");
        }
        catch { /* ignored: migration best-effort */ }
    }

    /// <summary>
    /// Löst einen relativen Pfad auf und validiert, dass er innerhalb des Data-Dirs liegt.
    /// </summary>
    // Suchpattern darf kein Directory-Traversal enthalten (kein "..",
    // keine Pfadtrenner ausserhalb des Dateinamens). Wirft bei Missbrauch.
    private static string SanitizeSearchPattern(string pattern)
    {
        string safe = string.IsNullOrWhiteSpace(pattern) ? "*.*" : pattern;
        if (safe.Contains("..") ||
            safe.IndexOf(Path.DirectorySeparatorChar) >= 0 ||
            safe.IndexOf(Path.AltDirectorySeparatorChar) >= 0)
            throw new UnauthorizedAccessException("Search pattern escapes sandbox.");
        foreach (char c in Path.GetInvalidFileNameChars())
        {
            if (c == '*' || c == '?' || c == '[' || c == ']') continue;
            if (safe.IndexOf(c) >= 0)
                throw new UnauthorizedAccessException("Search pattern contains invalid characters.");
        }
        return safe;
    }

    private static bool IsInsideSandbox(string dataDir, string fullPath)
    {
        try
        {
            string root = Path.GetFullPath(dataDir);
            string sep = root.EndsWith(Path.DirectorySeparatorChar.ToString())
                ? root : root + Path.DirectorySeparatorChar;
            string full = Path.GetFullPath(fullPath);
            return full.Equals(root, StringComparison.OrdinalIgnoreCase) ||
                   full.StartsWith(sep, StringComparison.OrdinalIgnoreCase);
        }
        catch { return false; }
    }

    private static string ResolveSafe(string dataDir, string relativePath)    {
        if (string.IsNullOrWhiteSpace(relativePath))
            throw new InvalidOperationException("Path cannot be empty");

        // Prevent path traversal
        string normalized = relativePath.Replace('/', Path.DirectorySeparatorChar);
        string fullPath = Path.GetFullPath(Path.Combine(dataDir, normalized));
        string dataDirFull = Path.GetFullPath(dataDir);
        string dataDirWithSep = dataDirFull.EndsWith(Path.DirectorySeparatorChar.ToString())
            ? dataDirFull
            : dataDirFull + Path.DirectorySeparatorChar;

        if (!fullPath.Equals(dataDirFull, StringComparison.OrdinalIgnoreCase) &&
            !fullPath.StartsWith(dataDirWithSep, StringComparison.OrdinalIgnoreCase))
            throw new UnauthorizedAccessException($"Access denied: path escapes sandbox ('{relativePath}')");

        return fullPath;
    }
}
