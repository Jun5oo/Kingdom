using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = ("Effect/DestroySO"))]
public class DestroySO : EffectSO
{
    [SerializeField] int amount;

    public override UniTask Apply(BaseObject caster, EffectContext context)
    {
        TokenManager tokenManager = ServiceLocator.Get<TokenManager>();

        if(context.TryGet<List<Vector2Int>>(ContextKey.Positions, out List<Vector2Int> result))
        {
            int min = Mathf.Min(result.Count, amount);

            for(int i=0; i<min; i++)
            {
                if(tokenManager.TryGetTokenFrom(result[i], out Token token))
                    tokenManager.DestroyToken(token); 
            }
        }

        return UniTask.CompletedTask; 
    }

    public override EffectType GetEffectType() => EffectType.Destroy;
}

