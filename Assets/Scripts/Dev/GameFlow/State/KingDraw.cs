using System.Collections;
using UnityEngine;

public class KingDraw : IGameState
{
    const float WAIT_TIME = 2f;

    PlayerHandManager handManager;
    DrawManager drawManager; 

    GameFlowStateMachine stateMachine;

    public KingDraw(GameFlowStateMachine stateMachine)
    {
        handManager = ServiceLocator.Get<PlayerHandManager>();
        drawManager = ServiceLocator.Get<DrawManager>();    

        this.stateMachine = stateMachine; 
    }

    public IEnumerator Enter()
    {
        Card firstCard = drawManager.DrawKing(stateMachine.firstID);
        handManager.AddCardToHand(stateMachine.firstID, firstCard); 
        Card secondCard = drawManager.DrawKing(stateMachine.secondID);
        handManager.AddCardToHand(stateMachine.secondID, secondCard);

        stateMachine.firstCard = firstCard; 
        stateMachine.secondCard = secondCard;

        yield return new WaitForSeconds(WAIT_TIME);

        KingPlacement placement = new KingPlacement(stateMachine);
        stateMachine.Enter(placement); 
    }
}
