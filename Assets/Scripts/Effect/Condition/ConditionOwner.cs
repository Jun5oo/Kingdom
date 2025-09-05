using UnityEngine;

[CreateAssetMenu(menuName =("Condition/ConditionOwnerSO"))]
public class ConditionOwner : ConditionSO
{
    [SerializeField] ConditionOperatorBool op; 

    public override bool IsTargetConditionSatisfied(BaseObject caster, BaseObject target)
    {
        bool sameOwner = caster.OwnerID == target.OwnerID;
        return CompareBool(sameOwner, op);
    }
}
