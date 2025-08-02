using Cysharp.Threading.Tasks;
using UnityEngine;

public class TurnSelection : IGameState
{
    // 턴 순서를 선택하는 State. 두 플레이어 중, 선공 후공을 정하는 작업 

    const int PLAYER_NUM = 2;
    const int WAIT_TIME_MS = 1000; 

    GameFlowStateMachine stateMachine; 

    public TurnSelection(GameFlowStateMachine stateMachine)
    {
        this.stateMachine = stateMachine; 
    }

    public async UniTask Enter()
    {
        Debug.Log("Turn Selection Phase"); 
        PlayerManager playerManager = ServiceLocator.Get<PlayerManager>();
        UIManager uiManager = ServiceLocator.Get<UIManager>();

        int[] playerID = new int[PLAYER_NUM];

        playerID[0] = playerManager.Local.PlayerID; 
        playerID[1] = playerManager.Remote.PlayerID; 
        
        int idx = Random.Range(0, PLAYER_NUM);

        int first = playerID[idx];
        int second = playerID[PLAYER_NUM - 1 - idx];

        if (first == playerID[0])
            uiManager.OnNotification("선공");
        else
            uiManager.OnNotification("후공");

        stateMachine.firstID = first; 
        stateMachine.secondID = second;

        await UniTask.Delay(WAIT_TIME_MS);
        KingDraw kingDraw = new KingDraw(stateMachine);
        stateMachine.Enter(kingDraw); 
    }
}
