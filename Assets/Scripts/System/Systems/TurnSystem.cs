using Cysharp.Threading.Tasks;
using System;
using UnityEngine; 

public enum TurnState
{
    Unable, 
    PlayerTurn, 
    EnemyTurn, 
    Waiting, 
    EndTurn, 
    GameOver 
}
public class TurnSystem : IGameSystem
{
    PlayerManager playerManager;
    UIManager uiManager;
    HandManager handManager;
    DrawManager drawManager; 
    ActionSystem actionSystem;

    TurnState currentTurnState; 
    public TurnState TurnState { get { return currentTurnState; } }

    int[] playerID;
    int currentPlayerID;

    public Action OnPlayerTurnStarted;
    public Action OnOpponentTurnStarted; 
    public Action OnPlayerTurnEnded;
    public Action OnOpponentTurnEnded;

    public void Init()
    {
        this.playerManager = ServiceLocator.Get<PlayerManager>();
        this.drawManager = ServiceLocator.Get<DrawManager>();
        this.uiManager = ServiceLocator.Get<UIManager>();
        this.handManager = ServiceLocator.Get<HandManager>(); 
        this.actionSystem = ServiceLocator.Get<ActionSystem>();

        DisableSystem(); 
    }
    public void SetTurnOrder(int[] playerID)
    {
        this.playerID = playerID;
        currentPlayerID = playerID[0];

    }
    public async UniTask BeginTurnLoop()
    {
        Debug.Log("Start TurnLoop!"); 
        await StartTurn(); 
    }
    public async UniTask StartTurn()
    {
        if (TurnState == TurnState.Unable)
            return;

        actionSystem.ResetActionCount(currentPlayerID); 

        if (playerManager.Local.PlayerID == currentPlayerID)
            uiManager.OnNotification("My Turn!", () => 
            { 
                currentTurnState = TurnState.PlayerTurn;
                OnPlayerTurnStarted?.Invoke(); 
            });

        else
            uiManager.OnNotification("Enemy Turn!", () => 
            { 
                currentTurnState = TurnState.EnemyTurn;
                OnOpponentTurnStarted?.Invoke(); 
            });

        Card card = await drawManager.Draw(currentPlayerID);
        handManager.AddCardToHand(currentPlayerID, card); 
    }
    public async UniTask EndTurn()
    {
        if (TurnState == TurnState.Unable)
            return;

        if (currentTurnState == TurnState.PlayerTurn)
            OnPlayerTurnEnded?.Invoke();
        else
            OnOpponentTurnEnded?.Invoke(); 

        currentTurnState = TurnState.EndTurn;

        foreach(var _playerID in playerID) 
        {
            if (currentPlayerID != _playerID)
            {
                currentPlayerID = _playerID;
                break; 
            }
        }

        await StartTurn(); 
    }
    public void Wait() => currentTurnState = TurnState.Waiting;
    public int GetCurrentTurnPlayerID() => currentPlayerID;
    public bool IsMyTurn() => currentPlayerID == playerManager.Local.PlayerID; 
    public void EnableSystem()
    {
        Wait(); 
        BeginTurnLoop(); 
    }
    public void DisableSystem() => currentTurnState = TurnState.Unable; 
}
