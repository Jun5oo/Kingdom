using Cysharp.Threading.Tasks;
using UnityEngine;

/// <summary>
/// 게임 시작 시 선·후공을 무작위로 결정하는 상태.
/// 결과를 UI 알림으로 표시하고, 1초 대기 후 KingDraw 상태로 전환한다.
/// </summary>
public class TurnSelection : IGameState
{
    const int PLAYER_NUM = 2;
    const int WAIT_TIME_MS = 1000;

    GameFlowStateMachine stateMachine;

    public TurnSelection(GameFlowStateMachine stateMachine)
    {
        this.stateMachine = stateMachine;
    }

    /// <summary>
    /// 두 플레이어 중 선공을 무작위 선택한다.
    /// 로컬 플레이어가 선공이면 "선공", 후공이면 "후공" 알림을 표시한다.
    /// stateMachine의 firstID/secondID를 설정한 뒤 KingDraw로 전환한다.
    /// </summary>
    public async UniTask Enter()
    {
        Debug.Log("Turn Selection Phase");
        PlayerManager playerManager = ServiceLocator.Get<PlayerManager>();
        UIManager uiManager = ServiceLocator.Get<UIManager>();

        int[] playerID = new int[PLAYER_NUM];

        playerID[0] = playerManager.Local.PlayerID;
        playerID[1] = playerManager.Remote.PlayerID;

        int idx = Random.Range(0, PLAYER_NUM);

        int first = playerID[idx];
        int second = playerID[PLAYER_NUM - 1 - idx];

        if (first == playerID[0])
            uiManager.OnNotification("선공");
        else
            uiManager.OnNotification("후공");

        stateMachine.firstID = first;
        stateMachine.secondID = second;

        await UniTask.Delay(WAIT_TIME_MS);
        KingDraw kingDraw = new KingDraw(stateMachine);
        stateMachine.Enter(kingDraw);
    }
}
