using UnityEngine;

[CreateAssetMenu (menuName = "Condition/ConditionGridSO")]
public class ConditionGrid : ConditionSO
{
    [SerializeField] ConditionOperatorBool op; 

    public override bool IsTargetConditionSatisfied(BaseObject caster, Vector2Int targetPosition)
    {
        TokenManager tokenManager = ServiceLocator.Get<TokenManager>();   
        
        // 해당 위치에 유닛이 존재하지 않으면 false 
        if(!tokenManager.TryGetTokenFrom(targetPosition, out var token))
            return CompareBool(true, op); 

        // 존재하면 true 
        return CompareBool(false, op);
    }
}
