using UnityEngine;

/// <summary>
/// 플레이어 리소스(행동 포인트, 어빌리티 코인 등)의 공통 인터페이스.
/// </summary>
public interface IResourceSystem
{
    public void Init();

    /// <summary> 리소스를 amount만큼 추가한다. </summary>
    public void Add(int playerID, int amount);
    /// <summary> 리소스를 cost만큼 차감한다. </summary>
    public void Consume(int playerID, int cost);
    /// <summary> 해당 플레이어가 cost 이상의 리소스를 보유하고 있는지 확인한다. </summary>
    public bool IsEnoughResources(int playerID, int cost);
    /// <summary> 현재 보유 리소스 양을 반환한다. </summary>
    public int GetCurrentResources(int playerID);
}
