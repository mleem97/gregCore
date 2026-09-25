/// <file-summary>
/// Layer:       Infrastructure (IO)
/// Purpose:     Sole owner of `.deactivated` moves. Only gregCore may call
///              this (explicit user activation/deactivation in the Mod-Hub).
///              Plugins/mods must never load from `.deactivated` directly —
///              activation always means moving the file/dir out first.
/// Maintainer:  File moves only; loaders never bypass the guard.
/// </file-summary>

namespace gregCore.Infrastructure.IO;

public static class GregModActivationService
{
    /// <summary>
    /// User activates a mod: moves `<parent>/.deactivated/<name>` to
    /// `<parent>/<name>`. No load happens in place.
    /// </summary>
    public static string Activate(string deactivatedPath)
    {
        if (string.IsNullOrWhiteSpace(deactivatedPath))
            throw new ArgumentException("Path must not be empty.", nameof(deactivatedPath));
        if (!GregDeactivatedGuard.IsDeactivatedPath(deactivatedPath))
            throw new InvalidOperationException($"Not inside a '{GregDeactivatedGuard.DeactivatedFolderName}' folder: {deactivatedPath}");

        var full = Path.GetFullPath(deactivatedPath);
        var fileOrDirExists = File.Exists(full) || Directory.Exists(full);
        if (!fileOrDirExists)
            throw new FileNotFoundException($"Deactivated mod not found: {full}");

        // <parent>/.deactivated/<name> [possibly deeper] -> <parent>/<remainder>
        var segments = full.Split(
            new[] { Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar },
            StringSplitOptions.None);
        var idx = Array.FindLastIndex(segments, s => string.Equals(
            s, GregDeactivatedGuard.DeactivatedFolderName, StringComparison.OrdinalIgnoreCase));
        if (idx <= 0)
            throw new InvalidOperationException($"Cannot resolve activation target for: {full}");

        var parent = string.Join(Path.DirectorySeparatorChar.ToString(), segments.Take(idx));
        var containingDir = parent;
        if (string.IsNullOrEmpty(containingDir) && Path.IsPathRooted(full))
            containingDir = Path.GetPathRoot(full) ?? string.Empty;
        var remainder = string.Join(Path.DirectorySeparatorChar.ToString(), segments.Skip(idx + 1));
        var target = Path.Combine(containingDir, remainder);

        if (string.Equals(full, target, OperatingSystem.IsWindows()
                ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal))
            throw new InvalidOperationException($"Activation target equals source: {full}");

        if (File.Exists(full))
        {
            var targetDir = Path.GetDirectoryName(target);
            if (!string.IsNullOrEmpty(targetDir)) Directory.CreateDirectory(targetDir);
            if (File.Exists(target) || Directory.Exists(target))
                throw new IOException($"Activation target already exists: {target}");
            File.Move(full, target);
            return target;
        }
        else
        {
            if (File.Exists(target) || Directory.Exists(target))
                throw new IOException($"Activation target already exists: {target}");
            var targetParent = Path.GetDirectoryName(target.TrimEnd(Path.DirectorySeparatorChar));
            if (!string.IsNullOrEmpty(targetParent)) Directory.CreateDirectory(targetParent);
            Directory.Move(full, target);
            return target;
        }
    }

    /// <summary>
    /// User deactivates a mod: moves `<parent>/<name>` to
    /// `<parent>/.deactivated/<name>`.
    /// </summary>
    public static string Deactivate(string activePath)
    {
        if (string.IsNullOrWhiteSpace(activePath))
            throw new ArgumentException("Path must not be empty.", nameof(activePath));
        if (GregDeactivatedGuard.IsDeactivatedPath(activePath))
            throw new InvalidOperationException($"Already inside a '{GregDeactivatedGuard.DeactivatedFolderName}' folder: {activePath}");

        var full = Path.GetFullPath(activePath.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar));
        var isFile = File.Exists(full);
        var isDir = !isFile && Directory.Exists(full);
        if (!isFile && !isDir)
            throw new FileNotFoundException($"Mod not found: {full}");

        var parentDir = Path.GetDirectoryName(full)
            ?? throw new InvalidOperationException($"Cannot resolve parent for: {full}");
        var name = Path.GetFileName(full);
        var target = Path.Combine(parentDir, GregDeactivatedGuard.DeactivatedFolderName, name);
        Directory.CreateDirectory(Path.Combine(parentDir, GregDeactivatedGuard.DeactivatedFolderName));
        if (File.Exists(target) || Directory.Exists(target))
            throw new IOException($"Deactivation target already exists: {target}");

        if (isFile) File.Move(full, target);
        else Directory.Move(full, target);
        return target;
    }
}
