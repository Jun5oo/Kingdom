using UnityEngine;

[CreateAssetMenu(menuName = "Condition/ConditionTurnSO")]
public class ConditionTurn : ConditionSO
{
    [SerializeField] ConditionOperatorBool op; 

    public override bool IsTriggerConditionSatisfied(AbilitySO ability, BaseObject caster, EffectContext context)
    {
        TurnSystem turnSystem = ServiceLocator.Get<TurnSystem>();

        bool isMyTurn = caster.OwnerID == turnSystem.GetCurrentTurnPlayerID();
        return CompareBool(isMyTurn, op); 
    }
}
