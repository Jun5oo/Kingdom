using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;

public class DestroyEffect : IEffect
{
    EffectData effectData;
    BaseObject baseObject;

    Trigger trigger;
    Target target;

    SelectionMode selectionMode; 

    int value;

    public EffectType EffectType => effectData.effectType; 
    public Trigger Trigger => effectData.trigger;

    public DestroyEffect(EffectData effectData, BaseObject baseObject)
    {
        this.effectData = effectData;
        this.target = effectData.target; 
        this.value = effectData.value;

        this.baseObject = baseObject;
    }
    public List<Func<UniTask>> ToEvents(BaseObject owner, EffectContext context)
    {
        var events = new List<Func<UniTask>>();

        /*

        if (candidates == null || candidates.Count == 0)
            return events;

        */ 

        events.Add(async () =>
        {

            await UniTask.CompletedTask; 
        });

        return events; 
    }

    List<Token> ResolveCandidates(BaseObject abilityOwner, Target target)
    {
        var list = new List<Token>();

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

        return list; 
    }
}
