using Cysharp.Threading.Tasks;
using NUnit.Framework;
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

    ActionType[] actionTypes;

    AISummonStrategy aiSummonStrategy;

    ActionSystem actionSystem;

    public void Init()
    {
        // AI 초기화 로직
        gridManager = ServiceLocator.Get<GridManager>();
        actionFactory = ServiceLocator.Get<ActionFactory>();
        handManager = ServiceLocator.Get<HandManager>();
        actionTypes = Enum.GetValues(typeof(ActionType)) as ActionType[]; // 모든 ActionType을 가져옴

        tokenManager = ServiceLocator.Get<TokenManager>();

        aiSummonStrategy = new AISummonStrategy(handManager, actionFactory, tokenManager);

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

        List<ActionType> availableActions = new List<ActionType>();
        int actionCount = actionSystem.GetCurrentActionCount();

        while (actionCount > 0)
        {
            bool canSummon = aiSummonStrategy.CanSummonAction(currentPlayerID, out SummonAction summonAction,
                out List<Vector2Int> validGridPosListForSummon);
            //bool canMove = gridManager.CanMoveAction(currentPlayerID);
            //bool canAttack = gridManager.CanAttackAction(currentPlayerID);

            if (canSummon)
                availableActions.Add(ActionType.Summon);

            ActionType randomAction = GetRandomAction(availableActions);

            switch (randomAction)
            {
                // 전체 action들이 가능한지 검사, action들을 다 수행했는지 검사 후 턴 넘김
                case ActionType.Summon:
                    await aiSummonStrategy.SummonRandomPos(summonAction, validGridPosListForSummon);
                    // 현재 자신의 영역 중 유효한 그리드 찾아서 배치 (유닛이 없는 곳에 배치)
                    break;
                case ActionType.Move:
                    // 현재 자신의 필드 내 유닛 중 하나를 선택하여 이동 (이동이 가능한지 체크 : 상하좌우)
                    break;
                case ActionType.Attack:
                    // 현재 자신의 필드 내 유닛들을 검사하여 공격이 가능한지 체크 후 실행
                    break;
            }

            actionCount--;
        }

        await OnAllActionsDone.Invoke();
    }

    private ActionType GetRandomAction(List<ActionType> availableActions)
    {
        int randomIndex = UnityEngine.Random.Range(0, availableActions.Count);
        return availableActions[randomIndex];
    }
}
