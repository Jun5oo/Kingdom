using Cysharp.Threading.Tasks;
using UnityEngine;

public class KingDraw : IGameState
{
    const int WAIT_TIME_MS = 2000;

    GameFlowStateMachine stateMachine;

    public KingDraw(GameFlowStateMachine stateMachine)
    {
        this.stateMachine = stateMachine; 
    }

    public async UniTask Enter()
    {
        HandManager handManager = ServiceLocator.Get<HandManager>();
        DrawManager drawManager = ServiceLocator.Get<DrawManager>();
        Debug.Log("KingDraw"); 

        Card firstCard = await drawManager.DrawKing(stateMachine.firstID);
        handManager.AddCardToHand(stateMachine.firstID, firstCard); 
        Card secondCard = await drawManager.DrawKing(stateMachine.secondID);
        handManager.AddCardToHand(stateMachine.secondID, secondCard);

        stateMachine.firstCard = firstCard; 
        stateMachine.secondCard = secondCard;

        await UniTask.Delay(WAIT_TIME_MS);

        KingPlacement placement = new KingPlacement(stateMachine);
        stateMachine.Enter(placement); 
    }
}
