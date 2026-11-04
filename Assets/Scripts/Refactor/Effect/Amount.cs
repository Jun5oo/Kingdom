using System;
using UnityEngine;

namespace Refactor
{
    // Damage / Heal / Gain 등에서 공용으로 쓰는 수치 계산기.
    // plain 클래스 + SerializeReference로 Operation 안에 인라인 저장된다.
    // 구체 타입에는 [Serializable] 필수.

    [Serializable]
    public abstract class Amount
    {
        public abstract int Resolve(EffectContext context, UnitState target);
    }

    // 1. 고정 수치
    [Serializable]
    public class FlatAmount : Amount
    {
        [SerializeField] int value;

        public override int Resolve(EffectContext context, UnitState target) => value;
    }

    // 2. 피격한 대상 수 기반
    [Serializable]
    public class CountScaledAmount : Amount
    {
        [SerializeField] int perTarget = 1;
        [SerializeField] int flatBonus;

        public override int Resolve(EffectContext context, UnitState target)
            => context.Targets.Count * perTarget + flatBonus;
    }
}
