using System;

public enum TurnState
{
    Unable, 
    PlayerTurn, 
    EnemyTurn, 
    Waiting, 
    EndTurn, 
    GameOver 
}
public class TurnManager : IGameSystem
{
    PlayerManager playerManager;
    UIManager uiManager;
    PlayerHandManager handManager;
    DrawManager drawManager; 
    ActionSystem actionSystem;

    AIController AIController;

    TurnState currentTurnState; 
    public TurnState TurnState { get { return currentTurnState; } }

    int[] playerID;
    int currentPlayerID;

    public Action OnTurnStarted; 
    public Action OnTurnEnded;

    public void Init()
    {
        this.playerManager = ServiceLocator.Get<PlayerManager>();
        this.drawManager = ServiceLocator.Get<DrawManager>();
        this.uiManager = ServiceLocator.Get<UIManager>();
        this.handManager = ServiceLocator.Get<PlayerHandManager>(); 
        this.actionSystem = ServiceLocator.Get<ActionSystem>();
        AIController = ServiceLocator.Get<AIController>();

        DisableSystem(); 
    }
    public void SetTurnOrder(int[] playerID)
    {
        this.playerID = playerID;
        currentPlayerID = playerID[0];

        actionSystem.OnActionDepleted -= Wait;  
        actionSystem.OnActionDepleted += Wait;
    }
    public void BeginTurnLoop()
    {
        StartTurn(); 
    }
    public void StartTurn()
    {
        actionSystem.ResetActionCount(); 

        if (playerManager.Local.PlayerID == currentPlayerID)
            uiManager.OnNotification("My Turn!", () => 
            { 
                currentTurnState = TurnState.PlayerTurn;
                OnTurnStarted?.Invoke(); 
            });

        else
            uiManager.OnNotification("Enemy Turn!", () => 
            { 
                currentTurnState = TurnState.EnemyTurn;
                OnTurnStarted?.Invoke();

                StartAITurn(currentPlayerID);
            });

        Card card = drawManager.Draw(currentPlayerID);
        handManager.AddCardToHand(currentPlayerID, card); 
    }

    private void StartAITurn(int currentPlayerID)
    {
        AIController.InvokeRandomAction(currentPlayerID, EndTurn);
    }

    public void EndTurn()
    {
        currentTurnState = TurnState.EndTurn;
        OnTurnEnded?.Invoke(); 

        foreach(var _playerID in playerID) 
        {
            if (currentPlayerID != _playerID)
            {
                currentPlayerID = _playerID;
                break; 
            }
        }

        StartTurn(); 
    }
    public void Wait()
    {
        currentTurnState = TurnState.Waiting; 
    }
    public int GetCurrentTurnPlayerID()
    {
        return currentPlayerID;
    }
    public bool IsMyTurn()
    {
        return currentPlayerID == playerManager.Local.PlayerID;
    }

    public void EnableSystem()
    {
        BeginTurnLoop(); 
    }
    public void DisableSystem()
    {
        currentTurnState = TurnState.Unable; 
    }
}
