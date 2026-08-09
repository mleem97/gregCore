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

## 2024-10-24 - Defense-in-Depth for File Operations
**Vulnerability:** Path traversal vulnerability in `SetPortrait` where `employeeId` was used directly in `Path.Combine` without validation, relying entirely on upstream validation during registration.
**Learning:** Even when inputs like `employeeId` are validated at creation/registration, any subsequent methods that consume them for file system operations must independently validate them to protect against direct calls, missing validations, or deserialization bypasses. Additionally, security checks must not use early returns if fallback logic (like default UI colors) is present later in the method.
**Prevention:** Always validate identifiers used in file paths at the point of use using `IndexOfAny(Path.GetInvalidFileNameChars()) >= 0 || Contains("..")`. Wrap vulnerable operations in conditional blocks rather than returning early to ensure fallback state is correctly applied.
