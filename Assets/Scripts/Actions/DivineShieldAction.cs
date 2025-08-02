using Cysharp.Threading.Tasks;
using System;
using UnityEngine;

public class DivineShieldAction : IAction
{
    ActionType actionType;
    HighlightLayer highlightLayer;
    HighlightType highlightType; 
    ActionPerformer performer;

    BaseObject target;
    int currentCost;

    GridManager gridManager;
    TokenManager tokenManager; 

    public ActionType ActionType { get { return actionType; } }
    public HighlightLayer HighlightLayer { get { return highlightLayer; } }
    public HighlightType HighlightType { get { return highlightType; } }
    public ActionPerformer Performer { get { return performer; } }

    public int Cost { get { return currentCost; } }

    public event Action OnActionCanceled;
    public event Action OnActionComplete;

    public DivineShieldAction(Token token, ActionPerformer performer)
    {
        // ActionType 추후에 정리할 필요가 있음. 
        actionType = ActionType.DivineShield;
        highlightLayer = HighlightLayer.Action;
        highlightType = HighlightType.SummonHighlight;
        this.performer = performer;

        this.gridManager = ServiceLocator.Get<GridManager>();
        this.tokenManager =ServiceLocator.Get<TokenManager>();

        this.target = token;
        this.currentCost = 2; 
    }

    public void Enter()
    {
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
        gridManager.UnhighlightGridCells(HighlightLayer); 
    }

    public bool IsValid()
    {
        return ServiceLocator.Get<ActionSystem>().GetCurrentActionCount() >= currentCost;
    }

}
