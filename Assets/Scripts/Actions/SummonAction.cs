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
    PlayerHandManager handManager;
    TokenManager tokenManager;
    TokenFactory tokenFactory; 
    
    UnitCard card;
    Token token;
    ActionPerformer performer;

    Vector2Int targetPosition;
    List<Vector2Int> validPositions;

    public event Action OnActionComplete;
    public event Action OnActionCanceled;

    int currentCost; 
    public int Cost { get { return currentCost; } }

    public SummonAction(UnitCard card, ActionPerformer performer)
    {
        actionType = ActionType.Summon;
        highlightLayer = HighlightLayer.Action; 
        highlightType = HighlightType.SummonHighlight;

        this.gridManager = ServiceLocator.Get<GridManager>();
        this.handManager = ServiceLocator.Get<PlayerHandManager>();
        this.tokenManager = ServiceLocator.Get<TokenManager>();
        this.tokenFactory = ServiceLocator.Get<TokenFactory>(); 
        
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
    public void Execute(Vector2Int targetPosition)
    {
        this.targetPosition = targetPosition;
        Transition(SummonState.Prepare); 
    }
    public void Exit() => gridManager.UnhighlightGridCells(highlightLayer);
    public bool IsValid()
    {
        return ServiceLocator.Get<ActionSystem>().GetCurrentActionCount() >= currentCost;  
    }
    void Transition(SummonState state)
    {
        switch (state)
        {
            case SummonState.Prepare:
                Prepare(); 
                break;
            case SummonState.Animation:
                Summon(); 
                break;
            case SummonState.Placing:
                Placing(); 
                break;
            case SummonState.Done:
                Done(); 
                break; 
        }
    } 
    void Prepare()
    {
        Exit();

        int playerID = card.OwnerPlayerID; 
        
        handManager.RemoveCardFromHand(playerID, card);
        
        CardMovement cardMovement = card.GetComponent<CardMovement>();  
        PRS prs = cardMovement.PRS;
        prs.position += Vector3.forward * 2f;

        cardMovement.MoveTransform(prs, 0.5f, false, () => 
        { 
            Transition(SummonState.Animation);
            card.gameObject.SetActive(false);
        });
    }
    void Summon()
    {
        Vector3 worldPosition = gridManager.GetWorldPosition(targetPosition); 

        Vector3 targetPos = worldPosition + (Vector3.up * 0.1f);
        Vector3 eulerAngles = new Vector3(90f, 0f, 0f);
        Quaternion quaternion = Quaternion.Euler(eulerAngles);
        Vector3 scale = Vector3.one;

        PRS prs = new PRS(targetPos, quaternion, scale);

        Token token = tokenFactory.CreateToken(card.UnitCardData, card.OwnerPlayerID);
        token.transform.position = targetPos + (Vector3.up * 10);
        token.transform.rotation = quaternion; 
        this.token = token;
    
        if (token.IsKing)
            tokenManager.AddKingToken(card.OwnerPlayerID, token); 

        TokenMovement tokenMovement = token.GetComponent<TokenMovement>();
        tokenMovement.MoveTransform(prs, 1f, false, () => { Transition(SummonState.Placing); }); 
    }
    void Placing()
    {
        tokenManager.PlaceTokenTo(token, targetPosition); 
        Transition(SummonState.Done); 
    }
    void Done()
    {
        OnActionComplete?.Invoke();
    }

    public int GetGridPosYForKing()
    {
        if (handManager.IsMyCard(card))
            return 0;
        else
            return 6;
    }

    private bool CanSummonAt(Vector2Int pos)
    {
        if (card.IsKing)
        {
            return pos.y == GetGridPosYForKing();
        }

        int playerID = card.OwnerPlayerID;

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
