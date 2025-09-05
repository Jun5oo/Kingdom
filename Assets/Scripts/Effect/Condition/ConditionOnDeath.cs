using UnityEngine;
[CreateAssetMenu(menuName = "Condition/ConditionOnDeathSO")]
public class ConditionOnDeath : ConditionSO
{
    [SerializeField] ConditionOperatorBool op;

    public override bool IsTriggerConditionSatisfied(AbilitySO ability, BaseObject caster, EffectContext context)
    {
        if(!context.TryGet<int>(ContextKey.VictimOwnerID, out var victimOwnerID)){
            Debug.Log("처치된 오브젝트가 없습니다.");
            return false; 
        }

        bool isMyUnitDead = caster.OwnerID == victimOwnerID; 
        return CompareBool(isMyUnitDead, op); 
    }
}
