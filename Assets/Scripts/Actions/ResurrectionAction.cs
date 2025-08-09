using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
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
    SummonSystem summonSystem; 

    Token actionOwner;

    Vector2Int targetPosition;

    int currentCost; 
    public int Cost { get { return currentCost;}}

    public ResourceType resourceType;
    public ResourceType ResourceType { get { return resourceType; } }
    public int OwnerID { get { return actionOwner.OwnerID; } }

    public ResurrectionAction(Token token, ActionPerformer performer)
    {
        actionType = ActionType.Resurrection;
        highlightLayer = HighlightLayer.Action;
        highlightType = HighlightType.SummonHighlight;
        this.performer = performer;

        this.gridManager = ServiceLocator.Get<GridManager>();
        this.tokenManager = ServiceLocator.Get<TokenManager>(); 
        this.summonSystem  = ServiceLocator.Get<SummonSystem>();

        this.actionOwner = token;

        currentCost = 2;

        resourceType = ResourceType.Ability; 
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
                if (!token.IsAllies(this.actionOwner.OwnerID))
                    return false;
                if (token.UnitData.Tag == UnitTag.Graveyard)
                    return true; 
            }

            return false; 

        }, HighlightType, HighlightLayer);

    }

    public async UniTask Execute(Vector2Int targetPosition)
    {
        EventQueue eventQueue = ServiceLocator.Get<EventQueue>();

        this.targetPosition = targetPosition;
        Revive();
        await eventQueue.ExecuteAllAsync(); 
    }

    public void Exit() => gridManager.UnhighlightGridCells(HighlightLayer);
    public bool IsValid()
    {
        return true; 
        // return ServiceLocator.Get<ActionSystem>().GetCurrentActionCount() >= currentCost;
    }

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

        var unitData = new List<UnitCardData>(targetToken.SourceObjects);
        
        tokenManager.DestroyToken(targetToken);
       
        EventQueue eventQueue = ServiceLocator.Get<EventQueue>();

        eventQueue.Enqueue(async () =>
        {
            await summonSystem.Summon(actionOwner.OwnerID, unitData[0], targetPosition, actionOwner.Data);
            OnActionComplete?.Invoke(); 
        });
    }
}
