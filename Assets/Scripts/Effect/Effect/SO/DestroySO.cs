using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = ("Effect/DestroySO"))]
public class DestroySO : EffectSO
{
    public override UniTask Apply(BaseObject caster, TriggeredEffect binding, EffectContext context)
    {
        TokenManager tokenManager = ServiceLocator.Get<TokenManager>();

        if(context.TryGet<List<Vector2Int>>(ContextKey.Position, out List<Vector2Int> result))
        {
            for(int i=0; i<result.Count; i++)
            {
                if(tokenManager.TryGetTokenFrom(result[i], out Token token))
                {
                    context.Set<ObjectContext>(ContextKey.Death, new ObjectContext
                    {
                        baseObject = token,
                        gridPosition = result[i],
                        objectData = token.Data,
                        ownerID = token.OwnerID,
                        parentData = token.ParentData,
                        sourceData = token.SourceData
                    });

                    // 디버깅
                    Debug.Log($"Destroying: {token.name}");
                    Debug.Log($"Token.Data: {token.Data?.name}");
                    Debug.Log($"Token.SourceData: {token.SourceData?.Count} items");
                    if (token.SourceData != null && token.SourceData.Count > 0)
                    {
                        Debug.Log($"First SourceData: {token.SourceData[0]?.name}");
                    }

                    tokenManager.DestroyToken(token);
                }

            }
        }

        return UniTask.CompletedTask; 
    }

    public override EffectType GetEffectType() => EffectType.Destroy;
}

