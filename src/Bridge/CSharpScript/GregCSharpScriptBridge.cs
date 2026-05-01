using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using MelonLoader;
using gregCore.API;

namespace gregCore.Bridge.CSharpScript;

/// <summary>
/// Orchestrates discovery, compilation, and lifecycle of C# script mods.
/// </summary>
public sealed class GregCSharpScriptBridge
{
    private static readonly List<GregCSharpModContext> _mods = new();
    private static bool _initialized;

    public static void Initialize()
    {
        if (_initialized) return;

        string gameRoot = global::MelonLoader.Utils.MelonEnvironment.GameRootDirectory;
        string csharpDir = Path.Combine(gameRoot, "UserData", "gregCore", "Mods", "CSharp");

        if (!Directory.Exists(csharpDir))
            Directory.CreateDirectory(csharpDir);

        if (!GregCSharpCompiler.IsAvailable)
        {
            MelonLogger.Warning("[CSharpScriptBridge] Roslyn not available — C# script mods will not be loaded.");
            _initialized = true;
            return;
        }

        LoadMods(csharpDir);
        _initialized = true;
    }

    private static void LoadMods(string csharpDir)
    {
        foreach (string modDir in Directory.GetDirectories(csharpDir))
        {
            string modId = Path.GetFileName(modDir);
            if (modId.StartsWith("@") || modId.StartsWith(".")) continue;

            string[] csFiles = Directory.GetFiles(modDir, "*.cs", SearchOption.TopDirectoryOnly);
            if (csFiles.Length == 0) continue;

            try
            {
                Assembly? assembly = GregCSharpCompiler.Compile(modId, csFiles);
                if (assembly == null) continue;

                var modType = assembly.GetTypes()
                    .FirstOrDefault(t => typeof(IGregCSharpMod).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract);

                if (modType == null)
                {
                    MelonLogger.Warning($"[CSharpScriptBridge] Mod '{modId}' has no class implementing IGregCSharpMod.");
                    continue;
                }

                var instance = (IGregCSharpMod?)Activator.CreateInstance(modType);
                if (instance == null)
                {
                    MelonLogger.Warning($"[CSharpScriptBridge] Failed to instantiate mod '{modId}'.");
                    continue;
                }

                var context = new GregCSharpModContext
                {
                    Id = instance.ModId,
                    Directory = modDir,
                    Assembly = assembly,
                    Instance = instance
                };

                context.SafeCall(() => context.Instance.OnInit(), "OnInit");
                context.Initialized = true;
                _mods.Add(context);

                MelonLogger.Msg($"[CSharpScriptBridge] Loaded mod: {instance.ModName} v{instance.Version} ({instance.ModId})");
            }
            catch (ReflectionTypeLoadException ex)
            {
                MelonLogger.Error($"[CSharpScriptBridge] Type load error in mod '{modId}': {ex.Message}");
                if (ex.LoaderExceptions != null)
                {
                    foreach (var le in ex.LoaderExceptions.Take(3))
                    {
                        if (le != null)
                            MelonLogger.Error($"    -> {le.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                MelonLogger.Error($"[CSharpScriptBridge] Failed to load mod '{modId}': {ex.Message}");
            }
        }
    }

    public static void OnUpdate(float dt)
    {
        if (!_initialized) return;
        foreach (var mod in _mods)
        {
            if (!mod.Initialized) continue;
            mod.SafeCall(() => mod.Instance.OnUpdate(dt), "OnUpdate");
        }
    }

    public static void OnSceneLoaded(string sceneName)
    {
        if (!_initialized) return;
        foreach (var mod in _mods)
        {
            if (!mod.Initialized) continue;
            mod.SafeCall(() => mod.Instance.OnSceneLoaded(sceneName), "OnSceneLoaded");
        }
    }

    public static void Shutdown()
    {
        if (!_initialized) return;
        foreach (var mod in _mods)
        {
            mod.SafeCall(() => mod.Instance.OnShutdown(), "OnShutdown");
        }
        _mods.Clear();
        _initialized = false;
    }
}
