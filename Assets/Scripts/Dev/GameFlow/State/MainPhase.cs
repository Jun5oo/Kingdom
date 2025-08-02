using Cysharp.Threading.Tasks;
using UnityEngine; 
public class MainPhase : IGameState
{
    // MainPhase 개시. 필요한 System들을 Enabled 시키고 첫 드로우를 실행. 

    const int DRAW_NUM = 3;
    const int DRAW_COOLTIME_MS = 300;

    GameFlowStateMachine stateMachine;

    public MainPhase(GameFlowStateMachine stateMachine)
    {   
        this.stateMachine = stateMachine;
    }

    public async UniTask Enter()
    {
        Debug.Log("Initial Draw Phase"); 

        await Draw(stateMachine.firstID);
        await Draw(stateMachine.secondID);

        TurnSystem turnSystem = ServiceLocator.Get<TurnSystem>();
        SelectionSystem selectionSystem = ServiceLocator.Get<SelectionSystem>();
        ActionSystem actionSystem = ServiceLocator.Get<ActionSystem>();

        turnSystem.SetTurnOrder(new int[] { stateMachine.firstID, stateMachine.secondID });

        turnSystem.EnableSystem(); 
        selectionSystem.EnableSystem();
        actionSystem.EnableSystem(); 
    }

    async UniTask Draw(int playerID)
    {
        DrawManager drawManager = ServiceLocator.Get<DrawManager>();
        HandManager handManager = ServiceLocator.Get<HandManager>();

        for (int i=0; i<DRAW_NUM; i++)
        {
            Card card = await drawManager.Draw(playerID);
            handManager.AddCardToHand(playerID, card);
            await UniTask.Delay(DRAW_COOLTIME_MS);
        }
    }

}
