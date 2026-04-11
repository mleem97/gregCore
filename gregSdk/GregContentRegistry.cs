using System.Collections.Generic;
using gregSdk.Interfaces;
using gregSdk.Services;

namespace gregSdk;

public class GregContentRegistry<TDefinition> : IGregContentRegistry<TDefinition>
{
    private readonly Dictionary<string, TDefinition> _registry = new Dictionary<string, TDefinition>();
    private readonly System.Func<TDefinition, string> _idSelector;
    private readonly IContentValidator<TDefinition> _validator;

    public GregContentRegistry(System.Func<TDefinition, string> idSelector, IContentValidator<TDefinition> validator = null)
    {
        _idSelector = idSelector;
        _validator = validator;
    }

    public void Register(TDefinition definition)
    {
        var id = _idSelector(definition);
        
        if (_validator != null && !_validator.Validate(definition, out var error))
        {
            GregDiagnostics.LogContentError(typeof(TDefinition).Name, id, $"Validation failed: {error}");
            return;
        }

        if (_registry.ContainsKey(id))
        {
            GregDiagnostics.LogContentWarning(typeof(TDefinition).Name, id, "Overwriting existing definition.");
        }

        _registry[id] = definition;
        
        // Emit lifecycle events
        gregEventDispatcher.Emit(gregNativeEventHooks.ContentRegistered, new { 
            Id = id, 
            Type = typeof(TDefinition).Name,
            Definition = definition 
        });
    }

    public bool TryGet(string id, out TDefinition definition)
    {
        return _registry.TryGetValue(id, out definition);
    }

    public IReadOnlyCollection<TDefinition> GetAll()
    {
        return _registry.Values;
    }

    public bool Remove(string id)
    {
        return _registry.Remove(id);
    }

    public void Clear()
    {
        _registry.Clear();
    }
}
