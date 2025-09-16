using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Ability
{
    #region Data 
    BaseObject caster;
    AbilitySO abilityData;
    public BaseObject Caster => caster;
    public AbilitySO AbilityData => abilityData;
    #endregion

    HashSet<Trigger> triggers;
    bool isSubscribed;

    public Ability(BaseObject baseObject, AbilitySO abilityData)
    {
        this.caster = baseObject;
        this.abilityData = abilityData;

        triggers = new HashSet<Trigger>();

        isSubscribed = false; 
        Subscribe(); 
    }

    #region Subscribe 
    public void Subscribe()
    {
        if (isSubscribed)
            return; 

        foreach(var binding in abilityData.triggeredEffects)
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
    public void Clear()
    {
        Unsubscribe();
    }
    #endregion 

    #region Event 
    void TurnStart(TurnStartEvent eventData)
    {
        ExecutePassive(Trigger.OnTurnStarted, eventData).Forget(); 
    }
    void TurnEnd(TurnEndEvent eventData)
    {
        ExecutePassive(Trigger.OnTurnEnded, eventData).Forget();

    }
    void UnitDead(UnitDeadEvent eventData)
    {
        ExecutePassive(Trigger.OnUnitDead, eventData).Forget();
    }
    #endregion

    #region Conditions 
    public bool IsTriggerConditionSatisfied(TriggeredEffect binding, EffectContext context)
    {
        if (binding.triggerConditions == null || binding.triggerConditions.Count == 0)
            return true; 

        foreach(var condition in binding.triggerConditions)
        {
            if (!condition.IsTriggerConditionSatisfied(caster, context))
                return false; 
        }

        return true; 
    }
    #endregion 

    // 액티브 효과 발동 
    public async UniTask ExecuteActive(Vector2Int targetPosition)
    {
        List<TriggeredEffect> activeEffects = AbilityData.triggeredEffects.Where(e => e.trigger == Trigger.Active).ToList();

        if (!activeEffects.Any())
        {
            Debug.LogWarning($"{AbilityData.abilityName}은 실행시킬 수 있는 Active 스킬이 없습니다.");
            return; 
        }

        var context = new EffectContext(); 
        context.Set(ContextKey.Position, new List<Vector2Int>() { targetPosition });

        foreach(var effect in activeEffects)
        {
            await Execute(effect, context); 
        }
    }
    // 패시브 효과 발동
    public async UniTask ExecutePassive(Trigger trigger, IGameEvent eventData)
    {
        var context = new EffectContext(); 
        FillContextFromEvent(context, eventData); 

        foreach (var effect in AbilityData.triggeredEffects)
        {
            if (effect.trigger != trigger)
                continue;

            await Execute(effect, context); 
        }
    }

    async UniTask Execute(TriggeredEffect binding, EffectContext context)
    {
        if (!CheckTriggerCondition(binding.triggerConditions, context))
            return; 

        if(binding.trigger != Trigger.Active)
        {
            var resolver = ServiceLocator.Get<TargetResolver>();
            var targets = await resolver.TryResolve(caster, binding.target, binding.targetConditions, binding.filters, context);

            if (targets != null && targets.Count > 0)
                context.Set(ContextKey.Position, targets); 
        }

        var eventQueue = ServiceLocator.Get<EventQueue>(); 

        foreach (var effect in binding.effects)
        {
            eventQueue.Enqueue(async () =>
            {
                await effect.Apply(caster, binding, context);
            }); 
        }

        await eventQueue.ExecuteAllAsync(); 

        if(binding.chainAbilities?.Count > 0)
        {
            foreach(var chain in binding.chainAbilities)
            {
                await Execute(chain, context); 
            }
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

    bool CheckTriggerCondition(List<ConditionSO> conditions, EffectContext context)
    {
        if (conditions == null || conditions.Count == 0)
            return true;

        return conditions.All(c => c.IsTriggerConditionSatisfied(caster, context)); 
    }

}
