using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

public class Ability
{
    BaseObject abilityOwner; 

    Trigger trigger;
    int groupID; 
    List<IEffect> effects;

    EventQueue eventQueue; 

    public Trigger Trigger => trigger;
    public int GroupID => groupID; 
    public List<IEffect> Effects => effects;

    public Ability(Trigger trigger, int groupID, List<IEffect> effects, BaseObject baseObject)
    {
        this.trigger = trigger;
        this.groupID = groupID;

        this.effects = effects; 

        this.abilityOwner = baseObject;

        eventQueue = ServiceLocator.Get<EventQueue>(); 
        Subscribe(); 
    }

    public void Subscribe()
    {
        switch (trigger)
        {
            case Trigger.OnTurnStarted:
                EventBus<TurnStartEvent>.Subscribe(TurnStart); 
                break; 
            case Trigger.OnTurnEnded:
                EventBus<TurnEndEvent>.Subscribe(TurnEnd); 
                break;
            case Trigger.OnAllyDead:
                EventBus<UnitDeadEvent>.Subscribe(AllyDead);
                break; 
            case Trigger.OnEnemyDead:
                EventBus<UnitDeadEvent>.Subscribe(EnemyDead);
                break; 
        }
    }
    public void Unsubscribe()
    {
        switch (trigger)
        {
            case Trigger.OnTurnStarted:
                EventBus<TurnStartEvent>.Unsubscribe(TurnStart);
                break;
            case Trigger.OnTurnEnded:
                EventBus<TurnEndEvent>.Unsubscribe(TurnEnd);
                break;
            case Trigger.OnAllyDead:
                EventBus<UnitDeadEvent>.Unsubscribe(AllyDead);
                break;
            case Trigger.OnEnemyDead:
                EventBus<UnitDeadEvent>.Unsubscribe(EnemyDead);
                break;
        }
    }

    #region Event 
    void TurnStart(TurnStartEvent eventData)
    {
        if (eventData.playerID == abilityOwner.OwnerID)
        {
            EffectContext context = new EffectContext();

            Execute(context);
        }

        Debug.Log($"{this} TurnStart 실행? "); 
    }
    void TurnEnd(TurnEndEvent eventData)
    {
        if (eventData.playerID == abilityOwner.OwnerID)
        {
            EffectContext context = new EffectContext();
            Execute(context);
        }

        Debug.Log($"{this} TurnEnd 실행? ");
    }
    void AllyDead(UnitDeadEvent eventData)
    {
        if(eventData.victim.OwnerID == abilityOwner.OwnerID)
        {
            EffectContext context = new EffectContext
            {
                // targetPosition = eventData.victimPosition
            };
            
            Execute(context);
        }
    }
    void EnemyDead(UnitDeadEvent eventData)
    {
        if (eventData.victim.OwnerID != abilityOwner.OwnerID)
        {
            EffectContext context = new EffectContext();
            Execute(context);
        }

        Debug.Log($"{this} EnemyDead 실행? ");
    }
    #endregion 

    public async UniTask Execute(EffectContext context)
    {
        foreach(var effect in effects)
        {
            foreach(var _event in effect.ToEvents(abilityOwner, context))
                eventQueue.Enqueue(_event); 
        }

        await eventQueue.ExecuteAllAsync();
    }

    void Clear()
    {
        Unsubscribe(); 
    }
}
