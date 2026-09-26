using System.Collections.Generic;
using System.Linq;

namespace gregCore.Sdk.Metadata;

public enum HookStatus { ENABLED, DISABLED, DEPRECATED }
public enum HookLayer { CORE, SDK, HARMONY, PLUGIN }

/// <summary>
    /// Metadata for a single hook in the framework catalog.
/// </summary>
public sealed record HookMetadata(
    string Name,
    HookStatus Status,
    HookLayer Layer,
    string Trigger,
    string PayloadType,
    string SinceVersion,
    string KnownIssues = ""
);

/// <summary>
    /// The central hook catalog (SDK layer).
    /// Serves as the source of truth for all 1850 hooks.
/// </summary>
public sealed class GregHookCatalog
{
    private readonly Dictionary<string, HookMetadata> _hooks = new();

    public void Register(HookMetadata metadata)
    {
        _hooks[metadata.Name] = metadata;
    }

    public HookMetadata? Get(string hookName)
    {
        _hooks.TryGetValue(hookName, out var metadata);
        return metadata;
    }

    public IEnumerable<HookMetadata> GetAll() => _hooks.Values;

    public IEnumerable<HookMetadata> GetByStatus(HookStatus status) => _hooks.Values.Where(h => h.Status == status);

    public int TotalCount => _hooks.Count;
}
