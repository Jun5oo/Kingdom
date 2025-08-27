using System;
using System.Threading;
using UnityEngine;

public class ActionSystem : MonoBehaviour, IGameSystem
{
    IAction currentAction;

    GridSelection gridSelection;
    HighlightResolver highlightResolver; 

    CancellationTokenSource selectCts; 

    ActionResourceSystem actionResourceSystem;
    AbilityResourceSystem abilityResourceSystem; 

    void Awake()
    {
        // 처음 게임이 시작할 때, 취소가 되서는 안됨. 
        DisableSystem(); 
    }

    public void Init()
    {
        DisableSystem();
        
        this.currentAction = null;

        actionResourceSystem = new ActionResourceSystem();
        abilityResourceSystem = new AbilityResourceSystem(); 

        actionResourceSystem.Init(); 
        abilityResourceSystem.Init();

        ServiceLocator.Register(actionResourceSystem); 
        ServiceLocator.Register(abilityResourceSystem);

        gridSelection = new GridSelection();
        gridSelection.Init();

        highlightResolver = new HighlightResolver(); 
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            Exit(); 
        }
    }

    public async void Enter(IAction action)
    {
        // 진행할 Action 세팅 
        currentAction = action;
        selectCts?.Cancel(); 
        selectCts = new CancellationTokenSource();

        currentAction.OnActionCanceled -= OnActionCanceled;
        currentAction.OnActionCanceled += OnActionCanceled;

        bool succeeded = false; 

        try
        {
            var ctx = highlightResolver.Resolve(currentAction.ActionType); 
            var pos = await gridSelection.WaitGridSelectionAsync(currentAction.Validation, ctx, selectCts.Token);

            await currentAction.Execute(pos);
            
            succeeded = true; 
        }
        catch (OperationCanceledException)
        {
            Debug.Log("Action이 취소되었습니다."); 
        }
        finally
        {
            if (succeeded)
                OnActionComplete(); 
        }

    }

    public void Exit()
    {
        if(currentAction != null)
        {
            selectCts.Cancel(); 
            currentAction.OnActionCanceled -= OnActionCanceled; 
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
        if (!enabled)
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
