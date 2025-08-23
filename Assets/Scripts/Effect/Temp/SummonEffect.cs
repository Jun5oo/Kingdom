using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine;

public class SummonEffect : IEffect
{
    EffectData data;
    BaseObject effectOwner; 

    public SummonEffect(EffectData data, BaseObject baseObject)
    {
        this.data = data;
        this.effectOwner = baseObject; 
    }

    public EffectType EffectType => EffectType.Summon;
    public Trigger Trigger => data.trigger; 

    public UniTask ExecuteAsync(EffectContext context)
    {
        Vector2Int targetPosition = Vector2Int.zero; 

        CardDatabase database = ServiceLocator.Get<CardDatabase>(); 
        SummonSystem summonSystem = ServiceLocator.Get<SummonSystem>();
        // 데이터베이스에서 Summon할 카드의 데이터를 찾고 cardData를 받은 후에 

        // CardData summonData = database.GetCardData<CardData>(data.parameter1);
    
        EventQueue eventQueue = ServiceLocator.Get<EventQueue>();

        eventQueue.Enqueue(async () =>
        {
            // await summonSystem.Summon(effectOwner.OwnerID, summonData, targetPosition, effectOwner.Data);
        });

        return UniTask.CompletedTask; 
    }

    public bool IsValid()
    {
        return true; 
    }

    public List<Func<UniTask>> ToEvents(BaseObject owner, EffectContext context)
    {
        var events = new List<Func<UniTask>>();

        events.Add(async () =>
        {
            if (!context.TryGet(ContextKey.Selected, out Vector2Int pos))
                return;

            var summonSystem = ServiceLocator.Get<SummonSystem>();
            var database = ServiceLocator.Get<CardDatabase>();

            /*
            switch (place)
            {
                case ContextKey.Selected:
                    if (context.TryGet(place, out Vector2Int _pos)
                        await summonSystem.Summon(effectOwner.OwnerID, parameter2, _pos, effectOwner.Data, null);
                    break;
                case ContextKey.LastDestroyed:
                    if (context.TryGet(place, out Vector2Int _pos)
                        await summonSystem.Summon(effectOwner.OwnerID, parameter2, _pos, effectOwner.Data, null);
                    break; 
            }
            */ 

            // parameter1 : summonPlace; 
            // parameter2 : summonData; 

        });

        return events; 
    }
}
