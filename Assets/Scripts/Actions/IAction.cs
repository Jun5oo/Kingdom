using Cysharp.Threading.Tasks;
using System;
using UnityEngine;

public interface IAction
{
    // 오브젝트 행동 인터페이스 
    public int OwnerID { get; }
    public BaseObject BaseObject { get; }

    public ActionType ActionType { get; }
    public Predicate<Vector2Int> Validation { get; }
    public ResourceType ResourceType { get; }

    public void Enter();
    public void Exit();
    public UniTask Execute(Vector2Int targetPosition); 
    public bool IsValid();

    public event Action OnActionCanceled;
    public event Action OnActionComplete; 

    public int Cost { get; }
}
