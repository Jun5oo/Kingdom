namespace Refactor
{
    // 해결을 기다리는 효과 하나. 트리거 큐와 선택 대기로 멈춘 효과가 함께 쓴다.
    internal readonly struct PendingEffect
    {
        public int SourceUnitId { get; }
        // 효과를 정의한 카드. 주인 유닛이 보드를 떠나도 읽을 수 있게 담아 둔다.
        public string SourceCardId { get; }
        public int OwnerId { get; }
        public EffectData Effect { get; }
        public int EffectIndex { get; }
        // 기동 효과면 null.
        public GameEvent TriggerEvent { get; }

        public PendingEffect(int sourceUnitId, string sourceCardId, int ownerId, EffectData effect, int effectIndex, GameEvent triggerEvent)
        {
            SourceUnitId = sourceUnitId;
            SourceCardId = sourceCardId;
            OwnerId = ownerId;
            Effect = effect;
            EffectIndex = effectIndex;
            TriggerEvent = triggerEvent;
        }
    }
}
