## 2024-04-26 - [Path Traversal in Persistence Layer]
**Vulnerability:** Path Traversal vulnerability in `src/Infrastructure/Config/GregPersistenceService.cs`.
**Learning:** Keys provided to the persistence service were interpolated directly into file paths without validation. If a user provided a key like `../../windows/system32/cmd`, it could allow reading or writing to arbitrary locations on the system.
**Prevention:** Validate file paths constructed from dynamic input to ensure they don't contain path traversal characters like `../`, invalid characters from `Path.GetInvalidFileNameChars()`, or directory separators (`Path.DirectorySeparatorChar`, `Path.AltDirectorySeparatorChar`).
