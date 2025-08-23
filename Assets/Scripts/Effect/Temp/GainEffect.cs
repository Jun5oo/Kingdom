using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine;

public class GainEffect : IEffect
{
    EffectType effectType;
    Trigger trigger;
    ResourceType resourceType;

    BaseObject owner;
    int value; 

    public GainEffect(EffectData effectData, BaseObject effectOwner)
    {
        this.effectType = effectData.effectType;    
        this.trigger = effectData.trigger;

        this.value = effectData.value;

        /*
        if(System.Enum.TryParse<ResourceType>(effectData.parameter1, out ResourceType resourceType))
            this.resourceType = resourceType;
        */ 

        this.owner = effectOwner;

        Debug.Log($"GainEffect Created: {resourceType}"); 
    }

    public EffectType EffectType => effectType;
    public Trigger Trigger => trigger;

    public UniTask ExecuteAsync(EffectContext context)
    {
        Debug.Log($"{EffectType}이 샐행되었습니다.");
        switch (resourceType)
        {
            case ResourceType.Action:
                ActionResourceSystem actionResourceSystem = ServiceLocator.Get<ActionResourceSystem>();
                actionResourceSystem.Add(owner.OwnerID, value);
                break;
            case ResourceType.Ability: 
                AbilityResourceSystem abilityResourceSystem = ServiceLocator.Get<AbilityResourceSystem>();
                abilityResourceSystem.Add(owner.OwnerID, value);
                break; 
        }

        return UniTask.CompletedTask; 
    }

    public List<Func<UniTask>> ToEvents(BaseObject owner, EffectContext context)
    {
        throw new NotImplementedException();
    }
}
