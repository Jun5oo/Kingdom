using System.Collections;
using UnityEngine;

public class FirstDraw : IGameState
{
    const int DRAW_NUM = 3;
    const float DRAW_COOLTIME = 0.3f; 

    TurnManager turnManager;
    DrawManager drawManager;
    PlayerHandManager handManager; 

    GameFlowStateMachine stateMachine;

    WaitForSeconds drawTime; 
    
    public FirstDraw(GameFlowStateMachine stateMachine)
    {
        turnManager = ServiceLocator.Get<TurnManager>(); 
        drawManager = ServiceLocator.Get<DrawManager>();
        handManager = ServiceLocator.Get<PlayerHandManager>();

        this.stateMachine = stateMachine;

        drawTime = new WaitForSeconds(DRAW_COOLTIME); 
    }

    public IEnumerator Enter()
    {
        yield return Draw(stateMachine.firstID);
        yield return Draw(stateMachine.secondID);

        turnManager.SetTurnOrder(new int[] {stateMachine.firstID, stateMachine.secondID});  
        turnManager.BeginTurnLoop(); 

        SelectionSystem selectionSystem = ServiceLocator.Get<SelectionSystem>();
        ActionSystem actionSystem = ServiceLocator.Get<ActionSystem>();

        selectionSystem.EnableSystem();
        actionSystem.EnableSystem(); 
    }

    IEnumerator Draw(int playerID)
    {
       for(int i=0; i<DRAW_NUM; i++)
        {
            Card card = drawManager.Draw(playerID);
            handManager.AddCardToHand(playerID, card); 
            yield return drawTime; 
        }
    }

}
