using System;
using System.Collections.Generic;

[Serializable]
public class TriggerBinding
{
    public Trigger trigger;
    public Target target; 

    public List<ConditionSO> conditions;
    public List<ConditionSO> targetConditions;

    public List<FilterSO> filters; 
    public List<EffectSO> effects;
}
