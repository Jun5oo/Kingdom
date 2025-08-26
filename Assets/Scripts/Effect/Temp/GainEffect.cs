using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine;

public class GainEffect : IEffect
{
    EffectData effectData;
    BaseObject effectOwner;

    ResourceType resourceType;

    int value; 

    public GainEffect(EffectData effectData, BaseObject effectOwner)
    {
        this.effectData = effectData; 
        this.effectOwner = effectOwner;

        Debug.Log($"GainEffect Created: {resourceType}"); 
    }

    public EffectType EffectType => effectData.effectType; 
    public Trigger Trigger => effectData.trigger;

    public EffectData EffectData => effectData;

    public List<Func<UniTask>> ToEvents(BaseObject owner, EffectContext context)
    {
        var events = new List<Func<UniTask>>();

        events.Add(async () =>
        {
            if (System.Enum.TryParse<ResourceType>(effectData.reward, true, out var result))
            {
                switch (result)
                {
                    case ResourceType.Action: 
                        ActionResourceSystem actionResourceSystem = ServiceLocator.Get<ActionResourceSystem>();
                        actionResourceSystem.Add(owner.OwnerID, effectData.value);
                        break;
                    case ResourceType.Ability:
                        AbilityResourceSystem abilityResourceSystem = ServiceLocator.Get<AbilityResourceSystem>();
                        abilityResourceSystem.Add(owner.OwnerID, effectData.value);
                        break;
                }
            }
        });

        return events;
    }
}
