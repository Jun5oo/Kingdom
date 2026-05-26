using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 필드 위의 무덤(Graveyard) 토큰을 원래 유닛으로 부활시키는 능력 액션.
/// 언데드 왕이 실행하며, 어빌리티 코인 2개를 소모한다.
/// </summary>
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

    TokenManager tokenManager;
    SummonSystem summonSystem; 

    Token actionOwner;

    Vector2Int targetPosition;

    int currentCost; 
    public int Cost { get { return currentCost;}}

    public ResourceType resourceType;
    public ResourceType ResourceType { get { return resourceType; } }
    public int OwnerID { get { return actionOwner.OwnerID; } }

    public BaseObject Executor => actionOwner;

    public Predicate<Vector2Int> Validation => CanRevive;

    public ResurrectionAction(Token token, ActionPerformer performer)
    {
        actionType = ActionType.Resurrection;
        highlightLayer = HighlightLayer.Action;
        highlightType = HighlightType.SummonHighlight;
        this.performer = performer;

        this.tokenManager = ServiceLocator.Get<TokenManager>(); 
        this.summonSystem  = ServiceLocator.Get<SummonSystem>();

        this.actionOwner = token;

        currentCost = 2;
        resourceType = ResourceType.Ability; 
    }

    public void Enter() { }

    /// <summary>
    /// 부활 대상이 유효한지 검사한다.
    /// 아군의 Graveyard 태그 토큰이 있어야 한다.
    /// </summary>
    bool CanRevive(Vector2Int gridPosition)
    {
        if (!tokenManager.IsTokenAtGridPosition(gridPosition))
            return false;

        Token token = tokenManager.GetTokenFrom(gridPosition);

        if (token != null)
        {
            if (!token.IsAllies(this.actionOwner.OwnerID))
                return false;
            if (token.Tag == UnitTag.Graveyard)
                return true;
        }

        return false;
    }

    /// <summary> 대상 위치를 저장하고 부활 로직을 실행한 뒤 EventQueue를 처리한다. </summary>
    public async UniTask Execute(Vector2Int targetPosition)
    {
        EventQueue eventQueue = ServiceLocator.Get<EventQueue>();

        this.targetPosition = targetPosition;
        Revive();

        await eventQueue.ExecuteAllAsync();
    }

    public void Exit() { }

    public bool IsValid() => true;

    /// <summary>
    /// 무덤 토큰을 제거하고, SourceObjects에 저장된 원본 CardData로 유닛을 다시 소환한다.
    /// </summary>
    public void Revive()
    {
        Exit();

        if (!tokenManager.IsTokenAtGridPosition(targetPosition))
        {
            OnActionCanceled?.Invoke();
            return;
        }

        Token targetToken = tokenManager.GetTokenFrom(targetPosition);
        var unitData = new List<CardData>(targetToken.SourceObjects);

        tokenManager.DestroyToken(targetToken);

        EventQueue eventQueue = ServiceLocator.Get<EventQueue>();

        eventQueue.Enqueue(async () =>
        {
            await summonSystem.Summon(actionOwner.OwnerID, unitData[0], targetPosition, actionOwner.Data);
            OnActionComplete?.Invoke();
        });
    }
}
