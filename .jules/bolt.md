## 2024-04-24 - Optimized Object Tracking in IL2CPP
**Learning:** `UnityEngine.Object.FindObjectsOfType<T>` is extremely expensive in large scenes as it scans the entire object hierarchy. In IL2CPP environments, frequent calls can cause noticeable hitches.
**Action:** Use a `HashSet<T>` to cache objects by hooking into their `Awake` and `OnDestroy` lifecycle methods via Harmony. Always prune destroyed instances by checking `obj.Pointer == IntPtr.Zero`.
