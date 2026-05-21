## 2024-05-24 - Cache Expensive GetComponentInChildren calls
**Learning:** Calling `GetComponentInChildren<T>(true)` frequently in an `Update` loop triggers significant hierarchy traversal and memory allocation under the hood in Unity/IL2CPP, causing severe CPU spikes, especially when scaling up the number of entities.
**Action:** Always fetch and cache components during instantiation (or lazily the first time) and store them in custom managed state rather than querying the scene hierarchy continuously in `Update()`.
