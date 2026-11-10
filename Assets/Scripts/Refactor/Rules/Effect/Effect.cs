using System;
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

    // 의미 분류 
    public enum EffectCategory
    {
        Summon,
        Damage,
        Destroy,
        Gain,
        Draw,
        Buff,
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
        OnChainEnd    = 1 << 3,
    }

    // 발동 횟수 제한. 무제한:0.
    public struct CountLimit
    {
        public int perTurn;
        public int perGame;

        public bool HasLimit => perTurn > 0 || perGame > 0;
    }

    // 순수 데이터. UnitLevelData가 List로 보유한다.
    // 4슬롯은 JSON에서 "type"으로 구체 클래스를 고른다.
    public class EffectData
    {
        [JsonProperty] public string Id { get; private set; }
        [JsonProperty] public string DisplayName { get; private set; }
        [JsonProperty] public string Description { get; private set; }

        // 발동
        [JsonProperty] public EffectType Type { get; private set; }
        [JsonProperty] public EventTrigger Trigger { get; private set; }     // Trigger 전용
        [JsonProperty] public ResetFlag ResetFlag { get; private set; }      // Continuous 전용
        [JsonProperty] public CountLimit CountLimit { get; private set; }

        // 파이프라인
        [JsonProperty] public Condition Condition { get; private set; }  // null = 조건 없음
        [JsonProperty] public Cost Cost { get; private set; }            // null = 비용 없음
        [JsonProperty] public Target Target { get; private set; }        // 대상 산출 + 필터
        [JsonProperty] public Operation Operation { get; private set; }  // 실제 효과
    }

    // 런타임 인스턴스. EffectData + 소유자 + 발동 횟수 상태.
    public class Effect
    {
        public EffectData Data { get; }
        public UnitState Owner { get; }

        int usedThisTurn;
        int usedTotal;

        public Effect(EffectData data, UnitState owner)
        {
            Data = data;
            Owner = owner;
        }

        public bool MatchesTrigger(EventTrigger t)
            => Data.Type == EffectType.Trigger && Data.Trigger == t;

        public bool IsWithinCountLimit()
        {
            CountLimit limit = Data.CountLimit;
            if (limit.perTurn > 0 && usedThisTurn >= limit.perTurn) return false;
            if (limit.perGame > 0 && usedTotal >= limit.perGame) return false;
            return true;
        }

        public void MarkUsed()
        {
            usedThisTurn++;
            usedTotal++;
        }

        public void ResetTurnCount() => usedThisTurn = 0;
    }
}
