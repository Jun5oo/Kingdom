using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine;

public class BuffEffect : IEffect
{
    BaseObject effectOwner;
    EffectData effectData;

    public EffectType EffectType => effectData.effectType; 

    public Trigger Trigger => effectData.trigger;

    public EffectData EffectData => effectData; 

    public BuffEffect(EffectData effectData, BaseObject effectOwner)
    {
        this.effectData = effectData;
        this.effectOwner = effectOwner;
    }

    public List<Func<UniTask>> ToEvents(BaseObject owner, EffectContext context)
    {
        var events = new List<Func<UniTask>>();

        events.Add(async () =>
        {

        });

        return events;
    }
}
