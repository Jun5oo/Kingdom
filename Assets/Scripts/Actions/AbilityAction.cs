using Cysharp.Threading.Tasks;
using System;
using UnityEngine;

public class AbilityAction : IAction
{
    public int OwnerID => caster.OwnerID; 

    BaseObject caster; 
    public BaseObject BaseObject => caster;

    public ActionType ActionType => ActionType.Summon;

    public ActionPerformer Performer => ActionPerformer.Player;

    public ResourceType ResourceType => ResourceType.Ability;

    public int Cost => 0;
    public Predicate<Vector2Int> Validation => throw new NotImplementedException();

    Ability ability; 

    public event Action OnActionCanceled;
    public event Action OnActionComplete;

    public AbilityAction(Ability ability, BaseObject caster)
    {
        this.caster = caster;
        this.ability = ability; 
    }

    public void Enter()
    {
        Debug.Log("AbilityAction Enter"); 
    }

    public async UniTask Execute(Vector2Int targetPosition)
    {
        Debug.Log("AbilityAction을 실행합니다.");
        await ability.RunBindings(Trigger.Active, null); 
    }

    public void Exit()
    {
        Debug.Log("Exit"); 
    }

    public bool IsValid()
    {
        return true; 
    }
}
