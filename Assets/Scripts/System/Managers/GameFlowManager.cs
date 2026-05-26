using UnityEngine;

/// <summary>
/// 게임 전체 흐름(상태 전환)을 관리하는 클래스.
/// GameFlowStateMachine을 통해 TurnSelection → KingDraw → KingPlacement → MainPhase → GameOver 순으로 상태를 전환한다.
/// DamageManager의 OnKingDefeated 이벤트를 구독하여 게임 종료를 감지한다.
/// </summary>
public class GameFlowManager
{
    GameFlowStateMachine stateMachine;

    TurnSelection turnSelection;
    GameOver gameOver;

    PlayerManager playerManager;
    UIManager uiManager;
    DamageManager damageManager;

    /// <summary> 상태 머신과 초기 상태를 생성하고, 왕 사망 이벤트를 구독한다. </summary>
    public void Init()
    {
        stateMachine = new GameFlowStateMachine();

        turnSelection = new TurnSelection(stateMachine);
        gameOver = new GameOver();

        playerManager = ServiceLocator.Get<PlayerManager>();
        uiManager = ServiceLocator.Get<UIManager>();
        damageManager = ServiceLocator.Get<DamageManager>();

        damageManager.OnKingDefeated -= GameOver;
        damageManager.OnKingDefeated += GameOver;
    }

    /// <summary> TurnSelection 상태로 진입하여 게임을 시작한다. </summary>
    public void GameStart()
    {
        stateMachine.Enter(turnSelection);
    }

    /// <summary>
    /// 왕이 사망했을 때 호출된다.
    /// loserID가 로컬 플레이어면 패배, 아니면 승리 알림을 표시한다.
    /// loserID == -1이면 무승부다.
    /// </summary>
    public void GameOver(int loserID)
    {
        stateMachine.Enter(gameOver);

        if (loserID == playerManager.Local.PlayerID)
            uiManager.OnNotification("패배");
        else
            uiManager.OnNotification("승리");
    }
}
