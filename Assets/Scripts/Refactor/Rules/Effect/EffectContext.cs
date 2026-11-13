using System;
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
        internal Processor Processor { get; }

        // 고른 대상. 해결 도중 보드를 떠나도 위치와 원래 카드를 읽을 수 있게 고른 시점의 값을 둔다.
        public IReadOnlyList<TargetUnit> Targets { get; internal set; } = Array.Empty<TargetUnit>();

        internal EffectContext(GameState state, CardDatabase cards, Processor processor,
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

    public readonly struct TargetUnit
    {
        public int UnitId { get; }
        public GridPos Position { get; }
        public string OriginCardId { get; }

        public TargetUnit(int unitId, GridPos position, string originCardId)
        {
            UnitId = unitId;
            Position = position;
            OriginCardId = originCardId;
        }
    }
}
