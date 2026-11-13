using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace Refactor
{
    // 발동 비용. IsCostEnough로 지불 가능 여부 판정, Pay로 실제 차감.
    // 호출 순서: IsCostEnough 전부 통과 후, Target/Operation 진행 직전 지불.
    // JSON에서는 "type"으로 구체 클래스를 고른다(CardJsonLoader 등록표).
    public abstract class Cost
    {
        public abstract bool IsCostEnough(EffectContext context);
        public abstract void Pay(EffectContext context);
    }

    // 나열된 비용을 모두 지불
    public class AllCost : Cost
    {
        [JsonProperty] private List<Cost> costs = new();

        public override bool IsCostEnough(EffectContext context)
            => costs.All(c => c == null || c.IsCostEnough(context));

        public override void Pay(EffectContext context)
        {
            foreach (Cost c in costs)
                c?.Pay(context);
        }
    }

    // 효과 주인의 능력 자원을 쓴다.
    public sealed class AbilityResourceCost : Cost
    {
        [JsonProperty(Required = Required.Always)] private int amount = 0;

        public override bool IsCostEnough(EffectContext context)
            => context.State.Players[context.OwnerId].AbilityResource >= amount;

        public override void Pay(EffectContext context)
            => context.Processor.ChangeAbilityResource(context.OwnerId, -amount);
    }
}
