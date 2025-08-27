using Cysharp.Threading.Tasks;
using System;
using UnityEngine;

public class DivineShieldAction : IAction
{
    ActionType actionType;
    public ActionType ActionType { get { return actionType; } }

    Token token; 
    BaseObject target;

    int currentCost;

    GridManager gridManager;
    TokenManager tokenManager; 

    public int Cost { get { return currentCost; } }

    public event Action OnActionCanceled;
    public event Action OnActionComplete;

    public ResourceType resourceType;
    public ResourceType ResourceType { get { return resourceType; } }
    public int OwnerID { get { return token.OwnerID; } }

    public BaseObject BaseObject => token;

    public Predicate<Vector2Int> Validation => throw new NotImplementedException();

    public DivineShieldAction(Token token)
    {
        actionType = ActionType.DivineShield;

        this.gridManager = ServiceLocator.Get<GridManager>();
        this.tokenManager =ServiceLocator.Get<TokenManager>();

        this.token = token; 
        this.target = token;
        this.currentCost = 2;

        resourceType = ResourceType.Ability; 
    }

    public void Enter()
    {
        /*
        gridManager.HighlightGridCells((Vector2Int gridPosition) =>
        {
            if (!tokenManager.IsTokenAtGridPosition(gridPosition))
                return false;

            Token token = tokenManager.GetTokenFrom(gridPosition);

            if (token != null)
            {
                if (!token.IsAllies(target.OwnerID))
                    return false;
                if (token is not IBuffable)
                    return false; 

                return true; 
            }

            return false;

        }, HighlightType, HighlightLayer);
    */
    }

    public async UniTask Execute(Vector2Int targetPosition)
    {
        Exit(); 

        // 나중에는 함수로 따로 작성 
        if (!tokenManager.IsTokenAtGridPosition(targetPosition))
        {
            OnActionCanceled?.Invoke();
            return;
        }

        Token target = tokenManager.GetTokenFrom(targetPosition);

        if (target is not IBuffable buffable)
        {
            OnActionCanceled?.Invoke();
            return;
        }
        IBuff buff = new DivineShield(buffable);
        await buff.OnApply();

        OnActionComplete?.Invoke(); 
    }

    public void Exit()
    {
        // gridManager.UnhighlightGridCells(HighlightLayer); 
    }

    public bool IsValid()
    {
        return true; 
    }

}
