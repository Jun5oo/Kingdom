using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine;

public class MoveAction : IGameAction
{
    ActionType actionType;
    public ActionType ActionType { get { return actionType; } }

    GridManager gridManager;
    TokenManager tokenManager; 
    Token token;

    public List<Vector2Int> MoveablePositions { get; private set; }

    Vector2Int targetPosition; 

    public event Action OnActionCanceled;
    public event Action OnActionComplete;

    int currentCost;
    public int Cost { get { return currentCost; } }

    public ResourceType resourceType;
    public ResourceType ResourceType { get { return resourceType; } }

    public int OwnerID { get { return token.OwnerID; } }

    public BaseObject BaseObject => token;

    public Predicate<Vector2Int> Validation => CanMoveTo;

    public MoveAction(Token token)
    {
        // 이동액션 초기화 
        actionType = ActionType.Move;

        this.gridManager = ServiceLocator.Get<GridManager>();
        this.tokenManager = ServiceLocator.Get<TokenManager>(); 

        this.token = token;
        this.MoveablePositions = token.MoveableRange;

        currentCost = 1;

        resourceType = ResourceType.Action; 
    }

    public void Enter()
    {

    }
    public async UniTask Execute(Vector2Int gridPosition)
    {
        if(token == null)
        {
            OnActionCanceled?.Invoke();
            return; 
        }

        targetPosition = gridPosition; 

        await Transition(MoveState.Prepare); 
    }
    public void Exit()
    {

    }

    public bool IsValid()
    {
        if(MoveablePositions.Count == 0) 
            return false;

        return true; 
    }

    async UniTask Transition(MoveState state)
    {
        switch (state)
        {
            case MoveState.Prepare:
                await Prepare();
                break;
            case MoveState.Animation:
                await Move(); 
                break;
            case MoveState.Placing:
                await Placing(); 
                break;
            case MoveState.Done:
                Done();
                break;
            default:
                Debug.LogError("Undefined MoveState");
                return; 
        }
    }

    async UniTask Prepare()
    {
        // Exit(); 
        await Transition(MoveState.Animation);
    }
    async UniTask Move()
    {
        TokenMovement tokenMovement = token.GetComponent<TokenMovement>();

        Vector3 targetWorldPos = gridManager.GetWorldPosition(targetPosition);
        Quaternion quaternion = tokenMovement.PRS.rotation;
        Vector3 scale = tokenMovement.PRS.scale;

        var taskComplete = new UniTaskCompletionSource(); 

        tokenMovement.MoveTransform(new PRS(targetWorldPos, quaternion, scale), 0.5f, false, () =>
        {
            taskComplete.TrySetResult(); 
        });

        await taskComplete.Task;
        await Transition(MoveState.Placing); 
    }
    async UniTask Placing()
    {
        tokenManager.MoveTokenTo(token, targetPosition); 
        await Transition(MoveState.Done);
    }

    bool CanMoveTo(Vector2Int pos)
    {
        if (token == null || tokenManager == null)
            return false; 

        var currentPos = tokenManager.GetGridPositionOfToken(token);

        foreach (Vector2Int delta in MoveablePositions)
        {
            Vector2Int availablePosition = currentPos + delta;
            if (availablePosition == pos && !tokenManager.IsTokenAtGridPosition(pos))
                return true;
        }

        return false;
    }

    void Done()
    {
    }
}
