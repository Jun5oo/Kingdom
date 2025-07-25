using Cysharp.Threading.Tasks;
using UnityEngine;

[DefaultExecutionOrder(-1)]
public class Bootstrapper : MonoBehaviour
{
    // 게임에 필요한 시스템들을 ServiceLocator에 등록하고 초기화하는 클래스 
    [SerializeField] UIManager uiManager;
    [SerializeField] GridManager gridManager;
    [SerializeField] HandManager handManager;
    [SerializeField] HoverSystem hoverSystem;
    [SerializeField] SelectionSystem selectionSystem;
    [SerializeField] ActionSystem actionSystem;

    GameFlowManager gameFlowManager;
    PlayerManager playerManager;
    TokenManager tokenManager;
    TurnSystem turnManager;
    DamageManager damageManager;

    CardFactory cardFactory;
    TokenFactory tokenFactory;
    ActionFactory actionFactory;

    [SerializeField] DeckManager deckManager;
    DrawManager drawManager;

    TextureLoader textureLoader;
    PrefabLoader prefabLoader;

    PoolManager poolManager; 

    [SerializeField] GameObject cardPrefab;
    [SerializeField] GameObject tokenPrefab;
    [SerializeField] GameObject kingTokenPrefab;


    void Awake()
    {
        gameFlowManager = new GameFlowManager(); 
        playerManager = new PlayerManager();
        turnManager = new TurnSystem();
        tokenManager = new TokenManager();
        damageManager = new DamageManager();
        
        cardFactory = new CardFactory();
        actionFactory = new ActionFactory();
        tokenFactory = new TokenFactory();
        
        drawManager = new DrawManager();
        textureLoader = new TextureLoader();
        prefabLoader = new PrefabLoader();

        poolManager = new PoolManager(); 

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

        ServiceLocator.Register(deckManager);
        ServiceLocator.Register(drawManager);

        ServiceLocator.Register(textureLoader);
        ServiceLocator.Register(prefabLoader);
        ServiceLocator.Register(poolManager); 

        Initialization();

        gameFlowManager.GameStart(); 
    }

    public async UniTask Initialization()
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

        hoverSystem.Init();
        selectionSystem.Init();

        deckManager.Init();
        drawManager.Init();

        textureLoader.Init(); 
        await poolManager.InitAsync();
    }

}
