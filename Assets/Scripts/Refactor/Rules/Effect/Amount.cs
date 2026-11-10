using Newtonsoft.Json;

namespace Refactor
{
    // Damage / Heal / Gain 등에서 공용으로 쓰는 수치 계산기.
    // JSON에서는 "type"으로 구체 클래스를 고른다(CardJsonLoader 등록표).
    public abstract class Amount
    {
        public abstract int Resolve(EffectContext context, UnitState target);
    }

    // 1. 고정 수치
    public class FlatAmount : Amount
    {
        [JsonProperty] private int value = 0;

        public override int Resolve(EffectContext context, UnitState target) => value;
    }

    // 2. 피격한 대상 수 기반
    public class CountScaledAmount : Amount
    {
        [JsonProperty] private int perTarget = 1;
        [JsonProperty] private int flatBonus = 0;

        public override int Resolve(EffectContext context, UnitState target)
            => context.Targets.Count * perTarget + flatBonus;
    }
}
