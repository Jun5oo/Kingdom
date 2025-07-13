using System;
using UnityEngine;

public class ActionSystem : MonoBehaviour
{
    GridManager gridManager; 
    IAction currentAction;

    bool isExecuted; 

    [SerializeField] int actionCount = 2;

    public event Action OnActionDepleted; 

    public void Init(GridManager gridManager)
    {
        this.gridManager = gridManager;

        this.currentAction = null; 

        gridManager.OnGridCellSelected -= Execute; 
        gridManager.OnGridCellSelected += Execute;

        isExecuted = false; 
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            if(currentAction?.Performer != ActionPerformer.System)
                Exit();
        }

    }

    public void Enter(IAction action)
    {
        currentAction = action; 
        currentAction?.Enter();

        currentAction.OnActionCanceled -= OnActionCanceled;
        currentAction.OnActionCanceled += OnActionCanceled;
    }

    public void Execute(Vector2Int gridPosition)
    {
        if(currentAction != null)
        {
            currentAction.OnActionCanceled -= OnActionCanceled;

            currentAction.OnActionComplete -= OnActionComplete;
            currentAction.OnActionComplete += OnActionComplete;

            currentAction?.Execute(gridPosition);
            isExecuted = true; 
        }
    }

    public void Exit()
    {
        if(currentAction != null)
        {
            currentAction.OnActionCanceled -= OnActionCanceled; 
            currentAction.OnActionComplete -= OnActionComplete;
            currentAction?.Exit();
            currentAction = null;
            isExecuted = false; 
        }
    }

    public bool IsActionInProgress() => currentAction != null;
    public IAction GetCurrentAction() => currentAction;

    void OnActionCanceled()
    {
        Debug.Log("Invalid action target");
        Exit(); 
    }

    void OnActionComplete()
    {
        if (currentAction?.Performer != ActionPerformer.System)
            actionCount--;

        Exit();

        if (actionCount <= 0)
        {
            OnActionDepleted?.Invoke();
            actionCount = 2; 
        }
    }

    public bool IsExecuted()
    {
        return isExecuted; 
    }
}
