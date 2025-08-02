using UnityEngine;

public class GameFlowManager
{
    GameFlowStateMachine stateMachine;

    TurnSelection turnSelection;
    GameOver gameOver;

    UIManager uiManager; 
    DamageManager damageManager; 

    public void Init()
    {
        stateMachine = new GameFlowStateMachine();

        turnSelection = new TurnSelection(stateMachine);
        gameOver = new GameOver();

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
        uiManager.OnNotification($"playerID {loserID} lose"); 
    }
}
