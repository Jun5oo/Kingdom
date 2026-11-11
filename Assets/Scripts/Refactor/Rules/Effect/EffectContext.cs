using System.Collections.Generic;

namespace Refactor
{
    // 효과 하나를 해결하는 동안 부품(Condition, Operation 등)이 읽는 정보.
    public sealed class EffectContext
    {
        public GameState State { get; }
        public CardDatabase Cards { get; }
        // 효과를 가진 유닛. 이미 보드를 떠났을 수 있다.
        public int SourceUnitId { get; }
        public int OwnerId { get; }
        // 효과를 발동시킨 이벤트. 기동 효과면 null.
        public GameEvent TriggerEvent { get; }

        // 상태 변경 입구. 발동 가능 여부만 확인할 때는 null이다.
        internal EffectProcessor Processor { get; }

        // Target.Resolve가 채운다. 대상 표현은 3, 4단계에서 정리한다.
        public List<GridPos> Targets { get; } = new();

        internal EffectContext(GameState state, CardDatabase cards, EffectProcessor processor,
            int sourceUnitId, int ownerId, GameEvent triggerEvent)
        {
            State = state;
            Cards = cards;
            Processor = processor;
            SourceUnitId = sourceUnitId;
            OwnerId = ownerId;
            TriggerEvent = triggerEvent;
        }
    }
}
