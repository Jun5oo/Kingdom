using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine;

public class Ability
{
    BaseObject abilityOwner;
    AbilitySO abilityData;

    HashSet<Trigger> triggerSet;

    bool isSubscribed;

    public Ability(BaseObject baseObject, AbilitySO abilityData)
    {
        this.abilityOwner = baseObject;
        this.abilityData = abilityData;

        triggerSet = new HashSet<Trigger>();

        isSubscribed = false; 
        Subscribe(); 
    }

    public void Subscribe()
    {
        if (isSubscribed)
            return; 

        foreach(var binding in abilityData.bindings)
        {
            switch (binding.trigger)
            {
                case Trigger.OnTurnStarted:
                    TrySubScribe(binding.trigger, () => { EventBus<TurnStartEvent>.Subscribe(TurnStart); Debug.Log("OnTurnStarted Event가 연결되었습니다."); });
                    break;
                case Trigger.OnTurnEnded:
                    TrySubScribe(binding.trigger, () => { EventBus<TurnEndEvent>.Subscribe(TurnEnd); Debug.Log("OnTurnEnded Event가 연결되었습니다."); });
                    break;
                case Trigger.OnAllyDead:
                case Trigger.OnEnemyDead:
                case Trigger.OnUnitDead:
                    TrySubScribe(binding.trigger, () => { EventBus<UnitDeadEvent>.Subscribe(UnitDead); Debug.Log("UnitDead Event가 연결되었습니다."); });
                    break;
            }
        }

    }
    public void Unsubscribe()
    {
        if (!isSubscribed)
            return; 

        if(triggerSet.Contains(Trigger.OnTurnStarted))
            EventBus<TurnStartEvent>.Unsubscribe(TurnStart);

        if(triggerSet.Contains(Trigger.OnTurnEnded))
            EventBus<TurnEndEvent>.Unsubscribe(TurnEnd);

        if (triggerSet.Contains(Trigger.OnUnitDead))
            EventBus<UnitDeadEvent>.Unsubscribe(UnitDead);

        isSubscribed = false; 
    }

    void TrySubScribe(Trigger trigger, Action action)
    {
        if (triggerSet.Add(trigger))
            action?.Invoke(); 
    }

    #region Event 
    void TurnStart(TurnStartEvent eventData)
    {
        RunBindings(Trigger.OnTurnStarted, eventData).Forget(); 
    }
    void TurnEnd(TurnEndEvent eventData)
    {
        RunBindings(Trigger.OnTurnEnded, eventData).Forget();

    }
    void UnitDead(UnitDeadEvent eventData)
    {
        RunBindings(Trigger.OnUnitDead, eventData).Forget();
        RunBindings(Trigger.OnEnemyDead, eventData).Forget();
        RunBindings(Trigger.OnAllyDead, eventData).Forget(); 
    }
    #endregion 

    async UniTask RunBindings(Trigger trigger, IGameEvent eventData)
    {
        TargetResolver resolver = ServiceLocator.Get<TargetResolver>();
        EventQueue eventQueue = ServiceLocator.Get<EventQueue>();

        foreach (var binding in abilityData.bindings)
        {
            if (binding.trigger != trigger)
                continue;

            var context = new EffectContext();
            FillContextFromEvent(context, eventData); 

            bool isAllConditionSatisfied = true; 

            // TriggerCondition 확인 
            foreach(var condition in binding.conditions)
            {
                if (!condition.IsTriggerConditionSatisfied(abilityData, abilityOwner, context))
                {
                    isAllConditionSatisfied = false;
                    break; 
                }
            }

            if (!isAllConditionSatisfied)
                continue;

            List<Vector2Int> candidates = await resolver.TryResolve(abilityOwner, binding.target, binding.targetConditions, binding.filters, context);
            Debug.Log(candidates); 

            if (candidates.Count == 0 || candidates == null)
                context.Set(ContextKey.Positions, candidates);

            foreach (var effect in binding.effects)
                await effect.Apply(abilityOwner, context);

            await eventQueue.ExecuteAllAsync(); 
        }
    }

    void FillContextFromEvent(EffectContext context, IGameEvent eventData)
    {
        switch (eventData)
        {
            case TurnStartEvent turnStart:
                context.Set<int>(ContextKey.PlayerID, turnStart.playerID);
                break;

            case TurnEndEvent turnEnd:
                context.Set<int>(ContextKey.PlayerID, turnEnd.playerID);
                break;

            case UnitDeadEvent dead:

                if(dead.killer != null)
                    context.Set<BaseObject>(ContextKey.KillerObject, dead.killer);

                context.Set<Vector2Int>(ContextKey.KillerPosition, dead.killerPosition);
                context.Set<CardData>(ContextKey.KillerData, dead.killer?.Data);
                context.Set<int>(ContextKey.KillerOwnerID, dead.killerOwnerID);

                if(dead.victim != null )
                    context.Set<BaseObject>(ContextKey.VictimObject, dead.victim);

                context.Set<int>(ContextKey.VictimOwnerID, dead.victimOwnerID);
                context.Set<CardData>(ContextKey.VictimData, dead.victim?.Data);
                context.Set<Vector2Int>(ContextKey.VictimPosition, dead.victimPosition);
                
                if(dead.victimSources.Count != 0 && dead.victimSources != null)
                    context.Set<CardData>(ContextKey.SourceData, dead.victimSources[0]); 

                break;
        
        }
    }
    public void Clear()
    {
        Unsubscribe(); 
    }
}
