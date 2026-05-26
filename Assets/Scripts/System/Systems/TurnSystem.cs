using Cysharp.Threading.Tasks;
using System;
using UnityEngine; 

public enum TurnState
{
    Unable, 
    PlayerTurn, 
    EnemyTurn, 
    Waiting, 
    EndTurn, 
    GameOver 
}
/// <summary>
/// 플레이어 간 턴 순서를 관리하는 시스템.
/// StartTurn → (플레이어 입력 또는 AI 자동 행동) → EndTurn → StartTurn 순으로 무한 반복한다.
/// TurnState.Unable이면 턴 루프가 동작하지 않아 게임이 일시 정지된다.
/// </summary>
public class TurnSystem : IGameSystem
{
    PlayerManager playerManager;
    UIManager uiManager;
    HandManager handManager;
    DrawManager drawManager;
    ActionSystem actionSystem;
    AIController aiController;

    TurnState currentTurnState;
    public TurnState TurnState { get { return currentTurnState; } }

    int[] playerID;       // 턴 순서 배열 [선공, 후공]
    int currentPlayerID;  // 현재 턴을 진행 중인 플레이어 ID

    public Action OnPlayerTurnStarted;    // 로컬 플레이어 턴 시작 시 발생
    public Action OnOpponentTurnStarted;  // 상대방 턴 시작 시 발생
    public Action OnPlayerTurnEnded;      // 로컬 플레이어 턴 종료 시 발생
    public Action OnOpponentTurnEnded;    // 상대방 턴 종료 시 발생
    public Action<int> onTurnStarted;     // 턴 시작 시 현재 플레이어 ID와 함께 발생

    public void Init()
    {
        this.playerManager = ServiceLocator.Get<PlayerManager>();
        this.drawManager = ServiceLocator.Get<DrawManager>();
        this.uiManager = ServiceLocator.Get<UIManager>();
        this.handManager = ServiceLocator.Get<HandManager>();
        this.actionSystem = ServiceLocator.Get<ActionSystem>();
        aiController = ServiceLocator.Get<AIController>();

        DisableSystem();
    }

    /// <summary> 선공/후공 순서를 설정하고 첫 번째 플레이어를 현재 턴으로 지정한다. </summary>
    public void SetTurnOrder(int[] playerID)
    {
        this.playerID = playerID;
        currentPlayerID = playerID[0];
    }

    /// <summary> 턴 루프 진입점. MainPhase에서 시스템이 활성화될 때 호출된다. </summary>
    public async UniTask BeginTurnLoop()
    {
        Debug.Log("TurnLoop");
        await StartTurn();
    }

    /// <summary>
    /// 현재 플레이어의 턴을 시작한다.
    /// 액션 포인트를 초기화하고 1장 드로우 후 UI 알림을 띄운다.
    /// 로컬 플레이어 턴이면 SelectionSystem이 활성화되고,
    /// AI 턴이면 AIController가 자동으로 행동을 수행한다.
    /// </summary>
    public async UniTask StartTurn()
    {
        if (TurnState == TurnState.Unable)
            return;

        actionSystem.ResetActionCount(currentPlayerID);
        onTurnStarted?.Invoke(currentPlayerID);

        if (playerManager.Local.PlayerID == currentPlayerID)
            uiManager.OnNotification("내 턴", () =>
            {
                currentTurnState = TurnState.PlayerTurn;
                OnPlayerTurnStarted?.Invoke();
            });
        else
            uiManager.OnNotification("상대 턴", () =>
            {
                currentTurnState = TurnState.EnemyTurn;
                StartAITurn(currentPlayerID);
                OnOpponentTurnStarted?.Invoke();
            });

        Card card = await drawManager.Draw(currentPlayerID);

        if (card == null)
            return;

        handManager.AddCardToHand(currentPlayerID, card);
    }

    /// <summary> AI 턴을 시작하고, 모든 AI 행동이 완료되면 EndTurn을 콜백으로 호출한다. </summary>
    private void StartAITurn(int currentPlayerID)
    {
        aiController.InvokeRandomAction(currentPlayerID, () => EndTurn());
    }

    /// <summary>
    /// 현재 턴을 종료하고 다음 플레이어로 전환한 뒤 StartTurn을 호출한다.
    /// TurnState.Unable이면 루프를 중단한다.
    /// </summary>
    public async UniTask EndTurn()
    {
        if (TurnState == TurnState.Unable)
            return;

        if (currentTurnState == TurnState.PlayerTurn)
            OnPlayerTurnEnded?.Invoke();
        else
            OnOpponentTurnEnded?.Invoke();

        currentTurnState = TurnState.EndTurn;

        // 다음 플레이어로 전환
        foreach (var _playerID in playerID)
        {
            if (currentPlayerID != _playerID)
            {
                currentPlayerID = _playerID;
                break;
            }
        }

        await StartTurn();
    }

    public void Wait() => currentTurnState = TurnState.Waiting;
    public int GetCurrentTurnPlayerID() => currentPlayerID;
    public bool IsMyTurn() => currentPlayerID == playerManager.Local.PlayerID;

    /// <summary> 시스템 활성화 및 턴 루프 시작 </summary>
    public void EnableSystem()
    {
        Wait();
        BeginTurnLoop();
    }

    public void DisableSystem() => currentTurnState = TurnState.Unable;
}
