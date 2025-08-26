using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine;

public class DestroyEffect : IEffect, IRequireSelection
{
    EffectData effectData;
    BaseObject effectOwner;

    public EffectType EffectType => effectData.effectType; 
    public Trigger Trigger => effectData.trigger;

    public EffectData EffectData => effectData; 

    public DestroyEffect(EffectData effectData, BaseObject baseObject)
    {
        this.effectData = effectData;
        this.effectOwner = baseObject;
    }

    public List<Func<UniTask>> ToEvents(BaseObject owner, EffectContext context)
    {
        var events = new List<Func<UniTask>>();

        events.Add(async () =>
        {
            Vector2Int targetPosition = -Vector2Int.one;
            
            try
            {
                switch (effectData.position)
                {
                    case "None":
                        break;
                    case "Selected":
                        if(context.TryGet<Vector2Int>(ContextKey.Selected, out Vector2Int pos))
                            targetPosition = pos;
                        break;
                    case "AllyPos":
                        if(context.TryGet<Vector2Int>(ContextKey.AllyPos, out Vector2Int allyPos))
                            targetPosition = allyPos;
                        break;
                    case "EnemyPos":
                        if(context.TryGet<Vector2Int>(ContextKey.EnemyPos, out Vector2Int enemyPos))
                            targetPosition = enemyPos;
                        break;
                    default:
                        Debug.LogError("이 외의 Context Position Key는 구현되지 않았습니다.");
                        break; 
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"{effectOwner}의 DestroyEffect: Try문에서 문제가 생겼습니다.");
                return; 
            }

            if(targetPosition.x < 0 || targetPosition.y < 0)
            {
                Debug.LogError($"{effectOwner}의 DestroyEffect: TargetPosition이 잘못됬습니다.");
                return; 
            }

            TokenManager tokenManager = ServiceLocator.Get<TokenManager>(); 

            if(!tokenManager.TryGetTokenFrom(targetPosition, out Token targetToken))
            {
                Debug.LogError("해당 위치에 토큰이 존재하지 않아 파괴할 수 없습니다.");
                return; 
            }

            tokenManager.DestroyToken(targetToken); 
        });

        return events; 
    }

    public Predicate<Vector2Int> GetValidation(BaseObject owner, EffectContext context)
    {
        throw new NotImplementedException();
    }
}
