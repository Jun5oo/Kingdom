using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 플레이어의 행동 포인트(Action Point)를 관리하는 리소스 시스템.
/// 턴 시작 시 ResetResources로 최대값으로 초기화되며, 액션 실행 시 Consume으로 차감된다.
/// </summary>
public class ActionResourceSystem : IActionResourceSystem
{
    Dictionary<int, int> playerActionResources;    // 플레이어별 현재 행동 포인트
    Dictionary<int, int> playerMaxActionResources; // 플레이어별 최대 행동 포인트

    public Action<int> onActionResourceChanged; // 행동 포인트 변경 시 발생 (UI 업데이트용)

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
