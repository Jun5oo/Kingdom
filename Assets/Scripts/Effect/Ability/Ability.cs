using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine;

public class Ability
{
    BaseObject abilityOwner;
    AbilitySO abilityData;

    HashSet<Trigger> triggers;

    bool isSubscribed;

    public Ability(BaseObject baseObject, AbilitySO abilityData)
    {
        this.abilityOwner = baseObject;
        this.abilityData = abilityData;

        triggers = new HashSet<Trigger>();

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

        if(triggers.Contains(Trigger.OnTurnStarted))
            EventBus<TurnStartEvent>.Unsubscribe(TurnStart);

        if(triggers.Contains(Trigger.OnTurnEnded))
            EventBus<TurnEndEvent>.Unsubscribe(TurnEnd);

        if (triggers.Contains(Trigger.OnUnitDead))
            EventBus<UnitDeadEvent>.Unsubscribe(UnitDead);

        isSubscribed = false; 
    }

    void TrySubScribe(Trigger trigger, Action action)
    {
        if (triggers.Add(trigger))
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
    }
    #endregion 

    public async UniTask RunBindings(Trigger trigger, IGameEvent eventData)
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
            foreach(var condition in binding.triggerConditions)
            {
                if (!condition.IsTriggerConditionSatisfied(abilityData, abilityOwner, context))
                {
                    isAllConditionSatisfied = false;
                    break; 
                }

                if(!condition.IsTriggerConditionSatisfied(abilityOwner, context))
                {
                    isAllConditionSatisfied = false;
                    break; 
                }
            }

            if (!isAllConditionSatisfied)
                continue;

            List<Vector2Int> candidates = await resolver.TryResolve(abilityOwner, binding.target, binding.targetConditions, binding.filters, context);

            if (candidates != null)
            {
                context.Set<List<Vector2Int>>(ContextKey.Position, candidates);
                Debug.Log($"{candidates.Count}");
            }

            foreach (var effect in binding.effects)
                await effect.Apply(abilityOwner, binding, context);

            await eventQueue.ExecuteAllAsync();

            if (binding.chainAbilities != null && binding.chainAbilities.Count > 0)
                await ExecuteChainAbilities(binding.chainAbilities, context); 
        }
    }

    async UniTask ExecuteChainAbilities(List<TriggerBinding> chainAbilities, EffectContext parentContext)
    {
        TargetResolver resolver = ServiceLocator.Get<TargetResolver>();
        EventQueue eventQueue = ServiceLocator.Get<EventQueue>();

        foreach (var chainBinding in chainAbilities)
        {
            var chainContext = parentContext; 

            bool isAllConditionSatisfied = true;

            foreach (var condition in chainBinding.triggerConditions)
            {
                if (!condition.IsTriggerConditionSatisfied(abilityData, abilityOwner, chainContext))
                {
                    isAllConditionSatisfied = false;
                    break;
                }
            }

            if (!isAllConditionSatisfied)
                continue;

            List<Vector2Int> chainCandidates = await resolver.TryResolve(abilityOwner, chainBinding.target, chainBinding.targetConditions, chainBinding.filters, chainContext); 

            if (chainCandidates != null)
                chainContext.Set(ContextKey.Position, chainCandidates);

            foreach (var effect in chainBinding.effects)
                await effect.Apply(abilityOwner, chainBinding, chainContext);

            await eventQueue.ExecuteAllAsync();

            if (chainBinding.chainAbilities != null && chainBinding.chainAbilities.Count > 0)
                await ExecuteChainAbilities(chainBinding.chainAbilities, chainContext);
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
                context.Set<ObjectContext>(ContextKey.Kill, dead.killer);
                context.Set<ObjectContext>(ContextKey.Death, dead.victim); 
                break;
        }
    }


    public void Clear()
    {
        Unsubscribe(); 
    }
}
