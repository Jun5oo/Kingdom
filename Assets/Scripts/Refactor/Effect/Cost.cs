using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Refactor
{
    // 발동 비용. IsCostEnough로 지불 가능 여부 판정, Pay로 실제 차감.
    // 호출 순서: IsCostEnough 전부 통과 후, Target/Operation 진행 직전 지불.
    // plain 클래스 + SerializeReference로 EffectData 안에 인라인 저장된다.
    // 구체 타입에는 [Serializable] 필수.

    [Serializable]
    public abstract class Cost
    {
        public abstract bool IsCostEnough(EffectContext context);
        public abstract void Pay(EffectContext context);
    }

    // 나열된 비용을 모두 지불
    [Serializable]
    public class AllCost : Cost
    {
        [SerializeReference, SubclassSelector] List<Cost> costs = new();

        public override bool IsCostEnough(EffectContext context)
            => costs.All(c => c == null || c.IsCostEnough(context));

        public override void Pay(EffectContext context)
        {
            foreach (var c in costs)
                c?.Pay(context);
        }
    }

    [Serializable]
    public class ResourceCost : Cost
    {
        [SerializeField] ResourceType resourceType = ResourceType.Action;
        [SerializeField, Min(0)] int amount = 1;

        public override bool IsCostEnough(EffectContext context)
            => GetSystem().IsEnoughResources(context.CasterOwnerId, amount);

        public override void Pay(EffectContext context)
            => GetSystem().Consume(context.CasterOwnerId, amount);

        IResourceSystem GetSystem()
        {
            switch (resourceType)
            {
                case ResourceType.Ability: return ServiceLocator.Get<AbilityResourceSystem>();
                default:                   return ServiceLocator.Get<ActionResourceSystem>();
            }
        }
    }
}
