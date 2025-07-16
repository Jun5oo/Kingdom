public class ActionFactory
{
    GridManager gridManager;
    CardManager cardManager;
    TokenManager tokenManager;
    DamageManager damageManager;
    TokenFactory tokenFactory;

    public void Init(GridManager gridManager, CardManager cardManager, TokenManager tokenManager, DamageManager damageManager, TokenFactory tokenFactory)
    {
        this.gridManager = gridManager;
        this.cardManager = cardManager;
        this.tokenManager = tokenManager;
        this.damageManager = damageManager;
        this.tokenFactory = tokenFactory;
    }

    // IAction을 생성. 생성될 actionType과 action을 실행한 카드를 파라미터 값으로 받음 
    public IAction CreateAction(ActionType actionType, Entity entity, ActionPerformer performer = ActionPerformer.Player)
    {
        IAction action = null;

        switch (actionType)
        {
            case ActionType.Summon:
                if(entity is UnitCard card)
                    action = new SummonAction(gridManager, cardManager, tokenManager, tokenFactory, card, performer); 
                break;
            case ActionType.Move:
                if(entity is Token moveableToken)
                    action = new MoveAction(gridManager, tokenManager, moveableToken, performer);
                break;
            case ActionType.Attack:
                if(entity is Token attackableToken)
                    action = new AttackAction(gridManager, tokenManager, damageManager,  attackableToken, performer); 
                break;
            case ActionType.Resurrection: 
                if(entity is Token kingToken)
                {
                    if (kingToken.IsKing)
                        action = new Resurrection(gridManager, tokenManager, kingToken, performer);
                }
                break; 
        }

        return action; 
    }
}
