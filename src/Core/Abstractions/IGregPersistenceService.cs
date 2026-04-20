namespace gregCore.Core.Abstractions;
public interface IGregPersistenceService
{
    void Set<T>(string key, T value) where T : notnull;
    T Get<T>(string key, T defaultValue = default!) where T : notnull;
    bool Has(string key);
    void Delete(string key);
}