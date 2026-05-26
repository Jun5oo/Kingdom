/// <summary>
/// AgentController가 관리하는 페이즈 상태의 공통 인터페이스.
/// Enter(agent) → Execute() (매 프레임) → Exit() 흐름으로 동작한다.
/// </summary>
public interface IPhaseState
{
    void Enter(AgentController agent);
    void Execute();
    void Exit();
}
