using System;
using System.Collections.Generic;

[Serializable]
public class TriggeredEffect
{
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

    public int cost; 

    public List<FilterSO> filters; 
    public List<EffectSO> effects;

    public List<TriggeredEffect> chainAbilities;
}
