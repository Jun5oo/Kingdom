using UnityEngine;

public class GameFlowManager : MonoBehaviour
{
    // 게임 진행 상태 정의
    enum TurnState
    {
        DecideFirstPlayer,  // 선공/후공 결정
        InitialDraw,        // 시작 드로우
        KingPlacement,      // 왕 배치
        PlayerTurn,         // 플레이어 턴
        EndTurn             // 턴 종료
    }

    private TurnState currentState;

    private int currentPlayerID; // 현재 턴의 플레이어 ID (0: 내 턴, 1: 적 턴)
    private int actionPoint;     // 현재 턴의 남은 행동력 (최대 2)

    [SerializeField] CardSystem cardSystem; // 카드 시스템 참조

    private bool isFirstTurn = true;             // 첫 턴 여부 플래그
    private bool isFirstPlayerKingPlaced = false; // 선공 플레이어 왕 배치 완료 여부
    private bool isSecondPlayerKingPlaced = false; // 후공 플레이어 왕 배치 완료 여부

    private int firstPlayerID;
    private int secondPlayerID;

    // 외부에서 CardSystem 참조를 주입받는 초기화 함수
    public void Init(CardSystem cardSystem)
    {
        this.cardSystem = cardSystem;
    }

    // 게임 시작을 트리거하는 함수
    public void StartGame()
    {
        currentState = TurnState.DecideFirstPlayer;
        DecideTurnOrder(); // 선/후공 결정 및 드로우 시작
    }

    // 선공/후공 결정 및 초기 드로우
    private void DecideTurnOrder()
    {
        firstPlayerID = UnityEngine.Random.Range(0, 2);
        secondPlayerID = 1 - firstPlayerID;
        currentPlayerID = firstPlayerID; // 선공부터 시작

        currentState = TurnState.InitialDraw;

        // 드로우
        cardSystem.DrawCard(firstPlayerID); // 선공: 3장
        cardSystem.DrawCard(firstPlayerID);
        cardSystem.DrawCard(firstPlayerID);

        cardSystem.DrawCard(secondPlayerID); // 후공: 4장
        cardSystem.DrawCard(secondPlayerID);
        cardSystem.DrawCard(secondPlayerID);
        cardSystem.DrawCard(secondPlayerID);

        Invoke(nameof(EnterKingPlacement), 1f);
    }

    // 왕 배치 흐름 (선공 → 후공 순서)
    private void EnterKingPlacement()
    {
        currentState = TurnState.KingPlacement;

        if (!isFirstPlayerKingPlaced)
        {
            Debug.Log($"선공 플레이어({firstPlayerID}) 왕 배치");
            cardSystem.SummonKing(cardSystem.GetPlayerKing(firstPlayerID));
        }
        else if (!isSecondPlayerKingPlaced)
        {
            Debug.Log($"후공 플레이어({secondPlayerID}) 왕 배치");
            cardSystem.SummonKing(cardSystem.GetPlayerKing(secondPlayerID));
        }
    }

    // 왕 배치가 완료되었을 때 호출되는 콜백 함수
    public void OnKingPlaced()
    {
        if (!isFirstPlayerKingPlaced)
        {
            isFirstPlayerKingPlaced = true;
            EnterKingPlacement(); // 후공 왕 배치로 이어짐
        }
        else if (!isSecondPlayerKingPlaced)
        {
            isSecondPlayerKingPlaced = true;
            StartTurn(); // 양측 왕 배치가 끝나면 첫 턴 시작
        }
    }

    // 현재 플레이어의 턴 시작
    private void StartTurn()
    {
        currentState = TurnState.PlayerTurn;

        // 항상 드로우 1장
        cardSystem.DrawCard(currentPlayerID);

        if (isFirstTurn)
            isFirstTurn = false;

        actionPoint = 2;

        Debug.Log($"Player {currentPlayerID}의 턴 시작. 행동력: {actionPoint}");
    }

    // 행동 1회 수행 시 호출 (소환/이동/공격 등)
    public void OnActionPerformed()
    {
        actionPoint--;

        if (actionPoint <= 0)
            EndTurn(); // 행동력이 0이 되면 턴 종료
    }

    // 현재 턴 종료 및 다음 턴으로 전환
    private void EndTurn()
    {
        currentState = TurnState.EndTurn;

        currentPlayerID = 1 - currentPlayerID; // 플레이어 전환
        StartTurn(); // 다음 플레이어 턴 시작
    }

    public bool IsMyTurn(bool isMyCard)
    {
        return currentPlayerID == (isMyCard ? 0 : 1);
    }
}
