using System;
using System.Collections.Generic;
using UnityEngine;

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
    [Serializable]
    public struct CountLimit
    {
        [Min(0)] public int perTurn;
        [Min(0)] public int perGame;

        public bool HasLimit => perTurn > 0 || perGame > 0;
    }

    // 순수 데이터. CardData가 List로 보유한다.
    // 4슬롯은 SerializeReference로 인라인 저장된다.
    [Serializable]
    public class EffectData
    {
        [SerializeField] string id;
        [SerializeField] string displayName;
        [TextArea(2, 5)]
        [SerializeField] string description;

        [Header("발동")]
        [SerializeField] EffectType type;
        [SerializeField] EventTrigger trigger;   // Trigger 전용 
        [SerializeField] ResetFlag resetFlag;    // Continuous 전용
        [SerializeField] CountLimit countLimit;

        [Header("파이프라인")]
        [SerializeReference, SubclassSelector] Condition condition;  // null = 조건 없음
        [SerializeReference, SubclassSelector] Cost cost;            // null = 비용 없음
        [SerializeReference, SubclassSelector] Target target;        // 대상 산출 + 필터
        [SerializeReference, SubclassSelector] Operation operation;  // 실제 효과

        public string Id => id;
        public string DisplayName => displayName;
        public string Description => description;

        public EffectType Type => type;
        public EventTrigger Trigger => trigger;
        public ResetFlag ResetFlag => resetFlag;
        public CountLimit CountLimit => countLimit;

        public Condition Condition => condition;
        public Cost Cost => cost;
        public Target Target => target;
        public Operation Operation => operation;
    }

    // 런타임 인스턴스. EffectData + 소유자 + 발동 횟수 상태.
    public class Effect
    {
        public EffectData Data { get; }
        public BaseObject Owner { get; }

        int usedThisTurn;
        int usedTotal;

        public Effect(EffectData data, BaseObject owner)
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
