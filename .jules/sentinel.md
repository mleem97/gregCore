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

## 2024-07-01 - Prefix-Matching Sandbox Escape
**Vulnerability:** In `GregIoLuaModule.cs`, sandbox path validation used `fullPath.StartsWith(dataDirFull)` without ensuring `dataDirFull` had a trailing directory separator. This allowed escaping the intended `Mods/MyMod/data` directory into paths like `Mods/MyMod/data_secret`.
**Learning:** Prefix matching for paths is vulnerable if boundaries are not strictly defined by directory separators.
**Prevention:** Always append a trailing directory separator to the base directory before using `String.StartsWith` for path validation, and explicitly allow exact matches to the base directory itself.

## 2024-07-01 - Path Traversal in Mod Entity Registration
**Vulnerability:** `CustomEmployeeManager.Register` accepted arbitrary employee IDs without validation, which were later used directly in `Path.Combine` to construct image loading paths, enabling path traversal (CWE-22).
**Learning:** Identifiers provided by mods or external sources must be treated as untrusted input and validated before being used in file system operations.
**Prevention:** Validate input strings that form part of a file path before concatenating them. Reject them if they contain directory traversal characters like `..`, `Path.DirectorySeparatorChar`, `Path.AltDirectorySeparatorChar`, or any invalid filename characters (using `Path.GetInvalidFileNameChars()`).
## 2026-08-05 - Path Traversal in CustomEmployeeManager
**Vulnerability:** Path traversal in `CustomEmployeeManager.SetPortrait` allowed bypassing intended limits of `ModAssets` directory using `employeeId` via `.Contains("..")`.
**Learning:** Checking for traversal requires `IndexOfAny(Path.GetInvalidFileNameChars()) >= 0 || id.Contains("..")`. Even if checked during object registration, methods executing later or deserializing must perform independent input validation. Avoid using early return if fallback logic in the method is needed for UI consistency.
**Prevention:** Always sanitize paths before `Path.Combine` and ensure defense-in-depth where parameters are used in file system operations. When validating, wrap logic in `if/else` instead of early returns to retain fallback UI changes.
