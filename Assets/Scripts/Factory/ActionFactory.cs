using System.Collections;
using UnityEngine;

public class ActionFactory
{
    GridManager gridSystem;
    CardManager cardSystem;
    TokenManager tokenManager;
    DamageManager damageManager;
    TokenFactory tokenFactory;

    public void Init(GridManager gridManager, CardManager cardManager, TokenManager tokenManager, DamageManager damageManager, TokenFactory tokenFactory)
    {
        this.gridSystem = gridManager;
        this.cardSystem = cardManager;
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
                if(entity is Card card)
                    action = new SummonAction(gridSystem, cardSystem, tokenManager, tokenFactory, card, performer); 
                break;
            case ActionType.Move:
                if(entity is Token moveableToken)
                    action = new MoveAction(gridSystem, tokenManager, moveableToken, performer);
                break;
            case ActionType.Attack:
                if(entity is Token attackableToken)
                action = new AttackAction(gridSystem, tokenManager, damageManager,  attackableToken, performer); 
                break; 
        }

        return action; 
    }
}
