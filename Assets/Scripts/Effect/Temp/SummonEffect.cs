using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine;

public struct SummonParameter
{
    public string id;
    public string position;
}

public class SummonEffect : IEffect
{
    EffectData effectData;
    BaseObject effectOwner;

    SummonParameter summonParam; 

    public SummonEffect(EffectData data, BaseObject baseObject)
    {
        this.effectData = data;
        this.effectOwner = baseObject;

        summonParam = new SummonParameter
        {
            id = string.Empty, 
            position = "Selected",
        }; 
    }

    public EffectType EffectType => EffectType.Summon;
    public Trigger Trigger => effectData.trigger;

    public EffectData EffectData => effectData;

    public bool IsValid()
    {
        return true; 
    }

    public List<Func<UniTask>> ToEvents(BaseObject owner, EffectContext context)
    {
        var events = new List<Func<UniTask>>();

        events.Add(async () =>
        {
            CardDatabase database = ServiceLocator.Get<CardDatabase>();
            SummonSystem summonSystem = ServiceLocator.Get<SummonSystem>();

            string cardID = string.Empty;
            Vector2Int targetPosition = -Vector2Int.one;

            // Parse Reward
            try
            {
                if (!string.IsNullOrEmpty(effectData.reward))
                {
                    if (database.GetCardData<CardData>(effectData.reward) != null)
                        cardID = effectData.reward; 
                    else
                    {
                        switch (effectData.reward)
                        {
                            case "AllySource":
                                if (context.TryGet<List<CardData>>(ContextKey.Ally, out List<CardData> allySources))
                                {
                                    if(allySources != null && allySources.Count > 0)
                                        summonParam.id = allySources[0].ID;
                                }
                                break;

                            case "EnemySource":
                                if (context.TryGet<List<CardData>>(ContextKey.EnemySource, out List<CardData> enemySources))
                                {
                                    if(enemySources != null && enemySources.Count > 0)
                                        summonParam.id = enemySources[0].ID;
                                }

                                break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogWarning("SummonEffect Parsing 실패");
                return;
            }

            // Parse Position 
            try
            {
                if (!string.IsNullOrEmpty(effectData.position))
                {
                    switch (effectData.position)
                    {
                        case "Selected":
                            if (context.TryGet<Vector2Int>(ContextKey.Selected, out Vector2Int pos))
                                targetPosition = pos;
                            break;
                        case "AllyPos":
                            if (context.TryGet<Vector2Int>(ContextKey.AllyPos, out Vector2Int allyPos))
                                targetPosition = allyPos;
                            break;
                        case "EnemyPos":
                            if (context.TryGet<Vector2Int>(ContextKey.EnemyPos, out Vector2Int enemyPos))
                                targetPosition = enemyPos;
                            break;
                        default:
                            Debug.LogError($"{effectOwner}의 SummonEffect: {effectData.position}을 찾을 수 없습니다.");
                            return; 
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogWarning("SummonEffect Parsing 실패");
                return;
            }
    
            if (cardID == string.Empty)
            {
                Debug.LogError($"{effectOwner}의 SummonEffect: 소환하려는 CardID를 찾을 수 없습니다.");
                return; 
            }

            if (targetPosition.x < 0 || targetPosition.y < 0)
            {
                Debug.LogError($"{effectOwner}의 SummonEffect: TargetPosition이 잘못되었습니다.");
                return;
            }

            CardData summonData = database.GetCardData<CardData>(cardID);

            if (summonData == null)
            {
                Debug.LogError($"{effectOwner}의 SummonEffect: {cardID}의 summonData를 찾을 수 없습니다. "); 
            }

            await summonSystem.Summon(effectOwner.OwnerID, summonData, targetPosition, effectOwner.Data);
        });

        return events; 
    }
}
