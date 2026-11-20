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
        public GridPos Position { get; }
        public int CP { get; }
        public int HP { get; }

        public UnitSummoned(int unitId, string cardId, int ownerId, GridPos position, int cp, int hp)
        {
            UnitId = unitId;
            CardId = cardId;
            OwnerId = ownerId;
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

    // 선택 대기 시작. 후보가 숨겨진 정보일 수 있으므로 고르는 플레이어에게만 공개한다.
    public sealed class ChoiceRequested : GameEvent
    {
        public PendingChoice Choice { get; }

        public ChoiceRequested(PendingChoice choice) : base(choice.PlayerId)
        {
            Choice = choice;
        }
    }

    public sealed class StatusAdded : GameEvent
    {
        public int UnitId { get; }
        public int StatusId { get; }
        // 키워드가 아니면 null.
        public StatusType? Type { get; }
        // null이면 영구.
        public int? RemainingTurns { get; }
        // 화면이 아이콘과 설명을 찾을 때 쓴다.
        public string SourceCardId { get; }

        public StatusAdded(int unitId, int statusId, StatusType? type, int? remainingTurns, string sourceCardId)
        {
            UnitId = unitId;
            StatusId = statusId;
            Type = type;
            RemainingTurns = remainingTurns;
            SourceCardId = sourceCardId;
        }
    }

    public sealed class StatusRemoved : GameEvent
    {
        public int UnitId { get; }
        public int StatusId { get; }

        public StatusRemoved(int unitId, int statusId)
        {
            UnitId = unitId;
            StatusId = statusId;
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
