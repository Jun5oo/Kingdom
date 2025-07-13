using Unity.VisualScripting;
using UnityEngine.Rendering;

public class TurnManager
{
    PlayerManager playerManager;
    UIManager uiManager;
    CardManager cardSystem;
    ActionSystem actionSystem;

    int[] playerID;
    int currentPlayerID; 

    public void Init(PlayerManager playerManager, UIManager uiManager, CardManager cardSystem, ActionSystem actionSystem)
    {
        this.playerManager = playerManager;
        this.uiManager = uiManager;
        this.cardSystem = cardSystem;
        this.actionSystem = actionSystem;
    }

    public void SetTurnOrder(int[] playerID)
    {
        this.playerID = playerID;
        currentPlayerID = playerID[0];

        actionSystem.OnActionDepleted -= EndTurn;
        actionSystem.OnActionDepleted += EndTurn;
    }

    public void BeginTurnLoop()
    {
        StartTurn(); 
    }
    public void StartTurn()
    {
        cardSystem.DrawCard(currentPlayerID);

        if (playerManager.LocalPlayerData.PlayerID == currentPlayerID)
            uiManager.OnNotification("My Turn!");
        else
            uiManager.OnNotification("Enemy Turn!"); 
    }

    public void EndTurn()
    {
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

    public int GetCurrentTurnPlayerID()
    {
        return currentPlayerID;
    }
    public bool IsMyTurn()
    {
        return currentPlayerID == playerManager.LocalPlayerData.PlayerID;
    }
}
