using System;
using System.Collections.Generic;
using UnityEngine;

public class EffectFactory
{
    Dictionary<EffectType, Func<EffectData, BaseObject, IEffect>> effects;

    public EffectFactory()
    {
        effects = new Dictionary<EffectType, Func<EffectData, BaseObject, IEffect>>();

        effects.Add(EffectType.Summon, (effectData, effectOwner) => {  return new SummonEffect(effectData, effectOwner); }); 
        effects.Add(EffectType.Gain, (effectData, effectOwner) => { return new GainEffect(effectData, effectOwner); });
        // effects.Add(EffectType.Buff, (effectData, effectOwner) => { return new BuffEffect(effectData); }); 
        // effects.Add(EffectType.Destroy, (effectData, effectOwner) => { return new DestroyEffect(effectData); });
    }

    public IEffect CreateEffect(EffectData effectData, BaseObject effectOwner)
    {
        if(effectOwner == null)
        {
            Debug.LogWarning($"{effectOwner}, 효과를 가진 오브젝트가 존재하지 않아 생성할 수 없습니다.");
            return null; 
        }

        if(!effects.TryGetValue(effectData.effectType, out var func))
        {
            Debug.LogError($"{effectData.effectType}의 효과를 생성할 수 없습니다.");
            return null; 
        }

        return func(effectData, effectOwner); 
    } 


}
