using System.Collections.Generic;

namespace gregCoreSDK.Sdk.Services;

public static class GregMetadataService
{
    private static readonly Dictionary<string, Dictionary<string, string>> _metadata = new Dictionary<string, Dictionary<string, string>>();
    private static readonly IReadOnlyDictionary<string, string> _emptyMetadata = new Dictionary<string, string>();

    public static void SetMetadata(string entityId, string key, string value)
    {
        if (!_metadata.ContainsKey(entityId))
            _metadata[entityId] = new Dictionary<string, string>();

        _metadata[entityId][key] = value;
    }

    public static IReadOnlyDictionary<string, string> GetMetadata(string entityId)
    {
        return _metadata.TryGetValue(entityId, out var data) ? data : _emptyMetadata;
    }
}
