using System;
using System.Collections.Generic;
using UnityEngine;

public class AbilityResourceSystem : IResourceSystem
{
    // Key: PlayerID, Value: Ability Coin 
    Dictionary<int, int> playerAbilityResources;

    // PlayerID, AbilityCount
    public Action<int, int> onAbilityCountChanged; 

    public void Init()
    {
        PlayerManager playerManager = ServiceLocator.Get<PlayerManager>(); 

        playerAbilityResources = new Dictionary<int, int>();
        playerAbilityResources.Add(playerManager.Local.PlayerID, 0); 
        playerAbilityResources.Add(playerManager.Remote.PlayerID, 0);
    }

    public void Add(int playerID, int amount)
    {
        playerAbilityResources[playerID] += amount;
        onAbilityCountChanged?.Invoke(playerID, GetCurrentResources(playerID));
    }
    public void Consume(int playerID, int cost)
    {
        playerAbilityResources[playerID] -= cost;
        onAbilityCountChanged?.Invoke(playerID, GetCurrentResources(playerID)); 
    }
    public int GetCurrentResources(int playerID) => playerAbilityResources[playerID];
    public bool IsEnoughResources(int playerID, int cost) => playerAbilityResources[playerID] >= cost;
        
}
