using System.Collections;
using UnityEngine;

/// <summary> 플레이어 턴 종료 페이즈 상태 (미구현 스텁). </summary>
public class PlayerEndTurnState : IPhaseState
{
    private AgentController agent;

    public void Enter(AgentController agent)
    {
        this.agent = agent;
        // InGameManager.Instance.AdvancePhase();
    }

    public void Execute()
    {
    }

    public void Exit() 
    {
    }
}