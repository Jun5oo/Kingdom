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

        // 공격 애니메이션 및 데미지 처리
        eventQueue.Enqueue(async () =>
        {
            var hit = new UniTaskCompletionSource();
            var end = new UniTaskCompletionSource(); 

            // 근접/원거리 공격 구분
            bool isMeleeAttack = IsMeleeAttack();
            
            if (isMeleeAttack)
            {
                tokenMovement.MeleeAttackTargetFrom(targetPosition, prs, onHitCallback: () =>
                {
                    Debug.Log("칼 휘두르기 히트!"); // Visual hit feedback
                    hit.TrySetResult(); // 히트 시점 알림
                }, onCompleteCallback: () =>
                {
                    Debug.Log($"공격 데미지 처리: {token} -> {target}, 데미지: {damage}");
                    damage = damageManager.ProcessDamage(token, target);
                    Debug.Log($"실제 적용된 데미지: {damage}, 타겟 HP: {target.CP}, 타겟 사망: {target.IsDead}");
                    end.TrySetResult(); 
                });
            }
            else
            {
                tokenMovement.RangeAttackTargetFrom(targetPosition, prs, onHitCallback: () =>
                {
                    Debug.Log("화살 히트!"); // Visual hit feedback
                    hit.TrySetResult(); // 히트 시점 알림
                }, onCompleteCallback: () =>
                {
                    Debug.Log($"공격 데미지 처리: {token} -> {target}, 데미지: {damage}");
                    damage = damageManager.ProcessDamage(token, target);
                    Debug.Log($"실제 적용된 데미지: {damage}, 타겟 HP: {target.CP}, 타겟 사망: {target.IsDead}");
                    end.TrySetResult(); 
                });
            }

            await hit.Task; 
            await end.Task; 
        });

        // 반격 데미지 계산 
        eventQueue.Enqueue(() =>
        {
            Debug.Log($"반격 데미지 처리: {target} -> {token}, 데미지: {counterDamage}");
            counterDamage = damageManager.ProcessCounterDamage(target, token, counterDamage);
            Debug.Log($"실제 반격 데미지: {counterDamage}, 공격자 HP: {token.CP}, 공격자 사망: {token.IsDead}");
            return UniTask.CompletedTask; 
        });

        // 왕에게의 간접 데미지 계산 
        eventQueue.Enqueue(() =>
        {
            damageManager.ProcessKingDamage(token, counterDamage);
            damageManager.ProcessKingDamage(target, damage);
            return UniTask.CompletedTask;
        });

        // 사망 처리 (왕의 HP 확인 전에)
        eventQueue.Enqueue(async () =>
        {
            // 타겟이 죽었는지 확인하고 처리
            if(target.TryGetComponent<IDestructible>(out IDestructible destructibleTarget))
            {
                if (target.IsDead)
                {
                    Debug.Log($"타겟 {target} 사망 처리");
                    await damageManager.ProcessUnitDeath(token, target); 
                }
            }

            // 공격자가 죽었는지 확인하고 처리
            if (token.TryGetComponent<IDestructible>(out IDestructible destructibleAttacker))
            {
                if (token.IsDead)
                {
                    Debug.Log($"공격자 {token} 사망 처리");
                    await damageManager.ProcessUnitDeath(target, token); 
                }
            }
        });

        // 왕의 HP 확인 (사망 처리 후)
        eventQueue.Enqueue(() =>
        {
            damageManager.IsKingDefeated();
            return UniTask.CompletedTask; 
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

    // 근접/원거리 공격 구분
    bool IsMeleeAttack()
    {
        Vector2Int currentGridPosition = tokenManager.GetGridPositionOfToken(token);
        Vector2Int targetGridPosition = this.targetPosition;
        int distance = Mathf.Abs(targetGridPosition.x - currentGridPosition.x) + 
                       Mathf.Abs(targetGridPosition.y - currentGridPosition.y);
        return distance == 1;
    }

    public bool IsValid()
    {
        if(attackablePositions.Count == 0) 
            return false;

        return ServiceLocator.Get<ActionSystem>().GetCurrentActionCount() >= currentCost;
    }
}
