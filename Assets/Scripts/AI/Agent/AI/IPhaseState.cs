public interface IPhaseState
{
    void Enter(AgentController agent);
    void Execute();
    void Exit();
}
