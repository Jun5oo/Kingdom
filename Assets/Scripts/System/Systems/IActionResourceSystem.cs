using UnityEngine;

/// <summary>
/// 최대치와 초기화 기능이 있는 행동 리소스 시스템 인터페이스.
/// IResourceSystem을 상속하며 최대값 관리 및 턴 시작 시 초기화를 추가로 제공한다.
/// </summary>
public interface IActionResourceSystem : IResourceSystem
{
    public int GetMaxResources(int playerID);
    public int IncreaseMaxResources(int playerID, int amount); // 최대 행동 포인트를 증가시킨다.
    public void ResetResources(int playerID);                  // 턴 시작 시 현재 포인트를 최대값으로 초기화한다.
}
