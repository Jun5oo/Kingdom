using UnityEngine;

[DefaultExecutionOrder(-1)]
public class Bootstrapper : MonoBehaviour
{
    [SerializeField] GameFlowManager gameFlowManager; 
    [SerializeField] UIManager uiManager;
    [SerializeField] GridManager gridManager;
    [SerializeField] PlayerHandManager handManager;
    [SerializeField] HoverSystem hoverSystem;
    [SerializeField] SelectionSystem selectionSystem;
    [SerializeField] ActionSystem actionSystem;

    PlayerManager playerManager;
    TokenManager tokenManager;
    TurnManager turnManager;
    DamageManager damageManager;

    CardFactory cardFactory;
    TokenFactory tokenFactory;
    ActionFactory actionFactory;

    UIInvoker uiInvoker;

    [SerializeField] DeckManager deckManager;
    DrawManager drawManager; 

    [SerializeField] GameObject cardPrefab;
    [SerializeField] GameObject tokenPrefab;
    [SerializeField] GameObject kingTokenPrefab;


    void Awake()
    {
        playerManager = new PlayerManager();
        turnManager = new TurnManager();
        tokenManager = new TokenManager();
        damageManager = new DamageManager();
        
        cardFactory = new CardFactory();
        actionFactory = new ActionFactory();
        tokenFactory = new TokenFactory();
        uiInvoker = new UIInvoker(); 
        
        drawManager = new DrawManager();

        ServiceLocator.Register(playerManager);
        ServiceLocator.Register(turnManager);
        ServiceLocator.Register(damageManager); 
        ServiceLocator.Register(uiManager); 
        ServiceLocator.Register(gridManager);
        ServiceLocator.Register(handManager);
        ServiceLocator.Register(tokenManager);
        ServiceLocator.Register(hoverSystem);
        ServiceLocator.Register(selectionSystem);
        ServiceLocator.Register(actionSystem);

        ServiceLocator.Register(cardFactory);
        ServiceLocator.Register(tokenFactory);
        ServiceLocator.Register(actionFactory);

        ServiceLocator.Register(uiInvoker);

        ServiceLocator.Register(deckManager);
        ServiceLocator.Register(drawManager); 

        Initialization();

        gameFlowManager.GameStart(); 
    }

    void Initialization()
    {
        playerManager.Init();
        gameFlowManager.Init(); 
        tokenManager.Init();
        gridManager.Init();
        actionSystem.Init();
        damageManager.Init();

        cardFactory.Init(cardPrefab);
        tokenFactory.Init(tokenPrefab, kingTokenPrefab);

        handManager.Init();
        turnManager.Init();

        uiInvoker.Init();

        hoverSystem.Init();
        selectionSystem.Init();

        deckManager.Init();
        drawManager.Init(); 
    }

}
