using Newtonsoft.Json;

namespace Refactor
{
    // 효과의 실제 동작 하나. 대상이 사라졌으면 건너뛴다(§4.4).
    public abstract class Operation
    {
        public abstract void Execute(EffectContext context);
    }

    // 효과 주인 자신을 릴리스한다.
    public sealed class ReleaseOperation : Operation
    {
        public override void Execute(EffectContext context)
            => context.Processor.Release(context.SourceUnitId, context.OwnerId);
    }

    public sealed class ChangeAbilityResourceOperation : Operation
    {
        [JsonProperty(Required = Required.Always)] private int amount = 0;

        public override void Execute(EffectContext context)
            => context.Processor.ChangeAbilityResource(context.OwnerId, amount);
    }

    // 발동 이벤트로 죽은 유닛의 칸에 효과 주인의 유닛으로 소환한다.
    public sealed class SummonOperation : Operation
    {
        [JsonProperty(Required = Required.Always)] public string CardId { get; private set; }
        [JsonProperty] public int Level { get; private set; } = 1;

        public override void Execute(EffectContext context)
        {
            if (context.TriggerEvent is UnitDied died)
                context.Processor.TrySummon(CardId, Level, context.OwnerId, died.Position);
        }
    }
}
