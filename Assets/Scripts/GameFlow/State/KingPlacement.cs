using Cysharp.Threading.Tasks;
using System;
using UnityEngine;

/// <summary>
/// 선·후공 플레이어가 순서대로 왕을 그리드에 배치하는 상태.
/// 로컬 플레이어는 ActionSystem을 통해 직접 배치하고, AI는 AIController가 자동으로 결정한다.
/// 양쪽 배치 완료 후 HUD를 표시하고 MainPhase로 전환한다.
/// </summary>
public class KingPlacement : IGameState
{
    const int WAIT_TIME = 500;

    AIController aiController;

    GameFlowStateMachine stateMachine;

    public KingPlacement(GameFlowStateMachine stateMachine)
    {
        this.stateMachine = stateMachine;
        aiController = ServiceLocator.Get<AIController>();
    }

    /// <summary> 선공→후공 순서로 왕 배치를 완료하고 HUD를 활성화한 뒤 MainPhase로 전환한다. </summary>
    public async UniTask Enter()
    {
        Debug.Log("King Placement Phase");

        await OnPlacement(stateMachine.firstID);
        await OnPlacement(stateMachine.secondID);

        HUDDisplayer hudDisplayer = ServiceLocator.Get<HUDDisplayer>();
        hudDisplayer.SetHUD();
        hudDisplayer.ActivateHUD();

        await UniTask.Delay(WAIT_TIME);

        MainPhase mainPhase = new MainPhase(stateMachine);
        stateMachine.Enter(mainPhase);
    }

    /// <summary>
    /// 한 플레이어의 왕 배치를 처리한다.
    /// AI이면 AIController.DecideKingPlacement()를 호출하고,
    /// 로컬 플레이어이면 ActionSystem.Enter()로 그리드 선택 UI를 띄운다.
    /// OnActionComplete 이벤트가 발생할 때까지 대기한다.
    /// </summary>
    public async UniTask OnPlacement(int playerID)
    {
        PlayerManager playerManager = ServiceLocator.Get<PlayerManager>();
        UIManager uiManager = ServiceLocator.Get<UIManager>();
        ActionSystem actionSystem = ServiceLocator.Get<ActionSystem>();
        ActionFactory actionFactory = ServiceLocator.Get<ActionFactory>();

        bool isAI = playerID != playerManager.Local.PlayerID;

        bool done = false;
        Action completeCallback = () => done = true;

        Card card = playerID == stateMachine.firstID ? stateMachine.firstCard : stateMachine.secondCard;

        IAction summon = actionFactory.CreateAction(ActionType.Summon, card, ActionPerformer.System);
        summon.OnActionComplete += completeCallback;

        if (isAI)
        {
            uiManager.OnNotification("상대 플레이어의 선택을 기다리는 중입니다.");
            aiController.DecideKingPlacement(summon as SummonAction);
        }
        else
        {
            uiManager.OnNotification("왕을 소환할 곳을 선택해주세요.");
            actionSystem.Enter(summon);
        }

        await UniTask.WaitUntil(() => done);
        Debug.Log($"{playerID}: KingPlacement Complete");
        summon.OnActionComplete -= completeCallback;
    }
}
