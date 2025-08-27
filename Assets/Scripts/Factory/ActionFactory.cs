using UnityEngine;
public class ActionFactory
{
    // IAction을 생성. 생성될 actionType과 action을 실행한 카드를 파라미터 값으로 받음 
    public IAction CreateAction(ActionType actionType, BaseObject baseObject)
    {
        IAction action = null;

        switch (actionType)
        {
            case ActionType.Summon:
                if (baseObject is Card card)
                    action = new SummonAction(card);
                break;
            case ActionType.Move:
                if(baseObject is Token moveableToken)
                    action = new MoveAction(moveableToken);
                break;
            case ActionType.Attack:
                if(baseObject is Token attackableToken)
                    action = new AttackAction(attackableToken); 
                break;
            case ActionType.Resurrection: 
                if(baseObject is Token undeadKing)
                {
                    if (undeadKing.Tag == UnitTag.King)
                        action = new ResurrectionAction(undeadKing);
                }
                break;
            case ActionType.DivineShield:
                if(baseObject is Token celestialKing)
                {
                    if (celestialKing.Tag == UnitTag.King)
                        action = new DivineShieldAction(celestialKing); 
                }
                break;
            case ActionType.Upgrade:
                if(baseObject is Token king)
                {
                    if(king.Tag == UnitTag.King)
                        action = new UpgradeAction(king);
                }
                break; 
        }

        return action; 
    }
}
