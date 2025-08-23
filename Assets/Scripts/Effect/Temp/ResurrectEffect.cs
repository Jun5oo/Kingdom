using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine;

public struct ResurrectParameter
{
    public string tag;
    public string race;

    public int amount;

    public string owner;

    public string position; 
}

public class ResurrectEffect : IEffect
{
    EffectData data;
    BaseObject owner; 

    public EffectType EffectType => data.effectType; 
    public Trigger Trigger => data.trigger; 


    public ResurrectEffect(EffectData effectData, BaseObject owner)
    {
        this.data = effectData;
        this.owner = owner; 
    }

    public List<Func<UniTask>> ToEvents(BaseObject owner, EffectContext context)
    {
        var events = new List<Func<UniTask>>();

        ResurrectParameter parameter = new ResurrectParameter(); 
        
        try
        {
            if (!string.IsNullOrEmpty(data.parameter))
                parameter = JsonUtility.FromJson<ResurrectParameter>(data.parameter); 
        }
        catch(Exception ex) 
        {
            Debug.LogError($"ResurrectEffect : parameter parse Error {data.parameter}"); 
        }

        events.Add(async () =>
        {
            var eventQueue = ServiceLocator.Get<EventQueue>(); 
            var tokenManager = ServiceLocator.Get<TokenManager>();
            var summonSystem = ServiceLocator.Get<SummonSystem>();
            var database = ServiceLocator.Get<CardDatabase>(); 

        });

        return events;
    }
}

