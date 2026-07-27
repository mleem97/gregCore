## 2024-04-28 - Path Traversal Vulnerability in Persistence Service

**Vulnerability:** The `GregPersistenceService` used un-sanitized user input (`key`) to construct file paths for reading, writing, and deleting configuration files (`Path.Combine(_saveDirectory, $"{key}.json")`), leading to a critical Path Traversal (CWE-22) vulnerability.
**Learning:** The public API surface allowed callers to pass arbitrary keys (e.g. `../../Windows/System32/config/SAM`). This vulnerability was caused by blindly trusting user-provided file names.
**Prevention:** Always validate user input against path traversal attacks when constructing file paths dynamically. Ensure that the input does not contain directory separators (`Path.DirectorySeparatorChar`, `Path.AltDirectorySeparatorChar`, or `..`) or invalid filename characters (`Path.GetInvalidFileNameChars()`). Use a validation wrapper or helper method before applying `Path.Combine`.## 2024-04-26 - [Path Traversal in Persistence Layer]
**Vulnerability:** Path Traversal vulnerability in `src/Infrastructure/Config/GregPersistenceService.cs`.
**Learning:** Keys provided to the persistence service were interpolated directly into file paths without validation. If a user provided a key like `../../windows/system32/cmd`, it could allow reading or writing to arbitrary locations on the system.
**Prevention:** Validate file paths constructed from dynamic input to ensure they don't contain path traversal characters like `../`, invalid characters from `Path.GetInvalidFileNameChars()`, or directory separators (`Path.DirectorySeparatorChar`, `Path.AltDirectorySeparatorChar`).

## 2024-05-01 - Path Traversal in ModConfigSystem
**Vulnerability:** Path traversal vulnerability due to unsanitized `modId` in `GetConfigPath` in `src/Compatibility/DataCenterModLoader/ModConfigSystem.cs`.
**Learning:** Concatenating user input (like a `modId`) directly into `Path.Combine` allows for directory traversal attacks (`../`, etc.) leading to arbitrary file read/write issues.
**Prevention:** Validate input strings that form part of a file path before concatenating them. Reject them if they contain directory traversal characters like `..`, `Path.DirectorySeparatorChar`, `Path.AltDirectorySeparatorChar`, or any invalid filename characters (using `Path.GetInvalidFileNameChars()`).

## 2024-05-15 - Prefix-Bypass Path Traversal in Sandbox Validation
**Vulnerability:** Path traversal vulnerability in `src/Infrastructure/Scripting/Lua/LuaModuleLoader.cs` due to using a simple `StartsWith` check for directory containment.
**Learning:** Checking if a path starts with a directory path (e.g., `/mods/myMod`) without appending a trailing slash allows prefix bypasses, where a sibling directory with a similar name (e.g., `/mods/myMod_Secret`) incorrectly passes the boundary check.
**Prevention:** When validating sandbox paths to prevent prefix-matching path traversal bypasses, ensure the base directory string ends with `Path.DirectorySeparatorChar` before performing a `String.StartsWith` check, and also allow exact matches to the base directory itself.
