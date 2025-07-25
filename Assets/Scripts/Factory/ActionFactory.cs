using UnityEngine;
public class ActionFactory
{
    // IAction을 생성. 생성될 actionType과 action을 실행한 카드를 파라미터 값으로 받음 
    public IAction CreateAction(ActionType actionType, BaseObject baseObject, ActionPerformer performer = ActionPerformer.Player)
    {
        IAction action = null;

        switch (actionType)
        {
            case ActionType.Summon:
                if (baseObject is UnitCard card)
                    action = new SummonAction(card, performer);
                else
                {
                    Debug.LogError($"[ActionFactory] Tried to create SummonAction for {baseObject.GetType()}, but it's not a UnitCard.");
                }
                break;
            case ActionType.Move:
                if(baseObject is Token moveableToken)
                    action = new MoveAction(moveableToken, performer);
                break;
            case ActionType.Attack:
                if(baseObject is Token attackableToken)
                    action = new AttackAction(attackableToken, performer); 
                break;
            case ActionType.Resurrection: 
                if(baseObject is Token kingToken)
                {
                    if (kingToken.IsKing)
                        action = new Resurrection(kingToken, performer);
                }
                break; 
        }

        return action; 
    }
}
