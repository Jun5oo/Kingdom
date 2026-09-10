using System;
using System.Collections.Generic;
using UnityEngine;

namespace Refactor
{
    // 대상 후보를 context.Targets에 채우고, 유효하면 true.
    // plain 클래스 + SerializeReference로 EffectData 안에 인라인 저장된다.
    // 구체 타입에는 [Serializable] 필수.

    [Serializable]
    public abstract class Target
    {
        public abstract bool Resolve(EffectContext context);
    }

    // 필터 기준
    public enum Side { Ally, Enemy, All }

    // 시전자 자신 (기존 Target.Self)
    [Serializable]
    public class SelfTarget : Target
    {
        public override bool Resolve(EffectContext context)
        {
            Token caster = context.Caster as Token;
            if (caster == null) return false;

            TokenManager tokenManager = ServiceLocator.Get<TokenManager>();
            Vector2Int pos = tokenManager.GetGridPositionOfToken(caster);
            if (pos == -Vector2Int.one) return false;

            context.SetTargets(new[] { pos });
            return true;
        }
    }

    // 필드의 모든 유닛 중 side에 맞는 것 
    [Serializable]
    public class AllUnitsTarget : Target
    {
        [SerializeField] Side side = Side.Enemy;
        [SerializeField] bool requireAtLeastOne = true;

        public override bool Resolve(EffectContext context)
        {
            TokenManager tokenManager = ServiceLocator.Get<TokenManager>();
            GridManager gridManager = ServiceLocator.Get<GridManager>();
            int casterId = context.CasterOwnerId;

            var result = new List<Vector2Int>();
            foreach (var pos in gridManager.GetAllPositions())
            {
                if (!tokenManager.TryGetTokenFrom(pos, out Token token)) continue;

                bool ally = token.OwnerID == casterId;
                if (side == Side.Ally && !ally) continue;
                if (side == Side.Enemy && ally) continue;

                result.Add(pos);
            }

            context.SetTargets(result);
            return !requireAtLeastOne || result.Count > 0;
        }
    }

    // 이벤트가 채운 위치 (기존 Target.Kill / Target.Death)
    // 예: ContextKey.Victim = 죽은 유닛, ContextKey.Killer = 처치한 유닛
    [Serializable]
    public class ContextPositionTarget : Target
    {
        [SerializeField] ContextKey key = ContextKey.Victim;

        public override bool Resolve(EffectContext context)
        {
            if (!context.TryGet(key, out ObjectContext obj)) return false;

            context.SetTargets(new[] { obj.gridPosition });
            return true;
        }
    }

    // 빈 칸 하나 (기존 ConditionGrid: 소환용 등)
    [Serializable]
    public class EmptyCellTarget : Target
    {
        public override bool Resolve(EffectContext context)
        {
            TokenManager tokenManager = ServiceLocator.Get<TokenManager>();
            GridManager gridManager = ServiceLocator.Get<GridManager>();

            var result = new List<Vector2Int>();
            foreach (var pos in gridManager.GetAllPositions())
            {
                if (!tokenManager.IsTokenAtGridPosition(pos))
                    result.Add(pos);
            }

            context.SetTargets(result);
            return result.Count > 0;
        }
    }
}
