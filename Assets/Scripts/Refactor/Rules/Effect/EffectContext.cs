using System.Collections.Generic;

namespace Refactor
{
    // 한 번의 효과 해석 동안 파이프라인 각 단계로 넘어가는 정보.
    // 자주 쓰는 것은 타입 필드, 나머지는 payload로.
    // 수정 예정. 

    public class EffectContext
    {
        // 효과를 발동하는 주체
        public UnitState Caster { get; set; }
        public int CasterOwnerId => Caster != null ? Caster.OwnerId : -1;

        // 발동 중인 효과(런타임)와 이를 촉발한 이벤트
        public Effect Source { get; set; }
        public GameEvent SourceEvent { get; set; }

        // Target.Resolve가 채운다. Operation이 읽는다.
        public List<GridPos> Targets { get; } = new List<GridPos>();

        // 특수 정보 (파괴된 유닛 스냅샷 등). 키는 PayloadKey.
        Dictionary<PayloadKey, object> payload;

        public void Set<T>(PayloadKey key, T value)
        {
            payload ??= new Dictionary<PayloadKey, object>();
            payload[key] = value;
        }

        public bool TryGet<T>(PayloadKey key, out T value)
        {
            if (payload != null && payload.TryGetValue(key, out object o) && o is T t)
            {
                value = t;
                return true;
            }

            value = default;
            return false;
        }

        public void SetTargets(IEnumerable<GridPos> positions)
        {
            Targets.Clear();
            if (positions != null)
                Targets.AddRange(positions);
        }
    }

    public enum PayloadKey
    {
        None,

        // 이 효과를 촉발한 파괴 이벤트의 정보
        Destroyer,              // 파괴를 일으킨 유닛
        DestroyedUnit,          // 파괴된 유닛

        // 가장 최근 기록
        LastSummonedUnit,       // 마지막으로 소환된 유닛
        LastDestroyedUnit,      // 마지막으로 파괴된 유닛
    }
}
