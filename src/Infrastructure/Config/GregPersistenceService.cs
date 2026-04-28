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

    private string GetSafePath(string key)
    {
        if (string.IsNullOrWhiteSpace(key))
            throw new System.ArgumentException("Key cannot be null or empty", nameof(key));

        if (key.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0 ||
            key.Contains(Path.DirectorySeparatorChar) ||
            key.Contains(Path.AltDirectorySeparatorChar) ||
            key.Contains(".."))
        {
            _logger.Error($"[Security] Attempted path traversal detected with key: {key}");
            throw new System.ArgumentException("Invalid characters in key", nameof(key));
        }

        return Path.Combine(_saveDirectory, $"{key}.json");
    }

    public void Set<T>(string key, T value) where T : notnull
    {
        var path = GetSafePath(key);
        File.WriteAllText(path, JsonSerializer.Serialize(value));
    }

    public T Get<T>(string key, T defaultValue = default!) where T : notnull
    {
<<<<<<< HEAD
=======
        var path = GetSafePath(key);
        if (!File.Exists(path)) return defaultValue;
>>>>>>> 94b9b12c1e148f8844b975f68126cc4b377dfe77
        try {
            var path = GetSafePath(key);
            if (!File.Exists(path)) return defaultValue;
            return JsonSerializer.Deserialize<T>(File.ReadAllText(path)) ?? defaultValue;
        } catch { return defaultValue; }
    }

<<<<<<< HEAD
    public bool Has(string key)
    {
        try { return File.Exists(GetSafePath(key)); }
        catch { return false; }
    }

    public void Delete(string key)
    {
        try { File.Delete(GetSafePath(key)); }
        catch { }
    }
=======
    public bool Has(string key) => File.Exists(GetSafePath(key));
    public void Delete(string key) => File.Delete(GetSafePath(key));
>>>>>>> 94b9b12c1e148f8844b975f68126cc4b377dfe77
}