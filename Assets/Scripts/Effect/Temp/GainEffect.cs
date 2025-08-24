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

        this.owner = effectOwner;

        Debug.Log($"GainEffect Created: {resourceType}"); 
    }

    public EffectType EffectType => effectType;
    public Trigger Trigger => trigger;

    public List<Func<UniTask>> ToEvents(BaseObject owner, EffectContext context)
    {
        var events = new List<Func<UniTask>>();

        events.Add(async () =>
        {

        });

        return events;
    }
}
