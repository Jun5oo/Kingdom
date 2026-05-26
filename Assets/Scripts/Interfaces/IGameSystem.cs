using UnityEngine;

/// <summary>
/// 턴 전환 시 활성화/비활성화가 필요한 게임 시스템이 구현하는 인터페이스.
/// MainPhase 진입 시 EnableSystem(), 종료 시 DisableSystem()을 호출한다.
/// </summary>
public interface IGameSystem
{
    // 게임 시작 및 종료 시, 활성화, 비활성화가 필요한 게임 시스템 인터페이스
    public void EnableSystem();
    public void DisableSystem();

}
