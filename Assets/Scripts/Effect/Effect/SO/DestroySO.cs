using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = ("Effect/DestroySO"))]
public class DestroySO : EffectSO
{
    public override UniTask Apply(BaseObject caster, TriggerBinding binding, EffectContext context)
    {
        TokenManager tokenManager = ServiceLocator.Get<TokenManager>();

        if(context.TryGet<List<Vector2Int>>(ContextKey.Position, out List<Vector2Int> result))
        {
            for(int i=0; i<result.Count; i++)
            {
                if(tokenManager.TryGetTokenFrom(result[i], out Token token))
                    tokenManager.DestroyToken(token); 
            }
        }

        return UniTask.CompletedTask; 
    }

    public override EffectType GetEffectType() => EffectType.Destroy;
}

