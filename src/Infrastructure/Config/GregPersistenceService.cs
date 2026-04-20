using System.IO;
using System.Text.Json;

namespace gregCore.Infrastructure.Config;

public sealed class GregPersistenceService : IGregPersistenceService
{
    private readonly IGregLogger _logger;
    private readonly string _saveDirectory;

    public GregPersistenceService(IGregLogger logger)
    {
        _logger = logger.ForContext("PersistenceService");
        _saveDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "gregCore", "Saves");
        Directory.CreateDirectory(_saveDirectory);
    }

    public void Set<T>(string key, T value) where T : notnull
    {
        var path = Path.Combine(_saveDirectory, $"{key}.json");
        File.WriteAllText(path, JsonSerializer.Serialize(value));
    }

    public T Get<T>(string key, T defaultValue = default!) where T : notnull
    {
        var path = Path.Combine(_saveDirectory, $"{key}.json");
        if (!File.Exists(path)) return defaultValue;
        try {
            return JsonSerializer.Deserialize<T>(File.ReadAllText(path)) ?? defaultValue;
        } catch { return defaultValue; }
    }

    public bool Has(string key) => File.Exists(Path.Combine(_saveDirectory, $"{key}.json"));
    public void Delete(string key) => File.Delete(Path.Combine(_saveDirectory, $"{key}.json"));
}