using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// AI 플레이어의 행동을 결정하는 컨트롤러.
/// 공격 → 부활/신성방패 → 소환/이동 우선순위로 매 AP마다 전략 클래스에 위임한다.
/// 가중치 기반 무작위 선택(GetRandomAction)으로 소환과 이동 중 하나를 고른다.
/// </summary>
public class AIController
{
    GridManager gridManager;
    ActionFactory actionFactory;
    HandManager handManager;

    TokenManager tokenManager;
    AbilityResourceSystem abilitySystem;

    ActionType[] actionTypes;

    AISummonStrategy aiSummonStrategy;
    AIMoveStrategy aiMoveStrategy;
    AIAttackStrategy aiAttackStrategy;
    AIResurrectionStrategy aiResurrectionStrategy;
    AIDivineShieldStrategy aiDivineShieldStrategy;

    ActionSystem actionSystem;

    // 소환과 이동의 기본 선택 가중치 (각 0.5 = 동확률)
    Dictionary<ActionType, float> baseWeights = new Dictionary<ActionType, float>
    {
        { ActionType.Move, 0.5f },
        { ActionType.Summon, 0.5f },
    };

    /// <summary> 전략 클래스들을 생성하고 서비스 참조를 초기화한다. </summary>
    public void Init()
    {
        gridManager = ServiceLocator.Get<GridManager>();
        actionFactory = ServiceLocator.Get<ActionFactory>();
        handManager = ServiceLocator.Get<HandManager>();
        actionTypes = Enum.GetValues(typeof(ActionType)) as ActionType[];

        tokenManager = ServiceLocator.Get<TokenManager>();

        abilitySystem = ServiceLocator.Get<AbilityResourceSystem>();

        aiSummonStrategy = new AISummonStrategy(handManager, actionFactory, tokenManager);
        aiMoveStrategy = new AIMoveStrategy(actionFactory, tokenManager);
        aiAttackStrategy = new AIAttackStrategy(actionFactory, tokenManager);
        aiResurrectionStrategy = new AIResurrectionStrategy(actionFactory, tokenManager, abilitySystem);
        aiDivineShieldStrategy = new AIDivineShieldStrategy(actionFactory, tokenManager, abilitySystem);

        actionSystem = ServiceLocator.Get<ActionSystem>();
    }

    /// <summary> AI가 왕을 무작위 X 위치에 배치한다. </summary>
    public async void DecideKingPlacement(SummonAction summon)
    {
        int posY = summon.GetGridPosYForKing();
        Vector2Int targetPos = new Vector2Int(gridManager.GetRandomGridXPos(), posY);
        await summon.Execute(targetPos);
    }

    /// <summary>
    /// 남은 AP만큼 우선순위 순으로 액션을 수행한다.
    /// 공격 → 부활/신성방패 → 소환/이동 순으로 시도하고, 아무것도 할 수 없으면 루프를 종료한다.
    /// 모든 액션 소진 후 OnAllActionsDone을 호출한다.
    /// </summary>
    public async void InvokeRandomAction(int currentPlayerID, Func<UniTask> OnAllActionsDone)
    {

        int actionCount = actionSystem.GetCurrentActionCount(currentPlayerID);
        List<ActionType> availableActions = new List<ActionType>();

        while (actionCount > 0)
        {
            if (tokenManager.GetPlayerToken(currentPlayerID).Count == 0)
            {
                if (aiSummonStrategy.CanSummonAction(currentPlayerID, out SummonAction summon, out List<Vector2Int> validGridPosList))
                {
                    actionCount--;
                    await aiSummonStrategy.SummonRandomPos(summon, validGridPosList);
                    actionSystem.EnterAI(summon);
                    await actionSystem.OnActionComplete();
                    continue;
                }
                else
                {
                    Debug.LogWarning("No tokens available for AI to perform actions.");
                    break;
                }
            }

            bool canAttack = aiAttackStrategy.CanAttackAction(currentPlayerID, out AttackAction attackAction,
                out List<Vector2Int> validGridPosListForAttack);

            if (canAttack)
            {
                actionCount--;
                await aiAttackStrategy.AttackRandomTarget(attackAction, validGridPosListForAttack);
                actionSystem.EnterAI(attackAction);
                await actionSystem.OnActionComplete();
                continue;
            }

            bool canResurrection = aiResurrectionStrategy.CanResurrectionAction(currentPlayerID, out ResurrectionAction resurrectionAction,
                out Vector2Int validGridPosForResurrection);
            bool canDivineShield = aiDivineShieldStrategy.CanDivineShieldAction(currentPlayerID, out DivineShieldAction divineShieldAction,
                out Vector2Int validGridPosForDivineShield);

            if (canResurrection)
            {
                actionCount--;
                await aiResurrectionStrategy.ResurrectionUnit(resurrectionAction, validGridPosForResurrection);
                actionSystem.EnterAI(resurrectionAction);
                await actionSystem.OnActionComplete();
                continue;
            }

            if (canDivineShield)
            {
                actionCount--;
                await aiDivineShieldStrategy.DivineShieldUnit(divineShieldAction, validGridPosForDivineShield);
                actionSystem.EnterAI(divineShieldAction);
                await actionSystem.OnActionComplete();
                continue;
            }

            bool canSummon = aiSummonStrategy.CanSummonAction(currentPlayerID, out SummonAction summonAction,
                out List<Vector2Int> validGridPosListForSummon);
            bool canMove = aiMoveStrategy.CanMoveAction(currentPlayerID, out MoveAction moveAction,
                out List<Vector2Int> validGridPosListForMove);


            if (canSummon)
            {
                availableActions.Add(ActionType.Summon);
            }
            if (canMove)
            {
                availableActions.Add(ActionType.Move);
            }
            if (availableActions.Count == 0)
            {
                break;
            }

            ActionType randomAction = GetRandomAction(availableActions);

            switch (randomAction)
            {
                // 전체 action들이 가능한지 검사, action들을 다 수행했는지 검사 후 턴 넘김
                case ActionType.Summon:
                    await aiSummonStrategy.SummonRandomPos(summonAction, validGridPosListForSummon);
                    actionSystem.EnterAI(summonAction);
                    await actionSystem.OnActionComplete();
                    // 현재 자신의 영역 중 유효한 그리드 찾아서 배치 (유닛이 없는 곳에 배치)
                    break;
                case ActionType.Move:
                    await aiMoveStrategy.MoveRandomPos(moveAction, validGridPosListForMove);
                    actionSystem.EnterAI(moveAction);
                    await actionSystem.OnActionComplete();
                    // 현재 자신의 필드 내 유닛 중 하나를 선택하여 이동 (이동이 가능한지 체크 : 상하좌우)
                    break;
            }

            actionCount--;
            availableActions.Clear();
        }

        await OnAllActionsDone.Invoke();
    }

    /// <summary>
    /// 가중치 기반 무작위로 ActionType을 선택한다.
    /// baseWeights 합산 후 정규화하여 누적 확률로 선택한다.
    /// </summary>
    private ActionType GetRandomAction(List<ActionType> availableActions)
    {
        Dictionary<ActionType, float> weightActionDict = new Dictionary<ActionType, float>();

        foreach (var actionType in availableActions)
        {
            weightActionDict.Add(actionType, baseWeights[actionType]);
        }

        float total = weightActionDict.Values.Sum();

        var normalized = weightActionDict.ToDictionary(
            pair => pair.Key,
            pair => pair.Value / total
        );

        float r = UnityEngine.Random.value;
        float cumulative = 0;

        foreach (var pair in normalized)
        {
            cumulative += pair.Value;
            if (r <= cumulative)
            {
                return pair.Key;
            }
        }

        return ActionType.Summon;
    }
}
