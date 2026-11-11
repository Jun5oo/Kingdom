namespace Refactor
{
    // 플레이어의 행동 요청. 데이터만 담고 실행은 GameEngine이 맡는다.
    public abstract class GameCommand
    {
    }

    public sealed class MoveCommand : GameCommand
    {
        public int UnitId { get; }
        public GridPos Target { get; }

        public MoveCommand(int unitId, GridPos target)
        {
            UnitId = unitId;
            Target = target;
        }
    }

    public sealed class AttackCommand : GameCommand
    {
        public int AttackerId { get; }
        public int DefenderId { get; }

        public AttackCommand(int attackerId, int defenderId)
        {
            AttackerId = attackerId;
            DefenderId = defenderId;
        }
    }

    public sealed class EndTurnCommand : GameCommand
    {
    }
}
