using System;
using System.Collections.Generic;

public class ActionResourceSystem : IActionResourceSystem
{
    // 행동을 하기 위해 필요한 Cost ResourceSystem 

    // 현재 플레이어의 ActionResource; 
    Dictionary<int, int> playerActionResources;
    // 플레이어의 Max ActionResource
    Dictionary<int, int> playerMaxActionResources;

    public Action<int> onActionResourceChanged; 

    public void Init()
    {
        PlayerManager playerManager = ServiceLocator.Get<PlayerManager>();

        playerActionResources = new Dictionary<int, int>();
        playerActionResources.Add(playerManager.Local.PlayerID, 2);
        playerActionResources.Add(playerManager.Remote.PlayerID, 2);

        playerMaxActionResources = new Dictionary<int, int>();
        playerMaxActionResources.Add(playerManager.Local.PlayerID, 2);
        playerMaxActionResources.Add(playerManager.Remote.PlayerID, 2);    
    }

    public void Add(int playerID, int amount)
    {
        playerActionResources[playerID] += amount;
        onActionResourceChanged?.Invoke(playerID); 
    }
    public void Consume(int playerID, int cost)
    {
        playerActionResources[playerID] -= cost;
        onActionResourceChanged?.Invoke(playerID);
    }
    public int GetCurrentResources(int playerID) => playerActionResources[playerID];
    public bool IsEnoughResources(int playerID, int cost) => playerActionResources[playerID] >= cost;

    public int GetMaxResources(int playerID) => playerMaxActionResources[playerID];
    public int IncreaseMaxResources(int playerID, int amount) => playerMaxActionResources[playerID] += amount; 
    public void ResetResources(int playerID)
    {
        playerActionResources[playerID] = playerMaxActionResources[playerID];
        onActionResourceChanged?.Invoke(playerID);
    }


}
