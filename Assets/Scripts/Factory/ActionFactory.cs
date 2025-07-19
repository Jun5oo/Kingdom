public class ActionFactory
{
    // IAction을 생성. 생성될 actionType과 action을 실행한 카드를 파라미터 값으로 받음 
    public IAction CreateAction(ActionType actionType, Entity entity, ActionPerformer performer = ActionPerformer.Player)
    {
        IAction action = null;

        switch (actionType)
        {
            case ActionType.Summon:
                if(entity is UnitCard card)
                    action = new SummonAction(card, performer); 
                break;
            case ActionType.Move:
                if(entity is Token moveableToken)
                    action = new MoveAction(moveableToken, performer);
                break;
            case ActionType.Attack:
                if(entity is Token attackableToken)
                    action = new AttackAction(attackableToken, performer); 
                break;
            case ActionType.Resurrection: 
                if(entity is Token kingToken)
                {
                    if (kingToken.IsKing)
                        action = new Resurrection(kingToken, performer);
                }
                break; 
        }

        return action; 
    }
}
