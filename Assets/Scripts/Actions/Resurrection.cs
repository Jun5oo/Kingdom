using System;
using UnityEngine;

public class Resurrection : IAction
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

    public Resurrection(GridManager gridManager, TokenManager tokenManager, Token kingToken, ActionPerformer performer)
    {
        actionType = ActionType.Resurrection;
        highlightLayer = HighlightLayer.Action;
        highlightType = HighlightType.SummonHighlight;
        this.performer = performer;

        this.gridManager = gridManager;
        this.tokenManager = tokenManager;

        this.kingToken = kingToken; 
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
                if (!token.IsAllies(kingToken.OwnerPlayerID))
                    return false; 
                if (token.TokenState == TokenState.Graveyard)
                    return true; 
            }

            return false; 

        }, HighlightType, HighlightLayer);

    }

    public void Execute(Vector2Int targetPosition)
    {
        this.targetPosition = targetPosition;
        Revive(); 
    }

    public void Exit() => gridManager.UnhighlightGridCells(HighlightLayer); 
    
    public void Revive()
    {
        Exit(); 

        if (!tokenManager.IsTokenAtGridPosition(targetPosition))
        {
            OnActionCanceled?.Invoke();
            return; 
        }

        Token targetToken = tokenManager.GetTokenFrom(targetPosition);
        TokenMovement tokenMovement = targetToken.GetComponent<TokenMovement>();
        
        tokenMovement.PlayerSpinToss(() =>
        {
            targetToken.Revive(); 
        }, () => { OnActionComplete?.Invoke(); });
    }
    
    
    public bool IsValid()
    {
        // TODO: Graveyard가 있는지 확인을 해야함 
        return true; 
    }
}
