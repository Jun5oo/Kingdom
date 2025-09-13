using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine;

public class ResurrectionAction : IAction
{
    ActionType actionType;
    public ActionType ActionType {get { return actionType;}}

    public event Action OnActionCanceled;
    public event Action OnActionComplete;

    TokenManager tokenManager;
    SummonSystem summonSystem; 

    Token actionOwner;

    Vector2Int targetPosition;

    int currentCost; 
    public int Cost { get { return currentCost;}}

    public ResourceType resourceType;
    public ResourceType ResourceType { get { return resourceType; } }
    public int OwnerID { get { return actionOwner.OwnerID; } }

    public BaseObject BaseObject => actionOwner;

    public Predicate<Vector2Int> Validation => throw new NotImplementedException();

    public ResurrectionAction(Token token)
    {
        actionType = ActionType.Resurrection;

        this.tokenManager = ServiceLocator.Get<TokenManager>(); 
        this.summonSystem  = ServiceLocator.Get<SummonSystem>();

        this.actionOwner = token;

        currentCost = 2;

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
            
            if(token != null)
            {
                if (!token.IsAllies(this.actionOwner.OwnerID))
                    return false;
                if (token.Data.Tag == UnitTag.Graveyard)
                    return true; 
            }

            return false; 

        }, HighlightType, HighlightLayer);
        */ 
    }

    public async UniTask Execute(Vector2Int targetPosition)
    {
        EventQueue eventQueue = ServiceLocator.Get<EventQueue>();

        this.targetPosition = targetPosition;
        Revive();
        await eventQueue.ExecuteAllAsync(); 
    }

    public void Exit()
    {
        // gridManager.UnhighlightGridCells(HighlightLayer);
    }
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

        var unitData = new List<CardData>(targetToken.SourceData);
        
        tokenManager.DestroyToken(targetToken);
       
        EventQueue eventQueue = ServiceLocator.Get<EventQueue>();

        eventQueue.Enqueue(async () =>
        {
            await summonSystem.Summon(actionOwner.OwnerID, unitData[0], targetPosition, actionOwner.Data);
            OnActionComplete?.Invoke(); 
        });
    }
}
