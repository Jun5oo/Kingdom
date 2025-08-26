using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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

    Dictionary<ActionType, float> baseWeights = new Dictionary<ActionType, float>
    {
        { ActionType.Move, 0.5f },
        { ActionType.Summon, 0.5f },
    };

    public void Init()
    {
        // AI 초기화 로직
        gridManager = ServiceLocator.Get<GridManager>();
        actionFactory = ServiceLocator.Get<ActionFactory>();
        handManager = ServiceLocator.Get<HandManager>();
        actionTypes = Enum.GetValues(typeof(ActionType)) as ActionType[]; // 모든 ActionType을 가져옴

        tokenManager = ServiceLocator.Get<TokenManager>();

        abilitySystem = ServiceLocator.Get<AbilityResourceSystem>();

        aiSummonStrategy = new AISummonStrategy(handManager, actionFactory, tokenManager);
        aiMoveStrategy = new AIMoveStrategy(actionFactory, tokenManager);
        aiAttackStrategy = new AIAttackStrategy(actionFactory, tokenManager);
        aiResurrectionStrategy = new AIResurrectionStrategy(actionFactory, tokenManager, abilitySystem);
        aiDivineShieldStrategy = new AIDivineShieldStrategy(actionFactory, tokenManager, abilitySystem);

        actionSystem = ServiceLocator.Get<ActionSystem>();
    }

    public async void DecideKingPlacement(SummonAction summon)
    {
        int posY = summon.GetGridPosYForKing();
        Vector2Int targetPos = new Vector2Int(gridManager.GetRandomGridXPos(), posY);
        await summon.Execute(targetPos); // AI places the king at the center of the board
    }

    public async void InvokeRandomAction(int currentPlayerID, Func<UniTask> OnAllActionsDone)
    {
        // 할 수 있는 액션 나열
        // 1. 필드 위에 있는 오브젝트들 이동 및 공격
        // 2. 새로운 유닛 소환
        // 3. 영웅 능력 사용 (현재는 제외)

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
