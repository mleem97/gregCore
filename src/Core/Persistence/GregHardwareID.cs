using System;
using UnityEngine;
using Il2CppInterop.Runtime.Injection;
using System.Collections.Generic;

namespace gregCore.Core.Persistence
{
    /// <summary>
    /// Ensures every spawned object has a unique, deterministic ID that survives save/load cycles.
    /// </summary>
    public class GregHardwareID : MonoBehaviour
    {
        public GregHardwareID(IntPtr ptr) : base(ptr) { }

        public string UniqueID = "";
        public string PrefabID = "";

        public void Initialize(string prefabId, string existingGuid = null)
        {
            UniqueID = string.IsNullOrEmpty(existingGuid) ? Guid.NewGuid().ToString() : existingGuid;
            PrefabID = prefabId;
            HardwareIDManager.Register(this);
        }

        private void OnDestroy() => HardwareIDManager.Unregister(UniqueID);
    }

    public static class HardwareIDManager
    {
        private static readonly Dictionary<string, GameObject> _registry = new();

        public static void Register(GregHardwareID hw) => _registry[hw.UniqueID] = hw.gameObject;
        public static void Unregister(string guid) => _registry.Remove(guid);
        public static GameObject Get(string guid) => _registry.TryGetValue(guid, out var go) ? go : null;
        public static IReadOnlyDictionary<string, GameObject> GetAllHardware() => _registry;
    }
}
