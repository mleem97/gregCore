using System;
using System.Collections.Generic;
using System.IO;
using MoonSharp.Interpreter;
using MelonLoader;

namespace gregCore.Infrastructure.Scripting.Lua;

/// <summary>
/// Implements require()-System für Lua-Mods mit Sandbox-Checks.
/// Sucht im Mod-eigenen Verzeichnis und unterstützt @shared/ Präfix.
/// </summary>
public class LuaModuleLoader
{
    private readonly Script _script;
    private readonly string _modRoot;
    private readonly string _sharedRoot;
    private readonly Dictionary<string, DynValue> _cache = new();

    public LuaModuleLoader(Script script, string modRoot, string? globalSharedRoot = null)
    {
        _script = script;
        _modRoot = modRoot;
        _sharedRoot = globalSharedRoot ?? Path.Combine(
            global::MelonLoader.Utils.MelonEnvironment.GameRootDirectory,
            "Mods", "Scripts", "_shared");
    }

    /// <summary>
    /// Registriert require() global in der Lua-Instanz.
    /// </summary>
    public void Register()
    {
        _script.Globals["require"] = (Func<string, DynValue>)Require;
    }

    private DynValue Require(string modulePath)
    {
        try
        {
            if (_cache.TryGetValue(modulePath, out var cached))
                return cached;

            string? fullPath = ResolvePath(modulePath);
            if (fullPath == null)
                throw new ScriptRuntimeException($"Module not found: {modulePath}");

            ValidateSandbox(fullPath);

            string code = File.ReadAllText(fullPath);
            var moduleEnv = new Table(_script);

            foreach (var pair in _script.Globals.Pairs)
            {
                if (pair.Key.String != "greg" && pair.Key.String != "require")
                    moduleEnv[pair.Key] = pair.Value;
            }

            moduleEnv["require"] = (Func<string, DynValue>)Require;

            DynValue result = _script.DoString(code, moduleEnv);
            DynValue moduleReturn = result;
            _cache[modulePath] = moduleReturn ?? DynValue.Nil;
            return moduleReturn ?? DynValue.Nil;
        }
        catch (Exception ex)
        {
            MelonLogger.Error($"[LuaModuleLoader] require('{modulePath}') failed: {ex.Message}");
            throw new ScriptRuntimeException($"require('{modulePath}') failed: {ex.Message}");
        }
    }

    private string? ResolvePath(string modulePath)
    {
        if (modulePath.StartsWith("@shared/", StringComparison.Ordinal))
        {
            string relative = modulePath.Substring(8).Replace('.', '/');
            return TryFindFile(Path.Combine(_sharedRoot, relative));
        }

        string localPath = modulePath.Replace('.', '/');
        return TryFindFile(Path.Combine(_modRoot, localPath))
            ?? TryFindFile(Path.Combine(_modRoot, "modules", localPath));
    }

    private static string? TryFindFile(string basePath)
    {
        string[] candidates = { basePath + ".lua", basePath + "/init.lua", basePath };
        foreach (var candidate in candidates)
        {
            if (File.Exists(candidate)) return candidate;
        }
        return null;
    }

    private void ValidateSandbox(string fullPath)
    {
        string normalized = Path.GetFullPath(fullPath);
        string modNormalized = Path.GetFullPath(_modRoot);
        string sharedNormalized = Path.GetFullPath(_sharedRoot);

        if (!IsPathWithin(normalized, modNormalized) && !IsPathWithin(normalized, sharedNormalized))
        {
            throw new UnauthorizedAccessException(
                $"Sandbox violation: Cannot load module outside mod directories: {fullPath}");
        }
    }

    private static bool IsPathWithin(string candidatePath, string rootPath)
    {
        StringComparison comparison = OperatingSystem.IsWindows()
            ? StringComparison.OrdinalIgnoreCase
            : StringComparison.Ordinal;

        if (candidatePath.Equals(rootPath, comparison))
            return true;

        string rootWithSeparator = Path.EndsInDirectorySeparator(rootPath)
            ? rootPath
            : rootPath + Path.DirectorySeparatorChar;

        return candidatePath.StartsWith(rootWithSeparator, comparison);
    }
}
