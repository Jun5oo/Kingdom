using System;
using System.Collections;
using UnityEngine;

public class KingPlacement : IGameState
{
    const float WAIT_TIME = 0.5f;

    PlayerManager playerManager; 
    UIManager uiManager; 
    TokenManager tokenManager;
    ActionSystem actionSystem;
    ActionFactory actionFactory;

    GameFlowStateMachine stateMachine; 

    public KingPlacement(GameFlowStateMachine stateMachine)
    {
        this.stateMachine = stateMachine; 

        playerManager = ServiceLocator.Get<PlayerManager>();
        uiManager = ServiceLocator.Get<UIManager>();
        tokenManager = ServiceLocator.Get<TokenManager>(); 
        actionSystem = ServiceLocator.Get<ActionSystem>();
        actionFactory = ServiceLocator.Get<ActionFactory>();
    }
    public IEnumerator Enter()
    {
        yield return OnPlacement(stateMachine.firstID);
        yield return OnPlacement(stateMachine.secondID);

        if (tokenManager.TryGetKingTokenFrom(playerManager.Local.PlayerID, out Token localToken))
            uiManager.SetHUD(playerManager.Local, localToken);
        else
            Debug.LogError("로컬 플레이어의 왕이 소환되지 않았습니다.");

        if (tokenManager.TryGetKingTokenFrom(playerManager.Remote.PlayerID, out Token remoteToken))
            uiManager.SetHUD(playerManager.Remote, remoteToken);
        else
            Debug.LogError("상대 플레이어의 왕이 소환되지 않았습니다.");

        uiManager.OnActiveHUD();

        yield return new WaitForSeconds(WAIT_TIME);

        FirstDraw firstDraw = new FirstDraw(stateMachine);
        stateMachine.Enter(firstDraw); 
    }

    public IEnumerator OnPlacement(int playerID)
    {
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

        yield return new WaitUntil(() => done); 

        summon.OnActionComplete -= completeCallback;
    }
}
