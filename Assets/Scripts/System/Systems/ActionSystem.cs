using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

/// <summary>
/// 플레이어 및 AI의 액션 실행을 담당하는 시스템.
/// 플레이어 액션은 Enter()를 통해 그리드 셀 선택을 비동기로 대기하고,
/// AI 액션은 EnterAI()를 통해 그리드 선택 없이 바로 실행한다.
/// 액션 완료 시 해당 리소스를 소모하고 상태를 정리한다.
/// </summary>
public class ActionSystem : MonoBehaviour, IGameSystem
{
    IAction currentAction; // 현재 진행 중인 액션

    GridSelection gridSelection;        // 그리드 셀 선택 대기 유틸리티
    CancellationTokenSource selectCts;  // 액션 취소를 위한 토큰 소스

    ActionResourceSystem actionResourceSystem;   // 행동 포인트 관리
    AbilityResourceSystem abilityResourceSystem; // 어빌리티 코인 관리

    /// <summary> 리소스 시스템 초기화 및 GridSelection 세팅 </summary>
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

    // 마우스 우클릭으로 플레이어 액션을 취소한다. AI 액션은 취소 불가.
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            if (currentAction?.Performer != ActionPerformer.System)
                Exit();
        }
    }

    /// <summary>
    /// AI 전용 액션 등록. 그리드 선택 없이 Execute가 외부에서 직접 호출되므로
    /// currentAction만 설정하고 GridSelection을 거치지 않는다.
    /// </summary>
    public void EnterAI(IAction action)
    {
        currentAction = action;
    }

    /// <summary>
    /// 플레이어 액션 실행. 액션 진입 → 그리드 셀 선택 대기 → Execute → 완료 처리 순으로 진행한다.
    /// 취소(우클릭 또는 OnActionCanceled)되면 OperationCanceledException을 통해 중단된다.
    /// </summary>
    public async void Enter(IAction action)
    {
        currentAction = action;
        selectCts?.Cancel();
        selectCts = new CancellationTokenSource();

        currentAction.OnActionCanceled -= OnActionCanceled;
        currentAction.OnActionCanceled += OnActionCanceled;

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

    /// <summary> 현재 액션을 취소하고 상태를 초기화한다. </summary>
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

    /// <summary>
    /// 액션 완료 처리: 리소스를 소모하고 AI 액션이면 0.5초 딜레이 후 상태를 정리한다.
    /// </summary>
    public async UniTask OnActionComplete()
    {
        IResourceSystem resourceSystem = (currentAction.ResourceType == ResourceType.Action) ? actionResourceSystem : abilityResourceSystem;
        resourceSystem.Consume(currentAction.OwnerID, currentAction.Cost);

        // AI 액션은 플레이어가 볼 수 있도록 짧은 딜레이를 추가한다.
        if (currentAction?.Performer == ActionPerformer.System)
            await Task.Delay(500);

        Exit();
    }

    /// <summary> 해당 플레이어가 이 액션을 수행할 리소스가 충분한지 확인한다. </summary>
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
