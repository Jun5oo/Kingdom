using Cysharp.Threading.Tasks;
using System.Collections;
using UnityEngine;

public class FirstDraw : IGameState
{
    const int DRAW_NUM = 3;
    const float DRAW_COOLTIME = 0.3f; 

    TurnSystem turnManager;
    DrawManager drawManager;
    HandManager handManager;

    PlayerManager playerManager;

    GameFlowStateMachine stateMachine;

    WaitForSeconds drawTime; 
    
    public FirstDraw(GameFlowStateMachine stateMachine)
    {
        turnManager = ServiceLocator.Get<TurnSystem>(); 
        drawManager = ServiceLocator.Get<DrawManager>();
        handManager = ServiceLocator.Get<HandManager>();
        playerManager = ServiceLocator.Get<PlayerManager>();

        this.stateMachine = stateMachine;

        drawTime = new WaitForSeconds(DRAW_COOLTIME); 
    }

    public async UniTask Enter()
    {
        await Draw(stateMachine.firstID);
        await Draw(stateMachine.secondID);

        turnManager.SetTurnOrder(new int[] {stateMachine.firstID, stateMachine.secondID});  
        await turnManager.BeginTurnLoop(); 

        SelectionSystem selectionSystem = ServiceLocator.Get<SelectionSystem>();
        ActionSystem actionSystem = ServiceLocator.Get<ActionSystem>();

        bool isAI = stateMachine.secondID != playerManager.Local.PlayerID;

        if (isAI)
        {
            selectionSystem.DisableSystem();
        }
        else
        {
            selectionSystem.EnableSystem();
        }

        actionSystem.EnableSystem(); 
    }

    async UniTask Draw(int playerID)
    {
       for(int i=0; i<DRAW_NUM; i++)
        {
            Card card = await drawManager.Draw(playerID);
            handManager.AddCardToHand(playerID, card); 
        }
    }

}
