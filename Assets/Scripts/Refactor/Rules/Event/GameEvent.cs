namespace Refactor
{
    // 엔진이 돌려주는 사건. 네트워크로 보낼 수 있도록 ID와 값만 담는다.
    public abstract class GameEvent
    {
        // null이면 전체 공개, 값이 있으면 그 플레이어에게만 공개한다.
        public int? VisibleTo { get; }

        protected GameEvent(int? visibleTo = null)
        {
            VisibleTo = visibleTo;
        }
    }

    public sealed class UnitMoved : GameEvent
    {
        public int UnitId { get; }
        public GridPos From { get; }
        public GridPos To { get; }

        public UnitMoved(int unitId, GridPos from, GridPos to)
        {
            UnitId = unitId;
            From = from;
            To = to;
        }
    }

    public sealed class UnitDamaged : GameEvent
    {
        public int UnitId { get; }
        public int Amount { get; }
        public int RemainingHP { get; }

        public UnitDamaged(int unitId, int amount, int remainingHP)
        {
            UnitId = unitId;
            Amount = amount;
            RemainingHP = remainingHP;
        }
    }

    public sealed class UnitDied : GameEvent
    {
        public int UnitId { get; }
        public DeathCause Cause { get; }

        public UnitDied(int unitId, DeathCause cause)
        {
            UnitId = unitId;
            Cause = cause;
        }
    }

    public sealed class ActionResourceChanged : GameEvent
    {
        public int PlayerId { get; }
        public int Remaining { get; }

        public ActionResourceChanged(int playerId, int remaining)
        {
            PlayerId = playerId;
            Remaining = remaining;
        }
    }

    public sealed class TurnStarted : GameEvent
    {
        public int PlayerId { get; }
        public int TurnNumber { get; }

        public TurnStarted(int playerId, int turnNumber)
        {
            PlayerId = playerId;
            TurnNumber = turnNumber;
        }
    }
}
