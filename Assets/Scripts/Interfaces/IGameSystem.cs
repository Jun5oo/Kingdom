using UnityEngine;

public interface IGameSystem
{
    // 게임 시작 및 종료 시, 활성화, 비활성화가 필요한 게임 시스템 인터페이스 
    public void EnableSystem();
    public void DisableSystem();

}
