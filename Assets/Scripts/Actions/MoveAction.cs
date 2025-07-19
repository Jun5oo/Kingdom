using System;
using System.Collections.Generic;
using UnityEngine;

public class MoveAction : IAction
{
    ActionType actionType;
    HighlightLayer highlightLayer;
    HighlightType highlightType;

    public ActionType ActionType { get { return actionType; } }
    public HighlightLayer HighlightLayer { get { return highlightLayer; } }
    public HighlightType HighlightType { get { return highlightType; } }
    public ActionPerformer Performer { get { return performer; } }

    GridManager gridManager;
    TokenManager tokenManager; 
    Token token;
    ActionPerformer performer;

    List<Vector2Int> moveablePositions;

    Vector2Int targetPosition; 

    public event Action OnActionComplete;
    public event Action OnActionCanceled;

    int currentCost;
    public int Cost { get { return currentCost; } }

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
        this.moveablePositions = token.CurrentMoveRange;

        currentCost = 1;
    }

    public void Enter()
    {
        Exit();

        gridManager.HighlightGridCells((Vector2Int gridPosition) =>
        {
            Vector2Int currentGridPosition = tokenManager.GetGridPositionOfToken(token); 

            foreach(Vector2Int position in moveablePositions)
            {
                Vector2Int availablePosition = currentGridPosition + position; 
                if (availablePosition == gridPosition && !tokenManager.IsTokenAtGridPosition(gridPosition))
                    return true; 
            }

            return false; 
        }, HighlightType.MoveHighlight, HighlightLayer.Action);

    }
    public void Execute(Vector2Int gridPosition)
    {
        if(token == null)
        {
            OnActionCanceled?.Invoke();
            return; 
        }

        targetPosition = gridPosition; 
        Transition(MoveState.Prepare); 
    }
    public void Exit()
    {
        gridManager.UnhighlightGridCells(HighlightLayer.Action); 
    }

    public bool IsValid()
    {
        if(moveablePositions.Count == 0) 
            return false;

        return ServiceLocator.Get<ActionSystem>().GetCurrentActionCount() >= currentCost;
    }

    void Transition(MoveState state)
    {
        switch (state)
        {
            case MoveState.Prepare:
                Prepare();
                break;
            case MoveState.Animation:
                Move(); 
                break;
            case MoveState.Placing:
                Placing(); 
                break;
            case MoveState.Done:
                Done();
                break;
            default:
                Debug.LogError("Undefined MoveState");
                return; 
        }
    }

    void Prepare()
    {
        Exit(); 
        Transition(MoveState.Animation);
    }
    void Move()
    {
        TokenMovement tokenMovement = token.GetComponent<TokenMovement>();

        Vector3 targetWorldPos = gridManager.GetWorldPosition(targetPosition);
        Quaternion quaternion = tokenMovement.PRS.rotation;
        Vector3 scale = tokenMovement.PRS.scale;

        tokenMovement.MoveTransform(new PRS(targetWorldPos, quaternion, scale), 0.5f, false, () =>
        {
            Transition(MoveState.Placing); 
        }); 
    }
    void Placing()
    {
        tokenManager.MoveTokenTo(token, targetPosition); 
        Transition(MoveState.Done);
    }

    void Done()
    {
        OnActionComplete?.Invoke(); 
    }

}
