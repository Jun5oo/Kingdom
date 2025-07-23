public interface IPlayerState
{
    void Enter(PlayerController player);
    void Execute();
    void Exit();
}
