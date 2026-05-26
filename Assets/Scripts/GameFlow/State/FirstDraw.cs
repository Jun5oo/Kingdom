using Cysharp.Threading.Tasks;
using System.Collections;
using UnityEngine;

/// <summary>
/// 초기 카드 드로우 후 턴 루프를 시작하는 상태 (현재 MainPhase와 역할 중복, 정리 예정).
/// 선·후공 각 3장 드로우 → 턴 순서 설정 → 턴 루프 개시 → 시스템 활성화 순으로 진행한다.
/// AI 대전이면 SelectionSystem을 비활성화하여 로컬 입력을 차단한다.
/// </summary>
public class FirstDraw : IGameState
{
    const int DRAW_NUM = 3;
    const float DRAW_COOLTIME = 0.3f;

    TurnSystem turnManager;
    DrawManager drawManager;
    HandManager handManager;

    PlayerManager playerManager;

    GameFlowStateMachine stateMachine;

    WaitForSeconds drawTime;

    public FirstDraw(GameFlowStateMachine stateMachine)
    {
        turnManager = ServiceLocator.Get<TurnSystem>();
        drawManager = ServiceLocator.Get<DrawManager>();
        handManager = ServiceLocator.Get<HandManager>();
        playerManager = ServiceLocator.Get<PlayerManager>();

        this.stateMachine = stateMachine;

        drawTime = new WaitForSeconds(DRAW_COOLTIME);
    }

    /// <summary>
    /// 드로우 → 턴 루프 시작 → 시스템 활성화 순으로 진행한다.
    /// AI 대전이면 SelectionSystem을 비활성화한다.
    /// </summary>
    public async UniTask Enter()
    {
        await Draw(stateMachine.firstID);
        await Draw(stateMachine.secondID);

        turnManager.SetTurnOrder(new int[] { stateMachine.firstID, stateMachine.secondID });
        await turnManager.BeginTurnLoop();

        SelectionSystem selectionSystem = ServiceLocator.Get<SelectionSystem>();
        ActionSystem actionSystem = ServiceLocator.Get<ActionSystem>();

        bool isAI = stateMachine.secondID != playerManager.Local.PlayerID;

        if (isAI)
            selectionSystem.DisableSystem();
        else
            selectionSystem.EnableSystem();

        actionSystem.EnableSystem();
    }

    /// <summary> 해당 플레이어에게 DRAW_NUM장을 드로우하여 핸드에 추가한다. </summary>
    async UniTask Draw(int playerID)
    {
        for (int i = 0; i < DRAW_NUM; i++)
        {
            Card card = await drawManager.Draw(playerID);
            handManager.AddCardToHand(playerID, card);
        }
    }
}
