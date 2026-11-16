using System.Collections.Generic;
using Newtonsoft.Json;

namespace Refactor
{
    // 효과의 실제 동작 하나. 대상이 사라졌으면 건너뛴다(§4.4).
    public abstract class Operation
    {
        public abstract void Execute(EffectContext context);
    }

    public enum OperationTarget
    {
        Self,    // 효과 주인
        Targets, // 선택 대기에서 고른 대상
    }

    public sealed class ReleaseOperation : Operation
    {
        [JsonProperty] private OperationTarget target = OperationTarget.Self;

        public override void Execute(EffectContext context)
        {
            if (target == OperationTarget.Self)
            {
                context.Processor.Release(context.SourceUnitId, context.OwnerId);
                return;
            }

            foreach (TargetUnit unit in context.Targets)
                context.Processor.Release(unit.UnitId, context.OwnerId);
        }
    }

    public sealed class ChangeAbilityResourceOperation : Operation
    {
        [JsonProperty(Required = Required.Always)] private int amount = 0;

        public override void Execute(EffectContext context)
            => context.Processor.ChangeAbilityResource(context.OwnerId, amount);
    }

    // 발동 이벤트로 죽은 유닛의 칸에 효과 주인의 유닛으로 소환한다. 소환된 유닛은 죽은 유닛을 원래 카드로 기억한다.
    public sealed class SummonOperation : Operation
    {
        [JsonProperty(Required = Required.Always)] public string CardId { get; private set; }
        [JsonProperty] public int Level { get; private set; } = 1;

        public override void Execute(EffectContext context)
        {
            if (context.TriggerEvent is UnitDied died)
                context.Processor.TrySummon(CardId, Level, context.OwnerId, died.Position, died.CardId);
        }
    }

    // 고른 대상의 칸에 그 대상의 원래 카드를 효과 주인의 유닛으로 소환한다. 앞선 Operation이 칸을 비워야 한다.
    public sealed class ReviveOperation : Operation
    {
        public override void Execute(EffectContext context)
        {
            foreach (TargetUnit unit in context.Targets)
            {
                // shortcut: 레벨업이 없어 1성으로 부활한다. 레벨업을 만들 때 원래 등급을 기억할지 정한다.
                if (unit.OriginCardId != null)
                    context.Processor.TrySummon(unit.OriginCardId, 1, context.OwnerId, unit.Position);
            }
        }
    }

    // 고른 대상의 칸에 오프셋(보드 절대 좌표, §5.4)을 더한 칸의 상대 유닛에게 효과 주인의 CP만큼 피해를 준다. 반격은 없다.
    public sealed class AreaDamageOperation : Operation
    {
        [JsonProperty(Required = Required.Always)] private List<GridPos> offsets = new();

        public override void Execute(EffectContext context)
        {
            // shortcut: 주인이 떠났으면 CP를 읽을 수 없어 건너뛴다. 사망 시 발동하는 범위 피해 카드가 생기면 떠나기 직전 CP를 쓰도록 바꾼다.
            if (!context.State.Units.TryGetValue(context.SourceUnitId, out UnitState source))
                return;

            CombatResolver combat = new();
            foreach (TargetUnit center in context.Targets)
            {
                foreach (GridPos offset in offsets)
                {
                    int x = center.Position.X + offset.X;
                    int y = center.Position.Y + offset.Y;
                    if (!context.State.Board.Contains(x, y))
                        continue;

                    int? unitId = context.State.Board.GetUnitIdAt(x, y);
                    if (!unitId.HasValue)
                        continue;

                    UnitState unit = context.State.Units[unitId.Value];
                    // 앞선 피해로 이미 HP가 0 이하인 유닛은 사망 처리를 기다리므로 다시 때리지 않는다.
                    if (!PlayerRelation.Opponent.Matches(unit.OwnerId, context.OwnerId) || unit.CurrentHP <= 0)
                        continue;

                    context.Processor.Damage(unit.Id, combat.CalculateDamage(source, unit), context.OwnerId);
                }
            }
        }
    }
}
