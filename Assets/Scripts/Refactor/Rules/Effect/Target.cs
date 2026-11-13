using System.Collections.Generic;
using Newtonsoft.Json;

namespace Refactor
{
    // 효과 대상의 후보를 정한다. 실제 대상은 선택 대기(§2.3)에서 효과 주인이 고른다.
    // JSON에서는 "type"으로 구체 클래스를 고른다(CardJsonLoader 등록표).
    public abstract class Target
    {
        // 고를 개수. 후보가 이보다 적으면 후보 수만큼 고른다.
        [JsonProperty] public int Count { get; private set; } = 1;

        // 상태를 바꾸지 않는다. 필드에 나온 순서(유닛 ID)로 정렬해 돌려준다.
        public abstract List<int> GetCandidates(EffectContext context);
    }

    // 보드 위 유닛 중 주인과 분류가 맞는 유닛.
    public sealed class ChooseUnitTarget : Target
    {
        [JsonProperty] private PlayerRelation owner = PlayerRelation.Any;
        [JsonProperty] private UnitTag? tag = null;

        public override List<int> GetCandidates(EffectContext context)
        {
            List<int> candidates = new();
            foreach (UnitState unit in context.State.Units.Values)
            {
                // HP가 0 이하인 유닛은 사망 처리를 기다리는 중이므로 고를 수 없다.
                if (unit.CurrentHP <= 0 || !owner.Matches(unit.OwnerId, context.OwnerId))
                    continue;

                if (!tag.HasValue ||
                    (context.Cards.TryGetCard(unit.CardDefinitionId, out CardDefinition card) && card.Tag == tag.Value))
                    candidates.Add(unit.Id);
            }

            candidates.Sort();
            return candidates;
        }
    }
}
