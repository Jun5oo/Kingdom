using Cysharp.Threading.Tasks;
using System;
using UnityEngine;

/// <summary>
/// 아군 유닛에게 신성 보호막(DivineShield) 버프를 부여하는 능력 액션.
/// 왕이 실행하며, 어빌리티 코인 2개를 소모한다.
/// 대상은 아군 일반 유닛(King 제외)이어야 한다.
/// </summary>
public class DivineShieldAction : IAction
{
    ActionType actionType;
    HighlightLayer highlightLayer;
    HighlightType highlightType; 
    ActionPerformer performer;

    Token token; 
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

    public ResourceType resourceType;
    public ResourceType ResourceType { get { return resourceType; } }
    public int OwnerID { get { return token.OwnerID; } }

    public BaseObject Executor => token;

    public Predicate<Vector2Int> Validation => CanBuff; 

    public DivineShieldAction(Token token, ActionPerformer performer)
    {
        actionType = ActionType.DivineShield;
        highlightLayer = HighlightLayer.Action;
        highlightType = HighlightType.SummonHighlight;
        this.performer = performer;

        this.gridManager = ServiceLocator.Get<GridManager>();
        this.tokenManager =ServiceLocator.Get<TokenManager>();

        this.token = token; 
        this.target = token;
        this.currentCost = 2;

        resourceType = ResourceType.Ability; 
    }

    public void Enter() { }

    /// <summary>
    /// 대상 위치의 토큰에게 DivineShield 버프를 적용한다.
    /// 대상이 없거나 IBuffable이 아니면 액션을 취소한다.
    /// </summary>
    public async UniTask Execute(Vector2Int targetPosition)
    {
        Exit();

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

    public void Exit() { }

    /// <summary>
    /// 버프 대상이 유효한지 검사한다.
    /// 아군이고, King이 아니며, IBuffable을 구현한 유닛이어야 한다.
    /// </summary>
    bool CanBuff(Vector2Int pos)
    {
        if (!tokenManager.IsTokenAtGridPosition(pos))
            return false;

        Token token = tokenManager.GetTokenFrom(pos);

        if (token != null)
        {
            if (!token.IsAllies(target.OwnerID))
                return false;
            if (token.Data.Tag == UnitTag.King)
                return false;
            if (token is not IBuffable)
                return false;

            return true;
        }

        return false;
    }

    public bool IsValid() => true;

}
