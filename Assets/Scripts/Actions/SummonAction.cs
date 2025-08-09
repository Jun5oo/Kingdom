using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine;

public class SummonAction : IAction
{
    private ActionType actionType;
    private HighlightLayer highlightLayer;
    private HighlightType highlightType;
   
    public ActionType ActionType { get { return actionType; } }
    public HighlightLayer HighlightLayer { get { return highlightLayer; } }
    public HighlightType HighlightType { get { return highlightType; } }
    public ActionPerformer Performer { get { return performer; } }

    // References 
    GridManager gridManager;
    HandManager handManager;
    TokenManager tokenManager;
    TokenFactory tokenFactory;
    SummonSystem summonSystem; 
    
    UnitCard card;
    Token token;
    ActionPerformer performer;

    Vector2Int targetPosition;
    List<Vector2Int> validPositions;

    public event Action OnActionComplete;
    public event Action OnActionCanceled;

    int currentCost; 
    public int Cost { get { return currentCost; } }

    public ResourceType resourceType;
    public ResourceType ResourceType { get { return resourceType; } }
    public int OwnerID { get { return card.OwnerID; } }

    public SummonAction(UnitCard card, ActionPerformer performer)
    {
        actionType = ActionType.Summon;
        highlightLayer = HighlightLayer.Action; 
        highlightType = HighlightType.SummonHighlight;

        this.gridManager = ServiceLocator.Get<GridManager>();
        this.handManager = ServiceLocator.Get<HandManager>();
        this.tokenManager = ServiceLocator.Get<TokenManager>();
        this.tokenFactory = ServiceLocator.Get<TokenFactory>(); 
        this.summonSystem = ServiceLocator.Get<SummonSystem>();

        this.card = card;
        this.token = null;

        this.performer = performer;

        validPositions = new List<Vector2Int>
        {
            new Vector2Int(1, 1),
            new Vector2Int(1, 0),
            new Vector2Int(1, -1),
            new Vector2Int(-1, 1),
            new Vector2Int(-1, 0),
            new Vector2Int(-1, -1),
            new Vector2Int(0, 1),
            new Vector2Int(0, -1)
        };

        currentCost = 1;

        resourceType = ResourceType.Action; 
    }
    public void Enter()
    {
        if(card == null)
        {
            Debug.LogError("The subject of the action is not exits");
            return; 
        }

        gridManager?.HighlightGridCells((Vector2Int gridPosition) => CanSummonAt(gridPosition), highlightType, highlightLayer); 
    }
    public async UniTask Execute(Vector2Int targetPosition)
    {
        this.targetPosition = targetPosition;
        await Transition(SummonState.Prepare); 
    }
    public void Exit() => gridManager.UnhighlightGridCells(highlightLayer);
    public bool IsValid()
    {
        return true; 
        // return ServiceLocator.Get<ActionSystem>().GetCurrentActionCount() >= currentCost;  
    }
    async UniTask Transition(SummonState state)
    {
        switch (state)
        {
            case SummonState.Prepare:
                await Prepare(); 
                break;
            case SummonState.Summon:
                await Summon(); 
                break;
            case SummonState.Placing:
                await Placing(); 
                break;
            case SummonState.Done:
                Done(); 
                break; 
        }
    } 
    async UniTask Prepare()
    {
        Exit();

        int playerID = card.OwnerID; 
        
        handManager.RemoveCardFromHand(playerID, card);
        
        CardMovement cardMovement = card.GetComponent<CardMovement>();  
        PRS prs = cardMovement.PRS;
        prs.position += Vector3.forward * 2f;

        var taskCompletion = new UniTaskCompletionSource(); 

        cardMovement.MoveTransform(prs, 0.5f, false, () => 
        {
            card.gameObject.SetActive(false);
            taskCompletion.TrySetResult();
        });

        await taskCompletion.Task; 
        await Transition(SummonState.Summon);
    }
    async UniTask Summon()
    {
        await summonSystem.Summon(card.OwnerID, card.UnitData, targetPosition); 
        await Transition(SummonState.Placing);
    }
    async UniTask Placing()
    {
        await Transition(SummonState.Done); 
    }
    void Done()
    {
        OnActionComplete?.Invoke();
    }
    private bool CanSummonAt(Vector2Int pos)
    {
        if (card.Tag == UnitTag.King)
        {
            if(handManager.IsMyCard(card))
                return pos.y < 1;
            else
                return pos.y >= 6; 
        }

        int playerID = card.OwnerID;

        if (tokenManager.TryGetKingTokenFrom(playerID, out Token kingToken))
        {
            Vector2Int gridPos = tokenManager.GetGridPositionOfToken(kingToken);

            foreach (var validPos in validPositions)
            {
                Vector2Int availablePos = gridPos + validPos; 
                if (!tokenManager.IsTokenAtGridPosition(availablePos) && availablePos == pos)
                    return true;
            }

            return false; 
        }

        else
        {
            Debug.LogError("왕 토큰이 존재하지 않습니다!");
            Exit();
            OnActionCanceled?.Invoke(); 
            return false;
        }
    }
}
