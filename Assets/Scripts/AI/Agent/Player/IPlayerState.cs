/// <summary>
/// PlayerController가 관리하는 플레이어 페이즈 상태의 공통 인터페이스 (미구현, 현재 IPhaseState와 역할 중복).
/// </summary>
public interface IPlayerState
{
    void Enter(PlayerController player);
    void Execute();
    void Exit();
}
