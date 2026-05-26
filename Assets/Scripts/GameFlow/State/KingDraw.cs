using Cysharp.Threading.Tasks;
using UnityEngine;

/// <summary>
/// 선·후공 플레이어에게 왕 카드를 드로우하여 핸드에 추가하는 상태.
/// 완료 후 KingPlacement 상태로 전환한다.
/// </summary>
public class KingDraw : IGameState
{
    const int WAIT_TIME_MS = 2000;

    GameFlowStateMachine stateMachine;

    public KingDraw(GameFlowStateMachine stateMachine)
    {
        this.stateMachine = stateMachine;
    }

    /// <summary>
    /// 2초 대기 후 선·후공 순서로 왕 카드를 드로우해 핸드에 추가한다.
    /// stateMachine에 firstCard/secondCard를 저장하고 KingPlacement로 전환한다.
    /// </summary>
    public async UniTask Enter()
    {
        await UniTask.Delay(WAIT_TIME_MS);

        HandManager handManager = ServiceLocator.Get<HandManager>();
        DrawManager drawManager = ServiceLocator.Get<DrawManager>();

        Debug.Log("Draw King Phase");

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
