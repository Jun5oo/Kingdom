using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 유닛이 적 유닛을 공격하는 액션.
/// 근접/원거리 공격을 구분하며, 반격(카운터 어택) 및 왕에게의 간접 데미지를 처리한다.
/// EventQueue를 사용해 공격 → 반격 → 간접 데미지 → 사망 확인 순으로 순차 실행한다.
/// </summary>
public class AttackAction : IAction
{
    ActionType actionType;
    HighlightLayer highlightLayer;
    HighlightType highlightType;

    public ActionType ActionType { get { return actionType; } }
    public HighlightLayer HighlightLayer { get { return highlightLayer; } }
    public HighlightType HighlightType { get { return highlightType; } }
    public ActionPerformer Performer { get { return performer; } }

    GridManager gridManager;
    TokenManager tokenManager;
    DamageManager damageManager;
    EventQueue eventQueue;

    Token token;   // 공격을 수행하는 유닛
    Token target;  // 공격을 받는 유닛

    ActionPerformer performer;

    Vector2Int targetPosition;

    public List<Vector2Int> AttackablePositions { get; private set; } // 공격 가능한 상대 위치 목록

    public event Action OnActionCanceled;
    public event Action OnActionComplete;

    int currentCost;
    public int Cost { get { return currentCost; } }

    public ResourceType resourceType;
    public ResourceType ResourceType { get { return resourceType; } }
    public int OwnerID { get { return token.OwnerID; } }

    public BaseObject Executor => token;

    public Predicate<Vector2Int> Validation => CanAttack;

    /// <summary> 공격 액션 초기화. 공격자 토큰과 수행 주체(플레이어/AI)를 받아 설정한다. </summary>
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

        AttackablePositions = token.AttackRange;

        currentCost = 1;
        resourceType = ResourceType.Action;
    }

    public void Enter() { }

    /// <summary>
    /// 현재 토큰의 공격 범위 내에 해당 그리드 좌표가 있는지 검사한다.
    /// GridSelection의 Validation 조건으로 사용된다.
    /// </summary>
    bool CanAttack(Vector2Int gridPosition)
    {
        Vector2Int currentGridPosition = tokenManager.GetGridPositionOfToken(token);

        if (currentGridPosition == -Vector2Int.one)
            return false;

        foreach (Vector2Int position in AttackablePositions)
        {
            Vector2Int availablePosition = currentGridPosition + position;
            if (availablePosition == gridPosition)
                return true;
        }

        return false;
    }

    /// <summary>
    /// 대상 그리드 위치의 토큰을 검증한 뒤 공격 FSM을 시작한다.
    /// 대상이 없거나 아군이면 액션을 취소한다.
    /// </summary>
    public async UniTask Execute(Vector2Int targetPosition)
    {
        var target = tokenManager.GetTokenFrom(targetPosition);
        this.target = target;
        this.targetPosition = targetPosition;

        if (target == null)
        {
            Debug.Log("공격할 대상이 존재하지 않습니다!");
            OnActionCanceled?.Invoke();
            return;
        }

        if (!target.TryGetComponent<IDamageable>(out IDamageable damageable))
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

    public void Exit() { }

    /// <summary> 공격 FSM 상태 전환 </summary>
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
                await Placing();
                break;
            case AttackState.Done:
                Done();
                break;
        }
    }

    /// <summary>
    /// 공격 실행에 필요한 이벤트들을 EventQueue에 순서대로 등록한다.
    /// 순서: 공격 애니메이션 → 반격 → 왕에게 간접 데미지 → 방어자 사망 확인 → 공격자 사망 확인 → 왕 사망 확인
    /// </summary>
    async UniTask Prepare()
    {
        Exit();

        Vector3 targetPosition = gridManager.GetWorldPosition(this.targetPosition);

        TokenMovement tokenMovement = token.GetComponent<TokenMovement>();
        PRS prs = tokenMovement.PRS;

        int damage = token.CP;
        int counterDamage = target.CP;

        bool counterAttackOccurred = false; // 반격이 실제로 발생했는지 추적

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
        // 반격 애니메이션 및 데미지 처리
        eventQueue.Enqueue(async () =>
        {
            // 반격 가능 여부 미리 확인
            bool canCounter = CanCounterAttack(target, token);

            if (canCounter && counterDamage > 0)
            {
                counterAttackOccurred = true; // 반격이 발생함을 표시

                var counterHit = new UniTaskCompletionSource();
                var counterEnd = new UniTaskCompletionSource();

                Vector3 attackerPosition = gridManager.GetWorldPosition(tokenManager.GetGridPositionOfToken(token));
                TokenMovement targetMovement = target.GetComponent<TokenMovement>();
                PRS targetPRS = targetMovement.PRS;

                // 근접/원거리 반격 구분
                bool isMeleeCounter = IsMeleeCounterAttack(target); // 반격은 타겟의 유닛 타입으로 판단

                if (isMeleeCounter)
                {
                    targetMovement.MeleeAttackTargetFrom(attackerPosition, targetPRS, onHitCallback: () =>
                    {
                        Debug.Log("반격 칼 휘두르기 히트!");
                        counterHit.TrySetResult();
                    }, onCompleteCallback: () =>
                    {
                        Debug.Log($"반격 데미지 처리: {target} -> {token}, 데미지: {counterDamage}");
                        counterDamage = damageManager.ProcessCounterDamage(target, token, counterDamage);
                        Debug.Log($"실제 반격 데미지: {counterDamage}, 공격자 HP: {token.CP}, 공격자 사망: {token.IsDead}");
                        counterEnd.TrySetResult();
                    });
                }
                else
                {
                    targetMovement.RangeAttackTargetFrom(attackerPosition, targetPRS, onHitCallback: () =>
                    {
                        Debug.Log("반격 화살 히트!");
                        counterHit.TrySetResult();
                    }, onCompleteCallback: () =>
                    {
                        Debug.Log($"반격 데미지 처리: {target} -> {token}, 데미지: {counterDamage}");
                        counterDamage = damageManager.ProcessCounterDamage(target, token, counterDamage);
                        Debug.Log($"실제 반격 데미지: {counterDamage}, 공격자 HP: {token.CP}, 공격자 사망: {token.IsDead}");
                        counterEnd.TrySetResult();
                    });
                }

                await counterHit.Task;
                await counterEnd.Task;
            }
            else
            {
                Debug.Log("반격 불가능: 공격 범위 밖이거나 반격할 수 없는 상황");
                counterDamage = 0;
            }
        });
        // 왕에게의 간접 데미지 계산 
        eventQueue.Enqueue(() =>
        {
            Debug.Log("카운터 데미지 확인" + counterDamage);
            // 공격 데미지는 항상 타겟에게 간접 데미지 적용
            damageManager.ProcessIndirectDamage(token, counterDamage);
            damageManager.ProcessIndirectDamage(target, damage);
            return UniTask.CompletedTask;
        });
        // 사망여부확인 (Defender) 
        eventQueue.Enqueue(async () =>
        {
            // 타겟이 죽었는지 확인하고 처리
            if (target.TryGetComponent<IDestructible>(out IDestructible destructibleTarget))
            {
                if (destructibleTarget.IsDead)
                {
                    Debug.Log($"타겟 {target} 사망 처리");
                    damageManager.ProcessUnitDeath(token, target);
                }
            }
        });
        // 사망여부확인 (Attacker) 
        eventQueue.Enqueue(async () =>
        {
            // 공격자가 죽었는지 확인하고 처리
            if (token.TryGetComponent<IDestructible>(out IDestructible destructibleAttacker))
            {
                if (destructibleAttacker.IsDead)
                {
                    Debug.Log($"공격자 {token} 사망 처리");
                    damageManager.ProcessUnitDeath(target, token);
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

    /// <summary> Prepare에서 등록된 이벤트들을 순차 실행한다. </summary>
    async UniTask Attack()
    {
        await eventQueue.ExecuteAllAsync();
        await Transition(AttackState.Placing);
    }

    async UniTask Placing()
    {
        await Transition(AttackState.Done);
    }

    void Done() { }

    /// <summary>
    /// 공격 유닛이 근접 공격인지 원거리 공격인지 판별한다.
    /// AttackRange에 거리 2 이상의 벡터가 있으면 원거리 유닛으로 간주한다.
    /// </summary>
    bool IsMeleeAttack()
    {
        bool isRangedUnit = IsRangedUnit(token);
        if (isRangedUnit) return false;

        Vector2Int currentGridPosition = tokenManager.GetGridPositionOfToken(token);
        Vector2Int targetGridPosition = this.targetPosition;
        int distance = Mathf.Abs(targetGridPosition.x - currentGridPosition.x) +
                       Mathf.Abs(targetGridPosition.y - currentGridPosition.y);
        return distance == 1;
    }

    /// <summary> 해당 유닛이 원거리 유닛인지 확인한다 (AttackRange 최대 거리 기준). </summary>
    bool IsRangedUnit(Token unit)
    {
        if (unit.AttackRange == null || unit.AttackRange.Count == 0)
            return false;

        foreach (var range in unit.AttackRange)
        {
            int distance = Mathf.Abs(range.x) + Mathf.Abs(range.y);
            if (distance >= 2) return true;
        }

        return false;
    }

    /// <summary> 반격 시 방어자가 근접 반격인지 원거리 반격인지 판별한다. </summary>
    bool IsMeleeCounterAttack(Token defender)
    {
        bool isRangedUnit = IsRangedUnit(defender);
        if (isRangedUnit) return false;

        Vector2Int defenderPos = tokenManager.GetGridPositionOfToken(defender);
        Vector2Int attackerPos = tokenManager.GetGridPositionOfToken(token);
        int distance = Mathf.Abs(attackerPos.x - defenderPos.x) +
                       Mathf.Abs(attackerPos.y - defenderPos.y);
        return distance == 1;
    }

    /// <summary>
    /// 방어자가 공격자를 반격할 수 있는지 확인한다.
    /// 방어자의 AttackRange 내에 공격자가 위치하면 반격 가능하다.
    /// </summary>
    bool CanCounterAttack(Token defender, Token attacker)
    {
        if (defender.AttackRange == null || defender.AttackRange.Count == 0)
            return false;

        Vector2Int defenderPos = tokenManager.GetGridPositionOfToken(defender);
        Vector2Int attackerPos = tokenManager.GetGridPositionOfToken(attacker);

        foreach (var position in defender.AttackRange)
        {
            if (defenderPos + position == attackerPos)
                return true;
        }

        return false;
    }

    /// <summary> 공격 범위 내에 적이 있는 경우에만 유효한 액션으로 판단한다. </summary>
    public bool IsValid()
    {
        if (AttackablePositions.Count == 0)
            return false;

        return true;
    }
}
