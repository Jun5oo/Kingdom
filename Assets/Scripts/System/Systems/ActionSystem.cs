using System;
using UnityEngine;

public class ActionSystem : MonoBehaviour, IGameSystem
{
    IAction currentAction;

    ActionResourceSystem actionResourceSystem;
    AbilityResourceSystem abilityResourceSystem; 

    public void Init()
    {
        DisableSystem();
        
        this.currentAction = null;

        GridManager gridManager = ServiceLocator.Get<GridManager>();

        gridManager.OnGridCellSelected -= Execute; 
        gridManager.OnGridCellSelected += Execute;

        actionResourceSystem = new ActionResourceSystem();
        abilityResourceSystem = new AbilityResourceSystem(); 

        actionResourceSystem.Init(); 
        abilityResourceSystem.Init();

        ServiceLocator.Register(actionResourceSystem); 
        ServiceLocator.Register(abilityResourceSystem);
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
        Debug.Log("Action Canceled");
        Exit(); 
    }
    void OnActionComplete()
    {
        if (currentAction?.Performer == ActionPerformer.System)
        {
            Exit();
            return; 
        }

        IResourceSystem resourceSystem = (currentAction.ResourceType == ResourceType.Action) ? actionResourceSystem : abilityResourceSystem;
        resourceSystem.Consume(currentAction.OwnerID, currentAction.Cost); 

        Exit();
    }

    public bool CanPerformAction(IAction action, int playerID)
    {
        IResourceSystem resourceSystem = (action.ResourceType == ResourceType.Action) ? actionResourceSystem : abilityResourceSystem;
        return resourceSystem.IsEnoughResources(playerID, action.Cost); 
    }

    public int GetCurrentActionCount(int playerID) => actionResourceSystem.GetCurrentResources(playerID);
    public int GetCurrentAbilityCount(int playerID) => abilityResourceSystem.GetCurrentResources(playerID);
    public void ResetActionCount(int playerID) => actionResourceSystem.ResetResources(playerID); 
   
    public void EnableSystem() => enabled = true; 
    public void DisableSystem() => enabled = false;
}
