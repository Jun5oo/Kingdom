using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine;

public class MoveAction : IAction
{
    ActionType actionType;
    HighlightLayer highlightLayer;
    HighlightType highlightType;
    ActionPerformer performer;

    public ActionType ActionType { get { return actionType; } }
    public HighlightLayer HighlightLayer { get { return highlightLayer; } }
    public HighlightType HighlightType { get { return highlightType; } }
    public ActionPerformer Performer { get { return performer; } }

    GridManager gridManager;
    TokenManager tokenManager; 
    Token token;

    public List<Vector2Int> MoveablePositions { get; private set; }

    Vector2Int targetPosition; 

    public event Action OnActionComplete;
    public event Action OnActionCanceled;

    int currentCost;
    public int Cost { get { return currentCost; } }

    public ResourceType resourceType;
    public ResourceType ResourceType { get { return resourceType; } }

    public int OwnerID { get { return token.OwnerID; } }

    public BaseObject Executor => token; 

    public MoveAction(Token token, ActionPerformer performer)
    {
        // 이동액션 초기화 
        actionType = ActionType.Move;
        highlightLayer = HighlightLayer.Action;
        highlightType = HighlightType.MoveHighlight;

        this.gridManager = ServiceLocator.Get<GridManager>();
        this.tokenManager = ServiceLocator.Get<TokenManager>(); 
        this.performer = performer;

        this.token = token;
        this.MoveablePositions = token.MoveableRange;

        currentCost = 1;

        resourceType = ResourceType.Action; 
    }

    public void Enter()
    {
        Exit();

        gridManager.HighlightGridCells((Vector2Int gridPosition) =>
        {
            Vector2Int currentGridPosition = tokenManager.GetGridPositionOfToken(token); 

            foreach(Vector2Int position in MoveablePositions)
            {
                Vector2Int availablePosition = currentGridPosition + position; 
                if (availablePosition == gridPosition && !tokenManager.IsTokenAtGridPosition(gridPosition))
                    return true; 
            }

            return false; 
        }, HighlightType.MoveHighlight, HighlightLayer.Action);

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
        gridManager.UnhighlightGridCells(HighlightLayer.Action); 
    }

    public bool IsValid()
    {
        if(MoveablePositions.Count == 0) 
            return false;

        return true; 
        // return ServiceLocator.Get<ActionSystem>().GetCurrentActionCount() >= currentCost;
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
        Exit(); 
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

    void Done()
    {
        OnActionComplete?.Invoke(); 
    }
}
