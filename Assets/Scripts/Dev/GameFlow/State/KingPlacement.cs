    using Cysharp.Threading.Tasks;
using System;
using UnityEngine;

public class KingPlacement : IGameState
{
    // 왕을 배치하는 State. 
    const int WAIT_TIME = 500;

    AIController aiController;

    GameFlowStateMachine stateMachine; 

    public KingPlacement(GameFlowStateMachine stateMachine)
    {
        this.stateMachine = stateMachine; 
        aiController = ServiceLocator.Get<AIController>();
    }
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

    public async UniTask OnPlacement(int playerID)
    {
        PlayerManager playerManager = ServiceLocator.Get<PlayerManager>();
        UIManager uiManager = ServiceLocator.Get<UIManager>();
        ActionSystem actionSystem = ServiceLocator.Get<ActionSystem>();
        ActionFactory actionFactory = ServiceLocator.Get<ActionFactory>();

        bool isAI = playerID != playerManager.Local.PlayerID;

        if (playerID != playerManager.Local.PlayerID)
            uiManager.OnNotification("상대 플레이어의 선택을 기다리는 중입니다.");

        else
            uiManager.OnNotification("왕을 소환할 곳을 선택해주세요."); 

        bool done = false;
        Action completeCallback = () => done = true;

        Card card = null;

        if (playerID == stateMachine.firstID)
            card = stateMachine.firstCard;
        else
            card = stateMachine.secondCard;

        if (card == null)
        {
            Debug.LogError($"KingPlacement: Card is null for player {playerID}");
            return; // 또는 throw
        }


        IAction summon = actionFactory.CreateAction(ActionType.Summon, card, ActionPerformer.System);
        summon.OnActionComplete += completeCallback;

        if (summon == null)
        {
            Debug.LogError("KingPlacement: Summon action is null");
            return;
        }

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

        await UniTask.WaitUntil(() => done).Timeout(TimeSpan.FromSeconds(10));
        Debug.Log($"{playerID}: KingPlacement Complete"); 
        summon.OnActionComplete -= completeCallback;
    }
}
