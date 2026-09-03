using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = ("Effect/DamageSO"))]
public class DamageSO : EffectSO
{
    public int multiplier;

    public override UniTask Apply(BaseObject caster, TriggeredEffect binding, EffectContext context)
    {
        TokenManager tokenManager = ServiceLocator.Get<TokenManager>();
        DamageManager damageManager = ServiceLocator.Get<DamageManager>();

        if(context.TryGet<List<Vector2Int>>(ContextKey.Position, out var positions))
        {
            foreach(var position in positions)
            {
                if(tokenManager.TryGetTokenFrom(position, out Token target))
                    damageManager.ProcessDamage(caster as Token, target, binding.value);
            }
        }

        return UniTask.CompletedTask;
    }

    public override EffectType GetEffectType() => EffectType.Damage;
}
