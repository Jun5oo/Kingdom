using Cysharp.Threading.Tasks;
using System;
using System.Linq;
using UnityEngine;

public class AbilityAction : IGameAction
{
    public int OwnerID => caster.OwnerID; 

    BaseObject caster; 
    public BaseObject BaseObject => caster;

    public ActionType ActionType => ActionType.Summon;
    public ResourceType ResourceType => ResourceType.Ability;

    public int Cost => 0;
    public Predicate<Vector2Int> Validation => ValidatePosition;

    Ability ability;
    TriggeredEffect activeEffect; 

    public event Action OnActionCanceled;
    public event Action OnActionComplete;

    public AbilityAction(Ability ability, BaseObject caster)
    {
        this.caster = caster;
        this.ability = ability;

        activeEffect = ability.AbilityData.triggeredEffects.FirstOrDefault(e => e.trigger == Trigger.Active); 
    }

    public async UniTask Execute(Vector2Int targetPosition)
    {
        Debug.Log("AbilityAction을 실행합니다.");
        await ability.ExecuteActive(targetPosition); 
    }

    public void Exit()
    {
        Debug.Log("Exit"); 
    }

    public bool IsValid()
    {
        TargetResolver resolver = ServiceLocator.Get<TargetResolver>();
        var candidates = resolver.GetValidSelectTarget(caster, activeEffect.targetConditions, activeEffect.filters);
        
        if (candidates.Count == 0)
            return false;

        return true; 
    }

    bool ValidatePosition(Vector2Int pos)
    {
        if(activeEffect == null) return false;

        var resolver = ServiceLocator.Get<TargetResolver>();
        var validTargets = resolver.GetValidSelectTarget(caster, activeEffect.targetConditions, activeEffect.filters);
        return validTargets.Contains(pos); 
    }
}
