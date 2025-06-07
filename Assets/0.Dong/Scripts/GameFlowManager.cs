using System.Collections;
using UnityEngine;

// 게임 전체 흐름을 관리하는 클래스 (턴 순서, 드로우, 왕 배치 등)
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

    private bool isFirstTurn = true; // 첫 턴인지 여부
    private bool isFirstPlayerKingPlaced = false; // 선공 왕 배치 완료 여부
    private bool isSecondPlayerKingPlaced = false; // 후공 왕 배치 완료 여부

    private int firstPlayerID;
    private int secondPlayerID;

    // 카드 시스템 의존성 주입 (초기화용)
    public void Init(CardSystem cardSystem)
    {
        this.cardSystem = cardSystem;
    }

    // 게임 시작 트리거
    public void StartGame()
    {
        currentState = TurnState.DecideFirstPlayer;
        DecideTurnOrder(); // 선공/후공 결정 및 초기 드로우
    }

    // 선공/후공 무작위 결정 + 안내 메시지 출력 → 초기 드로우로 전환
    private void DecideTurnOrder()
    {
        firstPlayerID = UnityEngine.Random.Range(0, 2);
        secondPlayerID = 1 - firstPlayerID;
        currentPlayerID = firstPlayerID;

        currentState = TurnState.InitialDraw;

        // 선/후공 UI 메시지 출력
        string turnInfo = currentPlayerID == 0 ? "당신은 선공입니다." : "당신은 후공입니다.";
        UIManager.Instance.ShowTurnOrder(turnInfo);

        StartCoroutine(DrawInitialCards());
    }

    // 초기 카드 드로우를 일정 간격으로 순차 실행
    private IEnumerator DrawInitialCards()
    {
        for (int i = 0; i < 3; i++)
        {
            cardSystem.DrawCard(firstPlayerID);
            yield return new WaitForSeconds(0.4f);
        }

        for (int i = 0; i < 4; i++)
        {
            cardSystem.DrawCard(secondPlayerID);
            yield return new WaitForSeconds(0.4f);
        }

        EnterKingPlacement(); // 왕 배치 단계로 전환
    }

    // 현재 상태에 따라 선공/후공 순서로 왕 배치 시작
    private void EnterKingPlacement()
    {
        currentState = TurnState.KingPlacement;

        if (!isFirstPlayerKingPlaced)
        {
            Debug.Log($"선공 플레이어({firstPlayerID}) 왕 배치");
            cardSystem.SummonKing(cardSystem.GetPlayerKing(firstPlayerID));

            // 왕 배치 메시지 표시
            if (firstPlayerID == 0)
                UIManager.Instance.ShowTurnMessage("왕을 배치해주세요");
            else
                UIManager.Instance.ShowTurnMessage("상대가 왕을 배치 중입니다...");
        }
        else if (!isSecondPlayerKingPlaced)
        {
            Debug.Log($"후공 플레이어({secondPlayerID}) 왕 배치");
            cardSystem.SummonKing(cardSystem.GetPlayerKing(secondPlayerID));

            if (secondPlayerID == 0)
                UIManager.Instance.ShowTurnMessage("왕을 배치해주세요");
            else
                UIManager.Instance.ShowTurnMessage("상대가 왕을 배치 중입니다...");
        }
    }

    // 왕 배치가 끝났을 때 호출되는 콜백 (선공 → 후공 → 턴 시작 순)
    public void OnKingPlaced()
    {
        if (!isFirstPlayerKingPlaced)
        {
            isFirstPlayerKingPlaced = true;
            EnterKingPlacement(); // 후공으로 넘어감
        }
        else if (!isSecondPlayerKingPlaced)
        {
            isSecondPlayerKingPlaced = true;
            StartTurn(); // 두 명 모두 왕 배치 완료 → 게임 시작
        }
    }

    // 현재 플레이어의 턴 시작 처리
    private void StartTurn()
    {
        currentState = TurnState.PlayerTurn;

        UIManager.Instance.HideTurnMessage();

        cardSystem.DrawCard(currentPlayerID); // 매 턴 드로우 1장

        if (isFirstTurn)
            isFirstTurn = false;

        actionPoint = 2; // 행동력 초기화

        Debug.Log($"Player {currentPlayerID}의 턴 시작. 행동력: {actionPoint}");

        // UI 갱신
        bool isMyTurn = currentPlayerID == 0;
        TurnUIManager.Instance.UpdateTurnOwner(isMyTurn);
        TurnUIManager.Instance.UpdateActionPoint(actionPoint);
    }

    // 한 번의 행동(소환, 이동 등) 후 호출
    public void OnActionPerformed()
    {
        actionPoint--;
        if (currentPlayerID == 0)
            TurnUIManager.Instance.UpdateActionPoint(actionPoint);

        if (actionPoint <= 0)
            EndTurn(); // 행동력이 0이면 턴 종료
    }

    // 턴 종료 → 플레이어 전환 → 다음 턴 시작
    private void EndTurn()
    {
        currentState = TurnState.EndTurn;

        currentPlayerID = 1 - currentPlayerID;
        StartTurn();
    }

    // 내 턴 여부 확인용 (카드에서 판단)
    public bool IsMyTurn(bool isMyCard)
    {
        return currentPlayerID == (isMyCard ? 0 : 1);
    }
}
