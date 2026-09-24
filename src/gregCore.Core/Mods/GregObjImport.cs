/// <file-summary>
/// Layer:       Core (Mods)
/// Purpose:     Safe wrapper around the vanilla ObjImporter (mod meshes from
///               .obj files): path validation (existence, extension, no
///               directory traversal, size limit), result checking
///               (non-null, vertexCount > 0). Never throws to callers.
/// </file-summary>

using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using MelonLoader;
using UnityEngine;

namespace gregCore.Core.Mods;

[ExcludeFromCodeCoverage(Justification = "Live Il2Cpp interop against game assemblies; needs running game.")]
public static class GregObjImport
{
    private const long MaxObjBytes = 64L * 1024L * 1024L;

    // Direct import with validation. Null on any error (with warning).
    public static Mesh ImportMesh(string filePath)
    {
        if (!TryValidatePath(filePath, out string full))
            return null;
        Mesh mesh = null;
        try { mesh = global::Il2Cpp.ObjImporter.ImportOBJ(full); }
        catch (Exception ex)
        {
            Warn($"Import failed ('{Short(full)}'): {Base(ex)}");
            return null;
        }
        if (mesh == null)
        {
            Warn($"Import lieferte null ('{Short(full)}').");
            return null;
        }
        int verts = -1;
        try { verts = mesh.vertexCount; } catch { verts = -1; }
        if (verts <= 0)
        {
            Warn($"Import ohne Vertices ('{Short(full)}').");
            return null;
        }
        return mesh;
    }

    public static bool TryImportMesh(string filePath, out Mesh mesh)
    {
        mesh = ImportMesh(filePath);
        return mesh != null;
    }

    // Convenience for ModPacks: model file relative to the pack folder, without
    // letting ".." escape the folder.
    public static Mesh ImportMeshForPack(string folderPath, string modelFile)
    {
        if (string.IsNullOrWhiteSpace(folderPath) || string.IsNullOrWhiteSpace(modelFile))
            return null;
        string combined = null;
        try
        {
            string root = Path.GetFullPath(folderPath);
            combined = Path.GetFullPath(Path.Combine(root, modelFile));
            if (!combined.StartsWith(root + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase)
                && !string.Equals(combined, root, StringComparison.OrdinalIgnoreCase))
            {
                Warn($"Pfad bricht aus dem Pack-Ordner aus ('{modelFile}').");
                return null;
            }
        }
        catch (Exception ex)
        {
            Warn($"Path combination failed: {Base(ex)}");
            return null;
        }
        return ImportMesh(combined);
    }

    private static bool TryValidatePath(string filePath, out string full)
    {
        full = null;
        if (string.IsNullOrWhiteSpace(filePath))
        {
            Warn("Leerer Dateipfad.");
            return false;
        }
        string candidate = filePath;
        try { candidate = Path.GetFullPath(filePath); } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
        if (!string.Equals(Path.GetExtension(candidate), ".obj", StringComparison.OrdinalIgnoreCase))
        {
            Warn($"No .obj path ('{Short(filePath)}').");
            return false;
        }
        FileInfo info = null;
        try { info = new FileInfo(candidate); } catch { info = null; }
        if (info == null || !info.Exists)
        {
            Warn($"File not found ('{Short(filePath)}').");
            return false;
        }
        try
        {
            if (info.Length > MaxObjBytes)
            {
                Warn($"Datei zu gross ({info.Length} Bytes, Limit {MaxObjBytes}): '{Short(filePath)}'.");
                return false;
            }
        }
        catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
        full = candidate;
        return true;
    }

    private static string Short(string path)
    {
        try
        {
            if (string.IsNullOrEmpty(path)) return "?";
            return path.Length > 80 ? "..." + path.Substring(path.Length - 77) : path;
        }
        catch { return "?"; }
    }

    private static string Base(Exception ex)
    {
        try { return ex != null ? ex.GetBaseException().Message : "?"; } catch { return "?"; }
    }

    private static void Warn(string message)
    {
        try { MelonLogger.Warning($"[gregCore][Mods] ObjImport: {message}"); } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
    }
}
