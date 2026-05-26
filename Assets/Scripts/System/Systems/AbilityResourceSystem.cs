using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 플레이어의 어빌리티 코인을 관리하는 리소스 시스템.
/// 아군 유닛이 적을 처치하거나 특정 패시브 발동 시 코인이 추가되며,
/// DivineShield, Resurrection 등 능력 액션 실행 시 차감된다.
/// ActionResourceSystem과 달리 턴 시작 시 자동 초기화되지 않는다.
/// </summary>
public class AbilityResourceSystem : IResourceSystem
{
    Dictionary<int, int> playerAbilityResources; // 플레이어별 현재 어빌리티 코인

    public Action<int, int> onAbilityCountChanged; // (playerID, count) 코인 변경 시 발생 (UI 업데이트용)

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
