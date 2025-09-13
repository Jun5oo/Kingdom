using UnityEngine;
[CreateAssetMenu(menuName = "Condition/ConditionOnDeathSO")]
public class ConditionOnDeath : ConditionSO
{
    [SerializeField] ConditionOperatorBool op;

    public override bool IsTriggerConditionSatisfied(BaseObject caster, EffectContext context)
    {
        if (caster == null)
            return false; 

        if(!context.TryGet<ObjectContext>(ContextKey.Death, out var objectContext))
        {
            Debug.Log("Context.Death 데이터를 찾을 수 없습니다.");
            return false; 
        }

        bool isAllyDead = caster.OwnerID == objectContext.ownerID;
        return CompareBool(isAllyDead, op); 
    }
}
