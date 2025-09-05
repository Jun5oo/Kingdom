using Cysharp.Threading.Tasks;
using System;
using UnityEngine;

public class AbilityAction : IAction
{
    public int OwnerID => actionOwner.OwnerID; 

    BaseObject actionOwner; 
    public BaseObject BaseObject => actionOwner;

    public ActionType ActionType => ActionType.Summon;

    public ActionPerformer Performer => ActionPerformer.Player; 

    public Predicate<Vector2Int> Validation => throw new NotImplementedException();

    public ResourceType ResourceType => ResourceType.Ability;

    public int Cost => 0; //ability.Effects[0].EffectData.cost; 

    Ability ability; 

    public event Action OnActionCanceled;
    public event Action OnActionComplete;

    public AbilityAction(Ability ability, BaseObject actionOwner)
    {
        this.actionOwner = actionOwner;
        this.ability = ability; 
    }

    public void Enter()
    {
        Debug.Log("AbilityAction Enter"); 
    }

    public UniTask Execute(Vector2Int targetPosition)
    {
        Debug.Log("AbilityAction을 실행합니다.");
        // foreach(ability.effect.ExecuteAsync) 
        return UniTask.CompletedTask; 
    }

    public void Exit()
    {
        Debug.Log("Exit"); 
    }

    public bool IsValid()
    {
        return true; 
        // Ability의 Trigger가 Active가 아니면 false? 
    }
}
