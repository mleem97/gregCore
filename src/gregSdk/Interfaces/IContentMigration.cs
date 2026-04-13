namespace greg.Sdk.Interfaces;

public interface IContentMigration
{
    bool NeedsMigration(string json, int currentVersion);
    string Migrate(string json);
}

