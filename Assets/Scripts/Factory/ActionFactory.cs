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
                if(baseObject is Token undeadKing)
                {
                    if (undeadKing.Tag == UnitTag.King)
                        action = new ResurrectionAction(undeadKing, performer);
                }
                break;
            case ActionType.DivineShield:
                if(baseObject is Token celestialKing)
                {
                    if (celestialKing.Tag == UnitTag.King)
                        action = new DivineShieldAction(celestialKing, performer); 
                }
                break;
            case ActionType.Upgrade:
                if(baseObject is Token king)
                {
                    if(king.Tag == UnitTag.King)
                        action = new UpgradeAction(king, performer);
                }
                break; 
        }

        return action; 
    }
}
