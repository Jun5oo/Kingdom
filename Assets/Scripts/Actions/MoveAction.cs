using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 유닛을 그리드 위의 다른 셀로 이동시키는 액션.
/// 이동 범위(MoveableRange) 내에서 빈 칸으로만 이동 가능하다.
/// </summary>
public class MoveAction : IAction
{
    ActionType actionType;
    HighlightLayer highlightLayer;
    HighlightType highlightType;
    ActionPerformer performer;

    public ActionType ActionType { get { return actionType; } }
    public HighlightLayer HighlightLayer { get { return highlightLayer; } }
    public HighlightType HighlightType { get { return highlightType; } }
    public ActionPerformer Performer { get { return performer; } }

    GridManager gridManager;
    TokenManager tokenManager;
    Token token;

    public List<Vector2Int> MoveablePositions { get; private set; }

    Vector2Int targetPosition;

    public event Action OnActionCanceled;
    public event Action OnActionComplete;

    int currentCost;
    public int Cost { get { return currentCost; } }

    public ResourceType resourceType;
    public ResourceType ResourceType { get { return resourceType; } }

    public int OwnerID { get { return token.OwnerID; } }

    public BaseObject Executor => token;

    public Predicate<Vector2Int> Validation => CanMoveTo;

    /// <summary> 이동 액션 초기화. 이동할 토큰과 수행 주체(플레이어/AI)를 받아 설정한다. </summary>
    public MoveAction(Token token, ActionPerformer performer)
    {
        actionType = ActionType.Move;
        highlightLayer = HighlightLayer.Action;
        highlightType = HighlightType.MoveHighlight;

        this.gridManager = ServiceLocator.Get<GridManager>();
        this.tokenManager = ServiceLocator.Get<TokenManager>();
        this.performer = performer;

        this.token = token;
        this.MoveablePositions = token.MoveableRange;

        currentCost = 1;

        resourceType = ResourceType.Action;
    }

    public void Enter() { }

    /// <summary> 대상 그리드 좌표를 저장하고 이동 FSM을 시작한다. </summary>
    public async UniTask Execute(Vector2Int gridPosition)
    {
        if (token == null)
        {
            OnActionCanceled?.Invoke();
            return;
        }

        targetPosition = gridPosition;
        await Transition(MoveState.Prepare);
    }

    public void Exit() { }

    /// <summary> 이동 가능한 위치가 존재하는 경우에만 유효한 액션으로 판단한다. </summary>
    public bool IsValid()
    {
        if (MoveablePositions.Count == 0)
            return false;

        return true;
    }

    /// <summary> 이동 FSM 상태 전환 </summary>
    async UniTask Transition(MoveState state)
    {
        switch (state)
        {
            case MoveState.Prepare:
                await Prepare();
                break;
            case MoveState.Animation:
                await Move();
                break;
            case MoveState.Placing:
                await Placing();
                break;
            case MoveState.Done:
                Done();
                break;
            default:
                Debug.LogError("Undefined MoveState");
                return;
        }
    }

    async UniTask Prepare()
    {
        await Transition(MoveState.Animation);
    }

    /// <summary> DOTween으로 토큰을 목표 위치까지 이동시키는 애니메이션을 실행한다. </summary>
    async UniTask Move()
    {
        TokenMovement tokenMovement = token.GetComponent<TokenMovement>();

        Vector3 targetWorldPos = gridManager.GetWorldPosition(targetPosition);
        Quaternion quaternion = tokenMovement.PRS.rotation;
        Vector3 scale = tokenMovement.PRS.scale;

        var taskComplete = new UniTaskCompletionSource();

        tokenMovement.MoveTransform(new PRS(targetWorldPos, quaternion, scale), 0.5f, false, () =>
        {
            taskComplete.TrySetResult();
        });

        await taskComplete.Task;
        await Transition(MoveState.Placing);
    }

    /// <summary> 애니메이션 완료 후 TokenManager에 실제 그리드 위치를 갱신한다. </summary>
    async UniTask Placing()
    {
        tokenManager.MoveTokenTo(token, targetPosition);
        await Transition(MoveState.Done);
    }

    /// <summary>
    /// 이동 목표 위치가 유효한지 검사한다.
    /// 토큰의 MoveableRange 내에 있고, 해당 위치에 다른 토큰이 없어야 한다.
    /// </summary>
    bool CanMoveTo(Vector2Int pos)
    {
        if (token == null || tokenManager == null)
            return false;

        var currentPos = tokenManager.GetGridPositionOfToken(token);

        foreach (Vector2Int delta in MoveablePositions)
        {
            Vector2Int availablePosition = currentPos + delta;
            if (availablePosition == pos && !tokenManager.IsTokenAtGridPosition(pos))
                return true;
        }

        return false;
    }

    void Done() { }
}
