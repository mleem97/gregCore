using System.Collections.Generic;

namespace greg.Sdk;

public interface IGregContentRegistry<TDefinition>
{
    void Register(TDefinition definition);
    bool TryGet(string id, out TDefinition definition);
    IReadOnlyCollection<TDefinition> GetAll();
    bool Remove(string id);
    void Clear();
}

