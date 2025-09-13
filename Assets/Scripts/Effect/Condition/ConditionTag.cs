using UnityEngine;

[CreateAssetMenu(menuName = "Condition/ConditionTagSO")]
public class ConditionTag : ConditionSO
{
    [SerializeField] UnitTag tag;
    [SerializeField] ConditionOperatorBool op; 

    public override bool IsTargetConditionSatisfied(BaseObject caster, Vector2Int targetPosition)
    {
        TokenManager tokenManager = ServiceLocator.Get<TokenManager>(); 

        if(!tokenManager.TryGetTokenFrom(targetPosition, out Token token))
        {
            Debug.Log("해당 위치에 오브젝트가 존재하지 않습니다.");
            return false; 
        }

        bool isSameTag = token.Data.Tag == tag; 
        return CompareBool(isSameTag, op); 
    }
}
