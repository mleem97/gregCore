## 2024-04-24 - Optimized Object Tracking in IL2CPP
**Learning:** `UnityEngine.Object.FindObjectsOfType<T>` is extremely expensive in large scenes as it scans the entire object hierarchy. In IL2CPP environments, frequent calls can cause noticeable hitches.
**Action:** Use a `HashSet<T>` to cache objects by hooking into their `Awake` and `OnDestroy` lifecycle methods via Harmony. Always prune destroyed instances by checking `obj.Pointer == IntPtr.Zero`.
## 2025-05-20 - Expensive OnUpdate Polling (FindObjectsOfType)
**Learning:** Found a major performance anti-pattern in `src/greg.QoL/Main.cs`: polling `FindObjectsOfType<T>()` on every frame inside `OnUpdate` while waiting for an object (like a UI element) to spawn. `FindObjectsOfType` is O(N) over all loaded objects and blocks the main thread, wasting immense CPU cycles before the condition is ever met.
**Action:** Throttle such condition checks using `Time.deltaTime` to run e.g. once per second (1Hz) instead of 60-144 times per second, or refactor to event-driven initialization.
## 2025-05-20 - Expensive OnUpdate Polling (NetworkMap)
**Learning:** Calling `NetworkMap.GetAllBrokenSwitches()` and `GetAllBrokenServers()` on every frame in `OnUpdate()` is a critical performance anti-pattern. These methods allocate arrays of components on the heap, which causes intense IL2CPP Garbage Collection pressure, especially in late-game scenarios with many devices.
**Action:** Throttle such network map queries using `Time.deltaTime` to run e.g. once per second (1Hz) instead of every frame.
