using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine;

public struct SummonParameter
{
    public string id;
    public string position;
    public int amount;
    public string owner; 
}

public class SummonEffect : IEffect
{
    EffectData data;
    BaseObject effectOwner;

    SummonParameter summonParam; 

    public SummonEffect(EffectData data, BaseObject baseObject)
    {
        this.data = data;
        this.effectOwner = baseObject;

        summonParam = new SummonParameter
        {
            amount = 1,
            position = "Selected",
            owner = "Self",
        }; 
        
        // Parameter Parsing 

        try
        {
            if (!string.IsNullOrEmpty(data.parameter))
            {
                var p = JsonUtility.FromJson<SummonParameter>(data.parameter);

                if (!string.IsNullOrEmpty(p.id))
                    summonParam.id = p.id;
                if (p.amount > 0)
                    summonParam.amount = p.amount;
                
                if (!string.IsNullOrEmpty(p.position))
                    summonParam.position = p.position;

                if (!string.IsNullOrEmpty(p.owner))
                    summonParam.owner = p.owner; 
            }
        }

        catch (Exception ex)
        {
            Debug.LogWarning("SummonEffect Parsing 실패");
            return; 
        }
    }

    public EffectType EffectType => EffectType.Summon;
    public Trigger Trigger => data.trigger; 

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
            TokenManager tokenManager = ServiceLocator.Get<TokenManager>();

            string cardID = null;
            // 적 유닛을 생성하는 것이 있다면 추후 이 부분 수정 필요 
            Debug.Log($"발동하려는 이 SummonEffect의 Owner는 {effectOwner}입니다."); 
            Vector2Int targetPosition = -Vector2Int.one;

            if (summonParam.position == "Destroyed")
            {
                if (!context.TryGet<Vector2Int>(ContextKey.DefenderPos, out Vector2Int pos))
                    Debug.Log("Destroyed Position을 찾을 수 없습니다.");
                else 
                    targetPosition = pos; 
            }

            else if (summonParam.position == "Selected")
            {
                if (!context.TryGet<Vector2Int>(ContextKey.DefenderPos, out Vector2Int pos))
                    Debug.Log("Selected Position을 찾을 수 없습니다.");
                else
                    targetPosition = pos;
            }

            if (summonParam.id == "Source")
            {
                if (!context.TryGet<BaseObject>(ContextKey.Defender, out BaseObject obj))
                    Debug.Log($"{obj}의 Source를 찾을 수 없습니다.");
                else
                    summonParam.id = obj.Data.ID; 

            }
            else
            {
                if (!string.IsNullOrEmpty(summonParam.id))
                {
                    cardID = summonParam.id;
                    Debug.Log(summonParam.id); 
                }
            }

            if (cardID == null)
            {
                Debug.LogError("SummonEffect: CardID를 찾을 수 없습니다.");
                return;
            }

            CardData summonData = database.GetCardData<CardData>(cardID);
            await summonSystem.Summon(effectOwner.OwnerID, summonData, targetPosition, effectOwner.Data);
        });

        return events; 
    }
}
