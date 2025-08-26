using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class ActionSystem : MonoBehaviour, IGameSystem
{
    IAction currentAction;

    GridSelection gridSelection;
    CancellationTokenSource selectCts;

    ActionResourceSystem actionResourceSystem;
    AbilityResourceSystem abilityResourceSystem;

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
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            if (currentAction?.Performer != ActionPerformer.System)
                Exit();
        }
    }

    public void EnterAI(IAction action)
    {
        currentAction = action;
    }

    public async void Enter(IAction action)
    {
        // 진행할 Action 세팅 
        currentAction = action;
        selectCts?.Cancel();
        selectCts = new CancellationTokenSource();

        currentAction.OnActionCanceled -= OnActionCanceled;
        currentAction.OnActionCanceled += OnActionCanceled;

        // 하이라이트 
        currentAction?.Enter();

        bool succeeded = false;

        try
        {
            var pos = await gridSelection.WaitGridSelectionAsync(currentAction.Validation, currentAction.HighlightType, currentAction.HighlightLayer, selectCts.Token);
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
                await OnActionComplete();
        }

    }

    public void Exit()
    {
        if (currentAction != null)
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

    public async UniTask OnActionComplete()
    {
        IResourceSystem resourceSystem = (currentAction.ResourceType == ResourceType.Action) ? actionResourceSystem : abilityResourceSystem;
        resourceSystem.Consume(currentAction.OwnerID, currentAction.Cost);

        if (currentAction?.Performer == ActionPerformer.System)
        {
            await Task.Delay(500);
        }
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
