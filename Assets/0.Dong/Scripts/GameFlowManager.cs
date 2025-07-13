using System;
using System.Collections;
using UnityEngine;

public class GameFlowManager : MonoBehaviour
{
    // 게임 진행 상태 정의
    enum GameFlowState
    {
        TurnSelection,  // 선공/후공 결정
        KingDraw,        // 왕 카드 드로우
        KingPlacement,      // 왕 배치
        Draw,           // 시작 드로우 
        Done 
    }

    const int PLAYER_NUM = 2;
    const int FIRST_PLAYER_DRAW = 3;
    const int LAST_PLAYER_DRAW = 4;
    const float DRAW_COOLTIME = 0.4f;
    const float WAIT_TIME = 0.5f;

    // 현재 CurrentState의 경우 사용하지 않지만, 나중에 네트워크가 추가될 경우, 연결 끊김 등 다시 복구를 시도할 때 필요할 수도 있어서 구현 
    private GameFlowState currentState;

    PlayerManager playerManager;
    UIManager uiManager;
    TurnManager turnManager;
    CardManager cardManager;
    TokenManager tokenManager;
    DamageManager damageManager;
    ActionSystem actionSystem;
    ActionFactory actionFactory;

    int[] playerID; 

    private int firstPlayerID;
    private int lastPlayerID;

    private Card firstPlayerCard; 
    private Card lastPlayerCard;

    WaitForSeconds waitTime;

    // static 사용을 고려. (주입을 위한 파라미터가 너무 많다.) 
    public void Init(PlayerManager playerManager, UIManager uiManager, TurnManager turnManager, CardManager cardManager, TokenManager tokenManager, DamageManager damageManager, ActionSystem actionSystem, ActionFactory actionFactory)
    {
        this.playerManager = playerManager;
        this.uiManager = uiManager;
        this.turnManager = turnManager;
        this.cardManager = cardManager;
        this.tokenManager = tokenManager;
        this.damageManager = damageManager;
        this.actionSystem = actionSystem; 
        this.actionFactory = actionFactory;

        playerID = new int[PLAYER_NUM];

        playerID[0] = playerManager.LocalPlayerData.PlayerID; 
        playerID[1] = playerManager.RemotePlayerData.PlayerID;

        damageManager.OnKingDefeated -= EndGame;
        damageManager.OnKingDefeated += EndGame;

        waitTime = new WaitForSeconds(WAIT_TIME); 

        Debug.Log($"{playerID[0]}, {playerID[1]}"); 
    }

    public void GameStart()
    {
        StartCoroutine(Initialization()); 
    }

    public void EndGame(int loserID)
    {
        Debug.Log($"End, {loserID} lose."); 

        if (loserID == playerManager.LocalPlayerData.PlayerID)
            uiManager.OnNotification("Defeated");
        else if (loserID == playerManager.RemotePlayerData.PlayerID)
            uiManager.OnNotification("Victory");
        else
            uiManager.OnNotification("Draw"); 
    }

    private IEnumerator Initialization()
    {
        yield return OnSelectTurnOrder();
        yield return OnDrawKingCard();
        yield return OnKingPlacement();
        yield return OnDrawCards();

        currentState = GameFlowState.Done; 

        int[] playerOrder = {firstPlayerID, lastPlayerID};

        turnManager.SetTurnOrder(playerOrder);
        turnManager.BeginTurnLoop();
    }
    private IEnumerator OnSelectTurnOrder()
    {
        currentState = GameFlowState.TurnSelection;

        int idx = UnityEngine.Random.Range(0, PLAYER_NUM);

        firstPlayerID = playerID[idx];
        lastPlayerID = playerID[PLAYER_NUM - idx - 1];

        if (firstPlayerID == playerID[0])
            uiManager.OnNotification("First");
        else
            uiManager.OnNotification("Second");

        yield return waitTime; 
    }
    private IEnumerator OnDrawKingCard()
    {
        currentState = GameFlowState.KingDraw;

        Card firstPlayerKing = cardManager?.DrawKingCard(firstPlayerID);
        Card lastPlayerKing = cardManager?.DrawKingCard(lastPlayerID);

        this.firstPlayerCard = firstPlayerKing;
        this.lastPlayerCard = lastPlayerKing;

        yield return waitTime; 
    }
    private IEnumerator OnKingPlacement()
    {
        currentState = GameFlowState.KingPlacement;

        yield return OnPlacement(firstPlayerCard) ; 
        yield return OnPlacement(lastPlayerCard);
   
        PlayerData localPlayer = playerManager.LocalPlayerData; 
        PlayerData remotePlayer = playerManager.RemotePlayerData;

        if(tokenManager.TryGetKingTokenFrom(localPlayer.PlayerID, out Token localToken))
            uiManager.SetHUD(localPlayer, localToken);
        else
            Debug.LogError("로컬 플레이어의 왕이 소환되지 않았습니다.");

        if (tokenManager.TryGetKingTokenFrom(lastPlayerID, out Token lastToken))
            uiManager.SetHUD(remotePlayer, lastToken);
        else
            Debug.LogError("상대 플레이어의 왕이 소환되지 않았습니다.");

        uiManager.OnActiveHUD();

        yield return waitTime; 
    }
    private IEnumerator OnPlacement(Card card)
    {
        bool isDone = false;

        Action onComplete = null;
        onComplete = () => isDone = true; 
   
        IAction summon = actionFactory.CreateAction(ActionType.Summon, card, ActionPerformer.System);
        summon.OnActionComplete += onComplete; 
   
        actionSystem?.Enter(summon);
        yield return new WaitUntil(() => isDone);

        summon.OnActionComplete -= onComplete;
    }
    private IEnumerator OnDrawCards()
    {
        currentState = GameFlowState.Draw; 

        for(int i=0; i<FIRST_PLAYER_DRAW; i++)
        {
            cardManager?.DrawCard(firstPlayerID);
            yield return new WaitForSeconds(DRAW_COOLTIME); 
        }

        for(int j=0; j<LAST_PLAYER_DRAW; j++)
        {
            cardManager?.DrawCard(lastPlayerID);
            yield return new WaitForSeconds(DRAW_COOLTIME);
        }

        yield return waitTime; 
    }

}
