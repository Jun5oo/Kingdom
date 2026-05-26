using Cysharp.Threading.Tasks;
using UnityEngine;

/// <summary>
/// 게임 시작 시 모든 시스템/매니저/팩토리를 생성하고 ServiceLocator에 등록하는 진입점.
/// [DefaultExecutionOrder(-1)]로 다른 MonoBehaviour보다 먼저 Awake가 실행된다.
/// Awake → RegisterServices(생성+등록+초기화) → Start → GameStart 순으로 동작한다.
/// </summary>
[DefaultExecutionOrder(-1)]
public class Bootstrapper : MonoBehaviour
{

    [SerializeField] PlayerConfig config; 

    [SerializeField] PlayerManager playerManager; 
    [SerializeField] UIManager uiManager;
    [SerializeField] GridManager gridManager;
    [SerializeField] HandManager handManager;
    [SerializeField] HoverSystem hoverSystem;
    [SerializeField] SelectionSystem selectionSystem;
    [SerializeField] ActionSystem actionSystem;

    GameFlowManager gameFlowManager;
    TokenManager tokenManager;
    TurnSystem turnSystem;
    DamageManager damageManager;
    [SerializeField] PoolManager poolManager;

    CardFactory cardFactory;
    TokenFactory tokenFactory;
    ActionFactory actionFactory;
    PassiveFactory passiveFactory;

    AIController aiController;

    [SerializeField] DeckManager deckManager;
    DrawManager drawManager;

    CardTextureLoader cardTextureLoader;
    TokenTextureLoader tokenTextureLoader;
    PrefabLoader prefabLoader;
    SpriteLoader spriteLoader;

    EventQueue eventQueue;
    [SerializeField] CardDatabase database;

    [SerializeField] HUDDisplayer hudDisplayer;

    SummonSystem summonSystem;
    UpgradeSystem upgradeSystem;

    RangeResolver rangeResolver;
    ActionResolver actionResolver;

    // 씬이 비활성화되거나 오브젝트가 파괴될 때 서비스를 모두 해제한다.
    void OnDisable()
    {
        UnRegisterServices();
    }

    // 가장 먼저 실행: 모든 서비스를 생성·등록·초기화한다.
    void Awake()
    {
        RegisterServices();
    }

    // 초기화가 끝난 뒤 게임 흐름을 시작한다.
    void Start()
    {
        gameFlowManager.GameStart();
    }

    /// <summary> 모든 시스템/매니저 인스턴스를 생성하고 ServiceLocator에 등록한 뒤 초기화한다. </summary>
    public async void RegisterServices()
    {
        gameFlowManager = new GameFlowManager();
        tokenManager = new TokenManager();
        damageManager = new DamageManager();
        drawManager = new DrawManager();

        aiController = new AIController();
        cardFactory = new CardFactory();
        tokenFactory = new TokenFactory();
        actionFactory = new ActionFactory();
        passiveFactory = new PassiveFactory();

        turnSystem = new TurnSystem();

        cardTextureLoader = new CardTextureLoader();
        tokenTextureLoader = new TokenTextureLoader();
        prefabLoader = new PrefabLoader();
        spriteLoader = new SpriteLoader();

        eventQueue = new EventQueue();

        summonSystem = new SummonSystem();
        upgradeSystem = new UpgradeSystem();

        rangeResolver = new RangeResolver();
        actionResolver = new ActionResolver(); 

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

        ServiceLocator.Register(aiController);

        ServiceLocator.Register(cardTextureLoader);
        ServiceLocator.Register(tokenTextureLoader);
        ServiceLocator.Register(prefabLoader);

        ServiceLocator.Register(spriteLoader);

        ServiceLocator.Register(eventQueue);
        ServiceLocator.Register(database);

        ServiceLocator.Register(summonSystem);
        ServiceLocator.Register(upgradeSystem);

        ServiceLocator.Register(hudDisplayer);

        ServiceLocator.Register(rangeResolver);
        ServiceLocator.Register(actionResolver); 

        await Initialization();
    }
    /// <summary> ServiceLocator에 등록된 모든 서비스를 해제한다. </summary>
    public void UnRegisterServices()
    {
        ServiceLocator.Unregister<PlayerManager>();
        ServiceLocator.Unregister<DamageManager>();
        ServiceLocator.Unregister<UIManager>();
        ServiceLocator.Unregister<GridManager>();
        ServiceLocator.Unregister<HandManager>();
        ServiceLocator.Unregister<TokenManager>();
        ServiceLocator.Unregister<PoolManager>();

        ServiceLocator.Unregister<TurnSystem>(); 
        ServiceLocator.Unregister<HoverSystem>();
        ServiceLocator.Unregister<SelectionSystem>();
        ServiceLocator.Unregister<ActionSystem>();
            
        ServiceLocator.Unregister<CardFactory>();
        ServiceLocator.Unregister<TokenFactory>();
        ServiceLocator.Unregister<ActionFactory>();
        ServiceLocator.Unregister<PassiveFactory>();

        ServiceLocator.Unregister<DrawManager>();
        ServiceLocator.Unregister<DeckManager>();

        ServiceLocator.Unregister<AIController>();

        ServiceLocator.Unregister<CardTextureLoader>();
        ServiceLocator.Unregister<TokenTextureLoader>();
        ServiceLocator.Unregister<PrefabLoader>();
        ServiceLocator.Unregister<SpriteLoader>();

        ServiceLocator.Unregister<EventQueue>();
        ServiceLocator.Unregister<CardDatabase>();

        ServiceLocator.Unregister<SummonSystem>();
        ServiceLocator.Unregister<UpgradeSystem>();

        ServiceLocator.Unregister<HUDDisplayer>();

        ServiceLocator.Unregister<RangeResolver>();
        ServiceLocator.Unregister<ActionResolver>();
    }
    /// <summary> 등록된 서비스들을 의존 순서에 맞게 초기화한다. 비동기 로더가 포함되어 있어 await가 필요하다. </summary>
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
        await spriteLoader.Init();

        await cardFactory.Init();
        await tokenFactory.Init();

        handManager.Init();
        turnSystem.Init();

        hoverSystem.Init();
        selectionSystem.Init();

        deckManager.Init();
        drawManager.Init();
        aiController.Init();

        database.Init();
        summonSystem.Init();
        upgradeSystem.Init();

        hudDisplayer.Init();

        await poolManager.InitAsync();
    }

}
