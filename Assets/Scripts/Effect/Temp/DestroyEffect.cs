using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine;

public class DestroyEffect : IEffect, IRequireSelection
{
    EffectData effectData;
    BaseObject baseObject;

    Trigger trigger;
    Target target;

    public EffectType EffectType => effectData.effectType; 
    public Trigger Trigger => effectData.trigger;

    public DestroyEffect(EffectData effectData, BaseObject baseObject)
    {
        this.effectData = effectData;
        this.target = effectData.target; 

        this.baseObject = baseObject;
    }
    public List<Func<UniTask>> ToEvents(BaseObject owner, EffectContext context)
    {
        var events = new List<Func<UniTask>>();

        events.Add(async () =>
        {

        });

        return events; 
    }

    List<Vector2Int> ResolveCandidates(BaseObject abilityOwner, Target target)
    {
        var list = new List<Token>();
        var pos = new List<Vector2Int>(); 

        switch (target)
        {
            case Target.Self:
                if (abilityOwner is Token self)
                    list.Add(self);
                break;
            case Target.Enemy:
            case Target.AllEnemies:
                {
                    var resolver = ServiceLocator.Get<TargetResolver>();
                    var targetContext = new TargetContext { target = target };
                    // var resolve = resolver.Resolve(abilityOwner, targetContext);
                    break; 
                }
            case Target.Ally:
                {
                    var resolver = ServiceLocator.Get<TargetResolver>();
                    var targetContext = new TargetContext { target = target };
                    // var resolve = resolver.Resolve(abilityOwner, targetContext);
                    break; 
                }
        }

        foreach(var token in list)
        {
            TokenManager tokenManager = ServiceLocator.Get<TokenManager>();
            if(token != null)
                pos.Add(tokenManager.GetGridPositionOfToken(token));
        }

        return pos; 
    }

    public Predicate<Vector2Int> GetValidation(BaseObject owner, EffectContext context)
    {
        throw new NotImplementedException();
    }
}
