using UnityEngine;

public abstract class ConditionSO : ScriptableObject
{
    public virtual bool IsTriggerConditionSatisfied(BaseObject caster, EffectContext context)
    {
        return true;
    }

    public virtual bool IsTargetConditionSatisfied(BaseObject caster, BaseObject target)
    {
        return true;
    }
    public virtual bool IsTargetConditionSatisfied(BaseObject target)
    {
        return true;
    }
    public virtual bool IsTargetConditionSatisfied(BaseObject caster, Vector2Int targetPosition)
    {
        return true;
    }

    public bool CompareBool(bool condition, ConditionOperatorBool op)
    {
        if (op == ConditionOperatorBool.False)
            return !condition;

        return condition; 
    }
}
