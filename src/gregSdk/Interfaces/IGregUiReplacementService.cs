using greg.Sdk.Definitions;
using UnityEngine;

namespace greg.Sdk.Services;

/// <summary>
/// Service for managing and applying UI replacements and overrides.
/// </summary>
public interface IGregUiReplacementService
{
    /// <summary>Adds or updates a UI override manifest.</summary>
    void RegisterOverride(GregUiReplacementManifest manifest);

    /// <summary>Removes a UI override by its path.</summary>
    void UnregisterOverride(string uiPath);

    /// <summary>Applies all registered overrides to the current scene.</summary>
    void ApplyAllOverrides();

    /// <summary>Tries to apply overrides to a specific GameObject if its path matches any registered override.</summary>
    void TryApplyOverride(GameObject gameObject);

    /// <summary>Checks if a path has an override.</summary>
    bool HasOverride(string uiPath, out GregUiReplacementManifest manifest);
}
