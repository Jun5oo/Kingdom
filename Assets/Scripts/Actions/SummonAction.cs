using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine;

public class SummonAction : IGameAction
{
    private ActionType actionType;
    public ActionType ActionType { get { return actionType; } }

    // References 
    HandManager handManager;
    TokenManager tokenManager;
    SummonSystem summonSystem; 

    Card card;

    Vector2Int targetPosition;
    public List<Vector2Int> ValidPositions { get; private set; }

    public event Action OnActionCanceled;
    public event Action OnActionComplete;

    int currentCost; 
    public int Cost { get { return currentCost; } }

    public SummonState CurrentSummonState { get; private set; } = SummonState.Prepare;
    public ResourceType resourceType;
    public ResourceType ResourceType { get { return resourceType; } }
    public int OwnerID { get { return card.OwnerID; } }

    public BaseObject BaseObject => card;

    public Predicate<Vector2Int> Validation => CanSummonAt;

    public SummonAction(Card card)
    {
        actionType = ActionType.Summon;

        this.handManager = ServiceLocator.Get<HandManager>();
        this.tokenManager = ServiceLocator.Get<TokenManager>();
        this.summonSystem = ServiceLocator.Get<SummonSystem>();

        this.card = card;

        ValidPositions = new List<Vector2Int>
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
            Debug.LogError("null 인 카드입니다.");
            return; 
        }

    }
    public async UniTask Execute(Vector2Int targetPosition)
    {
        this.targetPosition = targetPosition;
        await Transition(SummonState.Prepare); 
    }
    public void Exit()
    {

    }
    public bool IsValid()
    {
        return true; 
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
                CurrentSummonState = SummonState.Done;
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
        prs.position += Vector3.up * 2f;

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
        await summonSystem.Summon(card.OwnerID, card.Data, targetPosition); 
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

    public int GetGridPosYForKing()
    {
        if (handManager.IsMyCard(card))
            return 0;
        else
            return 6;
    }

    public bool CanSummonAt(Vector2Int pos)
    {
        if (card.Tag == UnitTag.King)
        {
            return pos.y == GetGridPosYForKing();
        }

        if (TryGetKingTokenPos(out Vector2Int gridPos))
        {
            foreach (var validPos in ValidPositions)
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

    public bool TryGetKingTokenPos(out Vector2Int gridPos)
    {
        int playerID = card.OwnerID;
        if (tokenManager.TryGetKingTokenFrom(playerID, out Token kingToken))
        {
            gridPos = tokenManager.GetGridPositionOfToken(kingToken);
            return true;
        }

        gridPos = Vector2Int.zero;
        return false;
    }
}
