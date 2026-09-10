using System.Collections.Generic;
using UnityEngine;

namespace Refactor
{
    // 한 번의 효과 해석 동안 파이프라인 각 단계로 넘어가는 정보.
    // 자주 쓰는 것은 타입 필드, 나머지는 payload로.
    // 수정 예정. 

    public class EffectContext
    {
        // 효과를 발동하는 주체
        public BaseObject Caster { get; set; }
        public int CasterOwnerId => Caster != null ? Caster.OwnerID : -1;

        // 발동 중인 효과(런타임)와 이를 촉발한 이벤트
        public Effect Source { get; set; }
        public IGameEvent SourceEvent { get; set; }

        // Target.Resolve가 채운다. Operation이 읽는다.
        public List<Vector2Int> Targets { get; } = new List<Vector2Int>();

        // 특수 정보 (파괴된 유닛 스냅샷 등). 키는 ContextKey.
        Dictionary<ContextKey, object> payload;

        public void Set<T>(ContextKey key, T value)
        {
            payload ??= new Dictionary<ContextKey, object>();
            payload[key] = value;
        }

        public bool TryGet<T>(ContextKey key, out T value)
        {
            if (payload != null && payload.TryGetValue(key, out object o) && o is T t)
            {
                value = t;
                return true;
            }

            value = default;
            return false;
        }

        public void SetTargets(IEnumerable<Vector2Int> positions)
        {
            Targets.Clear();
            if (positions != null)
                Targets.AddRange(positions);
        }
    }

    public enum ContextKey
    {
        None,

        Killer,
        Victim,

        LastSummoned,
        LastDestroyed,
    }
}
