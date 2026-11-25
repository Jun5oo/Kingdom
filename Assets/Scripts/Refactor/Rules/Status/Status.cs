namespace Refactor
{
    // 유닛에 붙는 상태 이상 하나. 하스스톤의 강화효과에 해당한다(§5.3).
    public sealed class Status
    {
        // 같은 종류가 여러 개 쌓일 수 있으므로 이벤트에서 어느 것인지 가리킬 때 쓴다.
        public int Id { get; }
        // 키워드가 아니면 null.
        public StatusType? Type { get; }
        // null이면 영구다.
        public int? RemainingTurns { get; internal set; }
        // 유닛 CP에 더하는 값. 조회할 때 합산한다.
        public int BonusCP { get; }
        public int OwnerId { get; }
        // 상태 이상을 건 유닛. 같은 카드 여러 장 중 어느 것이 걸었는지 구별한다. 이미 보드를 떠났을 수 있다.
        public int SourceUnitId { get; }
        public string SourceCardId { get; }
        // 턴 만료 때만 트리거 큐에 넣는다. 큐가 EffectData를 받으므로 operation 하나짜리로 감싸 둔다.
        internal EffectData OnExpire { get; }

        internal Status(int id, StatusType? type, int? remainingTurns, int bonusCP, int ownerId, int sourceUnitId, string sourceCardId, EffectData onExpire)
        {
            Id = id;
            Type = type;
            RemainingTurns = remainingTurns;
            BonusCP = bonusCP;
            OwnerId = ownerId;
            SourceUnitId = sourceUnitId;
            SourceCardId = sourceCardId;
            OnExpire = onExpire;
        }
    }
}
