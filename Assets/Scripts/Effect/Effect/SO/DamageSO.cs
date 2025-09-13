using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

[CreateAssetMenu(menuName = ("Effect/DamageSO"))]
public class DamageSO : EffectSO
{
    public int multiplier;

    public override async UniTask Apply(BaseObject caster, TriggerBinding binding, EffectContext context)
    {
        EventQueue queue = ServiceLocator.Get<EventQueue>();
        TokenManager tokenManager = ServiceLocator.Get<TokenManager>();
        DamageManager damageManager = ServiceLocator.Get<DamageManager>(); 
        
        if(!context.TryGet<List<Vector2Int>>(ContextKey.Position, out var positions))
        {
            foreach(var position in positions)
            {
                if(tokenManager.TryGetTokenFrom(position, out Token target))
                {
                    queue.Enqueue(async () =>
                    {
                        damageManager.ProcessDamage(caster as Token, target, binding.value);
                    });
                }
            }
        }
    }

    public override EffectType GetEffectType() => EffectType.Damage; 
}
