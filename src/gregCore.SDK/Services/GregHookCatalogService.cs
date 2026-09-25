using System;
using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json;
using gregCore.Core.Abstractions;
using gregCore.Sdk.Metadata;

namespace gregCore.Sdk.Services;

/// <summary>
    /// Service for managing and initializing the hook catalog (SDK layer).
/// </summary>
public sealed class GregHookCatalogService
{
    private readonly IGregLogger _logger;
    private readonly GregHookCatalog _catalog;

    public GregHookCatalogService(IGregLogger logger, GregHookCatalog catalog)
    {
        _logger = logger.ForContext("HookCatalogService");
        _catalog = catalog;
    }

    /// <summary>
    /// Loads all 1850 hooks from the game_hooks.json file.
    /// </summary>
    public void Initialize()
    {
        try
        {
            var filePath = Path.Combine(global::MelonLoader.Utils.MelonEnvironment.GameRootDirectory, "game_hooks.json");
            if (!File.Exists(filePath))
            {
                // Fallback if the file is in the mod folder
                filePath = Path.Combine(global::MelonLoader.Utils.MelonEnvironment.ModsDirectory, "game_hooks.json");
            }

            if (File.Exists(filePath))
            {
                var json = File.ReadAllText(filePath);
                var rawHooks = JsonConvert.DeserializeObject<List<RawHookData>>(json);
                if (rawHooks != null)
                {
                    foreach (var raw in rawHooks)
                    {
                        var hookName = $"greg.{raw.Group.ToUpper()}.{raw.ClassName}.{raw.MethodName}";
                        _catalog.Register(new HookMetadata(
                            hookName,
                            HookStatus.ENABLED,
                            HookLayer.HARMONY,
                            $"Triggered on {raw.ClassName}.{raw.MethodName}",
                            raw.IsVoid ? "None" : raw.ReturnType,
                            "1.1.0"
                        ));
                    }
                    _logger.Success($"{_catalog.TotalCount} Hooks successfully initialized from game_hooks.json.");
                }
            }
            else
            {
                _logger.Warning("game_hooks.json not found. Hook catalog is empty.");
            }
        }
        catch (Exception ex)
        {
            _logger.Error("Error initializing hook catalog", ex);
        }
    }

    private class RawHookData
    {
        public string Group { get; set; } = string.Empty;
        public string ClassName { get; set; } = string.Empty;
        public string MethodName { get; set; } = string.Empty;
        public string ReturnType { get; set; } = string.Empty;
        public bool IsVoid { get; set; }
    }
}
