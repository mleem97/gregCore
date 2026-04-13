using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using MelonLoader;

namespace greg.Sdk.Services
{
    /// <summary>
    /// Zentrales Management für alle Harmony-Patches aus dem greg-Ökosystem.
    /// Verhindert Doppel-Patches, liefert Patch-Status pro Methode und sichert den Lifecycle.
    /// </summary>
    public static class GregHarmonyService
    {
        private static HarmonyLib.Harmony _harmony;
        private static readonly Dictionary<string, List<MethodBase>> _ownerPatches = new Dictionary<string, List<MethodBase>>();

        /// <summary>
        /// Initialisiert die Harmony-Instanz für den übergebenen Owner (z.B. "gregIPAM" oder "gregCore").
        /// </summary>
        public static void Initialize(string ownerId)
        {
            if (string.IsNullOrWhiteSpace(ownerId))
                throw new ArgumentException("ownerId must not be empty.", nameof(ownerId));

            if (_harmony == null)
            {
                _harmony = new HarmonyLib.Harmony($"com.teamgreg.harmony.{ownerId}");
                MelonLogger.Msg($"[HarmonySvc] Initialized global Harmony instance for {ownerId}");
            }
            
            if (!_ownerPatches.ContainsKey(ownerId))
            {
                _ownerPatches[ownerId] = new List<MethodBase>();
            }
        }

        /// <summary>
        /// Führt einen sicheren Patch einer Zielmethode aus. Verhindert Abstürze durch saubere try/catch-Blöcke.
        /// </summary>
        public static bool PatchMethod(string targetClass,
                                       string methodName,
                                       string ownerId,
                                       HarmonyMethod prefix = null,
                                       HarmonyMethod postfix = null,
                                       HarmonyMethod transpiler = null)
        {
            if (_harmony == null)
            {
                MelonLogger.Error($"[HarmonySvc] Cannot patch method {targetClass}.{methodName}. Initialize() was not called.");
                return false;
            }

            try
            {
                // Versuche, den Typ im aktuellen Assembly (oder Unity-Assemblies) zu finden
                var type = AccessTools.TypeByName(targetClass);
                if (type == null)
                {
                    MelonLogger.Error($"[HarmonySvc] Target class '{targetClass}' not found.");
                    return false;
                }

                var method = AccessTools.Method(type, methodName);
                if (method == null)
                {
                    MelonLogger.Error($"[HarmonySvc] Target method '{methodName}' in class '{targetClass}' not found.");
                    return false;
                }

                // Patch ausführen
                _harmony.Patch(method, prefix, postfix, transpiler);

                // Tracking für UnpatchAll
                if (!_ownerPatches.ContainsKey(ownerId))
                {
                    _ownerPatches[ownerId] = new List<MethodBase>();
                }
                
                if (!_ownerPatches[ownerId].Contains(method))
                {
                    _ownerPatches[ownerId].Add(method);
                }

                MelonLogger.Msg($"[HarmonySvc] Successfully patched {targetClass}.{methodName} for owner '{ownerId}'");
                return true;
            }
            catch (Exception ex)
            {
                MelonLogger.Error($"[HarmonySvc] Failed to patch {targetClass}.{methodName}:\n{ex}");
                return false;
            }
        }

        /// <summary>
        /// Entfernt alle Patches, die von einem bestimmten Owner registriert wurden.
        /// </summary>
        public static void UnpatchAll(string ownerId)
        {
            if (_harmony == null || string.IsNullOrWhiteSpace(ownerId)) return;

            if (_ownerPatches.TryGetValue(ownerId, out var patchedMethods))
            {
                foreach (var method in patchedMethods)
                {
                    _harmony.Unpatch(method, HarmonyPatchType.All, _harmony.Id);
                }
                patchedMethods.Clear();
                MelonLogger.Msg($"[HarmonySvc] Unpatched all methods for owner '{ownerId}'");
            }
        }

        /// <summary>
        /// Gibt eine Liste aller Methoden (Signaturen) zurück, die von einem Owner gepatcht wurden.
        /// </summary>
        public static List<string> GetPatchedMethods(string ownerId)
        {
            if (_ownerPatches.TryGetValue(ownerId, out var methods))
            {
                return methods.Select(m => $"{m.DeclaringType?.Name}.{m.Name}").ToList();
            }
            return new List<string>();
        }

        /// <summary>
        /// Prüft, ob eine bestimmte Methode (Signatur) durch den Owner bereits gepatcht ist.
        /// </summary>
        public static bool IsPatchedBy(string methodSig, string ownerId)
        {
            if (_ownerPatches.TryGetValue(ownerId, out var methods))
            {
                return methods.Any(m => $"{m.DeclaringType?.Name}.{m.Name}" == methodSig);
            }
            return false;
        }
    }
}
