using System.Collections;
using UnityEngine;

public class TurnSelection : IGameState
{
    const int PLAYER_NUM = 2;
    const float WAIT_TIME = 1f; 

    PlayerManager playerManager;
    UIManager uiManager;

    GameFlowStateMachine stateMachine; 

    public TurnSelection(GameFlowStateMachine stateMachine)
    {
        playerManager = ServiceLocator.Get<PlayerManager>(); 
        uiManager = ServiceLocator.Get<UIManager>();

        this.stateMachine = stateMachine; 
    }

    public IEnumerator Enter()
    {
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

        yield return new WaitForSeconds(WAIT_TIME);

        KingDraw kingDraw = new KingDraw(stateMachine);

        stateMachine.Enter(kingDraw); 
    }
}
