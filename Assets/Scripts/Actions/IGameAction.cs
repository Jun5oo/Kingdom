using Cysharp.Threading.Tasks;
using System;
using UnityEngine;

public interface IGameAction
{
    // 플레이어가 상호작용 할 수 있는 게임 오브젝트의 행동 (소환, 진화, 업그레이드, 이동) 
    public int OwnerID { get; }
    public BaseObject BaseObject { get; }

    public ActionType ActionType { get; }
    public Predicate<Vector2Int> Validation { get; }
    public ResourceType ResourceType { get; }
    public int Cost { get; }

    public UniTask Execute(Vector2Int targetPosition);
    public void Exit();
    public bool IsValid();

    public event Action OnActionCanceled;
    public event Action OnActionComplete; 

}
