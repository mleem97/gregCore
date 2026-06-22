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
            // Already loaded?
            if (_cache.TryGetValue(modulePath, out var cached))
                return cached;

            string? fullPath = ResolvePath(modulePath);
            if (fullPath == null)
            {
                throw new ScriptRuntimeException($"Module not found: {modulePath}");
            }

            ValidateSandbox(fullPath);

            string code = File.ReadAllText(fullPath);

            // Execute the module in the same Script, but with an isolated environment table
            // to avoid cross-Script DynValue issues while still reusing the same runtime.
            var moduleEnv = new Table(_script);

            // Copy allowed globals (but not greg table or original require to isolate modules)
            foreach (var pair in _script.Globals.Pairs)
            {
                if (pair.Key.String != "greg" && pair.Key.String != "require")
                    moduleEnv[pair.Key] = pair.Value;
            }

            // Module gets its own require() pointing to same resolver
            moduleEnv["require"] = (Func<string, DynValue>)Require;

            // Execute the module code using the isolated environment
            DynValue result = _script.DoString(code, moduleEnv);

            // Cache the result (prefer return value, fallback to module table)
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
        // @shared/ prefix → global shared folder
        if (modulePath.StartsWith("@shared/"))
        {
            string relative = modulePath.Substring(8).Replace('.', '/');
            return TryFindFile(Path.Combine(_sharedRoot, relative));
        }

        // Relative to mod root
        string localPath = modulePath.Replace('.', '/');
        return TryFindFile(Path.Combine(_modRoot, localPath))
            ?? TryFindFile(Path.Combine(_modRoot, "modules", localPath));
    }

    private static string? TryFindFile(string basePath)
    {
        string[] candidates = { basePath + ".lua", basePath + "/init.lua", basePath };
        foreach (var c in candidates)
        {
            if (File.Exists(c)) return c;
        }
        return null;
    }

    private void ValidateSandbox(string fullPath)
    {
        string normalized = Path.GetFullPath(fullPath);
        string modNormalized = Path.GetFullPath(_modRoot);
        if (!modNormalized.EndsWith(Path.DirectorySeparatorChar.ToString()))
            modNormalized += Path.DirectorySeparatorChar;

        string sharedNormalized = Path.GetFullPath(_sharedRoot);
        if (!sharedNormalized.EndsWith(Path.DirectorySeparatorChar.ToString()))
            sharedNormalized += Path.DirectorySeparatorChar;

        if (!normalized.StartsWith(modNormalized, StringComparison.OrdinalIgnoreCase)
            && !normalized.StartsWith(sharedNormalized, StringComparison.OrdinalIgnoreCase))
        {
            throw new UnauthorizedAccessException(
                $"Sandbox violation: Cannot load module outside mod directories: {fullPath}");
        }
    }
}
