using Cysharp.Threading.Tasks;
using UnityEngine;

[DefaultExecutionOrder(-1)]
public class Bootstrapper : MonoBehaviour
{
    // 게임에 필요한 매니저, 시스템, 팩토리, 로더 클래스들을 생성하고 ServiceLocator에 등록하는 가장 먼저 실행되어야 하는 클래스
    // ExecutionOrder를 통해서 가장 먼저 실행 

    [SerializeField] UIManager uiManager;
    [SerializeField] GridManager gridManager;
    [SerializeField] HandManager handManager;
    [SerializeField] HoverSystem hoverSystem;
    [SerializeField] SelectionSystem selectionSystem;
    [SerializeField] ActionSystem actionSystem;

    GameFlowManager gameFlowManager;
    PlayerManager playerManager;
    TokenManager tokenManager;
    TurnSystem turnSystem;
    DamageManager damageManager;
    [SerializeField] PoolManager poolManager;

    CardFactory cardFactory;
    TokenFactory tokenFactory;
    ActionFactory actionFactory;
    PassiveFactory passiveFactory; 

    [SerializeField] DeckManager deckManager;
    DrawManager drawManager;

    CardTextureLoader cardTextureLoader;
    TokenTextureLoader tokenTextureLoader; 
    PrefabLoader prefabLoader;

    EventQueue eventQueue;
    [SerializeField] CardDatabase database;

    SummonSystem summonSystem;

    void Awake()
    {
        RegisterServices(); 
    }

    async void Start()
    {
        await Initialization();
        gameFlowManager.GameStart();
    }

    public void RegisterServices()
    {
        gameFlowManager = new GameFlowManager();
        playerManager = new PlayerManager();
        tokenManager = new TokenManager();
        damageManager = new DamageManager();
        drawManager = new DrawManager();

        cardFactory = new CardFactory();
        tokenFactory = new TokenFactory();
        actionFactory = new ActionFactory();
        passiveFactory = new PassiveFactory(); 

        turnSystem = new TurnSystem();

        cardTextureLoader = new CardTextureLoader();
        tokenTextureLoader = new TokenTextureLoader();
        prefabLoader = new PrefabLoader();

        eventQueue = new EventQueue(); 

        summonSystem = new SummonSystem();

        ServiceLocator.Register(playerManager);
        ServiceLocator.Register(damageManager);
        ServiceLocator.Register(uiManager);
        ServiceLocator.Register(gridManager);
        ServiceLocator.Register(handManager);
        ServiceLocator.Register(tokenManager);
        ServiceLocator.Register(poolManager);

        ServiceLocator.Register(turnSystem);
        ServiceLocator.Register(hoverSystem);
        ServiceLocator.Register(selectionSystem);
        ServiceLocator.Register(actionSystem);

        ServiceLocator.Register(cardFactory);
        ServiceLocator.Register(tokenFactory);
        ServiceLocator.Register(actionFactory);
        ServiceLocator.Register(passiveFactory); 
        
        ServiceLocator.Register(drawManager);
        ServiceLocator.Register(deckManager);

        ServiceLocator.Register(cardTextureLoader);
        ServiceLocator.Register(tokenTextureLoader);
        ServiceLocator.Register(prefabLoader);

        ServiceLocator.Register(eventQueue);
        ServiceLocator.Register(database);

        ServiceLocator.Register(summonSystem); 
    }

    public async UniTask Initialization()
    {
        playerManager.Init();
        gameFlowManager.Init(); 
        tokenManager.Init();
        gridManager.Init();
        actionSystem.Init();
        damageManager.Init();

        cardTextureLoader.Init();
        tokenTextureLoader.Init();
        
        await cardFactory.Init();
        await tokenFactory.Init();

        handManager.Init();
        turnSystem.Init();

        hoverSystem.Init();
        selectionSystem.Init();

        deckManager.Init();
        drawManager.Init();

        await poolManager.InitAsync();

        database.Init();
        summonSystem.Init();
    }

}
