using Cysharp.Threading.Tasks;
using System;
using UnityEngine;

public class KingPlacement : IGameState
{
    // 왕을 배치하는 State. 
    const int WAIT_TIME = 500;

    GameFlowStateMachine stateMachine; 

    public KingPlacement(GameFlowStateMachine stateMachine)
    {
        this.stateMachine = stateMachine; 
    }
    public async UniTask Enter()
    {
        Debug.Log("King Placement Phase"); 
        PlayerManager playerManager = ServiceLocator.Get<PlayerManager>();
        UIManager uiManager = ServiceLocator.Get<UIManager>();
        TokenManager tokenManager = ServiceLocator.Get<TokenManager>();

        await OnPlacement(stateMachine.firstID);
        await OnPlacement(stateMachine.secondID);

        if (tokenManager.TryGetKingTokenFrom(playerManager.Local.PlayerID, out Token localToken))
            uiManager.SetHUD(playerManager.Local, localToken);
        else
            Debug.LogError("로컬 플레이어의 왕이 소환되지 않았습니다.");

        if (tokenManager.TryGetKingTokenFrom(playerManager.Remote.PlayerID, out Token remoteToken))
            uiManager.SetHUD(playerManager.Remote, remoteToken);
        else
            Debug.LogError("상대 플레이어의 왕이 소환되지 않았습니다.");

        uiManager.OnActiveHUD();

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

        IAction summon = actionFactory.CreateAction(ActionType.Summon, card, ActionPerformer.System); 
        summon.OnActionComplete += completeCallback;

        actionSystem.Enter(summon);

        await UniTask.WaitUntil(() => done);
        Debug.Log($"{playerID}: KingPlacement Complete"); 
        summon.OnActionComplete -= completeCallback;
    }
}
