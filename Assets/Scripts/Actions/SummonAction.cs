using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

/// <summary>
/// 손패의 카드를 그리드 위에 토큰으로 소환하는 액션.
/// 왕 유닛은 자신의 진영 Y 라인에만, 일반 유닛은 아군 왕 주변 8칸 중 빈 칸에만 소환 가능하다.
/// </summary>
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
    SummonSystem summonSystem;

    Card card;
    ActionPerformer performer;

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

    public BaseObject Executor => card;

    public Predicate<Vector2Int> Validation => CanSummonAt;

    public SummonAction(Card card, ActionPerformer performer)
    {
        actionType = ActionType.Summon;
        highlightLayer = HighlightLayer.Action;
        highlightType = HighlightType.SummonHighlight;

        this.gridManager = ServiceLocator.Get<GridManager>();
        this.handManager = ServiceLocator.Get<HandManager>();
        this.tokenManager = ServiceLocator.Get<TokenManager>();
        this.summonSystem = ServiceLocator.Get<SummonSystem>();

        this.card = card;
        this.performer = performer;

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
        if (card == null)
        {
            Debug.LogError("null 인 카드입니다.");
            return;
        }
    }

    /// <summary> 대상 그리드 좌표를 저장하고 소환 FSM을 시작한다. </summary>
    public async UniTask Execute(Vector2Int targetPosition)
    {
        this.targetPosition = targetPosition;
        await Transition(SummonState.Prepare);
    }

    public void Exit() => gridManager.UnhighlightGridCells(highlightLayer);

    public bool IsValid() => true;
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
    /// <summary>
    /// 손패에서 카드를 제거하고 카드를 위로 올리는 연출을 실행한 뒤 소환 단계로 진행한다.
    /// </summary>
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

    /// <summary> SummonSystem을 통해 대상 위치에 토큰을 생성한다. </summary>
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

    /// <summary>
    /// 왕 카드 소환 시 배치 가능한 Y 좌표를 반환한다.
    /// 로컬 플레이어는 Y=0, 원격 플레이어는 Y=6 라인이다.
    /// </summary>
    public int GetGridPosYForKing()
    {
        if (handManager.IsMyCard(card))
            return 0;
        else
            return 6;
    }

    /// <summary>
    /// 소환 위치가 유효한지 검사한다.
    /// 왕은 자신의 Y 라인에만, 일반 유닛은 아군 왕 주변 8칸 중 빈 칸에만 소환 가능하다.
    /// </summary>
    public bool CanSummonAt(Vector2Int pos)
    {
        if (card.Tag == UnitTag.King)
            return pos.y == GetGridPosYForKing();

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

    /// <summary> 소유자 플레이어의 왕 토큰 그리드 위치를 반환한다. </summary>
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
