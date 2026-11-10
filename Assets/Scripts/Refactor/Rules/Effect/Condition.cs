using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace Refactor
{
    // "이 효과를 지금 발동해도 되는가?" 만 판정한다.
    // 대상이 유효한지(소유자/태그/빈 칸 등)는 Target 슬롯이 담당한다.
    // JSON에서는 "type"으로 구체 클래스를 고른다(CardJsonLoader 등록표).
    public abstract class Condition
    {
        public abstract bool CanExecute(EffectContext context);
    }

    // ── 복합 조건: 중첩으로 트리 구성 ──

    // 복수 조건
    public class AllCondition : Condition
    {
        [JsonProperty] private List<Condition> conditions = new();

        public override bool CanExecute(EffectContext context)
            => conditions.All(c => c == null || c.CanExecute(context));
    }

    // 하나라도 만족하면 됨
    public class AnyCondition : Condition
    {
        [JsonProperty] private List<Condition> conditions = new();

        public override bool CanExecute(EffectContext context)
            => conditions.Count == 0 || conditions.Any(c => c != null && c.CanExecute(context));
    }

    // 반전
    public class NotCondition : Condition
    {
        [JsonProperty] private Condition inner = null;

        public override bool CanExecute(EffectContext context)
            => inner == null || !inner.CanExecute(context);
    }
}
