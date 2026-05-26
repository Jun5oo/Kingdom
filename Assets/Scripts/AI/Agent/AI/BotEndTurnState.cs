using System.Collections;
using UnityEngine;

/// <summary> 봇 턴 종료 페이즈 상태 (미구현 스텁). </summary>
public class BotEndTurnState : IPhaseState
{
    private AgentController agent;

    public void Enter(AgentController agent)
    {
        this.agent = agent;
    }

    public void Execute()
    {
    }
    public void Exit() 
    {
    }
}