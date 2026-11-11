using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Refactor
{
    // 발동 방식
    public enum EffectType
    {
        Ignition,   // 플레이어 직접 발동
        Trigger,    // 게임 이벤트로 자동 발동
        Continuous, // 필드에 있는 동안 지속
    }

    // Trigger 타입일 때 어떤 이벤트에 반응하는지.
    public enum EventTrigger
    {
        None,
        OnTurnStarted,
        OnTurnEnded,
        OnUnitDead,
    }

    // Continuous 효과가 언제 사라지는지
    [Flags]
    public enum ResetFlag
    {
        None          = 0,
        OnLeaveField  = 1 << 0,
        OnSelfTurnEnd = 1 << 1,
        OnOppoTurnEnd = 1 << 2,
    }

    // 카드 JSON의 효과 정의. 런타임 상태(사용 횟수 등)는 UnitState가 가진다.
    public sealed class EffectData
    {
        [JsonProperty] public string Id { get; private set; }
        [JsonProperty] public string DisplayName { get; private set; }
        [JsonProperty] public string Description { get; private set; }

        // 발동
        [JsonProperty] public EffectType Type { get; private set; }
        [JsonProperty] public EventTrigger Trigger { get; private set; }     // Trigger 전용
        [JsonProperty] public ResetFlag ResetFlag { get; private set; }      // Continuous 전용
        // 턴당 사용 횟수. 0은 무제한.
        [JsonProperty] public int UsesPerTurn { get; private set; }

        // 파이프라인
        [JsonProperty] public Condition Condition { get; private set; }  // null = 조건 없음
        [JsonProperty] public Cost Cost { get; private set; }            // null = 비용 없음
        [JsonProperty] public Target Target { get; private set; }        // 대상 산출 + 필터
        // 순서대로 실행한다.
        [JsonProperty] public IReadOnlyList<Operation> Operations { get; private set; } = new List<Operation>();
    }
}
