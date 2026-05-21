## 2025-02-27 - Path Traversal bypass via StartsWith
**Vulnerability:** A path traversal vulnerability existed in `GregIoLuaModule.cs` where the prefix match (`StartsWith`) allowed escaping the sandbox. Because `dataDirFull` might not end in a directory separator, strings like `/base/data-secret/` could prefix-match `/base/data`.
**Learning:** `string.StartsWith` on paths is insufficient for security boundaries unless trailing path separators are strictly enforced.
**Prevention:** Always append directory separators (e.g., `Path.DirectorySeparatorChar`) to base path constants used in `StartsWith` checks, or prefer built-in path normalization APIs and strict equality matching where possible.

## 2025-02-27 - Path Traversal bypass via StartsWith
**Vulnerability:** A path traversal vulnerability existed in `GregIoLuaModule.cs` where the prefix match (`StartsWith`) allowed escaping the sandbox. Because `dataDirFull` might not end in a directory separator, strings like `/base/data-secret/` could prefix-match `/base/data`.
**Learning:** `string.StartsWith` on paths is insufficient for security boundaries unless trailing path separators are strictly enforced.
**Prevention:** Always append directory separators (e.g., `Path.DirectorySeparatorChar`) to base path constants used in `StartsWith` checks, or prefer built-in path normalization APIs and strict equality matching where possible.
