using System;
using System.Collections.Generic;

namespace Refactor
{
    // ygopro의 tevent를 참고한 공통 이벤트. 카드 상태 스냅샷은 보관하지 않는다.
    public readonly struct GameEvent
    {
        // 단일 유닛 이벤트의 대상. 공격자를 의미하지 않는다.
        // 현재는 UnitState를 사용한다. 손과 덱의 카드 참조는 추후 확장한다.
        public UnitState TriggerCard { get; }

        // 그룹 이벤트의 대상. 각 요소는 실제 UnitState의 참조다.
        public IReadOnlyList<UnitState> EventCards { get; }

        // 사건을 일으킨 원인 효과. 발동 사건의 대상 효과와는 구분한다.
        public Effect ReasonEffect { get; }

        public GameEventType EventCode { get; }

        // 이벤트별로 의미를 정하는 수치. 예: 실제 피해량.
        public int EventValue { get; }
        public EventReason Reason { get; }

        // 이벤트별로 의미를 정하는 플레이어. 사용하지 않으면 null.
        public int? EventPlayer { get; }

        // 원인을 제공한 플레이어. 이벤트 생성 당시 값을 보관한다.
        public int? ReasonPlayer { get; }

        public GameEvent(
            GameEventType eventCode,
            UnitState triggerCard = null,
            IEnumerable<UnitState> eventCards = null,
            Effect reasonEffect = null,
            EventReason reason = EventReason.None,
            int? eventPlayer = null,
            int? reasonPlayer = null,
            int eventValue = 0)
        {
            if (eventCode == GameEventType.None)
                throw new ArgumentException("이벤트 종류가 필요합니다.", nameof(eventCode));

            EventCode = eventCode;
            TriggerCard = triggerCard;
            ReasonEffect = reasonEffect;
            Reason = reason;
            EventPlayer = eventPlayer;
            ReasonPlayer = reasonPlayer;
            EventValue = eventValue;

            // 원본 목록 변경의 영향을 막는다. 유닛 자체를 복제하지는 않는다.
            EventCards = eventCards == null
                ? Array.Empty<UnitState>()
                : new List<UnitState>(eventCards).AsReadOnly();
        }
    }
}
