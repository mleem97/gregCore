/// <file-summary>
/// Schicht:      Infrastructure
/// Zweck:        Persistenz-Service basierend auf System.Text.Json.
/// Maintainer:   Schnelle, alloc-arme Serialisierung für Runtime-Daten.
/// </file-summary>

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

    public T? LoadData<T>(string key) where T : class
    {
        try
        {
            var path = Path.Combine(_saveDirectory, $"{key}.json");
            if (!File.Exists(path)) return null;

            using var stream = File.OpenRead(path);
            return JsonSerializer.Deserialize<T>(stream);
        }
        catch (Exception ex)
        {
            _logger.Error($"Fehler beim Laden von Daten für Schlüssel {key}", ex);
            return null;
        }
    }

    public void SaveData<T>(string key, T data) where T : class
    {
        try
        {
            var path = Path.Combine(_saveDirectory, $"{key}.json");
            using var stream = File.Create(path);
            JsonSerializer.Serialize(stream, data);
        }
        catch (Exception ex)
        {
            _logger.Error($"Fehler beim Speichern von Daten für Schlüssel {key}", ex);
        }
    }
}
