## 2024-04-24 - Optimized Object Tracking in IL2CPP
**Learning:** `UnityEngine.Object.FindObjectsOfType<T>` is extremely expensive in large scenes as it scans the entire object hierarchy. In IL2CPP environments, frequent calls can cause noticeable hitches.
**Action:** Use a `HashSet<T>` to cache objects by hooking into their `Awake` and `OnDestroy` lifecycle methods via Harmony. Always prune destroyed instances by checking `obj.Pointer == IntPtr.Zero`.
## 2025-05-20 - Expensive OnUpdate Polling (FindObjectsOfType)
**Learning:** Found a major performance anti-pattern in `src/greg.QoL/Main.cs`: polling `FindObjectsOfType<T>()` on every frame inside `OnUpdate` while waiting for an object (like a UI element) to spawn. `FindObjectsOfType` is O(N) over all loaded objects and blocks the main thread, wasting immense CPU cycles before the condition is ever met.
**Action:** Throttle such condition checks using `Time.deltaTime` to run e.g. once per second (1Hz) instead of 60-144 times per second, or refactor to event-driven initialization.
## 2025-05-20 - Expensive OnUpdate Polling (NetworkMap)
**Learning:** Calling `NetworkMap.GetAllBrokenSwitches()` and `GetAllBrokenServers()` on every frame in `OnUpdate()` is a critical performance anti-pattern. These methods allocate arrays of components on the heap, which causes intense IL2CPP Garbage Collection pressure, especially in late-game scenarios with many devices.
**Action:** Throttle such network map queries using `Time.deltaTime` to run e.g. once per second (1Hz) instead of every frame.
## 2025-05-20 - Collection Modification in Loops
**Learning:** Removing defensive `.ToArray()` allocations to optimize collection enumerations causes `InvalidOperationException` if the loop body mutates the underlying collection (e.g., calling `RepairDevice()` which removes the item from the broken list).
**Action:** When trying to eliminate GC pressure from allocations on collections that will be mutated during iteration, use an early return (`if (collection.Count == 0) return;`) to avoid the allocation when the collection is empty, which is often the most common state, while keeping the defensive copy when work actually needs to be done.

## 2025-05-15 - [Security Fix: Weak Hashing]
**Learning:** MD5 should be avoided for ID generation or any cryptographic purpose. SHA256 is a more secure alternative. When a 16-byte Guid is needed, the first 16 bytes of a SHA256 hash can be used.
**Action:** Always prefer SHA256 over MD5 for deterministic ID generation.

## 2025-05-15 - [Environment: Dotnet Tooling Timeouts]
**Learning:** The `dotnet` CLI can time out in certain restricted environments during restore or build.
**Action:** Use `--no-restore` if dependencies are already present, or use background execution with log files if commands take longer than the session allows. Ensure junk files are cleaned up before submission.
## 2024-05-21 - Expensive Polling for Count Check in API
**Learning:** `UnityEngine.Object.FindObjectsOfType<T>` was being used in several global counts check via `GregAPI`, `GregServerModule`, `GregNetworkModule` and `GregNpcModule`. Finding objects of a type across the entire hierarchy is very expensive, especially as the number of devices or objects grow over time.
**Action:** Always prefer using global singleton collections managed by the game over calling `FindObjectsOfType<T>`. For example, use `Il2Cpp.NetworkMap.instance.servers` to get servers, `Il2Cpp.NetworkMap.instance.switches` for switches and `Il2Cpp.TechnicianManager.instance.technicians` to get technicians. Ensure null checks are present.
