using System.Collections.Generic;
using greg.Sdk.Definitions;

namespace gregCoreSDK.Sdk.Services;

public class GregModelOverrideService : IGregModelOverrideService
{
    private readonly Dictionary<string, ModelOverrideManifest> _overrides = new Dictionary<string, ModelOverrideManifest>();

    public void ReplaceModel(string contentId, string modelPath, string fallbackPath)
    {
        _overrides[contentId] = new ModelOverrideManifest { 
            TargetContentId = contentId, 
            ReplacementPath = modelPath, 
            FallbackPath = fallbackPath 
        };
    }

    public void ApplyManifest(ModelOverrideManifest manifest)
    {
        if (manifest == null || string.IsNullOrEmpty(manifest.TargetContentId)) return;

        if (_overrides.TryGetValue(manifest.TargetContentId, out var existing))
        {
            if (manifest.Priority > existing.Priority)
            {
                GregDiagnostics.LogContentWarning("ModelOverride", manifest.TargetContentId, 
                    $"Conflict detected. Overriding existing override from {existing.Author} with higher priority from {manifest.Author}.");
                _overrides[manifest.TargetContentId] = manifest;
            }
            else
            {
                GregDiagnostics.LogContentWarning("ModelOverride", manifest.TargetContentId, 
                    $"Conflict detected. Ignoring override from {manifest.Author} because {existing.Author} has same or higher priority.");
            }
        }
        else
        {
            _overrides[manifest.TargetContentId] = manifest;
        }
    }

    public bool TryGetOverride(string contentId, out ModelOverrideManifest manifest)
    {
        return _overrides.TryGetValue(contentId, out manifest);
    }
}
