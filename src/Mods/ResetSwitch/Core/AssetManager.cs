using System;
using System.IO;
using MelonLoader;
using MelonLoader.Utils;
using UnityEngine;

namespace greg.Mods.ResetSwitch.Core;

/// <summary>
/// Minimal standalone asset management for greg.Mods.ResetSwitch.
/// Uses a local 'assets' folder in the game's greg.Mods.ResetSwitch directory.
/// </summary>
public static class AssetManager
{
    private static string _basePath;
    private static string _backupPath;

    public static void Init()
    {
        _basePath = Path.Combine(MelonEnvironment.GameRootDirectory, "greg.Mods.ResetSwitch");
        _backupPath = Path.Combine(_basePath, "Backups");

        try
        {
            if (!Directory.Exists(_basePath)) Directory.CreateDirectory(_basePath);
            if (!Directory.Exists(_backupPath)) Directory.CreateDirectory(_backupPath);
            
            MelonLogger.Msg($"[AssetManager] Initialized at {_basePath}");
        }
        catch (Exception ex)
        {
            MelonLogger.Error($"[AssetManager] Initialization failed: {ex.Message}");
        }
    }

    public static string GetBackupPath(string slotName)
    {
        return Path.Combine(_backupPath, $"{slotName}_backup_{DateTime.Now:yyyyMMdd_HHmmss}.json");
    }

    /// <summary>
    /// Placeholder for future sprite/texture loading if needed.
    /// In standalone mode, we prefer using built-in Unity primitives or simple colors.
    /// </summary>
    public static Texture2D LoadTexture(string name)
    {
        string path = Path.Combine(_basePath, "Textures", name);
        if (!File.Exists(path)) return null;

        try
        {
            byte[] data = File.ReadAllBytes(path);
            var tex = new Texture2D(2, 2);
            if (ImageConversion.LoadImage(tex, data)) return tex;
        }
        catch (Exception ex)
        {
            MelonLogger.Error($"[AssetManager] Failed to load texture {name}: {ex.Message}");
        }
        return null;
    }
}
