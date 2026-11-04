using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Refactor
{
    // "이 효과를 지금 발동해도 되는가?" 만 판정한다.
    // 대상이 유효한지(소유자/태그/빈 칸 등)는 Target 슬롯이 담당한다.
    // plain 클래스 + SerializeReference로 EffectData 안에 인라인 저장된다.
    // 구체 타입에는 [Serializable] 필수.
    [Serializable]
    public abstract class Condition
    {
        public abstract bool CanExecute(EffectContext context);
    }

    // ── 복합 조건: SerializeReference 중첩으로 트리 구성 ──

    // 복수 조건
    [Serializable]
    public class AllCondition : Condition
    {
        [SerializeReference, SubclassSelector] List<Condition> conditions = new();

        public override bool CanExecute(EffectContext context)
            => conditions.All(c => c == null || c.CanExecute(context));
    }

    // 하나라도 만족하면 됨
    [Serializable]
    public class AnyCondition : Condition
    {
        [SerializeReference, SubclassSelector] List<Condition> conditions = new();

        public override bool CanExecute(EffectContext context)
            => conditions.Count == 0 || conditions.Any(c => c != null && c.CanExecute(context));
    }

    // 반전
    [Serializable]
    public class NotCondition : Condition
    {
        [SerializeReference, SubclassSelector] Condition inner;

        public override bool CanExecute(EffectContext context)
            => inner == null || !inner.CanExecute(context);
    }
}
