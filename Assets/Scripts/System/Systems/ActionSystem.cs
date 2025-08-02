using System;
using UnityEngine;

public class ActionSystem : MonoBehaviour, IGameSystem
{
    IAction currentAction;

    [SerializeField] int actionCount = 2; 

    public void Init()
    {
        DisableSystem();
        
        this.currentAction = null;

        GridManager gridManager = ServiceLocator.Get<GridManager>();

        gridManager.OnGridCellSelected -= Execute; 
        gridManager.OnGridCellSelected += Execute;
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
        if (currentAction?.Performer == ActionPerformer.System)
        {
            Exit();
            return; 
        }

        actionCount -= currentAction.Cost; 
        Exit();
    }
    public int GetCurrentActionCount() => actionCount; 
    public void ResetActionCount() => actionCount = 2; 
    public void EnableSystem() => enabled = true; 
    public void DisableSystem() => enabled = false;
}
