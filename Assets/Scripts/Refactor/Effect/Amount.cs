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
        public abstract int Resolve(EffectContext context, Token target);
    }

    // 1. 고정 수치
    [Serializable]
    public class FlatAmount : Amount
    {
        [SerializeField] int value;

        public override int Resolve(EffectContext context, Token target) => value;
    }

    // 2. 맞은 대상의 스탯 기반 (예: 대상 CP의 절반)
    [Serializable]
    public class TargetScaledAmount : Amount
    {
        public enum Stat { CurrentCP, MaxCP }

        [SerializeField] Stat stat = Stat.CurrentCP;
        [SerializeField] float multiplier = 1f;
        [SerializeField] int flatBonus;

        public override int Resolve(EffectContext context, Token target)
        {
            if (target == null) return 0;
            int baseStat = stat == Stat.MaxCP ? target.MAXCP : target.CP;
            return Mathf.RoundToInt(baseStat * multiplier) + flatBonus;
        }
    }

    // 3. 시전자의 스탯 기반 (공격력 배수)
    [Serializable]
    public class CasterScaledAmount : Amount
    {
        public enum Stat { CurrentCP, MaxCP }

        [SerializeField] Stat stat = Stat.CurrentCP;
        [SerializeField] float multiplier = 1f;
        [SerializeField] int flatBonus;

        public override int Resolve(EffectContext context, Token target)
        {
            Token caster = context.Caster as Token;
            if (caster == null) return 0;
            int baseStat = stat == Stat.MaxCP ? caster.MAXCP : caster.CP;
            return Mathf.RoundToInt(baseStat * multiplier) + flatBonus;
        }
    }

    // 4. 피격한 대상 수 기반 
    [Serializable]
    public class CountScaledAmount : Amount
    {
        [SerializeField] int perTarget = 1;
        [SerializeField] int flatBonus;

        public override int Resolve(EffectContext context, Token target)
            => context.Targets.Count * perTarget + flatBonus;
    }
}
