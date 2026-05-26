using Cysharp.Threading.Tasks;
using UnityEngine;

/// <summary>
/// 본 게임 메인 페이즈를 시작하는 상태.
/// 선·후공 플레이어에게 초기 3장씩 드로우하고 TurnSystem/SelectionSystem/ActionSystem을 활성화한다.
/// </summary>
public class MainPhase : IGameState
{
    const int DRAW_NUM = 3;
    const int DRAW_COOLTIME_MS = 300;

    GameFlowStateMachine stateMachine;

    public MainPhase(GameFlowStateMachine stateMachine)
    {
        this.stateMachine = stateMachine;
    }

    /// <summary>
    /// 선·후공 순서로 각 3장을 드로우하고, 턴 순서를 설정한 뒤 시스템을 활성화한다.
    /// </summary>
    public async UniTask Enter()
    {
        Debug.Log("Initial Draw Phase");

        await Draw(stateMachine.firstID);
        await Draw(stateMachine.secondID);

        TurnSystem turnSystem = ServiceLocator.Get<TurnSystem>();
        SelectionSystem selectionSystem = ServiceLocator.Get<SelectionSystem>();
        ActionSystem actionSystem = ServiceLocator.Get<ActionSystem>();

        turnSystem.SetTurnOrder(new int[] { stateMachine.firstID, stateMachine.secondID });

        turnSystem.EnableSystem();
        selectionSystem.EnableSystem();
        actionSystem.EnableSystem();
    }

    /// <summary> 해당 플레이어에게 DRAW_NUM장을 0.3초 간격으로 드로우하여 핸드에 추가한다. </summary>
    async UniTask Draw(int playerID)
    {
        DrawManager drawManager = ServiceLocator.Get<DrawManager>();
        HandManager handManager = ServiceLocator.Get<HandManager>();

        for (int i = 0; i < DRAW_NUM; i++)
        {
            Card card = await drawManager.Draw(playerID);
            handManager.AddCardToHand(playerID, card);
            await UniTask.Delay(DRAW_COOLTIME_MS);
        }
    }
}
