using System.Collections.Generic;

public enum ContextKey
{
    None, 

    // Position 
    Selected,
    AllyPos, 
    EnemyPos,
    
    // BaseObject 
    Ally, 
    Enemy, 
    AllySource, 
    EnemySource, 
}

public struct EffectContext
{
    Dictionary<ContextKey, object> payload;

    public void Set<T>(ContextKey key, T value)
    {
        payload ??= new Dictionary<ContextKey, object>();
        payload[key] = value;
    }

    public bool TryGet<T>(ContextKey key, out T value)
    {
        if(payload != null && payload.TryGetValue(key, out var obj) && obj is T t)
        {
            value = t;
            return true; 
        }

        value = default;
        return false; 
    }
}
