/// <file-summary>
/// Schicht:      GameApi
/// Zweck:        Laufzeit-Helfer für alle generierten Spiel-Typ-Module.
/// Maintainer:   Instanz-Suche und On-Demand-Methoden-Hooks (Postfix only).
/// </file-summary>

using System.Reflection;
using HarmonyLib;
using gregCore.Core.Abstractions;
using gregCore.Core.Events;
using gregCore.Core.Models;

namespace gregCore.GameApi;

public static class GregGameModuleHost
{
    internal static global::HarmonyLib.Harmony? Harmony { get; private set; }
    private static GregEventBus? _eventBus;
    private static IGregLogger? _logger;

    public static void Configure(global::HarmonyLib.Harmony harmony, GregEventBus eventBus, IGregLogger logger)
    {
        Harmony = harmony;
        _eventBus = eventBus;
        _logger = logger.ForContext("GameApi");
    }

    public static T? FindFirst<T>() where T : global::UnityEngine.Object
    {
        try
        {
            var all = global::UnityEngine.Resources.FindObjectsOfTypeAll<T>();
            if (all == null) return null;
            foreach (var obj in all)
            {
                try
                {
                    if (obj == null) continue;
                    if (obj is Il2CppInterop.Runtime.InteropTypes.Il2CppObjectBase il2cpp &&
                        il2cpp.Pointer == System.IntPtr.Zero) continue;
                    return obj;
                }
                catch { /* einzelnes Objekt überspringen */ }
            }
        }
        catch (Exception ex)
        {
            _logger?.Warning($"FindFirst<{typeof(T).Name}> failed: {ex.Message}");
        }
        return null;
    }

    public static bool HookMethod(global::HarmonyLib.Harmony? harmony, Type gameType, string methodName,
        GregEventBus? eventBus, IGregLogger? logger)
    {
        harmony ??= Harmony;
        eventBus ??= _eventBus;
        try
        {
            if (harmony == null || eventBus == null || gameType == null ||
                string.IsNullOrWhiteSpace(methodName)) return false;
            const BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic |
                                       BindingFlags.Instance | BindingFlags.Static;
            MethodBase? target = null;
            foreach (var m in gameType.GetMethods(flags))
            {
                if (m.Name == methodName && !m.IsConstructor && !m.IsSpecialName) { target = m; break; }
            }
            if (target == null)
            {
                logger?.Warning($"Hook target not found: {gameType.FullName}.{methodName}");
                return false;
            }
            harmony.Patch(target, postfix: new HarmonyMethod(typeof(GregGameModuleHost), nameof(GameApiPostfix)));
            return true;
        }
        catch (Exception ex)
        {
            (logger ?? _logger)?.Error($"Hook {gameType?.FullName}.{methodName} failed", ex);
            return false;
        }
    }

    public static void GameApiPostfix(MethodBase __originalMethod)
    {
        try
        {
            if (__originalMethod == null || _eventBus == null) return;
            var hookName = $"greg.game.{__originalMethod.DeclaringType?.FullName}.{__originalMethod.Name}";
            _eventBus.Publish(hookName, new EventPayload
            {
                HookName = hookName,
                OccurredAtUtc = DateTime.UtcNow,
                Data = new Dictionary<string, object>
                {
                    { "declaringType", __originalMethod.DeclaringType?.FullName ?? "unknown" },
                    { "method", __originalMethod.Name }
                }
            });
        }
        catch { /* Hooks dürfen das Spiel nie brechen */ }
    }
}
