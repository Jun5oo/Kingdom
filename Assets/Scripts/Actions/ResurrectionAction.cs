using Cysharp.Threading.Tasks;
using System;
using UnityEngine;

public class ResurrectionAction : IAction
{
    ActionType actionType;
    HighlightLayer highlightLayer; 
    HighlightType highlightType;
    ActionPerformer performer;

    public ActionType ActionType {get { return actionType;}}
    public HighlightLayer HighlightLayer { get { return highlightLayer;}}   
    public HighlightType HighlightType {  get { return highlightType;}}
    public ActionPerformer Performer {  get { return performer;}}

    public event Action OnActionCanceled;
    public event Action OnActionComplete;

    GridManager gridManager;
    TokenManager tokenManager;

    Token kingToken;

    Vector2Int targetPosition;

    int currentCost; 
    public int Cost { get { return currentCost;}}

    public ResurrectionAction(Token kingToken, ActionPerformer performer)
    {
        actionType = ActionType.Resurrection;
        highlightLayer = HighlightLayer.Action;
        highlightType = HighlightType.SummonHighlight;
        this.performer = performer;

        this.gridManager = ServiceLocator.Get<GridManager>();
        this.tokenManager = ServiceLocator.Get<TokenManager>(); 

        this.kingToken = kingToken;

        currentCost = 2; 
    }

    public void Enter()
    {
        gridManager.HighlightGridCells((Vector2Int gridPosition) => 
        {
            if (!tokenManager.IsTokenAtGridPosition(gridPosition))
                return false;
            
            Token token = tokenManager.GetTokenFrom(gridPosition); 
            
            if(token != null)
            {
                if (!token.IsAllies(kingToken.OwnerID))
                    return false; 

                /*
                if (token.TokenState == TokenState.Graveyard)
                    return true;
                */ 
            }

            return false; 

        }, HighlightType, HighlightLayer);

    }

    public async UniTask Execute(Vector2Int targetPosition)
    {
        this.targetPosition = targetPosition;
        await Revive(); 
    }

    public void Exit() => gridManager.UnhighlightGridCells(HighlightLayer);
    public bool IsValid()
    {
        return ServiceLocator.Get<ActionSystem>().GetCurrentActionCount() >= currentCost;
    }

    public async UniTask Revive()
    {
        Exit(); 

        if (!tokenManager.IsTokenAtGridPosition(targetPosition))
        {
            OnActionCanceled?.Invoke();
            return; 
        }

        Token targetToken = tokenManager.GetTokenFrom(targetPosition);
        TokenMovement tokenMovement = targetToken.GetComponent<TokenMovement>();

        var taskComplete = new UniTaskCompletionSource(); 
        /*
        tokenMovement.PlayerSpinToss(() =>
        {
            targetToken.Revive(); 
        }, () => 
        { 
            OnActionComplete?.Invoke();
            taskComplete.TrySetResult(); 
        });
        */ 

        await taskComplete.Task; 
    }
}
