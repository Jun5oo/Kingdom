using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;


public class GameManager : MonoBehaviour
{
    [SerializeField] GameFlowManager gameFlowManager;
    [SerializeField] GridManager gridManager;
    [SerializeField] UIManager uiManager;
    [SerializeField] ActionSystem actionSystem;
    [SerializeField] CardManager cardManager;
    [SerializeField] HoverSystem hoverSystem; 
    [SerializeField] SelectionSystem selectionSystem;

    PlayerManager playerManager;
    TurnManager turnManager;
    TokenManager tokenManager;
    DamageManager damageManager; 

    CardFactory cardFactory;
    TokenFactory tokenFactory;
    ActionFactory actionFactory;

    UIInvoker uiInvoker;

    [SerializeField] GameObject cardPrefab;
    [SerializeField] GameObject tokenPrefab;
    [SerializeField] GameObject kingTokenPrefab;

    PlayerData[] players;

    void Awake()
    {
        // PlayerData (나중에 로비에서 플레이어의 데이터를 가져와야 함, 현재는 임시로 설정한 것) 
        players = new PlayerData[2];

        players[0] = new PlayerData(0, Race.Undead, "Local", true);
        players[1] = new PlayerData(1, Race.Celestial, "Remote", false);

        playerManager = new PlayerManager();
        turnManager = new TurnManager();
        tokenManager = new TokenManager();
        damageManager = new DamageManager();

        cardFactory = new CardFactory();
        tokenFactory = new TokenFactory();
        actionFactory = new ActionFactory();

        uiInvoker = new UIInvoker();

        Initialization(); 

        Invoke("StartGameDelayed", 4f);

    }
    void Initialization()
    {
        playerManager.Init(players);
        tokenManager.Init(playerManager);
        gridManager.Init(tokenManager);
        actionSystem.Init(gridManager);
        damageManager.Init(playerManager, tokenManager, uiManager);

        cardFactory.Init(cardPrefab);
        tokenFactory.Init(tokenPrefab, kingTokenPrefab);

        cardManager.Init(playerManager, cardFactory);
        turnManager.Init(playerManager, uiManager, cardManager, actionSystem);
        actionFactory.Init(gridManager, cardManager, tokenManager, damageManager, tokenFactory);

        uiInvoker.Init(uiManager, actionSystem, actionFactory);

        hoverSystem.Init(gridManager, tokenManager, actionSystem);
        selectionSystem.Init(turnManager, tokenManager, actionSystem, uiInvoker);

        gameFlowManager.Init(playerManager, uiManager, turnManager, cardManager, tokenManager, damageManager, actionSystem, actionFactory);
    }

    void StartGameDelayed()
    {
        gameFlowManager.GameStart();
    }
}
