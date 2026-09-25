/// <file-summary>
/// Layer:       Infrastructure
/// Purpose:      Sandboxed IO functions for Lua.
/// Maintainer:   May only access {modDir}/data/.
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
    /// Registers sandboxed I/O functions in the greg.io table.
    /// </summary>
    public static void Register(Table greg, Script script, string modId, string modDir)
    {
        MigrateLegacyDataDir(modDir, modId);
        string dataDir = Path.Combine(modDir, "data");
        Directory.CreateDirectory(dataDir);

        var ioTable = new Table(script);
        RegisterReadFile(ioTable, dataDir, modId);
        RegisterWriteFile(ioTable, dataDir, modId);
        RegisterAppendDelete(ioTable, dataDir, modId);
        RegisterFileExists(ioTable, dataDir, modId);
        RegisterListFiles(ioTable, dataDir, script, modId);
        RegisterReadJson(ioTable, dataDir, script, modId);
        RegisterWriteJson(ioTable, dataDir, modId);

        // Aliases matching the public docs (same implementation, no drift).
        ioTable["read_text"] = ioTable.Get("read_file");
        ioTable["write_text"] = ioTable.Get("write_file");

        ioTable["data_dir"] = dataDir.Replace('\\', '/');

        greg["io"] = ioTable;
    }

    private static void RegisterReadFile(Table t, string dataDir, string modId)
    {

        // greg.io.read_file(path) → string
        t["read_file"] = (Func<string, string>)(path =>
        {
            try
            {
                string fullPath = ResolveSafe(dataDir, path); // nosemgrep: csharp.lang.security.filesystem.unsafe-path-combine.unsafe-path-combine -- all file access routes through ResolveSafe (GetFullPath + sandbox prefix containment); dataDir is framework-resolved, never raw Lua input.
                return File.ReadAllText(fullPath);
            }
            catch (Exception ex)
            {
                LuaLog.Error($"[LuaMod:{modId}] io.read_file('{path}') failed: {ex.Message}");
                return "";
            }
        });
    }

    private static void RegisterWriteFile(Table t, string dataDir, string modId)
    {

        // greg.io.write_file(path, content)
        t["write_file"] = (Action<string, string>)((path, content) =>
        {
            try
            {
                string fullPath = ResolveSafe(dataDir, path); // nosemgrep: csharp.lang.security.filesystem.unsafe-path-combine.unsafe-path-combine -- all file access routes through ResolveSafe (GetFullPath + sandbox prefix containment); dataDir is framework-resolved, never raw Lua input.
                string dir = Path.GetDirectoryName(fullPath)!;
                if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
                File.WriteAllText(fullPath, content);
            }
            catch (Exception ex)
            {
                LuaLog.Error($"[LuaMod:{modId}] io.write_file('{path}') failed: {ex.Message}");
            }
        });
    }

    private static void RegisterAppendDelete(Table t, string dataDir, string modId)
    {
        // greg.io.append_file(path, content)
        t["append_file"] = (Action<string, string>)((path, content) =>
        {
            try
            {
                string fullPath = ResolveSafe(dataDir, path); // nosemgrep: csharp.lang.security.filesystem.unsafe-path-combine.unsafe-path-combine -- all file access routes through ResolveSafe (GetFullPath + sandbox prefix containment); dataDir is framework-resolved, never raw Lua input.
                File.AppendAllText(fullPath, content);
            }
            catch (Exception ex)
            {
                LuaLog.Error($"[LuaMod:{modId}] io.append_file('{path}') failed: {ex.Message}");
            }
        });

        // greg.io.delete_file(path)
        t["delete_file"] = (Action<string>)(path =>
        {
            try
            {
                string fullPath = ResolveSafe(dataDir, path); // nosemgrep: csharp.lang.security.filesystem.unsafe-path-combine.unsafe-path-combine -- all file access routes through ResolveSafe (GetFullPath + sandbox prefix containment); dataDir is framework-resolved, never raw Lua input.
                if (File.Exists(fullPath)) File.Delete(fullPath);
            }
            catch (Exception ex)
            {
                LuaLog.Error($"[LuaMod:{modId}] io.delete_file('{path}') failed: {ex.Message}");
            }
        });
    }

    private static void RegisterFileExists(Table t, string dataDir, string modId)
    {

        // greg.io.file_exists(path) → bool
        t["file_exists"] = (Func<string, bool>)(path =>
        {
            try
            {
                string fullPath = ResolveSafe(dataDir, path); // nosemgrep: csharp.lang.security.filesystem.unsafe-path-combine.unsafe-path-combine -- all file access routes through ResolveSafe (GetFullPath + sandbox prefix containment); dataDir is framework-resolved, never raw Lua input.
                return File.Exists(fullPath);
            }
            catch
            {
                return false;
            }
        });

        // greg.io.delete_file(path)
        t["delete_file"] = (Action<string>)(path =>
        {
            try
            {
                string fullPath = ResolveSafe(dataDir, path); // nosemgrep: csharp.lang.security.filesystem.unsafe-path-combine.unsafe-path-combine -- all file access routes through ResolveSafe (GetFullPath + sandbox prefix containment); dataDir is framework-resolved, never raw Lua input.
                if (File.Exists(fullPath)) File.Delete(fullPath);
            }
            catch (Exception ex)
            {
                LuaLog.Error($"[LuaMod:{modId}] io.delete_file('{path}') failed: {ex.Message}");
            }
        });
    }

    private static void RegisterListFiles(Table t, string dataDir, Script script, string modId)
    {

        // greg.io.list_files(pattern?) → table of strings
        t["list_files"] = (Func<string?, Table>)(pattern =>
        {
            try
            {
                // Sandbox: pattern must not escape dataDir.
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
    }

    private static void RegisterReadJson(Table t, string dataDir, Script script, string modId)
    {

        // greg.io.read_json(path) → table or nil
        t["read_json"] = (Func<string, DynValue>)((path) =>
        {
            try
            {
                string fullPath = ResolveSafe(dataDir, path); // nosemgrep: csharp.lang.security.filesystem.unsafe-path-combine.unsafe-path-combine -- all file access routes through ResolveSafe (GetFullPath + sandbox prefix containment); dataDir is framework-resolved, never raw Lua input.
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
    }

    private static void RegisterWriteJson(Table t, string dataDir, string modId)
    {

        // greg.io.write_json(path, table) → bool
        t["write_json"] = (Func<string, DynValue, bool>)((path, value) =>
        {
            try
            {
                string fullPath = ResolveSafe(dataDir, path); // nosemgrep: csharp.lang.security.filesystem.unsafe-path-combine.unsafe-path-combine -- all file access routes through ResolveSafe (GetFullPath + sandbox prefix containment); dataDir is framework-resolved, never raw Lua input.
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
    }

    /// <summary>
    /// One-time migration: earlier builds placed files under
    /// {modDir}/data/data (duplicate path). Existing files are
    /// copied up one level (never overwritten, originals kept).
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
    /// Resolves a relative path and validates that it lies inside the data dir.
    /// </summary>
    // Search pattern must not contain directory traversal (no "..",
    // no path separators outside the file name). Throws on misuse.
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
