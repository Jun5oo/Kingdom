using Cysharp.Threading.Tasks;
using System;
using UnityEngine;

public class AbilityAction : IAction
{
    public int OwnerID => throw new NotImplementedException();

    public BaseObject Executor => throw new NotImplementedException();

    public ActionType ActionType => throw new NotImplementedException();

    public HighlightLayer HighlightLayer => throw new NotImplementedException();

    public HighlightType HighlightType => throw new NotImplementedException();

    public ActionPerformer Performer => throw new NotImplementedException();

    public Predicate<Vector2Int> Validation => throw new NotImplementedException();

    public ResourceType ResourceType => throw new NotImplementedException();

    public int Cost => throw new NotImplementedException();

    public event Action OnActionCanceled;
    public event Action OnActionComplete;

    public AbilityAction(Ability ability, BaseObject actionOwner)
    {

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
