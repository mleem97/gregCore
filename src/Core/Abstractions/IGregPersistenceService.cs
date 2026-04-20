/// <file-summary>
/// Schicht:      Core
/// Zweck:        Interface für Daten-Persistenz.
/// Maintainer:   Wird für Spielstände und Mod-Daten (System.Text.Json) genutzt.
/// </file-summary>

namespace gregCore.Core.Abstractions;

public interface IGregPersistenceService
{
    T? LoadData<T>(string key) where T : class;
    void SaveData<T>(string key, T data) where T : class;
}
