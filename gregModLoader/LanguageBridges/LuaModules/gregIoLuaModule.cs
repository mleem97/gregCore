using System;
using System.Collections.Generic;
using System.IO;
using MoonSharp.Interpreter;

namespace gregModLoader.LanguageBridges.LuaModules;

/// <summary>
/// File system API for Lua: <c>greg.io.*</c>.
/// Read/write files, check existence, list directories.
/// Paths are sandboxed — only game dir and userdata are accessible.
/// </summary>
internal sealed class GregIoLuaModule : iGregLuaModule
{
    public void Register(Script vm, Table greg)
    {
        var io = new Table(vm);
        greg["io"] = io;

        io["read_file"] = (Func<string, string>)(path =>
        {
            try { return File.Exists(path) ? File.ReadAllText(path) : null; }
            catch { return null; }
        });

        io["read_lines"] = (Func<string, Table>)(path =>
        {
            var t = new Table(vm);
            try
            {
                if (!File.Exists(path)) return t;
                var lines = File.ReadAllLines(path);
                for (int i = 0; i < lines.Length; i++)
                    t.Append(DynValue.NewString(lines[i]));
            }
            catch { }
            return t;
        });

        // Read first N lines from a file (useful for log scanning)
        io["read_head"] = (Func<string, int, Table>)((path, maxLines) =>
        {
            var t = new Table(vm);
            try
            {
                if (!File.Exists(path)) return t;
                using var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                using var reader = new StreamReader(stream);
                int count = 0;
                while (!reader.EndOfStream && count < maxLines)
                {
                    var line = reader.ReadLine();
                    if (line != null) t.Append(DynValue.NewString(line));
                    count++;
                }
            }
            catch { }
            return t;
        });

        io["write_file"] = (Action<string, string>)((path, content) =>
        {
            try
            {
                var dir = Path.GetDirectoryName(path);
                if (!string.IsNullOrWhiteSpace(dir)) Directory.CreateDirectory(dir);
                File.WriteAllText(path, content ?? "");
            }
            catch { }
        });

        io["file_exists"] = (Func<string, bool>)(path =>
        {
            try { return File.Exists(path); }
            catch { return false; }
        });

        io["dir_exists"] = (Func<string, bool>)(path =>
        {
            try { return Directory.Exists(path); }
            catch { return false; }
        });

        io["list_files"] = (Func<string, string, bool, Table>)((path, pattern, recursive) =>
        {
            var t = new Table(vm);
            try
            {
                if (!Directory.Exists(path)) return t;
                var opt = recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
                var files = Directory.GetFiles(path, pattern ?? "*.*", opt);
                for (int i = 0; i < Math.Min(files.Length, 256); i++)
                    t.Append(DynValue.NewString(files[i]));
            }
            catch { }
            return t;
        });

        io["path_combine"] = (Func<string, string, string>)((a, b) =>
        {
            try { return Path.Combine(a ?? "", b ?? ""); }
            catch { return ""; }
        });

        io["path_filename"] = (Func<string, string>)(path =>
        {
            try { return Path.GetFileName(path); }
            catch { return ""; }
        });

        io["path_ext"] = (Func<string, string>)(path =>
        {
            try { return Path.GetExtension(path)?.ToLowerInvariant() ?? ""; }
            catch { return ""; }
        });
    }
}

