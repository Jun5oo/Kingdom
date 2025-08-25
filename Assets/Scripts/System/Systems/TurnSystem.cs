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

    AIController aiController;

    TurnState currentTurnState; 
    public TurnState TurnState { get { return currentTurnState; } }

    int[] playerID;
    int currentPlayerID;

    public Action<int> onTurnStarted;
    public Action<int> onTurnEnded; 

    public void Init()
    {
        this.playerManager = ServiceLocator.Get<PlayerManager>();
        this.drawManager = ServiceLocator.Get<DrawManager>();
        this.uiManager = ServiceLocator.Get<UIManager>();
        this.handManager = ServiceLocator.Get<HandManager>(); 
        this.actionSystem = ServiceLocator.Get<ActionSystem>();
        aiController = ServiceLocator.Get<AIController>();

        DisableSystem(); 
    }
    public void SetTurnOrder(int[] playerID)
    {
        this.playerID = playerID;
        currentPlayerID = playerID[0];

    }
    public async UniTask BeginTurnLoop()
    {
        Debug.Log("TurnLoop"); 
        await StartTurn(); 
    }
    public async UniTask StartTurn()
    {
        if (TurnState == TurnState.Unable)
            return;

        actionSystem.ResetActionCount(currentPlayerID);

        EventBus<TurnStartEvent>.Publish(new TurnStartEvent { playerID = currentPlayerID }); 

        onTurnStarted?.Invoke(currentPlayerID);

        if (playerManager.Local.PlayerID == currentPlayerID)
            uiManager.OnNotification("내 턴", () => 
            { 
                currentTurnState = TurnState.PlayerTurn;
            });

        else
            uiManager.OnNotification("상대 턴", () => 
            { 
                currentTurnState = TurnState.EnemyTurn;
                StartAITurn(currentPlayerID);
            });

        Card card = await drawManager.Draw(currentPlayerID);

        if (card == null)
            return; 

        handManager.AddCardToHand(currentPlayerID, card); 
    }

    private void StartAITurn(int currentPlayerID)
    {
        aiController.InvokeRandomAction(currentPlayerID, () => EndTurn());
    }

    public async UniTask EndTurn()
    {
        if (TurnState == TurnState.Unable)
            return;

        onTurnEnded?.Invoke(currentPlayerID);
        EventBus<TurnEndEvent>.Publish(new TurnEndEvent { playerID = currentPlayerID });

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
