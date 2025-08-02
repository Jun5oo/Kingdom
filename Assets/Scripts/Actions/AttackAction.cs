using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine;

public class AttackAction : IAction
{
    ActionType actionType;
    HighlightLayer highlightLayer;
    HighlightType highlightType;

    public ActionType ActionType { get { return actionType; } }
    public HighlightLayer HighlightLayer { get { return highlightLayer; } }
    public HighlightType HighlightType {  get { return highlightType; } }
    public ActionPerformer Performer {  get {  return performer; } }

    GridManager gridManager;
    TokenManager tokenManager;
    DamageManager damageManager;
    EventQueue eventQueue; 

    Token token;
    Token target;
    ActionPerformer performer;

    Vector2Int targetPosition; 

    List<Vector2Int> attackablePositions;

    public event Action OnActionComplete;
    public event Action OnActionCanceled;

    int currentCost; 
    public int Cost { get { return currentCost; } }

    public AttackAction(Token token, ActionPerformer performer)
    {
        actionType = ActionType.Attack;
        highlightLayer = HighlightLayer.Action;
        highlightType = HighlightType.AttackHighlight;

        this.gridManager = ServiceLocator.Get<GridManager>(); 
        this.damageManager = ServiceLocator.Get<DamageManager>();
        this.tokenManager = ServiceLocator.Get<TokenManager>(); 
        this.eventQueue = ServiceLocator.Get<EventQueue>();

        this.token = token;
        this.performer = performer;

        attackablePositions = token.AttackRange;

        currentCost = 1; 
    }

    public void Enter()
    {
        gridManager.HighlightGridCells((Vector2Int gridPosition) =>
        {
            Vector2Int currentGridPosition = tokenManager.GetGridPositionOfToken(token);

            if (currentGridPosition == -Vector2Int.one)
                return false; 

            foreach (Vector2Int position in attackablePositions)
            {
                Vector2Int availablePosition = currentGridPosition + position;
                if (availablePosition == gridPosition)
                    return true;
            }

            return false;

        }, HighlightType.AttackHighlight, HighlightLayer.Action);
    }
    public async UniTask Execute(Vector2Int targetPosition)
    {
        var target = tokenManager.GetTokenFrom(targetPosition);
        this.target = target;
        this.targetPosition = targetPosition; 

        if(target == null)
        {
            Debug.Log("공격할 대상이 존재하지 않습니다!");
            OnActionCanceled?.Invoke(); 
            return; 
        }

        if(!target.TryGetComponent<IDamageable>(out IDamageable damageable))
        {
            Debug.Log("공격할 수 없는 대상입니다!");
            OnActionCanceled?.Invoke();
            return; 
        }

        if (damageable.IsAllies(token.OwnerID))
        {
            Debug.Log("아군을 공격할 수 없습니다!");
            OnActionCanceled?.Invoke();
            return; 
        }

        await Transition(AttackState.Prepare); 
    }

    public void Exit()
    {
        gridManager?.UnhighlightGridCells(HighlightLayer.Action);
        gridManager?.UnhighlightGridCells(HighlightLayer.Hover); 
    }

    async UniTask Transition(AttackState state)
    {
        switch (state)
        {
            case AttackState.Prepare:
                await Prepare(); 
                break;
            case AttackState.Attack:
                await Attack(); 
                break;
            case AttackState.Placing:
                Placing(); 
                break;
            case AttackState.Done:
                Done(); 
                break;
        }
    }

    async UniTask Prepare()
    {
        Exit();

        // EventQueue에 데미지 계산 순서를 넣기. 

        Vector3 targetPosition = gridManager.GetWorldPosition(this.targetPosition);

        TokenMovement tokenMovement = token.GetComponent<TokenMovement>();
        PRS prs = tokenMovement.PRS;

        int damage = token.CP;
        int counterDamage = target.CP;

        // 공격 애니메이션 
        eventQueue.Enqueue(async () =>
        {
            var hit = new UniTaskCompletionSource();
            var end = new UniTaskCompletionSource(); 

            tokenMovement.AttackTargetFrom(targetPosition, prs, onHitCallback: () =>
            {
                damage = damageManager.ProcessDamage(token, target);
                hit.TrySetResult();
            }, onCompleteCallback: () =>
            {
                end.TrySetResult(); 
            });

            await hit.Task; 
            await end.Task; 
        });
        // 반격 데미지 계산 
        eventQueue.Enqueue(() =>
        {
            counterDamage = damageManager.ProcessCounterDamage(target, token, counterDamage);
            return UniTask.CompletedTask; 
        });
        // 왕에게의 간접 데미지 계산 
        eventQueue.Enqueue(() =>
        {
            damageManager.ProcessKingDamage(token, counterDamage);
            damageManager.ProcessKingDamage(target, damage);
            return UniTask.CompletedTask;
        });

        // 왕의 HP 확인 
        eventQueue.Enqueue(() =>
        {
            damageManager.IsKingDefeated();
            return UniTask.CompletedTask; 
        });

        eventQueue.Enqueue(async () =>
        {
            if(target.TryGetComponent<IDestructible>(out IDestructible destructibleTarget))
            {
                if (target.IsDead)
                    await damageManager.ProcessUnitDeath(token, target); 
            }

            if (token.TryGetComponent<IDestructible>(out IDestructible destructibleAttacker))
            {
                if (token.IsDead)
                    await damageManager.ProcessUnitDeath(target, token); 
            }
        }); 

        await Transition(AttackState.Attack); 
    }
    
    async UniTask Attack()
    {
        await eventQueue.ExecuteAllAsync(); 
        await Transition(AttackState.Placing);
    }

    async UniTask Placing()
    {
        await Transition(AttackState.Done);
    }

    void Done()
    {
        OnActionComplete?.Invoke();
    }

    public bool IsValid()
    {
        if(attackablePositions.Count == 0) 
            return false;

        return ServiceLocator.Get<ActionSystem>().GetCurrentActionCount() >= currentCost;
    }
}
