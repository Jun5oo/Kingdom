using UnityEngine;
[CreateAssetMenu(menuName = "Condition/ConditionOnKilledSO")]
public class ConditionOnKilled : ConditionSO
{
    [SerializeField] ConditionOperatorBool op;
    public override bool IsTriggerConditionSatisfied(BaseObject caster, EffectContext context)
    {
        if (caster == null)
            return false;

        if (!context.TryGet<ObjectContext>(ContextKey.Kill, out var objectContext))
        {
            Debug.Log("Context.Kill 데이터를 찾을 수 없습니다.");
            return false;
        }

        bool isAllyKill = caster.OwnerID == objectContext.ownerID;
        return CompareBool(isAllyKill, op);
    }
}
