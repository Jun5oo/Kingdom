using UnityEngine;

public class GameFlowManager
{
    GameFlowStateMachine stateMachine;

    TurnSelection turnSelection;
    GameOver gameOver;

    PlayerManager playerManager;
    UIManager uiManager; 
    DamageManager damageManager; 

    public void Init()
    {
        stateMachine = new GameFlowStateMachine();

        turnSelection = new TurnSelection(stateMachine);
        gameOver = new GameOver();

        playerManager = ServiceLocator.Get<PlayerManager>(); 
        uiManager = ServiceLocator.Get<UIManager>(); 
        damageManager = ServiceLocator.Get<DamageManager>();

        damageManager.OnKingDefeated -= GameOver;
        damageManager.OnKingDefeated += GameOver; 
    }

    public void GameStart()
    {
        stateMachine.Enter(turnSelection); 
    }

    public void GameOver(int loserID)
    {
        stateMachine.Enter(gameOver);

        if (loserID == playerManager.Local.PlayerID)
            uiManager.OnNotification("패배");
        else
            uiManager.OnNotification("승리"); 
    }
}
