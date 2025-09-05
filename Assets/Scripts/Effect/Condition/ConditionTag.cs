using UnityEngine;

[CreateAssetMenu(menuName = "Condition/ConditionTagSO")]
public class ConditionTag : ConditionSO
{
    [SerializeField] UnitTag tag;
    [SerializeField] ConditionOperatorBool op; 

    public override bool IsTargetConditionSatisfied(BaseObject target)
    {
        bool isSameTag = target.Data.Tag == tag;
        return CompareBool(isSameTag, op); 
    }
}
