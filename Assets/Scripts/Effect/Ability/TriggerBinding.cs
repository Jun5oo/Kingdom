using System;
using System.Collections.Generic;

[Serializable]
public class TriggerBinding
{
    // 하나의 Ability에 서로 다른 Trigger가 존재하는 효과가 있기 때문에 Binding
    // SubAbilitySO

    public Trigger trigger;
    public Target target;
    
    public int targetCount = 0; 

    public List<ConditionSO> triggerConditions;
    public List<ConditionSO> targetConditions;

    // 데미지, 소환 숫자 등 
    public int value;
    // 효과 유지시간 
    public int duration;
    // n턴 이후 효과 발동 
    public int delay; 

    public List<FilterSO> filters; 
    public List<EffectSO> effects;

    public List<TriggerBinding> chainAbilities;
}
