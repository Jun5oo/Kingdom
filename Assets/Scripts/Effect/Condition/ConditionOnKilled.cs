using UnityEngine;
[CreateAssetMenu(menuName = "Condition/ConditionOnKilledSO")]
public class ConditionOnKilled : ConditionSO
{
    [SerializeField] ConditionOperatorBool op;

    public override bool IsTriggerConditionSatisfied(AbilitySO ability, BaseObject caster, EffectContext context)
    {
        if(!context.TryGet<BaseObject>(ContextKey.KillerObject, out var killer))
        {
            Debug.Log("처치한 오브젝트가 없습니다.");
            return false; 
        }

        bool isMyUnit = caster.OwnerID == killer.OwnerID; 
        return CompareBool(isMyUnit, op); 
    }
}
