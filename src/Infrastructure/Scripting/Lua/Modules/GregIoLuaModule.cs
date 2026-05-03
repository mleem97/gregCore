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
                MelonLogger.Error($"[LuaMod:{modId}] io.read_file('{path}') failed: {ex.Message}");
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
                MelonLogger.Error($"[LuaMod:{modId}] io.write_file('{path}') failed: {ex.Message}");
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
                MelonLogger.Error($"[LuaMod:{modId}] io.append_file('{path}') failed: {ex.Message}");
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
                MelonLogger.Error($"[LuaMod:{modId}] io.delete_file('{path}') failed: {ex.Message}");
            }
        });

        // greg.io.list_files(pattern?) → table of strings
        ioTable["list_files"] = (Func<string?, Table>)(pattern =>
        {
            try
            {
                var files = Directory.GetFiles(dataDir, pattern ?? "*.*", SearchOption.AllDirectories)
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
                MelonLogger.Error($"[LuaMod:{modId}] io.list_files failed: {ex.Message}");
                return new Table(script);
            }
        });

        // greg.io.data_dir → string (read-only)
        ioTable["data_dir"] = dataDir.Replace('\\', '/');

        greg["io"] = ioTable;
    }

    /// <summary>
    /// Löst einen relativen Pfad auf und validiert, dass er innerhalb des Data-Dirs liegt.
    /// </summary>
    private static string ResolveSafe(string dataDir, string relativePath)
    {
        if (string.IsNullOrWhiteSpace(relativePath))
            throw new InvalidOperationException("Path cannot be empty");

        // Prevent path traversal
        string normalized = relativePath.Replace('/', Path.DirectorySeparatorChar);
        string fullPath = Path.GetFullPath(Path.Combine(dataDir, normalized));
        string dataDirFull = Path.GetFullPath(dataDir);

        // Fix for path traversal prefix match vulnerability
        if (!dataDirFull.EndsWith(Path.DirectorySeparatorChar.ToString()))
        {
            dataDirFull += Path.DirectorySeparatorChar;
        }

        if (!fullPath.StartsWith(dataDirFull, StringComparison.OrdinalIgnoreCase))
            throw new UnauthorizedAccessException($"Access denied: path escapes sandbox ('{relativePath}')");

        return fullPath;
    }
}
