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

    // 트리거가 해결될 때 유닛은 이미 상태에서 빠져 있으므로, Condition이 읽을 값을 함께 담는다.
    public sealed class UnitDied : GameEvent
    {
        public int UnitId { get; }
        public string CardId { get; }
        public int OwnerId { get; }
        public GridPos Position { get; }
        public DeathCause Cause { get; }
        // 죽게 만든 플레이어. 알 수 없으면 null.
        public int? KillerPlayerId { get; }

        public UnitDied(int unitId, string cardId, int ownerId, GridPos position, DeathCause cause, int? killerPlayerId)
        {
            UnitId = unitId;
            CardId = cardId;
            OwnerId = ownerId;
            Position = position;
            Cause = cause;
            KillerPlayerId = killerPlayerId;
        }
    }

    public sealed class UnitSummoned : GameEvent
    {
        public int UnitId { get; }
        public string CardId { get; }
        public int OwnerId { get; }
        public int Level { get; }
        public GridPos Position { get; }
        public int CP { get; }
        public int HP { get; }

        public UnitSummoned(int unitId, string cardId, int ownerId, int level, GridPos position, int cp, int hp)
        {
            UnitId = unitId;
            CardId = cardId;
            OwnerId = ownerId;
            Level = level;
            Position = position;
            CP = cp;
            HP = hp;
        }
    }

    public sealed class AbilityResourceChanged : GameEvent
    {
        public int PlayerId { get; }
        public int Remaining { get; }

        public AbilityResourceChanged(int playerId, int remaining)
        {
            PlayerId = playerId;
            Remaining = remaining;
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
